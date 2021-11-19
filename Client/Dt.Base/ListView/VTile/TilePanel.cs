#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
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

        #region 重写方法
        /// <summary>
        /// 获取数据行的垂直位置
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        internal override double GetRowVerPos(int p_index)
        {
            double height = 0;
            if (_owner.IsVir)
            {
                // 虚拟行，等高
                height = Math.Floor((double)p_index / _colCount) * _rowHeight;
                if (_owner.MapRows != null)
                {
                    // 等高有分组
                    int cnt = 0, iGrpRow = 0;
                    for (int i = 0; i < _owner.MapRows.Count; i++)
                    {
                        if (_owner.MapRows[i])
                            height += _owner.GroupRows[iGrpRow++].DesiredSize.Height;
                        else
                            cnt++;

                        if (cnt > p_index)
                            break;
                    }
                }
            }
            else
            {
                // 真实行
                if (_owner.MapRows == null)
                {
                    // 无分组
                    for (int i = 0; i < p_index; i++)
                    {
                        // 行末尾项
                        bool isRowLast = ((i + 1) % _colCount == 0);
                        if (isRowLast)
                            height += _rowHeight;
                    }
                }
                else
                {
                    // 有分组
                    int iDataRow = 0, iGrpRow = 0;
                    // 分组内的项目索引
                    int indexInGroup = 0;
                    LvRow lastRow = null;
                    for (int i = 0; i < _owner.MapRows.Count; i++)
                    {
                        if (_owner.MapRows[i])
                        {
                            if (lastRow != null)
                            {
                                // 上个分组最后未摆满一行
                                height += _rowHeight;
                                lastRow = null;
                            }
                            height += _owner.GroupRows[iGrpRow++].DesiredSize.Height;
                        }
                        else
                        {
                            bool isRowLast = ((indexInGroup + 1) % _colCount == 0);
                            if (isRowLast)
                            {
                                // 摆满一行
                                height += _rowHeight;
                                lastRow = null;
                            }
                            else
                            {
                                // 记录最后一项
                                lastRow = _dataRows[iDataRow];
                            }
                            iDataRow++;
                            indexInGroup++;
                        }

                        if (iDataRow >= p_index)
                            break;
                    }
                }
            }

            if (_groupHeader != null)
                height -= _groupHeader.DesiredSize.Height;
            return Math.Max(height, 0);
        }
        #endregion

        #region 虚拟行
        protected override bool CreateVirRows()
        {
            if (_owner.Rows.Count == 0)
                return false;

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
            var virRow = _createLvRow(_owner.Rows[0]);
            Children.Add(virRow);
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
                for (int i = 1; i < _pageItemCount; i++)
                {
                    virRow = _createLvRow(null);
                    Children.Add(virRow);
                    _dataRows.Add(virRow);
                }
            }
            return true;
        }

        protected override Size MeasureVirRows()
        {
            // 数据行
            if (_dataRows.Count == 0)
                return new Size(_maxSize.Width, 0);

            // 重新测量
            Size testSize = new Size(_itemWidth, _rowHeight);
            // 只在创建虚拟行时确定宽度，重新测量时不重置，避免不一样时死循环！
            foreach (var row in _dataRows)
            {
                row.Measure(testSize);
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
                if (_owner.GroupRows.Count > 1)
                    delta += GroupSeparatorHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_maxSize.Width, height);
        }

        protected override void ArrangeVirRows(Size p_finalSize)
        {
            // 无数据 或 整个面板不在滚动栏可视区时，也要重新布局
            if (_owner.Rows.Count == 0
                || _rowHeight == 0
                || _deltaY >= _maxSize.Height       // 面板在滚动栏下方外部
                || _deltaY <= -p_finalSize.Height)  // 面板在滚动栏上方外部
            {
                foreach (var elem in _dataRows)
                {
                    elem.Arrange(_rcEmpty);
                }
                return;
            }

            // 面板可见，在滚动栏下方，按正常顺序布局
            if (_deltaY >= 0 && _deltaY < _maxSize.Height)
            {
                int iDataRow = _dataRows.Count;
                for (int i = 0; i < _dataRows.Count; i++)
                {
                    var item = _dataRows[i];
                    double top = Math.Floor((double)i / _colCount) * _rowHeight + _toolbarHeight;

                    // 数据行已结束 或 剩下行不可见，结束布局
                    if (i >= _owner.Rows.Count || _deltaY + top > _maxSize.Height)
                    {
                        iDataRow = i;
                        break;
                    }

                    // 行末尾项宽度加1为隐藏右边框
                    bool isRowLast = (i + 1) % _colCount == 0;
                    double left = i % _colCount * _itemWidth;
                    item.Arrange(new Rect(left, top, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
                    item.SetViewRow(_owner.Rows[i], true);
                }

                // 将剩余的虚拟行布局到空区域
                if (iDataRow < _dataRows.Count)
                {
                    for (int i = iDataRow; i < _dataRows.Count; i++)
                    {
                        _dataRows[i].Arrange(_rcEmpty);
                    }
                }
                return;
            }

            // 页面偏移量
            double offset = _deltaY % _pageHeight;
            // 最顶部的数据行索引
            int iRow = (int)Math.Floor(-_deltaY / _rowHeight) * _colCount;
            // 最顶部的虚拟行索引
            int iVirRow = iRow % _dataRows.Count;
            // 页面顶部偏移
            double deltaTop = -_deltaY + offset + _toolbarHeight;

            for (int i = 0; i < _dataRows.Count; i++)
            {
                var item = _dataRows[iVirRow];
                if (iRow + i < _owner.Rows.Count)
                {
                    // 布局虚拟行
                    double top = Math.Floor((double)iVirRow / _colCount) * _rowHeight + deltaTop;
                    double left = iVirRow % _colCount * _itemWidth;
                    // 行末尾项宽度加1为隐藏右边框
                    bool isRowLast = (iVirRow + 1) % _colCount == 0;
                    item.Arrange(new Rect(left, top, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
                    item.SetViewRow(_owner.Rows[iRow + i], true);
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
            // 整个面板不在滚动栏可视区时，布局到空区域
            if (_deltaY >= _maxSize.Height          // 面板在滚动栏下方外部
                || _deltaY <= -p_finalSize.Height)  // 面板在滚动栏上方外部
            {
                // 虚拟数据行
                foreach (var elem in _dataRows)
                {
                    elem.Arrange(_rcEmpty);
                }

                // 分组行
                foreach (var grp in _owner.GroupRows)
                {
                    grp.Arrange(_rcEmpty);
                }

                // 分组导航头
                _groupHeader?.Arrange(_rcEmpty);
                return;
            }

            int iGrpRow = 0, iDataRow = 0;
            double totalHeight = _toolbarHeight;
            // 分组内的项目索引
            int indexInGroup = 0;
            LvRow lastRow = null;

            //----------------------------------------------
            // 面板可见，在滚动栏下方，按顺序布局所有行
            //----------------------------------------------
            if (_deltaY >= 0 && _deltaY < _maxSize.Height)
            {
                // 分组导航头
                if (_groupHeader != null)
                {
                    if (!_isScrolling && _deltaY < _groupHeader.DesiredSize.Height)
                    {
                        // 离滚动栏顶部距离小于分组导航头高度时显示
                        // 分组导航头高度可能为0！
                        _groupHeader.SetCurrentGroup(_owner.GroupRows[0]);
                        // -_deltaY确保始终在滚动栏顶部位置
                        _groupHeader.Arrange(new Rect(0, -_deltaY + _toolbarHeight, p_finalSize.Width, _groupHeader.DesiredSize.Height));
                    }
                    else
                    {
                        // 离滚动栏顶部太远时不显示分组导航头
                        _groupHeader.Arrange(_rcEmpty);
                    }
                }

                for (int i = 0; i < _owner.MapRows.Count; i++)
                {
                    if (_owner.MapRows[i])
                    {
                        // 布局分组行
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
                        // 布局数据行
                        var row = _dataRows[iDataRow];
                        // 行末尾项宽度加1为隐藏右边框
                        bool isRowLast = (indexInGroup + 1) % _colCount == 0;
                        row.Arrange(new Rect(indexInGroup % _colCount * _itemWidth, totalHeight, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
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

                        // 虚拟行都已布局时退出
                        if (iDataRow >= _dataRows.Count)
                            break;
                    }

                    // 剩下行不可见，结束布局
                    if (_deltaY + totalHeight > _maxSize.Height)
                        break;
                }

                // 将剩余的虚拟行和分组行布局到空区域
                if (iDataRow < _dataRows.Count)
                {
                    for (int i = iDataRow; i < _dataRows.Count; i++)
                    {
                        _dataRows[i].Arrange(_rcEmpty);
                    }
                }
                if (iGrpRow < _owner.GroupRows.Count)
                {
                    for (int i = iGrpRow; i < _owner.GroupRows.Count; i++)
                    {
                        _owner.GroupRows[i].Arrange(_rcEmpty);
                    }
                }
                return;
            }

            //----------------------------------------------
            // _deltaY < 0 && _deltaY > -p_finalSize.Height
            // 面板顶部超出滚动栏 并且 没有整个面板都超出，此时_deltaY为负数
            //----------------------------------------------

            // 避免所有行完全超出滚动栏顶部的情况
            int iAllRow = _owner.MapRows.Count;
            GroupRow lastGroup = null;

            // 布局顶部超出的分组、计算总高、数据行索引
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
                    if (totalHeight + height > -_deltaY)
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
                        if (totalHeight + _rowHeight > -_deltaY)
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

            // 分组导航头
            if (_groupHeader != null)
            {
                if (!_isScrolling && iAllRow < _owner.MapRows.Count)
                {
                    _groupHeader.SetCurrentGroup(lastGroup);
                    // -_deltaY确保始终在滚动栏顶部位置
                    _groupHeader.Arrange(new Rect(0, -_deltaY + _toolbarHeight, p_finalSize.Width, _groupHeader.DesiredSize.Height));
                }
                else
                {
                    // 所有数据行都在滚动栏上侧
                    _groupHeader.Arrange(_rcEmpty);
                }
            }

            // 布局可视行
            int iStart = iDataRow;
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
                    var row = _dataRows[iDataRow];
                    // 行末尾项宽度加1为隐藏右边框
                    bool isRowLast = (indexInGroup + 1) % _colCount == 0;
                    row.Arrange(new Rect(indexInGroup % _colCount * _itemWidth, totalHeight, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
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

                // 剩下行不可见，结束布局
                if (_deltaY + totalHeight > _maxSize.Height)
                    break;
            }

            // 无法确定剩余的虚拟行，上次布局在不可见区域不需再布局！

            // 将剩余分组布局到空区域
            if (iGrpRow < _owner.GroupRows.Count)
            {
                for (int i = iGrpRow; i < _owner.GroupRows.Count; i++)
                {
                    _owner.GroupRows[i].Arrange(_rcEmpty);
                }
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
                if (_owner.GroupRows.Count > 1)
                    delta += GroupSeparatorHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_maxSize.Width, height);
        }

        protected override void ArrangeRealRows(Size p_finalSize)
        {
            double totalHeight = _toolbarHeight;
            for (int i = 0; i < _dataRows.Count; i++)
            {
                var row = _dataRows[i];

                // 行末尾项宽度加1为隐藏右边框
                bool isRowLast = (i + 1) % _colCount == 0;
                row.Arrange(new Rect(i % _colCount * _itemWidth, totalHeight, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));
                if (isRowLast)
                    totalHeight += _rowHeight;
            }
        }

        protected override void ArrangeGroupRealRows(Size p_finalSize)
        {
            int iGrpRow = 0, iDataRow = 0;
            double totalHeight = _toolbarHeight;
            GroupRow lastGroup = null;
            bool firstVisible = true;
            LvRow lastRow = null;
            // 分组内的项目索引
            int indexInGroup = 0;

            for (int i = 0; i < _owner.MapRows.Count; i++)
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
                    var gr = _owner.GroupRows[iGrpRow++];

                    // 若未进入可视区
                    if (firstVisible)
                    {
                        // top为行的上侧和滚动栏上侧的距离，bottom为行的下侧距离
                        double top = totalHeight + _deltaY;
                        double bottom = top + gr.DesiredSize.Height;

                        // 可见区域：0 - _maxSize.Height
                        if ((top > 0 && top < _maxSize.Height)
                            || (bottom > 0 && bottom < _maxSize.Height))
                        {
                            // 初次进入可见区，确定可见区顶端行所属分组
                            firstVisible = false;
                            lastGroup = _owner.GroupRows[iGrpRow - 1];
                        }
                    }

                    gr.Arrange(new Rect(0, totalHeight, p_finalSize.Width, gr.DesiredSize.Height));
                    totalHeight += gr.DesiredSize.Height;
                    indexInGroup = 0;
                }
                else
                {
                    var row = _dataRows[iDataRow++];

                    // 若未进入可视区
                    if (firstVisible)
                    {
                        double top = totalHeight + _deltaY;
                        double bottom = top + row.DesiredSize.Height;

                        // 可见区域：0 - _maxSize.Height
                        if ((top > 0 && top < _maxSize.Height)
                            || (bottom > 0 && bottom < _maxSize.Height))
                        {
                            // 初次进入可见区，确定可见区顶端行所属分组
                            firstVisible = false;
                            lastGroup = _owner.GroupRows[iGrpRow - 1];
                        }
                    }

                    bool isRowLast = (indexInGroup + 1) % _colCount == 0;
                    row.Arrange(new Rect(indexInGroup % _colCount * _itemWidth, totalHeight, isRowLast ? _itemWidth + 1 : _itemWidth, _rowHeight));

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
                    indexInGroup++;
                }
            }

            // 分组导航头
            if (_groupHeader != null)
            {
                if (!_isScrolling
                    && _deltaY < _groupHeader.DesiredSize.Height
                    && lastGroup != null)
                {
                    // 离滚动栏顶部距离小于分组导航头高度时显示
                    // 分组导航头高度可能为0！
                    _groupHeader.SetCurrentGroup(lastGroup);
                    // -_deltaY确保始终在滚动栏顶部位置
                    _groupHeader.Arrange(new Rect(0, -_deltaY + _toolbarHeight, p_finalSize.Width, _groupHeader.DesiredSize.Height));
                }
                else
                {
                    // 第一行在滚动栏下侧太远时不显示分组导航头
                    _groupHeader.Arrange(_rcEmpty);
                }
            }
        }
        #endregion
    }
}