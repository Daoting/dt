#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.App.Workflow
{
    public partial class WorkflowDesign : Win
    {
        #region 成员变量
        readonly Repo<WfdPrc> _repoPrc = new Repo<WfdPrc>();
        readonly Repo<WfdAtv> _repoAtv = new Repo<WfdAtv>();
        readonly Repo<WfdTrs> _repoTrs = new Repo<WfdTrs>();
        readonly Repo<WfdAtvrole> _repoAtvRole = new Repo<WfdAtvrole>();

        WfdPrc _prc;
        Table<WfdAtv> _atvs;
        Table<WfdTrs> _trss;
        Table<WfdAtvrole> _atvRoles;

        WfInsertMenu _insertMenu;
        AlignMenu _alignMenu;
        WfAtvForm _atvForm;
        WfStartAtvForm _startAtvForm;
        WfSyncAtvForm _syncAtvForm;
        WfEndAtvForm _endAtvForm;
        WfTrsForm _trsForm;
        WfTextForm _txtForm;
        #endregion

        public WorkflowDesign(long p_prcID)
        {
            InitializeComponent();

            if (p_prcID < 0)
                CreateNewPrc();
            else
                LoadPrc(p_prcID);
            AttachSketch();
        }

        async void OnAdd(object sender, Mi e)
        {
            if (await CancelSave())
                CreateNewPrc();
        }

        async void CreateNewPrc()
        {
            _prc = new WfdPrc(
                ID: await AtCm.NewID(),
                Name: "新流程");
            _sketch.ReadXml("");
        }

        async void LoadPrc(long p_prcID)
        {
            var dt = new Dict { { "prcid", p_prcID } };
            _prc = await _repoPrc.Get("流程-编辑流程模板", dt);
            _atvs = await _repoAtv.Query("流程-编辑活动模板", dt);
            _trss = await _repoTrs.Query("流程-编辑迁移模板", dt);
            _atvRoles = await _repoAtvRole.Query("流程-编辑活动授权", dt);

            _fv.Data = _prc;
            _sketch.ReadXml(_prc.Diagram);
        }


        #region 图元事件
        void AttachSketch()
        {
            _sketch.Added += OnSketchAdded;
            _sketch.Deleted += OnSketchDeleted;
            _sketch.Tapped += OnSketchTapped;
            _sketch.RightTapped += OnSketchRightTapped;
        }

        void OnSketchAdded(object sender, List<FrameworkElement> e)
        {
            foreach (var item in e)
            {
                if (item is SNode node)
                {
                    if (item.Tag == null)
                    {
                        AtvType tp;
                        switch (node.Shape)
                        {
                            case "开始":
                                tp = AtvType.Start;
                                break;
                            case "同步":
                                tp = AtvType.Sync;
                                break;
                            case "结束":
                                tp = AtvType.Finish;
                                break;
                            default:
                                tp = AtvType.Normal;
                                break;
                        }

                        WfdAtv atv = new WfdAtv(
                            ID: node.ID,
                            PrcID: _prc.ID,
                            Name: node.Title,
                            Type: (byte)tp,
                            ExecScope: 0,
                            ExecLimit: 0,
                            AutoAccept: true,
                            CanDelete: (tp == AtvType.Start),
                            CanTerminate: false,
                            CanJumpInto: false,
                            JoinKind: 0,
                            TransKind: 0,
                            Ctime: AtSys.Now);
                        atv.IsChanged = true;

                        node.Tag = atv;
                        _atvs.Add(atv);
                    }
                    else
                    {
                        // 说明是撤销
                        _atvs.Add((WfdAtv)node.Tag);
                    }
                }
                else if (item is SLine line)
                {
                    if (item.Tag == null)
                    {
                        WfdTrs trs = new WfdTrs(
                            ID: line.ID,
                            PrcID: _prc.ID,
                            SrcAtvID: line.HeaderID,
                            TgtAtvID: line.TailID,
                            Type: 0);
                        trs.IsChanged = true;

                        line.Tag = trs;
                        _trss.Add(trs);
                    }
                    else
                    {
                        _trss.Add((WfdTrs)item.Tag);
                    }
                    if (e.Count == 1)
                        _sketch.SelectionClerk.SelectLine(line);
                }
            }

            if (_sketch.SelectedNodes.Count == 1)
            {
                if (_sketch.SelectedNodes[0] is SNode node)
                    LoadAtvForm(node);
                else if (_sketch.SelectedNodes[0] is TextBlock tb)
                    LoadTextForm(tb);
            }
            else if (_sketch.SelectedLine != null)
            {
                LoadTrsForm(_sketch.SelectedLine);
            }
            else
            {
                _tab.Content = null;
            }
        }

        void OnSketchDeleted(object sender, List<FrameworkElement> e)
        {
            foreach (var item in e)
            {
                if (item is SNode node)
                {
                    if (node.Tag is WfdAtv atv)
                        _atvs.Remove(atv);
                }
                else if (item is SLine line)
                {
                    if (line.Tag is WfdTrs trs)
                        _trss.Remove(trs);
                }
            }
            _tab.Content = null;
        }

        void OnSketchTapped(object sender, TappedRoutedEventArgs e)
        {
            if (_sketch.SelectedLine != null)
            {
                LoadTrsForm(_sketch.SelectedLine);
            }
            else if (_sketch.SelectedNodes.Count == 1)
            {
                if (_sketch.SelectedNodes[0] is SNode node)
                    LoadAtvForm(node);
                else if (_sketch.SelectedNodes[0] is TextBlock tb)
                    LoadTextForm(tb);
                else
                    _tab.Content = null;
            }
            else
            {
                _tab.Content = null;
            }
        }

        void OnSketchRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Menu menu = null;
            if (_sketch.SelectedLine != null)
            {
                LoadTrsForm(_sketch.SelectedLine);
            }
            else if (_sketch.SelectedNodes.Count > 1)
            {
                _tab.Content = null;
                var pt = e.GetPosition(_sketch.Container);
                FrameworkElement elem = _sketch.GetItemByPosition(pt);
                if (elem != null)
                {
                    // 多选且左其中一元素点击时显示对齐选择
                    if (_alignMenu == null)
                        _alignMenu = new AlignMenu(_sketch);
                    _alignMenu.OnAlign(elem);
                    menu = _alignMenu;
                }
            }
            else if (_sketch.SelectedNodes.Count == 1)
            {
                if (_sketch.SelectedNodes[0] is SNode node)
                    LoadAtvForm(node);
                else if (_sketch.SelectedNodes[0] is TextBlock tb)
                    LoadTextForm(tb);
                else
                    _tab.Content = null;
            }
            else
            {
                _tab.Content = null;
                if (_insertMenu == null)
                    _insertMenu = new WfInsertMenu(_sketch);
                menu = _insertMenu;
            }

            if (menu != null)
                _ = menu.OpenContextMenu(e.GetPosition(null));
        }
        #endregion

        #region 属性表单
        /// <summary>
        /// 加载活动表单
        /// </summary>
        /// <param name="p_node"></param>
        void LoadAtvForm(SNode p_node)
        {
            var data = p_node.Tag as WfdAtv;
            if (data == null)
            {
                _tab.Content = null;
                return;
            }

            switch (data.Type)
            {
                case 1:
                    // 开始活动
                    if (_startAtvForm == null)
                        _startAtvForm = new WfStartAtvForm();
                    _startAtvForm.LoadNode(p_node, _atvRoles);
                    _tab.Content = _startAtvForm;
                    break;
                case 2:
                    // 同步活动
                    if (_syncAtvForm == null)
                        _syncAtvForm = new WfSyncAtvForm();
                    _syncAtvForm.LoadNode(p_node);
                    _tab.Content = _syncAtvForm;
                    break;
                case 3:
                    // 结束活动
                    if (_endAtvForm == null)
                        _endAtvForm = new WfEndAtvForm();
                    _endAtvForm.LoadNode(p_node);
                    _tab.Content = _endAtvForm;
                    break;
                default:
                    // 普通活动
                    if (_atvForm == null)
                        _atvForm = new WfAtvForm();
                    _atvForm.LoadNode(p_node, _atvRoles);
                    _tab.Content = _atvForm;
                    break;
            }
        }

        /// <summary>
        /// 加载迁移表单
        /// </summary>
        /// <param name="p_line"></param>
        void LoadTrsForm(SLine p_line)
        {
            var data = p_line.Tag as WfdTrs;
            if (data == null)
            {
                _tab.Content = null;
                return;
            }

            if (_trsForm == null)
                _trsForm = new WfTrsForm();
            _trsForm.LoadData(_trss, data);
            _tab.Content = _trsForm;
        }

        /// <summary>
        /// 加载TextBlock属性编辑
        /// </summary>
        /// <param name="p_tb"></param>
        void LoadTextForm(TextBlock p_tb)
        {
            if (_txtForm == null)
                _txtForm = new WfTextForm();
            _txtForm.LoadNode(p_tb);
            _tab.Content = _txtForm;
        }
        #endregion

        void OnSave(object sender, Mi e)
        {

        }

        protected override Task<bool> OnClosing()
        {
            return CancelSave();
        }

        Task<bool> CancelSave()
        {
            //if (_prc.IsChanged)
            //{
            //    return AtKit.Confirm("数据已修改，确认要放弃修改吗？");
            //}
            return Task.FromResult(true);
        }

        void OnDel(object sender, Mi e)
        {

        }
    }
}