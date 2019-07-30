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

namespace Dts.Core
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

        public async Task Invoke(HttpContext p_context)
        {
            // 内部特殊路径格式：/.xxx
            switch (p_context.Request.Path.Value.ToLower())
            {
                case "/.c":
                    await Authenticate(p_context);
                    await new LobContext(p_context).Handle();
                    return;
                case "/.admin":
                    await ResponseAdminPage(p_context);
                    return;
                case "/.error":
                    await ResponseErrorPage(p_context);
                    return;
            }
            await _next(p_context);
        }

        async Task Authenticate(HttpContext p_context)
        {
            // 只本地认证(JWT格式)，未处理远程认证及重定向，原中间件见Authentication.txt
            var authScheme = await _schemes.GetDefaultAuthenticateSchemeAsync();
            if (authScheme != null)
            {
                var result = await p_context.AuthenticateAsync(authScheme.Name);
                if (result?.Principal != null)
                    p_context.User = result.Principal;
            }
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
                    using (var sr = new StreamReader(typeof(DtMiddleware).Assembly.GetManifestResourceStream("Dts.Core.Res.Admin.html")))
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
                    using (var sr = new StreamReader(typeof(DtMiddleware).Assembly.GetManifestResourceStream("Dts.Core.Res.Error.html")))
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
