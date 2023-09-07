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
    /// 用户一角色多对多
    /// </summary>
    [Tbl("CM_USER_ROLE")]
    public partial class UserRoleX : EntityX<UserRoleX>
    {
        #region 构造方法
        UserRoleX() { }

        public UserRoleX(CellList p_cells) : base(p_cells) { }

        public UserRoleX(
            long UserID,
            long RoleID)
        {
            Add("USER_ID", UserID);
            Add("ROLE_ID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["USER_ID"]; }
            set { this["USER_ID"] = value; }
        }

        public Cell cUserID => _cells["USER_ID"];

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