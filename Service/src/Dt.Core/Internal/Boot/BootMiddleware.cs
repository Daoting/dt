#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统内置中间件，wasm客户端访问中间件
    /// </summary>
    public class BootMiddleware
    {
        #region 成员变量
        readonly RequestDelegate _next;
        #endregion

        #region 构造方法
        public BootMiddleware(RequestDelegate p_next)
        {
            _next = p_next ?? throw new ArgumentNullException(nameof(p_next));
        }
        #endregion

        public Task Invoke(HttpContext p_context)
        {
            // 是否跨域请求：跨域预检 或 wasm标志
            if (p_context.Request.Method == "OPTIONS"
                || p_context.Request.Headers.ContainsKey("dt-wasm"))
            {
                var headers = p_context.Response.Headers;
                // 允许跨域请求的域
                headers.Append("Access-Control-Allow-Origin", "*");
                // 允许跨域请求时的HTTP方法，如 GET PUT POST OPTIONS
                headers.Append("Access-Control-Allow-Methods", "*");
                // 允许跨域请求时自定义 header 字段
                headers.Append("Access-Control-Allow-Headers", "*");
                // 若要支持iframe内的跨域请求，需要以下两个header
                //headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
                //headers.Append("Cross-Origin-Opener-Policy", "same-origin");

                // wasm客户端跨域请求的预检
                if (p_context.Request.Method == "OPTIONS")
                    return Task.CompletedTask;
            }

            // 下一中间件处理
            return _next(p_context);
        }
    }
}
