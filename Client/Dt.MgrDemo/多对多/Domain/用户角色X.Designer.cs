#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-23 创建
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
            AddCell("UserID", UserID);
            AddCell("RoleID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}