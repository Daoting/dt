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
    /// 用户一组多对多
    /// </summary>
    [Tbl("CM_USER_GROUP")]
    public partial class UserGroupX : EntityX<UserGroupX>
    {
        #region 构造方法
        UserGroupX() { }

        public UserGroupX(CellList p_cells) : base(p_cells) { }

        public UserGroupX(
            long UserID,
            long GroupID)
        {
            Add("USER_ID", UserID);
            Add("GROUP_ID", GroupID);
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
        /// 组标识
        /// </summary>
        public long GroupID
        {
            get { return (long)this["GROUP_ID"]; }
            set { this["GROUP_ID"] = value; }
        }

        public Cell cGroupID => _cells["GROUP_ID"];

        new public long ID { get { return -1; } }
    }
}