#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 角色一菜单多对多
    /// </summary>
    [Tbl("cm_role_menu")]
    public partial class RoleMenuX : EntityX<RoleMenuX>
    {
        #region 构造方法
        RoleMenuX() { }

        public RoleMenuX(CellList p_cells) : base(p_cells) { }

        public RoleMenuX(
            long RoleID,
            long MenuID)
        {
            Add("role_id", RoleID);
            Add("menu_id", MenuID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["role_id"]; }
            set { this["role_id"] = value; }
        }

        public Cell cRoleID => _cells["role_id"];

        /// <summary>
        /// 菜单标识
        /// </summary>
        public long MenuID
        {
            get { return (long)this["menu_id"]; }
            set { this["menu_id"] = value; }
        }

        public Cell cMenuID => _cells["menu_id"];

        new public long ID { get { return -1; } }
    }
}