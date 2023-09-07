#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 工作项
    /// </summary>
    [Tbl("CM_WFI_ITEM")]
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
            Add("ID", ID);
            Add("ATVI_ID", AtviID);
            Add("STATUS", Status);
            Add("ASSIGN_KIND", AssignKind);
            Add("SENDER", Sender);
            Add("STIME", Stime);
            Add("IS_ACCEPT", IsAccept);
            Add("ACCEPT_TIME", AcceptTime);
            Add("ROLE_ID", RoleID);
            Add("USER_ID", UserID);
            Add("NOTE", Note);
            Add("DISPIDX", Dispidx);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 活动实例标识
        /// </summary>
        public long AtviID
        {
            get { return (long)this["ATVI_ID"]; }
            set { this["ATVI_ID"] = value; }
        }

        public Cell cAtviID => _cells["ATVI_ID"];

        /// <summary>
        /// 工作项状态 0活动 1结束 2终止 3同步活动
        /// </summary>
        public WfiItemStatus Status
        {
            get { return (WfiItemStatus)this["STATUS"]; }
            set { this["STATUS"] = value; }
        }

        public Cell cStatus => _cells["STATUS"];

        /// <summary>
        /// 指派方式 0普通指派 1起始指派 2回退 3跳转 4追回 5回退指派
        /// </summary>
        public WfiItemAssignKind AssignKind
        {
            get { return (WfiItemAssignKind)this["ASSIGN_KIND"]; }
            set { this["ASSIGN_KIND"] = value; }
        }

        public Cell cAssignKind => _cells["ASSIGN_KIND"];

        /// <summary>
        /// 发送者
        /// </summary>
        public string Sender
        {
            get { return (string)this["SENDER"]; }
            set { this["SENDER"] = value; }
        }

        public Cell cSender => _cells["SENDER"];

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime Stime
        {
            get { return (DateTime)this["STIME"]; }
            set { this["STIME"] = value; }
        }

        public Cell cStime => _cells["STIME"];

        /// <summary>
        /// 是否签收此项任务
        /// </summary>
        public bool IsAccept
        {
            get { return (bool)this["IS_ACCEPT"]; }
            set { this["IS_ACCEPT"] = value; }
        }

        public Cell cIsAccept => _cells["IS_ACCEPT"];

        /// <summary>
        /// 签收时间
        /// </summary>
        public DateTime? AcceptTime
        {
            get { return (DateTime?)this["ACCEPT_TIME"]; }
            set { this["ACCEPT_TIME"] = value; }
        }

        public Cell cAcceptTime => _cells["ACCEPT_TIME"];

        /// <summary>
        /// 执行者角色标识
        /// </summary>
        public long? RoleID
        {
            get { return (long?)this["ROLE_ID"]; }
            set { this["ROLE_ID"] = value; }
        }

        public Cell cRoleID => _cells["ROLE_ID"];

        /// <summary>
        /// 执行者用户标识
        /// </summary>
        public long? UserID
        {
            get { return (long?)this["USER_ID"]; }
            set { this["USER_ID"] = value; }
        }

        public Cell cUserID => _cells["USER_ID"];

        /// <summary>
        /// 工作项备注
        /// </summary>
        public string Note
        {
            get { return (string)this["NOTE"]; }
            set { this["NOTE"] = value; }
        }

        public Cell cNote => _cells["NOTE"];

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["DISPIDX"]; }
            set { this["DISPIDX"] = value; }
        }

        public Cell cDispidx => _cells["DISPIDX"];

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["CTIME"]; }
            set { this["CTIME"] = value; }
        }

        public Cell cCtime => _cells["CTIME"];

        /// <summary>
        /// 最后一次状态改变的时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["MTIME"]; }
            set { this["MTIME"] = value; }
        }

        public Cell cMtime => _cells["MTIME"];
    }
}