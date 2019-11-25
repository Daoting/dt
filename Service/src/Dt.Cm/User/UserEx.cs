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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    public partial class User
    {
        public static User CreateByPhone(string p_phone)
        {
            // cm用户标志0
            return new User(
                ID: Id.New(0),
                Phone: p_phone,
                Name: p_phone,
                Pwd: Kit.GetMD5(p_phone.Substring(p_phone.Length - 4)));
        }

        public void ToggleExpired()
        {
            Expired = !Expired;
        }

        public void InitNewUser()
        {
            if (ID == 0)
                ID = Id.New(0);
            if (string.IsNullOrEmpty(Pwd))
                Pwd = Kit.GetMD5(Phone.Substring(Phone.Length - 4));
            Ctime = Mtime = Glb.Now;
        }

        public void ResetPwd()
        {
            // 初始密码为手机号后4位
            Pwd = Kit.GetMD5(Phone.Substring(Phone.Length - 4));
        }
    }
}
