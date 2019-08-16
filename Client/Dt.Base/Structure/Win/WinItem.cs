#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 可停靠项，内部子项为Tabs或WinItem
    /// </summary>
    public partial class WinItem : ItemsControl
    {
        #region 静态内容
        public static readonly DependencyProperty DockStateProperty = DependencyProperty.Register(
            "DockState",
            typeof(WinItemState),
            typeof(WinItem),
            new PropertyMetadata(WinItemState.DockedLeft, OnDockStateChanged));

        public static readonly DependencyProperty InitWidthProperty = DependencyProperty.Register(
            "InitWidth",
            typeof(double),
            typeof(WinItem),
            new PropertyMetadata(400.0));

        public static readonly DependencyProperty InitHeightProperty = DependencyProperty.Register(
            "InitHeight",
            typeof(double),
            typeof(WinItem),
            new PropertyMetadata(300.0));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation",
            typeof(Orientation),
            typeof(WinItem),
            new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        public static readonly DependencyProperty FloatPosProperty = DependencyProperty.Register(
            "FloatPos",
            typeof(FloatPosition),
            typeof(WinItem),
            new PropertyMetadata(FloatPosition.Center));

        public static readonly DependencyProperty ResizerPlacementProperty = DependencyProperty.Register(
            "ResizerPlacement",
            typeof(ItemPlacement?),
            typeof(WinItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FloatLocationProperty = DependencyProperty.Register(
            "FloatLocation",
            typeof(Point),
            typeof(WinItem),
            new PropertyMetadata(new Point(0.0, 0.0)));

        public static readonly DependencyProperty FloatSizeProperty = DependencyProperty.Register(
            "FloatSize",
            typeof(Size),
            typeof(WinItem),
            new PropertyMetadata(new Size(300.0, 300.0)));


        static void OnDockStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WinItem item = (WinItem)sender;
            switch (item.DockState)
            {
                case WinItemState.DockedLeft:
                    item.ResizerPlacement = ItemPlacement.Right;
                    break;

                case WinItemState.DockedBottom:
                    item.ResizerPlacement = ItemPlacement.Top;
                    break;

                case WinItemState.DockedRight:
                    item.ResizerPlacement = ItemPlacement.Left;
                    break;

                case WinItemState.DockedTop:
                    item.ResizerPlacement = ItemPlacement.Bottom;
                    break;

                default:
                    item.ResizerPlacement = null;
                    break;
            }

            FrameworkElement parent = item.GetParent();
            if (parent != null)
            {
                parent.InvalidateMeasure();
            }
        }

        static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WinItem item = (WinItem)sender;
            if (item._isLoaded)
            {
                item._itemsPanel.Orientation = (Orientation)e.NewValue;
                item.UpdateChildrenResizer();
            }
        }

#if UWP
        static WinItem()
        {
            EventManager.RegisterClassHandler(typeof(WinItem), GridResizer.PreviewResizeStartEvent, new EventHandler<ResizeEventArgs>(OnPreviewResize));
        }
#endif

        static void OnPreviewResize(object sender, ResizeEventArgs e)
        {
            WinItem container = sender as WinItem;
            e.ResizedTgt = container;
            WinItem parent = container.Container;
            if (parent != null)
            {
                e.AffectedTgt = parent.GetNextVisibleElement(container);
            }
        }

        #endregion

        #region 成员变量
        TabItemPanel _itemsPanel;
        Win _owner;
        bool _isLoaded;
        bool _isInCenter;
        bool _isInWindow;
        bool _recicled;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        public WinItem()
        {
            DefaultStyleKey = typeof(WinItem);
            Loaded += OnLoaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置停靠状态
        /// </summary>
        public WinItemState DockState
        {
            get { return (WinItemState)GetValue(DockStateProperty); }
            set { SetValue(DockStateProperty, value); }
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
        /// 获取尺寸调节器的位置，由DockState决定
        /// </summary>
        public ItemPlacement? ResizerPlacement
        {
            get { return (ItemPlacement?)GetValue(ResizerPlacementProperty); }
            internal set { SetValue(ResizerPlacementProperty, value); }
        }

        /// <summary>
        /// 获取父容器
        /// </summary>
        public WinItem Container
        {
            get { return (Parent as WinItem); }
        }

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

                    Tabs sect;
                    WinItem dockItem;
                    foreach (object item in Items)
                    {
                        DependencyObject con = ContainerFromItem(item);
                        if ((sect = con as Tabs) != null)
                        {
                            sect.IsInCenter = _isInCenter;
                        }
                        else if ((dockItem = con as WinItem) != null)
                        {
                            dockItem.IsInCenter = _isInCenter;
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

                    Tabs sect;
                    WinItem dockItem;
                    foreach (object item in Items)
                    {
                        DependencyObject con = ContainerFromItem(item);
                        if ((sect = con as Tabs) != null)
                        {
                            sect.IsInWindow = _isInWindow;
                        }
                        else if ((dockItem = con as WinItem) != null)
                        {
                            dockItem.IsInWindow = _isInWindow;
                        }
                    }
                }
            }
        }
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
                WinItem childDockItem;
                if ((childSect = item as Tabs) != null)
                {
                    foreach (Tab child in childSect.Items)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
                else if ((childDockItem = item as WinItem) != null)
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
                WinItem childDockItem;
                if ((childSect = item as Tabs) != null)
                {
                    yield return childSect;
                }
                else if ((childDockItem = item as WinItem) != null)
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
        /// 将目标WinItem相对与Tabs停靠在一边
        /// </summary>
        /// <param name="p_dockItem">要停靠的WinItem</param>
        /// <param name="p_dockPosition">停靠位置</param>
        /// <param name="p_relativeTo">相对与Tabs</param>
        public void AddItem(WinItem p_dockItem, DockPosition p_dockPosition, Tabs p_relativeTo)
        {
            if (p_dockItem == null || p_relativeTo == null || p_dockPosition == DockPosition.Center)
                return;

            p_dockItem.ClearValue(WinItem.DockStateProperty);

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

            WinItem newItem;
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
                        newItem = new WinItem();
                        newItem.Orientation = Orientation.Horizontal;
                        newItem.Items.Add(p_dockItem);
                        newItem.Items.Add(p_relativeTo);
                        Items.Insert(index, newItem);
                        return;

                    case DockPosition.Right:
                        Items.Remove(p_relativeTo);
                        newItem = new WinItem();
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
                        newItem = new WinItem();
                        newItem.Orientation = Orientation.Vertical;
                        newItem.Items.Add(p_dockItem);
                        newItem.Items.Add(p_relativeTo);
                        Items.Insert(index, newItem);
                        return;

                    case DockPosition.Bottom:
                        Items.Remove(p_relativeTo);
                        newItem = new WinItem();
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
        /// 从父容器中移除当前WinItem
        /// </summary>
        internal void RemoveFromParent()
        {
            ItemsControl itemsControl;
            Panel panel;
            ContentControl control;
            ClearValue(TabItemPanel.SplitterChangeProperty);

            if ((itemsControl = Parent as ItemsControl) != null)
            {
                itemsControl.Items.Remove(this);
            }
            else if ((panel = Parent as Panel) != null)
            {
                panel.Children.Remove(this);
            }
            else if ((control = base.Parent as ContentControl) != null)
            {
                control.ClearValue(ContentControl.ContentProperty);
            }
        }

        /// <summary>
        /// 无子项时直接移除当前WinItem，只一个子项时将子项向上级合并并移除当前WinItem
        /// </summary>
        internal void RemoveUnused()
        {
            if (Items.Count == 0)
            {
                _recicled = true;
                RemoveFromParent();
            }

            // 当WinItem的子项个数多于一个，且最后一个子项为也为WinItem的情况下，
            // 在调整Win布局后,恢复初始布局时，要调用LayoutManager类中的ClearAllItems()方法，
            // 在ClearItems(_owner)时，会进入此方法,当Item.Count == 1时，进入到parentPanel != null分支时，出错。

            //else if (Items.Count == 1)
            //{
            //    // 重新整合子项
            //    Tabs childSect = Items[0] as Tabs;
            //    WinItem childDockItem = Items[0] as WinItem;

            //    if (Container != null)
            //    {
            //        // 父容器为WinItem，将子项移动到父容器
            //        _recicled = true;
            //        int index = Container.Items.IndexOf(this);
            //        if (childSect != null)
            //        {
            //            Items.Remove(childSect);
            //            Container.Items.Insert(index, childSect);
            //        }
            //        else if (childDockItem != null)
            //        {
            //            Items.Remove(childDockItem);
            //            Container.Items.Insert(index, childDockItem);
            //        }
            //        Container.Items.Remove(this);
            //    }
            //    else if (childDockItem != null)
            //    {
            //        Panel parentPanel = Parent as Panel;
            //        ToolWindow parentWin = Parent as ToolWindow;
            //        // 子项为WinItem，将子项提到上级
            //        if (parentWin != null)
            //        {
            //            _recicled = true;
            //            Items.Remove(childDockItem);
            //            parentWin.Content = childDockItem;
            //        }
            //        else if (parentPanel != null)
            //        {
            //            int index = parentPanel.Children.IndexOf(this);
            //            parentPanel.Children.RemoveAt(index);
            //            parentPanel.Children.Insert(index, childDockItem);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 获取内部子元素相对大小之和
        /// </summary>
        /// <returns></returns>
        internal Tabs.RelativeSizes GetSumOfRelativeSizes()
        {
            Tabs.RelativeSizes size = new Tabs.RelativeSizes();
            bool horizontal = this.Orientation == Orientation.Horizontal;
            foreach (object item in Items)
            {
                FrameworkElement con = item as FrameworkElement;
                if (con != null && con.Visibility != Visibility.Collapsed)
                {
                    double splitterChange = TabItemPanel.GetSplitterChange(con);
                    double length = TabItemPanel.GetLength(con, horizontal);

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
            }
            return size;
        }

        #endregion

        #region 重写方法
        /// <summary>
        /// 增删子项
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            if (!_recicled && _isLoaded)
            {
                RemoveUnused();
                if (Parent != null)
                    RefreshInternal();
            }
        }

        /// <summary>
        /// 准备指定元素以显示指定项
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            WinItem dockItem;
            Tabs sect;
            if ((sect = element as Tabs) != null)
            {
                sect.IsInCenter = _isInCenter;
                sect.IsInWindow = _isInWindow;
            }
            else if ((dockItem = element as WinItem) != null)
            {
                dockItem.IsInCenter = _isInCenter;
                dockItem.IsInWindow = _isInWindow;
            }
        }

        /// <summary>
        /// 确定指定项是否为子项的容器
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is Tabs || item is WinItem);
        }

        /// <summary>
        /// 创建或标识用于显示给定项的元素
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Tabs();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            _itemsPanel = this.FindChildByType<TabItemPanel>();
            _itemsPanel.Orientation = Orientation;
            _owner = this.FindParentByType<Win>();
            _isLoaded = true;
            UpdateChildrenResizer();
        }

        /// <summary>
        /// 子项集合变化时刷新
        /// </summary>
        void RefreshInternal()
        {
            UpdateWindowHeader();
            int count = (from item in Items.OfType<UIElement>()
                         where item.Visibility == Visibility.Visible
                         select item).Count();
            Visibility visibility = (count == 0) ? Visibility.Collapsed : Visibility.Visible;
            if (Visibility != visibility)
            {
                Visibility = visibility;
                if (Container != null)
                {
                    Container.RefreshInternal();
                }
                else
                {
                    ToolWindow win = Parent as ToolWindow;
                    if ((win != null) && (visibility == Visibility.Collapsed))
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

                Tabs sect = element as Tabs;
                WinItem dockItem = element as WinItem;
                if (sect != null)
                {
                    if (!isFirst)
                    {
                        sect.ResizerPlacement = (Orientation == Orientation.Horizontal) ? ItemPlacement.Left : ItemPlacement.Top;
                    }
                    else
                    {
                        isFirst = false;
                        sect.ResizerPlacement = null;
                    }
                }
                else if (dockItem != null)
                {
                    if (!isFirst)
                    {
                        dockItem.ResizerPlacement = (Orientation == Orientation.Horizontal) ? ItemPlacement.Left : ItemPlacement.Top;
                    }
                    else
                    {
                        isFirst = false;
                        dockItem.ResizerPlacement = null;
                    }
                }
            }
        }

        /// <summary>
        /// 更新窗口标题
        /// </summary>
        void UpdateWindowHeader()
        {
            ToolWindow window = Parent as ToolWindow;
            if (window != null)
            {
                window.UpdateHeader();
            }
            else if (Container != null)
            {
                Container.UpdateWindowHeader();
            }
        }

        /// <summary>
        /// 获取同一容器下的下一可见元素
        /// </summary>
        /// <param name="p_container"></param>
        /// <returns></returns>
        FrameworkElement GetNextVisibleElement(WinItem p_container)
        {
            for (int i = IndexFromContainer(p_container) - 1; i > -1; i--)
            {
                UIElement nextElement = ContainerFromIndex(i) as UIElement;
                if ((nextElement != null) && (nextElement.Visibility == Visibility.Visible))
                {
                    return (nextElement as FrameworkElement);
                }
            }
            return null;
        }

        #endregion
    }
}