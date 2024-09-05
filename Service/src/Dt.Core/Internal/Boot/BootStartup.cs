#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Core
{
    public class BootStartup
    {
        /// <summary>
        /// 定义全局服务
        /// </summary>
        /// <param name="p_services"></param>
        /// <returns></returns>
        public void ConfigureServices(IServiceCollection p_services)
        {
            // 配置 KestrelServer
            p_services.Configure<KestrelServerOptions>(options =>
            {
                // KestrelServer 监听设置，配置在 service.json 的 KestrelListen 节
                Startup.ConfigureKestrelListen(options);

                // 不限制请求/响应的速率，不适合流模式长时间等待的情况！
                options.Limits.MinRequestBodyDataRate = null;
                options.Limits.MinResponseDataRate = null;

                Log.Information("启动 KestrelServer 成功");
            });

            // 配置 IISHttpServer
            p_services.Configure<IISServerOptions>(options => Log.Information("启动 IISHttpServer 成功"));

            Kit.ConfigureServices(p_services);
        }

        /// <summary>
        /// 定义请求管道的中间件
        /// </summary>
        /// <param name="p_app"></param>
        public void Configure(IApplicationBuilder p_app)
        {
            // 中间件
            p_app.UseMiddleware<BootMiddleware>();
            Kit.Configure(p_app);

            // 末尾中间件，显示自定义404页面
            p_app.UseMiddleware<EndMiddleware>();
            Log.Information("---启动完毕---");
        }
    }
}
