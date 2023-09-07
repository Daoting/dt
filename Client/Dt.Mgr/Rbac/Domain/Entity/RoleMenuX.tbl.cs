#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 角色一菜单多对多
    /// </summary>
    [Tbl("CM_ROLE_MENU")]
    public partial class RoleMenuX : EntityX<RoleMenuX>
    {
        #region 构造方法
        RoleMenuX() { }

        public RoleMenuX(CellList p_cells) : base(p_cells) { }

        public RoleMenuX(
            long RoleID,
            long MenuID)
        {
            Add("ROLE_ID", RoleID);
            Add("MENU_ID", MenuID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["ROLE_ID"]; }
            set { this["ROLE_ID"] = value; }
        }

        public Cell cRoleID => _cells["ROLE_ID"];

        /// <summary>
        /// 菜单标识
        /// </summary>
        public long MenuID
        {
            get { return (long)this["MENU_ID"]; }
            set { this["MENU_ID"] = value; }
        }

        public Cell cMenuID => _cells["MENU_ID"];

        new public long ID { get { return -1; } }
    }
}