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
        /// <param name="p_isSingletonSvc">是否为单体服务</param>
        public static void Run(string[] p_args, Stub p_stub, bool p_isSingletonSvc)
        {
            BuildStubs(p_stub, p_isSingletonSvc);
            Kit.EnableRabbitMQ = !p_isSingletonSvc;
            CreateLogger();
            LoadConfig();
            DbSchema.Init();
            Silo.CacheSql();
            RunWebHost(p_args);
            Log.CloseAndFlush();
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
                Kit.Config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "etc/config"))
                    .AddJsonFile("service.json", false, true)
                    .Build();
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

        /// <summary>
        /// 整理服务存根
        /// </summary>
        /// <param name="p_stub">服务存根</param>
        /// <param name="p_isSingletonSvc">是否为单体服务</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        static void BuildStubs(Stub p_stub, bool p_isSingletonSvc)
        {
            if (!p_isSingletonSvc && p_stub == null)
                throw new ArgumentException(nameof(p_stub));

            if (p_isSingletonSvc)
            {
                List<Stub> stubs = new List<Stub>();
                if (p_stub != null)
                    stubs.Add(p_stub);

                Type tp = Type.GetType("Dt.Cm.SvcStub,Dt.Cm");
                if (tp == null)
                    throw new Exception("缺少Dt.Cm.dll文件");
                stubs.Add((Stub)Activator.CreateInstance(tp));

                tp = Type.GetType("Dt.Msg.SvcStub,Dt.Msg");
                if (tp == null)
                    throw new Exception("缺少Dt.Msg.dll文件");
                stubs.Add((Stub)Activator.CreateInstance(tp));

                tp = Type.GetType("Dt.Fsm.SvcStub,Dt.Fsm");
                if (tp == null)
                    throw new Exception("缺少Dt.Fsm.dll文件");
                stubs.Add((Stub)Activator.CreateInstance(tp));

                Kit.Stubs = stubs.ToArray();
            }
            else
            {
                Kit.Stubs = new Stub[] { p_stub };
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
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "etc/config"))
                    .AddJsonFile("logger.json", false, true)
                    .Build();

                // 日志文件命名：
                // 服务名-服务实例ID-日期.txt，避免部署在k8s挂载宿主目录时文件名重复
                string svc = Kit.Stubs.Length == 1 ? Kit.Stubs[0].SvcName : "app";
                string path = Path.Combine(AppContext.BaseDirectory, "etc/log", $"{svc}-{Kit.SvcID}-.txt");
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(cfg)
                    .WriteTo.Html()
                    // 输出json文件，默认最大1G、最多保存31个文件、实时写、文件独占方式
                    .WriteTo.File(
                        new CompactJsonFormatter(),
                        path,
                        rollingInterval: RollingInterval.Day, // 文件名末尾加日期
                        rollOnFileSizeLimit: true) // 超过1G时新文件名末尾加序号
                    .CreateLogger();

                Log.Information($"启动 {string.Join('+', Kit.SvcNames)} ...");
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
    }
}
