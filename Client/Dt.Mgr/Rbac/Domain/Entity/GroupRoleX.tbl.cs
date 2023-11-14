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
    /// 组一角色多对多
    /// </summary>
    [Tbl("cm_group_role")]
    public partial class GroupRoleX : EntityX<GroupRoleX>
    {
        #region 构造方法
        GroupRoleX() { }

        public GroupRoleX(CellList p_cells) : base(p_cells) { }

        public GroupRoleX(
            long GroupID,
            long RoleID)
        {
            Add("group_id", GroupID);
            Add("role_id", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 组标识
        /// </summary>
        public long GroupID
        {
            get { return (long)this["group_id"]; }
            set { this["group_id"] = value; }
        }

        public Cell cGroupID => _cells["group_id"];

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["role_id"]; }
            set { this["role_id"] = value; }
        }

        public Cell cRoleID => _cells["role_id"];

        new public long ID { get { return -1; } }
    }
}