﻿#region 文件描述
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
    /// 用户一角色多对多
    /// </summary>
    [Tbl("cm_user_role")]
    public partial class UserRoleX : EntityX<UserRoleX>
    {
        #region 构造方法
        UserRoleX() { }

        public UserRoleX(CellList p_cells) : base(p_cells) { }

        public UserRoleX(
            long UserID,
            long RoleID)
        {
            Add("user_id", UserID);
            Add("role_id", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["user_id"]; }
            set { this["user_id"] = value; }
        }

        public Cell cUserID => _cells["user_id"];

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