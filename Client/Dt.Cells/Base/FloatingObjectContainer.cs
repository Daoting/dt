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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FloatingObjectContainer : Panel
    {
        Image _bottomCenterRect;
        Image _bottomLeftRect;
        Rectangle _bottomRect;
        Image _bottomRightRect;
        Border _contentPanel;
        Border _defaultContent;
        Dt.Cells.Data.FloatingObject _floatingObject;
        Grid _glyphGrid;
        Border _innerBorder;
        bool _isSelected;
        Rectangle _leftRect;
        Image _middleLeftRect;
        Image _middleRightRect;
        Border _outBorder;
        Rectangle _rightRect;
        Image _topCentertRect;
        Image _topLeftRect;
        Rectangle _topRect;
        Image _topRightRect;
        [ThreadStatic]
        static readonly SolidColorBrush BlackGripBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xae, 0xaf, 0xaf));

        [ThreadStatic]
#if !UWP
        new
#endif
        static readonly SolidColorBrush BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xd8, 0xd8, 0xd8));
        [ThreadStatic]
        static readonly SolidColorBrush BorderGapBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 0xeb, 0xeb, 0xeb));
        internal const int GrapStripSize = 8;

        public FloatingObjectContainer(Dt.Cells.Data.FloatingObject floatingObject, GcViewport parentViewport)
        {
            _floatingObject = floatingObject;
            base.Visibility = floatingObject.Visible ? Visibility.Visible : Visibility.Collapsed;
            ParentViewport = parentViewport;
            Loaded += FloatingObjectContainer_Loaded;
            _outBorder = new Border();
            _outBorder.BorderBrush = BorderBrush;
            _outBorder.Background = BorderGapBrush;
            _outBorder.BorderThickness = new Windows.UI.Xaml.Thickness(2.0);
            _outBorder.CornerRadius = new Windows.UI.Xaml.CornerRadius(5.0);
            base.Children.Add(_outBorder);
            _glyphGrid = new Grid();
            base.Children.Add(_glyphGrid);
            RowDefinition definition = new RowDefinition();
            definition.Height = new Windows.UI.Xaml.GridLength(7.0);
            _glyphGrid.RowDefinitions.Add(definition);
            _glyphGrid.RowDefinitions.Add(new RowDefinition());
            RowDefinition definition2 = new RowDefinition();
            definition2.Height = new Windows.UI.Xaml.GridLength(7.0);
            _glyphGrid.RowDefinitions.Add(definition2);
            ColumnDefinition definition3 = new ColumnDefinition();
            definition3.Width = new Windows.UI.Xaml.GridLength(7.0);
            _glyphGrid.ColumnDefinitions.Add(definition3);
            _glyphGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinition definition4 = new ColumnDefinition();
            definition4.Width = new Windows.UI.Xaml.GridLength(7.0);
            _glyphGrid.ColumnDefinitions.Add(definition4);
            _leftRect = new Rectangle();
            _leftRect.Margin = new Windows.UI.Xaml.Thickness(-1.0, 0.0, 1.0, 0.0);
            _leftRect.Fill = BorderGapBrush;
            _glyphGrid.Children.Add(_leftRect);
            Grid.SetColumn(_leftRect, 0);
            Grid.SetRowSpan(_leftRect, 3);
            _topRect = new Rectangle();
            _topRect.Margin = new Windows.UI.Xaml.Thickness(0.0, -1.0, 0.0, 1.0);
            _topRect.Fill = BorderGapBrush;
            _glyphGrid.Children.Add(_topRect);
            Grid.SetRow(_topRect, 0);
            Grid.SetColumnSpan(_topRect, 3);
            _rightRect = new Rectangle();
            _rightRect.Margin = new Windows.UI.Xaml.Thickness(1.0, 0.0, -1.0, 0.0);
            _rightRect.Fill = BorderGapBrush;
            _glyphGrid.Children.Add(_rightRect);
            Grid.SetColumn(_rightRect, 2);
            Grid.SetRowSpan(_rightRect, 3);
            _bottomRect = new Rectangle();
            _bottomRect.Margin = new Windows.UI.Xaml.Thickness(0.0, 1.0, 0.0, -1.0);
            _bottomRect.Fill = BorderGapBrush;
            _glyphGrid.Children.Add(_bottomRect);
            Grid.SetRow(_bottomRect, 2);
            Grid.SetColumnSpan(_bottomRect, 3);
            _topLeftRect = new Image();
            _topLeftRect.Width = 7.0;
            _topLeftRect.Height = 7.0;
            _topLeftRect.Source = Dt.Cells.UI.SR.GetImage("TopLeftGrip.png");
            _glyphGrid.Children.Add(_topLeftRect);
            Grid.SetRow(_topLeftRect, 0);
            Grid.SetColumn(_topLeftRect, 0);
            _topCentertRect = new Image();
            _topCentertRect.Width = 15.0;
            _topCentertRect.Height = 3.0;
            _topCentertRect.Source = Dt.Cells.UI.SR.GetImage("TopCenterGrip.png");
            _topCentertRect.VerticalAlignment = VerticalAlignment.Top;
            _glyphGrid.Children.Add(_topCentertRect);
            Grid.SetRow(_topCentertRect, 0);
            Grid.SetColumn(_topCentertRect, 1);
            _topRightRect = new Image();
            _topRightRect.Width = 7.0;
            _topRightRect.Height = 7.0;
            _topRightRect.Source = Dt.Cells.UI.SR.GetImage("TopRightGrip.png");
            _glyphGrid.Children.Add(_topRightRect);
            Grid.SetRow(_topRightRect, 0);
            Grid.SetColumn(_topRightRect, 2);
            _middleLeftRect = new Image();
            _middleLeftRect.Width = 3.0;
            _middleLeftRect.Height = 15.0;
            _middleLeftRect.Source = Dt.Cells.UI.SR.GetImage("MiddleLeftGrip.png");
            _middleLeftRect.HorizontalAlignment = HorizontalAlignment.Left;
            _glyphGrid.Children.Add(_middleLeftRect);
            Grid.SetRow(_middleLeftRect, 1);
            Grid.SetColumn(_middleLeftRect, 0);
            _middleRightRect = new Image();
            _middleRightRect.Width = 3.0;
            _middleRightRect.Height = 15.0;
            _middleRightRect.Source = Dt.Cells.UI.SR.GetImage("MiddleRightGrip.png");
            _middleRightRect.HorizontalAlignment = HorizontalAlignment.Right;
            _glyphGrid.Children.Add(_middleRightRect);
            Grid.SetRow(_middleRightRect, 1);
            Grid.SetColumn(_middleRightRect, 2);
            _bottomLeftRect = new Image();
            _bottomLeftRect.Width = 7.0;
            _bottomLeftRect.Height = 7.0;
            _bottomLeftRect.Source = Dt.Cells.UI.SR.GetImage("BottomLeftGrip.png");
            _glyphGrid.Children.Add(_bottomLeftRect);
            Grid.SetRow(_bottomLeftRect, 2);
            Grid.SetColumn(_bottomLeftRect, 0);
            _bottomCenterRect = new Image();
            _bottomCenterRect.Width = 15.0;
            _bottomCenterRect.Height = 3.0;
            _bottomCenterRect.Source = Dt.Cells.UI.SR.GetImage("BottomCenterGrip.png");
            _bottomCenterRect.VerticalAlignment = VerticalAlignment.Bottom;
            _glyphGrid.Children.Add(_bottomCenterRect);
            Grid.SetRow(_bottomCenterRect, 2);
            Grid.SetColumn(_bottomCenterRect, 1);
            _bottomRightRect = new Image();
            _bottomRightRect.Width = 7.0;
            _bottomRightRect.Height = 7.0;
            _bottomRightRect.Source = Dt.Cells.UI.SR.GetImage("BottomRightGrip.png");
            _glyphGrid.Children.Add(_bottomRightRect);
            Grid.SetRow(_bottomRightRect, 2);
            Grid.SetColumn(_bottomRightRect, 2);
            _innerBorder = new Border();
            _innerBorder.BorderBrush = BorderBrush;
            _innerBorder.BorderThickness = new Windows.UI.Xaml.Thickness(2.0);
            _innerBorder.CornerRadius = new Windows.UI.Xaml.CornerRadius(10.0);
            base.Children.Add(_innerBorder);
            _contentPanel = new Border();
            _contentPanel.Background = new SolidColorBrush(Colors.Transparent);
            base.Children.Add(_contentPanel);
            _defaultContent = new Border();
            _contentPanel.Child = _defaultContent;
            UpdateElements(false);
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            _outBorder.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            Windows.Foundation.Point location = new Windows.Foundation.Point(3.0, 3.0);
            Windows.Foundation.Size size = new Windows.Foundation.Size(finalSize.Width - 6.0, finalSize.Height - 6.0);
            _glyphGrid.Arrange(new Windows.Foundation.Rect(location, size));
            Windows.Foundation.Point point2 = new Windows.Foundation.Point(6.0, 6.0);
            Windows.Foundation.Size size2 = new Windows.Foundation.Size(finalSize.Width - 12.0, finalSize.Height - 12.0);
            _innerBorder.Arrange(new Windows.Foundation.Rect(point2, size2));
            Windows.Foundation.Point point3 = new Windows.Foundation.Point(7.0, 7.0);
            Windows.Foundation.Size size3 = new Windows.Foundation.Size(finalSize.Width - 14.0, finalSize.Height - 14.0);
            _contentPanel.Arrange(new Windows.Foundation.Rect(point3, size3));
            return base.ArrangeOverride(finalSize);
        }

        void FloatingObjectContainer_Loaded(object sender, RoutedEventArgs e)
        {
            IsSelected = _floatingObject.IsSelected;
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            _outBorder.Measure(availableSize);
            Windows.Foundation.Size size = new Windows.Foundation.Size(availableSize.Width - 6.0, availableSize.Height - 6.0);
            _glyphGrid.Measure(size);
            Windows.Foundation.Size size2 = new Windows.Foundation.Size(availableSize.Width - 12.0, availableSize.Height - 12.0);
            _innerBorder.Measure(size2);
            Windows.Foundation.Size size3 = new Windows.Foundation.Size(availableSize.Width - 14.0, availableSize.Height - 14.0);
            _contentPanel.Measure(size3);
            return base.MeasureOverride(availableSize);
        }

        internal virtual void Refresh(object parameter)
        {
            if (parameter != null)
            {
                string property = null;
                if (parameter is ChartChangedEventArgs)
                {
                    property = (parameter as ChartChangedEventArgs).Property;
                }
                else if (parameter is FloatingObjectChangedEventArgs)
                {
                    property = (parameter as FloatingObjectChangedEventArgs).Property;
                }
                else if (parameter is PictureChangedEventArgs)
                {
                    property = (parameter as PictureChangedEventArgs).Property;
                }
                if (property == "Visible")
                {
                    base.Visibility = _floatingObject.Visible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        internal void RefreshIsSelected()
        {
            if (FloatingObject != null)
            {
                IsSelected = FloatingObject.IsSelected;
            }
        }

        void UpdateElements(bool isSelected)
        {
            if (isSelected)
            {
                _outBorder.Background = new SolidColorBrush(Colors.Transparent);
                _outBorder.BorderBrush = BorderBrush;
                _contentPanel.CornerRadius = new Windows.UI.Xaml.CornerRadius(10.0);
                _contentPanel.Background = new SolidColorBrush(Colors.Transparent);
                _topLeftRect.Visibility = Visibility.Visible;
                _topCentertRect.Visibility = Visibility.Visible;
                _topRightRect.Visibility = Visibility.Visible;
                _middleLeftRect.Visibility = Visibility.Visible;
                _middleRightRect.Visibility = Visibility.Visible;
                _bottomLeftRect.Visibility = Visibility.Visible;
                _bottomCenterRect.Visibility = Visibility.Visible;
                _bottomRightRect.Visibility = Visibility.Visible;
                _innerBorder.Visibility = Visibility.Visible;
                _leftRect.Visibility = Visibility.Visible;
                _topRect.Visibility = Visibility.Visible;
                _rightRect.Visibility = Visibility.Visible;
                _bottomRect.Visibility = Visibility.Visible;
            }
            else
            {
                _outBorder.Background = new SolidColorBrush(Colors.Transparent);
                _outBorder.BorderBrush = null;
                _contentPanel.CornerRadius = new Windows.UI.Xaml.CornerRadius(0.0);
                _contentPanel.Background = null;
                _topLeftRect.Visibility = Visibility.Collapsed;
                _topCentertRect.Visibility = Visibility.Collapsed;
                _topRightRect.Visibility = Visibility.Collapsed;
                _middleLeftRect.Visibility = Visibility.Collapsed;
                _middleRightRect.Visibility = Visibility.Collapsed;
                _bottomLeftRect.Visibility = Visibility.Collapsed;
                _bottomCenterRect.Visibility = Visibility.Collapsed;
                _bottomRightRect.Visibility = Visibility.Collapsed;
                _innerBorder.Visibility = Visibility.Collapsed;
                _leftRect.Visibility = Visibility.Collapsed;
                _topRect.Visibility = Visibility.Collapsed;
                _rightRect.Visibility = Visibility.Collapsed;
                _bottomRect.Visibility = Visibility.Collapsed;
            }
        }

        public FrameworkElement Content
        {
            get { return  (_contentPanel.Child as FrameworkElement); }
            set
            {
                if (value != null)
                {
                    if ((value.Parent != null) && (value.Parent is Border))
                    {
                        (value.Parent as Border).Child = null;
                    }
                    _contentPanel.Child = value;
                }
            }
        }

        internal Dt.Cells.Data.FloatingObject FloatingObject
        {
            get { return  _floatingObject; }
        }

        public bool IsSelected
        {
            get { return  _isSelected; }
            set
            {
                if (value != IsSelected)
                {
                    _isSelected = value;
                    UpdateElements(_isSelected);
                }
            }
        }

        internal GcViewport ParentViewport { get; set; }

        internal virtual SpreadTheme Theme { get; set; }
    }
}

