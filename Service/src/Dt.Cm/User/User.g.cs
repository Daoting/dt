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
    [Tbl("cm_user")]
    public partial class User : Root
    {
        public User()
        { }

        public User(string p_phone, string p_name, string p_pwd, bool p_sex, bool p_expired, DateTime p_ctime, DateTime p_mtime)
        {
            Phone = p_phone;
            Name = p_name;
            Pwd = p_pwd;
            Sex = p_sex;
            Expired = p_expired;
            Ctime = p_ctime;
            Mtime = p_mtime;
        }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone { get; private set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string Pwd { get; private set; }

        /// <summary>
        /// 性别，0女1男
        /// </summary>
        public bool Sex { get; private set; }

        /// <summary>
        /// 是否停用
        /// </summary>
        public bool Expired { get; private set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime { get; private set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime { get; private set; }
    }
}
