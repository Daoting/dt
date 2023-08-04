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
    public static partial class Launcher
    {
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

            try
            {
                // 部署在IIS进程内模式时创建 IISHttpServer
                // 其他情况创建 KestrelServer
                // 两种 Web服务器的配置在Startup.ConfigureServices
                Host.CreateDefaultBuilder(p_args)
                    // 为WebHost配置默认设置
                    .ConfigureWebHostDefaults(web => web.UseStartup<BootStartup>())
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

            Log.CloseAndFlush();
        }
    }
}
