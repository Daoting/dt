#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-07-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 重写请求文件路径，指向压缩文件 *.gz
    /// </summary>
    public class RewriteGzFileMiddleware
    {
        readonly RequestDelegate _next;
        readonly PhysicalFileProvider _fileProvider;
        
        public RewriteGzFileMiddleware(RequestDelegate p_next, PhysicalFileProvider p_fileProvider)
        {
            _next = p_next;
            _fileProvider = p_fileProvider;
        }

        public async Task Invoke(HttpContext p_context)
        {
            var req = p_context.Request;

            // 若请求文件的gz文件存在，指向压缩文件
            if ((HttpMethods.IsGet(req.Method) || HttpMethods.IsHead(req.Method))
                && req.Path.Value is string path
                && path.LastIndexOf('.') > 0  // 有扩展名
                && path.StartsWith(Cfg.WasmVirPath, StringComparison.OrdinalIgnoreCase))
            {
                var fi = _fileProvider.GetFileInfo(path.Substring(Cfg.WasmVirPath.Length));
                if (File.Exists(fi.PhysicalPath + ".gz"))
                {
                    // gz文件存在
                    req.Path = path + ".gz";
                }
            }

            await _next(p_context);
        }
    }
}
