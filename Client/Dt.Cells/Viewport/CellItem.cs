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
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 单元格面板
    /// </summary>
    internal partial class CellItem : Panel
    {
        static Rect _rcEmpty = new Rect();
        TextBlock _tb;
        CellOverflowLayout _overflowLayout;
        Rect? _cachedClip;

        ConditionalFormatView _conditionalView;
        CustomDrawingObject _customDrawingObject;
        FrameworkElement _customDrawingObjectView;
        DataBarDrawingObject _dataBarObject;
        IconDrawingObject _iconObject;
        Sparkline _sparkInfo;
        BaseSparklineView _sparklineView;
        StrikethroughView _strikethroughView;
        FilterButton _filterButton;
        FilterButtonInfo _filterButtonInfo;
        InvalidDataPresenterInfo _dataValidationInvalidPresenterInfo;
        bool _lastUnderline;

        public CellItem(RowItem p_rowItem)
        {
            OwnRow = p_rowItem;
        }

        public RowItem OwnRow { get; }

        public int Row
        {
            get { return OwnRow.Row; }
        }

        public int Column { get; set; }

        public CellLayout CellLayout { get; set; }

        public Cell BindingCell { get; private set; }

        public Excel Excel
        {
            get { return OwnRow.OwnPanel.Excel; }
        }

        public double ZoomFactor
        {
            get { return (double)OwnRow.OwnPanel.Excel.ZoomFactor; }
        }

        public CellOverflowLayout CellOverflowLayout
        {
            get { return _overflowLayout; }
            set
            {
                if (!object.Equals(_overflowLayout, value))
                {
                    _overflowLayout = value;
                    InvalidateMeasure();
                }
            }
        }

        public FilterButtonInfo FilterButtonInfo
        {
            get { return _filterButtonInfo; }
            set
            {
                if (_filterButtonInfo != value)
                {
                    _filterButtonInfo = value;
                    InvalidateMeasure();
                }
            }
        }

        #region 外部方法
        public void ApplyState()
        {
            Background = BindingCell.ActualBackground;
            if (_filterButton != null)
                _filterButton.ApplyState();
        }

        public void CleanUpBeforeDiscard()
        {
            if (_customDrawingObjectView != null)
                Children.Remove(_customDrawingObjectView);

            if (_dataValidationInvalidPresenterInfo != null)
            {
                OwnRow.OwnPanel.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                _dataValidationInvalidPresenterInfo = null;
            }

            DettachSparklineEvents();
        }

        public void Refresh()
        {
            if (_sparklineView != null)
                UpdateSparkline();
            UpdateChildren();
            InvalidateMeasure();
            InvalidateArrange();
        }
        #endregion

        #region 测量布局
        //*** CellsPanel.Measure -> RowsLayer.Measure -> RowItem.UpdateChildren -> 行列改变时 CellItem.UpdateChildren -> RowItem.Measure -> CellItem.Measure ***//

        public void UpdateChildren()
        {
            // 刷新绑定的Cell
            int row = OwnRow.Row;
            int column = Column;
            if (CellLayout != null)
            {
                row = CellLayout.Row;
                column = CellLayout.Column;
            }
            BindingCell = OwnRow.OwnPanel.CellCache.GetCachedCell(row, column);

            var excel = Excel;
            if (excel == null || BindingCell == null)
                return;

            Worksheet sheet = BindingCell.Worksheet;

            // 迷你图
            Sparkline sparkline = sheet.GetSparkline(row, column);
            if (_sparkInfo != sparkline)
            {
                SparkLine = sparkline;
                SynSparklineView();
            }

            // 收集所有DrawingObject
            List<DrawingObject> list = new List<DrawingObject>();
            DrawingObject[] objArray = sheet.GetDrawingObject(row, column, 1, 1);
            if ((objArray != null) && (objArray.Length > 0))
            {
                list.AddRange(objArray);
            }

            IDrawingObjectProvider drawingObjectProvider = DrawingObjectManager.GetDrawingObjectProvider(excel);
            if (drawingObjectProvider != null)
            {
                DrawingObject[] objArray2 = drawingObjectProvider.GetDrawingObjects(sheet, row, column, 1, 1);
                if ((objArray2 != null) && (objArray2.Length > 0))
                {
                    list.AddRange(objArray2);
                }
            }

            _dataBarObject = null;
            _iconObject = null;
            _customDrawingObject = null;
            if (list.Count > 0)
            {
                foreach (DrawingObject obj in list)
                {
                    if (obj is DataBarDrawingObject bar)
                    {
                        _dataBarObject = bar;
                    }
                    else if (obj is IconDrawingObject icon)
                    {
                        _iconObject = icon;
                    }
                    else if (obj is CustomDrawingObject cust)
                    {
                        _customDrawingObject = cust;
                    }
                }
            }

            bool noBarIcon = SynContitionalView();
            bool noCust = SynCustomDrawingObjectView();

            if (sparkline == null && noBarIcon && noCust && !string.IsNullOrEmpty(BindingCell.Text))
            {
                if (_tb == null)
                {
                    _tb = new TextBlock { VerticalAlignment = VerticalAlignment.Center };
                    Children.Add(_tb);
                }
                _tb.Text = BindingCell.Text;
                ApplyStyle();
            }
            else if (_tb != null)
            {
                Children.Remove(_tb);
                _tb = null;
            }
            SynStrikethroughView();

            FilterButtonInfo info = excel.GetFilterButtonInfo(row, column, BindingCell.SheetArea);
            if (info != FilterButtonInfo)
            {
                FilterButtonInfo = info;
                SynFilterButton();
            }

            if (OwnRow.OwnPanel.Excel.HighlightInvalidData)
            {
                if (_dataValidationInvalidPresenterInfo == null)
                {
                    DataValidator actualDataValidator = BindingCell.ActualDataValidator;
                    if ((actualDataValidator != null) && !actualDataValidator.IsValid(excel.ActiveSheet, Row, Column, BindingCell.Value))
                    {
                        InvalidDataPresenterInfo info2 = new InvalidDataPresenterInfo
                        {
                            Row = Row,
                            Column = Column
                        };
                        _dataValidationInvalidPresenterInfo = info2;
                        OwnRow.OwnPanel.AddDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                    }
                }
                else if (_dataValidationInvalidPresenterInfo != null)
                {
                    DataValidator validator2 = BindingCell.ActualDataValidator;
                    if ((validator2 == null) || validator2.IsValid(excel.ActiveSheet, Row, Column, BindingCell.Value))
                    {
                        OwnRow.OwnPanel.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                        _dataValidationInvalidPresenterInfo = null;
                    }
                }
            }
            else if (_dataValidationInvalidPresenterInfo != null)
            {
                OwnRow.OwnPanel.RemoveDataValidationInvalidDataPresenterInfo(_dataValidationInvalidPresenterInfo);
                _dataValidationInvalidPresenterInfo = null;
            }

            ApplyState();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Column == -1
                || availableSize.Width == 0.0
                || availableSize.Height == 0.0)
                return new Size();

            Size sizeOverflow = availableSize;
            if (_overflowLayout != null && _overflowLayout.ContentWidth > availableSize.Width)
                sizeOverflow = new Size(_overflowLayout.ContentWidth, availableSize.Height);

            foreach (UIElement element in Children)
            {
                if (element is TextBlock)
                {
                    element.Measure(sizeOverflow);
                }
                else
                {
                    element.Measure(availableSize);
                }
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Column == -1 || finalSize.Width == 0 || finalSize.Height == 0)
            {
                if (Children.Count > 0)
                {
                    foreach (UIElement elem in Children)
                    {
                        elem.Arrange(_rcEmpty);
                    }
                }
                return finalSize;
            }

            double width = finalSize.Width;
            double height = finalSize.Height;
            Rect? nullable = null;
            double left = 0;
            double top = 0;
            Rect rect = new Rect(left, top, width, height);
            Rect rectOverflow = rect;
            if (_overflowLayout != null && _overflowLayout.ContentWidth > width)
            {
                switch (BindingCell.ToHorizontalAlignment())
                {
                    case HorizontalAlignment.Left:
                        if (CellOverflowLayout != null)
                        {
                            double w = CellOverflowLayout.RightBackgroundWidth;
                            if (w >= 0.0)
                                nullable = new Rect(0.0, 0.0, w, finalSize.Height);
                        }
                        break;

                    case HorizontalAlignment.Right:
                        left -= _overflowLayout.ContentWidth - width;
                        if (CellOverflowLayout != null)
                        {
                            double x = finalSize.Width - CellOverflowLayout.LeftBackgroundWidth;
                            double w = CellOverflowLayout.LeftBackgroundWidth;
                            if (w >= 0.0)
                                nullable = new Rect(x, 0.0, w, finalSize.Height);
                        }
                        break;

                    default:
                        left -= (_overflowLayout.ContentWidth - width) / 2.0;
                        if (CellOverflowLayout != null)
                        {
                            double x = 0.0;
                            if (CellOverflowLayout.LeftBackgroundWidth > 0.0)
                                x = (finalSize.Width / 2.0) - CellOverflowLayout.LeftBackgroundWidth;

                            double w = CellOverflowLayout.BackgroundWidth;
                            if (w >= 0.0)
                                nullable = new Rect(x, 0.0, w, finalSize.Height);
                        }
                        break;
                }
                width = _overflowLayout.ContentWidth;
                rectOverflow = new Rect(left, top, width, height);
            }

            if ((_cachedClip.HasValue != nullable.HasValue) || (_cachedClip.HasValue && (_cachedClip.Value != nullable.Value)))
            {
                _cachedClip = nullable;
                if (nullable.HasValue)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = nullable.Value;
                    Clip = geometry;
                }
                else
                {
                    ClearValue(ClipProperty);
                }
            }

            foreach (UIElement element in Children)
            {
                if (element != null)
                {
                    if (element is TextBlock)
                    {
                        element.Arrange(rectOverflow);
                    }
                    else
                    {
                        element.Arrange(rect);
                    }
                }
            }
            return finalSize;
        }
        #endregion

        #region 更新可视树
        bool SynContitionalView()
        {
            bool isContentVisible = true;
            if ((_dataBarObject != null) || (_iconObject != null))
            {
                if (_conditionalView == null)
                {
                    _conditionalView = new ConditionalFormatView(BindingCell);
                    Children.Add(_conditionalView);
                    Canvas.SetZIndex(_conditionalView, 500);
                }
                _conditionalView.SetDataBarObject(_dataBarObject);
                if (_iconObject != null)
                {
                    _conditionalView.SetImageContainer();
                    _conditionalView.SetIconObject(_iconObject, Excel.ZoomFactor, BindingCell);
                }
                bool flag = true;
                if (flag && (_dataBarObject != null))
                {
                    flag = !_dataBarObject.ShowBarOnly;
                }
                if (flag && (_iconObject != null))
                {
                    flag = !_iconObject.ShowIconOnly;
                }
                isContentVisible = flag;
            }
            else if (_conditionalView != null)
            {
                Children.Remove(_conditionalView);
                _conditionalView = null;
            }
            return isContentVisible;
        }

        void SynStrikethroughView()
        {
            bool actualStrikethrough = BindingCell.ActualStrikethrough;
            if (_strikethroughView != null)
            {
                Children.Remove(_strikethroughView);
                _strikethroughView = null;
            }
            if (actualStrikethrough && (_strikethroughView == null))
            {
                _strikethroughView = new StrikethroughView(BindingCell, this);
                _strikethroughView.SetLines(Excel.ZoomFactor, BindingCell);
                Children.Add(_strikethroughView);
            }
        }

        bool SynCustomDrawingObjectView()
        {
            bool isContentVisible = true;
            if (_customDrawingObject != null)
            {
                isContentVisible = !_customDrawingObject.ShowDrawingObjectOnly;
                FrameworkElement rootElement = _customDrawingObject.RootElement;
                if (_customDrawingObjectView != rootElement)
                {
                    if (_customDrawingObjectView != null)
                    {
                        Children.Remove(_customDrawingObjectView);
                    }
                    _customDrawingObjectView = rootElement;
                    if (_customDrawingObjectView != null)
                    {
                        Panel parent = _customDrawingObjectView.Parent as Panel;
                        if ((parent != null) && (parent != this))
                        {
                            parent.Children.Remove(_customDrawingObjectView);
                        }
                        if (!Children.Contains(_customDrawingObjectView))
                        {
                            Children.Add(_customDrawingObjectView);
                        }
                    }
                }
            }
            else if (_customDrawingObjectView != null)
            {
                Children.Remove(_customDrawingObjectView);
                _customDrawingObjectView = null;
            }
            return isContentVisible;
        }

        void SynFilterButton()
        {
            if (_filterButtonInfo != null)
            {
                if (_filterButton == null)
                {
                    FilterButton element = new FilterButton(this);
                    element.HorizontalAlignment = HorizontalAlignment.Right;
                    element.VerticalAlignment = VerticalAlignment.Bottom;
                    element.Area = SheetArea.Cells;
                    _filterButton = element;
                    Canvas.SetZIndex(element, 0xbb8);
                    Children.Add(element);
                }
                else
                {
                    _filterButton.ApplyState();
                }
            }
            else if (_filterButton != null)
            {
                Children.Remove(_filterButton);
                _filterButton = null;
            }
        }
        #endregion

        #region 迷你图
        Sparkline SparkLine
        {
            get { return _sparkInfo; }
            set
            {
                if (_sparkInfo != value)
                {
                    DettachSparklineEvents();
                    if (_sparklineView != null)
                    {
                        Children.Remove(_sparklineView);
                        _sparklineView = null;
                    }
                    _sparkInfo = value;
                    AttachSparklineEvents();
                }
            }
        }

        void AttachSparklineEvents()
        {
            if (_sparkInfo != null)
                _sparkInfo.SparklineChanged += new EventHandler(sparkline_SparklineChanged);
        }

        void DettachSparklineEvents()
        {
            if (_sparkInfo != null)
                _sparkInfo.SparklineChanged -= new EventHandler(sparkline_SparklineChanged);
        }

        void sparkline_SparklineChanged(object sender, EventArgs e)
        {
            Sparkline sparkline = sender as Sparkline;
            if ((_sparklineView == null) || (_sparklineView.SparklineType != sparkline.SparklineType))
            {
                if (_sparklineView != null)
                {
                    Children.Remove(_sparklineView);
                    _sparklineView = null;
                }
                SynSparklineView();
            }
            else
            {
                UpdateSparkline();
            }
        }

        void UpdateSparkline()
        {
            if (Excel != null && _sparklineView != null)
            {
                _sparklineView.Update(new Size(ActualWidth, ActualHeight), (double)Excel.ZoomFactor);
            }
        }

        void SynSparklineView()
        {
            var excel = Excel;
            if (excel == null)
                return;

            if (_sparkInfo != null)
            {
                if (_sparklineView == null)
                {
                    _sparklineView = CreateSparkline(_sparkInfo);
                    _sparklineView.ZoomFactor = OwnRow.OwnPanel.Excel.ZoomFactor;
                    ((IThemeContextSupport)_sparklineView).SetContext(excel.ActiveSheet);
                    Canvas.SetZIndex(_sparklineView, 0x3e8);
                    Children.Add(_sparklineView);
                    _sparklineView.Update(new Size(ActualWidth, ActualHeight), (double)excel.ZoomFactor);
                }
            }
            else if (_sparklineView != null)
            {
                DettachSparklineEvents();
                Children.Remove(_sparklineView);
                _sparklineView = null;
            }
        }

        BaseSparklineView CreateSparkline(Sparkline info)
        {
            if (info.SparklineType == SparklineType.Column)
            {
                return new ColumnSparklineView(new ColumnSparklineViewInfo(info));
            }
            if (info.SparklineType == SparklineType.Line)
            {
                return new LineSparklineView(new LineSparklineViewInfo(info));
            }
            return new WinLossSparklineView(new WinLossSparklineViewInfo(info));
        }
        #endregion

        #region 内部方法
        void ApplyStyle()
        {
            HorizontalAlignment horAlignment = BindingCell.ToHorizontalAlignment();
            if (_tb.HorizontalAlignment != horAlignment)
                _tb.HorizontalAlignment = horAlignment;

            Windows.UI.Xaml.TextAlignment textAlignment;
            switch (horAlignment)
            {
                case HorizontalAlignment.Center:
                    textAlignment = Windows.UI.Xaml.TextAlignment.Center;
                    break;
                case HorizontalAlignment.Right:
                    textAlignment = Windows.UI.Xaml.TextAlignment.Right;
                    break;
                default:
                    textAlignment = Windows.UI.Xaml.TextAlignment.Left;
                    break;
            }
            if (_tb.TextAlignment != textAlignment)
                _tb.TextAlignment = textAlignment;

            VerticalAlignment verAlignment;
            switch (BindingCell.ActualVerticalAlignment)
            {
                case CellVerticalAlignment.Top:
                    verAlignment = VerticalAlignment.Top;
                    break;
                case CellVerticalAlignment.Bottom:
                    verAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    verAlignment = VerticalAlignment.Center;
                    break;
            }
            if (_tb.VerticalAlignment != verAlignment)
                _tb.VerticalAlignment = verAlignment;

            var foreground = BindingCell.ActualForeground;
            if (foreground != null && foreground != _tb.Foreground)
                _tb.Foreground = foreground;

            var fontStyle = BindingCell.ActualFontStyle;
            if (_tb.FontStyle != fontStyle)
                _tb.FontStyle = fontStyle;

            var fontWeight = BindingCell.ActualFontWeight;
            if (_tb.FontWeight.Weight != fontWeight.Weight)
                _tb.FontWeight = fontWeight;

            var fontStretch = BindingCell.ActualFontStretch;
            if (_tb.FontStretch != fontStretch)
                _tb.FontStretch = fontStretch;

            var fontFamily = BindingCell.ActualFontFamily;
            if (fontFamily != null && _tb.FontFamily.Source != fontFamily.Source)
                _tb.FontFamily = fontFamily;

            bool wrap = BindingCell.ActualWordWrap;
            TextWrapping textWrap = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            if (_tb.TextWrapping != textWrap)
                _tb.TextWrapping = textWrap;

            double fontSize = BindingCell.ActualFontSize * ZoomFactor;
            double fitZoom = -1;
            if (!wrap && BindingCell.ActualShrinkToFit)
            {
                // 自动缩小字体适应单元格宽度
                double textWidth = MeasureHelper.MeasureText(
                    _tb.Text,
                    _tb.FontFamily,
                    fontSize,
                    _tb.FontStretch,
                    _tb.FontStyle,
                    _tb.FontWeight,
                    new Size(double.PositiveInfinity, double.PositiveInfinity),
                    false,
                    null,
                    _tb.UseLayoutRounding,
                    ZoomFactor).Width;
                double cellWidth = BindingCell.Worksheet.GetActualColumnWidth(BindingCell.Column.Index, BindingCell.ColumnSpan, BindingCell.SheetArea) * ZoomFactor;
                cellWidth = MeasureHelper.ConvertExcelCellSizeToTextSize(new Size(cellWidth, double.PositiveInfinity), ZoomFactor).Width;
                cellWidth = Math.Max((double)0.0, (double)(cellWidth - BindingCell.ActualTextIndent * ZoomFactor));
                if (cellWidth < textWidth)
                    fitZoom = cellWidth / textWidth;
            }
            if (fitZoom > 0)
                fontSize *= fitZoom;
            if (_tb.FontSize != fontSize)
                _tb.FontSize = fontSize;

            var padding = MeasureHelper.TextBlockDefaultMargin;
            var indent = BindingCell.ActualTextIndent * ZoomFactor;
            if (indent > 0 && _tb.TextAlignment != Windows.UI.Xaml.TextAlignment.Center)
            {
                if (_tb.TextAlignment == Windows.UI.Xaml.TextAlignment.Left)
                    padding.Left += indent;
                else if (_tb.TextAlignment == Windows.UI.Xaml.TextAlignment.Right)
                    padding.Right += indent;
            }
            if (_tb.Padding != padding)
                _tb.Padding = padding;

            if (BindingCell.ActualUnderline)
            {
                Underline underline = new Underline();
                Run run = new Run();
                run.Text = _tb.Text;
                underline.Inlines.Add(run);
                _tb.Inlines.Clear();
                _tb.Inlines.Add(underline);
                _lastUnderline = true;
            }
            else if (_lastUnderline)
            {
                string str = _tb.Text;
                _tb.Inlines.Clear();
                _tb.Text = str;
            }

            if (BindingCell.ActualStrikethrough)
            {
                foreach (UIElement element in (_tb.Parent as Panel).Children)
                {
                    if (element is StrikethroughView)
                    {
                        StrikethroughView view = element as StrikethroughView;
                        if (view.LineContainer != null)
                        {
                            foreach (var line in view.LineContainer.Children.OfType<Line>())
                            {
                                line.Stroke = _tb.Foreground;
                            }
                        }
                        break;
                    }
                }
            }
        }

        #endregion
    }
}

