#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
#endregion

namespace Dt.Boot
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
            mp.TryAdd(".clr", MediaTypeNames.Application.Octet);
            mp.TryAdd(".dat", MediaTypeNames.Application.Octet);
            mp.TryAdd(".blat", MediaTypeNames.Application.Octet);
            mp.TryAdd(".pdb", MediaTypeNames.Application.Octet);
            mp.TryAdd(".config", MediaTypeNames.Text.Xml);
            mp.TryAdd(".rsp", MediaTypeNames.Text.Plain);

            // mime类型在 OnPrepareResponse 时重置到非压缩文件的类型
            mp.TryAdd(".br", MediaTypeNames.Application.Octet);
        }

        /// <summary>
        /// 获取服务名称，小写
        /// </summary>
        public override string SvcName => "boot";

        /// <summary>
        /// 定义全局服务
        /// </summary>
        /// <param name="p_services"></param>
        public override void ConfigureServices(IServiceCollection p_services)
        {

        }

        /// <summary>
        /// 自定义请求处理或定义请求管道的中间件
        /// </summary>
        /// <param name="p_app"></param>
        /// <param name="p_handlers">注册自定义请求处理</param>
        public override void Configure(IApplicationBuilder p_app, IDictionary<string, RequestDelegate> p_handlers)
        {
            string pathBase = Kit.GetCfg<string>("WasmPath");
            if (!Path.IsPathRooted(pathBase))
            {
                // 相对路径
                pathBase = Path.Combine(AppContext.BaseDirectory, pathBase);
            }
            var fileProvider = new PhysicalFileProvider(pathBase);

            p_app.UseMiddleware<RewriteBrFileMiddleware>(fileProvider);

            // 该中间件处理访问根路径时的默认页，内部只重置 context.Request.Path 的值
            // 所以必须在UseStaticFiles之前调用，最终由 StaticFiles 中间件响应默认页
            p_app.UseDefaultFiles(new DefaultFilesOptions
            {
                FileProvider = fileProvider,
                RequestPath = ""
            });

            p_app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                ContentTypeProvider = _mimeTypeProvider,
                OnPrepareResponse = SetCacheHeaders
            });
        }

        void SetCacheHeaders(StaticFileResponseContext ctx)
        {
            // 参见uno: https://github.com/unoplatform/Uno.Wasm.Bootstrap/blob/main/src/Uno.Wasm.Bootstrap.Cli/Server/Startup.cs
            // By setting "Cache-Control: no-cache", we're allowing the browser to store
            // a cached copy of the response, but telling it that it must check with the
            // server for modifications (based on Etag) before using that cached copy.
            // Longer term, we should generate URLs based on content hashes (at least
            // for published apps) so that the browser doesn't need to make any requests
            // for unchanged files.
            var headers = ctx.Context.Response.GetTypedHeaders();
            if (headers.CacheControl == null)
            {
                headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            }

            if (!ctx.File.Name.EndsWith(".br"))
                return;

            // 重置到非压缩文件的mime类型
            var fileName = ctx.File.Name.Substring(0, ctx.File.Name.Length - 3);
            if (_mimeTypeProvider.TryGetContentType(fileName, out var mimeType))
            {
                headers.ContentType = new MediaTypeHeaderValue(new StringSegment(mimeType));
            }
        }
    }
}
