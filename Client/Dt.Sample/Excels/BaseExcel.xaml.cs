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
    public partial class BaseExcel : Win
    {
        BorderLineStyle[] LineStyleArray = new BorderLineStyle[] { BorderLineStyle.DashDotDot, BorderLineStyle.Medium, BorderLineStyle.Thick, BorderLineStyle.Double };
        //插入图形用到的变量
        int _cntEllipse = 1;
        int _cntRect = 1;
        MyShape _myRectangle;

        public BaseExcel()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                InitSpread();
            }
            InitProperty();
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
            _excel.EnterCell += gcSpreadSheet1_EnterCell;

            // 表格
            sheet.AddTable("sampleTable1", 22, 5, 10, 5, TableStyles.Medium3);

            SetBorder(sheet.Cells[20, 6], new BorderLine(Colors.Red, BorderLineStyle.DashDotDot));

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

        void InitProperty()
        {
            var properties = from p in typeof(TableStyles).GetTypeInfo().DeclaredProperties
                             where p.CanRead && p.GetMethod.IsStatic && p.GetMethod.IsPublic
                             select p;
            foreach (var property in properties)
            {
                if (property.Name != "CustomStyles")
                    _cbTableStyle.Items.Add(property.Name);
            }
            _cbTableStyle.SelectedIndex = 22;

            _cbLineStyle.Items.Add(BorderLineStyle.DashDot);
            _cbLineStyle.Items.Add(BorderLineStyle.DashDotDot);
            _cbLineStyle.Items.Add(BorderLineStyle.Dashed);
            _cbLineStyle.Items.Add(BorderLineStyle.Dotted);
            _cbLineStyle.Items.Add(BorderLineStyle.Double);
            _cbLineStyle.Items.Add(BorderLineStyle.None);
            _cbLineStyle.Items.Add(BorderLineStyle.Hair);
            _cbLineStyle.Items.Add(BorderLineStyle.Medium);
            _cbLineStyle.Items.Add(BorderLineStyle.MediumDashDot);
            _cbLineStyle.Items.Add(BorderLineStyle.MediumDashDotDot);
            _cbLineStyle.Items.Add(BorderLineStyle.MediumDashed);
            _cbLineStyle.Items.Add(BorderLineStyle.SlantedDashDot);
            _cbLineStyle.Items.Add(BorderLineStyle.Thick);
            _cbLineStyle.Items.Add(BorderLineStyle.Thin);
            _cbLineStyle.SelectedIndex = 0;

            properties = from p in typeof(Colors).GetTypeInfo().DeclaredProperties
                         where p.CanRead && p.GetMethod.IsStatic && p.GetMethod.IsPublic
                         select p;
            foreach (var property in properties)
            {
                _cbBorderColor.Items.Add(property.Name);
                _cbGridlineColor.Items.Add(property.Name);
            }
            _cbBorderColor.SelectedIndex = 0;
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

        #region 当前格
        void gcSpreadSheet1_EnterCell(object sender, EnterCellEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            _cbColCanResize.IsChecked = sheet.GetColumnResizable(sheet.ActiveColumnIndex, SheetArea.Cells);
            _cbRowCanResize.IsChecked = sheet.GetRowResizable(sheet.ActiveRowIndex, SheetArea.Cells);
        }

        void OnColumnCanResize(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            if ((bool)_cbColCanResize.IsChecked)
            {
                sheet.SetColumnResizable(sheet.ActiveColumnIndex, SheetArea.Cells, true);
                sheet.ActiveColumn.ResetBackground();
            }
            else
            {
                sheet.SetColumnResizable(sheet.ActiveColumnIndex, SheetArea.Cells, false);
                sheet.ActiveColumn.Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            }
        }

        void OnRowCanResize(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            if ((bool)_cbRowCanResize.IsChecked)
            {
                sheet.SetRowResizable(sheet.ActiveRowIndex, SheetArea.Cells, true);
                sheet.ActiveRow.ResetBackground();
            }
            else
            {
                sheet.SetRowResizable(sheet.ActiveRowIndex, SheetArea.Cells, false);
                sheet.ActiveRow.Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
            }
        }

        void showUnderLine_Checked(object sender, RoutedEventArgs e)
        {
            _excel.ActiveSheet.ActiveCell.Underline = (bool)(sender as CheckBox).IsChecked;
        }

        void showStrickLine_Checked(object sender, RoutedEventArgs e)
        {
            _excel.ActiveSheet.ActiveCell.Strikethrough = (bool)(sender as CheckBox).IsChecked;
        }
        #endregion

        #region 合并/拆分
        void OnMergeClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Worksheet sheet = _excel.ActiveSheet;
            foreach (CellRange cellRange in sheet.Selections)
            {
                CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                sheet.Cells[range.Row, range.Column].RowSpan = range.RowCount;
                sheet.Cells[range.Row, range.Column].ColumnSpan = range.ColumnCount;
            }
        }

        void OnUnMergeClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Worksheet sheet = _excel.ActiveSheet;
            foreach (CellRange cellRange in sheet.Selections)
            {
                CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                for (int i = 0; i < range.RowCount; i++)
                {
                    for (int j = 0; j < range.ColumnCount; j++)
                    {
                        sheet.Cells[range.Row + i, range.Column + j].RowSpan = 1;
                        sheet.Cells[range.Row + i, range.Column + j].ColumnSpan = 1;
                    }
                }
            }
        }

        CellRange GetActualCellRange(CellRange cellRange, int rowCount, int columnCount)
        {
            if (cellRange.Row == -1 && cellRange.Column == -1)
            {
                return new CellRange(0, 0, rowCount, columnCount);
            }
            else if (cellRange.Row == -1)
            {
                return new CellRange(0, cellRange.Column, rowCount, cellRange.ColumnCount);
            }
            else if (cellRange.Column == -1)
            {
                return new CellRange(cellRange.Row, 0, cellRange.RowCount, columnCount);
            }

            return cellRange;
        }
        #endregion

        #region 行列增删
        void OnAddCol(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.AddColumns(sheet.ActiveColumnIndex, 1);
        }

        void OnRemoveCol(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.RemoveColumns(sheet.ActiveColumnIndex, 1);
        }

        void OnAddRow(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.AddRows(sheet.ActiveRowIndex, 1);
        }

        void OnRemoveRow(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.RemoveRows(sheet.ActiveRowIndex, 1);
        }

        void btnAddColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.ColumnHeader.RowCount += 1;
        }

        void btnRemoveColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.ColumnHeader.RowCount -= 1;
        }

        void btnAddRowHeader_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.RowHeader.ColumnCount += 1;
        }

        void btnRemoveRowHeader_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            sheet.RowHeader.ColumnCount -= 1;
        }
        #endregion

        #region 表格
        void cboTableStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            foreach (var range in sheet.Selections)
            {
                var sel = GetActualCellRange(range, sheet.RowCount, sheet.ColumnCount);
                for (var r = 0; r < sel.RowCount; r++)
                {
                    for (var c = 0; c < sel.ColumnCount; c++)
                    {
                        var table = sheet.FindTable(r + sel.Row, c + sel.Column);
                        if (table != null)
                        {
                            table.Style = getTableStyle(_cbTableStyle.SelectedItem.ToString());
                        }
                    }
                }
            }
        }

        TableStyle getTableStyle(string propertyName)
        {
            var property = (from p in typeof(TableStyles).GetTypeInfo().DeclaredProperties
                            where p.Name == propertyName && p.CanRead && p.GetMethod.IsStatic && p.GetMethod.IsPublic
                            select p).First();
            return property.GetValue(null, null) as TableStyle;
        }

        void btnAddTable_Click(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            if (sheet.Selections.Count > 0)
            {
                var sel = sheet.Selections[0];
                sheet.AddTable(getTableName(), sel.Row, sel.Column, sel.RowCount, sel.ColumnCount, getTableStyle(_cbTableStyle.SelectedItem.ToString()));
            }
        }

        void btnRemoveTable_Click(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            foreach (var range in sheet.Selections)
            {
                var sel = GetActualCellRange(range, sheet.RowCount, sheet.ColumnCount);
                for (var r = 0; r < sel.RowCount; r++)
                {
                    for (var c = 0; c < sel.ColumnCount; c++)
                    {
                        var table = sheet.FindTable(r + sel.Row, c + sel.Column);
                        if (table != null)
                        {
                            sheet.RemoveTable(table);
                        }
                    }
                }
            }
        }

        string getTableName()
        {
            Worksheet sheet; ;
            var i = 0;
            do
            {
                var name = "sheetTable" + i.ToString();
                if (_excel.Workbook.FindTable(name, out sheet) == null)
                {
                    return name;
                }
                i++;
            } while (true);
        }
        #endregion

        #region 边框
        void setBorderButton_Click(object sender, RoutedEventArgs e)
        {
            using (_excel.Defer())
            {
                Worksheet sheet = _excel.ActiveSheet;
                foreach (CellRange cellRange in sheet.Selections)
                {
                    CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                    for (int i = 0; i < range.RowCount; i++)
                    {
                        for (int j = 0; j < range.ColumnCount; j++)
                        {
                            SetBorder(sheet.Cells[i + range.Row, j + range.Column], new BorderLine(GetColor(_cbBorderColor.SelectedItem.ToString()),
                                GetBorderLineStyle(_cbLineStyle.SelectedItem.ToString())));
                        }
                    }
                }
            }
        }

        void clearBorderButton_Click(object sender, RoutedEventArgs e)
        {
            using (_excel.Defer())
            {
                Worksheet sheet = _excel.ActiveSheet;

                foreach (CellRange cellRange in sheet.Selections)
                {
                    CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                    for (int i = 0; i < range.RowCount; i++)
                    {
                        for (int j = 0; j < range.ColumnCount; j++)
                        {
                            SetBorder(sheet.Cells[i + range.Row, j + range.Column], null);
                        }
                    }
                }
            }
        }

        void SetBorder(Cell cell, BorderLine border)
        {
            if (border == null)
            {
                cell.BorderLeft = cell.BorderTop = cell.BorderRight = cell.BorderBottom = null;
            }
            else
            {
                cell.BorderLeft = border.Clone() as BorderLine;
                cell.BorderTop = border.Clone() as BorderLine;
                cell.BorderRight = border.Clone() as BorderLine;
                cell.BorderBottom = border.Clone() as BorderLine;
            }
        }

        Color GetColor(string name)
        {
            return (Color)typeof(Colors).GetTypeInfo().GetDeclaredProperty(name).GetValue(null, null);
        }

        BorderLineStyle GetBorderLineStyle(int index)
        {
            return LineStyleArray[index % LineStyleArray.Length];
        }

        BorderLineStyle GetBorderLineStyle(string name)
        {
            return (BorderLineStyle)Enum.Parse(typeof(BorderLineStyle), name, true);
        }

        void borderColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cbGridlineColor.SelectedItem != null)
                _excel.ActiveSheet.GridLineColor = GetColor(_cbGridlineColor.SelectedItem.ToString());
        }
        #endregion

        #region 插入元素
        void AddRectangleButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Point point = new Point(500, 500);
            AddRectangle(point, GetRandomColor());
        }

        void AddEllipseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Point point = new Point(500, 500);
            AddEllipse(point, GetRandomColor());
        }

        async void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            StorageFile file = await filePicker.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    _excel.SuspendEvent();
                    var stream = await file.OpenStreamForReadAsync();
                    _excel.ActiveSheet.AddPicture(CreatePictureName(), stream);
                    stream.Dispose();
                }
                finally
                {
                    _excel.ResumeEvent();
                    _excel.RefreshPictures();
                }
            }
        }

        void AddBorderButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            if (sheet.Selections.Count == 0)
                return;

            CellRange cr = sheet.Selections[0];
            double X = 0d, Y = 0d;
            double width = 0d;
            double height = 0d;

            for (int i = 0; i < cr.Row; i++)
            {
                Y += sheet.GetRowHeight(i);
            }
            for (int i = 0; i < cr.Column; i++)
            {
                X += sheet.GetColumnWidth(i);
            }
            for (int i = 0; i < cr.RowCount; i++)
            {
                height += sheet.GetRowHeight(cr.Row + i);
            }
            for (int i = 0; i < cr.ColumnCount; i++)
            {
                width += sheet.GetColumnWidth(cr.Column + i);
            }
            if (_myRectangle != null)
            {
                sheet.FloatingObjects.Remove(_myRectangle);
                _myRectangle = null;
            }
            _myRectangle = new MyShape("selRec", X, Y, width, height);
            Rectangle rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.Transparent),
                StrokeThickness = 3,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            (_myRectangle.Content as Grid).Children.Add(rectangle);
            sheet.FloatingObjects.Add(_myRectangle);
        }

        string CreatePictureName()
        {
            SpreadPictures picutres = _excel.ActiveSheet.Pictures;
            return "Picture" + (picutres.Count > 0 ? Int32.Parse(picutres[picutres.Count - 1].Name.Substring(7)) + 1 : 1);
        }

        void AddRectangle(Point point, Color fillColor)
        {
            MyShape myShape = new MyShape("Rectuangle" + _cntRect, point.X, point.Y, 200, 200);
            Rectangle rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(fillColor),
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            (myShape.Content as Grid).Children.Add(rectangle);
            _excel.ActiveSheet.FloatingObjects.Add(myShape);

            _cntRect++;
        }

        void AddEllipse(Point point, Color fillColor)
        {
            MyShape myShape = new MyShape("Ellipse" + _cntEllipse, point.X, point.Y, 200, 200);
            Ellipse ellipse = new Ellipse()
            {
                Fill = new SolidColorBrush(fillColor),
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            (myShape.Content as Grid).Children.Add(ellipse);
            _excel.ActiveSheet.FloatingObjects.Add(myShape);
            _cntEllipse++;
        }

        class MyShape : CustomFloatingObject, IXmlSerializable
        {
            public MyShape(string name, double x, double y, double width, double height)
                : base(name, x, y, width, height)
            { }

            public MyShape(string name)
                : base(name, 0.0, 0.0, 200.0, 200.0)
            { }

            public MyShape()
                : base(string.Empty)
            { }

            FrameworkElement _content;
            public override FrameworkElement Content
            {
                get
                {
                    if (_content == null)
                    {
                        _content = new Grid();
                    }
                    return _content;
                }
            }

            public override object Clone()
            {
                MyShape myShape = new MyShape();
                myShape.Size = Size;
                Shape shape = (Shape)(Content as Grid).Children[0];
                //   string typeName = shape.GetType().UnderlyingSystemType.Name;
                string typeName = shape.GetType().Name;
                if (typeName.Equals("Rectangle"))
                {
                    Rectangle rectangleClone = new Rectangle()
                    {
                        Stroke = shape.Stroke,
                        StrokeThickness = shape.StrokeThickness,
                        Fill = shape.Fill
                    };
                    (myShape.Content as Grid).Children.Add(rectangleClone);
                    return myShape;
                }
                else
                {
                    Ellipse ellipseClone = new Ellipse()
                    {
                        Stroke = shape.Stroke,
                        StrokeThickness = shape.StrokeThickness,
                        Fill = shape.Fill
                    };
                    (myShape.Content as Grid).Children.Add(ellipseClone);
                    return myShape;
                }
            }
        }

        Color GetRandomColor()
        {
            Color color = new Color();
            Random random = new Random();
            int randomNmber = 0;
            randomNmber = random.Next(0, 255);
            color.A = (byte)randomNmber;
            randomNmber = random.Next(0, 255);
            color.R = (byte)randomNmber;
            randomNmber = random.Next(0, 255);
            color.G = (byte)randomNmber;
            randomNmber = random.Next(0, 255);
            color.B = (byte)randomNmber;
            return color;
        }
        #endregion

        #region 工作表
        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _excel.Sheets.Add(new Worksheet());
        }

        void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (_excel.ActiveSheet != null)
                _excel.Sheets.Remove(_excel.ActiveSheet);
        }
        void btnHide_Click(object sender, RoutedEventArgs e)
        {
            if (_excel.ActiveSheet != null)
                _excel.ActiveSheet.Visible = false;
        }

        void btnUnhide_Click(object sender, RoutedEventArgs e)
        {
            foreach (var sheet in _excel.Sheets)
            {
                sheet.Visible = true;
            }

            if (_excel.ActiveSheetIndex == -1)
            {
                _excel.ActiveSheetIndex = 0;
            }
        }

        void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _excel.Sheets.Clear();
        }
        #endregion

        #region 文件
        async void OpenFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(".xls");
            filePicker.FileTypeFilter.Add(".xlsx");
            filePicker.FileTypeFilter.Add(".xml");
            StorageFile storageFile = await filePicker.PickSingleFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForReadAsync();
                if (storageFile.FileType.ToLower() == ".xml")
                    await _excel.OpenXml(stream);
                else
                    await _excel.OpenExcel(stream, GetOpenFlag());
                stream.Dispose();
            }
        }

        async void SaveExcelFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("Excel Files", new List<string>(new string[] { ".xlsx" }));
            filePicker.FileTypeChoices.Add("Excel 97-2003 Files", new List<string>(new string[] { ".xls" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                var fileName = storageFile.FileType.ToUpperInvariant();
                var fileFormat = ExcelFileFormat.XLS;
                if (fileName.EndsWith(".XLSX"))
                    fileFormat = ExcelFileFormat.XLSX;
                else
                    fileFormat = ExcelFileFormat.XLS;
                await _excel.SaveExcel(stream, fileFormat, GetSaveFlag());
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void SavePDFFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("PDF文件", new List<string>(new string[] { ".pdf" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SavePdf(stream);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void SaveCsvFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("CSV文件", new List<string>(new string[] { ".csv" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SaveCSV(_excel.ActiveSheetIndex, stream, TextFileSaveFlags.AsViewed);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void SaveXmlFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("Xml文件", new List<string>(new string[] { ".xml" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SaveXmlAsync(stream);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        ExcelOpenFlags GetOpenFlag()
        {
            var flagText = (_openFlags.SelectedItem as ComboBoxItem).Content.ToString();
            var result = ExcelOpenFlags.NoFlagsSet;
            Enum.TryParse<ExcelOpenFlags>(flagText, true, out result);
            return result;
        }

        ExcelSaveFlags GetSaveFlag()
        {
            var flagText = (_saveFlags.SelectedItem as ComboBoxItem).Content.ToString();
            var result = ExcelSaveFlags.NoFlagsSet;
            Enum.TryParse<ExcelSaveFlags>(flagText, true, out result);
            return result;
        }
        #endregion
    }
}