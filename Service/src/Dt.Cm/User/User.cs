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
    [Tag(TblName = "cm_user", IsCached = true, CacheKey = "ID,Phone")]
    public class User : Root
    {
        public User()
        {
            ID = Id.New(0);
        }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 密码的MD5
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }

        /// <summary>
        /// 当前用户具有的角色，逗号隔开
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// 分组id
        /// </summary>
        //[JsonIgnore]
        //public long GroupID { get; set; }

        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //[JsonIgnore]
        //public DateTime CTime { get; set; }

        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //[JsonIgnore]
        //public DateTime MTime { get; set; }

        public bool ValidatePwd(string p_pwd)
        {
            return true;
        }
    }
}
