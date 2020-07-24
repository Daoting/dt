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
    internal class CustomProvider : IDrawingObjectProvider
    {
        Worksheet _sheet;

        public CustomProvider(Worksheet workshet)
        {
            _sheet = workshet;
        }

        public DrawingObject[] GetDrawingObjects(Worksheet sheet, int row, int column, int rowCount, int columnCount)
        {
            return _sheet.GetDrawingObject(row, column, rowCount, columnCount);
        }
    }
}

