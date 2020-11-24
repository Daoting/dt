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
using Windows.UI.Xaml.Data;
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

        BaseCommand _cmdSend;
        BaseCommand _cmdSave;
        BaseCommand _cmdRollback;
        BaseCommand _cmdLog;
        BaseCommand _cmdAccept;
        BaseCommand _cmdDelete;
        #endregion

        #region 构造方法
        public WfFormInfo(long p_prcID, long p_itemID)
        {
            _prcID = p_prcID;
            _itemID = p_itemID;
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
        /// 获取设置是否自动发送
        /// </summary>
        public bool AutoSend { get; set; }
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
        internal AtvRecvs NextRecvs { get; private set; }
        #endregion

        #region 命令
        /// <summary>
        /// 发送命令
        /// </summary>
        public BaseCommand CmdSend
        {
            get
            {
                if (_cmdSend == null)
                    _cmdSend = new BaseCommand((p_params) => Send());
                return _cmdSend;
            }
        }

        /// <summary>
        /// 回退命令
        /// </summary>
        public BaseCommand CmdRollback
        {
            get
            {
                if (_cmdRollback == null)
                    _cmdRollback = new BaseCommand((p_params) => Rollback());
                return _cmdRollback;
            }
        }

        /// <summary>
        /// 签收/取消签收命令
        /// </summary>
        public BaseCommand CmdAccept
        {
            get
            {
                if (_cmdAccept == null)
                    _cmdAccept = new BaseCommand((p_params) => ToggleAccept());
                return _cmdAccept;
            }
        }

        /// <summary>
        /// 保存表单命令
        /// </summary>
        public BaseCommand CmdSave
        {
            get
            {
                if (_cmdSave == null)
                    _cmdSave = new BaseCommand((p_params) => Save());
                return _cmdSave;
            }
        }

        /// <summary>
        /// 删除流程实例命令
        /// </summary>
        public BaseCommand CmdDelete
        {
            get
            {
                if (_cmdDelete == null)
                    _cmdDelete = new BaseCommand((p_params) => Delete());
                return _cmdDelete;
            }
        }

        /// <summary>
        /// 查看日志(流程图)命令
        /// </summary>
        public BaseCommand CmdLog
        {
            get
            {
                if (_cmdLog == null)
                    _cmdLog = new BaseCommand((p_params) => { OpenLog(); });
                return _cmdLog;
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取默认菜单，自动绑定命令
        /// </summary>
        /// <param name="p_fv"></param>
        /// <returns></returns>
        public Menu GetDefaultMenu(Fv p_fv)
        {
            Menu m = new Menu();
            Mi mi = new Mi { ID = "发送", Icon = Icons.发出 };
            mi.Click += (s, e) => Send();
            m.Items.Add(mi);

            mi = new Mi { ID = "回退", Icon = Icons.追回 };
            mi.Click += (s, e) => Rollback();
            m.Items.Add(mi);

            mi = new Mi { ID = "签收", Icon = Icons.锁卡, IsCheckable = true };
            mi.Click += (s, e) => ToggleAccept();
            if (WorkItem.IsAccept)
                mi.IsChecked = true;
            m.Items.Add(mi);

            mi = new Mi { ID = "保存", Icon = Icons.保存, Cmd = CmdSave };
            Binding bind = new Binding { Path = new PropertyPath("IsDirty"), Source = p_fv };
            mi.SetBinding(Mi.IsEnabledProperty, bind);
            mi.Click += (s, e) => Save();
            m.Items.Add(mi);

            m.Items.Add(new Mi { ID = "撤消", Icon = Icons.撤消, Cmd = p_fv.CmdUndo });

            mi = new Mi { ID = "删除", Icon = Icons.垃圾箱 };
            mi.Click += (s, e) => Delete();
            m.Items.Add(mi);

            mi = new Mi { ID = "日志", Icon = Icons.审核 };
            mi.Click += (s, e) => OpenLog();
            m.Items.Add(mi);
            return m;
        }

        /// <summary>
        /// 为菜单项绑定命令
        /// </summary>
        /// <param name="p_menu"></param>
        /// <param name="p_fv"></param>
        public void ApplyMenuCmd(Menu p_menu, Fv p_fv)
        {
            p_menu.DataContext = this;

            if (WorkItem.IsAccept)
            {
                foreach (var mi in p_menu.Items)
                {
                    // 此时无法获取Cmd的值
                    var exp = mi.GetBindingExpression(Mi.CmdProperty);
                    if (exp.ParentBinding.Path.Path == "CmdAccept")
                    {
                        if (!mi.IsCheckable)
                            mi.IsCheckable = true;
                        mi.IsChecked = true;
                        break;
                    }
                }
            }

            // 合并IsDirty属性
            CmdSave.AllowExecute = p_fv.IsDirty;
            p_fv.Dirty += (s, b) => CmdSave.AllowExecute = b;
        }
        #endregion

        #region 保存
        async void Save()
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                await SaveBody();
            }
            finally
            {
                _locked = false;
            }
        }

        async Task<bool> SaveBody()
        {
            // 先保存表单数据
            if (!await Form.Save())
                return false;

            // 标题
            string name = Form.GetPrcName();
            if (name != PrcInst.Name)
            {
                PrcInst.Name = name;
                ((Win)Form).Title = name;
            }

            bool suc = true;
            if (PrcInst.IsAdded)
            {
                DateTime time = AtSys.Now;
                PrcInst.Ctime = time;
                PrcInst.Mtime = time;
                PrcInst.Dispidx = await AtCm.NewSeq("sq_wfi_prc");

                AtvInst.Ctime = time;
                AtvInst.Mtime = time;

                WorkItem.AcceptTime = time;
                WorkItem.Dispidx = await AtCm.NewSeq("sq_wfi_item");
                WorkItem.Ctime = time;
                WorkItem.Mtime = time;
                WorkItem.Stime = time;
            }

            List<object> ls = new List<object>();
            if (PrcInst.IsAdded || PrcInst.IsChanged)
                ls.Add(PrcInst);
            if (AtvInst.IsAdded || AtvInst.IsChanged)
                ls.Add(AtvInst);
            if (WorkItem.IsAdded || WorkItem.IsChanged)
                ls.Add(WorkItem);

            if (ls.Count > 0)
                suc = await AtCm.BatchSave(ls, false);

            if (suc)
                AtKit.Msg("表单保存成功！");
            else
                AtKit.Warn("表单保存失败！");
            return suc;
        }
        #endregion

        #region 发送
        async void Send()
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                await SendBody();
            }
            finally
            {
                _locked = false;
            }
        }

        async Task SendBody()
        {
            // 先保存
            if (!await SaveBody())
                return;

            await AtvInst.UpdateFinished();
            WorkItem.Finished();

            // 判断当前活动是否结束（需要多人同时完成该活动的情况）
            if (!AtvInst.IsFinished)
            {
                // 活动未结束（不是最后一人），只结束当前工作项
                await SaveWorkItem();
                return;
            }

            // 获取所有后续活动的接收者列表
            Dict dt = await AtWf.GetNextRecvs(AtvDef.ID, PrcInst.ID);

            // 无后续活动
            if (dt == null)
            {
                // 结束当前工作项和活动
                await SaveWorkItem();
                return;
            }

            if (!dt.ContainsKey("manualsend") || !dt.ContainsKey("atv"))
            {
                AtKit.Warn("加载后续活动信息失败！");
                return;
            }

            // 构造数据
            NextRecvs = new AtvRecvs();
            foreach (var item in (Dict)dt["atv"])
            {
                AtvRecv ar = new AtvRecv();
                ar.Def = await AtCm.GetByID<WfdAtv>(item.Key);
                Dict dtRecv = item.Value as Dict;
                if (dtRecv != null)
                {
                    ar.IsRole = (bool)dtRecv["isrole"];
                    ar.Recvs = (Table)dtRecv["data"];
                }
                NextRecvs.Add(ar);
            }

            // 触发外部自定义执行者范围事件
            if (Sending != null)
            {
                WfSendingArgs args = new WfSendingArgs(NextRecvs);
                Sending(this, args);
                await args.EnsureAllCompleted();
            }

            if (NextRecvs != null && NextRecvs.Count > 0)
            {
                // 手动选择后发送
                if (dt.Bool("manualsend"))
                    new WfSendDlg().Show(this);
                else
                    DoSend(false);
            }
        }

        /// <summary>
        /// 执行发送
        /// </summary>
        internal async void DoSend(bool p_manualSend)
        {
            #region 后续活动
            // 生成后续活动的活动实例、工作项、迁移实例，一个或多个
            var tblAtvs = Table<WfiAtv>.Create();
            var tblItems = Table<WfiItem>.Create();
            var tblTrs = Table<WfiTrs>.Create();
            DateTime time = AtSys.Now;

            foreach (AtvRecv ar in NextRecvs)
            {
                WfiAtv atvInst = null;
                var atvType = ar.Def.Type;
                if (atvType == WfdAtvType.Normal)
                {
                    // 活动实例
                    atvInst = new WfiAtv(
                        ID: await AtCm.NewID(),
                        PrciID: PrcInst.ID,
                        AtvdID: ar.Def.ID,
                        Status: WfiAtvStatus.Active,
                        Ctime: time,
                        Mtime: time);

                    if (p_manualSend)
                    {
                        // 手动发送，已选择项可能为用户或角色
                        int cnt = 0;
                        foreach (var row in ar.Recvs)
                        {
                            //if (row.IsSelected)
                            {
                                var wi = await WfiItem.Create(atvInst.ID, time, ar.IsRole, row.ID, ar.Note, false);
                                tblItems.Add(wi);
                                cnt++;
                            }
                        }

                        // 无选择项时移除活动实例
                        if (cnt == 0)
                        {
                            atvInst = null;
                        }
                        else
                        {
                            atvInst.InstCount = cnt;
                            tblAtvs.Add(atvInst);
                        }
                    }
                    else
                    {
                        // 自动发送，按角色
                        atvInst.InstCount = ar.Recvs.Count;
                        tblAtvs.Add(atvInst);
                        foreach (var row in ar.Recvs)
                        {
                            var wi = await WfiItem.Create(atvInst.ID, time, ar.IsRole, row.ID, ar.Note, false);
                            tblItems.Add(wi);
                        }
                    }
                }
                else if (atvType == WfdAtvType.Sync)
                {
                    // 同步
                    // 活动实例
                    atvInst = new WfiAtv(
                        ID: await AtCm.NewID(),
                        PrciID: PrcInst.ID,
                        AtvdID: ar.Def.ID,
                        Status: WfiAtvStatus.Sync,
                        InstCount: 1,
                        Ctime: time,
                        Mtime: time);
                    tblAtvs.Add(atvInst);

                    // 工作项
                    WfiItem item = new WfiItem(
                        ID: await AtCm.NewID(),
                        AtviID: atvInst.ID,
                        AssignKind: WfiItemAssignKind.Normal,
                        Status: WfiItemStatus.Sync,
                        IsAccept: false,
                        UserID: AtUser.ID,
                        Sender: AtUser.Name,
                        Stime: time,
                        Ctime: time,
                        Mtime: time,
                        Dispidx: await AtCm.NewSeq("sq_wfi_item"));
                    tblItems.Add(item);
                }
                else if (atvType == WfdAtvType.Finish)
                {
                    // 完成
                    PrcInst.Status = WfiAtvStatus.Finish;
                    PrcInst.Mtime = time;
                }

                // 增加迁移实例
                if (atvInst != null)
                {
                    var trs = await CreateAtvTrs(ar.Def.ID, atvInst.ID, time, false);
                    tblTrs.Add(trs);
                }
            }

            // 发送是否有效
            // 1. 只有'完成'时有效
            // 2. 至少含有一个活动实例时有效
            if (tblAtvs.Count == 0 && PrcInst.Status != WfiAtvStatus.Finish)
            {
                AtKit.Msg("所有后续活动均无接收者，发送失败！");
                return;
            }
            #endregion

            #region 整理待保存数据
            List<object> data = new List<object>();
            if (PrcInst.IsChanged)
                data.Add(PrcInst);
            data.Add(AtvInst);
            data.Add(WorkItem);

            if (tblAtvs.Count > 0)
            {
                data.Add(tblAtvs);
                data.Add(tblItems);
                data.Add(tblTrs);
            }
            #endregion

            if (await AtCm.BatchSave(data, false))
            {
                AtKit.Msg("发送成功！");
                CloseWin();
                // 推送客户端提醒

            }
            else
            {
                AtKit.Warn("发送失败！");
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
        async Task<WfiTrs> CreateAtvTrs(long p_tatvid, long p_tatviid, DateTime p_date, bool p_rollback)
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
                SrcAtviID: AtvInst.AtvdID,
                TgtAtviID: p_tatviid,
                IsRollback: p_rollback,
                Ctime: p_date);
        }

        /// <summary>
        /// 保存当前工作项置完成状态
        /// </summary>
        /// <returns></returns>
        async Task SaveWorkItem()
        {
            List<object> ls = new List<object>();
            if (AtvInst.IsChanged)
                ls.Add(AtvInst);
            ls.Add(WorkItem);

            if (await AtCm.BatchSave(ls, false))
            {
                AtKit.Msg(AtvInst.IsFinished ? "任务结束" : "当前工作项完成");
                CloseWin();
            }
            else
            {
                AtKit.Warn("工作项保存失败");
            }
        }

        void CloseWin()
        {
            ((Win)Form).Close();
        }
        #endregion

        #region 回退
        async void Rollback()
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                await RollbackBody();
            }
            finally
            {
                _locked = false;
            }
        }

        async Task RollbackBody()
        {
            // 获得前一活动实例
            var pre = await AtvInst.GetRollbackAtv();
            if (pre == null)
            {
                AtKit.Msg("该活动不允许进行回退！");
                return;
            }

            // 活动执行者多于一人时，不允许进行回退
            if (AtvInst.InstCount > 1)
            {
                AtKit.Msg("该活动执行者多于一人，不允许进行回退！");
                return;
            }

            DateTime time = AtSys.Now;
            var newAtvInst = new WfiAtv(
                ID: await AtCm.NewID(),
                PrciID: PrcInst.ID,
                AtvdID: pre.AtvdID,
                Status: WfiAtvStatus.Active,
                InstCount: 1,
                Ctime: time,
                Mtime: time);

            // 创建迁移实例
            var newTrs = await CreateAtvTrs(pre.AtvdID, newAtvInst.ID, time, true);

            // 全部完成时，将当前活动实例状态置成完成
            await AtvInst.UpdateFinished();

            // 当前活动项置成完成状态
            WorkItem.Finished();

            Dict dict = new Dict();
            dict["name"] = await GetSender();
            long userId = await AtCm.GetScalar<long>("流程-获取用户ID", dict);
            var newItem = await WfiItem.Create(newAtvInst.ID, time, false, userId, null, true);

            List<object> ls = new List<object>();
            if (AtvInst.IsChanged)
                ls.Add(AtvInst);
            ls.Add(WorkItem);
            ls.Add(newAtvInst);
            ls.Add(newItem);
            ls.Add(newTrs);

            if (await AtCm.BatchSave(ls, false))
            {
                AtKit.Msg("回退成功！");
                CloseWin();
            }
            else
            {
                AtKit.Msg("回退失败！");
            }
        }

        async Task<string> GetSender()
        {
            string sender = WorkItem.Sender;
            if (WorkItem.AssignKind == WfiItemAssignKind.Back)
            {
                Dict dt = new Dict();
                dt["prciid"] = AtvInst.PrciID;
                dt["atvdid"] = AtvInst.AtvdID;
                long id = await AtCm.GetScalar<long>("流程-最后已完成活动ID", dt);
                if (id != 0)
                {
                    dt = new Dict();
                    dt["atviid"] = id;
                    sender = await AtCm.GetScalar<string>("流程-活动发送者", dt);
                }
            }
            return sender;
        }
        #endregion

        #region 签收
        async void ToggleAccept()
        {
            if (WorkItem.IsAccept)
            {
                WorkItem.IsAccept = false;
                WorkItem.AcceptTime = null;
                if (await AtCm.Save(WorkItem, false))
                    AtKit.Msg("已取消签收！");
            }
            else
            {
                WorkItem.IsAccept = true;
                WorkItem.AcceptTime = AtSys.Now;
                if (await AtCm.Save(WorkItem, false))
                    AtKit.Msg("已签收！");
            }
        }
        #endregion

        #region 删除
        async void Delete()
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                await DeleteBody();
            }
            finally
            {
                _locked = false;
            }
        }

        async Task DeleteBody()
        {
            if (AtvDef == null ||
                (!AtvDef.CanDelete && AtvDef.Type != WfdAtvType.Start))
            {
                AtKit.Warn("您无权删除当前表单！请回退或发送到其他用户进行删除。");
                return;
            }

            if (!await AtKit.Confirm("确认要删除当前表单吗？删除后表单将不可恢复！"))
                return;


            if (await Form.Delete())
            {
                if (await AtCm.Delete(PrcInst, false))
                    AtKit.Msg("表单删除成功！");
                else
                    AtKit.Warn("表单已删除，未找到待删除的流程实例！");
                CloseWin();
            }
        }
        #endregion

        #region 日志
        void OpenLog()
        {

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
            AtvDef = await AtCm.Get<WfdAtv>("流程-起始活动", new { prcid = _prcID });

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
            PrcInst = await AtCm.Get<WfiPrc>("流程-工作项的流程实例", dt);
            AtvInst = await AtCm.Get<WfiAtv>("流程-工作项的活动实例", dt);
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
