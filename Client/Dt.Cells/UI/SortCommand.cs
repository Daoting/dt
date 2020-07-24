#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Threading;
using System.Windows.Input;
#endregion

namespace Dt.Cells.UI
{
    internal class SortCommand : ICommand
    {
        bool _ascending;
        FilterButtonInfo _info;
        SheetView _sheetView;

        public event EventHandler CanExecuteChanged;

        public SortCommand(SheetView sheetView, FilterButtonInfo info, bool ascending)
        {
            _info = info;
            _ascending = ascending;
            _sheetView = sheetView;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (((_sheetView != null) && (_sheetView.Worksheet != null)) && (((_info != null) && (_info.RowFilter != null)) && (_info.RowFilter.Range != null)))
            {
                CellRange range = _info.RowFilter.Range;
                if ((range != null) && SheetView.HasArrayFormulas(_sheetView.Worksheet, range.Row, range.Column, range.RowCount, range.ColumnCount))
                {
                    _sheetView.RaiseInvalidOperation("Cannot change part of an array.", null, null);
                }
                else if (!_sheetView.RaiseRangeSorting(_info.Column, _ascending) && _info.RowFilter.SortColumn(_info.Column, _ascending))
                {
                    _sheetView.RaiseRangeSorted(_info.Column, _ascending);
                    _sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                }
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}

