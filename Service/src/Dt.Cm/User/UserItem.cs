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
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    [CacheCfg(TblName = "cm_user", PrefixKey = "user", OtherKey = "Phone", ExpiryHour = 1)]
    public class UserItem : ICacheItem
    {
        public long ID { get; set; }

        public string Phone { get; set; }

        public string Pwd { get; set; }

        public string Name { get; set; }

        public bool Sex { get; set; }

        /// <summary>
        /// 当前用户具有的角色，逗号隔开
        /// </summary>
        public string Roles { get; set; }

        async Task ICacheItem.Init(Db p_db)
        {
            var result = await p_db.EachRow("select roleid from cm_userrole where userid=@userid", new { userid = ID });
            StringBuilder sb = new StringBuilder();
            foreach (var row in result)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(row.Str("roleid"));
            }
            Roles = sb.ToString();
        }
    }
}
