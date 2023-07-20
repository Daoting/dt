#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.多对多
{
    /// <summary>
    /// 用户关联的角色
    /// </summary>
    [Tbl("demo_用户角色")]
    public partial class 用户角色X : EntityX<用户角色X>
    {
        #region 构造方法
        用户角色X() { }

        public 用户角色X(CellList p_cells) : base(p_cells) { }

        public 用户角色X(
            long UserID,
            long RoleID)
        {
            AddCell("user_id", UserID);
            AddCell("role_id", RoleID);
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

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["role_id"]; }
            set { this["role_id"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}