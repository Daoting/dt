#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    internal class SpreadXTabIndexNavigator : TabIndexNavigator
    {
        Excel _excel;

        public SpreadXTabIndexNavigator(Excel p_excel)
        {
            _excel = p_excel;
            _excel.SelectionChanging += new EventHandler<SelectionChangingEventArgs>(OnSelectionChanging);
        }

        public override void BringCellToVisible(CompositePosition position)
        {
            if ((!position.IsEmpty && (position.Type == DataSheetElementType.Cell)) && (_excel.ActiveSheet != null))
            {
                NavigatorHelper.BringCellToVisible(_excel, position.Row, position.Column);
            }
        }

        public override bool CanMoveCurrentTo(CompositePosition cellPosition)
        {
            var worksheet = _excel.ActiveSheet;
            if ((cellPosition.Type == DataSheetElementType.Cell) && (worksheet != null))
            {
                if (!worksheet.GetActualRowVisible(cellPosition.Row, SheetArea.Cells) || !worksheet.GetActualColumnVisible(cellPosition.Column, SheetArea.Cells))
                {
                    return false;
                }
                StyleInfo info = worksheet.GetActualStyleInfo(cellPosition.Row, cellPosition.Column, SheetArea.Cells);
                if (info != null)
                {
                    return (info.TabStop && info.Focusable);
                }
            }
            return base.CanMoveCurrentTo(cellPosition);
        }

        public override List<int> GetColumnIndexes()
        {
            List<int> list = new List<int>();
            if (_excel.ActiveSheet != null)
            {
                for (int i = 0; i < _excel.ActiveSheet.Columns.Count; i++)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        public override List<int> GetColumnIndexes(CompositeRange range)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < range.ColumnCount; i++)
            {
                list.Add(range.Column + i);
            }
            return list;
        }

        public override int GetColumnSpan(CompositePosition position)
        {
            if ((_excel.ActiveSheet != null) && (_excel.ActiveSheet.SpanModel != null))
            {
                CellRange range = _excel.ActiveSheet.SpanModel.Find(position.Row, position.Column);
                if (range != null)
                {
                    return range.ColumnCount;
                }
            }
            return 1;
        }

        public override CompositeRange GetFixedRange(CompositeRange range)
        {
            int compositeRowCount = CompositeRowCount;
            int compositeColumnCount = CompositeColumnCount;
            if ((range.Row == -1) && (range.Column == -1))
            {
                return new CompositeRange(DataSheetElementType.Cell, 0, 0, compositeRowCount, compositeColumnCount);
            }
            if (range.Row == -1)
            {
                return new CompositeRange(DataSheetElementType.Cell, 0, range.Column, compositeRowCount, range.ColumnCount);
            }
            if (range.Column == -1)
            {
                return new CompositeRange(DataSheetElementType.Cell, range.Row, 0, range.RowCount, compositeColumnCount);
            }
            return new CompositeRange(DataSheetElementType.Cell, range.Row, range.Column, range.RowCount, range.ColumnCount);
        }

        public override int GetRowSpan(CompositePosition position)
        {
            if ((_excel.ActiveSheet != null) && (_excel.ActiveSheet.SpanModel != null))
            {
                CellRange range = _excel.ActiveSheet.SpanModel.Find(position.Row, position.Column);
                if (range != null)
                {
                    return range.RowCount;
                }
            }
            return 1;
        }

        public override CompositePosition GetTopLeft(CompositeRange range)
        {
            if (!range.IsEmpty)
            {
                return new CompositePosition(range.Type, range.Row, range.Column);
            }
            return CompositePosition.Empty;
        }

        public override bool IsMerged(CompositePosition position, out CompositePosition topLeftPosition)
        {
            topLeftPosition = position;
            if ((_excel.ActiveSheet != null) && (_excel.ActiveSheet.SpanModel != null))
            {
                CellRange range = _excel.ActiveSheet.SpanModel.Find(position.Row, position.Column);
                if (range != null)
                {
                    topLeftPosition = new CompositePosition(DataSheetElementType.Cell, range.Row, range.Column);
                    return true;
                }
            }
            return false;
        }

        void OnSelectionChanging(object sender, SelectionChangingEventArgs e)
        {
            base.OnSelectionChanged(sender, EventArgs.Empty);
        }

        public override int CompositeColumnCount
        {
            get { return  _excel.ActiveSheet.ColumnCount; }
        }

        public override int CompositeRowCount
        {
            get { return  _excel.ActiveSheet.RowCount; }
        }

        public override IList<CompositeRange> Selections
        {
            get
            {
                List<CompositeRange> list = new List<CompositeRange>();
                var worksheet = _excel.ActiveSheet;
                if (worksheet != null)
                {
                    foreach (CellRange range in worksheet.Selections)
                    {
                        CompositeRange empty = CompositeRange.Empty;
                        if ((range.Row == -1) && (range.Column == -1))
                        {
                            empty = new CompositeRange(DataSheetElementType.Sheet, -1, -1, 0, 0);
                        }
                        else if (range.Row == -1)
                        {
                            if ((range.Column + range.ColumnCount) <= worksheet.ColumnCount)
                            {
                                empty = new CompositeRange(DataSheetElementType.Column, -1, range.Column, 0, range.ColumnCount);
                            }
                        }
                        else if (range.Column == -1)
                        {
                            if ((range.Row + range.RowCount) <= worksheet.RowCount)
                            {
                                empty = new CompositeRange(DataSheetElementType.Row, range.Row, -1, range.RowCount, 0);
                            }
                        }
                        else if (((range.Column + range.ColumnCount) <= worksheet.ColumnCount) && ((range.Row + range.RowCount) <= worksheet.RowCount))
                        {
                            empty = new CompositeRange(DataSheetElementType.Cell, range.Row, range.Column, range.RowCount, range.ColumnCount);
                        }
                        if (empty != CompositeRange.Empty)
                        {
                            list.Add(empty);
                        }
                    }
                }
                return (IList<CompositeRange>) list;
            }
        }
    }
}

