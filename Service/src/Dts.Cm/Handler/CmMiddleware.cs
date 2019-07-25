#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
#endregion

namespace Dts.Cm
{
    /// <summary>
    /// 中间件
    /// </summary>
    public class CmMiddleware
    {
        readonly RequestDelegate _next;

        public CmMiddleware(RequestDelegate p_next)
        {
            _next = p_next ?? throw new ArgumentNullException(nameof(p_next));
        }

        public async Task Invoke(HttpContext p_context)
        {
            if (p_context.Request.Path.Value.ToLower() == "/.model")
                await ModelHandler.GetFile(p_context);
            else
                await _next(p_context);
        }
    }
}
