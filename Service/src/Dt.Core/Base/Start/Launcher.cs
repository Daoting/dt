#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Extensions.ElapsedTime;
using Serilog.Formatting.Compact;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 启动微服务
    /// </summary>
    public static partial class Launcher
    {
        /// <summary>
        /// 初始化服务，任一环节失败即启动失败
        /// + 确定基础路径
        /// + 创建日志对象
        /// + 读取配置
        /// + 整理服务存根
        /// + 启动http服务器
        /// 此方法不可异步，否则启动有问题！！！
        /// </summary>
        /// <param name="p_args">命令行参数，基础路径，空时dll所在路径为基础路径</param>
        public static void Run(string[] p_args)
        {
            SetPathBase(p_args);
            CreateLogger();
            LoadConfig();
            BuildStubs();
            RunWebHost();
            Log.CloseAndFlush();
        }

        /// <summary>
        /// 确定基础路径
        /// </summary>
        /// <param name="p_args"></param>
        /// <exception cref="Exception"></exception>
        static void SetPathBase(string[] p_args)
        {
            if (p_args != null && p_args.Length > 0)
            {
                var path = p_args[0];
                if (!Path.IsPathRooted(path))
                {
                    // 相对路径
                    path = Path.Combine(AppContext.BaseDirectory, path);
                }
                if (!Directory.Exists(path))
                {
                    string msg = $"基础路径 {path} 不存在";
                    Console.WriteLine(msg);
                    throw new Exception(msg);
                }
                Kit.PathBase = path;
            }
            else
            {
                // dll所在路径为基础路径
                Kit.PathBase = AppContext.BaseDirectory;
            }
        }

        /// <summary>
        /// 创建日志对象
        /// </summary>
        static void CreateLogger()
        {
            try
            {
                // 支持动态调整
                var cfg = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("logger.json", false, true)
                    .Build();

                // 日志文件命名：
                // log-服务实例ID-日期.txt，避免部署在k8s挂载宿主目录时文件名重复
                string path = Path.Combine(Kit.PathBase, "etc/log", $"log-{Kit.SvcID}-.txt");
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(cfg)
                    .WriteTo.Html()
                    // 输出json文件，默认最大1G、最多保存31个文件、实时写、文件独占方式
                    .WriteTo.File(
                        new CompactJsonFormatter(),
                        path,
                        rollingInterval: RollingInterval.Day, // 文件名末尾加日期
                        rollOnFileSizeLimit: true) // 超过1G时新文件名末尾加序号
                    .WriteTo.Console()
                    .WithElapsed<Kit>()
                    .CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine($"创建日志对象失败：{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        static void LoadConfig()
        {
            try
            {
                var cfg = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("service.json", false, true)
                    .AddJsonFile("global.json", false, true)
                    .Build();
                Kit.InitConfig(cfg, true);
                Log.Information("读取配置成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取配置失败！");
                throw;
            }
        }

        /// <summary>
        /// 整理服务存根
        /// </summary>
        static void BuildStubs()
        {
            // 服务名
            var svcName = Kit.GetCfg<string>("SvcName", "").ToLower();

            List<Stub> stubs = new List<Stub>();
            if (svcName == "cm")
            {
                stubs.Add(CreateSysStub("Dt.Cm"));
            }
            else if (svcName == "msg")
            {
                stubs.Add(CreateSysStub("Dt.Msg"));
            }
            else if (svcName == "fsm")
            {
                stubs.Add(CreateSysStub("Dt.Fsm"));
            }
            else if (svcName == "app")
            {
                stubs.Add(CreateSysStub("Dt.App"));
            }
            else
            {
                var ls = GetAllStubs();
                if (svcName != "")
                {
                    // 自定义微服务
                    var stub = ls.FirstOrDefault(s => svcName.Equals(s.SvcName, StringComparison.OrdinalIgnoreCase));
                    if (stub == null)
                    {
                        var ex = new Exception($"服务 [{svcName}] 的 Stub 不存在！");
                        Log.Fatal(ex, "加载服务存根出错");
                        throw ex;
                    }
                    stubs.Add(stub);
                }
                else
                {
                    // 单体服务模式，所有微服务聚集成一个服务
                    stubs.AddRange(ls);
                    stubs.Add(CreateSysStub("Dt.Cm"));
                    stubs.Add(CreateSysStub("Dt.Msg"));
                    stubs.Add(CreateSysStub("Dt.Fsm"));
                    stubs.Add(CreateSysStub("Dt.App"));
                }
            }
            Kit.Stubs = stubs.ToArray();

            if (Kit.Stubs.Length > 1)
            {
                // 自定义服务的数据源键名
                var sect = Kit.Config.GetSection("CustomSvcDbKey");
                Dictionary<string, DbAccessInfo> svcDbs = null;
                foreach (var svc in sect.GetChildren())
                {
                    if (Kit.AllDbInfo.TryGetValue(svc.Value, out var di))
                    {
                        if (svcDbs == null)
                            svcDbs = new Dictionary<string, DbAccessInfo>(StringComparer.OrdinalIgnoreCase);
                        svcDbs[svc.Key] = di;

                        // 添加未包含的服务
                        var exist = (from s in stubs
                                     where s.SvcName.Equals(svc.Key, StringComparison.OrdinalIgnoreCase)
                                     select s).Any();
                        if (!exist)
                        {
                            var stub = new DefaultStub();
                            stub.SvcName = svc.Key;
                            stubs.Add(stub);
                        }
                    }
                    else
                    {
                        LogException($"数据源键名[{svc.Value}]在global.json无配置！");
                    }
                }

                // 多数据库
                if (svcDbs != null)
                {
                    foreach (var stub in Kit.Stubs)
                    {
                        if (!svcDbs.ContainsKey(stub.SvcName))
                            svcDbs[stub.SvcName] = Kit.DefaultDbInfo;
                    }
                    Kit.SingletonSvcDbs = svcDbs;
                }
            }

            // 单体不启用 RabbitMQ
            Kit.EnableRabbitMQ = Kit.Stubs.Length == 1;
            Log.Information($"启动 {Kit.AllSvcName}，版本：{typeof(Launcher).Assembly.GetName().Version.ToString(3)}");
        }

        /// <summary>
        /// 启动Web服务器
        /// </summary>
        static void RunWebHost()
        {
            try
            {
                // 部署在IIS进程内模式时创建 IISHttpServer
                // 其他情况创建 KestrelServer
                // 两种 Web服务器的配置在Startup.ConfigureServices
                Host.CreateDefaultBuilder()
                    // 为WebHost配置默认设置
                    .ConfigureWebHostDefaults(web => web.UseStartup<Startup>())
                    // 改用Autofac来实现依赖注入
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    // 内部注入AddSingleton<ILoggerFactory>(new SerilogLoggerFactory())
                    .UseSerilog()
                    // 实例化WebHost并初始化，调用Startup.ConfigureServices和Configure
                    .Build()
                    // 内部调用WebHost.StartAsync()
                    .Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Web服务器启动失败");
                throw;
            }
        }

        static void RunInitModeWebHost()
        {
            try
            {
                Log.Information("初始化数据库模式");

                // 部署在IIS进程内模式时创建 IISHttpServer
                // 其他情况创建 KestrelServer
                // 两种 Web服务器的配置在Startup.ConfigureServices
                Host.CreateDefaultBuilder()
                    // 为WebHost配置默认设置
                    .ConfigureWebHostDefaults(web => web.UseStartup<InitModeStartup>())
                    // 内部注入AddSingleton<ILoggerFactory>(new SerilogLoggerFactory())
                    .UseSerilog()
                    // 实例化WebHost并初始化，调用Startup.ConfigureServices和Configure
                    .Build()
                    // 内部调用WebHost.StartAsync()
                    .Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Web服务器启动失败");
                throw;
            }
        }

        static void LogException(string p_msg)
        {
            Log.Fatal(p_msg);
            throw new Exception(p_msg);
        }

        static void NoDllException(string p_dll)
        {
            LogException($"缺少[{p_dll}]文件，cm msg fsm内置微服务不支持单体模式！");
        }

        static Stub CreateSysStub(string p_name)
        {
            Type tp = Type.GetType($"{p_name}.SvcStub,{p_name}");
            if (tp == null)
                NoDllException($"{p_name}.dll");
            return (Stub)Activator.CreateInstance(tp);
        }

        /// <summary>
        /// 从所有dll中找出自定义微服务的Stub列表
        /// </summary>
        /// <returns></returns>
        static List<Stub> GetAllStubs()
        {
            List<Stub> ls = new List<Stub>();
            DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);
            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                if (fi.Name.Equals("Dt.Core.dll")
                    || fi.Name.Equals("Dt.Cm.dll")
                    || fi.Name.Equals("Dt.Fsm.dll")
                    || fi.Name.Equals("Dt.Msg.dll")
                    || fi.Name.Equals("Dt.App.dll"))
                    continue;

                using var stream = fi.OpenRead();
                using var peReader = new PEReader(stream);
                {
                    var meta = peReader.GetMetadataReader();
                    foreach (var ar in meta.AssemblyReferences)
                    {
                        var arf = meta.GetAssemblyReference(ar);
                        var rn = meta.GetString(arf.Name);
                        if (rn == "Dt.Core")
                        {
                            // 遍历所有公共类型
                            foreach (var typeHandle in meta.TypeDefinitions)
                            {
                                var type = meta.GetTypeDefinition(typeHandle);
                                if (type.BaseType.Kind == HandleKind.TypeReference)
                                {
                                    var bt = meta.GetTypeReference((TypeReferenceHandle)type.BaseType);
                                    if (meta.GetString(bt.Name) == "Stub" && meta.GetString(bt.Namespace) == "Dt.Core")
                                    {
                                        var asm = Assembly.LoadFrom(fi.FullName);
                                        var ns = meta.GetString(type.Namespace);
                                        var nn = meta.GetString(type.Name);
                                        var st = asm.GetType(ns + "." + nn);
                                        if (st != null)
                                            ls.Add((Stub)Activator.CreateInstance(st));
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return ls;
        }
    }
}
