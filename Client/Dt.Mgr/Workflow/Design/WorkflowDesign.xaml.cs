﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class WorkflowDesign : Win
    {
        #region 成员变量
        WfdPrcX _prc;

        WfInsertMenu _insertMenu;
        AlignMenu _alignMenu;
        WfDeleteMenu _delMenu;
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

            LoadPrc(p_prcID);
            AttachSketch();

            _menu["保存"].SetBinding(IsEnabledProperty, new Binding { Path = new PropertyPath("IsChanged"), Source = this });
        }

        #region 数据处理
        async void LoadPrc(long p_prcID)
        {
            _prc = (p_prcID < 0) ? await WfdPrcX.New() : await WfdPrcX.Get(p_prcID);

            IsChanged = false;
            _sketch.His.CmdChanged -= OnSketchChanged;

            _fv.Data = _prc;
            _sketch.ReadXml(_prc.Diagram);

            // 绑定所有节点和连线图元
            foreach (var obj in _sketch.Container.Children)
            {
                if (obj is SNode node)
                {
                    node.Tag = (from item in _prc.Atvs.Items
                                where item.ID == node.ID
                                select item).FirstOrDefault();
                }
                else if (obj is SLine line)
                {
                    line.Tag = (from item in _prc.Trss.Items
                                where item.ID == line.ID
                                select item).FirstOrDefault();
                }
            }

            _prc.Modified += UpdateSaveState;
            _sketch.His.Clear();
            _sketch.His.CmdChanged += OnSketchChanged;
        }

        async void OnAdd(Mi e)
        {
            if (await CancelSave())
                LoadPrc(-1);
        }

        void OnSketchChanged(object sender, EventArgs e)
        {
            UpdateSaveState();
        }

        void UpdateSaveState()
        {
            // _prc.Atvs和_prc.Trss集合变化会触发CmdChanged，无需重复判断
            IsChanged = _sketch.His.CanUndo || _prc.IsModified;
        }

        async void OnSave(Mi e)
        {
            DateTime now = Kit.Now;
            _prc.Mtime = now;
            _prc.Diagram = _sketch.WriteXml();

            foreach (var elem in _sketch.Container.Children)
            {
                if (elem is SNode node && node.Tag is WfdAtvX atv)
                {
                    // 标题以属性值为准
                    atv.Name = node.Title;
                    if (atv.IsChanged)
                        atv.Mtime = now;
                }
                else if (elem is SLine line && line.Tag is WfdTrsX trs)
                {
                    // 以最终起点终点标识为准
                    trs.SrcAtvID = line.HeaderID;
                    trs.TgtAtvID = line.TailID;
                }
            }

            if (await WfdDs.SavePrc(_prc))
            {
                _sketch.His.Clear();
                UpdateSaveState();
            }
        }

        protected override Task<bool> OnClosing()
        {
            return CancelSave();
        }

        Task<bool> CancelSave()
        {
            if (IsChanged)
            {
                return Kit.Confirm("数据已修改，确认要放弃修改吗？");
            }
            return Task.FromResult(true);
        }

        async void OnDel(Mi e)
        {
            if (await Kit.Confirm($"确认要删除流程模板[{_prc.Name}]吗？"))
            {
                if (await _prc.Delete())
                    Close();
            }
            else
            {
                Kit.Msg("已取消删除！");
            }
        }
        #endregion

        #region 图元事件
        void AttachSketch()
        {
            _sketch.Added += OnSketchAdded;
            _sketch.Deleted += OnSketchDeleted;
            _sketch.Tapped += OnSketchTapped;
            _sketch.RightTapped += OnSketchRightTapped;
        }

        async void OnSketchAdded(object sender, List<FrameworkElement> e)
        {
            foreach (var item in e)
            {
                if (item is SNode node)
                {
                    if (item.Tag == null)
                    {
                        // 新增
                        WfdAtvType tp;
                        switch (node.Shape)
                        {
                            case "开始":
                                tp = WfdAtvType.Start;
                                break;
                            case "同步":
                                tp = WfdAtvType.Sync;
                                break;
                            case "结束":
                                tp = WfdAtvType.Finish;
                                break;
                            default:
                                tp = WfdAtvType.Normal;
                                break;
                        }

                        WfdAtvX atv = new WfdAtvX(
                            ID: node.ID,
                            PrcID: _prc.ID,
                            Name: node.Title,
                            Type: tp,
                            ExecScope: 0,
                            ExecLimit: 0,
                            AutoAccept: true,
                            CanDelete: (tp == WfdAtvType.Start),
                            CanTerminate: false,
                            CanJumpInto: false,
                            JoinKind: 0,
                            TransKind: 0,
                            Ctime: Kit.Now,
                            Mtime: Kit.Now);

                        node.Tag = atv;
                        _prc.Atvs.Add(atv);
                    }
                    else if (node.Tag is WfdAtvX atv)
                    {
                        // 删除后撤消 或 撤消后重做
                        _prc.Atvs.Add(atv);
                    }
                }
                else if (item is SLine line)
                {
                    if (item.Tag == null)
                    {
                        line.ID = await WfdTrsX.NewID();
                        WfdTrsX trs = new WfdTrsX(
                            ID: line.ID,
                            PrcID: _prc.ID,
                            SrcAtvID: line.HeaderID,
                            TgtAtvID: line.TailID);

                        line.Tag = trs;
                        _prc.Trss.Add(trs);
                    }
                    else if (item.Tag is WfdTrsX trs)
                    {
                        _prc.Trss.Add(trs);
                    }
                    if (e.Count == 1)
                        _sketch.SelectLine(line);
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
                    if (node.Tag is WfdAtvX atv)
                        _prc.Atvs.Remove(atv);
                }
                else if (item is SLine line)
                {
                    if (line.Tag is WfdTrsX trs)
                        _prc.Trss.Remove(trs);
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
            Menu menu;
            if (_sketch.SelectedLine != null)
            {
                LoadTrsForm(_sketch.SelectedLine);
                menu = GetDelMenu();
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
                else
                {
                    menu = GetDelMenu();
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
                menu = GetDelMenu();
            }
            else
            {
                _tab.Content = null;
                if (_insertMenu == null)
                    _insertMenu = new WfInsertMenu(_sketch);
                menu = _insertMenu;
            }

            _ = menu.OpenContextMenu(e.GetPosition(null));
        }

        WfDeleteMenu GetDelMenu()
        {
            if (_delMenu == null)
                _delMenu = new WfDeleteMenu(_sketch);
            return _delMenu;
        }
        #endregion

        #region 属性表单
        /// <summary>
        /// 加载活动表单
        /// </summary>
        /// <param name="p_node"></param>
        void LoadAtvForm(SNode p_node)
        {
            var data = p_node.Tag as WfdAtvX;
            if (data == null)
            {
                _tab.Content = null;
                return;
            }

            switch (data.Type)
            {
                case WfdAtvType.Start:
                    // 开始活动
                    if (_startAtvForm == null)
                        _startAtvForm = new WfStartAtvForm();
                    _startAtvForm.LoadNode(p_node, _prc.AtvRoles);
                    _tab.Content = _startAtvForm;
                    break;
                case WfdAtvType.Sync:
                    // 同步活动
                    if (_syncAtvForm == null)
                        _syncAtvForm = new WfSyncAtvForm();
                    _syncAtvForm.LoadNode(p_node);
                    _tab.Content = _syncAtvForm;
                    break;
                case WfdAtvType.Finish:
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
                    _atvForm.LoadNode(p_node, _prc.AtvRoles);
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
            var data = p_line.Tag as WfdTrsX;
            if (data == null)
            {
                _tab.Content = null;
                return;
            }

            if (_trsForm == null)
                _trsForm = new WfTrsForm();
            _trsForm.LoadData(_prc.Trss, data);
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


        public bool IsChanged
        {
            get { return (bool)GetValue(IsChangedProperty); }
            set { SetValue(IsChangedProperty, value); }
        }

        public static readonly DependencyProperty IsChangedProperty = DependencyProperty.Register(
            "IsChanged",
            typeof(bool),
            typeof(WorkflowDesign),
            new PropertyMetadata(false));
    }
}