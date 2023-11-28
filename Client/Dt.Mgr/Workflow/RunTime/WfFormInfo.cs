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
using Dt.Mgr.Rbac;
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
        bool _locked;

        WfSaveCmd _cmdSave;
        WfSendCmd _cmdSend;
        WfRollbackCmd _cmdRollback;
        WfAcceptCmd _cmdAccept;
        WfDeleteCmd _cmdDelete;
        WfLogCmd _cmdLog;
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
        public string State => AtvDef.Name;

        /// <summary>
        /// 业务数据主键，也是流程实例主键
        /// </summary>
        public long ID => PrcInst.ID;

        /// <summary>
        /// 是否为新表单
        /// </summary>
        public bool IsNew => PrcInst.IsAdded;

        /// <summary>
        /// 表单是否只读状态
        /// </summary>
        public bool IsReadOnly { get; private set; }

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
        /// 流程表单所在窗口
        /// </summary>
        internal Win FormWin { get; set; }

        /// <summary>
        /// 流程表单界面
        /// </summary>
        internal IWfForm Form { get; private set; }

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
        /// <summary>
        /// 记录表单界面
        /// </summary>
        /// <param name="p_form"></param>
        /// <returns></returns>
        public void SetForm(IWfForm p_form)
        {
            Throw.IfNull(p_form, "表单 IWfForm 不可为null");
            Form = p_form;
            p_form.Info = this;
        }

        /// <summary>
        /// 加载默认菜单，自动绑定命令
        /// </summary>
        public async Task<Menu> CreateMenu()
        {
            Menu m = new Menu();
            if (!IsReadOnly)
            {
                m.Items.Add(new Mi { ID = "发送", Icon = Icons.发出, Cmd = CmdSend });

                if (await AllowRollback())
                {
                    m.Items.Add(new Mi { ID = "回退", Icon = Icons.追回, Cmd = CmdRollback });
                }

                if (!IsStartItem)
                {
                    Mi mi = new Mi { ID = "签收", Icon = Icons.锁卡, IsCheckable = true, Cmd = CmdAccept };
                    if (WorkItem.IsAccept)
                        mi.IsChecked = true;
                    m.Items.Add(mi);
                }

                // 合并IsDirty属性
                m.Items.Add(new Mi { ID = "保存", Icon = Icons.保存, Cmd = CmdSave });

                if (AtvDef.CanDelete || AtvDef.Type == WfdAtvType.Start)
                    m.Items.Add(new Mi { ID = "删除", Icon = Icons.垃圾箱, Cmd = CmdDelete });
            }
            return m;
        }
        #endregion

        #region 内部方法
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
        /// <param name="p_prcID">流程标识</param>
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
        /// 加载流程定义
        /// </summary>
        /// <param name="p_prcName"></param>
        /// <returns></returns>
        internal static async Task<WfdPrcX> GetPrcDef(string p_prcName)
        {
            WfdPrcX def = (from pr in _prcDefs
                           where pr.Value.Name == p_prcName
                           select pr.Value).FirstOrDefault();
            if (def == null)
            {
                def = await WfdPrcX.GetByKey("name", p_prcName);
                if (def != null)
                    _prcDefs[def.ID] = def;
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
        /// <summary>
        /// 1. itemID > 0 时，其余两项无效，以当前工作项为标准
        /// 2. prciID > 0 时，以该流程实例的最后工作项为标准
        /// 3. 提供流程名称时，创建新工作项、流程实例、起始活动实例
        /// </summary>
        /// <param name="p_itemID">工作项标识</param>
        /// <param name="p_prciID">流程实例标识</param>
        /// <param name="p_prcName">流程名称</param>
        internal async Task Init(long p_itemID, long p_prciID, string p_prcName)
        {
            if (p_itemID > 0)
            {
                // 当前工作项
                WorkItem = await WfiItemX.GetByID(p_itemID);
                AtvInst = await WfiAtvX.First($"where id={WorkItem.AtviID}");
                AtvDef = await WfdAtvX.GetByID(AtvInst.AtvdID);
                PrcInst = await WfiPrcX.First($"where id={AtvInst.PrciID}");
                PrcDef = await GetPrcDef(PrcInst.PrcdID);
            }
            else if (p_prciID > 0)
            {
                // 根据流程实例id获取最后工作项
                WorkItem = await WfiItemX.GetLastItem(p_prciID);
                AtvInst = await WfiAtvX.First($"where id={WorkItem.AtviID}");
                AtvDef = await WfdAtvX.GetByID(AtvInst.AtvdID);
                PrcInst = await WfiPrcX.First($"where id={p_prciID}");
                PrcDef = await GetPrcDef(PrcInst.PrcdID);
            }
            else if (!string.IsNullOrEmpty(p_prcName))
            {
                // 创建新工作项、流程实例、起始活动实例
                PrcDef = await GetPrcDef(p_prcName);
                AtvDef = await WfdAtvX.First($"where prc_id={PrcDef.ID} and type=1");

                PrcInst = await WfiPrcX.New(
                    PrcdID: PrcDef.ID,
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
            else
            {
                Throw.Msg("WfFormInfo 实例的所有标识都无效！");
            }

            FormType = Kit.GetTypeByAlias(typeof(WfFormAttribute), PrcDef.Name);
            Throw.IfNull(FormType, $"未指定流程表单类型，请在流程表单类型上添加 [WfForm(\"{PrcDef.Name}\")] 标签！");
            if (!FormType.IsSubclassOf(typeof(Win)) && FormType.GetInterface("IWfForm") != typeof(IWfForm))
                Throw.Msg("任务表单类型需要继承自IWfForm！");

            // 确定表单是否可编辑
            if (WorkItem.Status == WfiItemStatus.活动
                && (WorkItem.UserID == Kit.UserID
                    || (WorkItem.RoleID.HasValue && await RbacDs.ExistsUserRole(Kit.UserID, WorkItem.RoleID.Value))))
            {
                IsReadOnly = false;
            }
            else
            {
                IsReadOnly = true;
            }

            // 自动签收
            if (!IsNew
                && !IsReadOnly
                && AtvDef.AutoAccept
                && !WorkItem.IsAccept)
            {
                WorkItem.IsAccept = true;
                WorkItem.AcceptTime = Kit.Now;
                if (await WorkItem.Save(false))
                    Kit.Msg("已自动签收！");
            }
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
            if (obj is WfFormInfo info)
                return info.WorkItem.ID == WorkItem.ID;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
