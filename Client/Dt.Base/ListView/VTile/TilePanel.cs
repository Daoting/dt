#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 磁贴面板
    /// </summary>
    public partial class TilePanel : LvPanel
    {
        // 列数
        int _colCount;
        // 项目宽度
        double _itemWidth;
        // 所有虚拟行总数
        int _pageItemCount;

        public TilePanel(Lv p_owner) : base(p_owner)
        { }

        protected override void LoadOtherRows()
        {
            // 只为获得输入焦点！
            var con = new ContentControl();
            Children.Add(con);
            if (_owner.AutoFocus)
                con.Focus(FocusState.Programmatic);
        }

        #region 虚拟行
        protected override Size MeasureVirRows()
        {
            // 数据行
            if (!_initVirRow && _owner.Rows.Count > 0)
            {
                // 确定列数和列宽
                if (_maxSize.Width < _owner.MinItemWidth * 2)
                {
                    // 有效宽度无法放两列
                    _itemWidth = _maxSize.Width;
                    _colCount = 1;
                }
                else
                {
                    // >= 2列，确保始终铺满，列宽: <= 3/2 * MinItemWidth
                    _colCount = (int)Math.Floor(_maxSize.Width / _owner.MinItemWidth);
                    double leave = _maxSize.Width % _owner.MinItemWidth;
                    _itemWidth = _owner.MinItemWidth + leave / _colCount;
                }

                // 创建等高的虚拟行，时机：初次、切换行模板、面板大小变化
                // 先添加一行，作为行高标准
                FrameworkElement virRow = _createLvRow(_owner.Rows[0]);
                Children.Insert(0, virRow);
                _dataRows.Add(virRow);

                // 测量行高
                // 给最大高度才能测量出内容的实际高度
                Size testSize = new Size(_itemWidth, PanelMaxHeight);
                virRow.Measure(testSize);
                _rowHeight = virRow.DesiredSize.Height;

                if (_rowHeight > 0)
                {
                    // 确保子元素刚好摆满可见区域，计算所需行数
                    int rowCount = (int)Math.Ceiling(_maxSize.Height / _rowHeight) + 1;
                    _pageHeight = rowCount * _rowHeight;
                    _pageItemCount = rowCount * _colCount;

                    // 补充子元素
                    testSize = new Size(_itemWidth, _rowHeight);
                    for (int i = 1; i < _pageItemCount; i++)
                    {
                        virRow = _createLvRow(null);
                        Children.Insert(i, virRow);
                        _dataRows.Add(virRow);
                        virRow.Measure(testSize);
                    }
                }
                _initVirRow = true;
            }
            else if (_dataRows.Count > 0)
            {
                // 重新测量
                Size testSize = new Size(_itemWidth, _rowHeight);
                // 只在创建虚拟行时确定宽度，重新测量时不重置，避免不一样时死循环！
                foreach (var row in _dataRows)
                {
                    row.Measure(testSize);
                }
            }

            double height = 0;
            if (_owner.GroupRows != null)
            {
                // 有分组行
                foreach (var grp in _owner.GroupRows)
                {
                    grp.Top = height;
                    grp.Measure(_maxSize);
                    height += grp.DesiredSize.Height;
                    height += Math.Ceiling((double)grp.Data.Count / _colCount) * _rowHeight;
                }
            }
            else
            {
                // 只数据行
                height = Math.Ceiling((double)_owner.Rows.Count / _colCount) * _rowHeight;
            }

            // 分组导航头，出现垂直滚动栏时才显示
            if (_groupHeader != null && height > _maxSize.Height)
            {
                _groupHeader.Measure(_maxSize);
                height += _groupHeader.DesiredSize.Height;
                // 增加高度使最底部分组能够滚动到顶部，确保和导航位置同步！
                var group = _owner.GroupRows[_owner.GroupRows.Count - 1];
                double delta = _maxSize.Height - _groupHeader.DesiredSize.Height - group.DesiredSize.Height - Math.Ceiling((double)group.Data.Count / _colCount) * _rowHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_maxSize.Width, height);
        }

        protected override void ArrangeVirRows(Size p_finalSize)
        {
            // 无数据时，也要重新布局
            if (_owner.Rows.Count == 0 || _rowHeight == 0)
            {
                foreach (var elem in _dataRows)
                {
                    elem.Arrange(_rcEmpty);
                }
                return;
            }

            double scrollY = _owner.Scroll.VerticalOffset;
            // 页面偏移量
            double offset = -scrollY % _pageHeight;
            // 最顶部的数据行索引
            int iRow = (int)Math.Floor(scrollY / _rowHeight) * _colCount;
            // 最顶部的虚拟行索引
            int iVirRow = iRow % _dataRows.Count;
            // 页面顶部偏移
            double deltaTop = scrollY + offset;

            for (int i = 0; i < _dataRows.Count; i++)
            {
                var item = _dataRows[iVirRow];
                if (iRow + i < _owner.Rows.Count)
                {
                    // 布局虚拟行
                    double top = Math.Floor((double)iVirRow / _colCount) * _rowHeight + deltaTop;
                    // 行末尾项宽度加1为隐藏右边框
                    bool isRowLast = ((iVirRow + 1) % _colCount == 0);
                    item.Arrange(new Rect((iVirRow % _colCount) * _itemWidth, top, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
                    ((LvRow)item).SetViewRow(_owner.Rows[iRow + i], true);
                }
                else
                {
                    // 多余的行布局在外部
                    item.Arrange(_rcEmpty);
                }

                iVirRow++;
                if (iVirRow >= _dataRows.Count)
                {
                    // 虚拟行放入下页
                    deltaTop += _pageHeight;
                    iVirRow = 0;
                }
            }
        }

        protected override void ArrangeGroupVirRows(Size p_finalSize)
        {
            int iAllRow = 0, iGrpRow = 0, iDataRow = 0;
            GroupRow lastGroup = null;
            double totalHeight = 0;
            double scrollY = _owner.Scroll.VerticalOffset;
            // 分组内的项目索引
            int indexInGroup = 0;

            // 有垂直滚动时，计算以上变量值
            if (scrollY > 0)
            {
                for (int i = 0; i < _owner.MapRows.Count; i++)
                {
                    if (_owner.MapRows[i])
                    {
                        // 分组行
                        // 布局上个分组
                        if (lastGroup != null)
                            lastGroup.Arrange(_rcEmpty);

                        lastGroup = _owner.GroupRows[iGrpRow];
                        double height = lastGroup.DesiredSize.Height;
                        if (totalHeight + height > scrollY)
                        {
                            // 进入可见区
                            iAllRow = i;
                            break;
                        }

                        iGrpRow++;
                        totalHeight += height;
                        indexInGroup = 0;
                    }
                    else
                    {
                        // 数据行
                        if (indexInGroup % _colCount == 0)
                        {
                            // 换行
                            if (totalHeight + _rowHeight > scrollY)
                            {
                                // 进入可见区
                                iAllRow = i;
                                break;
                            }
                            totalHeight += _rowHeight;
                        }
                        iDataRow++;
                        indexInGroup++;
                    }
                }
                // 可见数据行在所有虚拟行的索引
                iDataRow = iDataRow % _pageItemCount;
            }

            // 分组导航头
            if (_groupHeader != null && _owner.Scroll.ScrollableHeight > 0)
            {
                _groupHeader.SetCurrentGroup(lastGroup?? _owner.GroupRows[0]);
                _groupHeader.Arrange(new Rect(0, scrollY, p_finalSize.Width, _groupHeader.DesiredSize.Height));
                totalHeight += _groupHeader.DesiredSize.Height;
            }

            // 布局可视行
            int iStart = iDataRow;
            LvRow lastRow = null;
            for (int i = iAllRow; i < _owner.MapRows.Count; i++)
            {
                if (_owner.MapRows[i])
                {
                    // 布局分组
                    if (lastRow != null)
                    {
                        // 上个分组最后未摆满一行
                        totalHeight += _rowHeight;
                        lastRow = null;
                    }

                    var gr = _owner.GroupRows[iGrpRow];
                    gr.Arrange(new Rect(0, totalHeight, p_finalSize.Width, gr.DesiredSize.Height));
                    totalHeight += gr.DesiredSize.Height;
                    iGrpRow++;
                    indexInGroup = 0;
                }
                else
                {
                    var row = (LvRow)_dataRows[iDataRow];
                    // 行末尾项宽度加1为隐藏右边框
                    bool isRowLast = ((indexInGroup + 1) % _colCount == 0);
                    row.Arrange(new Rect((indexInGroup % _colCount) * _itemWidth, totalHeight, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
                    row.SetViewRow(_owner.Rows[i - iGrpRow], true);

                    if (isRowLast)
                    {
                        // 摆满一行
                        totalHeight += _rowHeight;
                        lastRow = null;
                    }
                    else
                    {
                        // 记录最后一项
                        lastRow = row;
                    }

                    iDataRow++;
                    indexInGroup++;
                    if (iDataRow >= _dataRows.Count)
                        iDataRow = 0;
                    if (iDataRow == iStart)
                        break;
                }
            }

            // 布局剩余的分组，不可见
            for (int i = iGrpRow; i < _owner.GroupRows.Count; i++)
            {
                _owner.GroupRows[i].Arrange(_rcEmpty);
            }
        }
        #endregion

        #region 真实行
        protected override Size MeasureRealRows()
        {
            // 确定列数和列宽
            if (_maxSize.Width < _owner.MinItemWidth * 2)
            {
                // 有效宽度无法放两列
                _itemWidth = _maxSize.Width;
                _colCount = 1;
            }
            else
            {
                // >= 2列，确保始终铺满，列宽: <= 3/2 * MinItemWidth
                _colCount = (int)Math.Floor(_maxSize.Width / _owner.MinItemWidth);
                double leave = _maxSize.Width % _owner.MinItemWidth;
                _itemWidth = _owner.MinItemWidth + leave / _colCount;
            }

            // 确定行高，统一取最大行高
            _rowHeight = 0;
            Size rowSize = new Size(_itemWidth, PanelMaxHeight);
            foreach (var row in _dataRows)
            {
                row.Measure(rowSize);
                if (row.DesiredSize.Height > _rowHeight)
                    _rowHeight = row.DesiredSize.Height;
            }

            double height = 0;
            Size grpSize = new Size(_maxSize.Width, PanelMaxHeight);
            if (_owner.GroupRows != null)
            {
                // 有分组行
                foreach (var grp in _owner.GroupRows)
                {
                    grp.Top = height;
                    grp.Measure(grpSize);
                    height += grp.DesiredSize.Height;
                    height += Math.Ceiling((double)grp.Data.Count / _colCount) * _rowHeight;
                }
            }
            else
            {
                // 只数据行
                height = Math.Ceiling((double)_dataRows.Count / _colCount) * _rowHeight;
            }

            // 分组导航头，出现垂直滚动栏时才显示
            if (_groupHeader != null && height > _maxSize.Height)
            {
                _groupHeader.Measure(grpSize);
                height += _groupHeader.DesiredSize.Height;
                // 增加高度使最底部分组能够滚动到顶部，确保和导航位置同步！
                var group = _owner.GroupRows[_owner.GroupRows.Count - 1];
                double delta = _maxSize.Height - _groupHeader.DesiredSize.Height - group.DesiredSize.Height - Math.Ceiling((double)group.Data.Count / _colCount) * _rowHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_maxSize.Width, height);
        }

        protected override void ArrangeRealRows(Size p_finalSize)
        {
            int index = 0;
            double totalHeight = 0;
            double scrollY = _owner.Scroll.VerticalOffset;
            double bottomY = GetViewBottom();

            // 布局滚动后不可见行
            if (scrollY > 0)
            {
                for (int i = 0; i < _dataRows.Count; i++)
                {
                    if (i % _colCount == 0)
                    {
                        if (totalHeight + _rowHeight > scrollY)
                        {
                            // 进入可见
                            index = i;
                            break;
                        }
                        totalHeight += _rowHeight;
                    }
                    // 不可见
                    _dataRows[i].Arrange(_rcEmpty);
                }
            }

            // 布局可视行
            for (int i = index; i < _dataRows.Count; i++)
            {
                // 行末尾项宽度加1为隐藏右边框
                bool isRowLast = ((i + 1) % _colCount == 0);
                _dataRows[i].Arrange(new Rect((i % _colCount) * _itemWidth, totalHeight, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
                if (isRowLast)
                    totalHeight += _rowHeight;
                index = i + 1;
                if (totalHeight >= bottomY)
                    break;
            }

            // 底部不可见行
            for (int i = index; i < _dataRows.Count; i++)
            {
                _dataRows[i].Arrange(_rcEmpty);
            }
        }

        protected override void ArrangeGroupRealRows(Size p_finalSize)
        {
            int iAllRow = 0, iGrpRow = 0, iDataRow = 0;
            GroupRow lastGroup = null;
            double totalHeight = 0;
            double scrollY = _owner.Scroll.VerticalOffset;
            double bottomY = GetViewBottom();
            // 分组内的项目索引
            int indexInGroup = 0;

            // 有垂直滚动时，计算以上变量值
            if (scrollY > 0)
            {
                for (int i = 0; i < _owner.MapRows.Count; i++)
                {
                    if (_owner.MapRows[i])
                    {
                        // 分组行
                        // 布局上个分组
                        if (lastGroup != null)
                            lastGroup.Arrange(_rcEmpty);

                        lastGroup = _owner.GroupRows[iGrpRow];
                        double height = lastGroup.DesiredSize.Height;
                        if (totalHeight + height > scrollY)
                        {
                            // 进入可见区
                            iAllRow = i;
                            break;
                        }

                        iGrpRow++;
                        totalHeight += height;
                        indexInGroup = 0;
                    }
                    else
                    {
                        // 数据行
                        if (indexInGroup % _colCount == 0)
                        {
                            // 换行
                            if (totalHeight + _rowHeight > scrollY)
                            {
                                // 进入可见区
                                iAllRow = i;
                                break;
                            }
                            totalHeight += _rowHeight;
                        }

                        _dataRows[iDataRow++].Arrange(_rcEmpty);
                        indexInGroup++;
                    }
                }
            }

            // 分组导航头
            if (_groupHeader != null && _owner.Scroll.ScrollableHeight > 0)
            {
                _groupHeader.SetCurrentGroup(lastGroup ?? _owner.GroupRows[0]);
                _groupHeader.Arrange(new Rect(0, scrollY, p_finalSize.Width, _groupHeader.DesiredSize.Height));
                totalHeight += _groupHeader.DesiredSize.Height;
            }

            // 布局可视行
            LvRow lastRow = null;
            for (int i = iAllRow; i < _owner.MapRows.Count; i++)
            {
                if (_owner.MapRows[i])
                {
                    // 布局分组
                    if (lastRow != null)
                    {
                        // 上个分组最后未摆满一行
                        totalHeight += _rowHeight;
                        lastRow = null;
                    }

                    var gr = _owner.GroupRows[iGrpRow];
                    gr.Arrange(new Rect(0, totalHeight, p_finalSize.Width, gr.DesiredSize.Height));
                    totalHeight += gr.DesiredSize.Height;
                    iGrpRow++;
                    indexInGroup = 0;
                }
                else
                {
                    var row = (LvRow)_dataRows[iDataRow];
                    // 行末尾项宽度加1为隐藏右边框
                    bool isRowLast = ((indexInGroup + 1) % _colCount == 0);
                    row.Arrange(new Rect((indexInGroup % _colCount) * _itemWidth, totalHeight, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));

                    if (isRowLast)
                    {
                        // 摆满一行
                        totalHeight += _rowHeight;
                        lastRow = null;
                    }
                    else
                    {
                        // 记录最后一项
                        lastRow = row;
                    }
                    iDataRow++;
                    indexInGroup++;
                }

                iAllRow = i + 1;
                if (totalHeight >= bottomY)
                    break;
            }

            // 底部不可见行
            for (int i = iAllRow; i < _owner.MapRows.Count; i++)
            {
                if (_owner.MapRows[i])
                    _owner.GroupRows[iGrpRow++].Arrange(_rcEmpty);
                else
                    _dataRows[iDataRow++].Arrange(_rcEmpty);
            }
        }
        #endregion
    }
}