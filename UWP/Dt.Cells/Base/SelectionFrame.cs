#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class SelectionFrame : Panel
    {
        static Rect _rcEmpty = new Rect();
        SolidColorBrush _brush;
        Rectangle _bottomRectangle;
        Rectangle _fillIndicator;
        Rect _fillIndicatorBounds = Rect.Empty;
        FillIndicatorPosition _fillIndicatorPosition;
        Rectangle _leftRectangle;
        Rectangle _rightRectangle;
        double _thickness = 3.0;
        Rectangle _topRectangle;

        public SelectionFrame(CellsPanel owingViewport)
        {
            OwingViewport = owingViewport;
            _brush = new SolidColorBrush(owingViewport.Excel.ActiveSheet.SelectionBorderColor);
            _leftRectangle = new Rectangle();
            _leftRectangle.Fill = _brush;
            _leftRectangle.Stroke = null;
            _leftRectangle.StrokeThickness = 0.0;
            _leftRectangle.Width = 3.0;
            Children.Add(_leftRectangle);

            _topRectangle = new Rectangle();
            _topRectangle.Fill = _brush;
            _topRectangle.Stroke = null;
            _topRectangle.StrokeThickness = 0.0;
            _topRectangle.Height = 3.0;
            Children.Add(_topRectangle);

            _rightRectangle = new Rectangle();
            _rightRectangle.Fill = _brush;
            _rightRectangle.Stroke = null;
            _rightRectangle.StrokeThickness = 0.0;
            _rightRectangle.Width = 3.0;
            Children.Add(_rightRectangle);

            _bottomRectangle = new Rectangle();
            _bottomRectangle.Fill = _brush;
            _bottomRectangle.Stroke = null;
            _bottomRectangle.StrokeThickness = 0.0;
            _bottomRectangle.Height = 3.0;
            Children.Add(_bottomRectangle);

            // 右下矩形标志
            _fillIndicator = new Rectangle();
            _fillIndicator.Fill = _brush;
            Children.Add(_fillIndicator);
            _fillIndicatorPosition = FillIndicatorPosition.BottomRight;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _leftRectangle.Measure(new Size(Thickness, availableSize.Height));
            _rightRectangle.Measure(new Size(Thickness, availableSize.Height));
            _topRectangle.Measure(new Size(availableSize.Width, Thickness));
            _bottomRectangle.Measure(new Size(availableSize.Width, Thickness));
            _fillIndicator.Measure(new Size(5.0, 5.0));
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rc;
            bool indicatorVisible = IsFillIndicatorVisible;

            if (indicatorVisible && FillIndicatorPosition == FillIndicatorPosition.BottomLeft)
            {
                double height = Math.Max(finalSize.Height - 4.0, 0.0);
                rc = new Rect(0.0, 0.0, Thickness, height);
            }
            else
            {
                rc = new Rect(0.0, 0.0, Thickness, finalSize.Height);
            }
            _leftRectangle.Arrange(rc);

            if (indicatorVisible && _fillIndicatorPosition == FillIndicatorPosition.TopRight)
            {
                double num2 = Math.Max(finalSize.Height - 5.0 - 4.0, 0.0);
                rc = new Rect(finalSize.Width - Thickness, 9.0, Thickness, num2);
            }
            else if (indicatorVisible && _fillIndicatorPosition == FillIndicatorPosition.BottomRight)
            {
                double num3 = Math.Max(finalSize.Height - 5.0, 0.0);
                rc = new Rect(finalSize.Width - Thickness, 0.0, Thickness, num3);
            }
            else
            {
                rc = new Rect(finalSize.Width - Thickness, 0.0, Thickness, finalSize.Height);
            }
            _rightRectangle.Arrange(rc);

            if (indicatorVisible && FillIndicatorPosition == FillIndicatorPosition.TopRight)
            {
                double width = Math.Max(finalSize.Width - 4.0, 0.0);
                rc = new Rect(0.0, 0.0, width, Thickness);
            }
            else
            {
                rc = new Rect(0.0, 0.0, finalSize.Width, Thickness);
            }
            _topRectangle.Arrange(rc);

            if (indicatorVisible && _fillIndicatorPosition == FillIndicatorPosition.BottomLeft)
            {
                double num5 = Math.Max(finalSize.Width - 5.0 - 4.0, 0.0);
                rc = new Rect(9.0, finalSize.Height - Thickness, num5, Thickness);
            }
            else if (indicatorVisible && _fillIndicatorPosition == FillIndicatorPosition.BottomRight)
            {
                double num6 = Math.Max(finalSize.Width - 5.0, 0.0);
                rc = new Rect(0.0, finalSize.Height - Thickness, num6, Thickness);
            }
            else
            {
                rc = new Rect(0.0, finalSize.Height - Thickness, finalSize.Width, Thickness);
            }
            _bottomRectangle.Arrange(rc);

            if (indicatorVisible)
            {
                if (_fillIndicatorPosition == FillIndicatorPosition.TopRight)
                {
                    _fillIndicatorBounds = new Rect((finalSize.Width - 5.0) + 1.0, 3.0, 5.0, 5.0);
                }
                else if (_fillIndicatorPosition == FillIndicatorPosition.BottomLeft)
                {
                    _fillIndicatorBounds = new Rect(3.0, (finalSize.Height - 5.0) + 1.0, 5.0, 5.0);
                }
                else
                {
                    _fillIndicatorBounds = new Rect((finalSize.Width - 5.0) + 1.0, (finalSize.Height - 5.0) + 1.0, 5.0, 5.0);
                }
            }
            else
            {
                _fillIndicatorBounds = _rcEmpty;
            }
            _fillIndicator.Arrange(_fillIndicatorBounds);

            OwingViewport.Excel.ArrangeSelectionGripper();
            return finalSize;
        }

        internal bool IsMouseInFillIndicator(Point viewportPoint)
        {
            return IsFillIndicatorVisible && _fillIndicatorBounds.Contains(viewportPoint);
        }

        internal void ResetSelectionFrameStoke()
        {
            _leftRectangle.Fill = _brush;
            _topRectangle.Fill = _brush;
            _rightRectangle.Fill = _brush;
            _bottomRectangle.Fill = _brush;
        }

        internal void SetSelectionFrameStroke(Brush brush)
        {
            _leftRectangle.Fill = brush;
            _topRectangle.Fill = brush;
            _rightRectangle.Fill = brush;
            _bottomRectangle.Fill = brush;
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
                    InvalidateMeasure();
                    InvalidateArrange();
                }
            }
        }

        public void HideAll()
        {
            IsLeftVisible = false;
            IsRightVisible = false;
            IsTopVisible = false;
            IsBottomVisible = false;
            IsFillIndicatorVisible = false;
        }

        public bool IsBottomVisible
        {
            get { return _bottomRectangle.Fill != null; }
            set
            {
                if (value && _bottomRectangle.Fill == null)
                    _bottomRectangle.Fill = _brush;
                else if (!value && _bottomRectangle.Fill != null)
                    _bottomRectangle.Fill = null;
            }
        }

        public bool IsFillIndicatorVisible
        {
            get { return _fillIndicator.Fill != null; }
            set
            {
                if (value && _fillIndicator.Fill == null)
                    _fillIndicator.Fill = _brush;
                else if (!value && _fillIndicator.Fill != null)
                    _fillIndicator.Fill = null;
            }
        }

        public bool IsLeftVisible
        {
            get { return _leftRectangle.Fill != null; }
            set
            {
                if (value && _leftRectangle.Fill == null)
                    _leftRectangle.Fill = _brush;
                else if (!value && _leftRectangle.Fill != null)
                    _leftRectangle.Fill = null;
            }
        }

        public bool IsRightVisible
        {
            get { return _rightRectangle.Fill != null; }
            set
            {
                if (value && _rightRectangle.Fill == null)
                    _rightRectangle.Fill = _brush;
                else if (!value && _rightRectangle.Fill != null)
                    _rightRectangle.Fill = null;
            }
        }

        public bool IsTopVisible
        {
            get { return _topRectangle.Fill != null; }
            set
            {
                if (value && _topRectangle.Fill == null)
                    _topRectangle.Fill = _brush;
                else if (!value && _topRectangle.Fill != null)
                    _topRectangle.Fill = null;
            }
        }

        public CellsPanel OwingViewport { get; set; }

        public double Thickness
        {
            get { return _thickness; }
            set
            {
                if (_thickness != value)
                {
                    _thickness = value;
                    _leftRectangle.Width = value;
                    _rightRectangle.Width = value;
                    _topRectangle.Height = value;
                    _bottomRectangle.Height = value;
                }
            }
        }
    }
}

