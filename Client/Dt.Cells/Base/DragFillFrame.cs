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
        private Rectangle _bottomRectangle1;
        private Rectangle _bottomRectangle2;
        private Grid _indicator = new Grid();
        private bool _isBottomVisible = true;
        private bool _isLeftVisible = true;
        private bool _isRightVisible = true;
        private bool _isTopVisible = true;
        private Rectangle _leftRectangle1;
        private Rectangle _leftRectangle2;
        private Rectangle _rightRectangle1;
        private Rectangle _rightRectangle2;
        private Rectangle _topRectangle1;
        private Rectangle _topRectangle2;

        public DragFillFrame()
        {
            SolidColorBrush brush = null;
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate {
                brush = new SolidColorBrush(Colors.Black);
            });
            Rectangle rectangle = new Rectangle();
            rectangle.Stroke = brush;
            rectangle.StrokeThickness = 1.0;
            rectangle.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle.StrokeDashOffset = 0.5;
            this._leftRectangle1 = rectangle;
            Rectangle rectangle2 = new Rectangle();
            rectangle2.Stroke = brush;
            rectangle2.StrokeThickness = 1.0;
            rectangle2.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle2.StrokeDashOffset = 0.5;
            rectangle2.Margin = new Windows.UI.Xaml.Thickness(1.0);
            this._leftRectangle2 = rectangle2;
            Rectangle rectangle3 = new Rectangle();
            rectangle3.Stroke = brush;
            rectangle3.StrokeThickness = 1.0;
            rectangle3.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle3.StrokeDashOffset = 0.5;
            this._rightRectangle1 = rectangle3;
            Rectangle rectangle4 = new Rectangle();
            rectangle4.Stroke = brush;
            rectangle4.StrokeThickness = 1.0;
            rectangle4.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle4.StrokeDashOffset = 0.5;
            rectangle4.Margin = new Windows.UI.Xaml.Thickness(1.0);
            this._rightRectangle2 = rectangle4;
            Rectangle rectangle5 = new Rectangle();
            rectangle5.Stroke = brush;
            rectangle5.StrokeThickness = 1.0;
            rectangle5.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle5.StrokeDashOffset = 0.5;
            this._topRectangle1 = rectangle5;
            Rectangle rectangle6 = new Rectangle();
            rectangle6.Stroke = brush;
            rectangle6.StrokeThickness = 1.0;
            rectangle6.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle6.StrokeDashOffset = 0.5;
            rectangle6.Margin = new Windows.UI.Xaml.Thickness(1.0);
            this._topRectangle2 = rectangle6;
            Rectangle rectangle7 = new Rectangle();
            rectangle7.Stroke = brush;
            rectangle7.StrokeThickness = 1.0;
            rectangle7.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle7.StrokeDashOffset = 0.5;
            this._bottomRectangle1 = rectangle7;
            Rectangle rectangle8 = new Rectangle();
            rectangle8.Stroke = brush;
            rectangle8.StrokeThickness = 1.0;
            rectangle8.StrokeDashArray = new DoubleCollection { 1.0, 1.0 };
            rectangle8.StrokeDashOffset = 0.5;
            rectangle8.Margin = new Windows.UI.Xaml.Thickness(1.0);
            this._bottomRectangle2 = rectangle8;
            this._indicator.Children.Add(this._leftRectangle1);
            this._indicator.Children.Add(this._leftRectangle2);
            this._indicator.Children.Add(this._topRectangle1);
            this._indicator.Children.Add(this._topRectangle2);
            this._indicator.Children.Add(this._rightRectangle1);
            this._indicator.Children.Add(this._rightRectangle2);
            this._indicator.Children.Add(this._bottomRectangle1);
            this._indicator.Children.Add(this._bottomRectangle2);
            base.Children.Add(this._indicator);
            this.IsLeftVisibe = true;
            this.IsRightVisibe = true;
            this.IsTopVisibie = true;
            this.IsBottomVisibe = true;
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (!finalSize.IsEmpty)
            {
                Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
                Windows.Foundation.Rect rect2 = Windows.Foundation.Rect.Empty;
                Windows.Foundation.Rect rect3 = Windows.Foundation.Rect.Empty;
                Windows.Foundation.Rect rect4 = Windows.Foundation.Rect.Empty;
                if (this.IsLeftVisibe)
                {
                    empty = new Windows.Foundation.Rect(0.0, 0.0, this.Thickness, finalSize.Height);
                }
                if (this.IsTopVisibie)
                {
                    rect2 = new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, this.Thickness);
                }
                if (this.IsRightVisibe)
                {
                    rect3 = new Windows.Foundation.Rect(finalSize.Width - this.Thickness, 0.0, this.Thickness, finalSize.Height);
                }
                if (this.IsBottomVisibe)
                {
                    rect4 = new Windows.Foundation.Rect(0.0, finalSize.Height - this.Thickness, finalSize.Width, this.Thickness);
                }
                if (this.IsLeftVisibe)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = empty;
                    this._leftRectangle1.Clip = geometry;
                    RectangleGeometry geometry2 = new RectangleGeometry();
                    geometry2.Rect = empty;
                    this._leftRectangle2.Clip = geometry2;
                }
                if (this.IsRightVisibe)
                {
                    RectangleGeometry geometry3 = new RectangleGeometry();
                    geometry3.Rect = rect3;
                    this._rightRectangle1.Clip = geometry3;
                    RectangleGeometry geometry4 = new RectangleGeometry();
                    geometry4.Rect = rect3;
                    this._rightRectangle2.Clip = geometry4;
                }
                if (this.IsTopVisibie)
                {
                    RectangleGeometry geometry5 = new RectangleGeometry();
                    geometry5.Rect = rect2;
                    this._topRectangle1.Clip = geometry5;
                    RectangleGeometry geometry6 = new RectangleGeometry();
                    geometry6.Rect = rect2;
                    this._topRectangle2.Clip = geometry6;
                }
                if (this.IsBottomVisibe)
                {
                    RectangleGeometry geometry7 = new RectangleGeometry();
                    geometry7.Rect = rect4;
                    this._bottomRectangle1.Clip = geometry7;
                    RectangleGeometry geometry8 = new RectangleGeometry();
                    geometry8.Rect = rect4;
                    this._bottomRectangle2.Clip = geometry8;
                }
                this._indicator.Arrange(new Windows.Foundation.Rect(new Windows.Foundation.Point(0.0, 0.0), finalSize));
            }
            return finalSize;
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._indicator.Measure(availableSize);
            return this._indicator.DesiredSize;
        }

        public bool IsBottomVisibe
        {
            get { return  this._isBottomVisible; }
            set
            {
                if (this.IsBottomVisibe != value)
                {
                    this._isBottomVisible = value;
                    if (value)
                    {
                        this._bottomRectangle1.Visibility = Visibility.Visible;
                        this._bottomRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this._bottomRectangle1.Visibility = Visibility.Collapsed;
                        this._bottomRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsLeftVisibe
        {
            get { return  this._isLeftVisible; }
            set
            {
                if (this.IsLeftVisibe != value)
                {
                    this._isLeftVisible = value;
                    if (value)
                    {
                        this._leftRectangle1.Visibility = Visibility.Visible;
                        this._leftRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this._leftRectangle1.Visibility = Visibility.Collapsed;
                        this._leftRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsRightVisibe
        {
            get { return  this._isRightVisible; }
            set
            {
                if (this.IsRightVisibe != value)
                {
                    this._isRightVisible = value;
                    if (value)
                    {
                        this._rightRectangle1.Visibility = Visibility.Visible;
                        this._rightRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this._rightRectangle1.Visibility = Visibility.Collapsed;
                        this._rightRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsTopVisibie
        {
            get { return  this._isTopVisible; }
            set
            {
                if (this.IsTopVisibie != value)
                {
                    this._isTopVisible = value;
                    if (value)
                    {
                        this._topRectangle1.Visibility = Visibility.Visible;
                        this._topRectangle2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this._topRectangle1.Visibility = Visibility.Collapsed;
                        this._topRectangle2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private double Thickness
        {
            get { return  3.0; }
        }
    }
}

