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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    public class UserRepo : DbRepo<User, long>
    {
        public int _rn;

        public UserRepo()
        {
            _rn = new Random().Next(1, 100000);
        }

        internal Task<User> GetByPhone(string p_phone)
        {
            return Task.FromResult(default(User));
        }
    }
}
