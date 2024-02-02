#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-08-19 创建
**************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class HttpContextEx
    {
        /// <summary>
        /// 获取客户端的ip地址
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        public static string GetClientIpPort(this HttpContext p_context)
        {
            var ip = p_context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = $"{p_context.Connection.RemoteIpAddress}:{p_context.Connection.RemotePort}";
            }
            return ip;
        }

    }
}
