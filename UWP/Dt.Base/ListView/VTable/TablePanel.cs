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
        protected override void LoadColHeader()
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

        protected override void ClearColHeader()
        {
            _colHeader = null;
            _topLeft = null;
        }

        protected override void OnScrollViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // 始终刷新布局
            InvalidateArrange();
        }

        internal override void OnSortDescChanged()
        {
            base.OnSortDescChanged();
            _colHeader?.SyncSortIcon();
        }
        #endregion

        #region 虚拟行
        protected override bool CreateVirRows()
        {
            if (_owner.Rows.Count == 0)
                return false;

            double topLeftWidth = (_owner.SelectionMode == SelectionMode.Multiple) ? 81 : 40;
            // 超过宽度时水平滚动
            double maxWidth = Math.Max(_maxSize.Width, _owner.Cols.TotalWidth + topLeftWidth);

            // 创建等高的虚拟行，时机：初次、切换行模板、面板大小变化
            // 先添加一行，作为行高标准
            var virRow = _createLvRow(_owner.Rows[0]);
            Children.Add(virRow);
            _dataRows.Add(virRow);

            // 测量行高
            // 给最大高度才能测量出内容的实际高度
            Size testSize = new Size(maxWidth, PanelMaxHeight);
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
                    virRow = _createLvRow(null);
                    Children.Add(virRow);
                    _dataRows.Add(virRow);
                }
            }
            return true;
        }

        protected override Size MeasureVirRows()
        {
            // 列头
            _colHeader.Measure(new Size(_maxSize.Width - _topLeftWidth, _maxSize.Height));
            _topLeft.Measure(new Size(_topLeftWidth, _colHeader.DesiredSize.Height));

            double height = _colHeader.DesiredSize.Height;
            // 超过宽度时水平滚动
            double maxWidth = Math.Max(_maxSize.Width, _owner.Cols.TotalWidth + _topLeftWidth);

            // 数据行
            if (_dataRows.Count > 0)
            {
                // 重新测量
                Size testSize = new Size(maxWidth, _rowHeight);
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
            Size size = new Size(_finalWidth, _maxSize.Height);
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
            if (_groupHeader != null && height > _maxSize.Height)
            {
                _groupHeader.Measure(size);
                height += _groupHeader.DesiredSize.Height;
                // 增加高度使最底部分组能够滚动到顶部，确保和导航位置同步！
                var group = _owner.GroupRows[_owner.GroupRows.Count - 1];
                double delta = _maxSize.Height - _colHeader.DesiredSize.Height - _groupHeader.DesiredSize.Height - group.DesiredSize.Height - group.Data.Count * _rowHeight;
                if (_owner.GroupRows.Count > 1)
                    delta += GroupSeparatorHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_finalWidth, height);
        }

        protected override void ArrangeVirRows(Size p_finalSize)
        {
            ArrangeHeader();

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

            double startTop = _colHeader.DesiredSize.Height + _toolbarHeight;

            // 面板可见，在滚动栏下方，按正常顺序布局
            if (_deltaY >= 0 && _deltaY < _maxSize.Height)
            {
                int iDataRow = _dataRows.Count;
                for (int i = 0; i < _dataRows.Count; i++)
                {
                    var item = _dataRows[i];
                    double top = i * _rowHeight + startTop;

                    // 数据行已结束 或 剩下行不可见，结束布局
                    if (i >= _owner.Rows.Count || _deltaY + top > _maxSize.Height)
                    {
                        iDataRow = i;
                        break;
                    }

                    item.Arrange(new Rect(0, top, p_finalSize.Width, _rowHeight));
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

            // _deltaY < 0 && _deltaY > -p_finalSize.Height
            // 面板顶部超出滚动栏 并且 没有整个面板都超出，此时_deltaY为负数

            // 页面偏移量
            double offset = _deltaY % _pageHeight;
            // 最顶部的数据行索引
            int iRow = (int)Math.Floor(-_deltaY / _rowHeight);
            // 最顶部的虚拟行索引
            int iVirRow = iRow % _dataRows.Count;
            // 页面顶部偏移
            double deltaTop = -_deltaY + offset + startTop;

            for (int i = 0; i < _dataRows.Count; i++)
            {
                var item = _dataRows[iVirRow];
                if (iRow + i < _owner.Rows.Count)
                {
                    // 布局虚拟行
                    double top = iVirRow * _rowHeight + deltaTop;
                    item.Arrange(new Rect(0, top, _finalWidth, _rowHeight));
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
            ArrangeHeader();

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

            // 分组导航头宽度
            double groupWidth = _finalWidth - _deltaX;
            // 顶部起始位置：列头高 + 工具栏高
            double startTop = _colHeader.DesiredSize.Height + _toolbarHeight;

            int iAllRow = 0, iGrpRow = 0, iDataRow = 0;
            double totalHeight = 0;

            //----------------------------------------------
            // 面板可见，在滚动栏下方，按顺序布局所有行
            //----------------------------------------------
            if (_deltaY >= 0 && _deltaY < _maxSize.Height)
            {
                // 分组导航头
                if (_groupHeader != null)
                {
                    if (_groupHeader.DesiredSize.Height > 0)
                    {
                        _groupHeader.SetCurrentGroup(_owner.GroupRows[0]);
                        // 未在滚动栏顶部
                        _groupHeader.Arrange(new Rect(-_deltaX, startTop, groupWidth, _groupHeader.DesiredSize.Height));
                    }
                    else
                    {
                        _groupHeader.Arrange(_rcEmpty);
                    }
                }

                totalHeight = startTop;
                for (int i = 0; i < _owner.MapRows.Count; i++)
                {
                    if (_owner.MapRows[i])
                    {
                        // 布局分组行
                        var gr = _owner.GroupRows[iGrpRow];
                        gr.Arrange(new Rect(0, totalHeight, p_finalSize.Width, gr.DesiredSize.Height));
                        totalHeight += gr.DesiredSize.Height;
                        iGrpRow++;
                    }
                    else
                    {
                        // 布局数据行
                        var row = _dataRows[iDataRow];
                        row.Arrange(new Rect(0, totalHeight, p_finalSize.Width, _rowHeight));
                        row.SetViewRow(_owner.Rows[i - iGrpRow], true);
                        totalHeight += _rowHeight;

                        // 虚拟行都已布局时退出
                        iDataRow++;
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
            iAllRow = _owner.MapRows.Count;
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
                }
                else
                {
                    // 数据行
                    if (totalHeight + _rowHeight > -_deltaY)
                    {
                        // 进入可见区
                        iAllRow = i;
                        break;
                    }

                    totalHeight += _rowHeight;
                }
            }
            // 可见数据行在所有虚拟行的索引
            iDataRow = (iAllRow - iGrpRow) % _dataRows.Count;
            totalHeight += startTop;

            // 分组导航头
            if (_groupHeader != null)
            {
                _groupHeader.SetCurrentGroup(lastGroup);
                _groupHeader.Arrange(new Rect(-_deltaX, -_deltaY + startTop, groupWidth, _groupHeader.DesiredSize.Height));
            }

            // 布局可视行
            int iStart = iDataRow;
            for (int i = iAllRow; i < _owner.MapRows.Count; i++)
            {
                if (_owner.MapRows[i])
                {
                    // 布局分组
                    var gr = _owner.GroupRows[iGrpRow];
                    gr.Arrange(new Rect(-_deltaX, totalHeight, groupWidth, gr.DesiredSize.Height));
                    totalHeight += gr.DesiredSize.Height;
                    iGrpRow++;
                }
                else
                {
                    var row = _dataRows[iDataRow];
                    row.Arrange(new Rect(0, totalHeight, _finalWidth, _rowHeight));
                    row.SetViewRow(_owner.Rows[i - iGrpRow], true);
                    totalHeight += _rowHeight;

                    iDataRow++;
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
            double maxWidth = _maxSize.Width;

            // 列头
            _colHeader.Measure(new Size(maxWidth - _topLeftWidth, _maxSize.Height));
            double colHeaderHeight = _colHeader.DesiredSize.Height;
            double height = colHeaderHeight;
            _topLeft.Measure(new Size(_topLeftWidth, colHeaderHeight));
            double w = _owner.Cols.TotalWidth + _topLeftWidth;
            // 超过宽度时水平滚动
            if (w > maxWidth)
                maxWidth = w;
            Size size = new Size(maxWidth, PanelMaxHeight);

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
                        grp.Top = Math.Max(top - colHeaderHeight, 0);
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
            if (_groupHeader != null && height > _maxSize.Height)
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
                double delta = _maxSize.Height - colHeaderHeight - _groupHeader.DesiredSize.Height - group.DesiredSize.Height - bottomHeight;
                if (_owner.GroupRows.Count > 1)
                    delta += GroupSeparatorHeight;
                // 因uno加1
                if (delta > 0)
                    height += delta + 1;
            }
            return new Size(_finalWidth, height);
        }

        /*********************************************************************************************************/
        // 真实行布局
        // 1. 整个面板不在滚动栏可视区(_deltaY >= _maxSize.Height 或 _deltaY <= -p_finalSize.Height）时，布局到空区域
        // 2. 按真实行顺序布局，行超出下方时的将剩余行布局到空区域，其余的行可见时正常布局，超出上方的行布局到空区域
        /*********************************************************************************************************/

        protected override void ArrangeRealRows(Size p_finalSize)
        {
            ArrangeHeader();

            // 无数据 或 整个面板不在滚动栏可视区时，也要重新布局
            if (_owner.Rows.Count == 0
                || _deltaY >= _maxSize.Height       // 面板在滚动栏下方外部
                || _deltaY <= -p_finalSize.Height)  // 面板在滚动栏上方外部
            {
                foreach (var elem in _dataRows)
                {
                    elem.Arrange(_rcEmpty);
                }
                return;
            }

            double totalHeight = _colHeader.DesiredSize.Height + _toolbarHeight;
            int iDataRow = _dataRows.Count;
            for (int i = 0; i < _dataRows.Count; i++)
            {
                var row = _dataRows[i];

                // top为行的上侧和滚动栏上侧的距离，bottom为行的下侧距离
                double top = totalHeight + _deltaY;
                double bottom = top + row.DesiredSize.Height;

                // 剩下行都不可见，结束布局
                if (top >= _maxSize.Height)
                {
                    iDataRow = i;
                    break;
                }

                // 可见区域：0 - _maxSize.Height
                if ((top > 0 && top < _maxSize.Height)
                    || (bottom > 0 && bottom < _maxSize.Height))
                {
                    // 在可见区域
                    row.Arrange(new Rect(0, totalHeight, _finalWidth, row.DesiredSize.Height));
                }
                else
                {
                    // 不可见
                    row.Arrange(_rcEmpty);
                }
                totalHeight += row.DesiredSize.Height;
            }

            // 将剩余的虚拟行布局到空区域
            if (iDataRow < _dataRows.Count)
            {
                for (int i = iDataRow; i < _dataRows.Count; i++)
                {
                    _dataRows[i].Arrange(_rcEmpty);
                }
            }
        }

        protected override void ArrangeGroupRealRows(Size p_finalSize)
        {
            ArrangeHeader();

            // 整个面板不在滚动栏可视区时，布局到空区域
            if (_deltaY >= _maxSize.Height          // 面板在滚动栏下方外部
                || _deltaY <= -p_finalSize.Height)  // 面板在滚动栏上方外部
            {
                // 数据行
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

            // 分组导航头宽度
            double groupWidth = _finalWidth - _deltaX;
            // 顶部起始位置：列头高 + 工具栏高
            double startTop = _colHeader.DesiredSize.Height + _toolbarHeight;
            int iGrpRow = 0, iDataRow = 0;
            double totalHeight = startTop;
            FrameworkElement row;
            GroupRow lastGroup = null;
            bool firstVisible = true;

            for (int i = 0; i < _owner.MapRows.Count; i++)
            {
                // 按顺序取分组行或数据行
                row = _owner.MapRows[i] ? (FrameworkElement)_owner.GroupRows[iGrpRow++] : _dataRows[iDataRow++];

                // top为行的上侧和滚动栏上侧的距离，bottom为行的下侧距离
                double top = totalHeight + _deltaY;
                double bottom = top + row.DesiredSize.Height;

                // 剩下行都不可见，结束布局
                if (top >= _maxSize.Height)
                {
                    if (_owner.MapRows[i])
                        iGrpRow--;
                    else
                        iDataRow--;
                    break;
                }

                // 可见区域：0 - _maxSize.Height
                if ((top > 0 && top < _maxSize.Height)
                    || (bottom > 0 && bottom < _maxSize.Height))
                {
                    // 在可见区域
                    if (_owner.MapRows[i])
                        row.Arrange(new Rect(-_deltaX, totalHeight, groupWidth, row.DesiredSize.Height));
                    else
                        row.Arrange(new Rect(0, totalHeight, p_finalSize.Width, row.DesiredSize.Height));

                    // 初次进入可见区，确定可见区顶端行所属分组
                    if (firstVisible)
                    {
                        firstVisible = false;
                        lastGroup = _owner.GroupRows[iGrpRow - 1];
                    }
                }
                else
                {
                    // 不可见
                    row.Arrange(_rcEmpty);
                }
                totalHeight += row.DesiredSize.Height;
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

            // 分组导航头
            if (_groupHeader != null)
            {
                if (lastGroup != null)
                {
                    _groupHeader.SetCurrentGroup(lastGroup);
                    double top = _deltaY < 0 ? -_deltaY + startTop : startTop;
                    _groupHeader.Arrange(new Rect(-_deltaX, top, groupWidth, _groupHeader.DesiredSize.Height));
                }
                else
                {
                    _groupHeader.Arrange(_rcEmpty);
                }
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
                    Background = Res.浅灰1,
                    BorderBrush = Res.浅灰2,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(0f, GridUnitType.Auto)},
                        new ColumnDefinition { Width = new GridLength(0f, GridUnitType.Auto)},
                    },
                };

                Button btn = new Button
                {
                    Style = Res.字符按钮,
                    Content = "\uE020",
                };
                ToolTipService.SetToolTip(btn, "全选");
                btn.Click += (s, e) => _owner.SelectAll();
                grid.Children.Add(btn);

                btn = new Button
                {
                    Style = Res.字符按钮,
                    Content = "\uE009",
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
                    Background = Res.浅灰1,
                    BorderBrush = Res.浅灰2,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    IsHitTestVisible = false,
                };
            }
        }

        void ArrangeHeader()
        {
            // 面板在滚动栏下侧时正常布局，超出上侧时始终布局在滚动栏顶部位置
            double top = _deltaY < 0 ? -_deltaY + _toolbarHeight : _toolbarHeight;
            _colHeader.Arrange(new Rect(_topLeftWidth, top, _owner.Cols.TotalWidth, _colHeader.DesiredSize.Height));
            // 测量时DesiredSize(0,0)
            _topLeft.Arrange(new Rect(-_deltaX, top, _topLeftWidth, _colHeader.DesiredSize.Height));
        }
        #endregion
    }
}