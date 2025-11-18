#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Net.Mime;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 服务存根
    /// </summary>
    public class SvcStub : Stub
    {
        readonly FileExtensionContentTypeProvider _mimeTypeProvider;

        public SvcStub()
        {
            _mimeTypeProvider = new FileExtensionContentTypeProvider();
            var mp = _mimeTypeProvider.Mappings;
            mp.TryAdd(".pdb", MediaTypeNames.Application.Octet);
            mp.TryAdd(".dat", MediaTypeNames.Application.Octet);
            mp.TryAdd(".msix", MediaTypeNames.Application.Octet);
            mp.TryAdd(".cer", MediaTypeNames.Application.Octet);
            mp.TryAdd(".apk", MediaTypeNames.Application.Octet);
        }

        /// <summary>
        /// 定义全局服务
        /// </summary>
        /// <param name="p_services"></param>
        public override void ConfigureServices(IServiceCollection p_services)
        {
            p_services.AddSingleton<SqliteFileHandler>();
            // 增加浏览目录功能
            p_services.AddDirectoryBrowser();
        }

        /// <summary>
        /// 自定义请求处理或定义请求管道的中间件
        /// </summary>
        /// <param name="p_app"></param>
        /// <param name="p_handlers">注册自定义请求处理</param>
        public override void Configure(IApplicationBuilder p_app, IDictionary<string, RequestDelegate> p_handlers)
        {
            Cfg.Init();

            // 注册请求路径处理
            Kit.GetService<SqliteFileHandler>().Init(p_handlers);
            p_handlers[""] = (p_context) =>
            {
                // 重定向到带后缀 /
                p_context.Response.Redirect(p_context.Request.PathBase + "/");
                return Task.CompletedTask;
            };
            p_handlers["/"] = (p_context) => new HomeMiddleware().Handle(p_context);

            // 安装包目录可浏览可手动下载
            p_app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(Cfg.PackageDir),
                RequestPath = Cfg.PackageVirPath
            });
            p_app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Cfg.PackageDir),
                ContentTypeProvider = _mimeTypeProvider,
                RequestPath = Cfg.PackageVirPath
            });
            
            // wasm目录，wasm网站静态文件
            var fileProvider = new PhysicalFileProvider(Cfg.WasmDir);

            // 该中间件处理访问根路径时的默认页，内部只重置 context.Request.Path 的值
            // 所以必须在UseStaticFiles之前调用，最终由 StaticFiles 中间件响应默认页
            p_app.UseDefaultFiles(new DefaultFilesOptions
            {
                FileProvider = fileProvider,
                RequestPath = Cfg.WasmVirPath
            });

            // 请求路径指向压缩文件 .gz
            p_app.UseMiddleware<RewriteGzFileMiddleware>(fileProvider);

            p_app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                ContentTypeProvider = _mimeTypeProvider,
                RequestPath = Cfg.WasmVirPath,
                OnPrepareResponse = context =>
                {
                    if (!context.File.Name.EndsWith(".gz"))
                        return;

                    var fileName = context.File.Name.Substring(0, context.File.Name.Length - 3);
                    int index = fileName.LastIndexOf('.');
                    if (index > 0)
                    {
                        if (_mimeTypeProvider.TryGetContentType(fileName.Substring(index), out var mimeType))
                        {
                            var headers = context.Context.Response.Headers;
                            headers["Content-Encoding"] = "gzip";
                            // 重置为实际请求的类型，否则客户端出错
                            headers["Content-Type"] = mimeType;
                            
                            // 增加安全策略，确保本地网站可运行
                            if (fileName == "index.html")
                                headers["Content-Security-Policy"] = "frame-ancestors 'self' https://html https://md https://pdf;";
                        }
                    }
                }
            });
            Log.Information($"cm：/  {Cfg.WasmVirPath}  {Cfg.PackageVirPath}，3个虚拟目录");
        }
    }
}
