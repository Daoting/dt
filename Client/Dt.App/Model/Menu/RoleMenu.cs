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
    [Tbl("cm_rolemenu", "cm")]
    public partial class RoleMenu : Entity
    {
        public RoleMenu()
        { }

        public RoleMenu(
            long RoleID,
            long MenuID)
        {
            AddCell<long>("RoleID", RoleID);
            AddCell<long>("MenuID", MenuID);
            IsAdded = true;
        }

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)_cells["RoleID"].Val; }
            private set { _cells["RoleID"].Val = value; }
        }

        /// <summary>
        /// 菜单标识
        /// </summary>
        public long MenuID
        {
            get { return (long)_cells["MenuID"].Val; }
            private set { _cells["MenuID"].Val = value; }
        }

        new public long ID { get { return -1; } }
    }
}
