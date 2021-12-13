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
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a class that can be attached to a <see cref="T:GrapeCity.Windows.SpreadSheet.UI.GcSpreadSheet" /> control by 
    /// <see cref="T:GrapeCity.Windows.SpreadSheet.UI.DrawingObjectManager.DrawingObjectManager" /> to provide custom drawing
    /// objects.
    /// </summary>
    public interface IDrawingObjectProvider
    {
        /// <summary>
        /// Return a list of custom drawing objects.
        /// </summary>
        /// <param name="sheet">The worksheet to get drawing objects. </param>
        /// <param name="row">The base row index.</param>
        /// <param name="column">The base column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <returns></returns>
        DrawingObject[] GetDrawingObjects(Worksheet sheet, int row, int column, int rowCount, int columnCount);
    }
}

