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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    public partial class User
    {
        public User(string p_phone)
        {
            // cm用户标志0
            ID = Id.New(0);
            Phone = p_phone;
            Name = p_phone;
            ResetPwd();
        }

        public void ToggleExpired()
        {
            Expired = !Expired;
        }

        public void ResetPwd()
        {
            // 初始密码为手机号后4位
            Pwd = Kit.GetMD5(Phone.Substring(Phone.Length - 4));
        }
    }
}
