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
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Dt.Sample
{
    public partial class FloatingObject : Win
    {
        int ellipseCount = 1;
        int rectangleCount = 1;

        MyShape myRectangle;
        public FloatingObject()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                Color color = Colors.Red;
                color.A = (byte)((1 - 0.8) * 255);
                AddEllipse(new Point(150, 100), color);
                color = Colors.Green;
                color.A = (byte)((1 - 0.8) * 255);
                AddRectangle(new Point(500, 100), color);
            }
            _excel.SelectionChanging += OnSelectionChaging;
        }

        void AddRectangleButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Point point = new Point(0, 0);
            AddRectangle(point, GetRandomColor());
        }

        void AddEllipseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Point point = new Point(0, 0);
            AddEllipse(point, GetRandomColor());
        }

        void testButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

            if (myRectangle != null)
            {
                sheet.FloatingObjects.Remove(myRectangle);
                myRectangle = null;
            }
            myRectangle = new MyShape("selRec", X, Y, width, height);
            Rectangle rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.Transparent),
                StrokeThickness = 3,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            (myRectangle.Content as Grid).Children.Add(rectangle);
            sheet.FloatingObjects.Add(myRectangle);
        }

        void selectButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            if (sheet.Selections.Count == 0)
                return;

            CellRange cr = sheet.Selections[0];

            _excel.SuspendEvent();
            sheet.SelectionPolicy = SelectionPolicy.MultiRange;
            sheet.SelectionUnit = SelectionUnit.Cell;

            sheet.AddSelection(cr.Row, cr.Column, 1, 1);
            sheet.AddSelection(cr);
            sheet.SelectionPolicy = SelectionPolicy.Range;

            sheet.SelectionBackground = new SolidColorBrush(Colors.Teal);
            sheet.SelectionBorderColor = Colors.Red;
            _excel.ResumeEvent();
        }

        void lineButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

            if (myRectangle != null)
            {
                sheet.FloatingObjects.Remove(myRectangle);
                myRectangle = null;
            }
            myRectangle = new MyShape("selRec", X, 0, 3, 500);
            Rectangle rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.Transparent),
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            (myRectangle.Content as Grid).Children.Add(rectangle);
            sheet.FloatingObjects.Add(myRectangle);
            myRectangle.CanPrint = false;
            myRectangle.DynamicMove = false;
            myRectangle.DynamicSize = false;
        }

        void resetSelectionStyle()
        {
            Worksheet sheet = _excel.ActiveSheet;

            sheet.SelectionBackground = new SolidColorBrush(Colors.Transparent);
            sheet.SelectionBorderColor = Colors.Black;
        }

        void OnSelectionChaging(object sender, EventArgs e)
        {
            resetSelectionStyle();
        }
        void AddRectangle(Point point, Color fillColor)
        {
            MyShape myShape = new MyShape("Rectuangle" + rectangleCount, point.X, point.Y, 200, 200);
            Rectangle rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(fillColor),
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            (myShape.Content as Grid).Children.Add(rectangle);
            _excel.ActiveSheet.FloatingObjects.Add(myShape);
            rectangleCount++;
        }


        void AddEllipse(Point point, Color fillColor)
        {
            MyShape myShape = new MyShape("Ellipse" + ellipseCount, point.X, point.Y, 200, 200);
            Ellipse ellipse = new Ellipse()
            {
                Fill = new SolidColorBrush(fillColor),
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            (myShape.Content as Grid).Children.Add(ellipse);
            _excel.ActiveSheet.FloatingObjects.Add(myShape);
            ellipseCount++;
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

        ExcelSaveFlags GetSaveFlag()
        {
            var flagText = (_saveFlags.SelectedItem as ComboBoxItem).Content.ToString();
            var result = ExcelSaveFlags.NoFlagsSet;
            Enum.TryParse<ExcelSaveFlags>(flagText, true, out result);
            return result;
        }
    }
}