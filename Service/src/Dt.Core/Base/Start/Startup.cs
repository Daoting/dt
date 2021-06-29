#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
#endregion

namespace Dt.Core
{
    public class Startup
    {
        /// <summary>
        /// 定义全局服务
        /// </summary>
        /// <param name="p_services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection p_services)
        {
            // 配置 KestrelServer
            p_services.Configure<KestrelServerOptions>(options =>
            {
                // 未使用Listen方法，因无法应用外部设置的端口！
                options.ConfigureEndpointDefaults(listenOptions =>
                {
                    // 浏览器要求http2.0协议必须采用https通信，http2.0协议和https之间本来没有依赖关系！
                    // 默认http2
                    listenOptions.Protocols = HttpProtocols.Http2;

                    // 为Kestrel加载X509证书，证书名称tls.pfx，位置在根目录，默认证书为localhost，生成环境可替换
                    var tls = new FileInfo(Path.Combine(AppContext.BaseDirectory, "tls.pfx"));
                    if (tls.Exists)
                    {
                        using (var stream = tls.OpenRead())
                        {
                            byte[] pfx = new byte[tls.Length];
                            stream.Read(pfx, 0, (int)stream.Length);
                            listenOptions.UseHttps(new X509Certificate2(pfx, "dt"));
                        }
                    }
                    else
                    {
                        Log.Error("Kestrel缺少X509证书文件tls.pfx");
                    }
                });

                // 不限制请求/响应的速率，不适合流模式长时间等待的情况！
                options.Limits.MinRequestBodyDataRate = null;
                options.Limits.MinResponseDataRate = null;

                long maxSize = Kit.GetCfg<long>("MaxRequestBodySize", 0);
                if (maxSize > 0)
                {
                    // 设置post的body的最大长度，默认28.6M
                    options.Limits.MaxRequestBodySize = maxSize;
                }
                
                Log.Information("启动 KestrelServer 成功");
            });

            // 配置 IISHttpServer
            p_services.Configure<IISServerOptions>(options =>
            {
                long maxSize = Kit.GetCfg<long>("MaxRequestBodySize", 0);
                if (maxSize > 0)
                {
                    // 设置post的body的最大长度，默认28.6M
                    // web.config 和 service.json都需设置
                    options.MaxRequestBodySize = maxSize;
                }

                Log.Information("启动 IISHttpServer 成功");
            });

            Kit.ConfigureServices(p_services);
            return Silo.ConfigureServices(p_services);
        }

        /// <summary>
        /// 定义请求管道的中间件
        /// </summary>
        /// <param name="p_app"></param>
        public void Configure(IApplicationBuilder p_app)
        {
            // 添加中间件，注意先后顺序！
            // 异常处理中间件放在管道的最前端，内部try { await _next(context); }捕获异常时重定向到 /.error
            p_app.UseExceptionHandler("/.error");

            // 内置中间件
            p_app.UseMiddleware<DtMiddleware>();

            // 外部中间件
            Kit.Configure(p_app);

            // 默认页和静态页面改为在外部启用！
            //p_app.UseDefaultFiles();
            //p_app.UseStaticFiles();

            // 末尾中间件，显示自定义404页面
            p_app.UseMiddleware<EndMiddleware>();

            // 订阅事件
            RemoteEventBus.Subscribe(p_app.ApplicationServices);

            var version = Kit.Stub.GetType().Assembly.GetName().Version;
            Log.Information("---启动完毕---");
        }
    }
}
