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
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    internal static class CellHorizontalAlignmentHelper
    {
        static bool IsExcelRightAlignmentValue(object value)
        {
            return ((((((value is TimeSpan) || (value is DateTime)) || ((value is double) || (value is float))) || (((value is decimal) || (value is long)) || ((value is int) || (value is short)))) || (((value is sbyte) || (value is ulong)) || ((value is uint) || (value is ushort)))) || (value is byte));
        }

        public static VerticalAlignment ToVerticalAlignment(this CellVerticalAlignment cellAlignment)
        {
            switch (cellAlignment)
            {
                case CellVerticalAlignment.Top:
                    return VerticalAlignment.Top;

                case CellVerticalAlignment.Center:
                    return VerticalAlignment.Center;

                case CellVerticalAlignment.Bottom:
                    return VerticalAlignment.Bottom;
            }
            return VerticalAlignment.Center;
        }
    }
}

