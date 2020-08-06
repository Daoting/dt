#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the data with a specified change type for the CellChanged event.
    /// </summary>
    public class SheetChangedEventArgs : CellChangedEventArgs
    {
        SheetChangedEventAction type;

        internal SheetChangedEventArgs(string propertyName, int row, int column, SheetArea sheetArea, SheetChangedEventAction t) : base(propertyName, row, column, 1, 1, sheetArea)
        {
            this.type = t;
        }

        internal SheetChangedEventArgs(string propertyName, int row, int column, int rowCount, int columnCount, SheetArea sheetArea, SheetChangedEventAction t) : base(propertyName, row, column, rowCount, columnCount, sheetArea)
        {
            this.type = t;
        }

        /// <summary>
        /// Gets the change type for the event.
        /// </summary>
        /// <value>The change type for the event.</value>
        public SheetChangedEventAction Type
        {
            get { return  this.type; }
        }
    }
}

