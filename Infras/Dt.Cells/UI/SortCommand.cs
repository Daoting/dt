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
using System.Threading;
using System.Windows.Input;
#endregion

namespace Dt.Cells.UI
{
    internal class SortCommand : ICommand
    {
        bool _ascending;
        FilterButtonInfo _info;
        Excel _excel;

        public event EventHandler CanExecuteChanged;

        public SortCommand(Excel p_excel, FilterButtonInfo info, bool ascending)
        {
            _info = info;
            _ascending = ascending;
            _excel = p_excel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (((_excel != null) && (_excel.ActiveSheet != null)) && (((_info != null) && (_info.RowFilter != null)) && (_info.RowFilter.Range != null)))
            {
                CellRange range = _info.RowFilter.Range;
                if ((range != null) && Excel.HasArrayFormulas(_excel.ActiveSheet, range.Row, range.Column, range.RowCount, range.ColumnCount))
                {
                    _excel.RaiseInvalidOperation("Cannot change part of an array.", null, null);
                }
                else if (!_excel.RaiseRangeSorting(_info.Column, _ascending) && _info.RowFilter.SortColumn(_info.Column, _ascending))
                {
                    _excel.RaiseRangeSorted(_info.Column, _ascending);
                    _excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
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

