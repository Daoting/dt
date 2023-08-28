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
    /// 活动模板
    /// </summary>
    [Tbl("cm_wfd_atv")]
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
            Add("id", ID);
            Add("prc_id", PrcID);
            Add("name", Name);
            Add("type", Type);
            Add("exec_scope", ExecScope);
            Add("exec_limit", ExecLimit);
            Add("exec_atv_id", ExecAtvID);
            Add("auto_accept", AutoAccept);
            Add("can_delete", CanDelete);
            Add("can_terminate", CanTerminate);
            Add("can_jump_into", CanJumpInto);
            Add("trans_kind", TransKind);
            Add("join_kind", JoinKind);
            Add("ctime", Ctime);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["prc_id"]; }
            set { this["prc_id"] = value; }
        }

        /// <summary>
        /// 活动名称，同时作为状态名称
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动
        /// </summary>
        public WfdAtvType Type
        {
            get { return (WfdAtvType)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// 执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户
        /// </summary>
        public WfdAtvExecScope ExecScope
        {
            get { return (WfdAtvExecScope)this["exec_scope"]; }
            set { this["exec_scope"] = value; }
        }

        /// <summary>
        /// 执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者
        /// </summary>
        public WfdAtvExecLimit ExecLimit
        {
            get { return (WfdAtvExecLimit)this["exec_limit"]; }
            set { this["exec_limit"] = value; }
        }

        /// <summary>
        /// 在执行者限制为3或4时选择的活动
        /// </summary>
        public long? ExecAtvID
        {
            get { return (long?)this["exec_atv_id"]; }
            set { this["exec_atv_id"] = value; }
        }

        /// <summary>
        /// 是否自动签收，打开工作流视图时自动签收工作项
        /// </summary>
        public bool AutoAccept
        {
            get { return (bool)this["auto_accept"]; }
            set { this["auto_accept"] = value; }
        }

        /// <summary>
        /// 能否删除流程实例和业务数据，0否 1
        /// </summary>
        public bool CanDelete
        {
            get { return (bool)this["can_delete"]; }
            set { this["can_delete"] = value; }
        }

        /// <summary>
        /// 能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能
        /// </summary>
        public bool CanTerminate
        {
            get { return (bool)this["can_terminate"]; }
            set { this["can_terminate"] = value; }
        }

        /// <summary>
        /// 是否可作为跳转目标，0不可跳转 1可以
        /// </summary>
        public bool CanJumpInto
        {
            get { return (bool)this["can_jump_into"]; }
            set { this["can_jump_into"] = value; }
        }

        /// <summary>
        /// 当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择
        /// </summary>
        public WfdAtvTransKind TransKind
        {
            get { return (WfdAtvTransKind)this["trans_kind"]; }
            set { this["trans_kind"] = value; }
        }

        /// <summary>
        /// 同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步
        /// </summary>
        public WfdAtvJoinKind JoinKind
        {
            get { return (WfdAtvJoinKind)this["join_kind"]; }
            set { this["join_kind"] = value; }
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
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }
    }
}