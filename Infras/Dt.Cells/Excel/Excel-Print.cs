#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2024-03-15 ����
******************************************************************************/
#endregion

#region ��������
using Dt.Cells.Data;
using Dt.Cells.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        /// <summary>
        /// ��ӡSheet����
        /// </summary>
        /// <param name="p_printInfo">��ӡ����</param>
        /// <param name="p_sheetIndex">Ҫ��ӡ��Sheet������-1��ʾ��ǰ�Sheet</param>
        /// <param name="p_title">����</param>
        public void Print(PrintInfo p_printInfo = null, int p_sheetIndex = -1, string p_title = null)
        {
#if WIN
            // ������ӡ��Χ
            if (p_sheetIndex >= SheetCount)
                return;

            PrintInfo printInfo = p_printInfo;
            if (printInfo == null)
            {
                printInfo = new PrintInfo();
            }

            int index = p_sheetIndex;
            if (index == -1)
            {
                index = ActiveSheetIndex;
            }

            string jobName = p_title;
            if (string.IsNullOrWhiteSpace(jobName))
            {
                jobName = Sheets[index].Name;
            }
            
            ExcelPrinter printer = new ExcelPrinter();
            printer.Print(this, printInfo, index, jobName);
#else
            ExcelKit.Warn("��ӡ������ʱֻ֧��Windows��");
#endif
        }

        /// <summary>
        /// ��ӡSheet��ָ������
        /// </summary>
        /// <param name="p_range">����Χ</param>
        /// <param name="p_sheetIndex">Ҫ��ӡ��Sheet������-1��ʾ��ǰ�Sheet</param>
        public void Print(CellRange p_range, int p_sheetIndex = -1)
        {
            PrintInfo pi = new PrintInfo();
            if (p_range != null)
            {
                pi.RowStart = p_range.Row;
                pi.RowEnd = p_range.Row + p_range.RowCount - 1;
                pi.ColumnStart = p_range.Column;
                pi.ColumnEnd = p_range.Column + p_range.ColumnCount - 1;
            }
            Print(pi, p_sheetIndex);
        }
    }
}