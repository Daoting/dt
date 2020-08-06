#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class DragFillFrame : Panel
    {
        Rectangle _bottomRectangle1;
        Rectangle _bottomRectangle2;
        Grid _indicator = new Grid();
        bool _isBottomVisible = true;
        bool _isLeftVisible = true;
        bool _isRightVisible = true;
        bool _isTopVisible = true;
        Rectangle _leftRectangle1;
        Rectangle _leftRectangle2;
        Rectangle _rightRectangle1;
        Rectangle _rightRectangle2;
        Rectangle _topRectangle1;
        Rectangle _topRectangle2;

        public DragFillFrame()
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            Rectangle rectangle = new Rectangle();
            rectangle.Stroke = brush;
            rectangle.StrokeThickness = 1.0;
            rectangle.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle.StrokeDashOffset = 0.5;
            _leftRectangle1 = rectangle;
            Rectangle rectangle2 = new Rectangle();
            rectangle2.Stroke = brush;
            rectangle2.StrokeThickness = 1.0;
            rectangle2.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle2.StrokeDashOffset = 0.5;
            rectangle2.Margin = new Windows.UI.Xaml.Thickness(1.0);
            _leftRectangle2 = rectangle2;
            Rectangle rectangle3 = new Rectangle();
            rectangle3.Stroke = brush;
            rectangle3.StrokeThickness = 1.0;
            rectangle3.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle3.StrokeDashOffset = 0.5;
            _rightRectangle1 = rectangle3;
            Rectangle rectangle4 = new Rectangle();
            rectangle4.Stroke = brush;
            rectangle4.StrokeThickness = 1.0;
            rectangle4.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle4.StrokeDashOffset = 0.5;
            rectangle4.Margin = new Windows.UI.Xaml.Thickness(1.0);
            _rightRectangle2 = rectangle4;
            Rectangle rectangle5 = new Rectangle();
            rectangle5.Stroke = brush;
            rectangle5.StrokeThickness = 1.0;
            rectangle5.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle5.StrokeDashOffset = 0.5;
            _topRectangle1 = rectangle5;
            Rectangle rectangle6 = new Rectangle();
            rectangle6.Stroke = brush;
            rectangle6.StrokeThickness = 1.0;
            rectangle6.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle6.StrokeDashOffset = 0.5;
            rectangle6.Margin = new Windows.UI.Xaml.Thickness(1.0);
            _topRectangle2 = rectangle6;
            Rectangle rectangle7 = new Rectangle();
            rectangle7.Stroke = brush;
            rectangle7.StrokeThickness = 1.0;
            rectangle7.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle7.StrokeDashOffset = 0.5;
            _bottomRectangle1 = rectangle7;
            Rectangle rectangle8 = new Rectangle();
            rectangle8.Stroke = brush;
            rectangle8.StrokeThickness = 1.0;
            rectangle8.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle8.StrokeDashOffset = 0.5;
            rectangle8.Margin = new Windows.UI.Xaml.Thickness(1.0);
            _bottomRectangle2 = rectangle8;
            _indicator.Children.Add(_leftRectangle1);
            _indicator.Children.Add(_leftRectangle2);
            _indicator.Children.Add(_topRectangle1);
            _indicator.Children.Add(_topRectangle2);
            _indicator.Children.Add(_rightRectangle1);
            _indicator.Children.Add(_rightRectangle2);
            _indicator.Children.Add(_bottomRectangle1);
            _indicator.Children.Add(_bottomRectangle2);
            base.Children.Add(_indicator);
            IsLeftVisibe = true;
            IsRightVisibe = true;
            IsTopVisibie = true;
            IsBottomVisibe = true;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!finalSize.IsEmpty)
            {
                Rect empty = Rect.Empty;
                Rect rect2 = Rect.Empty;
                Rect rect3 = Rect.Empty;
                Rect rect4 = Rect.Empty;
                if (IsLeftVisibe)
                {
                    empty = new Rect(0.0, 0.0, Thickness, finalSize.Height);
                }
                if (IsTopVisibie)
                {
                    rect2 = new Rect(0.0, 0.0, finalSize.Width, Thickness);
                }
                if (IsRightVisibe)
                {
                    rect3 = new Rect(finalSize.Width - Thickness, 0.0, Thickness, finalSize.Height);
                }
                if (IsBottomVisibe)
                {
                    rect4 = new Rect(0.0, finalSize.Height - Thickness, finalSize.Width, Thickness);
                }
                if (IsLeftVisibe)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = empty;
                    _leftRectangle1.Clip = geometry;
                    RectangleGeometry geometry2 = new RectangleGeometry();
                    geometry2.Rect = empty;
                    _leftRectangle2.Clip = geometry2;
                }
                if (IsRightVisibe)
                {
                    RectangleGeometry geometry3 = new RectangleGeometry();
                    geometry3.Rect = rect3;
                    _rightRectangle1.Clip = geometry3;
                    RectangleGeometry geometry4 = new RectangleGeometry();
                    geometry4.Rect = rect3;
                    _rightRectangle2.Clip = geometry4;
                }
                if (IsTopVisibie)
                {
                    RectangleGeometry geometry5 = new RectangleGeometry();
                    geometry5.Rect = rect2;
                    _topRectangle1.Clip = geometry5;
                    RectangleGeometry geometry6 = new RectangleGeometry();
                    geometry6.Rect = rect2;
                    _topRectangle2.Clip = geometry6;
                }
                if (IsBottomVisibe)
                {
                    RectangleGeometry geometry7 = new RectangleGeometry();
                    geometry7.Rect = rect4;
                    _bottomRectangle1.Clip = geometry7;
                    RectangleGeometry geometry8 = new RectangleGeometry();
                    geometry8.Rect = rect4;
                    _bottomRectangle2.Clip = geometry8;
                }
                _indicator.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _indicator.Measure(availableSize);
            return _indicator.DesiredSize;
        }

        public bool IsBottomVisibe
        {
            get { return  _isBottomVisible; }
            set
            {
                if (IsBottomVisibe != value)
                {
                    _isBottomVisible = value;
                    if (value)
                    {
                        _bottomRectangle1.Visibility = Visibility.Visible;
                        _bottomRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _bottomRectangle1.Visibility = Visibility.Collapsed;
                        _bottomRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsLeftVisibe
        {
            get { return  _isLeftVisible; }
            set
            {
                if (IsLeftVisibe != value)
                {
                    _isLeftVisible = value;
                    if (value)
                    {
                        _leftRectangle1.Visibility = Visibility.Visible;
                        _leftRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _leftRectangle1.Visibility = Visibility.Collapsed;
                        _leftRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsRightVisibe
        {
            get { return  _isRightVisible; }
            set
            {
                if (IsRightVisibe != value)
                {
                    _isRightVisible = value;
                    if (value)
                    {
                        _rightRectangle1.Visibility = Visibility.Visible;
                        _rightRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _rightRectangle1.Visibility = Visibility.Collapsed;
                        _rightRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsTopVisibie
        {
            get { return  _isTopVisible; }
            set
            {
                if (IsTopVisibie != value)
                {
                    _isTopVisible = value;
                    if (value)
                    {
                        _topRectangle1.Visibility = Visibility.Visible;
                        _topRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _topRectangle1.Visibility = Visibility.Collapsed;
                        _topRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        double Thickness
        {
            get { return  3.0; }
        }
    }
}

