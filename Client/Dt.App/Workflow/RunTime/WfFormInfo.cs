#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.App.Workflow;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 流程描述信息，加载流程表单的参数
    /// </summary>
    public class WfFormInfo
    {
        #region 成员变量
        static readonly Dictionary<long, WfdPrc> _prcDefs = new Dictionary<long, WfdPrc>();
        long _prcID;
        long _itemID;
        bool _locked;
        #endregion

        #region 构造方法
        public WfFormInfo(long p_prcID, long p_itemID)
        {
            _prcID = p_prcID;
            _itemID = p_itemID;

            CmdSave = new WfSaveCmd(this);
            CmdSend = new WfSendCmd(this);
            CmdRollback = new WfRollbackCmd(this);
            CmdAccept = new WfAcceptCmd(this);
            CmdDelete = new WfDeleteCmd(this);
            CmdLog = new WfLogCmd(this);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 发送前事件，用于外部自定义执行者范围
        /// </summary>
        public EventHandler<WfSendingArgs> Sending;

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前状态名称（即活动名称）
        /// </summary>
        public string State
        {
            get { return AtvDef.Name; }
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
        /// 获取流程模板定义
        /// </summary>
        public WfdPrc PrcDef { get; private set; }

        /// <summary>
        /// 获取当前活动定义
        /// </summary>
        public WfdAtv AtvDef { get; private set; }

        /// <summary>
        /// 获取流程实例
        /// </summary>
        public WfiPrc PrcInst { get; private set; }

        /// <summary>
        /// 获取当前活动实例
        /// </summary>
        public WfiAtv AtvInst { get; private set; }

        /// <summary>
        /// 获取当前工作项
        /// </summary>
        public WfiItem WorkItem { get; private set; }

        /// <summary>
        /// 获取设置表单是否只读
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 获取是否为回退活动
        /// </summary>
        public bool IsRollback
        {
            get { return WorkItem != null && WorkItem.AssignKind == WfiItemAssignKind.Rollback; }
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
        internal IWfForm Form { get; set; }

        /// <summary>
        /// 表单窗口类型
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
        public WfSendCmd CmdSend { get; }

        /// <summary>
        /// 回退命令
        /// </summary>
        public WfRollbackCmd CmdRollback { get; }

        /// <summary>
        /// 签收/取消签收命令
        /// </summary>
        public WfAcceptCmd CmdAccept { get; }

        /// <summary>
        /// 保存表单命令
        /// </summary>
        public WfSaveCmd CmdSave { get; }

        /// <summary>
        /// 删除流程实例命令
        /// </summary>
        public WfDeleteCmd CmdDelete { get; }

        /// <summary>
        /// 查看日志(流程图)命令
        /// </summary>
        public WfLogCmd CmdLog { get; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取默认菜单，自动绑定命令
        /// </summary>
        /// <param name="p_fv"></param>
        /// <returns></returns>
        public Menu GetDefaultMenu(Fv p_fv)
        {
            if (IsReadOnly)
                return null;

            Menu m = new Menu();
            m.Items.Add(new Mi { ID = "发送", Icon = Icons.发出, Cmd = CmdSend });

            // 合并IsDirty属性
            CmdSave.AllowExecute = p_fv.IsDirty;
            p_fv.Dirty += (s, b) => CmdSave.AllowExecute = b;
            m.Items.Add(new Mi { ID = "保存", Icon = Icons.保存, Cmd = CmdSave });
            m.Items.Add(new Mi { ID = "撤消", Icon = Icons.撤消, Cmd = p_fv.CmdUndo });

            if (AtvDef.CanDelete || AtvDef.Type == WfdAtvType.Start)
                m.Items.Add(new Mi { ID = "删除", Icon = Icons.垃圾箱, Cmd = CmdDelete });

            if (!IsStartItem)
            {
                m.Items.Insert(1, new Mi { ID = "回退", Icon = Icons.追回, Cmd = CmdRollback });

                Mi mi = new Mi { ID = "签收", Icon = Icons.锁卡, IsCheckable = true, Cmd = CmdAccept };
                if (WorkItem.IsAccept)
                    mi.IsChecked = true;
                m.Items.Insert(2, mi);

                m.Items.Add(new Mi { ID = "日志", Icon = Icons.审核, Cmd = CmdLog });
            }
            return m;
        }

        /// <summary>
        /// 为菜单项绑定命令
        /// </summary>
        /// <param name="p_menu"></param>
        /// <param name="p_fv"></param>
        public void ApplyMenuCmd(Menu p_menu, Fv p_fv)
        {
            if (IsReadOnly)
            {
                p_menu.Visibility = Visibility.Collapsed;
                return;
            }

            if (IsStartItem)
            {
                // 隐藏无用的项
                foreach (var mi in p_menu.Items)
                {
                    var exp = mi.GetBindingExpression(Mi.CmdProperty);
                    if (exp != null)
                    {
                        var path = exp.ParentBinding.Path.Path;
                        if (path == "CmdRollback"
                            || path == "CmdAccept"
                            || path == "CmdLog")
                        {
                            mi.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            else
            {
                // 整理菜单项
                foreach (var mi in p_menu.Items)
                {
                    var exp = mi.GetBindingExpression(Mi.CmdProperty);
                    if (exp != null)
                    {
                        var path = exp.ParentBinding.Path.Path;
                        if (path == "CmdAccept")
                        {
                            if (!mi.IsCheckable)
                                mi.IsCheckable = true;
                            mi.IsChecked = WorkItem.IsAccept;
                        }
                        else if (path == "CmdDelete")
                        {
                            if (!AtvDef.CanDelete)
                                mi.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            // 合并IsDirty属性
            CmdSave.AllowExecute = p_fv.IsDirty;
            p_fv.Dirty += (s, b) => CmdSave.AllowExecute = b;

            p_menu.DataContext = this;
        }

        internal async void RunCmd(Func<Task> p_func)
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                await p_func();
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
        internal async Task<WfiTrs> CreateAtvTrs(long p_tatvid, long p_tatviid, DateTime p_date, bool p_rollback)
        {
            Dict dt = new Dict();
            dt["prcid"] = PrcInst.PrcdID;
            dt["SrcAtvID"] = AtvInst.AtvdID;
            dt["TgtAtvID"] = p_tatvid;
            dt["IsRollback"] = p_rollback;
            long trsdid = await AtCm.GetScalar<long>("流程-迁移模板ID", dt);

            // 存在同步
            if (trsdid == 0)
            {
                var sync = (from ar in NextRecvs
                            where ar.Def.Type == WfdAtvType.Sync
                            select ar.Def).FirstOrDefault();
                if (sync != null)
                {
                    dt["SrcAtvID"] = sync.ID;
                    trsdid = await AtCm.GetScalar<long>("流程-迁移模板ID", dt);
                }
            }

            return new WfiTrs(
                ID: await AtCm.NewID(),
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

        internal void CloseWin()
        {
            ((Win)Form).Close();
        }
        #endregion

        #region 初始化
        internal async Task Init()
        {
            // 加载流程定义
            if (_prcDefs.TryGetValue(_prcID, out var def))
            {
                PrcDef = def;
            }
            else
            {
                PrcDef = await AtCm.GetByID<WfdPrc>(_prcID);
                _prcDefs[_prcID] = PrcDef;
            }

            Throw.IfNullOrEmpty(PrcDef.FormType, "流程定义中未设置表单窗口类型！");
            FormType = Type.GetType(PrcDef.FormType);
            Throw.IfNull(FormType, $"表单窗口类型[{PrcDef.FormType}]不存在！");
            if (FormType.GetInterface("IWfForm") != typeof(IWfForm))
                Throw.Msg("流程表单窗口需要实现IWfForm接口！");

            // 加载活动定义、流程实例、活动实例、工作项
            if (_itemID < 0)
                await CreateWorkItem();
            else
                await LoadWorkItem();

            // 自动签收
            if (_itemID > 0
                && !IsReadOnly
                && AtvDef.AutoAccept
                && !WorkItem.IsAccept)
            {
                WorkItem.IsAccept = true;
                WorkItem.AcceptTime = AtSys.Now;
                if (await AtCm.Save(WorkItem))
                    AtKit.Msg("已自动签收！");
            }
        }

        async Task CreateWorkItem()
        {
            AtvDef = await AtCm.First<WfdAtv>("流程-起始活动", new { prcid = _prcID });

            PrcInst = new WfiPrc(
                ID: await AtCm.NewID(),
                PrcdID: _prcID,
                Name: PrcDef.Name);

            AtvInst = new WfiAtv(
                ID: await AtCm.NewID(),
                PrciID: PrcInst.ID,
                AtvdID: AtvDef.ID,
                InstCount: 1);

            WorkItem = new WfiItem(
                ID: await AtCm.NewID(),
                AtviID: AtvInst.ID,
                AssignKind: WfiItemAssignKind.Start,
                IsAccept: true,
                Status: WfiItemStatus.Active,
                UserID: AtUser.ID,
                Sender: AtUser.Name);
        }

        async Task LoadWorkItem()
        {
            Dict dt = new Dict { { "itemid", _itemID } };
            PrcInst = await AtCm.First<WfiPrc>("流程-工作项的流程实例", dt);
            AtvInst = await AtCm.First<WfiAtv>("流程-工作项的活动实例", dt);
            WorkItem = await AtCm.GetByID<WfiItem>(_itemID);
            AtvDef = await AtCm.GetByID<WfdAtv>(AtvInst.AtvdID);
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
