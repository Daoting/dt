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
        private int _column;
        private FilterButtonInfo _info;
        private SheetView _sheetView;

        public event EventHandler CanExecuteChanged;

        public FilterCommand(SheetView sheet, FilterButtonInfo info, int column)
        {
            this._sheetView = sheet;
            this._column = column;
            this._info = info;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if ((((this._info != null) && (this._info.RowFilter != null)) && ((this._sheetView != null) && (this._sheetView.Worksheet != null))) && ((this._column >= 0) && (this._column < this._sheetView.Worksheet.ColumnCount)))
            {
                HideRowFilter rowFilter = this._info.RowFilter;
                object[] filterValues = parameter as object[];
                this._sheetView.Worksheet.Workbook.SuspendEvent();
                try
                {
                    rowFilter.Unfilter(this._column);
                    rowFilter.RemoveFilterItems(this._column);
                    if (filterValues != null)
                    {
                        for (int i = 0; i < filterValues.Length; i++)
                        {
                            object obj2 = filterValues[i];
                            if (obj2 is DateTime)
                            {
                                DateCondition filterItem = DateCondition.FromDateTime(DateCompareType.EqualsTo, (DateTime) obj2);
                                rowFilter.AddFilterItem(this._column, filterItem);
                            }
                            else if (obj2 is TimeSpan)
                            {
                                TimeSpan span = (TimeSpan) obj2;
                                TimeCondition condition2 = TimeCondition.FromDateTime(DateCompareType.EqualsTo, Dt.Cells.Data.DateTimeExtension.FromOADate(span.TotalDays));
                                rowFilter.AddFilterItem(this._column, condition2);
                            }
                            else
                            {
                                string expected = (string) (obj2 as string);
                                if ((obj2 == BlankFilterItem.Blank) || (expected == null))
                                {
                                    expected = string.Empty;
                                }
                                TextCondition condition3 = TextCondition.FromString(TextCompareType.EqualsTo, expected);
                                rowFilter.AddFilterItem(this._column, condition3);
                            }
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    this._sheetView.Worksheet.Workbook.ResumeEvent();
                }
                if (!this._sheetView.RaiseRangeFiltering(this._column, filterValues))
                {
                    rowFilter.Filter(this._column);
                    this._sheetView.RaiseRangeFiltered(this._column, filterValues);
                }
                this._sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                this._sheetView.InvalidateFloatingObjects();
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

