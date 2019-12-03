#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.App.Model
{
    [Tbl("cm_roleprv", "cm")]
    public partial class RolePrv : Entity
    {
        public RolePrv()
        { }

        public RolePrv(
            long roleid,
            string prvid)
        {
            AddCell<long>("roleid", roleid);
            AddCell<string>("prvid", prvid);
            IsAdded = true;
        }

        /// <summary>
        /// 角色标识
        /// </summary>
        public long roleid
        {
            get { return (long)_cells["roleid"].Val; }
            private set { _cells["roleid"].Val = value; }
        }

        /// <summary>
        /// 权限标识
        /// </summary>
        public string prvid
        {
            get { return (string)_cells["prvid"].Val; }
            private set { _cells["prvid"].Val = value; }
        }

        new public long ID { get { return -1; } }
    }
}
