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
    /// 活动模板
    /// </summary>
    [Tbl("CM_WFD_ATV")]
    public partial class WfdAtvX : EntityX<WfdAtvX>
    {
        #region 构造方法
        WfdAtvX() { }

        public WfdAtvX(CellList p_cells) : base(p_cells) { }

        public WfdAtvX(
            long ID,
            long PrcID = default,
            string Name = default,
            WfdAtvType Type = default,
            WfdAtvExecScope ExecScope = default,
            WfdAtvExecLimit ExecLimit = default,
            long? ExecAtvID = default,
            bool AutoAccept = default,
            bool CanDelete = default,
            bool CanTerminate = default,
            bool CanJumpInto = default,
            WfdAtvTransKind TransKind = default,
            WfdAtvJoinKind JoinKind = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            Add("ID", ID);
            Add("PRC_ID", PrcID);
            Add("NAME", Name);
            Add("TYPE", Type);
            Add("EXEC_SCOPE", ExecScope);
            Add("EXEC_LIMIT", ExecLimit);
            Add("EXEC_ATV_ID", ExecAtvID);
            Add("AUTO_ACCEPT", AutoAccept);
            Add("CAN_DELETE", CanDelete);
            Add("CAN_TERMINATE", CanTerminate);
            Add("CAN_JUMP_INTO", CanJumpInto);
            Add("TRANS_KIND", TransKind);
            Add("JOIN_KIND", JoinKind);
            Add("CTIME", Ctime);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["PRC_ID"]; }
            set { this["PRC_ID"] = value; }
        }

        public Cell cPrcID => _cells["PRC_ID"];

        /// <summary>
        /// 活动名称，同时作为状态名称
        /// </summary>
        public string Name
        {
            get { return (string)this["NAME"]; }
            set { this["NAME"] = value; }
        }

        public Cell cName => _cells["NAME"];

        /// <summary>
        /// 活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动
        /// </summary>
        public WfdAtvType Type
        {
            get { return (WfdAtvType)this["TYPE"]; }
            set { this["TYPE"] = value; }
        }

        public Cell cType => _cells["TYPE"];

        /// <summary>
        /// 执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户
        /// </summary>
        public WfdAtvExecScope ExecScope
        {
            get { return (WfdAtvExecScope)this["EXEC_SCOPE"]; }
            set { this["EXEC_SCOPE"] = value; }
        }

        public Cell cExecScope => _cells["EXEC_SCOPE"];

        /// <summary>
        /// 执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者
        /// </summary>
        public WfdAtvExecLimit ExecLimit
        {
            get { return (WfdAtvExecLimit)this["EXEC_LIMIT"]; }
            set { this["EXEC_LIMIT"] = value; }
        }

        public Cell cExecLimit => _cells["EXEC_LIMIT"];

        /// <summary>
        /// 在执行者限制为3或4时选择的活动
        /// </summary>
        public long? ExecAtvID
        {
            get { return (long?)this["EXEC_ATV_ID"]; }
            set { this["EXEC_ATV_ID"] = value; }
        }

        public Cell cExecAtvID => _cells["EXEC_ATV_ID"];

        /// <summary>
        /// 是否自动签收，打开工作流视图时自动签收工作项
        /// </summary>
        public bool AutoAccept
        {
            get { return (bool)this["AUTO_ACCEPT"]; }
            set { this["AUTO_ACCEPT"] = value; }
        }

        public Cell cAutoAccept => _cells["AUTO_ACCEPT"];

        /// <summary>
        /// 能否删除流程实例和业务数据，0否 1
        /// </summary>
        public bool CanDelete
        {
            get { return (bool)this["CAN_DELETE"]; }
            set { this["CAN_DELETE"] = value; }
        }

        public Cell cCanDelete => _cells["CAN_DELETE"];

        /// <summary>
        /// 能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能
        /// </summary>
        public bool CanTerminate
        {
            get { return (bool)this["CAN_TERMINATE"]; }
            set { this["CAN_TERMINATE"] = value; }
        }

        public Cell cCanTerminate => _cells["CAN_TERMINATE"];

        /// <summary>
        /// 是否可作为跳转目标，0不可跳转 1可以
        /// </summary>
        public bool CanJumpInto
        {
            get { return (bool)this["CAN_JUMP_INTO"]; }
            set { this["CAN_JUMP_INTO"] = value; }
        }

        public Cell cCanJumpInto => _cells["CAN_JUMP_INTO"];

        /// <summary>
        /// 当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择
        /// </summary>
        public WfdAtvTransKind TransKind
        {
            get { return (WfdAtvTransKind)this["TRANS_KIND"]; }
            set { this["TRANS_KIND"] = value; }
        }

        public Cell cTransKind => _cells["TRANS_KIND"];

        /// <summary>
        /// 同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步
        /// </summary>
        public WfdAtvJoinKind JoinKind
        {
            get { return (WfdAtvJoinKind)this["JOIN_KIND"]; }
            set { this["JOIN_KIND"] = value; }
        }

        public Cell cJoinKind => _cells["JOIN_KIND"];

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
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["MTIME"]; }
            set { this["MTIME"] = value; }
        }

        public Cell cMtime => _cells["MTIME"];
    }
}