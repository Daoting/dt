#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 工作项
    /// </summary>
    [Tbl("cm_wfi_item")]
    public partial class WfiItemX : EntityX<WfiItemX>
    {
        #region 构造方法
        WfiItemX() { }

        public WfiItemX(CellList p_cells) : base(p_cells) { }

        public WfiItemX(
            long ID,
            long AtviID = default,
            WfiItemStatus Status = default,
            WfiItemAssignKind AssignKind = default,
            string Sender = default,
            DateTime Stime = default,
            bool IsAccept = default,
            DateTime? AcceptTime = default,
            long? RoleID = default,
            long? UserID = default,
            string Note = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("id", ID);
            Add("atvi_id", AtviID);
            Add("status", Status);
            Add("assign_kind", AssignKind);
            Add("sender", Sender);
            Add("stime", Stime);
            Add("is_accept", IsAccept);
            Add("accept_time", AcceptTime);
            Add("role_id", RoleID);
            Add("user_id", UserID);
            Add("note", Note);
            Add("dispidx", Dispidx);
            Add("ctime", Ctime);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 活动实例标识
        /// </summary>
        public long AtviID
        {
            get { return (long)this["atvi_id"]; }
            set { this["atvi_id"] = value; }
        }

        /// <summary>
        /// 工作项状态 0活动 1结束 2终止 3同步活动
        /// </summary>
        public WfiItemStatus Status
        {
            get { return (WfiItemStatus)this["status"]; }
            set { this["status"] = value; }
        }

        /// <summary>
        /// 指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派
        /// </summary>
        public WfiItemAssignKind AssignKind
        {
            get { return (WfiItemAssignKind)this["assign_kind"]; }
            set { this["assign_kind"] = value; }
        }

        /// <summary>
        /// 发送者
        /// </summary>
        public string Sender
        {
            get { return (string)this["sender"]; }
            set { this["sender"] = value; }
        }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Stime
        {
            get { return (DateTime)this["stime"]; }
            set { this["stime"] = value; }
        }

        /// <summary>
        /// 是否签收此项任务
        /// </summary>
        public bool IsAccept
        {
            get { return (bool)this["is_accept"]; }
            set { this["is_accept"] = value; }
        }

        /// <summary>
        /// 签收时间
        /// </summary>
        public DateTime? AcceptTime
        {
            get { return (DateTime?)this["accept_time"]; }
            set { this["accept_time"] = value; }
        }

        /// <summary>
        /// 执行者角色标识
        /// </summary>
        public long? RoleID
        {
            get { return (long?)this["role_id"]; }
            set { this["role_id"] = value; }
        }

        /// <summary>
        /// 执行者用户标识
        /// </summary>
        public long? UserID
        {
            get { return (long?)this["user_id"]; }
            set { this["user_id"] = value; }
        }

        /// <summary>
        /// 工作项备注
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["dispidx"]; }
            set { this["dispidx"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["ctime"]; }
            set { this["ctime"] = value; }
        }

        /// <summary>
        /// 最后一次状态改变的时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }
    }
}