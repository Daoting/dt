#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endregion

namespace Dt.Base.TreeViews
{
    /// <summary>
    /// 树节点布局面板
    /// </summary>
    public partial class TvPanel : Panel
    {
        #region 成员变量
        const double PanelMaxHeight = 5000;
        static Size _sizeEmpty = new Size();

        Tv _owner;
        bool _initVirRow;
        double _rowHeight;
        double _pageHeight;

        /// <summary>
        /// 面板最大尺寸，宽高始终不为无穷大！
        /// </summary>
        Size _maxSize = Size.Empty;

        /// <summary>
        /// 以滚动栏为参照物，面板与滚动栏的水平距离，面板在右侧时为正数
        /// </summary>
        double _deltaX;

        /// <summary>
        /// 以滚动栏为参照物，面板与滚动栏的垂直距离，面板在下方时为正数
        /// </summary>
        double _deltaY;
        
        /// <summary>
        /// 数据与顶部的间距，因为筛选框、工具栏占用
        /// </summary>
        double _topMargin;
        #endregion

        public TvPanel(Tv p_owner)
        {
            _owner = p_owner;
            Background = Res.TransparentBrush;

            if (!_owner.IsVirtualized)
                LoaRealRows();
        }

        /// <summary>
        /// 设置面板的最大尺寸，宽高始终不为无穷大！
        /// </summary>
        internal void SetMaxSize(Size p_size)
        {
            // 尺寸变化大于2有效，否则iOS版易造成死循环，每次 p_size 有微小变化！！！
            if (Math.Abs(_maxSize.Width - p_size.Width) > 2 || Math.Abs(_maxSize.Height - p_size.Height) > 2)
            {
                _maxSize = p_size;
                if (_owner.IsVirtualized)
                    LoadVirRows();
            }
        }

        /// <summary>
        /// 切换模板、调整是否采用虚拟化时重新加载
        /// </summary>
        internal void Reload()
        {
            if (_owner.IsVirtualized)
                LoadVirRows();
            else
                LoaRealRows();
        }

        /// <summary>
        /// 切换数据源时
        /// </summary>
        internal void OnRowsChanged()
        {
            if (_owner.IsVirtualized)
            {
                if (_initVirRow)
                    InvalidateMeasure();
                else
                    LoadVirRows();
            }
            else
            {
                LoaRealRows();
            }
        }

        /// <summary>
        /// 从根节点展开到当前节点，并滚动到可视范围
        /// </summary>
        /// <param name="p_item"></param>
        internal void ScrollIntoItem(TvItem p_item)
        {
            if (p_item == null)
                return;

            p_item.ExpandAll();
            UpdateLayout();

            var scroll = _owner.Scroll;
            if (scroll.ScrollableHeight == 0)
                return;

            if (_owner.IsInnerScroll)
            {
                // 内置滚动栏时
                double scrollBottom = scroll.VerticalOffset + scroll.ViewportHeight;
                if (_owner.IsVirtualized)
                {
                    double top = _owner.RootItems.GetVerIndex(p_item) * _rowHeight;
                    if (top < scroll.VerticalOffset)
                        scroll.ChangeView(null, top, null);
                    else if (top + _rowHeight > scrollBottom)
                        scroll.ChangeView(null, scroll.VerticalOffset + (top + _rowHeight - scrollBottom), null);
                }
                else
                {
                    Rect rc = _owner.RootItems.GetExpandedPostion(p_item, this);
                    if (rc.Top < scroll.VerticalOffset)
                        scroll.ChangeView(null, rc.Top, null);
                    else if (rc.Bottom > scrollBottom)
                        scroll.ChangeView(null, scroll.VerticalOffset + (rc.Bottom - scrollBottom), null);
                }
            }
            else
            {
                // 滚动栏在外部
                var pt = TransformToVisual(_owner.Scroll).TransformPoint(new Point());
                if (_owner.IsVirtualized)
                {
                    double top = _owner.RootItems.GetVerIndex(p_item) * _rowHeight + pt.Y + scroll.VerticalOffset;
                    if (top < scroll.VerticalOffset)
                        scroll.ChangeView(null, top, null);
                    else if (top + _rowHeight > scroll.ViewportHeight)
                        scroll.ChangeView(null, top + _rowHeight - scroll.ViewportHeight, null);
                }
                else
                {
                    Rect rc = _owner.RootItems.GetExpandedPostion(p_item, this);
                    double top = rc.Top + pt.Y + scroll.VerticalOffset;
                    if (top < scroll.VerticalOffset)
                        scroll.ChangeView(null, top, null);
                    else if (rc.Bottom + pt.Y + scroll.VerticalOffset > scroll.ViewportHeight)
                        scroll.ChangeView(null, scroll.VerticalOffset + rc.Bottom + pt.Y - scroll.ViewportHeight, null);
                }
            }
        }

        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner.View == null)
                return new Size();

