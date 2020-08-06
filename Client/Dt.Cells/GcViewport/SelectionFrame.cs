#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class SelectionFrame : Panel
    {
        [ThreadStatic]
        static readonly Brush DefaultSelectionBorderBrush = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
        const int BORDER_OFFSET = 4;
        internal const int FILL_WEIGHT = 5;
        const int SelectionBorderThickness = 3;

        Rectangle _bottomRectangle;
        Color _currentBorderColor;
        Rectangle _fillIndicator;
        Rect _fillIndicatorBounds = Rect.Empty;
        FillIndicatorPosition _fillIndicatorPosition;
        bool _isBottomVisible = true;
        bool _isFillIndicatorVisible = true;
        bool _isLeftVisible = true;
        bool _isRightVisible = true;
        bool _isTopVisible = true;
        Rectangle _leftRectangle;
        Rectangle _rightRectangle;
        Color _selectionBorderColor;
        double _thickNess = 3.0;
        Rectangle _topRectangle;

        public SelectionFrame(GcViewport owingViewport)
        {
            OwingViewport = owingViewport;
            _selectionBorderColor = GetSelectionBorderColor(OwingViewport.Sheet.Worksheet.SelectionBorderColor, OwingViewport.Sheet.Worksheet.SelectionBorderThemeColor);
            _currentBorderColor = _selectionBorderColor;
            _leftRectangle = new Rectangle();
            _leftRectangle.Fill = DefaultSelectionBorderBrush;
            _leftRectangle.Stroke = null;
            _leftRectangle.StrokeThickness = 0.0;
            _leftRectangle.Width = 3.0;
            base.Children.Add(_leftRectangle);
            _topRectangle = new Rectangle();
            _topRectangle.Fill = DefaultSelectionBorderBrush;
            _topRectangle.Stroke = null;
            _topRectangle.StrokeThickness = 0.0;
            _topRectangle.Height = 3.0;
            base.Children.Add(_topRectangle);
            _rightRectangle = new Rectangle();
            _rightRectangle.Fill = DefaultSelectionBorderBrush;
            _rightRectangle.Stroke = null;
            _rightRectangle.StrokeThickness = 0.0;
            _rightRectangle.Width = 3.0;
            base.Children.Add(_rightRectangle);
            _bottomRectangle = new Rectangle();
            _bottomRectangle.Fill = DefaultSelectionBorderBrush;
            _bottomRectangle.Stroke = null;
            _bottomRectangle.StrokeThickness = 0.0;
            _bottomRectangle.Height = 3.0;
            base.Children.Add(_bottomRectangle);
            IsLeftVisible = true;
            IsRightVisible = true;
            IsTopVisible = true;
            IsBottomVisible = true;
            _fillIndicator = new Rectangle();
            base.Children.Add(_fillIndicator);
            SetSelectionStyle(_currentBorderColor);
            _fillIndicatorPosition = FillIndicatorPosition.BottomRight;
        }

        Rect ArrangeFillRect(Size finalSize)
        {
            Rect empty = Rect.Empty;
            switch (_fillIndicatorPosition)
            {
                case FillIndicatorPosition.TopRight:
                    empty = new Rect((finalSize.Width - 5.0) + 1.0, 3.0, 5.0, 5.0);
                    break;

                case FillIndicatorPosition.BottomLeft:
                    empty = new Rect(3.0, (finalSize.Height - 5.0) + 1.0, 5.0, 5.0);
                    break;

                case FillIndicatorPosition.BottomRight:
                    empty = new Rect((finalSize.Width - 5.0) + 1.0, (finalSize.Height - 5.0) + 1.0, 5.0, 5.0);
                    break;
            }
            _fillIndicator.Arrange(empty);
            return empty;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!finalSize.IsEmpty)
            {
                Rect empty = Rect.Empty;
                Rect rect2 = Rect.Empty;
                Rect rect3 = Rect.Empty;
                Rect rect4 = Rect.Empty;
                if (IsLeftVisible)
                {
                    if (IsFillIndicatorVisible && (FillIndicatorPosition == FillIndicatorPosition.BottomLeft))
                    {
                        double height = finalSize.Height - 4.0;
                        if (height < 0.0)
                        {
                            height = 0.0;
                        }
                        empty = new Rect(0.0, 0.0, Thickness, height);
                    }
                    else
                    {
                        empty = new Rect(0.0, 0.0, Thickness, finalSize.Height);
                    }
                }
                if (IsRightVisible)
                {
                    if (IsFillIndicatorVisible)
                    {
                        if (_fillIndicatorPosition == FillIndicatorPosition.TopRight)
                        {
                            double num2 = (finalSize.Height - 5.0) - 4.0;
                            if (num2 < 0.0)
                            {
                                num2 = 0.0;
                            }
                            rect2 = new Rect(finalSize.Width - Thickness, 9.0, Thickness, num2);
                        }
                        else if (_fillIndicatorPosition == FillIndicatorPosition.BottomRight)
                        {
                            double num3 = finalSize.Height - 5.0;
                            if (num3 < 0.0)
                            {
                                num3 = 0.0;
                            }
                            rect2 = new Rect(finalSize.Width - Thickness, 0.0, Thickness, num3);
                        }
                        else
                        {
                            rect2 = new Rect(finalSize.Width - Thickness, 0.0, Thickness, finalSize.Height);
                        }
                    }
                    else
                    {
                        rect2 = new Rect(finalSize.Width - Thickness, 0.0, Thickness, finalSize.Height);
                    }
                }
                if (IsTopVisible)
                {
                    if (IsFillIndicatorVisible && (FillIndicatorPosition == FillIndicatorPosition.TopRight))
                    {
                        double width = finalSize.Width - 4.0;
                        if (width < 0.0)
                        {
                            width = 0.0;
                        }
                        rect3 = new Rect(0.0, 0.0, width, Thickness);
                    }
                    else
                    {
                        rect3 = new Rect(0.0, 0.0, finalSize.Width, Thickness);
                    }
                }
                if (IsBottomVisible)
                {
                    if (IsFillIndicatorVisible)
                    {
                        if (_fillIndicatorPosition == FillIndicatorPosition.BottomLeft)
                        {
                            double num5 = (finalSize.Width - 5.0) - 4.0;
                            if (num5 < 0.0)
                            {
                                num5 = 0.0;
                            }
                            rect4 = new Rect(9.0, finalSize.Height - Thickness, num5, Thickness);
                        }
                        else if (_fillIndicatorPosition == FillIndicatorPosition.BottomRight)
                        {
                            double num6 = finalSize.Width - 5.0;
                            if (num6 < 0.0)
                            {
                                num6 = 0.0;
                            }
                            rect4 = new Rect(0.0, finalSize.Height - Thickness, num6, Thickness);
                        }
                        else
                        {
                            rect4 = new Rect(0.0, finalSize.Height - Thickness, finalSize.Width, Thickness);
                        }
                    }
                    else
                    {
                        rect4 = new Rect(0.0, finalSize.Height - Thickness, finalSize.Width, Thickness);
                    }
                }
                if (IsLeftVisible)
                {
                    _leftRectangle.Arrange(empty);
                }
                if (IsTopVisible)
                {
                    _topRectangle.Arrange(rect3);
                }
                if (IsRightVisible)
                {
                    _rightRectangle.Arrange(rect2);
                }
                if (IsBottomVisible)
                {
                    _bottomRectangle.Arrange(rect4);
                }
                if (IsFillIndicatorVisible)
                {
                    _fillIndicatorBounds = ArrangeFillRect(finalSize);
                }
                (OwingViewport.Sheet as SpreadView).UpdateTouchSelectionGripper();
            }
            return finalSize;
        }

        Color GetSelectionBorderColor(Color selectionBorderColor, string themeColor)
        {
            if (themeColor != null)
            {
                return OwingViewport.Sheet.Worksheet.Workbook.GetThemeColor(themeColor);
            }
            return selectionBorderColor;
        }

        internal bool IsMouseInFillIndicator(Point viewportPoint)
        {
            return _fillIndicatorBounds.Contains(viewportPoint);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _selectionBorderColor = GetSelectionBorderColor(OwingViewport.Sheet.Worksheet.SelectionBorderColor, OwingViewport.Sheet.Worksheet.SelectionBorderThemeColor);
            if (_currentBorderColor != _selectionBorderColor)
            {
                _currentBorderColor = _selectionBorderColor;
                SetSelectionStyle(_currentBorderColor);
            }
            _leftRectangle.Measure(new Size(Thickness, availableSize.Height));
            _rightRectangle.Measure(new Size(Thickness, availableSize.Height));
            _topRectangle.Measure(new Size(availableSize.Width, Thickness));
            _bottomRectangle.Measure(new Size(availableSize.Width, Thickness));
            _fillIndicator.Measure(new Size(5.0, 5.0));
            return availableSize;
        }

        internal async void ResetSelectionFrameStoke()
        {
            _leftRectangle.Fill = DefaultSelectionBorderBrush;
            _topRectangle.Fill = DefaultSelectionBorderBrush;
            _rightRectangle.Fill = DefaultSelectionBorderBrush;
            _bottomRectangle.Fill = DefaultSelectionBorderBrush;
            await base.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(InvalidateMeasure));
        }

        internal async void SetSelectionFrameStroke(Brush brush)
        {
            _leftRectangle.Fill = brush;
            _topRectangle.Fill = brush;
            _rightRectangle.Fill = brush;
            _bottomRectangle.Fill = brush;
            await base.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(InvalidateMeasure));
        }

        void SetSelectionStyle(Color currentBorderColor)
        {
            SolidColorBrush brush = new SolidColorBrush(currentBorderColor);
            _leftRectangle.Fill = brush;
            _topRectangle.Fill = brush;
            _rightRectangle.Fill = brush;
            _bottomRectangle.Fill = brush;
            _fillIndicator.Fill = new SolidColorBrush(currentBorderColor);
        }

        public Rect FillIndicatorBounds
        {
            get { return _fillIndicatorBounds; }
        }

        public FillIndicatorPosition FillIndicatorPosition
        {
            get { return _fillIndicatorPosition; }
            set
            {
                if (FillIndicatorPosition != value)
                {
                    _fillIndicatorPosition = value;
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                }
            }
        }

        public bool IsBottomVisible
        {
            get { return _isBottomVisible; }
            set
            {
                if (_isBottomVisible != value)
                {
                    _isBottomVisible = value;
                    if (value)
                    {
                        _bottomRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _bottomRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsFillIndicatorVisible
        {
            get { return _isFillIndicatorVisible; }
            set
            {
                if (IsFillIndicatorVisible != value)
                {
                    _isFillIndicatorVisible = value;
                    _fillIndicator.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        public bool IsLeftVisible
        {
            get { return _isLeftVisible; }
            set
            {
                if (_isLeftVisible != value)
                {
                    _isLeftVisible = value;
                    if (value)
                    {
                        _leftRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _leftRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsRightVisible
        {
            get { return _isRightVisible; }
            set
            {
                if (_isRightVisible != value)
                {
                    _isRightVisible = value;
                    if (value)
                    {
                        _rightRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _rightRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsTopVisible
        {
            get { return _isTopVisible; }
            set
            {
                if (_isTopVisible != value)
                {
                    _isTopVisible = value;
                    if (value)
                    {
                        _topRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _topRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public GcViewport OwingViewport { get; set; }

        public double Thickness
        {
            get { return _thickNess; }
            set
            {
                if (Thickness != value)
                {
                    _thickNess = value;
                    _leftRectangle.Width = value;
                    _rightRectangle.Width = value;
                    _topRectangle.Height = value;
                    _bottomRectangle.Height = value;
                }
            }
        }
    }
}

