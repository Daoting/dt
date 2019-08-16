#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.TreeView
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

        Tv _owner;
        bool _initVirRow;
        double _rowHeight;
        double _pageHeight;
        #endregion

        /// <summary>
        /// 通过Tv获取有效高度，因在ScrollViewer内！
        /// </summary>
        internal Size AvailableSize { get; set; }

        internal void SetOwner(Tv p_owner)
        {
            _owner = p_owner;
            if (_owner.IsVirtualized)
                InvalidateMeasure();
            else
                LoaAllRows();

            // 处理大小变化，uno上Loaded后触发！
            if (AtSys.System == TargetSystem.Windows)
                _owner.SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// 切换模板、调整是否采用虚拟化时重新加载
        /// </summary>
        internal void ReloadAllRows()
        {
            ClearAllRows();
            if (!_owner.IsVirtualized)
                LoaAllRows();
        }

        /// <summary>
        /// 切换数据源时
        /// </summary>
        internal void OnRowsChanged()
        {
            if (_owner.IsVirtualized)
            {
                InvalidateMeasure();
            }
            else
            {
                ClearAllRows();
                LoaAllRows();
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
            if (_owner.Scroll.ScrollableHeight == 0)
                return;

            double scrollBottom = _owner.Scroll.VerticalOffset + _owner.Scroll.ViewportHeight;
            if (_owner.IsVirtualized)
            {
                double top = _owner.RootItems.GetVerIndex(p_item) * _rowHeight;
                if (top < _owner.Scroll.VerticalOffset)
                    _owner.Scroll.ChangeView(null, top, null);
                else if (top + _rowHeight > scrollBottom)
                    _owner.Scroll.ChangeView(null, _owner.Scroll.VerticalOffset + (top + _rowHeight - scrollBottom), null);
            }
            else
            {
                Rect rc = _owner.RootItems.GetExpandedPostion(p_item, this);
                if (rc.Top < _owner.Scroll.VerticalOffset)
                    _owner.Scroll.ChangeView(null, rc.Top, null);
                else if (rc.Bottom > scrollBottom)
                    _owner.Scroll.ChangeView(null, _owner.Scroll.VerticalOffset + (rc.Bottom - scrollBottom), null);
            }
        }

        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner.RootItems.Count == 0 || _owner.View == null)
                return new Size();

            double maxWidth = double.IsInfinity(AvailableSize.Width) ? AtUI.ViewWidth : AvailableSize.Width;
            double maxHeight = double.IsInfinity(AvailableSize.Height) ? AtUI.ViewHeight : AvailableSize.Height;
            return _owner.IsVirtualized ? MeasureVirRows(maxWidth, maxHeight) : MeasureRealRows(maxWidth, maxHeight);
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
        Size MeasureVirRows(double p_maxWidth, double p_maxHeight)
        {
            if (!_initVirRow)
            {
                // 创建等高的虚拟行，时机：初次、切换行模板、面板大小变化
                // 先添加一行，作为行高标准
                var virRow = CreateVirRow(_owner.RootItems[0]);
                Children.Add(virRow);

                // 测量行高
                // 给最大高度才能测量出内容的实际高度
                Size testSize = new Size(p_maxWidth, PanelMaxHeight);
                virRow.Measure(testSize);
                _rowHeight = virRow.DesiredSize.Height;

                if (_rowHeight > 0)
                {
                    // 确保子元素刚好摆满可见区域，计算所需行数
                    int rowCount = (int)Math.Ceiling(p_maxHeight / _rowHeight) + 1;
                    _pageHeight = rowCount * _rowHeight;

                    // 补充子元素
                    testSize = new Size(p_maxWidth, _rowHeight);
                    for (int i = 1; i < rowCount; i++)
                    {
                        virRow = CreateVirRow(null);
                        Children.Add(virRow);
                        virRow.Measure(testSize);
                    }
                }
                _initVirRow = true;
            }
            else
            {
                // 重新测量
                Size testSize = new Size(p_maxWidth, _rowHeight);
                foreach (var row in Children)
                {
                    ((UIElement)row).Measure(testSize);
                }
            }
            double height = _rowHeight * _owner.RootItems.GetExpandedCount();
            return new Size(p_maxWidth, height);
        }

        void ArrangeVirRows(Size p_finalSize)
        {
            // 无数据时，也要重新布局
            if (_owner.RootItems.Count == 0 || _rowHeight == 0)
            {
                foreach (var elem in Children)
                {
                    ((UIElement)elem).Arrange(_rcEmpty);
                }
                return;
            }

            double scrollY = _owner.Scroll.VerticalOffset;
            // 页面偏移量
            double offset = -scrollY % _pageHeight;
            // 最顶部的数据行索引
            int iRow = (int)Math.Floor(scrollY / _rowHeight);
            // 最顶部的虚拟行索引
            int iVirRow = iRow % Children.Count;
            // 页面顶部偏移
            double deltaTop = scrollY + offset;
            // 跳过不显示的节点
            var tvItems = _owner.RootItems.GetExpandedItems().Skip(iRow).GetEnumerator();
            bool hasNext = true;

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
        Size MeasureRealRows(double p_maxWidth, double p_maxHeight)
        {
            MeasureInfo info = new MeasureInfo();
            info.Size = new Size(p_maxWidth, PanelMaxHeight);
            foreach (var item in _owner.RootItems)
            {
                var elem = (UIElement)Children[info.Index];
                elem.Measure(info.Size);
                info.TotalHeight += elem.DesiredSize.Height;
                info.Index++;
                MeasureChildRows(info, item);
            }
            return new Size(p_maxWidth, info.TotalHeight);
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
        /// 面板大小变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 过滤从可视树卸载后再加载的情况，造成切换任务栏时非常慢！！！
            if (e.PreviousSize.Width > 0 && e.PreviousSize.Height > 0)
            {
                if (_owner.IsVirtualized)
                    ClearAllRows();
                else
                    InvalidateArrange();
            }
        }

        /// <summary>
        /// 非虚拟化时生成所有行
        /// </summary>
        void LoaAllRows()
        {
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
        /// <param name="p_vr"></param>
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
