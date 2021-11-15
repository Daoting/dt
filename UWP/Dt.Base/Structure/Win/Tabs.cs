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
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        /// 尺寸调节器的位置，由父容器Pane的Orientation决定
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
        #endregion

        #region 成员变量
        bool _isInCenter;
        bool _isInWindow;
        #endregion

        #region 构造方法
        public Tabs()
        {
            // PhoneUI模式时不在可视树，省去uno在xaml自动生成代码时调用ApplyTemplate
            if (!Kit.IsPhoneUI)
                DefaultStyleKey = typeof(Tabs);
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
        /// 获取尺寸调节器的位置，由父容器Pane的Orientation决定
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
                            sect.IsFloating = _isInWindow;
                    }

                    if (!value)
                    {
                        ShowHeader = true;
                    }
                }
            }
        }

        /// <summary>
        /// 所属Pane
        /// </summary>
        internal Pane OwnWinItem
        {
            get
            {
                if (Parent is TabItemPanel pnl)
                    return pnl.Owner;
                return null;
            }
        }

        /// <summary>
        /// 所属Win
        /// </summary>
        internal Win OwnWin
        {
            get { return OwnWinItem?.OwnWin; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 将目标Pane的项停靠，合并到当前Tabs或停靠在一边
        /// </summary>
        /// <param name="p_winItem"></param>
        /// <param name="p_dockPosition"></param>
        public void AddItem(Pane p_winItem, DockPosition p_dockPosition)
        {
            if (p_winItem == null)
                return;

            if (p_dockPosition == DockPosition.Center)
            {
                // 停靠在中部，合并Pane中的所有标签项
                var ls = GetAllChildTab(p_winItem);
                LayoutManager.ClearItems(p_winItem);
                foreach (Tab item in ls)
                {
                    Items.Add(item);
                }
            }
            else
            {
                // 停靠在当前Tabs的一边
                OwnWinItem.AddItem(p_winItem, p_dockPosition, this);
                p_winItem.RemoveUnused();
            }
        }

        /// <summary>
        /// 从父容器中移除当前Tabs
        /// </summary>
        public void RemoveFromParent()
        {
            var par = OwnWinItem;
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
                pane.IsFloating = IsInWindow;
            }
        }

        protected override void OnSwappedItem()
        {
            OwnWin?.OnLayoutChanged();
        }
        #endregion

        #region 拖拽中可停靠区域
        /// <summary>
        /// 拖拽过程中计算在当前Tabs中的可停靠区域
        /// </summary>
        /// <param name="p_dockPos"></param>
        /// <param name="p_dockItem"></param>
        /// <returns></returns>
        internal Rect GetRectDimenstion(DockPosition p_dockPos, Pane p_dockItem)
        {
            Point topLeft = new Point();
            Size parentSize = Size.Empty;
            Size size = new Size(0.0, 0.0);
            bool isHor = false;
            if (OwnWinItem != null)
            {
                parentSize = OwnWinItem.RenderSize;
                isHor = OwnWinItem.Orientation == Orientation.Horizontal;
                RelativeSizes sumOfSizes = OwnWinItem.GetSumOfRelativeSizes();

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
                bool isParentHor = OwnWinItem.Orientation == Orientation.Horizontal;
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
                int index = OwnWinItem.Items.IndexOf(this);
                if ((dock == DockPosition.Right) || (dock == DockPosition.Bottom))
                {
                    index++;
                }

                bool isDockHor = (dock == DockPosition.Left) || (dock == DockPosition.Right);
                bool isParentHor = OwnWinItem.Orientation == Orientation.Horizontal;
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
                        FrameworkElement element = OwnWinItem.Items[i] as FrameworkElement;
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
                return base.TransformToVisual(OwnWinItem).TransformPoint(topLeft);
            }
            UIElement firstChild = OwnWinItem.Items[0] as UIElement;
            return firstChild.TransformToVisual(OwnWinItem).TransformPoint(topLeft);
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
        protected override void OnLoadTemplate()
        {
            base.OnLoadTemplate();
            var header = (TabHeader)GetTemplateChild("HeaderElement");
            header.Owner = this;
            var resizer = (GridResizer)GetTemplateChild("Resizer");
            resizer.Owner = this;
            UpdateTabStrip();
        }
        #endregion

        #region 内部方法
        void UpdateTabStrip()
        {
            _itemsPanel.Visibility = Items.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 递归获取容器内所有的Tab
        /// </summary>
        /// <param name="p_winItem"></param>
        /// <returns></returns>
        List<Tab> GetAllChildTab(Pane p_winItem)
        {
            List<Tab> ls = new List<Tab>();
            if (p_winItem != null && p_winItem.Items.Count > 0)
            {
                foreach (var item in p_winItem.Items)
                {
                    if (item is Tabs childTabs)
                    {
                        ls.AddRange(childTabs.Items.OfType<Tab>());
                    }
                    else if (item is Pane childWinItem)
                    {
                        ls.AddRange(GetAllChildTab(childWinItem));
                    }
                }
            }
            return ls;
        }

        internal struct RelativeSizes
        {
            internal double WithoutChange;
            internal double WithChangeSet;
            internal double ChangesSum;
            internal double LengthSum;
        }
        #endregion
    }
}