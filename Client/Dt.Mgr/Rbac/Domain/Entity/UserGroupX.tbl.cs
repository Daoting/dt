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
            Add("user_id", UserID);
            Add("group_id", GroupID);
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
        /// 组标识
        /// </summary>
        public long GroupID
        {
            get { return (long)this["group_id"]; }
            set { this["group_id"] = value; }
        }

        public Cell cGroupID => _cells["group_id"];

        new public long ID { get { return -1; } }
    }
}