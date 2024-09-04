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
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Printing;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Printing;
#endregion

namespace Dt.Cells.UI
{
#if !WIN
    internal class ExcelPrinter
    {
        public ExcelPrinter(Excel p_excel, PrintInfo p_printInfo, int p_sheetIndex)
        {
        }
        
        public void Print(string p_jobName)
        {
        }
        
        /// <summary>
        /// 获得列头或Excel当前表总高
        /// </summary>
        /// <param name="p_sheet"></param>
        /// <param name="p_count"></param>
        /// <param name="p_area"></param>
        /// <returns></returns>
        internal static double GetTotalHeight(Worksheet p_sheet, int p_count, SheetArea p_area)
        {
            double height = 0.0;
            for (int i = 0; i < p_count; i++)
            {
                height += p_sheet.GetActualRowHeight(i, p_area);
            }
            return height;
        }

        /// <summary>
        /// 获得行头或Excel当前表总宽
        /// </summary>
        /// <param name="p_sheet"></param>
        /// <param name="p_count"></param>
        /// <param name="p_area"></param>
        /// <returns></returns>
        internal static double GetTotalWidth(Worksheet p_sheet, int p_count, SheetArea p_area)
        {
            double width = 0.0;
            for (int i = 0; i < p_count; i++)
            {
                width += p_sheet.GetActualColumnWidth(i, p_area);
            }
            return width;
        }

    }
#endif
}

