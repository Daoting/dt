#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    /// <summary>
    /// 用户关联的角色
    /// </summary>
    [Tbl("crud_用户角色")]
    public partial class 用户角色X : EntityX<用户角色X>
    {
        #region 构造方法
        用户角色X() { }

        public 用户角色X(CellList p_cells) : base(p_cells) { }

        public 用户角色X(
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