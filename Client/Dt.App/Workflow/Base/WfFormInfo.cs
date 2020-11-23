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
using System.Reflection;
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

        #region 属性
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
        /// 获取设置是否自动发送
        /// </summary>
        public bool AutoSend { get; set; }

        /// <summary>
        /// 流程表单窗口
        /// </summary>
        internal IWfForm Form { get; set; }

        /// <summary>
        /// 表单窗口类型
        /// </summary>
        internal Type FormType { get; private set; }
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

            mi = new Mi { ID = "签收", Icon = Icons.锁卡 };
            mi.Click += (s, e) => ToggleAccept();
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

            // 合并IsDirty属性
            CmdSave.AllowExecute = p_fv.IsDirty;
            p_fv.Dirty += (s, b) => CmdSave.AllowExecute = b;
        }
        #endregion

        #region 命令方法
        async Task<bool> Save()
        {
            // 先保存表单数据
            if (!await Form.Save())
                return false;

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

        async void Send()
        {
            // 先保存
            if (!await Save())
                return;

            // 判断当前活动是否结束（需要多人同时完成该活动的情况）
            if (!await IsAtvFinished())
            {
                // 活动未结束（不是最后一人），只结束当前工作项
                await FinishCurItem(false);
                return;
            }

            // 获取所有后续活动的接收者列表
            Dict dt = new Dict(); // await AtWf.GetNextRecvs(_atvDef.ID, _prcInst.ID);

            // 无后续活动
            if (dt == null)
            {
                // 结束当前工作项和活动
                await FinishCurItem(true);
                return;
            }
        }

        void Rollback()
        {

        }

        void ToggleAccept()
        {

        }

        void OpenLog()
        {

        }

        void Delete()
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
                Status: 0);

            AtvInst = new WfiAtv(
                ID: await AtCm.NewID(),
                PrciID: PrcInst.ID,
                AtvdID: AtvDef.ID,
                Status: 0,
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

        /// <summary>
        /// 将当前工作项置完成状态
        /// </summary>
        /// <param name="p_isFinishedAtv">活动是否完成</param>
        /// <returns></returns>
        async Task FinishCurItem(bool p_isFinishedAtv)
        {
            DateTime time = AtSys.Now;

            if (p_isFinishedAtv)
            {
                // 当前活动已完成
                AtvInst.Status = WfiAtvStatus.Finish;
                AtvInst.Mtime = time;
            }

            WorkItem.Status = WfiItemStatus.Finish;
            WorkItem.Mtime = time;
            WorkItem.UserID = AtUser.ID;

            List<object> ls = new List<object>();
            if (AtvInst.IsChanged)
                ls.Add(AtvInst);
            ls.Add(WorkItem);
            if (await AtCm.BatchSave(ls, false))
            {
                AtKit.Msg(p_isFinishedAtv ? "任务结束" : "当前工作项完成");
                CloseWin();
            }
            else
            {
                AtKit.Warn("工作项保存失败");
            }
        }

        /// <summary>
        /// 判断当前发送者是否为当前活动的最后一个发送者
        /// </summary>
        /// <returns></returns>
        async Task<bool> IsAtvFinished()
        {
            int count = await AtCm.GetScalar<int>("流程-工作项个数", new { atviid = AtvInst.ID });
            return (count + 1) >= AtvInst.InstCount;
        }

        void CloseWin()
        {
            ((Win)Form).Close();
        }

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
