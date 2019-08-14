#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统内置中间件，完成：
    /// JWT格式的本地认证；
    /// 内部特殊路径处理；
    /// </summary>
    public class DtMiddleware
    {
        #region 成员变量
        static string _adminPage;
        static string _errorPage;

        readonly RequestDelegate _next;
        readonly IAuthenticationSchemeProvider _schemes;
        #endregion

        #region 构造方法
        public DtMiddleware(RequestDelegate p_next, IAuthenticationSchemeProvider p_schemes)
        {
            _next = p_next ?? throw new ArgumentNullException(nameof(p_next));
            _schemes = p_schemes ?? throw new ArgumentNullException(nameof(p_schemes));
        }
        #endregion

        public Task Invoke(HttpContext p_context)
        {
            // 内部特殊路径格式：/.xxx
            string path = p_context.Request.Path.Value.ToLower();
            if (path == "/.c")
                return new LobContext(p_context).Handle(_schemes);
            if (path == "/.admin")
                return ResponseAdminPage(p_context);
            if (path == "/.error")
                return ResponseErrorPage(p_context);

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
