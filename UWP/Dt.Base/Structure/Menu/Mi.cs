#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.MenuView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 菜单项
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public partial class Mi : DtControl
    {
        #region 静态内容
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register(
            "ID",
            typeof(string),
            typeof(Mi),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(Mi),
            new PropertyMetadata(Icons.None));

        public static readonly DependencyProperty CmdProperty = DependencyProperty.Register(
            "Cmd",
            typeof(ICommand),
            typeof(Mi),
            new PropertyMetadata(null, OnCommandChanged));

        public static readonly DependencyProperty CmdParamProperty = DependencyProperty.Register(
            "CmdParam",
            typeof(object),
            typeof(Mi),
            new PropertyMetadata(null, OnCommandParameterChanged));

        public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register(
            "IsCheckable",
            typeof(bool),
            typeof(Mi),
            new PropertyMetadata(false, OnIsCheckableChanged));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked",
            typeof(bool),
            typeof(Mi),
            new PropertyMetadata(false, OnIsCheckedChanged));

        public readonly static DependencyProperty ShowInPhoneProperty = DependencyProperty.Register(
            "ShowInPhone",
            typeof(VisibleInPhone),
            typeof(Mi),
            new PropertyMetadata(VisibleInPhone.ID, OnShowInPhoneChanged));

        public static readonly DependencyProperty StaysOpenOnClickProperty = DependencyProperty.Register(
            "StaysOpenOnClick",
            typeof(bool),
            typeof(Mi),
            new PropertyMetadata(false));

        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName",
            typeof(string),
            typeof(Mi),
            null);

        public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register(
            "Scope",
            typeof(MiScope),
            typeof(Mi),
            new PropertyMetadata(MiScope.Both, OnScopeChanged));

        internal static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(Mi),
            new PropertyMetadata(false, OnIsSelectedChanged));

        internal static readonly DependencyProperty IsSubmenuOpenProperty = DependencyProperty.Register(
            "IsSubmenuOpen",
            typeof(bool),
            typeof(Mi),
            new PropertyMetadata(false, OnIsSubmenuOpenChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Mi)d).ChangeCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Mi)d).CanExecuteApply();
        }

        static void OnScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Mi mi = (Mi)d;
            if (Kit.IsPhoneUI)
                mi.Visibility = ((MiScope)e.NewValue) == MiScope.Windows ? Visibility.Collapsed : Visibility.Visible;
            else
                mi.Visibility = ((MiScope)e.NewValue) == MiScope.Phone ? Visibility.Collapsed : Visibility.Visible;
        }

        static void OnIsCheckableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                ((Mi)d).UpdateCheckedIcon();
        }

        static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Mi mi = (Mi)d;
            if (mi.IsCheckable)
            {
                mi.UpdateCheckedIcon();
                if ((bool)e.NewValue)
                    mi.Checked?.Invoke(mi, EventArgs.Empty);
                else
                    mi.Unchecked?.Invoke(mi, EventArgs.Empty);
            }
        }

        static void OnShowInPhoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Mi mi = (Mi)d;
            if (mi._loaded && mi.ParentMi == null)
                mi.UpdateRoleState();
        }

        static void OnIsSubmenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Mi)d).OpenOrCloseSubMenu();
        }

        static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Mi item = (Mi)d;
            bool newValue = (bool)e.NewValue;
            if (!newValue)
            {
                item.IsMouseOver = false;
                item.ChangeState(MenuItemState.Normal);
            }
            else
            {
                item.ChangeState(MenuItemState.Pressed);
            }

            if ((bool)e.OldValue)
            {
                if (item.IsSubmenuOpen)
                    item.CloseSubMenu();
            }

            // 冒泡处理，只处理转为选择状态的
            if (newValue)
                item.Owner?.OnItemIsSelected(item);
        }
        #endregion

        #region 成员变量
        bool _loaded;
        readonly Locker _locker = new Locker();
        Mi _selectedMi;
        Mi _lastSelected;
        uint? _pointerID;
        SubMenuDlg _dlg;
        #endregion

        #region 构造方法
        public Mi()
        {
            DefaultStyleKey = typeof(Mi);
            Items = new MiList();
            IsEnabledChanged += OnIsEnabledChanged;

            // Mi内部未设置上下边距
            MinHeight = Kit.IsPhoneUI ? 50 : 40;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 点击事件
        /// </summary>
#if ANDROID
        new
#endif
        public event EventHandler<Mi> Click;

        /// <summary>
        /// 勾选事件
        /// </summary>
        public event EventHandler Checked;

        /// <summary>
        /// 取消勾选事件
        /// </summary>
        public event EventHandler Unchecked;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置菜单项标题
        /// </summary>
        public string ID
        {
            get { return (string)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        /// <summary>
        /// 获取设置图标
        /// </summary>
        public Icons Icon
        {
            get { return (Icons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置命令对象
        /// </summary>
        public ICommand Cmd
        {
            get { return (ICommand)GetValue(CmdProperty); }
            set { SetValue(CmdProperty, value); }
        }

        /// <summary>
        ///  获取设置命令参数
        /// </summary>
        public object CmdParam
        {
            get { return GetValue(CmdParamProperty); }
            set { SetValue(CmdParamProperty, value); }
        }

        /// <summary>
        /// 获取设置菜单项是否为可选择状态
        /// </summary>
        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        /// <summary>
        /// 获取设置是否为选择状态
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// 获取设置在手机工具栏上的显示内容，默认ID，可以为Icon, IconAndID
        /// </summary>
        public VisibleInPhone ShowInPhone
        {
            get { return (VisibleInPhone)GetValue(ShowInPhoneProperty); }
            set { SetValue(ShowInPhoneProperty, value); }
        }

        /// <summary>
        /// 获取设置点击后是否不关闭菜单，默认false表自动关闭
        /// </summary>
        public bool StaysOpenOnClick
        {
            get { return (bool)GetValue(StaysOpenOnClickProperty); }
            set { SetValue(StaysOpenOnClickProperty, value); }
        }

        /// <summary>
        /// 获取设置分组名称，用在组内单选情况
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// 获取设置菜单项使用范围，默认Both始终显示
        /// </summary>
        public MiScope Scope
        {
            get { return (MiScope)GetValue(ScopeProperty); }
            set { SetValue(ScopeProperty, value); }
        }

        /// <summary>
        /// 获取子菜单集合
        /// </summary>
        public MiList Items { get; }

        /// <summary>
        /// 递归获取所有子级菜单项
        /// </summary>
        public IEnumerable<Mi> AllItems
        {
            get
            {
                foreach (Mi mi in Items)
                {
                    yield return mi;
                    if (mi.Items.Count > 0)
                    {
                        foreach (Mi child in mi.AllItems)
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取上下文菜单的目标数据
        /// </summary>
        public object Data
        {
            get { return Owner?.TargetData; }
        }

        /// <summary>
        /// 获取上下文菜单的目标数据
        /// </summary>
        public Row Row
        {
            get { return Owner?.TargetData as Row; }
        }
        #endregion

        #region 内部属性
        /// <summary>
        /// 所属菜单
        /// </summary>
        internal Menu Owner { get; set; }

        /// <summary>
        /// 父菜单项
        /// </summary>
        internal Mi ParentMi { get; set; }

        /// <summary>
        /// 菜单项是否为焦点
        /// </summary>
        internal bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 获取子菜单是否打开
        /// </summary>
        internal bool IsSubmenuOpen
        {
            get { return (bool)GetValue(IsSubmenuOpenProperty); }
            set { SetValue(IsSubmenuOpenProperty, value); }
        }

        internal bool IsMouseOver { get; set; }

        /// <summary>
        /// 当前选择的子项
        /// </summary>
        internal Mi SelectedMi
        {
            get { return _selectedMi; }
            set
            {
                if (_selectedMi != value)
                {
                    if (_selectedMi != null)
                        _selectedMi.IsSelected = false;
                    _selectedMi = value;
                    if (_selectedMi != null)
                        _selectedMi.IsSelected = true;
                }
            }
        }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            CanExecuteApply();
            ChangeState(MenuItemState.Normal);
            UpdateRoleState();
            _loaded = true;
        }

        internal void UpdateOwner(Menu p_owner, Mi p_parent)
        {
            Owner = p_owner;
            ParentMi = p_parent;
            if (_loaded)
            {
                UpdateRoleState();
            }
            else
            {
                InitSubsOwner();
                Items.ItemsChanged -= OnItemsChanged;
                Items.ItemsChanged += OnItemsChanged;
            }
        }

        void InitOwner(Menu p_owner, Mi p_parent)
        {
            Owner = p_owner;
            ParentMi = p_parent;
            InitSubsOwner();
            Items.ItemsChanged -= OnItemsChanged;
            Items.ItemsChanged += OnItemsChanged;
        }

        void InitSubsOwner()
        {
            if (Items.Count > 0)
            {
                foreach (var mi in Items)
                {
                    mi.InitOwner(Owner, this);
                }
            }
        }

        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                Mi mi = ((MiList)sender)[e.Index];
                mi.InitOwner(Owner, this);
            }
            else if (e.CollectionChange == CollectionChange.Reset)
            {
                InitSubsOwner();
            }
            if (_loaded)
                UpdateRoleState();
        }
        #endregion

        #region 执行菜单
        /// <summary>
        /// 点击菜单项
        /// </summary>
        void OnClickMi()
        {
            if (Items.Count > 0)
            {
                // 打开关闭子菜单
                if (IsSubmenuOpen)
                    CloseSubMenu();
                else
                    OpenSubMenu();
            }
            else
            {
                // 执行
                ExecuteMi();
            }
        }

        /// <summary>
        /// 执行菜单项
        /// </summary>
        void ExecuteMi()
        {
            // 加锁控制重复运行
            if (_locker.IsLocked)
                return;

            try
            {
                _locker.Lock();
                IsSelected = true;
                if (IsCheckable)
                {
                    // 增加对分组单选的支持
                    if (string.IsNullOrEmpty(GroupName))
                    {
                        IsChecked = !IsChecked;
                    }
                    else if (!IsChecked)
                    {
                        IEnumerable<Mi> groupItems = null;
                        if (ParentMi == null)
                        {
                            groupItems = from obj in Owner.Items
                                         let item = obj as Mi
                                         where item != null && item.GroupName == GroupName
                                         select item;
                        }
                        else
                        {
                            groupItems = from obj in ParentMi.Items
                                         let item = obj as Mi
                                         where item != null && item.GroupName == GroupName
                                         select item;
                        }

                        foreach (Mi item in groupItems)
                        {
                            if (item != this)
                                item.IsChecked = false;
                        }
                        IsChecked = true;
                    }
                }

                // 触发本菜单项点击事件和ItemClick事件，两事件原型相同
                Click?.Invoke(this, this);
                Owner?.OnItemClick(this);
                Cmd?.Execute(CmdParam);

                // 关闭整个菜单
                if (!StaysOpenOnClick)
                {
                    IsMouseOver = false;
                    ChangeState(MenuItemState.Normal);
                    // phone模式关闭上级窗口
                    if (Kit.IsPhoneUI && ParentMi != null && ParentMi.IsSubmenuOpen)
                        ParentMi.IsSubmenuOpen = false;
                    else
                        Owner?.Close();
                }
                else
                {
                    ChangeState(MenuItemState.PointerOver);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("执行菜单项时异常：{0}", ex.Message));
            }
            finally
            {
                _locker.Unlock();
            }
        }

        void ChangeCommand(ICommand p_oldCommand, ICommand p_newCommand)
        {
            if (p_oldCommand != null)
                p_oldCommand.CanExecuteChanged -= CanExecuteChanged;
            if (p_newCommand != null)
                p_newCommand.CanExecuteChanged += CanExecuteChanged;
            CanExecuteApply();
        }

        void CanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteApply();
        }

        void CanExecuteApply()
        {
            if (Cmd != null)
                IsEnabled = Cmd.CanExecute(CmdParam);
        }
        #endregion

        #region 打开关闭菜单
        internal void OpenSubMenu()
        {
            if (Items.Count == 0 || IsSubmenuOpen)
                return;

            if (ParentMi != null)
            {
                // 关闭同级已打开的菜单
                if (Kit.IsPhoneUI && ParentMi.IsSubmenuOpen)
                {
                    // phone模式关闭上级窗口
                    ParentMi.IsSubmenuOpen = false;
                }
                else
                {
                    Mi last = ParentMi._lastSelected;
                    if (last != null && last != this)
                    {
                        last.CloseSubMenu();
                        last.IsSelected = false;
                    }
                }
                ParentMi._lastSelected = this;
            }
            else if (Kit.IsPhoneUI && Owner.IsContextMenu)
            {
                Owner.Close();
            }

            IsSelected = true;
            IsSubmenuOpen = true;
        }

        void OpenOrCloseSubMenu()
        {
            if (IsSubmenuOpen)
            {
                IsSelected = true;
                SelectedMi = null;

                // 打开子菜单窗口
                if (_dlg == null)
                    _dlg = new SubMenuDlg(this);
                _dlg.ShowDlg();
            }
            else
            {
                if (SelectedMi != null && SelectedMi.IsSubmenuOpen)
                    SelectedMi.CloseSubMenu();
                SelectedMi = null;

                if (!IsMouseOver)
                    ChangeState(MenuItemState.Normal);
                // 关闭子菜单窗口
                if (_dlg != null)
                    _dlg.Close();
            }
        }

        /// <summary>
        /// 递归关闭所有子窗口
        /// </summary>
        internal void CloseSubMenu()
        {
            if (_lastSelected != null)
            {
                _lastSelected.CloseSubMenu();
                _lastSelected = null;
            }
            if (IsSubmenuOpen)
                IsSubmenuOpen = false;
        }
        #endregion

        #region IsSelected冒泡事件
        internal void OnChildIsSelected(Mi p_mi)
        {
            // 子菜单项为选择状态
            if (SelectedMi != p_mi)
            {
                if (SelectedMi != null)
                {
                    SelectedMi.CloseSubMenu();
                    SelectedMi.IsSelected = false;
                }

                SelectedMi = p_mi;
                if (!IsSelected)
                    IsSelected = true;
            }
        }
        #endregion

        #region 交互样式
        void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled && Items.Count > 0 && IsSubmenuOpen)
                CloseSubMenu();
            ChangeState(MenuItemState.Normal);
        }

        void ChangeState(MenuItemState p_state)
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
            else if (IsSubmenuOpen)
                VisualStateManager.GoToState(this, "Pressed", true);
            else
                VisualStateManager.GoToState(this, p_state.ToString(), true);
        }

        protected virtual void UpdateRoleState()
        {
            if (Kit.IsPhoneUI)
            {
                if (ParentMi == null && !Owner.IsContextMenu)
                    VisualStateManager.GoToState(this, ShowInPhone.ToString(), true);
                else
                    VisualStateManager.GoToState(this, Items.Count > 0 ? "PhoneSubGroup" : "PhoneSubItem", true);
            }
            else
            {
                if (ParentMi != null || Owner.IsContextMenu)
                    VisualStateManager.GoToState(this, Items.Count > 0 ? "WinSubGroup" : "WinSubItem", true);
                else if (string.IsNullOrEmpty(ID) || Icon == Icons.None)
                    VisualStateManager.GoToState(this, "WinIconOrID", true);
                else
                    VisualStateManager.GoToState(this, "WinTopItem", true);
            }
        }

        void UpdateCheckedIcon()
        {
            SetValue(IconProperty, IsChecked ? Icons.复选已选 : Icons.复选未选);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            OnClickMi();
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            Focus(FocusState.Programmatic);
            if (CapturePointer(e.Pointer))
            {
                _pointerID = e.Pointer.PointerId;
                e.Handled = true;
                ChangeState(MenuItemState.Pressed);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (_pointerID != e.Pointer.PointerId)
                return;

            ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            _pointerID = null;
            IsMouseOver = false;
            ChangeState(MenuItemState.Normal);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            // 触摸时不自动打开下级菜单
            if (Owner == null || Kit.IsPhoneUI || e.IsTouch())
                return;

            IsMouseOver = true;
            ChangeState(MenuItemState.PointerOver);

            // 一级菜单且已有选择项
            if (ParentMi == null)
            {
                if ((Owner.SelectedMi != null && Owner.SelectedMi != this)
                    || Owner.IsContextMenu)
                {
                    IsSelected = true;
                    if (Items.Count > 0)
                        OpenSubMenu();
                }
                return;
            }

            // 子菜单
            if (IsSubmenuOpen)
            {
                IsSelected = true;
            }
            else if (Items.Count > 0)
            {
                if (ParentMi.SelectedMi == null)
                    IsSelected = true;
                OpenSubMenu();
            }
            else
            {
                IsSelected = true;
            }
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (e.IsTouch())
            {
                ChangeState(MenuItemState.Normal);
                return;
            }

            IsMouseOver = false;
            bool unselect = (ParentMi == null && Items.Count > 0 && !IsSubmenuOpen)
                || (ParentMi != null && (Items.Count == 0 || !IsSubmenuOpen));

            if (unselect && IsSelected)
                IsSelected = false;
            else
                ChangeState(MenuItemState.Normal);
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            ChangeState(MenuItemState.Normal);
        }
        #endregion
    }
}
