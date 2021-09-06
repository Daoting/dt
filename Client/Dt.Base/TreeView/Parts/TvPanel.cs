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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        static Rect _rcEmpty = new Rect();
        static Size _sizeEmpty = new Size();

        TreeView _owner;
        bool _initVirRow;
        double _rowHeight;
        double _pageHeight;

        /// <summary>
        /// 面板最大尺寸，宽高始终不为无穷大！
        /// </summary>
        Size _maxSize = Size.Empty;
        #endregion

        public TvPanel(TreeView p_owner)
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
            if (_maxSize.Width != p_size.Width || _maxSize.Height != p_size.Height)
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
            if (Children.Count == 0)
                return new Size();

            return _owner.IsVirtualized ? MeasureVirRows() : MeasureRealRows();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            if (_owner.IsVirtualized)
                ArrangeVirRows(finalSize);
            else
                ArrangeRealRows(finalSize);
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
            double deltaY = 0;
            if (!_owner.IsInnerScroll)
            {
                // 面板与ScrollViewer的相对距离，以滚动栏为参照物，面板在右下方时为正数
#if UWP
                if (_owner.Scroll.ActualHeight > 0)
                {
                    // 当切换win时，再次显示Scroll时ActualHeight为0，计算相对位置错误！采用切换前的相对位置
                    var pt = TransformToVisual(_owner.Scroll).TransformPoint(new Point());
                    deltaY = pt.Y;
                }
#else
                // uno中面板与Scroll的相对距离始终为滚动栏未移动时之间的距离！
                var pt = TransformToVisual(_owner.Scroll).TransformPoint(new Point());
                deltaY = pt.Y - _owner.Scroll.VerticalOffset;
#endif
            }
            else
            {
                // 内置滚动栏时，垂直距离始终 <= 0
                deltaY = -_owner.Scroll.VerticalOffset;
            }

            // 无数据时，也要重新布局
            if (_owner.RootItems.Count == 0
                || _rowHeight == 0
                || deltaY >= _maxSize.Height       // 面板在滚动栏下方外部
                || deltaY <= -p_finalSize.Height)  // 面板在滚动栏上方外部
            {
                foreach (var elem in Children)
                {
                    ((UIElement)elem).Arrange(_rcEmpty);
                }
                return;
            }

            IEnumerator<TvItem> tvItems;
            bool hasNext = true;

            // 面板可见，在滚动栏下方，按正常顺序布局
            if (deltaY >= 0 && deltaY < _maxSize.Height)
            {
                int iDataRow = Children.Count;
                tvItems = _owner.RootItems.GetExpandedItems().GetEnumerator();
                for (int i = 0; i < Children.Count; i++)
                {
                    var item = (TvPanelItem)Children[i];
                    double top = i * _rowHeight;
                    if (hasNext)
                        hasNext = tvItems.MoveNext();

                    // 数据行已结束 或 剩下行不可见，结束布局
                    if (deltaY + top > _maxSize.Height || !hasNext)
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
                if (iDataRow < Children.Count)
                {
                    for (int i = iDataRow; i < Children.Count; i++)
                    {
                        ((UIElement)Children[i]).Arrange(_rcEmpty);
                    }
                }
                return;
            }

            // deltaY < 0 && deltaY > -p_finalSize.Height
            // 面板顶部超出滚动栏 并且 没有整个面板都超出，此时deltaY为负数

            // 页面偏移量
            double offset = deltaY % _pageHeight;
            // 最顶部的数据行索引
            int iRow = (int)Math.Floor(-deltaY / _rowHeight);
            // 最顶部的虚拟行索引
            int iVirRow = iRow % Children.Count;
            // 页面顶部偏移
            double deltaTop = -deltaY + offset;
            // 跳过不显示的节点
            tvItems = _owner.RootItems.GetExpandedItems().Skip(iRow).GetEnumerator();

            for (int i = 0; i < Children.Count; i++)
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
                    item.Arrange(_rcEmpty);
                }

                iVirRow++;
                if (iVirRow >= Children.Count)
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
                    elem.Arrange(_rcEmpty);
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
