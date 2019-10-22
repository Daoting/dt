#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    public class UserRepo : Repo<User>
    {
        public async Task<User> GetByPhone(string p_phone)
        {
            User user = await GetFromCache("phone", p_phone);
            if (user != null)
                return user;

            user = await _.Db.First<User>("登录-手机号获取用户", new { phone = p_phone });
            if (user != null)
                await AddToCache(user);
            return user;
        }

        protected override Task OnGot(User p_entity)
        {
            
            return Task.CompletedTask;
        }
    }
}
