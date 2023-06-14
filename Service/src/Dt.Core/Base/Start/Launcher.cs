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
using Serilog.Formatting.Compact;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 启动微服务
    /// </summary>
    public static class Launcher
    {
        /// <summary>
        /// 初始化服务，任一环节失败即启动失败
        /// + 创建日志对象
        /// + 读取配置
        /// + 缓存默认库表结构、同步时间
        /// + 缓存Sql语句
        /// + 启动http服务器
        /// 此方法不可异步，否则启动有问题！！！
        /// </summary>
        /// <param name="p_args">启动参数</param>
        /// <param name="p_stub">服务存根</param>
        public static void Run(string[] p_args, Stub p_stub)
        {
            CreateLogger();
            LoadConfig();
            BuildStubs(p_stub);
            DbSchema.Init();
            DataAccess.CacheSql();
            RunWebHost(p_args);
            Log.CloseAndFlush();
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
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "etc/config"))
                    .AddJsonFile("logger.json", false, true)
                    .Build();

                // 日志文件命名：
                // log-服务实例ID-日期.txt，避免部署在k8s挂载宿主目录时文件名重复
                string path = Path.Combine(AppContext.BaseDirectory, "etc/log", $"log-{Kit.SvcID}-.txt");
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
                Kit.Config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "etc/config"))
                    .AddJsonFile("service.json", false, true)
                    .AddJsonFile("global.json", false, true)
                    .Build();
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
        /// <param name="p_stub">服务存根</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        static void BuildStubs(Stub p_stub)
        {
            if (p_stub == null)
                p_stub = new DefaultStub();

            // 服务名
            if (string.IsNullOrEmpty(p_stub.SvcName))
            {
                var name = Kit.GetCfg<string>("SvcName");
                if (string.IsNullOrEmpty(name))
                {
                    LogException("未设置服务名称，启动失败！");
                }

                p_stub.SvcName = name.ToLower();
            }

            // 配置中允许单体服务 且 服务Stub中也允许单体服务时，才单体服务
            bool isSingletonSvc = Kit.GetCfg("IsSingletonSvc", false);
            if (isSingletonSvc && p_stub.AllowSingleton)
            {
                List<Stub> stubs = new List<Stub> { p_stub };

                // 公共服务
                Type tp = Type.GetType("Dt.Cm.SvcStub,Dt.Cm");
                if (tp == null)
                    LogException("缺少Dt.Cm.dll文件");
                stubs.Add((Stub)Activator.CreateInstance(tp));

                tp = Type.GetType("Dt.Msg.SvcStub,Dt.Msg");
                if (tp == null)
                    LogException("缺少Dt.Msg.dll文件");
                stubs.Add((Stub)Activator.CreateInstance(tp));

                tp = Type.GetType("Dt.Fsm.SvcStub,Dt.Fsm");
                if (tp == null)
                    LogException("缺少Dt.Fsm.dll文件");
                stubs.Add((Stub)Activator.CreateInstance(tp));

                // 自定义服务的数据源键名
                var sect = Kit.Config.GetSection("CustomSvcDbKey");
                Dictionary<string, DbInfo> svcDbs = null;
                foreach (var svc in sect.GetChildren())
                {
                    if (Kit.AllDbInfo.TryGetValue(svc.Value, out var di))
                    {
                        if (svcDbs == null)
                            svcDbs = new Dictionary<string, DbInfo>(StringComparer.OrdinalIgnoreCase);
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
                    if (!svcDbs.ContainsKey(p_stub.SvcName))
                        svcDbs[p_stub.SvcName] = Kit.DefaultDbInfo;
                    if (!svcDbs.ContainsKey("cm"))
                        svcDbs["cm"] = Kit.DefaultDbInfo;
                    if (!svcDbs.ContainsKey("msg"))
                        svcDbs["msg"] = Kit.DefaultDbInfo;
                    if (!svcDbs.ContainsKey("fsm"))
                        svcDbs["fsm"] = Kit.DefaultDbInfo;

                    Kit.SingletonSvcDbs = svcDbs;
                }

                Kit.Stubs = stubs.ToArray();
            }
            else
            {
                Kit.Stubs = new Stub[] { p_stub };
            }

            Kit.EnableRabbitMQ = !isSingletonSvc;
            Log.Information($"启动 {string.Join('+', Kit.SvcNames)} ...");
        }

        /// <summary>
        /// 启动Web服务器
        /// </summary>
        /// <param name="p_args">启动参数</param>
        static void RunWebHost(string[] p_args)
        {
            try
            {
                // 部署在IIS进程内模式时创建 IISHttpServer
                // 其他情况创建 KestrelServer
                // 两种 Web服务器的配置在Startup.ConfigureServices
                Host.CreateDefaultBuilder(p_args)
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

        /// <summary>
        /// 启动boot服务
        /// </summary>
        /// <param name="p_args"></param>
        /// <param name="p_stub"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void StartBoot(string[] p_args, Stub p_stub)
        {
            if (p_stub == null)
                throw new ArgumentException(nameof(p_stub));
            Kit.Stubs = new Stub[] { p_stub };
            Kit.EnableRabbitMQ = false;

            CreateLogger();

            try
            {
                var cfg = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "etc/config"))
                    .AddJsonFile("service.json", false, true)
                    .Build();
                Kit.SetBootConfig(cfg);
                Log.Information("读取配置成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取配置失败！");
                throw;
            }

            // 无db连接，无sql

            RunWebHost(p_args);
            Log.CloseAndFlush();
        }

        static void LogException(string p_msg)
        {
            Log.Fatal(p_msg);
            throw new Exception(p_msg);
        }
    }
}
