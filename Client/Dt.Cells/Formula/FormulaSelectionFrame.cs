#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FormulaSelectionFrame : Panel
    {
        Line _bottom;
        Rectangle _bottomBackground;
        bool _canChangeBoundsByUI = true;
        bool _flag;
        Line _innerBottom;
        Line _innerLeft;
        Line _innerRight;
        Line _innerTop;
        bool _isBottomVisible;
        bool _isFlickering = true;
        bool _isLeftVisible;
        bool _isMouseOvering;
        bool _isRightVisible;
        bool _isTopVisible;
        Line _left;
        Rectangle _leftBackground;
        Rectangle _leftBottom;
        Rectangle _leftTop;
        Line _right;
        Rectangle _rightBackground;
        Rectangle _rightBottom;
        Rectangle _rightTop;
        FormulaSelectionItem _selectionItem;
        Line _top;
        Rectangle _topBackground;

        public FormulaSelectionFrame(FormulaSelectionItem selectionItem)
        {
            CreateInnerBorders(selectionItem);
            CreateBackground(selectionItem);
            CreateCorners(selectionItem);
            CreateOutterBorders(selectionItem);
            _selectionItem = selectionItem;
            _selectionItem.PropertyChanged += new PropertyChangedEventHandler(SelectionItemPropertyChanged);
            _isFlickering = selectionItem.IsFlickering;
            _canChangeBoundsByUI = selectionItem.CanChangeBoundsByUI;
            UpdateVisibilities();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_isMouseOvering)
            {
                _innerLeft.Y2 = finalSize.Height;
                _innerLeft.Arrange(new Rect(0.0, 0.0, 1.0, finalSize.Height));
                _innerRight.Y2 = finalSize.Height;
                _innerRight.Arrange(new Rect(finalSize.Width - 1.0, 0.0, 1.0, finalSize.Height));
                _innerTop.X2 = finalSize.Width;
                _innerTop.Arrange(new Rect(0.0, 0.0, finalSize.Width, 1.0));
                _innerBottom.X2 = finalSize.Width;
                _innerBottom.Arrange(new Rect(0.0, finalSize.Height - 1.0, finalSize.Width, 1.0));
                _leftBackground.Arrange(new Rect(1.0, 0.0, 2.0, finalSize.Height));
                _rightBackground.Arrange(new Rect(finalSize.Width - 3.0, 0.0, 2.0, finalSize.Height));
                _topBackground.Arrange(new Rect(0.0, 1.0, finalSize.Width, 2.0));
                _bottomBackground.Arrange(new Rect(0.0, finalSize.Height - 3.0, finalSize.Width, 2.0));
            }
            else
            {
                _innerLeft.Y2 = finalSize.Height;
                _innerLeft.Arrange(new Rect(1.0, 0.0, 1.0, finalSize.Height));
                _innerRight.Y2 = finalSize.Height;
                _innerRight.Arrange(new Rect(finalSize.Width - 2.0, 0.0, 1.0, finalSize.Height));
                _innerTop.X2 = finalSize.Width;
                _innerTop.Arrange(new Rect(0.0, 1.0, finalSize.Width, 1.0));
                _innerBottom.X2 = finalSize.Width;
                _innerBottom.Arrange(new Rect(0.0, finalSize.Height - 2.0, finalSize.Width, 1.0));
                _leftBackground.Arrange(new Rect(0.0, 0.0, 1.0, finalSize.Height));
                _rightBackground.Arrange(new Rect(finalSize.Width - 1.0, 0.0, 1.0, finalSize.Height));
                _topBackground.Arrange(new Rect(0.0, 0.0, finalSize.Width, 1.0));
                _bottomBackground.Arrange(new Rect(0.0, finalSize.Height - 1.0, finalSize.Width, 1.0));
            }
            double width = 5.0;
            double height = 5.0;
            _leftTop.Arrange(new Rect(-1.0, -1.0, width, height));
            _rightTop.Arrange(new Rect((finalSize.Width - width) + 1.0, -1.0, width, height));
            _leftBottom.Arrange(new Rect(-1.0, (finalSize.Height - height) + 1.0, width, height));
            _rightBottom.Arrange(new Rect((finalSize.Width - width) + 1.0, (finalSize.Height - height) + 1.0, width, height));
            if (_isMouseOvering)
            {
                _left.Y2 = finalSize.Height;
                _left.Arrange(new Rect(1.0, 0.0, 1.0, finalSize.Height));
                _right.Y2 = finalSize.Height;
                _right.Arrange(new Rect(finalSize.Width - 2.0, 0.0, 1.0, finalSize.Height));
                _top.X2 = finalSize.Width;
                _top.Arrange(new Rect(0.0, 1.0, finalSize.Width, 1.0));
                _bottom.X2 = finalSize.Width;
                _bottom.Arrange(new Rect(0.0, finalSize.Height - 2.0, finalSize.Width, 1.0));
            }
            else
            {
                _left.Y2 = finalSize.Height;
                _left.Arrange(new Rect(0.0, 0.0, 1.0, finalSize.Height));
                _right.Y2 = finalSize.Height;
                _right.Arrange(new Rect(finalSize.Width - 1.0, 0.0, 1.0, finalSize.Height));
                _top.X2 = finalSize.Width;
                _top.Arrange(new Rect(0.0, 0.0, finalSize.Width, 1.0));
                _bottom.X2 = finalSize.Width;
                _bottom.Arrange(new Rect(0.0, finalSize.Height - 1.0, finalSize.Width, 1.0));
            }
            return base.ArrangeOverride(finalSize);
        }

        void CreateBackground(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(selectionItem.Color);
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 1.0;
            rectangle.Height = base.Height;
            rectangle.Fill = brush;
            _leftBackground = rectangle;
            Rectangle rectangle2 = new Rectangle();
            rectangle2.Width = 1.0;
            rectangle2.Height = base.Height;
            rectangle2.Fill = brush;
            _rightBackground = rectangle2;
            Rectangle rectangle3 = new Rectangle();
            rectangle3.Width = base.Width;
            rectangle3.Height = 1.0;
            rectangle3.Fill = brush;
            _topBackground = rectangle3;
            Rectangle rectangle4 = new Rectangle();
            rectangle4.Width = base.Width;
            rectangle4.Height = 1.0;
            rectangle4.Fill = brush;
            _bottomBackground = rectangle4;
            base.Children.Add(_leftBackground);
            base.Children.Add(_rightBackground);
            base.Children.Add(_topBackground);
            base.Children.Add(_bottomBackground);
        }

        void CreateCorners(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(selectionItem.Color);
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 5.0;
            rectangle.Height = 5.0;
            rectangle.Fill = brush;
            _leftTop = rectangle;
            Rectangle rectangle2 = new Rectangle();
            rectangle2.Width = 5.0;
            rectangle2.Height = 5.0;
            rectangle2.Fill = brush;
            _rightTop = rectangle2;
            Rectangle rectangle3 = new Rectangle();
            rectangle3.Width = 5.0;
            rectangle3.Height = 5.0;
            rectangle3.Fill = brush;
            _leftBottom = rectangle3;
            Rectangle rectangle4 = new Rectangle();
            rectangle4.Width = 5.0;
            rectangle4.Height = 5.0;
            rectangle4.Fill = brush;
            _rightBottom = rectangle4;
            base.Children.Add(_leftTop);
            base.Children.Add(_rightTop);
            base.Children.Add(_leftBottom);
            base.Children.Add(_rightBottom);
        }

        DoubleCollection CreateDoubleCollection()
        {
            return new DoubleCollection { 1.0, 1.0 };
        }

        void CreateInnerBorders(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            Line line = new Line();
            line.Stroke = brush;
            line.StrokeThickness = 4.0;
            line.StrokeDashArray = CreateDoubleCollection();
            line.StrokeDashOffset = -0.25;
            _innerLeft = line;
            Line line2 = new Line();
            line2.Stroke = brush;
            line2.StrokeThickness = 4.0;
            line2.StrokeDashArray = CreateDoubleCollection();
            line2.StrokeDashOffset = 0.25;
            _innerRight = line2;
            Line line3 = new Line();
            line3.Stroke = brush;
            line3.StrokeThickness = 4.0;
            line3.StrokeDashArray = CreateDoubleCollection();
            line3.StrokeDashOffset = -0.25;
            _innerTop = line3;
            Line line4 = new Line();
            line4.Stroke = brush;
            line4.StrokeThickness = 4.0;
            line4.StrokeDashArray = CreateDoubleCollection();
            line4.StrokeDashOffset = 0.25;
            _innerBottom = line4;
            base.Children.Add(_innerLeft);
            base.Children.Add(_innerRight);
            base.Children.Add(_innerTop);
            base.Children.Add(_innerBottom);
        }

        void CreateOutterBorders(FormulaSelectionItem selectionItem)
        {
            Color color = selectionItem.Color;
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0xff, (byte)(0xff - color.R), (byte)(0xff - color.G), (byte)(0xff - color.B)));
            Line line = new Line();
            line.Stroke = brush;
            line.StrokeThickness = 4.0;
            line.StrokeDashArray = CreateDoubleCollection();
            _left = line;
            Line line2 = new Line();
            line2.Stroke = brush;
            line2.StrokeThickness = 4.0;
            line2.StrokeDashArray = CreateDoubleCollection();
            _right = line2;
            Line line3 = new Line();
            line3.Stroke = brush;
            line3.StrokeThickness = 4.0;
            line3.StrokeDashArray = CreateDoubleCollection();
            _top = line3;
            Line line4 = new Line();
            line4.Stroke = brush;
            line4.StrokeThickness = 4.0;
            line4.StrokeDashArray = CreateDoubleCollection();
            _bottom = line4;
            base.Children.Add(_left);
            base.Children.Add(_right);
            base.Children.Add(_top);
            base.Children.Add(_bottom);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in base.Children)
            {
                element.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        public void OnTick()
        {
            if (_selectionItem.IsFlickering)
            {
                if (_flag)
                {
                    _left.StrokeDashOffset = 1.0;
                    _top.StrokeDashOffset = 1.0;
                    _right.StrokeDashOffset = 1.0;
                    _bottom.StrokeDashOffset = 1.0;
                    _innerLeft.StrokeDashOffset = 0.75;
                    _innerRight.StrokeDashOffset = -0.75;
                    _innerTop.StrokeDashOffset = 0.75;
                    _innerBottom.StrokeDashOffset = -0.75;
                }
                else
                {
                    _left.StrokeDashOffset = 0.0;
                    _top.StrokeDashOffset = 0.0;
                    _right.StrokeDashOffset = 0.0;
                    _bottom.StrokeDashOffset = 0.0;
                    _innerLeft.StrokeDashOffset = -0.25;
                    _innerRight.StrokeDashOffset = 0.25;
                    _innerTop.StrokeDashOffset = -0.25;
                    _innerBottom.StrokeDashOffset = 0.25;
                }
                _flag = !_flag;
            }
        }

        void SelectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Range")
            {
                UIElement parent = VisualTreeHelper.GetParent(this) as UIElement;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
            else if (e.PropertyName == "Color")
            {
                Color color = _selectionItem.Color;
                SolidColorBrush brush = new SolidColorBrush(color);
                _leftBackground.Fill = brush;
                _rightBackground.Fill = brush;
                _topBackground.Fill = brush;
                _bottomBackground.Fill = brush;
                _leftTop.Fill = brush;
                _rightTop.Fill = brush;
                _leftBottom.Fill = brush;
                _rightBottom.Fill = brush;
                brush = new SolidColorBrush(Color.FromArgb(0xff, (byte)(0xff - color.R), (byte)(0xff - color.G), (byte)(0xff - color.B)));
                _left.Stroke = brush;
                _right.Stroke = brush;
                _top.Stroke = brush;
                _bottom.Stroke = brush;
            }
            else if (e.PropertyName == "IsFlickering")
            {
                IsFlickering = _selectionItem.IsFlickering;
            }
            else if (e.PropertyName == "IsMouseOver")
            {
                IsMouseOvering = _selectionItem.IsMouseOver;
            }
            else if (e.PropertyName == "CanChangeBoundsByUI")
            {
                CanChangeBoundsByUI = _selectionItem.CanChangeBoundsByUI;
            }
        }

        void UpdateVisibilities()
        {
            if (IsLeftVisible)
            {
                _leftBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _left.Visibility = Visibility.Visible;
                    _innerLeft.Visibility = Visibility.Visible;
                }
                else
                {
                    _left.Visibility = Visibility.Collapsed;
                    _innerLeft.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _leftBackground.Visibility = Visibility.Collapsed;
                _left.Visibility = Visibility.Collapsed;
                _innerLeft.Visibility = Visibility.Collapsed;
            }
            if (IsRightVisible)
            {
                _rightBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _right.Visibility = Visibility.Visible;
                    _innerRight.Visibility = Visibility.Visible;
                }
                else
                {
                    _right.Visibility = Visibility.Collapsed;
                    _innerRight.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _rightBackground.Visibility = Visibility.Collapsed;
                _right.Visibility = Visibility.Collapsed;
                _innerRight.Visibility = Visibility.Collapsed;
            }
            if (IsTopVisible)
            {
                _topBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _top.Visibility = Visibility.Visible;
                    _innerTop.Visibility = Visibility.Visible;
                }
                else
                {
                    _top.Visibility = Visibility.Collapsed;
                    _innerTop.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _topBackground.Visibility = Visibility.Collapsed;
                _top.Visibility = Visibility.Collapsed;
                _innerTop.Visibility = Visibility.Collapsed;
            }
            if (IsBottomVisible)
            {
                _bottomBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _bottom.Visibility = Visibility.Visible;
                    _innerBottom.Visibility = Visibility.Visible;
                }
                else
                {
                    _bottom.Visibility = Visibility.Collapsed;
                    _innerBottom.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _bottomBackground.Visibility = Visibility.Collapsed;
                _bottom.Visibility = Visibility.Collapsed;
                _innerBottom.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsLeftVisible) && IsTopVisible)
            {
                _leftTop.Visibility = Visibility.Visible;
            }
            else
            {
                _leftTop.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsRightVisible) && IsTopVisible)
            {
                _rightTop.Visibility = Visibility.Visible;
            }
            else
            {
                _rightTop.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsLeftVisible) && IsBottomVisible)
            {
                _leftBottom.Visibility = Visibility.Visible;
            }
            else
            {
                _leftBottom.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsRightVisible) && IsBottomVisible)
            {
                _rightBottom.Visibility = Visibility.Visible;
            }
            else
            {
                _rightBottom.Visibility = Visibility.Collapsed;
            }
        }

        public bool CanChangeBoundsByUI
        {
            get { return _canChangeBoundsByUI; }
            set
            {
                if (_canChangeBoundsByUI != value)
                {
                    _canChangeBoundsByUI = value;
                    UpdateVisibilities();
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
                    UpdateVisibilities();
                }
            }
        }

        public bool IsFlickering
        {
            get { return _isFlickering; }
            set
            {
                if (_isFlickering != value)
                {
                    _isFlickering = value;
                    UpdateVisibilities();
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
                    UpdateVisibilities();
                }
            }
        }

        public bool IsMouseOvering
        {
            get { return _isMouseOvering; }
            set
            {
                if (_isMouseOvering != value)
                {
                    _isMouseOvering = value;
                    if (_isMouseOvering)
                    {
                        _leftBackground.Width = 2.0;
                        _rightBackground.Width = 2.0;
                        _topBackground.Height = 2.0;
                        _bottomBackground.Height = 2.0;
                    }
                    else
                    {
                        _leftBackground.Width = 1.0;
                        _rightBackground.Width = 1.0;
                        _topBackground.Height = 1.0;
                        _bottomBackground.Height = 1.0;
                    }
                    base.InvalidateArrange();
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
                    UpdateVisibilities();
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
                    UpdateVisibilities();
                }
            }
        }

        public FormulaSelectionItem SelectionItem
        {
            get { return _selectionItem; }
        }
    }
}

