#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 用户一组多对多
    /// </summary>
    [Tbl("cm_user_group")]
    public partial class UserGroupX : EntityX<UserGroupX>
    {
        #region 构造方法
        UserGroupX() { }

        public UserGroupX(CellList p_cells) : base(p_cells) { }

        public UserGroupX(
            long UserID,
            long GroupID)
        {
            AddCell("user_id", UserID);
            AddCell("group_id", GroupID);
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
        /// 组标识
        /// </summary>
        public long GroupID
        {
            get { return (long)this["group_id"]; }
            set { this["group_id"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}