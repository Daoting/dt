#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 可停靠项，内部子项为 Tabs 或 Pane
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public partial class Pane : DtControl, IPaneList
    {
        #region 静态内容
        public static readonly DependencyProperty PosProperty = DependencyProperty.Register(
            "Pos",
            typeof(PanePosition),
            typeof(Pane),
            new PropertyMetadata(PanePosition.Left, OnPosChanged));

        public static readonly DependencyProperty InitWidthProperty = DependencyProperty.Register(
            "InitWidth",
            typeof(double),
            typeof(Pane),
            new PropertyMetadata(400.0));

        public static readonly DependencyProperty InitHeightProperty = DependencyProperty.Register(
            "InitHeight",
            typeof(double),
            typeof(Pane),
            new PropertyMetadata(300.0));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(Pane),
            new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        public static readonly DependencyProperty FloatPosProperty = DependencyProperty.Register(
            "FloatPos",
            typeof(FloatPosition),
            typeof(Pane),
            new PropertyMetadata(FloatPosition.Center));

        public static readonly DependencyProperty ResizerPlacementProperty = DependencyProperty.Register(
            "ResizerPlacement",
            typeof(ItemPlacement?),
            typeof(Pane),
            new PropertyMetadata(ItemPlacement.Right));

        public static readonly DependencyProperty FloatLocationProperty = DependencyProperty.Register(
            "FloatLocation",
            typeof(Point),
            typeof(Pane),
            new PropertyMetadata(new Point(0.0, 0.0)));

        public static readonly DependencyProperty FloatSizeProperty = DependencyProperty.Register(
            "FloatSize",
            typeof(Size),
            typeof(Pane),
            new PropertyMetadata(new Size(300.0, 300.0)));


        static void OnPosChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Pane item = (Pane)sender;
            switch (item.Pos)
            {
                case PanePosition.Left:
                    item.ResizerPlacement = ItemPlacement.Right;
                    break;

                case PanePosition.Bottom:
                    item.ResizerPlacement = ItemPlacement.Top;
                    break;

                case PanePosition.Right:
                    item.ResizerPlacement = ItemPlacement.Left;
                    break;

                case PanePosition.Top:
                    item.ResizerPlacement = ItemPlacement.Bottom;
                    break;

                default:
                    item.ResizerPlacement = null;
                    break;
            }

            var parent = item.GetParent();
            if (parent != null)
            {
                parent.InvalidateMeasure();
            }
        }

        static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Pane item = (Pane)sender;
            if (item._isLoaded)
            {
                item._itemsPanel.Orientation = (Orientation)e.NewValue;
                item.UpdateChildrenResizer();
            }
        }
        #endregion

        #region 成员变量
        TabItemPanel _itemsPanel;
        bool _isLoaded;
        bool _isInCenter;
        bool _isInWindow;
        Win _ownWin;
        #endregion

        #region 构造方法
        public Pane()
        {
            // PhoneUI模式时不在可视树，省去uno在xaml自动生成代码时调用ApplyTemplate
            if (!Kit.IsPhoneUI)
                DefaultStyleKey = typeof(Pane);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置停靠位置
        /// </summary>
        public PanePosition Pos
        {
            get { return (PanePosition)GetValue(PosProperty); }
            set { SetValue(PosProperty, value); }
        }

        /// <summary>
        /// 获取设置停靠时的初始宽度，默认400
        /// </summary>
        public double InitWidth
        {
            get { return (double)GetValue(InitWidthProperty); }
            set { SetValue(InitWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置停靠时的初始高度
        /// </summary>
        public double InitHeight
        {
            get { return (double)GetValue(InitHeightProperty); }
            set { SetValue(InitHeightProperty, value); }
        }

        /// <summary>
        /// 获取设置子元素排序方式
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 获取设置浮动状态时的相对位置，优先级低于FloatLocation
        /// </summary>
        public FloatPosition FloatPos
        {
            get { return (FloatPosition)GetValue(FloatPosProperty); }
            set { SetValue(FloatPosProperty, value); }
        }

        /// <summary>
        /// 获取设置浮动状态时的初始位置
        /// </summary>
        public Point FloatLocation
        {
            get { return (Point)GetValue(FloatLocationProperty); }
            set { SetValue(FloatLocationProperty, value); }
        }

        /// <summary>
        /// 获取设置浮动状态时的初始大小
        /// </summary>
        public Size FloatSize
        {
            get { return (Size)GetValue(FloatSizeProperty); }
            set { SetValue(FloatSizeProperty, value); }
        }

        /// <summary>
        /// 获取尺寸调节器的位置，由Pos决定
        /// </summary>
        public ItemPlacement? ResizerPlacement
        {
            get { return (ItemPlacement?)GetValue(ResizerPlacementProperty); }
            internal set { SetValue(ResizerPlacementProperty, value); }
        }

        /// <summary>
        /// 获取内容元素集合
        /// </summary>
        public PaneList Items { get; } = new PaneList();

        /// <summary>
        /// 获取设置当前是否停靠在中部
        /// </summary>
        public bool IsInCenter
        {
            get { return _isInCenter; }
            set
            {
                if (_isInCenter != value)
                {
                    _isInCenter = value;
                    foreach (var item in Items)
                    {
                        if (item is Tabs tabs)
                        {
                            tabs.IsInCenter = _isInCenter;
                        }
                        else if (item is Pane wi)
                        {
                            wi.IsInCenter = _isInCenter;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取设置父容器是否为ToolWindow
        /// </summary>
        public bool IsInWindow
        {
            get { return _isInWindow; }
            set
            {
                if (_isInWindow != value)
                {
                    _isInWindow = value;
                    foreach (var item in Items)
                    {
                        if (item is Tabs tabs)
                        {
                            tabs.IsInWindow = _isInWindow;
                        }
                        else if (item is Pane wi)
                        {
                            wi.IsInWindow = _isInWindow;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 所属Win
        /// </summary>
        internal Win OwnWin
        {
            get
            {
                if (_ownWin == null)
                    _ownWin = this.FindParentByType<Win>();
                return _ownWin;
            }
        }

        /// <summary>
        /// 在PanePanel中占的区域，iOS中Bounds与基类重名
        /// </summary>
        internal Rect Region { get; set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取包含的所有Tab的枚举
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tab> GetAllTabItems()
        {
            foreach (object item in Items)
            {
                Tabs childSect;
                Pane childDockItem;
                if ((childSect = item as Tabs) != null)
                {
                    foreach (Tab child in childSect.Items)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
                else if ((childDockItem = item as Pane) != null)
                {
                    foreach (Tab child in childDockItem.GetAllTabItems())
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有Tabs的枚举
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tabs> GetAllTabs()
        {
            foreach (object item in Items)
            {
                Tabs childSect;
                Pane childDockItem;
                if ((childSect = item as Tabs) != null)
                {
                    yield return childSect;
                }
                else if ((childDockItem = item as Pane) != null)
                {
                    foreach (Tabs child in childDockItem.GetAllTabs())
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// 将目标Pane相对与Tabs停靠在一边
        /// </summary>
        /// <param name="p_dockItem">要停靠的Pane</param>
        /// <param name="p_dockPosition">停靠位置</param>
        /// <param name="p_relativeTo">相对与Tabs</param>
        public void AddItem(Pane p_dockItem, DockPosition p_dockPosition, Tabs p_relativeTo)
        {
            if (p_dockItem == null || p_relativeTo == null || p_dockPosition == DockPosition.Center)
                return;

            p_dockItem.ClearValue(Pane.PosProperty);

            // 调整排序方式
            if (Orientation == Orientation.Vertical
                && (p_dockPosition == DockPosition.Left || p_dockPosition == DockPosition.Right)
                && Items.Count == 1)
            {
                Orientation = Orientation.Horizontal;
            }
            else if (Orientation == Orientation.Horizontal
                && (p_dockPosition == DockPosition.Top || p_dockPosition == DockPosition.Bottom)
                && Items.Count == 1)
            {
                Orientation = Orientation.Vertical;
            }

            Pane newItem;
            int index = Items.IndexOf(p_relativeTo);
            if (Orientation == Orientation.Vertical)
            {
                switch (p_dockPosition)
                {
                    case DockPosition.Top:
                        Items.Insert(index, p_dockItem);
                        return;

                    case DockPosition.Bottom:
                        Items.Insert(index + 1, p_dockItem);
                        return;

                    case DockPosition.Left:
                        Items.Remove(p_relativeTo);
                        newItem = new Pane();
                        newItem.Orientation = Orientation.Horizontal;
                        newItem.Items.Add(p_dockItem);
                        newItem.Items.Add(p_relativeTo);
                        Items.Insert(index, newItem);
                        return;

                    case DockPosition.Right:
                        Items.Remove(p_relativeTo);
                        newItem = new Pane();
                        newItem.Orientation = Orientation.Horizontal;
                        newItem.Items.Add(p_relativeTo);
                        newItem.Items.Add(p_dockItem);
                        Items.Insert(index, newItem);
                        return;
                }
            }
            else if (Orientation == Orientation.Horizontal)
            {
                switch (p_dockPosition)
                {
                    case DockPosition.Top:
                        Items.Remove(p_relativeTo);
                        newItem = new Pane();
                        newItem.Orientation = Orientation.Vertical;
                        newItem.Items.Add(p_dockItem);
                        newItem.Items.Add(p_relativeTo);
                        Items.Insert(index, newItem);
                        return;

                    case DockPosition.Bottom:
                        Items.Remove(p_relativeTo);
                        newItem = new Pane();
                        newItem.Orientation = Orientation.Vertical;
                        newItem.Items.Add(p_relativeTo);
                        newItem.Items.Add(p_dockItem);
                        Items.Insert(index, newItem);
                        return;

                    case DockPosition.Left:
                        Items.Insert(index, p_dockItem);
                        return;

                    case DockPosition.Right:
                        Items.Insert(index + 1, p_dockItem);
                        //Items.RemoveAt(index + 1);
                        //Items.Insert(index, p_relativeTo);
                        return;
                }
            }
        }

        /// <summary>
        /// 从父容器中移除当前Pane
        /// </summary>
        void RemoveFromParent()
        {
            ClearValue(TabItemPanel.SplitterChangeProperty);

            if (Parent is TabItemPanel panel)
            {
                panel.Owner?.Items.Remove(this);
            }
            else if (Parent is PanePanel winPnl)
            {
                OwnWin?.Items.Remove(this);
            }
            else if (Parent is ContentControl control)
            {
                control.ClearValue(ContentControl.ContentProperty);
            }
        }

        /// <summary>
        /// 无子项时直接移除当前Pane
        /// </summary>
        internal void RemoveUnused()
        {
            if (Items.Count == 0 && !IsInCenter)
            {
                RemoveFromParent();
            }
        }

        /// <summary>
        /// 获取内部子元素相对大小之和
        /// </summary>
        /// <returns></returns>
        internal Tabs.RelativeSizes GetSumOfRelativeSizes()
        {
            Tabs.RelativeSizes size = new Tabs.RelativeSizes();
            bool horizontal = this.Orientation == Orientation.Horizontal;
            foreach (var item in Items)
            {
                if (item.Visibility == Visibility.Collapsed)
                    continue;

                double splitterChange = TabItemPanel.GetSplitterChange(item);
                double length = TabItemPanel.GetLength(item, horizontal);

                size.LengthSum += length;
                if (splitterChange == 0.0)
                {
                    size.WithoutChange += length;
                }
                else
                {
                    size.ChangesSum += splitterChange;
                    size.WithChangeSet += length;
                }
            }
            return size;
        }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            _itemsPanel = (TabItemPanel)GetTemplateChild("TabItemPanel");
            _itemsPanel.Owner = this;
            var resizer = (GridResizer)GetTemplateChild("Resizer");
            resizer.Owner = this;
            _isLoaded = true;
            LoadAllItems();
            UpdateChildrenResizer();
            Items.ItemsChanged += OnItemsChanged;
        }

        void LoadAllItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                AddItem(Items[i], i);
            }
        }

        void AddItem(FrameworkElement p_item, int p_index)
        {
            FrameworkElement elem = p_item;
            if (p_item is Tabs tabs)
            {
                tabs.IsInCenter = _isInCenter;
                tabs.IsInWindow = _isInWindow;
            }
            else if (p_item is Pane wi)
            {
                wi.IsInCenter = _isInCenter;
                wi.IsInWindow = _isInWindow;
            }
            else
            {
                throw new Exception("Pane子项类型为Tabs或Pane！");
            }
            _itemsPanel.Children.Insert(p_index, elem);
        }

        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemRemoved)
            {
                _itemsPanel.Children.RemoveAt(e.Index);
                RemoveUnused();
                RefreshInternal();
            }
            else if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                AddItem(Items[e.Index], e.Index);
                RefreshInternal();
            }
            else
            {
                throw new Exception("Pane不支持子项重置！");
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 子项集合变化时刷新
        /// </summary>
        void RefreshInternal()
        {
            int count = (from item in Items
                         where item.Visibility == Visibility.Visible
                         select item).Count();
            Visibility visibility = (count == 0) ? Visibility.Collapsed : Visibility.Visible;
            if (Visibility != visibility)
            {
                Visibility = visibility;
                if (Parent is TabItemPanel pnl)
                {
                    pnl.Owner?.RefreshInternal();
                }
                else if (Parent is ToolWindow win && visibility == Visibility.Collapsed)
                {
                    win.Close();
                }
            }
            UpdateChildrenResizer();
        }

        /// <summary>
        /// 更新内部的尺寸调节器
        /// </summary>
        void UpdateChildrenResizer()
        {
            bool isFirst = true;
            foreach (UIElement element in _itemsPanel.Children)
            {
                if (element.Visibility == Visibility.Collapsed)
                    continue;

                if (element is Tabs tabs)
                {
                    if (!isFirst)
                    {
                        tabs.ResizerPlacement = (Orientation == Orientation.Horizontal) ? ItemPlacement.Left : ItemPlacement.Top;
                    }
                    else
                    {
                        isFirst = false;
                        tabs.ResizerPlacement = null;
                    }
                }
                else if (element is Pane wi)
                {
                    if (!isFirst)
                    {
                        wi.ResizerPlacement = (Orientation == Orientation.Horizontal) ? ItemPlacement.Left : ItemPlacement.Top;
                    }
                    else
                    {
                        isFirst = false;
                        wi.ResizerPlacement = null;
                    }
                }
            }
        }
        #endregion
    }
}