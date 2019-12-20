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
        private bool _ascending;
        private FilterButtonInfo _info;
        private SheetView _sheetView;

        public event EventHandler CanExecuteChanged;

        public SortCommand(SheetView sheetView, FilterButtonInfo info, bool ascending)
        {
            this._info = info;
            this._ascending = ascending;
            this._sheetView = sheetView;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (((this._sheetView != null) && (this._sheetView.Worksheet != null)) && (((this._info != null) && (this._info.RowFilter != null)) && (this._info.RowFilter.Range != null)))
            {
                CellRange range = this._info.RowFilter.Range;
                if ((range != null) && SheetView.HasArrayFormulas(this._sheetView.Worksheet, range.Row, range.Column, range.RowCount, range.ColumnCount))
                {
                    this._sheetView.RaiseInvalidOperation("Cannot change part of an array.", null, null);
                }
                else if (!this._sheetView.RaiseRangeSorting(this._info.Column, this._ascending) && this._info.RowFilter.SortColumn(this._info.Column, this._ascending))
                {
                    this._sheetView.RaiseRangeSorted(this._info.Column, this._ascending);
                    this._sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                }
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}

