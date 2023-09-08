#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.多对多
{
    /// <summary>
    /// 用户关联的角色
    /// </summary>
    [Tbl("DEMO_用户角色")]
    public partial class 用户角色X : EntityX<用户角色X>
    {
        #region 构造方法
        用户角色X() { }

        public 用户角色X(CellList p_cells) : base(p_cells) { }

        public 用户角色X(
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