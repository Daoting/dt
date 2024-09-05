#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 末尾中间件，处理未找到请求目标的情况
    /// </summary>
    public class EndMiddleware
    {
        static string _page;

        public EndMiddleware(RequestDelegate p_next)
        { }

        public async Task Invoke(HttpContext p_context)
        {
            if (string.IsNullOrEmpty(_page))
            {
                try
                {
                    using (var sr = new StreamReader(typeof(EndMiddleware).Assembly.GetManifestResourceStream("Dt.Core.Res.404.html")))
                    {
                        _page = sr.ReadToEnd();
                    }
                }
                catch { }
            }

            // 内部特殊路径格式：/.xxx
            string path = p_context.Request.Path.Value;
            p_context.Response.ContentType = "text/html";
            p_context.Response.StatusCode = 404;
            await p_context.Response.WriteAsync(string.Format(_page, path));
        }
    }
}
