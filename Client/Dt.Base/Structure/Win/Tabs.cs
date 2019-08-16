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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 包含标题栏的TabControl：
    /// 有标题栏，标签在下方，一个子项时不显示标签
    /// </summary>
    public partial class Tabs : TabControl
    {
        #region 静态内容
        /// <summary>
        /// 是否显示标题栏
        /// </summary>
        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register(
            "ShowHeader",
            typeof(bool),
            typeof(Tabs),
            new PropertyMetadata(true));

        /// <summary>
        /// 是否显示标签
        /// </summary>
        public static readonly DependencyProperty ShowStripProperty = DependencyProperty.Register(
            "ShowStrip",
            typeof(bool),
            typeof(Tabs),
            new PropertyMetadata(true));

        /// <summary>
        /// 尺寸调节器的位置，由父容器WinItem的Orientation决定
        /// </summary>
        public static readonly DependencyProperty ResizerPlacementProperty = DependencyProperty.Register(
            "ResizerPlacement",
            typeof(ItemPlacement?),
            typeof(Tabs),
            new PropertyMetadata(null));

        /// <summary>
        /// 是否显示切换Outlook样式的按钮
        /// </summary>
        public static readonly DependencyProperty OutlookButtonVisibleProperty = DependencyProperty.Register(
            "OutlookButtonVisible",
            typeof(Visibility),
            typeof(Tabs),
            new PropertyMetadata(Visibility.Collapsed));

        /// <summary> 
        /// 初始宽度
        /// </summary>
        public static readonly DependencyProperty InitWidthProperty = DependencyProperty.Register(
            "InitWidth",
            typeof(double),
            typeof(Tabs),
            new PropertyMetadata(200.0));

        /// <summary> 
        /// 初始高度
        /// </summary>
        public static readonly DependencyProperty InitHeightProperty = DependencyProperty.Register(
            "InitHeight",
            typeof(double),
            typeof(Tabs),
            new PropertyMetadata(200.0));

#if UWP
        static Tabs()
        {
            EventManager.RegisterClassHandler(typeof(Tabs), GridResizer.PreviewResizeStartEvent, new EventHandler<ResizeEventArgs>(OnPreviewResize));
        }
#endif
        static void OnPreviewResize(object sender, ResizeEventArgs e)
        {
            Tabs sect = sender as Tabs;
            e.ResizedTgt = sect;
            WinItem container = sect.Container;
            if (container != null)
            {
                e.AffectedTgt = sect.GetNextVisibleElement(container, sect);
            }
        }
        #endregion

        #region 成员变量
        bool _isInCenter;
        bool _isInWindow;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        public Tabs()
        {
            DefaultStyleKey = typeof(Tabs);
            Loaded += OnLoaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取是否显示标题栏
        /// </summary>
        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        /// <summary>
        /// 获取是否显示标签
        /// </summary>
        public bool ShowStrip
        {
            get { return (bool)GetValue(ShowStripProperty); }
            internal set { SetValue(ShowStripProperty, value); }
        }

        /// <summary>
        /// 获取尺寸调节器的位置，由父容器WinItem的Orientation决定
        /// </summary>
        public ItemPlacement? ResizerPlacement
        {
            get { return (ItemPlacement?)GetValue(ResizerPlacementProperty); }
            internal set { SetValue(ResizerPlacementProperty, value); }
        }

        /// <summary>
        /// 获取是否显示切换Outlook样式的按钮
        /// </summary>
        public Visibility OutlookButtonVisible
        {
            get { return (Visibility)GetValue(OutlookButtonVisibleProperty); }
            internal set { SetValue(OutlookButtonVisibleProperty, value); }
        }

        /// <summary>
        /// 获取设置停靠时的初始宽度
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
        /// 获取父容器
        /// </summary>
        public WinItem Container
        {
            get { return (Parent as WinItem); }
        }

        /// <summary>
        /// 获取设置当前Tabs是否停靠在中部
        /// </summary>
        public bool IsInCenter
        {
            get { return _isInCenter; }
            internal set
            {
                if (_isInCenter != value)
                {
                    _isInCenter = value;
                    foreach (object item in Items)
                    {
                        Tab sect = item as Tab;
                        if (sect == null)
                            sect = ContainerFromItem(item) as Tab;

                        if (sect != null)
                            sect.IsInCenter = _isInCenter;
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
            internal set
            {
                if (_isInWindow != value)
                {
                    _isInWindow = value;
                    foreach (object item in Items)
                    {
                        Tab sect = item as Tab;
                        if (sect == null)
                            sect = ContainerFromItem(item) as Tab;
                        if (sect != null)
                            sect.IsInWindow = _isInWindow;
                    }

                    if (!value)
                    {
                        ShowHeader = true;
                    }
                }
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 将目标WinItem的项停靠，合并到当前Tabs或停靠在一边
        /// </summary>
        /// <param name="p_dockItem"></param>
        /// <param name="p_dockPosition"></param>
        public void AddItem(WinItem p_dockItem, DockPosition p_dockPosition)
        {
            if (p_dockItem == null
                || (p_dockItem.Container != null && p_dockItem.Container.Items.Contains(p_dockItem)))
            {
                throw new ArgumentNullException("要停靠的内容需要从父容器中移除！");
            }

            if (p_dockPosition == DockPosition.Center)
            {
                // 停靠在中部，合并WinItem中的所有标签项
                foreach (Tab item in GetAndRemoveItems(p_dockItem))
                {
                    Items.Add(item);
                }
            }
            else
            {
                // 停靠在当前Tabs的一边
                Container.AddItem(p_dockItem, p_dockPosition, this);
                p_dockItem.RemoveUnused();
            }
        }

        /// <summary>
        /// 移除指定的子项
        /// </summary>
        /// <param name="p_item"></param>
        public void RemoveItem(Tab p_item)
        {
            if (Items.Contains(p_item))
            {
                Items.Remove(p_item);
            }
            else if (!p_item.IsPinned)
            {
                AutoHideTab parentArea = p_item.Parent as AutoHideTab;
                if ((parentArea != null) && parentArea.Items.Contains(p_item))
                {
                    //parentArea.Remove(p_item);
                    //this.UnpinnedPanesInternal.Remove(p_item);
                }
            }
        }

        /// <summary>
        /// 从父容器中移除当前Tabs
        /// </summary>
        public void RemoveFromParent()
        {
            if ((Container != null) && Container.Items.Contains(this))
            {
                ClearValue(TabItemPanel.SplitterChangeProperty);
                Container.Items.Remove(this);
            }
        }

        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TabHeader header = GetTemplateChild("HeaderElement") as TabHeader;
            if (header != null)
                header.Owner = this;
        }

        /// <summary>
        /// 增删子项
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);

            if (_isLoaded)
            {
                UpdateTabStrip();
                if (Items.Count == 0)
                    RemoveFromParent();
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
            Tab pane = element as Tab;
            if (pane != null)
            {
                pane.IsInCenter = IsInCenter;
                pane.IsInWindow = IsInWindow;
            }
        }

        /// <summary>
        /// 确定指定项是否为子项的容器
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is Tab);
        }

        /// <summary>
        /// 创建或标识用于显示给定项的元素
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Tab();
        }

        /// <summary>
        /// 清除子项
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            Tab pane = element as Tab;
            if (pane != null)
            {
                pane.IsInCenter = false;
                pane.IsInWindow = false;
            }
        }

        /// <summary>
        /// IsAutoHide变化时不做操作
        /// </summary>
        protected override void OnIsAutoHideChanged()
        {
        }
        #endregion

        #region 拖拽中可停靠区域
        /// <summary>
        /// 拖拽过程中计算在当前Tabs中的可停靠区域
        /// </summary>
        /// <param name="p_dockPos"></param>
        /// <param name="p_dockItem"></param>
        /// <returns></returns>
        internal Rect GetRectDimenstion(DockPosition p_dockPos, WinItem p_dockItem)
        {
            Point topLeft = new Point();
            Size parentSize = Size.Empty;
            Size size = new Size(0.0, 0.0);
            bool isHor = false;
            if (Container != null)
            {
                parentSize = Container.RenderSize;
                isHor = Container.Orientation == Orientation.Horizontal;
                RelativeSizes sumOfSizes = Container.GetSumOfRelativeSizes();

                if (isHor)
                {
                    sumOfSizes.LengthSum += p_dockItem.InitWidth;
                    sumOfSizes.WithoutChange += p_dockItem.InitWidth;
                    size.Width = sumOfSizes.LengthSum;
                }
                else
                {
                    sumOfSizes.LengthSum += p_dockItem.InitHeight;
                    sumOfSizes.WithoutChange += p_dockItem.InitHeight;
                    size.Height = sumOfSizes.LengthSum;
                }

                size = GetSize(p_dockPos, sumOfSizes, parentSize, new Size(p_dockItem.InitWidth, p_dockItem.InitHeight));
                topLeft = GetTopLeft(p_dockPos, sumOfSizes, parentSize, size);
            }
            return new Rect(topLeft, size);
        }

        Size GetSize(DockPosition p_dock, RelativeSizes p_relativeSizes, Size p_parentSize, Size p_draggedSize)
        {
            Size size = base.RenderSize;
            if (p_dock != DockPosition.Center)
            {
                bool isDockHor = (p_dock == DockPosition.Left) || (p_dock == DockPosition.Right);
                bool isParentHor = Container.Orientation == Orientation.Horizontal;
                bool findHorizontal = isParentHor == (isDockHor == isParentHor);
                double length = 0.0;

                if (isDockHor != isParentHor)
                {
                    double relativeSizeSum = TabItemPanel.GetLength(this, isDockHor) + GetLength(p_draggedSize, isDockHor);
                    p_relativeSizes = GetRelativeSizesSum(relativeSizeSum);
                }

                length = GetRenderLength(
                    0.0,
                    GetLength(p_draggedSize, findHorizontal),
                    GetLength(p_parentSize, findHorizontal),
                    p_relativeSizes);

                if (findHorizontal)
                {
                    size.Width = length;
                    return size;
                }
                size.Height = length;
            }
            return size;
        }

        Point GetTopLeft(DockPosition dock, RelativeSizes relativeSizes, Size parentRenderSize, Size draggedElementRenderSize)
        {
            Point topLeft = new Point();
            bool shouldTransform = true;
            if (dock != DockPosition.Center)
            {
                int index = Container.Items.IndexOf(this);
                if ((dock == DockPosition.Right) || (dock == DockPosition.Bottom))
                {
                    index++;
                }

                bool isDockHor = (dock == DockPosition.Left) || (dock == DockPosition.Right);
                bool isParentHor = Container.Orientation == Orientation.Horizontal;
                double length = 0.0;

                if (isParentHor != isDockHor)
                {
                    if ((dock == DockPosition.Right) || (dock == DockPosition.Bottom))
                    {
                        length = GetLength(parentRenderSize, isDockHor) - GetLength(draggedElementRenderSize, isDockHor);
                    }
                }
                else
                {
                    for (int i = 0; i < index; i++)
                    {
                        FrameworkElement element = Container.Items[i] as FrameworkElement;
                        if (element.Visibility == Visibility.Visible)
                        {
                            length += GetRenderLength(
                                TabItemPanel.GetSplitterChange(element),
                                TabItemPanel.GetLength(element, isParentHor),
                                GetLength(parentRenderSize, isParentHor),
                                relativeSizes);
                        }
                    }
                    shouldTransform = false;
                }
                if (isParentHor == (isDockHor == isParentHor))
                {
                    topLeft.X = length;
                }
                else
                {
                    topLeft.Y = length;
                }
            }
            if (shouldTransform)
            {
                return base.TransformToVisual(Container).TransformPoint(topLeft);
            }
            UIElement firstChild = Container.Items[0] as UIElement;
            return firstChild.TransformToVisual(Container).TransformPoint(topLeft);
        }

        static double GetRenderLength(double splitterChange, double p_length, double p_availableLength, RelativeSizes relativeSizes)
        {
            if (splitterChange == 0.0)
            {
                return (p_length * (p_availableLength / (relativeSizes.WithoutChange + relativeSizes.WithChangeSet)));
            }
            return (((splitterChange * relativeSizes.WithChangeSet) / relativeSizes.ChangesSum) * (p_availableLength / (relativeSizes.WithoutChange + relativeSizes.WithChangeSet)));
        }

        static double GetLength(Size size, bool isHorizontal)
        {
            if (!isHorizontal)
            {
                return size.Height;
            }
            return size.Width;
        }

        static RelativeSizes GetRelativeSizesSum(double sum)
        {
            RelativeSizes rs = new RelativeSizes();
            rs.ChangesSum = 0.0;
            rs.LengthSum = sum;
            rs.WithChangeSet = 0.0;
            rs.WithoutChange = sum;
            return rs;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            UpdateTabStrip();
        }

        /// <summary>
        /// 更新标签显示状态、边距、Outlook样式切换按钮
        /// </summary>
        void UpdateTabStrip()
        {
            if (Items.Count > 1)
            {
                ShowStrip = true;
                OutlookButtonVisible = Visibility.Visible;
            }
            else
            {
                ShowStrip = false;
                OutlookButtonVisible = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 获取容器的所有子项并从容器移除
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tab> GetAndRemoveItems(WinItem p_dockItem)
        {
            foreach (object item in p_dockItem.Items)
            {
                Tabs childSect;
                WinItem childDockItem;
                if ((childSect = item as Tabs) != null)
                {
                    int index = 0;
                    while (index < childSect.Items.Count)
                    {
                        Tab child = childSect.Items[index] as Tab;
                        if (child != null)
                        {
                            childSect.Items.RemoveAt(index);
                            yield return child;
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
                else if ((childDockItem = item as WinItem) != null)
                {
                    foreach (Tab child in GetAndRemoveItems(childDockItem))
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        internal struct RelativeSizes
        {
            internal double WithoutChange;
            internal double WithChangeSet;
            internal double ChangesSum;
            internal double LengthSum;
        }

        FrameworkElement GetNextVisibleElement(WinItem p_container, Tabs p_sect)
        {
            for (int i = p_container.IndexFromContainer(p_sect) - 1; i > -1; i--)
            {
                UIElement nextElement = p_container.ContainerFromIndex(i) as UIElement;
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