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
            if ("InitDb".Equals(Kit.GetCfg<string>("SvcName"), StringComparison.OrdinalIgnoreCase))
            {
                // 进入初始化数据库模式
                RunInitModeWebHost();
            }
            else
            {
                LoadSvcs();
                RunWebHost();
            }
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
                Kit.InitConfig(cfg);
                Log.Information("读取配置成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取配置失败！");
                throw;
            }
        }

        /// <summary>
        /// 加载服务
        /// </summary>
        static void LoadSvcs()
        {
            // 服务名
            var svcName = Kit.GetCfg<string>("SvcName", "").Trim().ToLower();

            var svcs = new SvcList();
            if (_sysSvcs.Contains(svcName))
            {
                // 系统内置服务
                svcs.Add(GetSysSvc(svcName));
            }
            else
            {
                // 所有外部定义的微服务
                var ls = GetAllSvcs();
                
                if (svcName != "")
                {
                    // 外部微服务
                    if (!ls.TryGetValue(svcName, out var svc))
                    {
                        LogException($"服务 {svcName} 不存在！");
                    }
                    svcs.Add(svc);
                }
                else
                {
                    // 单体服务模式，所有微服务聚集成一个服务
                    foreach (var svc in ls)
                    {
                        svcs.Add(svc);
                    }
                    foreach (var name in _sysSvcs)
                    {
                        svcs.Add(GetSysSvc(name));
                    }
                }
            }
            Kit.Svcs = svcs;

            // 完善每个微服务的数据源jian
            foreach (var svc in Kit.Svcs)
            {
                string file = svc.SvcName + ".json";
                string path = Path.Combine(Kit.PathBase, "etc/config", file);
                
                if (!File.Exists(path))
                {
                    // 无配置文件，使用默认数据源
                    svc.DbInfo = Kit.DefaultDbInfo;
                    continue;
                }

                string dbKey = null;
                try
                {
                    var cfg = new ConfigurationBuilder()
                        .AddJsonFile(path, false, false)
                        .Build();
                    dbKey = cfg["DbKey"];
                }
                catch
                {
                    svc.DbInfo = Kit.DefaultDbInfo;
                    continue;
                }

                if (string.IsNullOrEmpty(dbKey))
                {
                    // 无 DbKey 配置，使用默认数据源
                    svc.DbInfo = Kit.DefaultDbInfo;
                }
                else if (Kit.AllDbInfo.TryGetValue(dbKey, out var di))
                {
                    svc.DbInfo = di;
                }
                else
                {
                    svc.DbInfo = Kit.DefaultDbInfo;
                    Log.Warning($"{file} 中的数据源键名 {dbKey} 无连接串配置，使用默认数据源！");
                }
            }

            // 单体不启用 RabbitMQ
            Kit.EnableRabbitMQ = Kit.Svcs.Count == 1;
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
        
        static SvcInfo GetSysSvc(string p_name)
        {
            var path = Path.Combine(AppContext.BaseDirectory, $"Dt.{p_name}.dll");
            var asm = Assembly.LoadFrom(path);
            var svc = asm.GetCustomAttribute<SvcStubAttribute>();
            
            if (svc == null)
                LogException($"Dt.{p_name}.dll 缺少 SvcStubAttribute 标签！");
            
            return new SvcInfo(svc.SvcName, (Stub)Activator.CreateInstance(svc.StubType));
        }

        /// <summary>
        /// 从所有dll中找出自定义的微服务列表
        /// </summary>
        /// <returns></returns>
        static SvcList GetAllSvcs()
        {
            var ls = new SvcList();
            DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);
            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                bool skip = false;
                foreach (var filter in _dllFilter)
                {
                    if (fi.Name.StartsWith(filter))
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip)
                    continue;

                try
                {
                    using var stream = fi.OpenRead();
                    using var peReader = new PEReader(stream);
                    {
                        var meta = peReader.GetMetadataReader();
                        var refs = meta.AssemblyReferences;
                        foreach (var ar in refs)
                        {
                            var arf = meta.GetAssemblyReference(ar);
                            var rn = meta.GetString(arf.Name);

                            // 只处理引用 Dt.Core.dll 的程序集
                            if (rn == "Dt.Core")
                            {
                                // 遍历程序集标签
                                foreach (var customAttr in meta.CustomAttributes)
                                {
                                    var attr = meta.GetCustomAttribute(customAttr);
                                    try
                                    {
                                        // 捕获类型转换的异常
                                        var constructor = meta.GetMemberReference((MemberReferenceHandle)attr.Constructor);
                                        var attrType = meta.GetTypeReference((TypeReferenceHandle)constructor.Parent);
                                        var tpName = meta.GetString(attrType.Name);
                                        if (tpName == "SvcStubAttribute")
                                        {
                                            var asm = Assembly.LoadFrom(fi.FullName);
                                            var svc = asm.GetCustomAttribute<SvcStubAttribute>();
                                            ls.Add(new SvcInfo(svc.SvcName, (Stub)Activator.CreateInstance(svc.StubType)));
                                            break;
                                        }
                                    }
                                    catch { }
                                }
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, $"{fi.Name} 提取Stub时出错！");
                    throw;
                }
            }
            return ls;
        }

        static List<string> _dllFilter = new List<string> { "Microsoft.", "System.", "Autofac.", "Castle.", "Dapper.", "Dt.Core.", "Dt.Da.", "Dt.Cm.", "Dt.Fsm.", "Dt.Msg.", "MailKit.", "MimeKit.", "MySqlConnector.", "Nito.", "Npgsql.", "Oracle.", "Polly.", "RabbitMQ.", "Serilog.", "SQLitePCLRaw.", "StackExchange.", "BouncyCastle.", "Pipelines." };
        static List<string> _sysSvcs = new List<string> { "cm", "fsm", "msg", "da" };
    }
}
