#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.FileProviders;
#endregion

namespace Dt.Boot
{
    /// <summary>
    /// 重写请求文件路径，指向Brotli压缩文件 *.br
    /// </summary>
    public class RewriteBrFileMiddleware
    {
        readonly RequestDelegate _next;
        readonly PhysicalFileProvider _fileProvider;

        public RewriteBrFileMiddleware(RequestDelegate p_next, PhysicalFileProvider p_fileProvider)
        {
            _next = p_next;
            _fileProvider = p_fileProvider;
        }

        public async Task Invoke(HttpContext p_context)
        {
            var req = p_context.Request;

            // 浏览器支持Brotli压缩、请求文件的br文件存在
            if ((HttpMethods.IsGet(req.Method) || HttpMethods.IsHead(req.Method))
                && IsFile(req.Path.Value))
            {
                //var fi = _fileProvider.GetFileInfo(req.Path.Value);
                //if (File.Exists(fi.PhysicalPath + ".gz"))
                //{
                //    // gz文件存在
                //    req.Path += ".gz";
                //    // 响应头标志br压缩
                //    p_context.Response.Headers["Content-Encoding"] = "gzip";
                //}
            }

            await _next(p_context);
        }

        /// <summary>
        /// 是否有扩展名
        /// </summary>
        /// <param name="p_path"></param>
        /// <returns></returns>
        static bool IsFile(string p_path)
        {
            if (string.IsNullOrWhiteSpace(p_path))
                return false;
            return p_path.LastIndexOf('.') > 0;
        }
    }
}
