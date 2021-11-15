#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Dt.Sample
{
    public partial class PrintExcel : Win
    {
        PrintInfo _printInfo = new PrintInfo();

        public PrintExcel()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                InitSpread();
            }
            InitPrintInfo();
        }

        #region 初始化
        void InitSpread()
        {
            // 三层列头
            var sheet = _excel.ActiveSheet;
            sheet.AddSpanCell(1, 1, 1, 3);
            sheet.SetValue(1, 1, "Store");
            sheet.AddSpanCell(1, 4, 1, 7);
            sheet.SetValue(1, 4, "Goods");
            sheet.AddSpanCell(2, 1, 1, 2);
            sheet.SetValue(2, 1, "Area");
            sheet.AddSpanCell(2, 3, 2, 1);
            sheet.SetValue(2, 3, "ID");
            sheet.AddSpanCell(2, 4, 1, 2);
            sheet.SetValue(2, 4, "Fruits");
            sheet.AddSpanCell(2, 6, 1, 2);
            sheet.SetValue(2, 6, "Vegetables");
            sheet.AddSpanCell(2, 8, 1, 2);
            sheet.SetValue(2, 8, "Foods");
            sheet.AddSpanCell(2, 10, 2, 1);
            sheet.SetValue(2, 10, "Total");
            sheet.SetValue(3, 1, "State");
            sheet.SetValue(3, 2, "City");
            sheet.SetValue(3, 4, "Grape");
            sheet.SetValue(3, 5, "Apple");
            sheet.SetValue(3, 6, "Potato");
            sheet.SetValue(3, 7, "Tomato");
            sheet.SetValue(3, 8, "SandWich");
            sheet.SetValue(3, 9, "Hamburger");

            // 尾部合计
            sheet.AddSpanCell(4, 1, 7, 1);
            sheet.AddSpanCell(4, 2, 3, 1);
            sheet.AddSpanCell(7, 2, 3, 1);
            sheet.AddSpanCell(10, 2, 1, 2);
            sheet.SetValue(10, 2, "Sub Total:");
            sheet.AddSpanCell(11, 1, 7, 1);
            sheet.AddSpanCell(11, 2, 3, 1);
            sheet.AddSpanCell(14, 2, 3, 1);
            sheet.AddSpanCell(17, 2, 1, 2);
            sheet.SetValue(17, 2, "Sub Total:");
            sheet.AddSpanCell(18, 1, 1, 3);
            sheet.SetValue(18, 1, "Total:");

            // 数据固定列
            sheet.SetValue(4, 1, "NC");
            sheet.SetValue(4, 2, "Raleigh");
            sheet.SetValue(7, 2, "Charlotte");
            sheet.SetValue(4, 3, "001");
            sheet.SetValue(5, 3, "002");
            sheet.SetValue(6, 3, "003");
            sheet.SetValue(7, 3, "004");
            sheet.SetValue(8, 3, "005");
            sheet.SetValue(9, 3, "006");
            sheet.SetValue(11, 1, "PA");
            sheet.SetValue(11, 2, "Philadelphia");
            sheet.SetValue(14, 2, "Pittsburgh");
            sheet.SetValue(11, 3, "007");
            sheet.SetValue(12, 3, "008");
            sheet.SetValue(13, 3, "009");
            sheet.SetValue(14, 3, "010");
            sheet.SetValue(15, 3, "011");
            sheet.SetValue(16, 3, "012");

            // 合计
            sheet.SetFormula(10, 4, 1, 6, "=SUM(E5:E10)");
            sheet.SetFormula(17, 4, 1, 6, "=SUM(E12:E17)");
            sheet.SetFormula(4, 10, 14, 1, "=SUM(E5:J5)");
            sheet.SetFormula(18, 4, 1, 7, "=E11+E18");

            // 行头列头样式
            sheet[1, 1, 3, 10].Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            sheet[4, 1, 18, 3].Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
            sheet[1, 1, 3, 10].HorizontalAlignment = CellHorizontalAlignment.Center;

            sheet.SetBorder(new CellRange(1, 1, 18, 10), new BorderLine(Colors.Black, BorderLineStyle.Thin), SetBorderOptions.All);
            sheet.SetBorder(new CellRange(4, 4, 3, 6), new BorderLine(Colors.Black, BorderLineStyle.Dotted), SetBorderOptions.Inside);
            sheet.SetBorder(new CellRange(7, 4, 3, 6), new BorderLine(Colors.Black, BorderLineStyle.Dotted), SetBorderOptions.Inside);
            sheet.SetBorder(new CellRange(11, 4, 3, 6), new BorderLine(Colors.Black, BorderLineStyle.Dotted), SetBorderOptions.Inside);
            sheet.SetBorder(new CellRange(14, 4, 3, 6), new BorderLine(Colors.Black, BorderLineStyle.Dotted), SetBorderOptions.Inside);

            // 生成随机数据
            FillSampleData(sheet, new CellRange(4, 4, 6, 6));
            FillSampleData(sheet, new CellRange(11, 4, 6, 6));

            _excel.ActiveSheet.FrozenRowCount = 4;
            _excel.ActiveSheet.FrozenColumnCount = 4;

            _excel.ActiveSheet.AddSelection(new CellRange(4, 4, 3, 6));

            // 表格
            sheet.AddTable("sampleTable1", 22, 5, 10, 5, TableStyles.Medium3);

            // 第二页
            sheet = new Worksheet("多行列头");
            _excel.Sheets.Add(sheet);
            sheet.RowCount = 10;
            sheet.ColumnCount = 8;
            sheet.ColumnHeader.RowCount = 2;
            sheet.RowHeader.ColumnCount = 2;

            sheet.AddSpanCell(0, 0, 2, 1, SheetArea.ColumnHeader);
            sheet.SetValue(0, 0, SheetArea.ColumnHeader, "State");

            sheet.AddSpanCell(0, 1, 1, 2, SheetArea.ColumnHeader);
            sheet.SetValue(0, 1, SheetArea.ColumnHeader, "Company");

            sheet.AddSpanCell(0, 3, 1, 2, SheetArea.ColumnHeader);
            sheet.SetValue(0, 3, SheetArea.ColumnHeader, "Category");

            sheet.AddSpanCell(0, 5, 1, 3, SheetArea.ColumnHeader);
            sheet.SetValue(0, 5, SheetArea.ColumnHeader, "Products");

            sheet.SetValue(1, 1, SheetArea.ColumnHeader, "GC");
            sheet.SetValue(1, 2, SheetArea.ColumnHeader, "MS");
            sheet.SetValue(1, 3, SheetArea.ColumnHeader, "License");
            sheet.SetValue(1, 4, SheetArea.ColumnHeader, "Activate");
            sheet.SetValue(1, 5, SheetArea.ColumnHeader, "Win");
            sheet.SetValue(1, 6, SheetArea.ColumnHeader, "Web");
            sheet.SetValue(1, 7, SheetArea.ColumnHeader, "XAML");

            sheet.AddSpanCell(0, 0, 3, 1, SheetArea.RowHeader);
            sheet.AddSpanCell(3, 0, 3, 1, SheetArea.RowHeader);
            sheet.AddSpanCell(6, 0, 3, 1, SheetArea.RowHeader);
            sheet.AddSpanCell(9, 0, 1, 2, SheetArea.RowHeader);

            sheet.SetValue(0, 0, SheetArea.RowHeader, ".NET");
            sheet.SetValue(3, 0, SheetArea.RowHeader, "Java");
            sheet.SetValue(6, 0, SheetArea.RowHeader, "HTML");
            sheet.SetValue(9, 0, SheetArea.RowHeader, "Total:");

            sheet.SetBorder(new CellRange(9, 0, 1, 8), new BorderLine(Colors.Black, BorderLineStyle.Double), SetBorderOptions.Top);

            sheet.SetFormula(9, 0, 1, 8, "=SUM(A1:A9)");

            FillSampleData(sheet, new CellRange(0, 0, 9, 8));
        }

        void InitPrintInfo()
        {
            _printInfo.HeaderLeft = "<Ts><T Text=\"《:sheetname:》\" Bold=\"true\" Underline=\"true\" /><T Text=\"  :date:\" Foreground=\"#FF4500\" /></Ts>";
            _printInfo.HeaderCenter = "<Ts><T Text=\"标识\" Bold=\"true\" /><Img Width=\"30\" /></Ts>";
            using (var stream = ResKit.GetResource("Logo.png"))
            using (var sr = new StreamReader(stream))
            {
                byte[] img = new byte[stream.Length];
                stream.Read(img, 0, (int)stream.Length);
                _printInfo.HeaderCenterImage = img;
            }
            _printInfo.HeaderRight = "<Ts><T Text=\":num:/:cnt:\" Italic=\"true\" /></Ts>";
            _printInfo.FooterRight = "<Ts><T Text=\":num:[:row:-:col:]\" /></Ts>";
            _fv.Data = _printInfo;
        }

        void FillSampleData(Worksheet sheet, CellRange range)
        {
            System.Random r = new System.Random();
            for (int i = 0; i < range.RowCount; i++)
            {
                for (int j = 0; j < range.ColumnCount; j++)
                {
                    sheet.SetValue(range.Row + i, range.Column + j, r.Next(50, 300));
                }
            }
        }
        #endregion

        void OnPrintExcel(object sender, RoutedEventArgs e)
        {
            _excel.Print(_printInfo);
        }
    }
}