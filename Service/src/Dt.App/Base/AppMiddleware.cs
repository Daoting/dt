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
    public class AppMiddleware
    {
        public Task Handle(HttpContext p_context)
        {
            p_context.Response.ContentType = "text/html";
            return p_context.Response.WriteAsync(_page);
        }

        static AppMiddleware()
        {
            try
            {
                using (var sr = new StreamReader(typeof(AppMiddleware).Assembly.GetManifestResourceStream("Dt.App.Res.Home.html")))
                {
                    _page = sr.ReadToEnd();
                }
            }
            catch { }
        }
        
        static string _page;
    }
}
