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
    internal class FilterCommand : ICommand
    {
        int _column;
        FilterButtonInfo _info;
        SheetView _sheetView;

        public event EventHandler CanExecuteChanged;

        public FilterCommand(SheetView sheet, FilterButtonInfo info, int column)
        {
            _sheetView = sheet;
            _column = column;
            _info = info;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if ((((_info != null) && (_info.RowFilter != null)) && ((_sheetView != null) && (_sheetView.ActiveSheet != null))) && ((_column >= 0) && (_column < _sheetView.ActiveSheet.ColumnCount)))
            {
                HideRowFilter rowFilter = _info.RowFilter;
                object[] filterValues = parameter as object[];
                _sheetView.ActiveSheet.Workbook.SuspendEvent();
                try
                {
                    rowFilter.Unfilter(_column);
                    rowFilter.RemoveFilterItems(_column);
                    if (filterValues != null)
                    {
                        for (int i = 0; i < filterValues.Length; i++)
                        {
                            object obj2 = filterValues[i];
                            if (obj2 is DateTime)
                            {
                                DateCondition filterItem = DateCondition.FromDateTime(DateCompareType.EqualsTo, (DateTime) obj2);
                                rowFilter.AddFilterItem(_column, filterItem);
                            }
                            else if (obj2 is TimeSpan)
                            {
                                TimeSpan span = (TimeSpan) obj2;
                                TimeCondition condition2 = TimeCondition.FromDateTime(DateCompareType.EqualsTo, Dt.Cells.Data.DateTimeExtension.FromOADate(span.TotalDays));
                                rowFilter.AddFilterItem(_column, condition2);
                            }
                            else
                            {
                                string expected = (string) (obj2 as string);
                                if ((obj2 == BlankFilterItem.Blank) || (expected == null))
                                {
                                    expected = string.Empty;
                                }
                                TextCondition condition3 = TextCondition.FromString(TextCompareType.EqualsTo, expected);
                                rowFilter.AddFilterItem(_column, condition3);
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    _sheetView.ActiveSheet.Workbook.ResumeEvent();
                }
                if (!_sheetView.RaiseRangeFiltering(_column, filterValues))
                {
                    rowFilter.Filter(_column);
                    _sheetView.RaiseRangeFiltered(_column, filterValues);
                }
                _sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                _sheetView.InvalidateFloatingObjects();
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

