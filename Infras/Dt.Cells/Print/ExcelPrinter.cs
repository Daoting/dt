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

        public PrintInfo Info => null;

        public int SheetIndex => 0;

        public Size PageSize => new Size();

        public Rect PrintArea => new Rect();

        public double HeaderMargin => 0;

        public double FooterMargin => 0;

        public int PageCount => 0;

        public void Print(string p_jobName)
        {
        }
    }
#endif
}

