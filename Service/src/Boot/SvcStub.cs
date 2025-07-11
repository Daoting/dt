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
            mp.TryAdd(".pdb", MediaTypeNames.Application.Octet);
            mp.TryAdd(".dat", MediaTypeNames.Application.Octet);
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
            if (!Directory.Exists(pathBase))
            {
                var msg = "wasm根路径不存在：" + pathBase;
                Log.Fatal(msg);
                throw new Exception(msg);
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
            });
        }
    }
}
