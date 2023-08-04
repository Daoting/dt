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
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统内置中间件，初始化数据库
    /// </summary>
    public class InitModeMiddleware
    {
        #region 成员变量
        static string _adminPage;
        #endregion

        #region 构造方法
        public InitModeMiddleware(RequestDelegate p_next)
        {
        }
        #endregion

        public Task Invoke(HttpContext p_context)
        {
            string path = p_context.Request.Path.Value.ToLower();
            if (path == "/.c")
                return new InitModeInvoker(p_context).Handle();

            if (path == "/.log")
                return DtMiddleware.ResponseLog(p_context);

            if (path == "/.admin")
                return ResponseAdminPage(p_context);

            p_context.Response.Redirect(p_context.Request.PathBase + "/.admin");
            return Task.CompletedTask;
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
                    using (var sr = new StreamReader(typeof(DtMiddleware).Assembly.GetManifestResourceStream("Dt.Core.Res.Init.html")))
                    {
                        _adminPage = sr.ReadToEnd();
                    }
                }
                catch { }
            }
            p_context.Response.ContentType = "text/html";
            await p_context.Response.WriteAsync(_adminPage);
        }
    }
}
