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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
            p_services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    "application/wasm"
                });
            });
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
                pathBase = Path.Combine(AppContext.BaseDirectory, pathBase);

            p_app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(pathBase),
                ContentTypeProvider = CreateContentTypeProvider(true),
                OnPrepareResponse = SetCacheHeaders
            });
        }


        static IContentTypeProvider CreateContentTypeProvider(bool enableDebugging)
        {
            var result = new FileExtensionContentTypeProvider();
            result.Mappings.Add(".clr", MediaTypeNames.Application.Octet);
            result.Mappings.Add(".dat", MediaTypeNames.Application.Octet);
            // result.Mappings.Add(".wasm", "application/wasm");

            if (enableDebugging)
            {
                result.Mappings.Add(".pdb", MediaTypeNames.Application.Octet);
            }

            return result;
        }

        static void SetCacheHeaders(StaticFileResponseContext ctx)
        {
            // By setting "Cache-Control: no-cache", we're allowing the browser to store
            // a cached copy of the response, but telling it that it must check with the
            // server for modifications (based on Etag) before using that cached copy.
            // Longer term, we should generate URLs based on content hashes (at least
            // for published apps) so that the browser doesn't need to make any requests
            // for unchanged files.
            var headers = ctx.Context.Response.GetTypedHeaders();
            if (headers.CacheControl == null)
            {
                headers.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true
                };
            }
        }
    }
}
