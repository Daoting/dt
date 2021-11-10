#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.IO;
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
        /// <param name="p_stub">服务存根</param>
        /// <param name="p_args">启动参数</param>
        public static void Run(ISvcStub p_stub, string[] p_args)
        {
            Kit.Stub = p_stub ?? throw new ArgumentNullException(nameof(p_stub));
            CreateLogger();
            LoadConfig();
            DbSchema.Init();
            Silo.CacheSql();
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
                // k8s：服务名 -服务实例ID-日期.txt，避免部署在k8s挂载宿主目录时文件名重复
                // windows：服务名-日期.txt
                string fileName = Kit.IsInDocker ? $"{Kit.SvcName}-{Kit.SvcID}-.txt" : $"{Kit.SvcName}-.txt";
                string path = Path.Combine(AppContext.BaseDirectory, "etc/log", fileName);
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(cfg)
                    //.WriteTo.Console()
                    .WriteTo.Html()
                    // 输出json文件，默认最大1G、最多保存31个文件、实时写、文件独占方式
                    .WriteTo.File(
                        new CompactJsonFormatter(),
                        path,
                        rollingInterval: RollingInterval.Day, // 文件名末尾加日期
                        rollOnFileSizeLimit: true) // 超过1G时新文件名末尾加序号
                    .CreateLogger();

                var version = Kit.Stub.GetType().Assembly.GetName().Version;
                Log.Information($"启动{Kit.SvcName}(V{version.Major}.{version.Minor}.{version.Build})...");
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
                WebHost.CreateDefaultBuilder<Startup>(p_args)
                    // 内部注入AddSingleton<ILoggerFactory>(new SerilogLoggerFactory())
                    .UseSerilog()
                    // 实例化WebHost并初始化，调用Startup.ConfigureServices和Configure
                    .Build()
                    // 内部调用WebHost.StartAsync()
                    .Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "启动 {0} 失败", Kit.SvcName);
                throw;
            }
        }
    }
}
