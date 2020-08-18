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
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the print view for print page.
    /// Include 3 parts:
    /// - Header(Top: HeaderMargin, Left/Right: Margin)
    /// - Content(Margin)
    /// - Footer(Bottom: FooterMargin, Left/Right: Margin)
    /// </summary>
    internal partial class PrintPagePresenter : Panel
    {
        #region 成员变量
        public static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true };
        internal static readonly Thickness _buildInMargin = new Thickness(1.0);
        ExcelPrinter _printer;
        Excel _excel;
        Worksheet _sheet;
        int _index;

        Border _border;
        UIElement _footerCenterView;
        UIElement _footerLeftView;
        UIElement _footerRightView;
        UIElement _headerCenterView;
        UIElement _headerLeftView;
        UIElement _headerRightView;
        SheetPageInfo _pageInfo;
        #endregion

        public PrintPagePresenter(ExcelPrinter p_printer, int p_index, Excel p_excel, SheetPageInfo p_pageInfo)
        {
            _printer = p_printer;
            _index = p_index + _printer.Info.FirstPageNumber;
            _excel = p_excel;
            _excel.ActiveSheetIndex = _printer.SheetIndex;
            _sheet = _excel.Sheets[_printer.SheetIndex];
            _pageInfo = p_pageInfo;
            OnInit();
        }

        void OnInit()
        {
            AddHeaderFooter();

            _excel.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _excel.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _excel.TabStripVisibility = Visibility.Collapsed;
            _excel.ShowRowRangeGroup = false;
            _excel.ShowColumnRangeGroup = false;

            _sheet.ZoomFactor = 1f;
            ViewportInfo info = new ViewportInfo(_sheet, 1, 1);
            _sheet.SetViewportInfo(info);
            _sheet.ShowGridLine = _printer.Info.ShowGridLine;
            _sheet.RowHeader.IsVisible = ShowRowHeader;
            _sheet.ColumnHeader.IsVisible = ShowColumnHeader;
            _sheet.FrozenRowCount = 0;
            _sheet.FrozenColumnCount = 0;
            _sheet.FrozenTrailingRowCount = 0;
            _sheet.FrozenTrailingColumnCount = 0;
            if (_sheet.RowFilter != null)
                _sheet.RowFilter.ShowFilterButton = false;

            SheetTable[] tables = _sheet.GetTables();
            if (tables != null)
            {
                for (int i = 0; i < tables.Length; i++)
                {
                    tables[i].RowFilter.ShowFilterButton = false;
                }
            }

            if (_printer.Info.BestFitColumns)
            {
                int num2 = (_printer.Info.ColumnStart != -1) ? _printer.Info.ColumnStart : 0;
                int num3 = _printer.Info.UseMax ? _sheet.GetLastDirtyColumn(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data) : (_sheet.ColumnCount - 1);
                int num4 = (_printer.Info.ColumnEnd != -1) ? _printer.Info.ColumnEnd : num3;
                for (int j = num2; j <= num4; j++)
                {
                    _excel.AutoFitColumn(j, false);
                }
                for (int k = 0; k < _sheet.RowHeader.ColumnCount; k++)
                {
                    _excel.AutoFitColumn(k, true);
                }
            }

            if (_printer.Info.BestFitRows)
            {
                int num7 = (_printer.Info.RowStart != -1) ? _printer.Info.RowStart : 0;
                int num8 = _printer.Info.UseMax ? _sheet.GetLastDirtyRow(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data) : (_sheet.RowCount - 1);
                int num9 = (_printer.Info.RowEnd != -1) ? _printer.Info.RowEnd : num8;
                for (int m = num7; m <= num9; m++)
                {
                    _excel.AutoFitRow(m, false);
                }
                for (int n = 0; n < _sheet.ColumnHeader.RowCount; n++)
                {
                    _excel.AutoFitRow(n, true);
                }
            }

            if ((_pageInfo.ColumnPage.RepeatItemStart != -1) && (_pageInfo.ColumnPage.RepeatItemEnd != -1))
            {
                _sheet.FrozenColumnCount = _pageInfo.ColumnPage.RepeatItemEnd + 1;
                for (int num12 = 0; num12 < _pageInfo.ColumnPage.RepeatItemStart; num12++)
                {
                    _sheet.Columns[num12].IsVisible = false;
                }
                _excel.ShowFreezeLine = false;
            }

            if ((_pageInfo.RowPage.RepeatItemStart != -1) && (_pageInfo.RowPage.RepeatItemEnd != -1))
            {
                _sheet.FrozenRowCount = _pageInfo.RowPage.RepeatItemEnd + 1;
                for (int num13 = 0; num13 < _pageInfo.RowPage.RepeatItemStart; num13++)
                {
                    _sheet.Rows[num13].IsVisible = false;
                }
                _excel.ShowFreezeLine = false;
            }

            int itemStart = _pageInfo.ColumnPage.ItemStart;
            int row = _pageInfo.RowPage.ItemStart;
            if (((itemStart >= 0) && (itemStart < _sheet.ColumnCount)) && ((row >= 0) && (row < _sheet.RowCount)))
            {
                _excel.ShowColumn(0, itemStart, HorizontalPosition.Left);
                _excel.ShowRow(0, row, VerticalPosition.Top);
            }
            Children.Add(_excel);

            if (_printer.Info.ShowBorder)
            {
                _border = new Border();
                _border.BorderBrush = new SolidColorBrush(Colors.Black);
                _border.BorderThickness = new Thickness(1.0);
                Children.Add(_border);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            MeasureHeaderAndFooter();
            ScaleTransform transform = new ScaleTransform();
            transform.ScaleX = InnerHorizontalZoomFactor;
            transform.ScaleY = InnerVerticalZoomFactor;
            _excel.RenderTransform = transform;
            Size pageSize = _pageInfo.GetPageSize();
            _excel.Measure(pageSize);
            if (_border != null)
            {
                _border.RenderTransform = transform;
                _border.Measure(new Size(pageSize.Width + 1.0, pageSize.Height + 1.0));
            }
            return _printer.PageSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            ArrangeHeaderAndFooter();
            double printLeftMargin = PrintLeftMargin;
            double printTopMargin = PrintTopMargin;
            Size pageSize = _pageInfo.GetPageSize();
            if ((_printer.Info.Centering == Centering.Horizontal) || (_printer.Info.Centering == Centering.Both))
            {
                double num3 = (_printer.PrintArea.Width - (pageSize.Width * InnerHorizontalZoomFactor)) / 2.0;
                if (num3 < 0.0)
                    num3 = 0.0;
                printLeftMargin += num3;
            }
            if ((_printer.Info.Centering == Centering.Vertical) || (_printer.Info.Centering == Centering.Both))
            {
                double num4 = (_printer.PrintArea.Height - (pageSize.Height * InnerVerticalZoomFactor)) / 2.0;
                if (num4 < 0.0)
                    num4 = 0.0;
                printTopMargin += num4;
            }
            _excel.Arrange(new Rect(new Point(printLeftMargin, printTopMargin), pageSize));
            if (_border != null)
                _border.Arrange(new Rect(new Point(printLeftMargin - 1.0, printTopMargin - 1.0), new Size(pageSize.Width + 1.0, pageSize.Height + 1.0)));
            return _printer.PageSize;
        }

        double InnerHorizontalZoomFactor
        {
            get { return (1 * 0.96); }
        }

        double InnerVerticalZoomFactor
        {
            get { return (1 * 0.96); }
        }

        double PrintLeftMargin
        {
            get { return (_printer.PrintArea.Left + _buildInMargin.Left); }
        }

        double PrintTopMargin
        {
            get { return (_printer.PrintArea.Top + _buildInMargin.Top); }
        }

        bool ShowColumnHeader
        {
            get
            {
                if (_printer.Info.ShowColumnHeader == VisibilityType.Inherit)
                    return _sheet.ColumnHeader.IsVisible;
                return ((_printer.Info.ShowColumnHeader == VisibilityType.Show) || ((_printer.Info.ShowColumnHeader == VisibilityType.ShowOnce) && (_pageInfo.RowPageIndex == 0)));
            }
        }

        bool ShowRowHeader
        {
            get
            {
                if (_printer.Info.ShowRowHeader == VisibilityType.Inherit)
                    return _sheet.RowHeader.IsVisible;
                return ((_printer.Info.ShowRowHeader == VisibilityType.Show) || ((_printer.Info.ShowRowHeader == VisibilityType.ShowOnce) && (_pageInfo.ColumnPageIndex == 0)));
            }
        }

        #region 页眉页脚
        void AddHeaderFooter()
        {
            _headerLeftView = CreateBlock(_printer.Info.HeaderLeft, HorizontalAlignment.Left, _printer.Info.HeaderLeftImage.ToImageSource());
            _headerCenterView = CreateBlock(_printer.Info.HeaderCenter, HorizontalAlignment.Center, _printer.Info.HeaderCenterImage.ToImageSource());
            _headerRightView = CreateBlock(_printer.Info.HeaderRight, HorizontalAlignment.Right, _printer.Info.HeaderRightImage.ToImageSource());
            if (_headerLeftView != null)
                Children.Add(_headerLeftView);
            if (_headerCenterView != null)
                Children.Add(_headerCenterView);
            if (_headerRightView != null)
                Children.Add(_headerRightView);

            _footerLeftView = CreateBlock(_printer.Info.FooterLeft, HorizontalAlignment.Left, _printer.Info.FooterLeftImage.ToImageSource());
            _footerCenterView = CreateBlock(_printer.Info.FooterCenter, HorizontalAlignment.Center, _printer.Info.FooterCenterImage.ToImageSource());
            _footerRightView = CreateBlock(_printer.Info.FooterRight, HorizontalAlignment.Right, _printer.Info.FooterRightImage.ToImageSource());
            if (_footerLeftView != null)
                Children.Add(_footerLeftView);
            if (_footerCenterView != null)
                Children.Add(_footerCenterView);
            if (_footerRightView != null)
                Children.Add(_footerRightView);
        }


        UIElement CreateBlock(string p_xml, HorizontalAlignment p_horAlignment, ImageSource p_img)
        {
            if (string.IsNullOrEmpty(p_xml))
                return null;

            HeaderFooterPanel panel = new HeaderFooterPanel();
            panel.HorizontalAlignment = p_horAlignment;
            panel.VerticalAlignment = VerticalAlignment.Top;

            // <Ts>
            //   <T Text="" Bold="true" Italic="true" Underline="true" Strikeout="true" FontName="" FontSize="" Foreground="" />
            //   <Img Stretch="" Width="" Height="" />
            // </Ts>
            using (StringReader stream = new StringReader(p_xml))
            {
                using (XmlReader reader = XmlReader.Create(stream, _readerSettings))
                {
                    // <Ts>
                    reader.Read();
                    reader.Read();
                    string temp;
                    while (reader.NodeType != XmlNodeType.None)
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Ts")
                            break;

                        if (reader.Name == "Img")
                        {
                            if (p_img != null)
                            {
                                Image img = new Image();
                                temp = reader.GetAttribute("Stretch");
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    Stretch stretch;
                                    if (Enum.TryParse(temp, out stretch))
                                        img.Stretch = stretch;
                                }
                                temp = reader.GetAttribute("Width");
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    double width;
                                    if (double.TryParse(temp, out width))
                                        img.Width = width;
                                }
                                temp = reader.GetAttribute("Height");
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    double height;
                                    if (double.TryParse(temp, out height))
                                        img.Height = height;
                                }
                                img.Source = p_img;
                                panel.Children.Add(img);
                            }
                            reader.Read();
                            continue;
                        }

                        string txt = reader.GetAttribute("Text");
                        if (string.IsNullOrEmpty(txt))
                        {
                            reader.Read();
                            continue;
                        }

                        TextPanel tp = new TextPanel();
                        tp.Text = txt
                            .Replace(":num:", _index.ToString())
                            .Replace(":cnt:", _printer.PageCount.ToString())
                            .Replace(":row:", (_pageInfo.RowPageIndex + 1).ToString())
                            .Replace(":col:", (_pageInfo.ColumnPageIndex + 1).ToString())
                            .Replace(":datetime:", DateTime.Now.ToString())
                            .Replace(":date:", DateTime.Now.ToShortDateString())
                            .Replace(":time:", DateTime.Now.ToShortTimeString())
                            .Replace(":sheetname:", _sheet.Name);

                        temp = reader.GetAttribute("Bold");
                        if (!string.IsNullOrEmpty(temp) && temp.ToLower() == "true")
                            tp.FontWeight = FontWeights.Bold;
                        temp = reader.GetAttribute("Italic");
                        if (!string.IsNullOrEmpty(temp) && temp.ToLower() == "true")
                            tp.FontStyle = FontStyle.Italic;
                        temp = reader.GetAttribute("Underline");
                        if (!string.IsNullOrEmpty(temp) && temp.ToLower() == "true")
                            tp.IsUnderLine = true;
                        temp = reader.GetAttribute("Strikeout");
                        if (!string.IsNullOrEmpty(temp) && temp.ToLower() == "true")
                            tp.IsStrikeThrough = true;
                        temp = reader.GetAttribute("FontName");
                        if (!string.IsNullOrEmpty(temp))
                            tp.FontFamily = new FontFamily(temp);
                        temp = reader.GetAttribute("FontSize");
                        if (!string.IsNullOrEmpty(temp))
                        {
                            double size;
                            if (double.TryParse(temp, out size))
                                tp.FontSize = size;
                        }
                        temp = reader.GetAttribute("Foreground");
                        if (!string.IsNullOrEmpty(temp) && temp.Length == 7 && temp.StartsWith("#"))
                        {
                            byte r, g, b;
                            if (byte.TryParse(temp.Substring(1, 2), NumberStyles.AllowHexSpecifier, null, out r)
                                && byte.TryParse(temp.Substring(3, 2), NumberStyles.AllowHexSpecifier, null, out g)
                                && byte.TryParse(temp.Substring(5, 2), NumberStyles.AllowHexSpecifier, null, out b))
                                tp.Foreground = new SolidColorBrush(Color.FromArgb(0xff, r, g, b));
                        }
                        panel.Children.Add(tp);
                        reader.Read();
                    }
                }
            }
            return panel;
        }

        void MeasureHeaderAndFooter()
        {
            ScaleTransform transform = new ScaleTransform();
            transform.ScaleX = InnerHorizontalZoomFactor;
            transform.ScaleY = InnerVerticalZoomFactor;
            Size size = new Size(_printer.PrintArea.Width, _printer.PageSize.Height);
            if (_headerLeftView != null)
            {
                _headerLeftView.RenderTransform = transform;
                _headerLeftView.Measure(size);
            }
            if (_headerCenterView != null)
            {
                _headerCenterView.RenderTransform = transform;
                _headerCenterView.Measure(size);
            }
            if (_headerRightView != null)
            {
                _headerRightView.RenderTransform = transform;
                _headerRightView.Measure(size);
            }
            if (_footerLeftView != null)
            {
                _footerLeftView.RenderTransform = transform;
                _footerLeftView.Measure(size);
            }
            if (_footerCenterView != null)
            {
                _footerCenterView.RenderTransform = transform;
                _footerCenterView.Measure(size);
            }
            if (_footerRightView != null)
            {
                _footerRightView.RenderTransform = transform;
                _footerRightView.Measure(size);
            }
        }

        void ArrangeHeaderAndFooter()
        {
            double y = _printer.HeaderMargin + _buildInMargin.Top;
            new RotateTransform().Angle = 90.0;
            if (_headerLeftView != null)
            {
                _headerLeftView.Arrange(new Rect(new Point(PrintLeftMargin, y), new Size(_printer.PrintArea.Width / 3.0, _headerLeftView.DesiredSize.Height)));
            }
            if (_headerCenterView != null)
            {
                _headerCenterView.Arrange(new Rect(new Point(PrintLeftMargin + (_printer.PrintArea.Width / 3.0), y), new Size(_printer.PrintArea.Width / 3.0, _headerCenterView.DesiredSize.Height)));
            }
            if (_headerRightView != null)
            {
                _headerRightView.Arrange(new Rect(new Point(PrintLeftMargin + ((_printer.PrintArea.Width * 2.0) / 3.0), y), new Size(_printer.PrintArea.Width / 3.0, _headerRightView.DesiredSize.Height)));
            }
            double num2 = (_printer.PageSize.Height - _printer.FooterMargin) - _buildInMargin.Bottom;
            if (_footerLeftView != null)
            {
                _footerLeftView.Arrange(new Rect(new Point(PrintLeftMargin, num2 - _footerLeftView.DesiredSize.Height), new Size(_printer.PrintArea.Width / 3.0, _footerLeftView.DesiredSize.Height)));
            }
            if (_footerCenterView != null)
            {
                _footerCenterView.Arrange(new Rect(new Point(PrintLeftMargin + (_printer.PrintArea.Width / 3.0), num2 - _footerCenterView.DesiredSize.Height), new Size(_printer.PrintArea.Width / 3.0, _footerCenterView.DesiredSize.Height)));
            }
            if (_footerRightView != null)
            {
                _footerRightView.Arrange(new Rect(new Point(PrintLeftMargin + ((_printer.PrintArea.Width * 2.0) / 3.0), num2 - _footerRightView.DesiredSize.Height), new Size(_printer.PrintArea.Width / 3.0, _footerRightView.DesiredSize.Height)));
            }
        }
        #endregion
    }
}

