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
using System.Reflection;
using System.Runtime.InteropServices;
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
    /// Represents an individual <see cref="T:Dt.Cells.UI.GcSpreadSheet" /> cell.
    /// </summary>
    [TemplatePart(Name = "Root", Type = typeof(CellBackgroundPanel))]
    public partial class CellPresenter : CellPresenterBase
    {
        private ConditionalFormatView _conditionalView;
        private CustomDrawingObject _customDrawingObject;
        private FrameworkElement _customDrawingObjectView;
        private DataBarDrawingObject _dataBarObject;
        private IconDrawingObject _iconObject;
        private Sparkline _sparkInfo;
        private BaseSparklineView _sparklineView;
        private StrikethroughView _strikethroughView;

        /// <summary>
        /// Creates a new instance of the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.CellPresenter" /> class.
        /// </summary>
        public CellPresenter()
        {
            base.DefaultStyleKey = typeof(CellPresenter);
        }

        private void AttachSparklineEvents()
        {
            Sparkline sparkline = this._sparkInfo;
            if (sparkline != null)
            {
                sparkline.SparklineChanged += new EventHandler(this.sparkline_SparklineChanged);
            }
        }

        internal override void CleanUpBeforeDiscard()
        {
            base.CleanUpBeforeDiscard();
            if (this._customDrawingObjectView != null)
            {
                base.RootPanel.Children.Remove(this._customDrawingObjectView);
            }
        }

        private BaseSparklineView CreateSparkline(Sparkline info)
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

        internal override void DetachEvents()
        {
            base.DetachEvents();
            this.DettachSparklineEvents();
        }

        private void DettachSparklineEvents()
        {
            Sparkline sparkline = this._sparkInfo;
            if (sparkline != null)
            {
                sparkline.SparklineChanged -= new EventHandler(this.sparkline_SparklineChanged);
            }
        }

        private void HideContent(bool visible)
        {
            base.SetContentVisible(visible);
        }

        internal override void Invalidate()
        {
            if (this._sparklineView != null)
            {
                this.UpdateSparkline();
            }
            base.Invalidate();
        }

        private void sparkline_SparklineChanged(object sender, EventArgs e)
        {
            Sparkline sparkline = sender as Sparkline;
            if ((this._sparklineView == null) || (this._sparklineView.SparklineType != sparkline.SparklineType))
            {
                if (this._sparklineView != null)
                {
                    base.RootPanel.Children.Remove(this._sparklineView);
                    this._sparklineView = null;
                }
                this.SynSparklineView();
            }
            else
            {
                this.UpdateSparkline();
            }
        }

        private void SynContitionalView(out bool isContentVisible)
        {
            isContentVisible = true;
            if ((this._dataBarObject != null) || (this._iconObject != null))
            {
                if (this._conditionalView == null)
                {
                    this._conditionalView = new ConditionalFormatView(base.BindingCell);
                    base.RootPanel.Children.Add(this._conditionalView);
                    Canvas.SetZIndex(this._conditionalView, 500);
                }
                this._conditionalView.SetDataBarObject(this._dataBarObject);
                if (this._iconObject != null)
                {
                    this._conditionalView.SetImageContainer();
                    this._conditionalView.SetIconObject(this._iconObject, base.SheetView.ZoomFactor, base.BindingCell);
                }
                bool flag = true;
                if (flag && (this._dataBarObject != null))
                {
                    flag = !this._dataBarObject.ShowBarOnly;
                }
                if (flag && (this._iconObject != null))
                {
                    flag = !this._iconObject.ShowIconOnly;
                }
                isContentVisible = flag;
            }
            else
            {
                if (this._conditionalView != null)
                {
                    base.RootPanel.Children.Remove(this._conditionalView);
                    this._conditionalView = null;
                }
                isContentVisible = true;
            }
        }

        private void SynCustomDrawingObjectView(out bool isContentVisible)
        {
            isContentVisible = true;
            if (this._customDrawingObject != null)
            {
                isContentVisible = !this._customDrawingObject.ShowDrawingObjectOnly;
                FrameworkElement rootElement = this._customDrawingObject.RootElement;
                if (this._customDrawingObjectView != rootElement)
                {
                    if (this._customDrawingObjectView != null)
                    {
                        base.RootPanel.Children.Remove(this._customDrawingObjectView);
                    }
                    this._customDrawingObjectView = rootElement;
                    if (this._customDrawingObjectView != null)
                    {
                        Panel parent = this._customDrawingObjectView.Parent as Panel;
                        if ((parent != null) && (parent != base.RootPanel))
                        {
                            parent.Children.Remove(this._customDrawingObjectView);
                        }
                        if (!base.RootPanel.Children.Contains(this._customDrawingObjectView))
                        {
                            base.RootPanel.Children.Add(this._customDrawingObjectView);
                        }
                    }
                }
            }
            else if (this._customDrawingObjectView != null)
            {
                base.RootPanel.Children.Remove(this._customDrawingObjectView);
                this._customDrawingObjectView = null;
            }
        }

        private void SynSparklineView()
        {
            SheetView sheetView = base.SheetView;
            if (sheetView != null)
            {
                if (this._sparkInfo != null)
                {
                    if (this._sparklineView == null)
                    {
                        this._sparklineView = this.CreateSparkline(this._sparkInfo);
                        this._sparklineView.ZoomFactor = base.OwningRow.OwningPresenter.Sheet.ZoomFactor;
                        ((IThemeContextSupport)this._sparklineView).SetContext(sheetView.Worksheet);
                        Canvas.SetZIndex(this._sparklineView, 0x3e8);
                        base.RootPanel.Children.Add(this._sparklineView);
                        if (base.SheetView != null)
                        {
                            this._sparklineView.Update(new Windows.Foundation.Size(base.ActualWidth, base.ActualHeight), (double)base.SheetView.ZoomFactor);
                        }
                    }
                }
                else if (this._sparklineView != null)
                {
                    this.DettachSparklineEvents();
                    base.RootPanel.Children.Remove(this._sparklineView);
                    this._sparklineView = null;
                }
            }
        }

        private void SynStrikethroughView()
        {
            bool actualStrikethrough = base.BindingCell.ActualStrikethrough;
            if (this._strikethroughView != null)
            {
                base.RootPanel.Children.Remove(this._strikethroughView);
                this._strikethroughView = null;
            }
            if (actualStrikethrough && (this._strikethroughView == null))
            {
                this._strikethroughView = new StrikethroughView(base.BindingCell, base.RootPanel);
                this._strikethroughView.SetLines(base.SheetView.ZoomFactor, base.BindingCell);
                base.RootPanel.Children.Add(this._strikethroughView);
            }
        }

        internal override bool TryUpdateVisualTree()
        {
            bool flag = false;
            Cell bindingCell = base.BindingCell;
            if (bindingCell == null)
            {
                return false;
            }
            Worksheet sheet = bindingCell.Worksheet;
            int row = base.Row;
            int column = base.Column;
            if (base.CellLayout != null)
            {
                row = base.CellLayout.Row;
                column = base.CellLayout.Column;
            }
            Sparkline objB = sheet.GetSparkline(row, column);
            if (!object.Equals(this._sparkInfo, objB))
            {
                this.SparkLine = objB;
                this.SynSparklineView();
                flag = true;
            }
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            List<DrawingObject> list = new List<DrawingObject>();
            DrawingObject[] objArray = sheet.GetDrawingObject(row, column, 1, 1);
            if ((objArray != null) && (objArray.Length > 0))
            {
                list.AddRange(objArray);
            }
            if (((base.SheetView != null) && (base.SheetView._host != null)) && (base.SheetView._host is Excel))
            {
                IDrawingObjectProvider drawingObjectProvider = DrawingObjectManager.GetDrawingObjectProvider(base.SheetView._host as Excel);
                if (drawingObjectProvider != null)
                {
                    DrawingObject[] objArray2 = drawingObjectProvider.GetDrawingObjects(sheet, row, column, 1, 1);
                    if ((objArray2 != null) && (objArray2.Length > 0))
                    {
                        list.AddRange(objArray2);
                    }
                }
            }
            if ((list != null) && (list.Count > 0))
            {
                foreach (DrawingObject obj2 in list)
                {
                    if (!flag2)
                    {
                        DataBarDrawingObject obj3 = obj2 as DataBarDrawingObject;
                        if (obj3 != null)
                        {
                            flag2 = true;
                            this._dataBarObject = obj3;
                            flag = true;
                            continue;
                        }
                    }
                    if (!flag3)
                    {
                        IconDrawingObject obj4 = obj2 as IconDrawingObject;
                        if (obj4 != null)
                        {
                            flag3 = true;
                            this._iconObject = obj4;
                            flag = true;
                            continue;
                        }
                    }
                    if (!flag4)
                    {
                        CustomDrawingObject obj5 = obj2 as CustomDrawingObject;
                        if (obj5 != null)
                        {
                            flag4 = true;
                            this._customDrawingObject = obj5;
                            flag = true;
                        }
                    }
                }
            }
            if (!flag2)
            {
                this._dataBarObject = null;
            }
            if (!flag3)
            {
                this._iconObject = null;
            }
            bool flag5 = false;
            bool isContentVisible = true;
            this.SynContitionalView(out isContentVisible);
            flag5 |= !isContentVisible;
            this.SynStrikethroughView();
            if (!flag4)
            {
                this._customDrawingObject = null;
            }
            bool flag7 = true;
            this.SynCustomDrawingObjectView(out flag7);
            flag5 |= !flag7;
            this.HideContent(!flag5);
            bool flag8 = base.TryUpdateVisualTree();
            if (!flag)
            {
                return flag8;
            }
            return true;
        }

        private void UpdateSparkline()
        {
            if (base.SheetView != null)
            {
                BaseSparklineView view = this._sparklineView;
                if (view != null)
                {
                    view.Update(new Windows.Foundation.Size(base.ActualWidth, base.ActualHeight), (double)base.SheetView.ZoomFactor);
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates that the cell's viewport is active. 
        /// </summary>
        protected override bool IsActive
        {
            get
            {
                SheetView sheet = base.OwningRow.OwningPresenter.Sheet;
                return ((sheet.GetActiveColumnViewportIndex() == base.OwningRow.OwningPresenter.ColumnViewportIndex) && (sheet.GetActiveRowViewportIndex() == base.OwningRow.OwningPresenter.RowViewportIndex));
            }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is the active cell.
        /// </summary>
        protected override bool IsCurrent
        {
            get
            {
                Worksheet worksheet = base.OwningRow.OwningPresenter.Sheet.Worksheet;
                return ((worksheet.ActiveRowIndex == base.Row) && (worksheet.ActiveColumnIndex == base.Column));
            }
        }

        internal override bool IsRecylable
        {
            get { return ((this._customDrawingObject == null) && base.IsRecylable); }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is selected.
        /// </summary>
        protected override bool IsSelected
        {
            get { return base.OwningRow.OwningPresenter.Sheet.Worksheet.IsSelected(base.Row, base.Column); }
        }

        private Sparkline SparkLine
        {
            get { return this._sparkInfo; }
            set
            {
                if (this._sparkInfo != value)
                {
                    this.DettachSparklineEvents();
                    if (this._sparklineView != null)
                    {
                        base.RootPanel.Children.Remove(this._sparklineView);
                        this._sparklineView = null;
                    }
                    this._sparkInfo = value;
                    this.AttachSparklineEvents();
                    base.InvalidateMeasure();
                }
            }
        }
    }
    internal partial class ConditionalFormatView : Panel
    {
        private Canvas _axisCanvas;
        private Windows.UI.Xaml.Shapes.Line _axisLine;
        private Cell _bindingCell;
        private double _cachedAxisPosition;
        private Windows.UI.Color _cachedFillColor;
        private GradientStop _cachedGradientEnd;
        private GradientStop _cachedGradientStart;
        private GradientStop _cachedGradientTransparentEnd;
        private Image _cachedImage;
        private double _cachedScale;
        private float _cachedZoomFactor;
        private LinearGradientBrush _dataBarBackground;
        private DataBarDrawingObject _databarObject;
        private Rectangle _dataBarRectangle;
        private IconDrawingObject _iconObject;
        private Border _imageContainer;
        private const int AxisWidth = 1;
        public const int DatabarZIndex = 100;
        private const int DefaultIcontHeight = 0x10;
        private const int DefaultIconWidth = 0x10;
        public const int IconSetZIndex = 200;
        private const int ViewMargin = 1;

        public ConditionalFormatView(Cell bindingCell)
        {
            base.Margin = new Windows.UI.Xaml.Thickness(1.0);
            base.UseLayoutRounding = true;
            this._bindingCell = bindingCell;
            this._axisCanvas = new Canvas();
            Windows.UI.Xaml.Shapes.Line line = new Windows.UI.Xaml.Shapes.Line();
            line.StrokeThickness = 1.0;
            line.StrokeDashArray = new DoubleCollection { 2.0, 1.0 };
            this._axisLine = line;
            this._axisCanvas.Children.Add(this._axisLine);
            base.Children.Add(this._axisCanvas);
            this._databarObject = null;
            this._dataBarRectangle = new Rectangle();
            this._dataBarRectangle.UseLayoutRounding = true;
            base.Children.Add(this._dataBarRectangle);
            this._dataBarBackground = new LinearGradientBrush();
            GradientStop stop = new GradientStop();
            stop.Color = Colors.Transparent;
            stop.Offset = 0.0;
            this._cachedGradientStart = stop;
            this._dataBarBackground.GradientStops.Add(this._cachedGradientStart);
            GradientStop stop2 = new GradientStop();
            stop2.Color = Colors.Transparent;
            stop2.Offset = 0.0;
            this._cachedGradientEnd = stop2;
            this._dataBarBackground.GradientStops.Add(this._cachedGradientEnd);
            GradientStop stop3 = new GradientStop();
            stop3.Color = Colors.Transparent;
            stop3.Offset = 0.0;
            this._cachedGradientTransparentEnd = stop3;
            this._dataBarBackground.GradientStops.Add(this._cachedGradientTransparentEnd);
            this._dataBarBackground.EndPoint = new Windows.Foundation.Point(1.0, 0.0);
            this._dataBarRectangle.Fill = this._dataBarBackground;
            this._iconObject = null;
            this._imageContainer = new Border();
            this._imageContainer.Style = null;
            this._cachedImage = new Image();
            this._cachedImage.HorizontalAlignment = HorizontalAlignment.Left;
            this._imageContainer.Child = this._cachedImage;
        }

        private void ArrangeAxis(Windows.Foundation.Size availableSize)
        {
            this._axisCanvas.Width = availableSize.Width;
            this._axisCanvas.Height = availableSize.Height;
            double num = Math.Round((double)(availableSize.Width * this._cachedAxisPosition));
            if ((num > 0.0) && (num < availableSize.Width))
            {
                this._axisLine.StrokeThickness = 1.0;
                this._axisLine.X1 = num + 0.5;
                this._axisLine.Y1 = -1.0;
                this._axisLine.X2 = this._axisLine.X1;
                this._axisLine.Y2 = availableSize.Height + 1.0;
            }
            else
            {
                this._axisLine.StrokeThickness = 0.0;
                this._axisLine.X1 = 0.0;
                this._axisLine.Y1 = 0.0;
                this._axisLine.X2 = 0.0;
                this._axisLine.Y2 = 0.0;
            }
            this._axisCanvas.Arrange(new Windows.Foundation.Rect(0.0, 0.0, availableSize.Width, availableSize.Height));
        }

        private void ArrangeDataBarRectangle(Windows.Foundation.Size availableSize)
        {
            double num = Math.Round((double)(availableSize.Width * this._cachedAxisPosition));
            double width = Math.Round((double)(availableSize.Width * Math.Abs(this._cachedScale)));
            Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
            if ((this._cachedAxisPosition > 0.0) && (this._cachedAxisPosition < 1.0))
            {
                if (this._cachedScale >= 0.0)
                {
                    double x = num + this._axisLine.StrokeThickness;
                    empty = new Windows.Foundation.Rect(x, 0.0, width, availableSize.Height);
                }
                else
                {
                    double num4 = num - width;
                    empty = new Windows.Foundation.Rect(num4, 0.0, width, availableSize.Height);
                }
            }
            else if (this._cachedScale >= 0.0)
            {
                empty = new Windows.Foundation.Rect(0.0, 0.0, width, availableSize.Height);
            }
            else
            {
                empty = new Windows.Foundation.Rect(availableSize.Width - width, 0.0, width, availableSize.Height);
            }
            empty.Intersect(new Windows.Foundation.Rect(0.0, 0.0, availableSize.Width, availableSize.Height));
            this._dataBarRectangle.Arrange(empty);
        }

        private void ArrangeIconSet(Windows.Foundation.Size availableSize)
        {
            this._imageContainer.Arrange(new Windows.Foundation.Rect(0.0, 0.0, availableSize.Width, availableSize.Height));
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this.ArrangeAxis(finalSize);
            this.ArrangeDataBarRectangle(finalSize);
            this.ArrangeIconSet(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        private void ClearDataBar()
        {
            this._cachedAxisPosition = 0.0;
            this._axisLine.Stroke = null;
            this._axisLine.StrokeThickness = 0.0;
            this._axisLine.X1 = 0.0;
            this._axisLine.Y1 = 0.0;
            this._axisLine.X2 = 0.0;
            this._axisLine.Y2 = 0.0;
            this._cachedGradientStart.Color = Colors.Transparent;
            this._cachedGradientEnd.Color = Colors.Transparent;
            this._cachedGradientTransparentEnd.Color = Colors.Transparent;
            this._cachedScale = 0.0;
        }

        private void ClearIcon()
        {
            if (this._cachedImage != null)
            {
                this._cachedImage.Source = null;
            }
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._imageContainer.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        private void SetDataBarAxis()
        {
            Action action = null;
            if (this._databarObject != null)
            {
                this._cachedAxisPosition = (this._databarObject.DataBarDirection == BarDirection.LeftToRight) ? this._databarObject.DataBarAxisPosition : (1.0 - this._databarObject.DataBarAxisPosition);
                if ((this._cachedAxisPosition <= 0.0) || (this._cachedAxisPosition >= 1.0))
                {
                    this._axisLine.Stroke = null;
                    this._axisLine.StrokeThickness = 0.0;
                }
                else
                {
                    if (action == null)
                    {
                        action = delegate
                        {
                            this._axisLine.Stroke = new SolidColorBrush(this._databarObject.AxisColor);
                        };
                    }
                    Dt.Cells.Data.UIAdaptor.InvokeSync(action);
                    this._axisLine.StrokeThickness = 1.0;
                }
            }
        }

        private void SetDataBarBorder()
        {
            Action action = null;
            if ((this._databarObject != null) && this._databarObject.ShowBorder)
            {
                this._dataBarRectangle.StrokeThickness = 1.0;
                if (action == null)
                {
                    action = delegate
                    {
                        this._dataBarRectangle.Stroke = new SolidColorBrush(this._databarObject.BorderColor);
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action);
            }
            else
            {
                this._dataBarRectangle.StrokeThickness = 0.0;
                this._dataBarRectangle.Stroke = null;
            }
        }

        private void SetDataBarColor()
        {
            if (this._databarObject != null)
            {
                this._cachedFillColor = this._databarObject.Color;
                if (this._databarObject.Gradient)
                {
                    this._cachedGradientStart.Color = this._cachedFillColor;
                    float num = 0.9f;
                    this._cachedGradientEnd.Color = Windows.UI.Color.FromArgb(this._cachedFillColor.A, (byte)((255f * num) + (this._cachedFillColor.R * (1f - num))), (byte)((255f * num) + (this._cachedFillColor.G * (1f - num))), (byte)((255f * num) + (this._cachedFillColor.B * (1f - num))));
                }
                else
                {
                    this._cachedGradientStart.Color = this._cachedFillColor;
                    this._cachedGradientEnd.Color = this._cachedFillColor;
                    this._cachedGradientTransparentEnd.Color = this._cachedFillColor;
                }
            }
        }

        public void SetDataBarObject(DataBarDrawingObject databarObject)
        {
            if (!object.Equals(this._databarObject, databarObject))
            {
                this._databarObject = databarObject;
                if (this._databarObject != null)
                {
                    this.SetDataBarAxis();
                    this.SetDataBarScale();
                    this.SetDataBarColor();
                    this.SetDataBarBorder();
                }
                else
                {
                    this.ClearDataBar();
                }
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        private void SetDataBarScale()
        {
            if (this._databarObject != null)
            {
                this._cachedScale = (this._databarObject.DataBarDirection == BarDirection.LeftToRight) ? this._databarObject.Scale : -this._databarObject.Scale;
                Math.Abs(this._cachedScale);
                if (this._cachedScale >= 0.0)
                {
                    this._dataBarBackground.StartPoint = new Windows.Foundation.Point(0.0, 0.0);
                    this._dataBarBackground.EndPoint = new Windows.Foundation.Point(1.0, 0.0);
                }
                else
                {
                    this._dataBarBackground.StartPoint = new Windows.Foundation.Point(1.0, 0.0);
                    this._dataBarBackground.EndPoint = new Windows.Foundation.Point(0.0, 0.0);
                }
                this._cachedGradientEnd.Offset = 1.0;
                this._cachedGradientTransparentEnd.Offset = 1.0;
            }
        }

        public void SetIconObject(IconDrawingObject iconObject, float zoomFactor, Cell bindingCell)
        {
            if (!object.Equals(this._iconObject, iconObject))
            {
                if (iconObject != null)
                {
                    this._cachedImage.Source = ConditionalFormatIcons.GetIconSource(iconObject.IconSetType, iconObject.IndexOfIcon);
                }
                else
                {
                    this.ClearIcon();
                }
                this._iconObject = iconObject;
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
            if ((this._iconObject != null) && (this._cachedImage != null))
            {
                HorizontalAlignment left = HorizontalAlignment.Left;
                if (this._iconObject.ShowIconOnly)
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
                this._cachedImage.HorizontalAlignment = left;
                VerticalAlignment alignment2 = bindingCell.ActualVerticalAlignment.ToVerticalAlignment();
                this._cachedImage.VerticalAlignment = alignment2;
            }
            if (this._cachedZoomFactor != zoomFactor)
            {
                this._cachedImage.Width = (double)(16f * zoomFactor);
                this._cachedImage.Height = (double)(16f * zoomFactor);
                this._cachedZoomFactor = zoomFactor;
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        public void SetImageContainer()
        {
            if (!base.Children.Contains(this._imageContainer))
            {
                base.Children.Add(this._imageContainer);
            }
        }

        public static class ConditionalFormatIcons
        {
            [ThreadStatic]
            private static string[,] _cachedIconNames;
            [ThreadStatic]
            private static ImageSource[,] _cachedImageSources;

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

            private static string[,] CachedIconNames
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

            private static ImageSource[,] CachedImageSources
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

