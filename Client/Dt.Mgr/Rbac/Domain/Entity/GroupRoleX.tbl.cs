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
    /// 组一角色多对多
    /// </summary>
    [Tbl("CM_GROUP_ROLE")]
    public partial class GroupRoleX : EntityX<GroupRoleX>
    {
        #region 构造方法
        GroupRoleX() { }

        public GroupRoleX(CellList p_cells) : base(p_cells) { }

        public GroupRoleX(
            long GroupID,
            long RoleID)
        {
            Add("GROUP_ID", GroupID);
            Add("ROLE_ID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 组标识
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GROUP_ID"]; }
            set { this["GROUP_ID"] = value; }
        }

        public Cell cGroupID => _cells["GROUP_ID"];

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["ROLE_ID"]; }
            set { this["ROLE_ID"] = value; }
        }

        public Cell cRoleID => _cells["ROLE_ID"];

        new public long ID { get { return -1; } }
    }
}