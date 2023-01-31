#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Autofac;
using Dt.Core.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
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
        public void ConfigureServices(IServiceCollection p_services)
        {
            // 配置 KestrelServer
            p_services.Configure<KestrelServerOptions>(options =>
            {
                // KestrelServer 监听设置，配置在 service.json 的 KestrelListen 节
                ConfigureKestrelListen(options);

                // 不限制请求/响应的速率，不适合流模式长时间等待的情况！
                options.Limits.MinRequestBodyDataRate = null;
                options.Limits.MinResponseDataRate = null;

                long maxSize = Kit.GetCfg<long>("MaxRequestBodySize", 0);
                if (maxSize > 0)
                {
                    // 设置post的body的最大长度，默认28.6M
                    options.Limits.MaxRequestBodySize = maxSize;
                    Log.Information("请求内容的最大长度 " + Kit.GetFileSizeDesc((ulong)maxSize));
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
                Log.Information("请求内容的最大长度 " + Kit.GetFileSizeDesc((ulong)maxSize));
            });

            // 添加MySql数据库访问，可根据配置切换成其它库类型
            p_services.AddTransient<IEntityAccess, MySqlAccess>();

            Kit.ConfigureServices(p_services);
        }

        /// <summary>
        /// ConfigureServices之后调用
        /// </summary>
        /// <param name="p_builder"></param>
        public void ConfigureContainer(ContainerBuilder p_builder)
        {
            Silo.ConfigureContainer(p_builder);
        }

        /// <summary>
        /// KestrelServer 监听设置
        /// </summary>
        /// <param name="p_options"></param>
        void ConfigureKestrelListen(KestrelServerOptions p_options)
        {
            var sect = Kit.Config.GetSection("KestrelListen").GetChildren();

            // service.json 中无配置
            if (!sect.Any())
            {
                // 使用 launchSettings.json 中配置，
                // 都无配置使用缺省：http://localhost:5000; https://localhost:5001
                Log.Information("service.json无监听配置，使用默认");
                return;
            }

            // 根据 service.json 的 KestrelListen 节配置设置监听
            // 可以监听多个Url，每个监听的Url配置一次
            foreach (var item in sect)
            {
                // 使用协议：http https
                string scheme = item.GetValue<string>("Scheme");
                string address = item.GetValue<string>("Address");
                int port = item.GetValue<int>("Port");

                if ("https".Equals(scheme, StringComparison.OrdinalIgnoreCase))
                {
                    // https协议
                    p_options.Listen(IPAddress.Parse(address), port, listenOptions =>
                    {
                        // 浏览器要求http2.0协议必须采用https通信，http2.0协议和https之间本来没有依赖关系！
                        // 系统默认 Http1AndHttp2
                        //listenOptions.Protocols = HttpProtocols.Http1AndHttp2;

                        string cerFileName = item.GetValue<string>("Certificate:FileName");
                        string cerPwd = item.GetValue<string>("Certificate:Password");
                        try
                        {
                            if (!string.IsNullOrEmpty(cerFileName))
                            {
                                // 加载X509证书
                                listenOptions.UseHttps(new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "etc/config/", cerFileName), cerPwd));
                            }
                            else
                            {
                                // 无证书使用默认localhost证书
                                listenOptions.UseHttps();
                            }
                            Log.Information($"监听：{listenOptions}");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"Kestrel加载X509证书文件[{cerFileName}]时异常");
                        }
                    });
                }
                else
                {
                    // http协议，无X509证书
                    p_options.Listen(IPAddress.Parse(address), port, listenOptions =>
                    {
                        Log.Information($"监听：{listenOptions}");
                    });
                }
            }
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
            if (Kit.EnableRabbitMQ)
                RabbitMQCenter.Subscribe(p_app.ApplicationServices);

            Log.Information("---启动完毕---");
        }
    }
}
