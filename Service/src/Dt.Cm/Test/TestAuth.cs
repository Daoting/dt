#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Domain;
using Microsoft.AspNetCore.Http;
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
        public async Task Test()
        {
            User u = new User();
            u.ID = 13436621442719744;
            u.Phone = "15948371821";
            u.Name = "daoting";
            u.Pwd = "124";
            await new Repo<User>().Update(u, CudEvent.Local);
        }

        public string NoAuth()
        {
            return "无授权验证";
        }

        [Auth]
        public string Auth()
        {
            return "所有登录用户都可访问";
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
            return Task.FromResult(true);
        }
    }

    public class InsertHandler : DeleteEventHandler<User>
    {
        public override Task Handle(DeleteEvent<User> p_event)
        {
            return base.Handle(p_event);
        }
    }
}
