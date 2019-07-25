#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-05-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 默认内置静态页面
    /// </summary>
    internal static class PageFileHandler
    {
        static string _adminPage;
        static string _errorPage;

        /// <summary>
        /// 获取管理页面
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        public static async Task Admin(HttpContext p_context)
        {
            if (string.IsNullOrEmpty(_adminPage))
            {
                try
                {
                    using (var sr = new StreamReader(typeof(PageFileHandler).Assembly.GetManifestResourceStream("Dts.Core.Res.Admin.html")))
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
        public static async Task Error(HttpContext p_context)
        {
            if (string.IsNullOrEmpty(_errorPage))
            {
                try
                {
                    using (var sr = new StreamReader(typeof(PageFileHandler).Assembly.GetManifestResourceStream("Dts.Core.Res.Error.html")))
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