            // 虚拟行/真实行
            Size size = _owner.IsVirtualized ? MeasureVirRows() : MeasureRealRows();

            // 筛选框工具栏
            _topMargin = 0;
            if (_filterBox != null)
            {
                _filterBox.Measure(_maxSize);
                _topMargin = _filterBox.DesiredSize.Height;
                size.Height += _topMargin;
            }
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_owner.View == null)
                return finalSize;

            if (!_owner.IsInnerScroll)
            {
                // 外部有ScrollViewer时
                // 面板与ScrollViewer的相对距离，以滚动栏为参照物，面板在右下方时为正数
                // uno4.1.8 后和WinUI一致！！！之前uno中面板与Scroll的相对距离始终为滚动栏未移动时之间的距离！
                if (_owner.Scroll.ActualHeight > 0)
                {
                    // 当切换win时，再次显示Scroll时ActualHeight为0，计算相对位置错误！采用切换前的相对位置
                    var pt = TransformToVisual(_owner.Scroll).TransformPoint(new Point());
                    _deltaX = pt.X;
                    _deltaY = pt.Y;
                }
            }
            else
            {
                // 内置滚动栏时，垂直距离始终 <= 0
                _deltaX = -_owner.Scroll.HorizontalOffset;
                _deltaY = -_owner.Scroll.VerticalOffset;
            }
            
            if (_owner.IsVirtualized)
                ArrangeVirRows(finalSize);
            else
                ArrangeRealRows(finalSize);

            // 宽度采用_maxSize.Width，若finalSize.Width造成iOS上死循环！
            double top = _deltaY > 0 ? 0 : -_deltaY;
            if (_filterBox != null)
                _filterBox.Arrange(new Rect(-_deltaX, top, _maxSize.Width, _filterBox.DesiredSize.Height));
            return finalSize;
        }
        #endregion

        #region 虚拟行
        Size MeasureVirRows()
        {
            Size testSize = new Size(_maxSize.Width, _rowHeight);
            foreach (var row in Children)
            {
                ((UIElement)row).Measure(testSize);
            }
            double height = _rowHeight * _owner.RootItems.GetExpandedCount();
            return new Size(_maxSize.Width, height);
        }

        void ArrangeVirRows(Size p_finalSize)
        {
            int rowCount = Children.Count;
            if (_filterBox != null)
                rowCount--;
            
            // 无数据时，也要重新布局
            if (_owner.RootItems.Count == 0
                || _rowHeight == 0
                || _deltaY >= _maxSize.Height       // 面板在滚动栏下方外部
                || _deltaY <= -p_finalSize.Height)  // 面板在滚动栏上方外部
            {
                // 避免把_filterBox隐藏
                for (int i = 0; i < rowCount; i++)
                {
                    ((UIElement)Children[i]).Arrange(Res.HideRect);
                }
                return;
            }

            IEnumerator<TvItem> tvItems;
            bool hasNext = true;

            // 面板可见，在滚动栏下方，按正常顺序布局
            if (_deltaY >= 0 && _deltaY < _maxSize.Height)
            {
                int iDataRow = rowCount;
                tvItems = _owner.RootItems.GetExpandedItems().GetEnumerator();
                for (int i = 0; i < rowCount; i++)
                {
                    var item = (TvPanelItem)Children[i];
                    double top = i * _rowHeight + _topMargin;
                    if (hasNext)
                        hasNext = tvItems.MoveNext();

                    // 数据行已结束 或 剩下行不可见，结束布局
                    if (_deltaY + top > _maxSize.Height || !hasNext)
                    {
                        iDataRow = i;
                        break;
                    }

                    // 布局虚拟行
                    item.SetItem(tvItems.Current, true);
                    item.Arrange(new Rect(0, top, p_finalSize.Width, _rowHeight));
                }
                tvItems.Dispose();

                // 将剩余的虚拟行布局到空区域
                if (iDataRow < rowCount)
                {
                    for (int i = iDataRow; i < rowCount; i++)
                    {
                        ((UIElement)Children[i]).Arrange(Res.HideRect);
                    }
                }
                return;
            }

            // deltaY < 0 && deltaY > -p_finalSize.Height
            // 面板顶部超出滚动栏 并且 没有整个面板都超出，此时deltaY为负数

            // 页面偏移量
            double offset = _deltaY % _pageHeight;
            // 最顶部的数据行索引
            int iRow = (int)Math.Floor(-_deltaY / _rowHeight);
            // 最顶部的虚拟行索引
            int iVirRow = iRow % rowCount;
            // 页面顶部偏移
            double deltaTop = -_deltaY + offset + _topMargin;
            // 跳过不显示的节点
            tvItems = _owner.RootItems.GetExpandedItems().Skip(iRow).GetEnumerator();

            for (int i = 0; i < rowCount; i++)
            {
                var item = (TvPanelItem)Children[iVirRow];
                if (hasNext)
                    hasNext = tvItems.MoveNext();
                if (hasNext)
                {
                    // 布局虚拟行
                    TvItem ti = tvItems.Current;
                    double top = iVirRow * _rowHeight + deltaTop;
                    item.SetItem(ti, true);
                    item.Arrange(new Rect(0, top, p_finalSize.Width, _rowHeight));
                }
                else
                {
                    // 多余的行布局在外部
                    item.Arrange(Res.HideRect);
                }

                iVirRow++;
                if (iVirRow >= rowCount)
                {
                    // 虚拟行放入下页
                    deltaTop += _pageHeight;
                    iVirRow = 0;
                }
            }
            tvItems.Dispose();
        }
        #endregion

        #region 真实行
        Size MeasureRealRows()
        {
            MeasureInfo info = new MeasureInfo();
            info.Size = new Size(_maxSize.Width, PanelMaxHeight);
            foreach (var item in _owner.RootItems)
            {
                var elem = (UIElement)Children[info.Index];
                elem.Measure(info.Size);
                info.TotalHeight += elem.DesiredSize.Height;
                info.Index++;
                MeasureChildRows(info, item);
            }
            return new Size(_maxSize.Width, info.TotalHeight);
        }

        void MeasureChildRows(MeasureInfo p_info, TvItem p_item)
        {
            if (p_item.Children.Count == 0)
                return;

            if (p_item.IsExpanded)
            {
                foreach (var item in p_item.Children)
                {
                    var elem = (UIElement)Children[p_info.Index];
                    elem.Measure(p_info.Size);
                    p_info.TotalHeight += elem.DesiredSize.Height;
                    p_info.Index++;
                    MeasureChildRows(p_info, item);
                }
            }
            else
            {
                foreach (var item in p_item.Children)
                {
                    var elem = (UIElement)Children[p_info.Index];
                    elem.Measure(_sizeEmpty);
                    p_info.Index++;
                    MeasureChildRows(p_info, item);
                }
            }
        }

        class MeasureInfo
        {
            public double TotalHeight;
            public int Index;
            public Size Size;
        }

        void ArrangeRealRows(Size p_finalSize)
        {
            ArrangeInfo info = new ArrangeInfo();
            info.FinalSize = p_finalSize;
            info.Top = _topMargin;
            foreach (var item in _owner.RootItems)
            {
                var elem = (UIElement)Children[info.Index];
                elem.Arrange(new Rect(0, info.Top, p_finalSize.Width, elem.DesiredSize.Height));
                info.Top += elem.DesiredSize.Height;
                info.Index++;
                ArrangeChildRows(info, item);
            }
        }

        void ArrangeChildRows(ArrangeInfo p_info, TvItem p_item)
        {
            if (p_item.Children.Count == 0)
                return;

            if (p_item.IsExpanded)
            {
                foreach (var item in p_item.Children)
                {
                    var elem = (UIElement)Children[p_info.Index];
                    elem.Arrange(new Rect(0, p_info.Top, p_info.FinalSize.Width, elem.DesiredSize.Height));
                    p_info.Top += elem.DesiredSize.Height;
                    p_info.Index++;
                    ArrangeChildRows(p_info, item);
                }
            }
            else
            {
                foreach (var item in p_item.Children)
                {
                    var elem = (UIElement)Children[p_info.Index];
                    elem.Arrange(Res.HideRect);
                    p_info.Index++;
                    ArrangeChildRows(p_info, item);
                }
            }
        }

        class ArrangeInfo
        {
            public double Top;
            public int Index;
            public Size FinalSize;
        }
        #endregion

        #region FilterBox
        TextBox _filterBox;
        DispatcherTimer _filterTimer;

        void LoadFilterBox()
        {
            if (_owner.FilterCfg == null)
                return;

            if (_filterBox == null)
            {
                _filterBox = _owner.FilterCfg.FilterBox;
                _filterBox.TextChanged += OnTextChanged;
                // android只支持KeyUp，只在enter时触发！
                _filterBox.KeyUp += OnTextKeyUp;
                Children.Add(_filterBox);
            }
        }

        void RemoveFilterBox()
        {
            // 非虚拟行或有分组时数据变化会清除所以元素
            if (_filterBox != null)
            {
                _filterBox.TextChanged -= OnTextChanged;
                _filterBox.KeyUp -= OnTextKeyUp;
                if (_filterTimer != null)
                {
                    _filterTimer.Stop();
                    _filterTimer.Tick -= OnTimerTick;
                    _filterTimer = null;
                }
                _filterBox = null;
            }
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_owner.FilterCfg.IsRealtime)
                StartTimer();
        }

        void StartTimer()
        {
            if (_filterTimer == null)
            {
                _filterTimer = new DispatcherTimer();
                _filterTimer.Interval = TimeSpan.FromMilliseconds(300);
                _filterTimer.Tick += OnTimerTick;
            }
            _filterTimer.Start();
        }

        void OnTimerTick(object sender, object e)
        {
            _filterTimer.Stop();
            _owner.ApplyFilterFlag();
        }

        void OnTextKeyUp(object sender, KeyRoutedEventArgs e)
        {
            // android只支持KeyUp，只在enter时触发！
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                _owner.ApplyFilterFlag();
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 加载虚拟模式的所有行
        /// </summary>
        void LoadVirRows()
        {
            ClearAllRows();

            if (_owner.RootItems.Count == 0 || _owner.View == null)
            {
                _initVirRow = false;
                return;
            }

            // 创建等高的虚拟行，时机：初次、切换行模板、面板大小变化
            // 先添加一行，作为行高标准
            var virRow = CreateVirRow(_owner.RootItems[0]);
            Children.Add(virRow);

            // 测量行高
            // 给最大高度才能测量出内容的实际高度
            Size testSize = new Size(_maxSize.Width, PanelMaxHeight);
            virRow.Measure(testSize);
            _rowHeight = virRow.DesiredSize.Height;

            if (_rowHeight > 0)
            {
                // 确保子元素刚好摆满可见区域，计算所需行数
                int rowCount = (int)Math.Ceiling(_maxSize.Height / _rowHeight) + 1;
                _pageHeight = rowCount * _rowHeight;

                // 补充子元素
                for (int i = 1; i < rowCount; i++)
                {
                    virRow = CreateVirRow(null);
                    Children.Add(virRow);
                }
            }
            _initVirRow = true;

            LoadFilterBox();
        }

        /// <summary>
        /// 加载真实模式的所有行
        /// </summary>
        void LoaRealRows()
        {
            ClearAllRows();
            if (_owner.RootItems.Count > 0)
            {
                foreach (var item in _owner.RootItems)
                {
                    Children.Add(new TvPanelItem(_owner, item));
                    AddChildRows(item);
                }
                LoadFilterBox();
            }
        }

        void AddChildRows(TvItem p_parent)
        {
            if (p_parent.Children.Count > 0)
            {
                foreach (var item in p_parent.Children)
                {
                    Children.Add(new TvPanelItem(_owner, item));
                    AddChildRows(item);
                }
            }
        }

        /// <summary>
        /// 清除所有行
        /// </summary>
        void ClearAllRows()
        {
            Children.Clear();
            RemoveFilterBox();
            _initVirRow = false;
        }

        /// <summary>
        /// 生成行内容
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        TvPanelItem CreateVirRow(TvItem p_item)
        {
            var row = new TvPanelItem(_owner);
            if (p_item != null)
                row.SetItem(p_item, false);
            return row;
        }
        #endregion
    }
}
