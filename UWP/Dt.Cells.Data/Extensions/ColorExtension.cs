#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Runtime.CompilerServices;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    internal static class ColorExtension
    {
        internal static IExcelColor ToExcelColor(this Windows.UI.Color color)
        {
            color = Dt.Cells.Data.ColorHelper.MixTranslucentColor(Colors.White, color);
            return new ExcelColor(GcColor.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}

