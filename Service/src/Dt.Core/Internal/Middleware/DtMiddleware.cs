#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统内置中间件，完成内部特殊路径处理；
    /// </summary>
    public class DtMiddleware
    {
        #region 成员变量
        /// <summary>
        /// 外部自定义请求处理
        /// </summary>
        internal static readonly Dictionary<string, RequestDelegate> RequestHandlers = new Dictionary<string, RequestDelegate>();

        static string _adminPage;
        static string _errorPage;

        readonly RequestDelegate _next;
        #endregion

        #region 构造方法
        public DtMiddleware(RequestDelegate p_next)
        {
            _next = p_next ?? throw new ArgumentNullException(nameof(p_next));
        }
        #endregion

        public Task Invoke(HttpContext p_context)
        {
            // 内部特殊路径格式：/.xxx
            string path = p_context.Request.Path.Value.ToLower();
            if (path == "/.c")
                return new ApiInvoker(p_context).Handle();
            if (path == "/.admin")
                return ResponseAdminPage(p_context);
            if (path == "/.log")
                return ResponseLog(p_context);
            if (path == "/.download")
                return DownloadFile(p_context);
            if (path == "/.error")
                return ResponseErrorPage(p_context);

            // 外部自定义路径，截取路径的前一节 /.xxx/xxx/xxx
            int index = path.TrimStart('/').IndexOf('/');
            if (index > -1)
                path = path.Substring(0, index + 1);
            if (RequestHandlers.TryGetValue(path, out var callback))
                return callback(p_context);

            return _next(p_context);
        }

        /// <summary>
        /// 获取管理页面
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        static async Task ResponseAdminPage(HttpContext p_context)
        {
            if (string.IsNullOrEmpty(_adminPage))
            {
                try
                {
                    using (var sr = new StreamReader(typeof(DtMiddleware).Assembly.GetManifestResourceStream("Dt.Core.Res.Admin.html")))
                    {
                        _adminPage = sr.ReadToEnd();
                    }
                }
                catch { }
            }
            p_context.Response.ContentType = "text/html";
            await p_context.Response.WriteAsync(_adminPage);
        }

        /// <summary>
        /// 获取出错页面
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        static async Task ResponseErrorPage(HttpContext p_context)
        {
            if (string.IsNullOrEmpty(_errorPage))
            {
                try
                {
                    using (var sr = new StreamReader(typeof(DtMiddleware).Assembly.GetManifestResourceStream("Dt.Core.Res.Error.html")))
                    {
                        _errorPage = sr.ReadToEnd();
                    }
                }
                catch { }
            }
            p_context.Response.ContentType = "text/html";
            await p_context.Response.WriteAsync(_errorPage);
        }

        /// <summary>
        /// 实时获取日志内容，未使用.c的rpc方式，因为方法内部若输出日志会造成死循环！
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        static async Task ResponseLog(HttpContext p_context)
        {
            try
            {
                string msg = null;
                p_context.Response.ContentType = "text/html";
                if (p_context.Request.Query.TryGetValue("index", out var val)
                    && int.TryParse(val, out int index))
                {
                    // 实时获取日志
                    msg = await HtmlLogHub.GetLog(index);
                }
                await p_context.Response.WriteAsync(msg == null ? "" : msg, p_context.RequestAborted);
            }
            catch { }
        }

        /// <summary>
        /// 下载文件，如：admin页面下载日志
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        static async Task DownloadFile(HttpContext p_context)
        {
            string filePath;
            using (StreamReader sr = new StreamReader(p_context.Request.Body))
            {
                // 客户端提供完整路径
                filePath = Path.Combine(AppContext.BaseDirectory, await sr.ReadToEndAsync());
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                p_context.Response.Headers["error"] = WebUtility.UrlEncode("😢下载失败，文件不存在！");
                Log.Information("文件不存在：" + filePath);
                return;
            }

            var response = p_context.Response;
            response.Headers["Content-Type"] = "application/octet-stream";
            response.Headers["Content-Transfer-Encoding"] = "binary";
            response.Headers["Content-Length"] = fileInfo.Length.ToString();
            // 不以附件形式下载
            //response.Headers["Content-Disposition"] = "attachment;filename=" + path.Substring(path.LastIndexOf('/') + 1);

            try
            {
                await response.SendFileAsync(filePath, p_context.RequestAborted);
            }
            catch { }
        }
    }
}
