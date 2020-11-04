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
    [Tbl("cm_wfd_atv", "cm")]
    public partial class WfdAtv : Entity
    {
        #region 构造方法
        WfdAtv() { }

        public WfdAtv(
            long ID,
            long PrcID = default,
            string Name = default,
            byte Type = default,
            byte ExecScope = default,
            byte ExecLimit = default,
            long? ExecAtvID = default,
            bool AutoAccept = default,
            bool CanDelete = default,
            bool CanTerminate = default,
            bool CanJumpInto = default,
            byte TransKind = default,
            byte JoinKind = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<long>("PrcID", PrcID);
            AddCell<string>("Name", Name);
            AddCell<byte>("Type", Type);
            AddCell<byte>("ExecScope", ExecScope);
            AddCell<byte>("ExecLimit", ExecLimit);
            AddCell<long?>("ExecAtvID", ExecAtvID);
            AddCell<bool>("AutoAccept", AutoAccept);
            AddCell<bool>("CanDelete", CanDelete);
            AddCell<bool>("CanTerminate", CanTerminate);
            AddCell<bool>("CanJumpInto", CanJumpInto);
            AddCell<byte>("TransKind", TransKind);
            AddCell<byte>("JoinKind", JoinKind);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
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
        /// 活动名称，同时作为状态名称，可重复
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动
        /// </summary>
        public byte Type
        {
            get { return (byte)this["Type"]; }
            set { this["Type"] = value; }
        }

        /// <summary>
        /// 执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户
        /// </summary>
        public byte ExecScope
        {
            get { return (byte)this["ExecScope"]; }
            set { this["ExecScope"] = value; }
        }

        /// <summary>
        /// 执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者
        /// </summary>
        public byte ExecLimit
        {
            get { return (byte)this["ExecLimit"]; }
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
        public byte TransKind
        {
            get { return (byte)this["TransKind"]; }
            set { this["TransKind"] = value; }
        }

        /// <summary>
        /// 同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步
        /// </summary>
        public byte JoinKind
        {
            get { return (byte)this["JoinKind"]; }
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

        #region 可复制
        /*
        void OnSaving()
        {
        }

        void OnDeleting()
        {
        }

        void SetID(long p_value)
        {
        }

        void SetPrcID(long p_value)
        {
        }

        void SetName(string p_value)
        {
        }

        void SetType(byte p_value)
        {
        }

        void SetExecScope(byte p_value)
        {
        }

        void SetExecLimit(byte p_value)
        {
        }

        void SetExecAtvID(long? p_value)
        {
        }

        void SetAutoAccept(bool p_value)
        {
        }

        void SetCanDelete(bool p_value)
        {
        }

        void SetCanTerminate(bool p_value)
        {
        }

        void SetCanJumpInto(bool p_value)
        {
        }

        void SetTransKind(byte p_value)
        {
        }

        void SetJoinKind(byte p_value)
        {
        }

        void SetCtime(DateTime p_value)
        {
        }

        void SetMtime(DateTime p_value)
        {
        }
        */
        #endregion
    }
    #endregion
}