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
        /// 尺寸调节器的位置，由父容器WinItem的Orientation决定
        /// </summary>
        public static readonly DependencyProperty ResizerPlacementProperty = DependencyProperty.Register(
            "ResizerPlacement",
            typeof(ItemPlacement?),
            typeof(Tabs),
            new PropertyMetadata(null));

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

        static Tabs()
        {
            EventManager.RegisterClassHandler(typeof(Tabs), GridResizer.PreviewResizeStartEvent, new EventHandler<ResizeEventArgs>(OnPreviewResize));
        }

        static void OnPreviewResize(object sender, ResizeEventArgs e)
        {
            Tabs tabs = sender as Tabs;
            e.ResizedTgt = tabs;
            WinItem wi = tabs.Owner;
            if (wi != null)
            {
                e.AffectedTgt = tabs.GetNextVisibleElement(wi, tabs);
            }
        }
        #endregion

        #region 成员变量
        bool _isInCenter;
        bool _isInWindow;
        #endregion

        #region 构造方法
        public Tabs()
        {
            // PhoneUI模式时不在可视树
            if (!AtSys.IsPhoneUI)
            {
                // 若用DefaultStyleKey，当前控件在xaml文件有子元素时，uno中不调用OnApplyTemplate！
                // uno中设置Style时同步调用OnApplyTemplate，即构造方法直接调用了OnApplyTemplate！
                Style = (Style)Application.Current.Resources["DefaultTabs"];
            }
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
        /// 获取尺寸调节器的位置，由父容器WinItem的Orientation决定
        /// </summary>
        public ItemPlacement? ResizerPlacement
        {
            get { return (ItemPlacement?)GetValue(ResizerPlacementProperty); }
            internal set { SetValue(ResizerPlacementProperty, value); }
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
        /// 获取所属的WinItem
        /// </summary>
        public WinItem Owner
        {
            get
            {
                if (Parent is TabItemPanel pnl)
                    return pnl.Owner;
                return null;
            }
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
                    foreach (var item in Items)
                    {
                        Tab sect = item as Tab;
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
                    foreach (var item in Items)
                    {
                        Tab sect = item as Tab;
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
        /// <param name="p_winItem"></param>
        /// <param name="p_dockPosition"></param>
        public void AddItem(WinItem p_winItem, DockPosition p_dockPosition)
        {
            if (p_winItem == null)
                return;

            if (p_dockPosition == DockPosition.Center)
            {
                // 停靠在中部，合并WinItem中的所有标签项
                foreach (Tab item in GetAndRemoveItems(p_winItem))
                {
                    Items.Add(item);
                }
            }
            else
            {
                // 停靠在当前Tabs的一边
                Owner.AddItem(p_winItem, p_dockPosition, this);
                p_winItem.RemoveUnused();
            }
        }

        /// <summary>
        /// 从父容器中移除当前Tabs
        /// </summary>
        public void RemoveFromParent()
        {
            var par = Owner;
            if (par != null && par.Items.Contains(this))
            {
                ClearValue(TabItemPanel.SplitterChangeProperty);
                par.Items.Remove(this);
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 增删子项
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged()
        {
            UpdateTabStrip();
            if (Items.Count == 0)
                RemoveFromParent();
        }

        protected override void InitItem(TabItem p_item)
        {
            base.InitItem(p_item);
            Tab pane = p_item as Tab;
            if (pane != null)
            {
                pane.IsInCenter = IsInCenter;
                pane.IsInWindow = IsInWindow;
            }
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
            if (Owner != null)
            {
                parentSize = Owner.RenderSize;
                isHor = Owner.Orientation == Orientation.Horizontal;
                RelativeSizes sumOfSizes = Owner.GetSumOfRelativeSizes();

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
                bool isParentHor = Owner.Orientation == Orientation.Horizontal;
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
                int index = Owner.Items.IndexOf(this);
                if ((dock == DockPosition.Right) || (dock == DockPosition.Bottom))
                {
                    index++;
                }

                bool isDockHor = (dock == DockPosition.Left) || (dock == DockPosition.Right);
                bool isParentHor = Owner.Orientation == Orientation.Horizontal;
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
                        FrameworkElement element = Owner.Items[i] as FrameworkElement;
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
                return base.TransformToVisual(Owner).TransformPoint(topLeft);
            }
            UIElement firstChild = Owner.Items[0] as UIElement;
            return firstChild.TransformToVisual(Owner).TransformPoint(topLeft);
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

        #region 加载过程
#if UWP
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitTemplate();
        }
#else
        protected override void OnUnoLoaded()
        {
            InitTemplate();
        }
#endif

        void InitTemplate()
        {
            TabHeader header = GetTemplateChild("HeaderElement") as TabHeader;
            if (header != null)
                header.Owner = this;
            UpdateTabStrip();
        }
        #endregion

        #region 内部方法
        void UpdateTabStrip()
        {
            _itemsPanel.Visibility = Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取容器的所有子项并从容器移除
        /// </summary>
        /// <param name="p_winItem"></param>
        /// <returns></returns>
        IEnumerable<Tab> GetAndRemoveItems(WinItem p_winItem)
        {
            foreach (var item in p_winItem.Items)
            {
                if (item is Tabs childTabs)
                {
                    int index = 0;
                    while (index < childTabs.Items.Count)
                    {
                        Tab child = childTabs.Items[index] as Tab;
                        if (child != null)
                        {
                            childTabs.Items.RemoveAt(index);
                            yield return child;
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
                else if (item is WinItem childWinItem)
                {
                    foreach (Tab child in GetAndRemoveItems(childWinItem))
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
            for (int i = p_container.Items.IndexOf(p_sect) - 1; i > -1; i--)
            {
                if (p_container.Items[i].Visibility == Visibility.Visible)
                {
                    return p_container.Items[i];
                }
            }
            return null;
        }
        #endregion
    }
}