#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 选项卡控件
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public partial class TabControl : DtControl
    {
        #region 静态内容
        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register(
            "TabStripPlacement",
            typeof(ItemPlacement),
            typeof(TabControl),
            new PropertyMetadata(ItemPlacement.Bottom, new PropertyChangedCallback(OnTabStripPlacementChanged)));

        public readonly static DependencyProperty AllowSwapItemProperty = DependencyProperty.Register(
            "AllowSwapItem",
            typeof(bool),
            typeof(TabControl),
            new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            "SelectedIndex",
            typeof(int),
            typeof(TabControl),
            new CoercePropertyMetadata(-1, OnSelectedIndexChanged, CoerceSelectedIndex));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(TabItem),
            typeof(TabControl),
            new PropertyMetadata(null, OnSelectedItemChanged));

        public static readonly DependencyProperty PopWidthProperty = DependencyProperty.Register(
            "PopWidth",
            typeof(double),
            typeof(TabControl),
            new PropertyMetadata(double.NaN, OnPopSizeChanged));

        public static readonly DependencyProperty PopHeightProperty = DependencyProperty.Register(
            "PopHeight",
            typeof(double),
            typeof(TabControl),
            new PropertyMetadata(double.NaN, OnPopSizeChanged));

        public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register(
            "SelectedContent",
            typeof(object),
            typeof(TabControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ContentTransitionsProperty = DependencyProperty.Register(
            "ContentTransitions",
            typeof(TransitionCollection),
            typeof(TabControl),
            new PropertyMetadata(null));

        static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            if (!tab._selector.Locked && tab._isLoaded)
            {
                int newValue = (int)e.NewValue;
                if (newValue == -1)
                {
                    tab._selector.Select(null);
                }
                else if (newValue > -1 && newValue < tab.Items.Count)
                {
                    tab._selector.Select(tab.Items[newValue]);
                }
            }
        }

        static object CoerceSelectedIndex(DependencyObject d, object value)
        {
            TabControl tab = (TabControl)d;
            if (!tab._updatingSelection
                && (value is int)
                && (((int)value) >= tab.Items.Count || ((int)value) < -1))
            {
                return -1;
            }
            return value;
        }

        static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            if (!tab._selector.Locked && tab._isLoaded)
            {
                if (e.NewValue != null)
                {
                    tab._selector.Select((TabItem)e.NewValue);
                }
                else
                {
                    tab._selector.Select(null);
                }
            }
        }

        static void OnTabStripPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            tab.OnPlacementChanged();
        }

        static void OnPopSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControl tab = (TabControl)d;
            tab.ApplyPopStyle();
        }
        #endregion

        #region 成员变量
        readonly SelectionChanger _selector;
        protected StackPanel _itemsPanel;
        protected bool _isLoaded;
        Grid _mainGrid;
        SizedPresenter _contentPresenter;
        Dlg _dlg;
        bool _updatingSelection;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public TabControl()
        {
            DefaultStyleKey = typeof(TabControl);
            Items = new TabItemsList();
            _selector = new SelectionChanger(this);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 当选定的 TabItem 更改时发生
        /// </summary>
        public event EventHandler<SelectedChangedEventArgs> SelectedChanged;
        #endregion

        #region 属性
        /// <summary>
        /// 获取子项集合
        /// </summary>
        public TabItemsList Items { get; }

        /// <summary>
        /// 获取或设置 TabItem 标题相对于内容的对齐方式
        /// </summary>
        public ItemPlacement TabStripPlacement
        {
            get { return (ItemPlacement)GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        /// <summary>
        /// 获取设置TabItem是否可拖拽调整位置
        /// </summary>
        public bool AllowSwapItem
        {
            get { return (bool)GetValue(AllowSwapItemProperty); }
            set { SetValue(AllowSwapItemProperty, value); }
        }

        /// <summary>
        /// 获取设置弹出面板的宽度
        /// </summary>
        public double PopWidth
        {
            get { return (double)GetValue(PopWidthProperty); }
            set { SetValue(PopWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置弹出面板的高度
        /// </summary>
        public double PopHeight
        {
            get { return (double)GetValue(PopHeightProperty); }
            set { SetValue(PopHeightProperty, value); }
        }

        /// <summary>
        /// 获取或设置当前选定的 TabItem 的索引
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, (int)value); }
        }

        /// <summary>
        /// 获取或设置当前选定的 TabItem
        /// </summary>
        public TabItem SelectedItem
        {
            get { return (TabItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 获取设置当前选择的 TabItem 的内容
        /// </summary>
        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }
            internal set { SetValue(SelectedContentProperty, value); }
        }

        /// <summary>
        /// 获取或设置切换内容时的转换
        /// </summary>
        public TransitionCollection ContentTransitions
        {
            get { return (TransitionCollection)GetValue(ContentTransitionsProperty); }
            set { SetValue(ContentTransitionsProperty, value); }
        }

        /// <summary>
        /// 获取是否显示当前选择的 TabItem 的内容，false时在弹出面板显示
        /// </summary>
        public bool ShowContent
        {
            get { return _contentPresenter != null; }
        }

        /// <summary>
        /// 获取设置是否采用outlook导航样式，只在标签在上下侧时有效
        /// </summary>
        public bool IsOutlookStyle
        {
            get { return _itemsPanel?.Orientation == Orientation.Vertical; }
        }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            _mainGrid = (Grid)GetTemplateChild("MainGrid");
            _itemsPanel = (StackPanel)GetTemplateChild("ItemsPanel");
            _contentPresenter = (SizedPresenter)GetTemplateChild("TabContent");
            _isLoaded = true;

            if (TabStripPlacement != ItemPlacement.Bottom)
                OnPlacementChanged();

            LoadAllItems();
            InitSelection();
            Items.ItemsChanged += OnItemsChanged;
            SizeChanged += OnSizeChanged;
        }
        #endregion

        #region Items管理
        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                var item = Items[e.Index];
                InitItem(item);
                _itemsPanel.Children.Insert(e.Index, item);
            }
            else if (e.CollectionChange == CollectionChange.ItemRemoved)
            {
                ((TabItem)_itemsPanel.Children[e.Index]).IsSelected = false;
                _itemsPanel.Children.RemoveAt(e.Index);
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    if (_itemsPanel.Children.Count > i)
                    {
                        var elem = _itemsPanel.Children[i];

                        // 内容没变
                        if (item == elem)
                            continue;

                        // 变了移除旧元素
                        _itemsPanel.Children.RemoveAt(i);
                    }
                    InitItem(item);
                    _itemsPanel.Children.Insert(i, item);
                }

                // 移除多余的元素
                while (_itemsPanel.Children.Count > Items.Count)
                {
                    _itemsPanel.Children.RemoveAt(_itemsPanel.Children.Count - 1);
                }
            }

            if (!ShowContent)
            {
                _selector.Select(null);
            }
            else if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                _selector.Select(Items[e.Index]);
            }
            else
            {
                _selector.ResetSelection();
            }
            ApplyOutlookStyle();
            OnItemsChanged();
        }

        void LoadAllItems()
        {
            foreach (var item in Items)
            {
                InitItem(item);
                try
                {
                    _itemsPanel.Children.Add(item);
                }
                catch
                {
                    item.ClearParent();
                    _itemsPanel.Children.Add(item);
                }
            }
        }

        protected virtual void InitItem(TabItem p_item)
        {
            p_item.Owner = this;
            p_item.TabStripPlacement = TabStripPlacement;
            p_item.BorderBrush = BorderBrush;
        }

        protected virtual void OnItemsChanged()
        { }
        #endregion

        #region 内部方法
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplyOutlookStyle();
        }

        /// <summary>
        /// 弹出面板关闭时清除选择项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPopClosed(object sender, bool e)
        {
            SelectedIndex = -1;
        }

        /// <summary>
        /// 初始选择状态，优先级：SelectedIndex > SelectedItem > TabItem.IsSelected
        /// </summary>
        void InitSelection()
        {
            if (_contentPresenter == null)
                return;

            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                _selector.Select(Items[SelectedIndex]);
            }
            else if (SelectedItem != null)
            {
                _selector.Select(SelectedItem);
            }
            else if (Items.Count == 1)
            {
                _selector.Select(Items[0]);
            }
            else if (Items.Count > 0)
            {
                TabItem item = (from it in Items
                                where it.IsSelected
                                select it).FirstOrDefault();
                if (item != null)
                    _selector.Select(item);
                else
                    _selector.Select(Items[0]);
            }
        }
        #endregion

        #region 拖拽调序
        /// <summary>
        /// 内部元素拖拽过程
        /// </summary>
        /// <param name="p_src">被拖拽元素</param>
        /// <param name="p_pt">鼠标位置</param>
        /// <returns>false 表示不在有效区域</returns>
        internal bool DoSwap(TabItem p_src, Point p_pt)
        {
            TabItem target = (from item in Items
                              where item.ContainPoint(p_pt)
                              select item).FirstOrDefault();

            // 交换位置
            if (target != null && target != p_src)
            {
                // 防止两元素宽度不一致交换时的抖动问题
                if (p_src.ActualWidth < target.ActualWidth)
                {
                    if (IsCriticalPoint(target, p_src, p_pt))
                        return true;
                }

                try
                {
                    Items.ItemsChanged -= OnItemsChanged;
                    int srcIndex = Items.IndexOf(p_src);
                    int tgtIndex = Items.IndexOf(target);
                    Items.RemoveAt(tgtIndex);
                    _itemsPanel.Children.RemoveAt(tgtIndex);

                    Items.Insert(srcIndex, target);
                    _itemsPanel.Children.Insert(srcIndex, target);
                    OnSwappedItem();
                }
                finally
                {
                    Items.ItemsChanged += OnItemsChanged;
                }
            }
            return target != null;
        }

        /// <summary>
        /// 当前鼠标是否在两个待交换标签的临界处
        /// </summary>
        /// <param name="p_tgt">目标</param>
        /// <param name="p_src">源</param>
        /// <param name="p_pt"></param>
        /// <returns></returns>
        public static bool IsCriticalPoint(Control p_tgt, Control p_src, Point p_pt)
        {
            bool critical = false;
            double width = p_tgt.ActualWidth - p_src.ActualWidth;
            MatrixTransform mtTgt = p_tgt.TransformToVisual(null) as MatrixTransform;
            MatrixTransform mtSrc = p_src.TransformToVisual(null) as MatrixTransform;
            if (mtTgt != null && mtSrc != null)
            {
                //源在目标的前面
                if (mtSrc.Matrix.OffsetX < mtTgt.Matrix.OffsetX)
                {
                    if ((mtSrc.Matrix.OffsetX + width) > p_pt.X)
                        critical = true;
                }
                else
                {
                    if ((mtSrc.Matrix.OffsetX - width) < p_pt.X)
                        critical = true;
                }
            }
            return critical;
        }

        /// <summary>
        /// 拖拽调序后，提供Win保存布局
        /// </summary>
        protected virtual void OnSwappedItem()
        {
        }
        #endregion

        #region 布局样式
        /// <summary>
        /// 切换标签相对于内容的对齐方式
        /// </summary>
        void OnPlacementChanged()
        {
            if (!_isLoaded)
                return;

            if (TabStripPlacement == ItemPlacement.Left)
            {
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Star);

                Grid.SetColumn(_itemsPanel, 0);
                Grid.SetColumnSpan(_itemsPanel, 1);
                Grid.SetRow(_itemsPanel, 0);
                Grid.SetRowSpan(_itemsPanel, 2);

                if (_contentPresenter != null)
                {
                    Grid.SetColumn(_contentPresenter, 1);
                    Grid.SetColumnSpan(_contentPresenter, 1);
                    Grid.SetRow(_contentPresenter, 0);
                    Grid.SetRowSpan(_contentPresenter, 2);
                }
            }
            else if (TabStripPlacement == ItemPlacement.Right)
            {
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Star);

                Grid.SetColumn(_itemsPanel, 1);
                Grid.SetColumnSpan(_itemsPanel, 1);
                Grid.SetRow(_itemsPanel, 0);
                Grid.SetRowSpan(_itemsPanel, 2);

                if (_contentPresenter != null)
                {
                    Grid.SetColumn(_contentPresenter, 0);
                    Grid.SetColumnSpan(_contentPresenter, 1);
                    Grid.SetRow(_contentPresenter, 0);
                    Grid.SetRowSpan(_contentPresenter, 2);
                }
            }
            else if (TabStripPlacement == ItemPlacement.Bottom)
            {
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);

                Grid.SetRow(_itemsPanel, 1);
                Grid.SetRowSpan(_itemsPanel, 1);
                Grid.SetColumn(_itemsPanel, 0);
                Grid.SetColumnSpan(_itemsPanel, 2);

                if (_contentPresenter != null)
                {
                    Grid.SetColumn(_contentPresenter, 0);
                    Grid.SetColumnSpan(_contentPresenter, 2);
                    Grid.SetRow(_contentPresenter, 0);
                    Grid.SetRowSpan(_contentPresenter, 1);
                }
            }
            else if (TabStripPlacement == ItemPlacement.Top)
            {
                _mainGrid.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.RowDefinitions[1].Height = new GridLength(1.0, GridUnitType.Star);
                _mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                _mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Star);

                Grid.SetRow(_itemsPanel, 0);
                Grid.SetRowSpan(_itemsPanel, 1);
                Grid.SetColumn(_itemsPanel, 0);
                Grid.SetColumnSpan(_itemsPanel, 2);

                if (_contentPresenter != null)
                {
                    Grid.SetColumn(_contentPresenter, 0);
                    Grid.SetColumnSpan(_contentPresenter, 2);
                    Grid.SetRow(_contentPresenter, 1);
                    Grid.SetRowSpan(_contentPresenter, 1);
                }
            }

            ApplyPopStyle();

            if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                _itemsPanel.Orientation = Orientation.Vertical;
            else
                ApplyOutlookStyle();

            foreach (var item in Items)
            {
                item.TabStripPlacement = TabStripPlacement;
            }
        }

        /// <summary>
        /// 自适应outlook模式
        /// </summary>
        void ApplyOutlookStyle()
        {
            if (!_isLoaded
                || ActualWidth == 0
                || TabStripPlacement == ItemPlacement.Left
                || TabStripPlacement == ItemPlacement.Right)
                return;

            // 20 是默认stripborder的留白
            double width = 20;
            Size size = new Size(5000, 100);
            foreach (TabItem item in Items)
            {
                item.Measure(size);
                width += item.DesiredSize.Width;
            }
            _itemsPanel.Orientation = ActualWidth < width ? Orientation.Vertical : Orientation.Horizontal;
        }

        /// <summary>
        /// 弹出式标签属性设置
        /// </summary>
        void ApplyPopStyle()
        {
            if (ShowContent || _dlg == null)
                return;

            if (TabStripPlacement == ItemPlacement.Left)
            {
                _dlg.PhonePlacement = DlgPlacement.FromRight;
                _dlg.WinPlacement = DlgPlacement.FromRight;
                _dlg.Height = double.NaN;
                _dlg.Width = PopWidth;
            }
            else if (TabStripPlacement == ItemPlacement.Right)
            {
                _dlg.PhonePlacement = DlgPlacement.FromLeft;
                _dlg.WinPlacement = DlgPlacement.FromLeft;
                _dlg.Height = double.NaN;
                _dlg.Width = PopWidth;
            }
            else if (TabStripPlacement == ItemPlacement.Bottom)
            {
                _dlg.PhonePlacement = DlgPlacement.FromTop;
                _dlg.WinPlacement = DlgPlacement.FromTop;
                _dlg.Height = PopHeight;
                _dlg.Width = double.NaN;
            }
            else if (TabStripPlacement == ItemPlacement.Top)
            {
                _dlg.PhonePlacement = DlgPlacement.FromBottom;
                _dlg.WinPlacement = DlgPlacement.FromBottom;
                _dlg.Height = PopHeight;
                _dlg.Width = double.NaN;
            }
        }
        #endregion

        #region 选择相关
        /// <summary>
        /// 处理给定项容器的选择状态
        /// </summary>
        /// <param name="p_container"></param>
        /// <param name="p_selected"></param>
        internal void NotifyIsSelectedChanged(TabItem p_container, bool p_selected)
        {
            if (_isLoaded && !_selector.Locked && p_container != null)
            {
                if (p_selected)
                {
                    _selector.Select(p_container);
                }
                else
                {
                    _selector.Unselect(p_container);
                }
            }
        }

        /// <summary>
        /// 重置SelectedIndex, SelectedItem
        /// </summary>
        void UpdateSelectionProperties()
        {
            try
            {
                _updatingSelection = true;

                // 内部实际的选项索引
                int curIndex = -1;
                if (_selector.Selection != null)
                    curIndex = Items.IndexOf(_selector.Selection);
                if (SelectedIndex != curIndex)
                    SetValue(SelectedIndexProperty, curIndex);

                if (SelectedItem != _selector.Selection)
                    SetValue(SelectedItemProperty, _selector.Selection);
            }
            finally
            {
                _updatingSelection = false;
            }
        }

        /// <summary>
        /// 设置指定项的IsSelected值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        void SetItemIsSelected(TabItem item, bool value)
        {
            if (item != null && item.IsSelected != value)
                item.IsSelected = value;
        }

        void CreateDlg()
        {
            _dlg = new Dlg() { ClipElement = _itemsPanel, PlacementTarget = _itemsPanel, Resizeable = false, HideTitleBar = true };
            _dlg.Content = LoadDlgContent();
            switch (TabStripPlacement)
            {
                case ItemPlacement.Left:
                    _dlg.WinPlacement = DlgPlacement.TargetTopRight;
                    break;
                case ItemPlacement.Top:
                    _dlg.WinPlacement = DlgPlacement.TargetBottomLeft;
                    break;
                case ItemPlacement.Right:
                    _dlg.WinPlacement = DlgPlacement.TargetOuterLeftTop;
                    break;
                case ItemPlacement.Bottom:
                    _dlg.WinPlacement = DlgPlacement.TargetOuterTop;
                    break;
                default:
                    break;
            }
            _dlg.Closed += OnPopClosed;
        }

        protected virtual object LoadDlgContent()
        {
            ContentPresenter content = new ContentPresenter()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            Binding contentBinding = new Binding() { Path = new PropertyPath("SelectedContent"), Source = this };
            content.SetBinding(ContentPresenter.ContentProperty, contentBinding);
            return content;
        }

        /// <summary>
        /// 选择变化后的处理
        /// </summary>
        /// <param name="p_unselectItem"></param>
        /// <param name="p_selectItem"></param>
        void AfterSelectionChanged(TabItem p_unselectItem, TabItem p_selectItem)
        {
            // 设置内容
            TabItem item = SelectedItem;
            if (item != null)
            {
                SelectedContent = item.Content;
                if (!ShowContent)
                {
                    // 弹出式
                    if (_dlg == null)
                        CreateDlg();

                    if (TabStripPlacement == ItemPlacement.Left || TabStripPlacement == ItemPlacement.Right)
                    {
                        if (item.ReadLocalValue(TabItem.PopWidthProperty) != DependencyProperty.UnsetValue)
                        {
                            _dlg.Width = item.PopWidth;
                            _dlg.Height = _itemsPanel.ActualHeight;
                        }
                    }
                    else
                    {
                        if (item.ReadLocalValue(TabItem.PopHeightProperty) != DependencyProperty.UnsetValue)
                        {
                            _dlg.Height = item.PopHeight;
                            _dlg.Width = _itemsPanel.ActualWidth;
                        }
                    }

                    _dlg.Show();
                }
            }
            else
            {
                SelectedContent = null;
                if (!ShowContent && _dlg != null)
                    _dlg.Close();
            }

            SelectedChanged?.Invoke(this, new SelectedChangedEventArgs(p_unselectItem, p_selectItem));
        }

        /// <summary>
        /// 管理选择项变化
        /// </summary>
        internal class SelectionChanger
        {
            TabControl _tab;
            bool _locked;
            TabItem _internalSelection;
            TabItem _itemToSelect;
            TabItem _itemToUnselect;

            public SelectionChanger(TabControl p_tab)
            {
                _tab = p_tab;
                _itemToSelect = null;
                _itemToUnselect = null;
                _internalSelection = null;
                _locked = false;
            }

            /// <summary>
            /// 选择指定项
            /// </summary>
            /// <param name="p_item"></param>
            internal void Select(TabItem p_item)
            {
                if (_internalSelection != p_item)
                {
                    Begin();
                    _itemToSelect = p_item;
                    _itemToUnselect = _internalSelection;
                    End();
                }
            }

            /// <summary>
            /// 取消指定项的选择状态
            /// </summary>
            /// <param name="p_item"></param>
            internal void Unselect(TabItem p_item)
            {
                if (p_item != null && _internalSelection == p_item)
                {
                    Begin();
                    _itemToUnselect = _internalSelection;
                    foreach (var item in _tab.Items)
                    {
                        if (item != _internalSelection)
                        {
                            _itemToSelect = item;
                            break;
                        }
                    }
                    End();
                }
            }

            /// <summary>
            /// 重置有效的标签
            /// </summary>
            internal void ResetSelection()
            {
                if (_tab.Items.Count == 0)
                    return;

                bool exist = (from item in _tab.Items
                              where item == _internalSelection
                              select item).Any();
                if (!exist)
                {
                    Begin();
                    _itemToSelect = _tab.Items[0];
                    _itemToUnselect = _internalSelection;
                    End();
                }
            }

            void Begin()
            {
                _locked = true;
                _itemToSelect = null;
                _itemToUnselect = null;
            }

            void End()
            {
                try
                {
                    SynchronizeSelection();
                    _tab.UpdateSelectionProperties();
                }
                finally
                {
                    _locked = false;
                    if (_itemToUnselect != null || _itemToSelect != null)
                    {
                        _tab.AfterSelectionChanged(_itemToUnselect, _itemToSelect);
                    }
                }
            }

            /// <summary>
            /// 同步现有的选择项
            /// </summary>
            void SynchronizeSelection()
            {
                if (_itemToUnselect != null)
                    _tab.SetItemIsSelected(_itemToUnselect, false);
                if (_itemToSelect != null)
                    _tab.SetItemIsSelected(_itemToSelect, true);
                _internalSelection = _itemToSelect;
            }

            internal bool Locked
            {
                get { return _locked; }
            }

            internal TabItem Selection
            {
                get { return _internalSelection; }
            }
        }
        #endregion
    }

    /// <summary>
    /// 选择项变化事件参数
    /// </summary>
    public class SelectedChangedEventArgs : EventArgs
    {
        TabItem _unselectItem;
        TabItem _selectItem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_unselect"></param>
        /// <param name="p_select"></param>
        public SelectedChangedEventArgs(TabItem p_unselect, TabItem p_select)
        {
            _unselectItem = p_unselect;
            _selectItem = p_select;
        }

        /// <summary>
        /// 获取要取消的选择项
        /// </summary>
        public TabItem UnselectItem
        {
            get { return _unselectItem; }
        }

        /// <summary>
        /// 获取新的选择项
        /// </summary>
        public TabItem SelectItem
        {
            get { return _selectItem; }
        }
    }

    /// <summary>
    /// 子项列表，直接用泛型在xaml设计时异常
    /// </summary>
    public class TabItemsList : ItemList<TabItem>
    { }
}
