#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.App.Workflow
{
    #region 自动生成
    [Tbl("cm_wfd_atv")]
    public partial class WfdAtvObj : Entity
    {
        #region 构造方法
        WfdAtvObj() { }

        public WfdAtvObj(
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
            AddCell("ID", ID);
            AddCell("PrcID", PrcID);
            AddCell("Name", Name);
            AddCell("Type", Type);
            AddCell("ExecScope", ExecScope);
            AddCell("ExecLimit", ExecLimit);
            AddCell("ExecAtvID", ExecAtvID);
            AddCell("AutoAccept", AutoAccept);
            AddCell("CanDelete", CanDelete);
            AddCell("CanTerminate", CanTerminate);
            AddCell("CanJumpInto", CanJumpInto);
            AddCell("TransKind", TransKind);
            AddCell("JoinKind", JoinKind);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 流程标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["PrcID"]; }
            set { this["PrcID"] = value; }
        }

        /// <summary>
        /// 活动名称，同时作为状态名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动
        /// </summary>
        public WfdAtvType Type
        {
            get { return (WfdAtvType)this["Type"]; }
            set { this["Type"] = value; }
        }

        /// <summary>
        /// 执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户
        /// </summary>
        public WfdAtvExecScope ExecScope
        {
            get { return (WfdAtvExecScope)this["ExecScope"]; }
            set { this["ExecScope"] = value; }
        }

        /// <summary>
        /// 执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者
        /// </summary>
        public WfdAtvExecLimit ExecLimit
        {
            get { return (WfdAtvExecLimit)this["ExecLimit"]; }
            set { this["ExecLimit"] = value; }
        }

        /// <summary>
        /// 在执行者限制为3或4时选择的活动
        /// </summary>
        public long? ExecAtvID
        {
            get { return (long?)this["ExecAtvID"]; }
            set { this["ExecAtvID"] = value; }
        }

        /// <summary>
        /// 是否自动签收，打开工作流视图时自动签收工作项
        /// </summary>
        public bool AutoAccept
        {
            get { return (bool)this["AutoAccept"]; }
            set { this["AutoAccept"] = value; }
        }

        /// <summary>
        /// 能否删除流程实例和业务数据，0否 1
        /// </summary>
        public bool CanDelete
        {
            get { return (bool)this["CanDelete"]; }
            set { this["CanDelete"] = value; }
        }

        /// <summary>
        /// 能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能
        /// </summary>
        public bool CanTerminate
        {
            get { return (bool)this["CanTerminate"]; }
            set { this["CanTerminate"] = value; }
        }

        /// <summary>
        /// 是否可作为跳转目标，0不可跳转 1可以
        /// </summary>
        public bool CanJumpInto
        {
            get { return (bool)this["CanJumpInto"]; }
            set { this["CanJumpInto"] = value; }
        }

        /// <summary>
        /// 当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择
        /// </summary>
        public WfdAtvTransKind TransKind
        {
            get { return (WfdAtvTransKind)this["TransKind"]; }
            set { this["TransKind"] = value; }
        }

        /// <summary>
        /// 同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步
        /// </summary>
        public WfdAtvJoinKind JoinKind
        {
            get { return (WfdAtvJoinKind)this["JoinKind"]; }
            set { this["JoinKind"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }
        #endregion
    }
    #endregion
}