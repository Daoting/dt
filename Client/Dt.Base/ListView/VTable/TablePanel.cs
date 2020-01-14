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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 表格面板
    /// </summary>
    public partial class TablePanel : LvPanel
    {
        #region 成员变量
        double _finalWidth;
        ColHeader _colHeader;
        FrameworkElement _topLeft;
        double _topLeftWidth;
        #endregion

        #region 构造方法
        public TablePanel(Lv p_owner) : base(p_owner)
        { }
        #endregion

        #region 重写方法
        protected override void LoadOtherRows()
        {
            // 列头
            if (_colHeader == null)
                CreateColHeader();
            Children.Add(_colHeader);
            Children.Add(_topLeft);
            if (_owner.AutoFocus && _colHeader.Children.Count > 0)
                ((Control)_colHeader.Children[0]).Focus(FocusState.Programmatic);
        }

        internal override void ReceiveFocus()
        {
            if (_colHeader != null && _colHeader.Children.Count > 0)
                ((Control)_colHeader.Children[0]).Focus(FocusState.Programmatic);
        }

        protected override void ClearOtherRows()
        {
            if (_colHeader != null)
            {
                _colHeader = null;
                _topLeft = null;
            }
        }
        #endregion

        #region 虚拟行
        protected override Size MeasureVirRows(double p_maxWidth, double p_maxHeight)
        {
            // 列头
            _colHeader.Measure(new Size(p_maxWidth - _topLeftWidth, p_maxHeight));
            _topLeft.Measure(new Size(_topLeftWidth, _colHeader.DesiredSize.Height));
            double height = _colHeader.DesiredSize.Height;
            double w = _owner.Cols.TotalWidth + _topLeftWidth;
            // 超过宽度时水平滚动
            if (w > p_maxWidth)
                p_maxWidth = w;

            // 数据行
            if (!_initVirRow && _owner.Rows.Count > 0)
            {
                // 创建等高的虚拟行，时机：初次、切换行模板、面板大小变化
                // 先添加一行，作为行高标准
                FrameworkElement virRow = _createLvRow(_owner.Rows[0]);
                Children.Insert(0, virRow);
                _dataRows.Add(virRow);

                // 测量行高
                // 给最大高度才能测量出内容的实际高度
                Size testSize = new Size(p_maxWidth, PanelMaxHeight);
                virRow.Measure(testSize);
                _rowHeight = virRow.DesiredSize.Height;
                _finalWidth = virRow.DesiredSize.Width;

                if (_rowHeight > 0)
                {
                    // 确保子元素刚好摆满可见区域，计算所需行数
                    int rowCount = (int)Math.Ceiling(p_maxHeight / _rowHeight) + 1;
                    _pageHeight = rowCount * _rowHeight;

                    // 补充子元素
                    testSize = new Size(p_maxWidth, _rowHeight);
                    for (int i = 1; i < rowCount; i++)
                    {
                        virRow = _createLvRow(null);
                        Children.Insert(i, virRow);
                        _dataRows.Add(virRow);
                        virRow.Measure(testSize);

                        if (virRow.DesiredSize.Width > _finalWidth)
                            _finalWidth = virRow.DesiredSize.Width;
                    }
                }
                _initVirRow = true;
            }
            else if (_dataRows.Count > 0)
            {
                // 重新测量
                Size testSize = new Size(p_maxWidth, _rowHeight);
                // 表格时始终重置宽度，若只在创建时确定列宽，造成调整列宽时不一致！
                _finalWidth = 0;
                foreach (var row in _dataRows)
                {
                    row.Measure(testSize);
                    if (row.DesiredSize.Width > _finalWidth)
                        _finalWidth = row.DesiredSize.Width;
                }
            }
            height += _rowHeight * _owner.Rows.Count;

            // 分组行
            Size size = new Size(_finalWidth, p_maxHeight);
            if (_owner.GroupRows != null)
            {
                double top = 0;
                foreach (var grp in _owner.GroupRows)
                {
                    grp.Top = top;
                    grp.Measure(size);
                    height += grp.DesiredSize.Height;
                    top += grp.DesiredSize.Height;
                    top += grp.Data.Count * _rowHeight;
                }
            }

            // 分组导航头，出现垂直滚动栏时才显示
            if (_groupHeader != null && height > p_maxHeight)
            {
                _groupHeader.Measure(size);
                height += _groupHeader.DesiredSize.Height;
                // 增加高度使最底部分组能够滚动到顶部，确保和导航位置同步！
                var group = _owner.GroupRows[_owner.GroupRows.Count - 1];
                double delta = p_maxHeight - _colHeader.DesiredSize.Height - _groupHeader.DesiredSize.Height - group.DesiredSize.Height - group.Data.Count * _rowHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_finalWidth, height);
        }

        protected override void ArrangeVirRows(Size p_finalSize)
        {
            ArrangeHeader();
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
            int iRow = (int)Math.Floor(scrollY / _rowHeight);
            // 最顶部的虚拟行索引
            int iVirRow = iRow % _dataRows.Count;
            // 页面顶部偏移
            double deltaTop = scrollY + offset + _colHeader.DesiredSize.Height;

            for (int i = 0; i < _dataRows.Count; i++)
            {
                var item = _dataRows[iVirRow];
                if (iRow + i < _owner.Rows.Count)
                {
                    // 布局虚拟行
                    double top = iVirRow * _rowHeight + deltaTop;
                    item.Arrange(new Rect(0, top, _finalWidth, _rowHeight));
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
            ArrangeHeader();

            int iAllRow = 0, iGrpRow = 0, iDataRow = 0;
            GroupRow lastGroup = null;
            double totalHeight = 0;
            double scrollX = _owner.Scroll.HorizontalOffset;
            double scrollY = _owner.Scroll.VerticalOffset;

            // 有垂直滚动时，计算以上变量值
            if (scrollY > 0)
            {
                double totalRowHeight = 0;
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
                    }
                    else
                    {
                        // 数据行
                        if (totalHeight + _rowHeight > scrollY)
                        {
                            // 进入可见区
                            iAllRow = i;
                            break;
                        }

                        totalHeight += _rowHeight;
                        totalRowHeight += _rowHeight;
                    }
                }
                // 可见数据行在所有虚拟行的索引
                iDataRow = (int)Math.Floor((totalRowHeight % _pageHeight) / _rowHeight);
            }

            // 分组导航头
            double groupWidth = _finalWidth - scrollX;
            totalHeight += _colHeader.DesiredSize.Height;
            if (_groupHeader != null && _owner.Scroll.ScrollableHeight > 0)
            {
                _groupHeader.SetCurrentGroup(lastGroup ?? _owner.GroupRows[0]);
                _groupHeader.Arrange(new Rect(scrollX, scrollY + _colHeader.DesiredSize.Height, groupWidth, _groupHeader.DesiredSize.Height));
                totalHeight += _groupHeader.DesiredSize.Height;
            }

            // 布局可视行
            int iStart = iDataRow;
            for (int i = iAllRow; i < _owner.MapRows.Count; i++)
            {
                if (_owner.MapRows[i])
                {
                    // 布局分组
                    var gr = _owner.GroupRows[iGrpRow];
                    gr.Arrange(new Rect(scrollX, totalHeight, groupWidth, gr.DesiredSize.Height));
                    totalHeight += gr.DesiredSize.Height;
                    iGrpRow++;
                }
                else
                {
                    var row = _dataRows[iDataRow];
                    row.Arrange(new Rect(0, totalHeight, _finalWidth, _rowHeight));
                    ((LvRow)row).SetViewRow(_owner.Rows[i - iGrpRow], true);
                    totalHeight += _rowHeight;

                    iDataRow++;
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
        protected override Size MeasureRealRows(double p_maxWidth, double p_maxHeight)
        {
            // 列头
            _colHeader.Measure(new Size(p_maxWidth - _topLeftWidth, p_maxHeight));
            double height = _colHeader.DesiredSize.Height;
            _topLeft.Measure(new Size(_topLeftWidth, _colHeader.DesiredSize.Height));
            double w = _owner.Cols.TotalWidth + _topLeftWidth;
            // 超过宽度时水平滚动
            if (w > p_maxWidth)
                p_maxWidth = w;
            Size size = new Size(p_maxWidth, PanelMaxHeight);

            _finalWidth = 0;
            if (_owner.MapRows != null)
            {
                // 有分组行
                double top = 0;
                int iGrp = 0, iData = 0;
                foreach (var isGrp in _owner.MapRows)
                {
                    if (isGrp)
                    {
                        var grp = _owner.GroupRows[iGrp++];
                        grp.Top = top;
                        grp.Measure(size);
                        height += grp.DesiredSize.Height;
                        top += grp.DesiredSize.Height;
                    }
                    else
                    {
                        var row = _dataRows[iData++];
                        row.Measure(size);
                        height += row.DesiredSize.Height;
                        top += row.DesiredSize.Height;
                        if (row.DesiredSize.Width > _finalWidth)
                            _finalWidth = row.DesiredSize.Width;
                    }
                }
            }
            else
            {
                // 只数据行
                foreach (var row in _dataRows)
                {
                    row.Measure(size);
                    height += row.DesiredSize.Height;
                    if (row.DesiredSize.Width > _finalWidth)
                        _finalWidth = row.DesiredSize.Width;
                }
            }

            // 分组导航头，出现垂直滚动栏时才显示
            if (_groupHeader != null && height > p_maxHeight)
            {
                _groupHeader.Measure(new Size(_finalWidth, PanelMaxHeight));
                height += _groupHeader.DesiredSize.Height;
                // 增加高度使最底部分组能够滚动到顶部，确保和导航位置同步！
                var group = _owner.GroupRows[_owner.GroupRows.Count - 1];
                double bottomHeight = 0;
                for (int i = 1; i <= group.Data.Count; i++)
                {
                    bottomHeight += _dataRows[_dataRows.Count - i].DesiredSize.Height;
                }
                double delta = p_maxHeight - _colHeader.DesiredSize.Height - _groupHeader.DesiredSize.Height - group.DesiredSize.Height - bottomHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_finalWidth, height);
        }

        protected override void ArrangeRealRows(Size p_finalSize)
        {
            int index = 0;
            double totalHeight = 0;
            double scrollY = _owner.Scroll.VerticalOffset;
            double bottomY = GetViewBottom();

            ArrangeHeader();
            // 布局滚动后不可见行
            if (scrollY > 0)
            {
                for (int i = 0; i < _dataRows.Count; i++)
                {
                    double height = _dataRows[i].DesiredSize.Height;
                    if (totalHeight + height > scrollY)
                    {
                        // 进入可见
                        index = i;
                        break;
                    }

                    // 不可见
                    _dataRows[i].Arrange(_rcEmpty);
                    totalHeight += height;
                }
            }
            totalHeight += _colHeader.DesiredSize.Height;

            // 布局可视行
            for (int i = index; i < _dataRows.Count; i++)
            {
                var row = _dataRows[i];
                row.Arrange(new Rect(0, totalHeight, _finalWidth, row.DesiredSize.Height));
                totalHeight += row.DesiredSize.Height;
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
            double scrollX = _owner.Scroll.HorizontalOffset;
            double scrollY = _owner.Scroll.VerticalOffset;
            double bottomY = GetViewBottom();

            ArrangeHeader();
            // 有垂直滚动时，计算以上变量值
            if (scrollY > 0)
            {
                double height;
                for (int i = 0; i < _owner.MapRows.Count; i++)
                {
                    if (_owner.MapRows[i])
                    {
                        // 分组行
                        // 布局上个分组
                        if (lastGroup != null)
                            lastGroup.Arrange(_rcEmpty);

                        lastGroup = _owner.GroupRows[iGrpRow];
                        height = lastGroup.DesiredSize.Height;
                        if (totalHeight + height > scrollY)
                        {
                            // 进入可见区
                            iAllRow = i;
                            break;
                        }

                        iGrpRow++;
                        totalHeight += height;
                    }
                    else
                    {
                        // 数据行
                        height = _dataRows[iDataRow].DesiredSize.Height;
                        if (totalHeight + height > scrollY)
                        {
                            // 进入可见区
                            iAllRow = i;
                            break;
                        }

                        totalHeight += height;
                        _dataRows[iDataRow++].Arrange(_rcEmpty);
                    }
                }
            }

            // 分组导航头
            double groupWidth = _finalWidth - scrollX;
            totalHeight += _colHeader.DesiredSize.Height;
            if (_groupHeader != null && _owner.Scroll.ScrollableHeight > 0)
            {
                _groupHeader.SetCurrentGroup(lastGroup ?? _owner.GroupRows[0]);
                _groupHeader.Arrange(new Rect(scrollX, scrollY + _colHeader.DesiredSize.Height, groupWidth, _groupHeader.DesiredSize.Height));
                totalHeight += _groupHeader.DesiredSize.Height;
            }

            // 布局可视行
            for (int i = iAllRow; i < _owner.MapRows.Count; i++)
            {
                bool isGrp = _owner.MapRows[i];
                if (isGrp)
                {
                    // 布局分组
                    var gr = _owner.GroupRows[iGrpRow];
                    gr.Arrange(new Rect(scrollX, totalHeight, groupWidth, gr.DesiredSize.Height));
                    totalHeight += gr.DesiredSize.Height;
                    iGrpRow++;
                }
                else
                {
                    var row = _dataRows[iDataRow];
                    row.Arrange(new Rect(0, totalHeight, _finalWidth, row.DesiredSize.Height));
                    totalHeight += row.DesiredSize.Height;
                    iDataRow++;
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

        #region 内部方法
        void CreateColHeader()
        {
            // 只附加一次
            _owner.Cols.Update += (s, e) => InvalidateMeasure();
            _colHeader = new ColHeader(_owner);

            // 左上角
            _topLeftWidth = (_owner.SelectionMode == SelectionMode.Multiple) ? 81 : 40;
            if (_owner.SelectionMode == SelectionMode.Multiple)
            {
                Grid grid = new Grid
                {
                    Background = AtRes.浅灰背景,
                    BorderBrush = AtRes.浅灰边框,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(0f, GridUnitType.Auto)},
                        new ColumnDefinition { Width = new GridLength(0f, GridUnitType.Auto)},
                    },
                };

                Button btn = new Button
                {
                    Style = AtRes.字符按钮,
                    Content = "\uE08A",
                };
                ToolTipService.SetToolTip(btn, "全选");
                btn.Click += (s, e) => _owner.SelectAll();
                grid.Children.Add(btn);

                btn = new Button
                {
                    Style = AtRes.字符按钮,
                    Content = "\uE018",
                };
                ToolTipService.SetToolTip(btn, "清除所选");
                btn.Click += (s, e) => _owner.ClearSelection();
                Grid.SetColumn(btn, 1);
                grid.Children.Add(btn);
                _topLeft = grid;
            }
            else
            {
                _topLeft = new Border
                {
                    Background = AtRes.浅灰背景,
                    BorderBrush = AtRes.浅灰边框,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    IsHitTestVisible = false,
                };
            }
        }

        void ArrangeHeader()
        {
            _colHeader.Arrange(new Rect(_topLeftWidth, _owner.Scroll.VerticalOffset, _owner.Cols.TotalWidth, _colHeader.DesiredSize.Height));
            // 测量时DesiredSize(0,0)
            _topLeft.Arrange(new Rect(_owner.Scroll.HorizontalOffset, _owner.Scroll.VerticalOffset, _topLeftWidth, _colHeader.DesiredSize.Height));
        }
        #endregion
    }
}