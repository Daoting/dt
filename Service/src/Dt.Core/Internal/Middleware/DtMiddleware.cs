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
using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
