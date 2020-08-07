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
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents an individual <see cref="T:GcSpreadSheet" /> cell.
    /// </summary>
    public partial class CellPresenter : CellPresenterBase
    {
        ConditionalFormatView _conditionalView;
        CustomDrawingObject _customDrawingObject;
        FrameworkElement _customDrawingObjectView;
        DataBarDrawingObject _dataBarObject;
        IconDrawingObject _iconObject;
        Sparkline _sparkInfo;
        BaseSparklineView _sparklineView;
        StrikethroughView _strikethroughView;

        public CellPresenter()
        {
            DefaultStyleKey = typeof(CellPresenter);
        }

        internal override void CleanUpBeforeDiscard()
        {
            base.CleanUpBeforeDiscard();
            if (_customDrawingObjectView != null)
            {
                _rootPanel.Children.Remove(_customDrawingObjectView);
            }
        }

        internal override void DetachEvents()
        {
            base.DetachEvents();
            DettachSparklineEvents();
        }

        internal override void Invalidate()
        {
            if (_sparklineView != null)
            {
                UpdateSparkline();
            }
            base.Invalidate();
        }

        #region 更新可视树
        internal override bool TryUpdateVisualTree()
        {
            Cell bindingCell = BindingCell;
            if (bindingCell == null)
                return false;

            Worksheet sheet = bindingCell.Worksheet;
            int row = Row;
            int column = Column;
            if (CellLayout != null)
            {
                row = CellLayout.Row;
                column = CellLayout.Column;
            }

            bool update = false;
            Sparkline sparkline = sheet.GetSparkline(row, column);
            if (!object.Equals(_sparkInfo, sparkline))
            {
                SparkLine = sparkline;
                SynSparklineView();
                update = true;
            }

            List<DrawingObject> list = new List<DrawingObject>();
            DrawingObject[] objArray = sheet.GetDrawingObject(row, column, 1, 1);
            if ((objArray != null) && (objArray.Length > 0))
            {
                list.AddRange(objArray);
            }

            if (SheetView != null && SheetView._host is Excel excel)
            {
                IDrawingObjectProvider drawingObjectProvider = DrawingObjectManager.GetDrawingObjectProvider(excel);
                if (drawingObjectProvider != null)
                {
                    DrawingObject[] objArray2 = drawingObjectProvider.GetDrawingObjects(sheet, row, column, 1, 1);
                    if ((objArray2 != null) && (objArray2.Length > 0))
                    {
                        list.AddRange(objArray2);
                    }
                }
            }

            _dataBarObject = null;
            _iconObject = null;
            _customDrawingObject = null;
            if ((list != null) && (list.Count > 0))
            {
                foreach (DrawingObject obj in list)
                {
                    if (obj is DataBarDrawingObject bar)
                    {
                        _dataBarObject = bar;
                        update = true;
                    }
                    else if (obj is IconDrawingObject icon)
                    {
                        _iconObject = icon;
                        update = true;
                    }
                    else if (obj is CustomDrawingObject cust)
                    {
                        _customDrawingObject = cust;
                        update = true;
                    }
                }
            }

            bool noBarIcon = SynContitionalView();
            bool noCust = SynCustomDrawingObjectView();
            ShowContent = noBarIcon && noCust;
            SynStrikethroughView();

            bool baseUpdate = base.TryUpdateVisualTree();
            return update || baseUpdate;
        }

        bool SynContitionalView()
        {
            bool isContentVisible = true;
            if ((_dataBarObject != null) || (_iconObject != null))
            {
                if (_conditionalView == null)
                {
                    _conditionalView = new ConditionalFormatView(BindingCell);
                    _rootPanel.Children.Add(_conditionalView);
                    Canvas.SetZIndex(_conditionalView, 500);
                }
                _conditionalView.SetDataBarObject(_dataBarObject);
                if (_iconObject != null)
                {
                    _conditionalView.SetImageContainer();
                    _conditionalView.SetIconObject(_iconObject, SheetView.ZoomFactor, BindingCell);
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
            else
            {
                if (_conditionalView != null)
                {
                    _rootPanel.Children.Remove(_conditionalView);
                    _conditionalView = null;
                }
            }
            return isContentVisible;
        }

        void SynStrikethroughView()
        {
            bool actualStrikethrough = BindingCell.ActualStrikethrough;
            if (_strikethroughView != null)
            {
                _rootPanel.Children.Remove(_strikethroughView);
                _strikethroughView = null;
            }
            if (actualStrikethrough && (_strikethroughView == null))
            {
                _strikethroughView = new StrikethroughView(BindingCell, _rootPanel);
                _strikethroughView.SetLines(SheetView.ZoomFactor, BindingCell);
                _rootPanel.Children.Add(_strikethroughView);
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
                        _rootPanel.Children.Remove(_customDrawingObjectView);
                    }
                    _customDrawingObjectView = rootElement;
                    if (_customDrawingObjectView != null)
                    {
                        Panel parent = _customDrawingObjectView.Parent as Panel;
                        if ((parent != null) && (parent != _rootPanel))
                        {
                            parent.Children.Remove(_customDrawingObjectView);
                        }
                        if (!_rootPanel.Children.Contains(_customDrawingObjectView))
                        {
                            _rootPanel.Children.Add(_customDrawingObjectView);
                        }
                    }
                }
            }
            else if (_customDrawingObjectView != null)
            {
                _rootPanel.Children.Remove(_customDrawingObjectView);
                _customDrawingObjectView = null;
            }
            return isContentVisible;
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
                        _rootPanel.Children.Remove(_sparklineView);
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
                    _rootPanel.Children.Remove(_sparklineView);
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
            if (SheetView != null && _sparklineView != null)
            {
                _sparklineView.Update(new Size(ActualWidth, ActualHeight), (double)SheetView.ZoomFactor);
            }
        }

        void SynSparklineView()
        {
            SheetView sheetView = SheetView;
            if (sheetView == null)
                return;

            if (_sparkInfo != null)
            {
                if (_sparklineView == null)
                {
                    _sparklineView = CreateSparkline(_sparkInfo);
                    _sparklineView.ZoomFactor = OwningRow.OwningPresenter.Sheet.ZoomFactor;
                    ((IThemeContextSupport)_sparklineView).SetContext(sheetView.Worksheet);
                    Canvas.SetZIndex(_sparklineView, 0x3e8);
                    _rootPanel.Children.Add(_sparklineView);
                    _sparklineView.Update(new Size(ActualWidth, ActualHeight), (double)sheetView.ZoomFactor);
                }
            }
            else if (_sparklineView != null)
            {
                DettachSparklineEvents();
                _rootPanel.Children.Remove(_sparklineView);
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

        /// <summary>
        /// Gets a value that indicates that the cell's viewport is active. 
        /// </summary>
        protected override bool IsActive
        {
            get
            {
                SheetView sheet = OwningRow.OwningPresenter.Sheet;
                return ((sheet.GetActiveColumnViewportIndex() == OwningRow.OwningPresenter.ColumnViewportIndex) && (sheet.GetActiveRowViewportIndex() == OwningRow.OwningPresenter.RowViewportIndex));
            }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is the active cell.
        /// </summary>
        protected override bool IsCurrent
        {
            get
            {
                Worksheet worksheet = OwningRow.OwningPresenter.Sheet.Worksheet;
                return ((worksheet.ActiveRowIndex == Row) && (worksheet.ActiveColumnIndex == Column));
            }
        }

        internal override bool IsRecylable
        {
            get { return ((_customDrawingObject == null) && IsRecylable); }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is selected.
        /// </summary>
        protected override bool IsSelected
        {
            get { return OwningRow.OwningPresenter.Sheet.Worksheet.IsSelected(Row, Column); }
        }

    }
    internal partial class ConditionalFormatView : Panel
    {
        Canvas _axisCanvas;
        Line _axisLine;
        Cell _bindingCell;
        double _cachedAxisPosition;
        Windows.UI.Color _cachedFillColor;
        GradientStop _cachedGradientEnd;
        GradientStop _cachedGradientStart;
        GradientStop _cachedGradientTransparentEnd;
        Image _cachedImage;
        double _cachedScale;
        float _cachedZoomFactor;
        LinearGradientBrush _dataBarBackground;
        DataBarDrawingObject _databarObject;
        Rectangle _dataBarRectangle;
        IconDrawingObject _iconObject;
        Border _imageContainer;
        const int AxisWidth = 1;
        public const int DatabarZIndex = 100;
        const int DefaultIcontHeight = 0x10;
        const int DefaultIconWidth = 0x10;
        public const int IconSetZIndex = 200;
        const int ViewMargin = 1;

        public ConditionalFormatView(Cell bindingCell)
        {
            Margin = new Thickness(1.0);
            UseLayoutRounding = true;
            _bindingCell = bindingCell;
            _axisCanvas = new Canvas();
            Line line = new Line();
            line.StrokeThickness = 1.0;
            line.StrokeDashArray = new DoubleCollection { 2.0, 1.0 };
            _axisLine = line;
            _axisCanvas.Children.Add(_axisLine);
            Children.Add(_axisCanvas);
            _databarObject = null;
            _dataBarRectangle = new Rectangle();
            _dataBarRectangle.UseLayoutRounding = true;
            Children.Add(_dataBarRectangle);
            _dataBarBackground = new LinearGradientBrush();
            GradientStop stop = new GradientStop();
            stop.Color = Colors.Transparent;
            stop.Offset = 0.0;
            _cachedGradientStart = stop;
            _dataBarBackground.GradientStops.Add(_cachedGradientStart);
            GradientStop stop2 = new GradientStop();
            stop2.Color = Colors.Transparent;
            stop2.Offset = 0.0;
            _cachedGradientEnd = stop2;
            _dataBarBackground.GradientStops.Add(_cachedGradientEnd);
            GradientStop stop3 = new GradientStop();
            stop3.Color = Colors.Transparent;
            stop3.Offset = 0.0;
            _cachedGradientTransparentEnd = stop3;
            _dataBarBackground.GradientStops.Add(_cachedGradientTransparentEnd);
            _dataBarBackground.EndPoint = new Point(1.0, 0.0);
            _dataBarRectangle.Fill = _dataBarBackground;
            _iconObject = null;
            _imageContainer = new Border();
            _imageContainer.Style = null;
            _cachedImage = new Image();
            _cachedImage.HorizontalAlignment = HorizontalAlignment.Left;
            _imageContainer.Child = _cachedImage;
        }

        void ArrangeAxis(Size availableSize)
        {
            _axisCanvas.Width = availableSize.Width;
            _axisCanvas.Height = availableSize.Height;
            double num = Math.Round((double)(availableSize.Width * _cachedAxisPosition));
            if ((num > 0.0) && (num < availableSize.Width))
            {
                _axisLine.StrokeThickness = 1.0;
                _axisLine.X1 = num + 0.5;
                _axisLine.Y1 = -1.0;
                _axisLine.X2 = _axisLine.X1;
                _axisLine.Y2 = availableSize.Height + 1.0;
            }
            else
            {
                _axisLine.StrokeThickness = 0.0;
                _axisLine.X1 = 0.0;
                _axisLine.Y1 = 0.0;
                _axisLine.X2 = 0.0;
                _axisLine.Y2 = 0.0;
            }
            _axisCanvas.Arrange(new Rect(0.0, 0.0, availableSize.Width, availableSize.Height));
        }

        void ArrangeDataBarRectangle(Size availableSize)
        {
            double num = Math.Round((double)(availableSize.Width * _cachedAxisPosition));
            double width = Math.Round((double)(availableSize.Width * Math.Abs(_cachedScale)));
            Rect empty = Rect.Empty;
            if ((_cachedAxisPosition > 0.0) && (_cachedAxisPosition < 1.0))
            {
                if (_cachedScale >= 0.0)
                {
                    double x = num + _axisLine.StrokeThickness;
                    empty = new Rect(x, 0.0, width, availableSize.Height);
                }
                else
                {
                    double num4 = num - width;
                    empty = new Rect(num4, 0.0, width, availableSize.Height);
                }
            }
            else if (_cachedScale >= 0.0)
            {
                empty = new Rect(0.0, 0.0, width, availableSize.Height);
            }
            else
            {
                empty = new Rect(availableSize.Width - width, 0.0, width, availableSize.Height);
            }
            empty.Intersect(new Rect(0.0, 0.0, availableSize.Width, availableSize.Height));
            _dataBarRectangle.Arrange(empty);
        }

        void ArrangeIconSet(Size availableSize)
        {
            _imageContainer.Arrange(new Rect(0.0, 0.0, availableSize.Width, availableSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeAxis(finalSize);
            ArrangeDataBarRectangle(finalSize);
            ArrangeIconSet(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        void ClearDataBar()
        {
            _cachedAxisPosition = 0.0;
            _axisLine.Stroke = null;
            _axisLine.StrokeThickness = 0.0;
            _axisLine.X1 = 0.0;
            _axisLine.Y1 = 0.0;
            _axisLine.X2 = 0.0;
            _axisLine.Y2 = 0.0;
            _cachedGradientStart.Color = Colors.Transparent;
            _cachedGradientEnd.Color = Colors.Transparent;
            _cachedGradientTransparentEnd.Color = Colors.Transparent;
            _cachedScale = 0.0;
        }

        void ClearIcon()
        {
            if (_cachedImage != null)
            {
                _cachedImage.Source = null;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _imageContainer.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        void SetDataBarAxis()
        {
            if (_databarObject != null)
            {
                _cachedAxisPosition = (_databarObject.DataBarDirection == BarDirection.LeftToRight) ? _databarObject.DataBarAxisPosition : (1.0 - _databarObject.DataBarAxisPosition);
                if ((_cachedAxisPosition <= 0.0) || (_cachedAxisPosition >= 1.0))
                {
                    _axisLine.Stroke = null;
                    _axisLine.StrokeThickness = 0.0;
                }
                else
                {
                    _axisLine.Stroke = new SolidColorBrush(_databarObject.AxisColor);
                    _axisLine.StrokeThickness = 1.0;
                }
            }
        }

        void SetDataBarBorder()
        {
            if ((_databarObject != null) && _databarObject.ShowBorder)
            {
                _dataBarRectangle.StrokeThickness = 1.0;
                _dataBarRectangle.Stroke = new SolidColorBrush(_databarObject.BorderColor);
            }
            else
            {
                _dataBarRectangle.StrokeThickness = 0.0;
                _dataBarRectangle.Stroke = null;
            }
        }

        void SetDataBarColor()
        {
            if (_databarObject != null)
            {
                _cachedFillColor = _databarObject.Color;
                if (_databarObject.Gradient)
                {
                    _cachedGradientStart.Color = _cachedFillColor;
                    float num = 0.9f;
                    _cachedGradientEnd.Color = Windows.UI.Color.FromArgb(_cachedFillColor.A, (byte)((255f * num) + (_cachedFillColor.R * (1f - num))), (byte)((255f * num) + (_cachedFillColor.G * (1f - num))), (byte)((255f * num) + (_cachedFillColor.B * (1f - num))));
                }
                else
                {
                    _cachedGradientStart.Color = _cachedFillColor;
                    _cachedGradientEnd.Color = _cachedFillColor;
                    _cachedGradientTransparentEnd.Color = _cachedFillColor;
                }
            }
        }

        public void SetDataBarObject(DataBarDrawingObject databarObject)
        {
            if (!object.Equals(_databarObject, databarObject))
            {
                _databarObject = databarObject;
                if (_databarObject != null)
                {
                    SetDataBarAxis();
                    SetDataBarScale();
                    SetDataBarColor();
                    SetDataBarBorder();
                }
                else
                {
                    ClearDataBar();
                }
                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        void SetDataBarScale()
        {
            if (_databarObject != null)
            {
                _cachedScale = (_databarObject.DataBarDirection == BarDirection.LeftToRight) ? _databarObject.Scale : -_databarObject.Scale;
                Math.Abs(_cachedScale);
                if (_cachedScale >= 0.0)
                {
                    _dataBarBackground.StartPoint = new Point(0.0, 0.0);
                    _dataBarBackground.EndPoint = new Point(1.0, 0.0);
                }
                else
                {
                    _dataBarBackground.StartPoint = new Point(1.0, 0.0);
                    _dataBarBackground.EndPoint = new Point(0.0, 0.0);
                }
                _cachedGradientEnd.Offset = 1.0;
                _cachedGradientTransparentEnd.Offset = 1.0;
            }
        }

        public void SetIconObject(IconDrawingObject iconObject, float zoomFactor, Cell bindingCell)
        {
            if (!object.Equals(_iconObject, iconObject))
            {
                if (iconObject != null)
                {
                    _cachedImage.Source = ConditionalFormatIcons.GetIconSource(iconObject.IconSetType, iconObject.IndexOfIcon);
                }
                else
                {
                    ClearIcon();
                }
                _iconObject = iconObject;
                InvalidateMeasure();
                InvalidateArrange();
            }
            if ((_iconObject != null) && (_cachedImage != null))
            {
                HorizontalAlignment left = HorizontalAlignment.Left;
                if (_iconObject.ShowIconOnly)
                {
                    switch (bindingCell.ActualHorizontalAlignment)
                    {
                        case CellHorizontalAlignment.Center:
                            left = HorizontalAlignment.Center;
                            break;

                        case CellHorizontalAlignment.Right:
                            left = HorizontalAlignment.Right;
                            break;
                    }
                }
                _cachedImage.HorizontalAlignment = left;
                VerticalAlignment alignment2 = bindingCell.ActualVerticalAlignment.ToVerticalAlignment();
                _cachedImage.VerticalAlignment = alignment2;
            }
            if (_cachedZoomFactor != zoomFactor)
            {
                _cachedImage.Width = (double)(16f * zoomFactor);
                _cachedImage.Height = (double)(16f * zoomFactor);
                _cachedZoomFactor = zoomFactor;
                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        public void SetImageContainer()
        {
            if (!Children.Contains(_imageContainer))
            {
                Children.Add(_imageContainer);
            }
        }

        public static class ConditionalFormatIcons
        {
            [ThreadStatic]
            static string[,] _cachedIconNames;
            [ThreadStatic]
            static ImageSource[,] _cachedImageSources;

            public static ImageSource GetIconSource(IconSetType iconType, int iconIndex)
            {
                int num = (int)iconType;
                ImageSource source = CachedImageSources[num, iconIndex];
                if (source == null)
                {
                    // hdt
                    Uri uri = new Uri(string.Format("ms-appx:///Dt.Cells/Icons/ConditionalFormats/{0}", CachedIconNames[num, iconIndex]));
                    source = new BitmapImage(uri);
                    CachedImageSources[num, iconIndex] = source;
                }
                return source;
            }

            static string[,] CachedIconNames
            {
                get
                {
                    if (_cachedIconNames == null)
                    {
                        string[,] strArray = new string[20, 5];
                        strArray[0, 0] = "ArrowRedDown.png";
                        strArray[0, 1] = "ArrowYellowRight.png";
                        strArray[0, 2] = "ArrowBlueUp.png";
                        strArray[1, 0] = "ArrowGrayDown.png";
                        strArray[1, 1] = "ArrowGrayRight.png";
                        strArray[1, 2] = "ArrowGrayUp.png";
                        strArray[2, 0] = "TriangleRedDown.png";
                        strArray[2, 1] = "TriangleYellow.png";
                        strArray[2, 2] = "TriangleGreenUp.png";
                        strArray[3, 0] = "Star0.png";
                        strArray[3, 1] = "Star1.png";
                        strArray[3, 2] = "Star2.png";
                        strArray[4, 0] = "RedFlag.png";
                        strArray[4, 1] = "YellowFlag.png";
                        strArray[4, 2] = "GreenFlag.png";
                        strArray[5, 0] = "RedCircle.png";
                        strArray[5, 1] = "YellowCircle.png";
                        strArray[5, 2] = "GreenCircle.png";
                        strArray[6, 0] = "RedTraficLight.png";
                        strArray[6, 1] = "YellowTraficLight.png";
                        strArray[6, 2] = "GreenTraficLight.png";
                        strArray[7, 0] = "RedDiamond.png";
                        strArray[7, 1] = "YellowTrangle.png";
                        strArray[7, 2] = "GreenCircle.png";
                        strArray[8, 0] = "RedCrossSymbol.png";
                        strArray[8, 1] = "YellowExclamationSymbol.png";
                        strArray[8, 2] = "GreenCheckSymbol.png";
                        strArray[9, 0] = "RedCross.png";
                        strArray[9, 1] = "YellowExclamation.png";
                        strArray[9, 2] = "GreenCheck.png";
                        strArray[10, 0] = "ArrowRedDown.png";
                        strArray[10, 1] = "Arrow45YellowDown.png";
                        strArray[10, 2] = "Arrow45YellowUp.png";
                        strArray[10, 3] = "ArrowBlueUp.png";
                        strArray[11, 0] = "ArrowGrayDown.png";
                        strArray[11, 1] = "Arrow45GrayDown.png";
                        strArray[11, 2] = "Arrow45GrayUp.png";
                        strArray[11, 3] = "ArrowGrayUp.png";
                        strArray[12, 0] = "BlackFillCircles.png";
                        strArray[12, 1] = "GrayFillCicle.png";
                        strArray[12, 2] = "PinkFillCircle.png";
                        strArray[12, 3] = "RedFillCircle.png";
                        strArray[13, 0] = "Rating1.png";
                        strArray[13, 1] = "Rating2.png";
                        strArray[13, 2] = "Rating3.png";
                        strArray[13, 3] = "Rating4.png";
                        strArray[14, 0] = "BlackCircle.png";
                        strArray[14, 1] = "RedCircle.png";
                        strArray[14, 2] = "YellowCircle.png";
                        strArray[14, 3] = "GreenCircle.png";
                        strArray[15, 0] = "ArrowRedDown.png";
                        strArray[15, 1] = "Arrow45YellowDown.png";
                        strArray[15, 2] = "ArrowYellowRight.png";
                        strArray[15, 3] = "Arrow45YellowUp.png";
                        strArray[15, 4] = "ArrowBlueUp.png";
                        strArray[0x10, 0] = "ArrowGrayDown.png";
                        strArray[0x10, 1] = "Arrow45GrayDown.png";
                        strArray[0x10, 2] = "ArrowGrayRight.png";
                        strArray[0x10, 3] = "Arrow45GrayUp.png";
                        strArray[0x10, 4] = "ArrowGrayUp.png";
                        strArray[0x11, 0] = "Rating0.png";
                        strArray[0x11, 1] = "Rating1.png";
                        strArray[0x11, 2] = "Rating2.png";
                        strArray[0x11, 3] = "Rating3.png";
                        strArray[0x11, 4] = "Rating4.png";
                        strArray[0x12, 0] = "Quarter0.png";
                        strArray[0x12, 1] = "Quarter1.png";
                        strArray[0x12, 2] = "Quarter2.png";
                        strArray[0x12, 3] = "Quarter3.png";
                        strArray[0x12, 4] = "Quarter4.png";
                        strArray[0x13, 0] = "Box0.png";
                        strArray[0x13, 1] = "Box1.png";
                        strArray[0x13, 2] = "Box2.png";
                        strArray[0x13, 3] = "Box3.png";
                        strArray[0x13, 4] = "Box4.png";
                        _cachedIconNames = strArray;
                    }
                    return _cachedIconNames;
                }
            }

            static ImageSource[,] CachedImageSources
            {
                get
                {
                    if (_cachedImageSources == null)
                    {
                        _cachedImageSources = new ImageSource[20, 5];
                    }
                    return _cachedImageSources;
                }
            }
        }
    }
}

