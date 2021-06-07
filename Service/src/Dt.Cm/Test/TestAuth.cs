#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试")]
    public class TestAuth : BaseApi
    {
        public string NoAuth()
        {
            return "无授权验证";
        }

        [Auth]
        public string Auth()
        {
            return "必须登录后可访问";
        }

        [Auth(typeof(MyAuth))]
        public string CustomAuth()
        {
            return "自定义授权验证";
        }
    }

    public class MyAuth : ICustomAuth
    {
        public Task<bool> IsAuthenticated(HttpContext p_context)
        {
            string userID = p_context.Request.Headers["uid"];
            return Task.FromResult(false);
        }
    }
}
