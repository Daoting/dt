#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
#endregion

namespace Dt.Cells.Data
{
    internal interface IFloatingObjectSheet : ICalcEvaluator, IThemeSupport
    {
        event EventHandler<CellChangedEventArgs> CellChanged;

        event EventHandler<SheetChangedEventArgs> ColumnChanged;

        event PropertyChangedEventHandler PropertyChanged;

        event EventHandler<SheetChangedEventArgs> RowChanged;

        SpreadChart FindChart(string name);
        FloatingObject FindFloatingObject(string name);
        double GetActualColumnWidth(int column, SheetArea sheetArea);
        double GetActualRowHeight(int row, SheetArea sheetArea);

        int ColumnCount { get; }

        int RowCount { get; }

        Dt.Cells.Data.Workbook Workbook { get; }
    }
}

