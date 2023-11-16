#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Mgr.Workflow;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 流程描述信息，加载流程表单的参数
    /// </summary>
    public class WfFormInfo
    {
        #region 成员变量
        static readonly Dictionary<long, WfdPrcX> _prcDefs = new Dictionary<long, WfdPrcX>();
        long _prcID;
        long _prciID;
        long _itemID;
        bool _locked;

        WfSaveCmd _cmdSave;
        WfSendCmd _cmdSend;
        WfRollbackCmd _cmdRollback;
        WfAcceptCmd _cmdAccept;
        WfDeleteCmd _cmdDelete;
        WfLogCmd _cmdLog;
        #endregion

        #region 构造方法
        public WfFormInfo(long p_prcID, long p_itemID, WfFormUsage p_usage)
        {
            _prcID = p_prcID;
            _itemID = p_itemID;
            _prciID = -1;
            Usage = p_usage;
        }

        public WfFormInfo(long p_prciID, WfFormUsage p_usage = WfFormUsage.Read)
        {
            // 流程模板id 和 最后工作项id根据流程实例id查询
            _prciID = p_prciID;
            _prcID = -1;
            _itemID = -1;
            Usage = p_usage;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 发送前事件，用于外部自定义执行者范围
        /// </summary>
        public EventHandler<WfSendingArgs> Sending;

        /// <summary>
        /// 表单关闭事件
        /// </summary>
        public EventHandler FormClosed;
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前状态名称（即活动名称）
        /// </summary>
        public string State
        {
            get
            {
                // 管理时无状态
                if (Usage == WfFormUsage.Manage)
                    return null;
                return AtvDef.Name;
            }
        }

        /// <summary>
        /// 业务数据主键，也是流程实例主键
        /// </summary>
        public long ID
        {
            get { return PrcInst.ID; }
        }

        /// <summary>
        /// 是否为新表单
        /// </summary>
        public bool IsNew
        {
            get { return _itemID < 0; }
        }

        /// <summary>
        /// 获取设置表单的使用场景
        /// </summary>
        public WfFormUsage Usage { get; }

        /// <summary>
        /// 获取表单菜单
        /// </summary>
        public Menu Menu { get; internal set; }

        /// <summary>
        /// 获取流程模板定义
        /// </summary>
        public WfdPrcX PrcDef { get; private set; }

        /// <summary>
        /// 获取当前活动定义
        /// </summary>
        public WfdAtvX AtvDef { get; private set; }

        /// <summary>
        /// 获取流程实例
        /// </summary>
        public WfiPrcX PrcInst { get; private set; }

        /// <summary>
        /// 获取当前活动实例
        /// </summary>
        public WfiAtvX AtvInst { get; private set; }

        /// <summary>
        /// 获取当前工作项
        /// </summary>
        public WfiItemX WorkItem { get; private set; }

        /// <summary>
        /// 获取是否为回退活动
        /// </summary>
        public bool IsRollback
        {
            get { return WorkItem != null && WorkItem.AssignKind == WfiItemAssignKind.追回; }
        }

        /// <summary>
        /// 是否为开始活动工作项
        /// </summary>
        public bool IsStartItem
        {
            get { return AtvDef.Type == WfdAtvType.Start; }
        }
        #endregion

        #region 内部属性
        /// <summary>
        /// 流程表单窗口
        /// </summary>
        internal WfFormWin FormWin { get; set; }

        /// <summary>
        /// 表单类型
        /// </summary>
        internal Type FormType { get; private set; }

        /// <summary>
        /// 获取后续活动列表
        /// </summary>
        internal AtvRecvs NextRecvs { get; set; }
        #endregion

        #region 命令
        /// <summary>
        /// 发送命令
        /// </summary>
        public WfSendCmd CmdSend
        {
            get
            {
                if (_cmdSend == null)
                    _cmdSend = new WfSendCmd(this);
                return _cmdSend;
            }
        }

        /// <summary>
        /// 回退命令
        /// </summary>
        public WfRollbackCmd CmdRollback
        {
            get
            {
                if (_cmdRollback == null)
                    _cmdRollback = new WfRollbackCmd(this);
                return _cmdRollback;
            }
        }

        /// <summary>
        /// 签收/取消签收命令
        /// </summary>
        public WfAcceptCmd CmdAccept
        {
            get
            {
                if (_cmdAccept == null)
                    _cmdAccept = new WfAcceptCmd(this);
                return _cmdAccept;
            }
        }

        /// <summary>
        /// 保存表单命令
        /// </summary>
        public WfSaveCmd CmdSave
        {
            get
            {
                if (_cmdSave == null)
                    _cmdSave = new WfSaveCmd(this);
                return _cmdSave;
            }
        }

        /// <summary>
        /// 删除流程实例命令
        /// </summary>
        public WfDeleteCmd CmdDelete
        {
            get
            {
                if (_cmdDelete == null)
                    _cmdDelete = new WfDeleteCmd(this);
                return _cmdDelete;
            }
        }

        /// <summary>
        /// 查看日志(流程图)命令
        /// </summary>
        public WfLogCmd CmdLog
        {
            get
            {
                if (_cmdLog == null)
                    _cmdLog = new WfLogCmd(this);
                return _cmdLog;
            }
        }
        #endregion

        #region 外部方法
        internal async void RunCmd(Func<WfFormInfo, Task> p_func)
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                await p_func(this);
            }
            finally
            {
                _locked = false;
            }
        }

        /// <summary>
        /// 创建迁移实例
        /// </summary>
        /// <param name="p_tatvid">目标活动模板标识</param>
        /// <param name="p_tatviid">目标活动实例标识</param>
        /// <param name="p_date">创建时间</param>
        /// <param name="p_rollback">是否回退</param>
        /// <returns></returns>
        internal async Task<WfiTrsX> CreateAtvTrs(long p_tatvid, long p_tatviid, DateTime p_date, bool p_rollback)
        {
            long trsdid = await WfdDs.GetWfdTrsID(PrcInst.PrcdID, AtvInst.AtvdID, p_tatvid, p_rollback);
            Throw.If(trsdid == 0, "未找到流程迁移模板");

            return await WfiTrsX.New(
                TrsdID: trsdid,
                SrcAtviID: AtvInst.ID,
                TgtAtviID: p_tatviid,
                IsRollback: p_rollback,
                Ctime: p_date);
        }

        internal Task OnSending()
        {
            if (Sending != null)
            {
                WfSendingArgs args = new WfSendingArgs(NextRecvs);
                Sending(this, args);
                return args.EnsureAllCompleted();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 加载流程定义
        /// </summary>
        /// <param name="p_prcID"></param>
        /// <returns></returns>
        internal static async Task<WfdPrcX> GetPrcDef(long p_prcID)
        {
            WfdPrcX def;
            if (!_prcDefs.TryGetValue(p_prcID, out def))
            {
                def = await WfdPrcX.GetByID(p_prcID);
                _prcDefs[p_prcID] = def;
            }
            return def;
        }

        /// <summary>
        /// 是否允许回退
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> AllowRollback()
        {
            if (AtvDef.Type == WfdAtvType.Start || AtvInst.InstCount > 1)
                return false;
            var pre = await AtvInst.GetRollbackAtv();
            return pre != null;
        }

        internal void CloseWin()
        {
            FormWin.Close();
            FormClosed?.Invoke(FormWin, EventArgs.Empty);
        }
        #endregion

        #region 初始化
        internal async Task Init()
        {
            // 根据流程实例id获取流程id 和 最后工作项id
            if (_prciID > 0)
            {
                var row = await WfiItemX.GetLastItem(_prciID);
                _prcID = row.Long("prcID");
                _itemID = row.Long("itemID");
            }

            // 加载流程定义
            PrcDef = await GetPrcDef(_prcID);

            FormType = Kit.GetTypeByAlias(typeof(WfFormAttribute), PrcDef.Name);
            Throw.IfNull(FormType, $"未指定流程表单类型，请在流程表单类型上添加 [WfForm(\"{PrcDef.Name}\")] 标签！");
            if (FormType.GetInterface("IWfForm") != typeof(IWfForm))
                Throw.Msg("任务表单类型需要继承自IWfForm！");

            // 加载活动定义、流程实例、活动实例、工作项
            if (_itemID < 0)
                await CreateWorkItem();
            else
                await LoadWorkItem();

            // 自动签收
            if (_itemID > 0
                && Usage == WfFormUsage.Edit
                && AtvDef.AutoAccept
                && !WorkItem.IsAccept)
            {
                WorkItem.IsAccept = true;
                WorkItem.AcceptTime = Kit.Now;
                if (await WorkItem.Save())
                    Kit.Msg("已自动签收！");
            }
        }

        async Task CreateWorkItem()
        {
            // 起始活动
            AtvDef = await WfdAtvX.First($"where prc_id={_prcID} and type=1");

            PrcInst = await WfiPrcX.New(
                PrcdID: _prcID,
                Name: PrcDef.Name);

            AtvInst = await WfiAtvX.New(
                PrciID: PrcInst.ID,
                AtvdID: AtvDef.ID,
                InstCount: 1);

            WorkItem = await WfiItemX.New(
                AtviID: AtvInst.ID,
                AssignKind: WfiItemAssignKind.起始指派,
                IsAccept: true,
                Status: WfiItemStatus.活动,
                UserID: Kit.UserID,
                SenderID: Kit.UserID,
                Sender: Kit.UserName);
        }

        async Task LoadWorkItem()
        {
            PrcInst = await WfiPrcX.First($"where id=(select prci_id from cm_wfi_atv where id=(select atvi_id from cm_wfi_item where id={_itemID}))");
            AtvInst = await WfiAtvX.First($"where id=(select atvi_id from cm_wfi_item where id={_itemID})");
            WorkItem = await WfiItemX.GetByID(_itemID);
            AtvDef = await WfdAtvX.GetByID(AtvInst.AtvdID);
        }
        #endregion

        #region 比较
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is WfFormInfo))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            // 只比较标识，识别窗口用
            WfFormInfo info = (WfFormInfo)obj;
            return info._itemID == _itemID && info._prcID == _prcID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
