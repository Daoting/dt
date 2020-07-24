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
        private Image _bottomCenterRect;
        private Image _bottomLeftRect;
        private Rectangle _bottomRect;
        private Image _bottomRightRect;
        private Border _contentPanel;
        private Border _defaultContent;
        private Dt.Cells.Data.FloatingObject _floatingObject;
        private Grid _glyphGrid;
        private Border _innerBorder;
        private bool _isSelected;
        private Rectangle _leftRect;
        private Image _middleLeftRect;
        private Image _middleRightRect;
        private Border _outBorder;
        private Rectangle _rightRect;
        private Image _topCentertRect;
        private Image _topLeftRect;
        private Rectangle _topRect;
        private Image _topRightRect;
        [ThreadStatic]
        private static readonly SolidColorBrush BlackGripBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xae, 0xaf, 0xaf));

        [ThreadStatic]
#if !UWP
        new
#endif
        static readonly SolidColorBrush BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xd8, 0xd8, 0xd8));
        [ThreadStatic]
        private static readonly SolidColorBrush BorderGapBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 0xeb, 0xeb, 0xeb));
        internal const int GrapStripSize = 8;

        public FloatingObjectContainer(Dt.Cells.Data.FloatingObject floatingObject, GcViewport parentViewport)
        {
            this._floatingObject = floatingObject;
            base.Visibility = floatingObject.Visible ? Visibility.Visible : Visibility.Collapsed;
            this.ParentViewport = parentViewport;
            Loaded += FloatingObjectContainer_Loaded;
            this._outBorder = new Border();
            this._outBorder.BorderBrush = BorderBrush;
            this._outBorder.Background = BorderGapBrush;
            this._outBorder.BorderThickness = new Windows.UI.Xaml.Thickness(2.0);
            this._outBorder.CornerRadius = new Windows.UI.Xaml.CornerRadius(5.0);
            base.Children.Add(this._outBorder);
            this._glyphGrid = new Grid();
            base.Children.Add(this._glyphGrid);
            RowDefinition definition = new RowDefinition();
            definition.Height = new Windows.UI.Xaml.GridLength(7.0);
            this._glyphGrid.RowDefinitions.Add(definition);
            this._glyphGrid.RowDefinitions.Add(new RowDefinition());
            RowDefinition definition2 = new RowDefinition();
            definition2.Height = new Windows.UI.Xaml.GridLength(7.0);
            this._glyphGrid.RowDefinitions.Add(definition2);
            ColumnDefinition definition3 = new ColumnDefinition();
            definition3.Width = new Windows.UI.Xaml.GridLength(7.0);
            this._glyphGrid.ColumnDefinitions.Add(definition3);
            this._glyphGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinition definition4 = new ColumnDefinition();
            definition4.Width = new Windows.UI.Xaml.GridLength(7.0);
            this._glyphGrid.ColumnDefinitions.Add(definition4);
            this._leftRect = new Rectangle();
            this._leftRect.Margin = new Windows.UI.Xaml.Thickness(-1.0, 0.0, 1.0, 0.0);
            this._leftRect.Fill = BorderGapBrush;
            this._glyphGrid.Children.Add(this._leftRect);
            Grid.SetColumn(this._leftRect, 0);
            Grid.SetRowSpan(this._leftRect, 3);
            this._topRect = new Rectangle();
            this._topRect.Margin = new Windows.UI.Xaml.Thickness(0.0, -1.0, 0.0, 1.0);
            this._topRect.Fill = BorderGapBrush;
            this._glyphGrid.Children.Add(this._topRect);
            Grid.SetRow(this._topRect, 0);
            Grid.SetColumnSpan(this._topRect, 3);
            this._rightRect = new Rectangle();
            this._rightRect.Margin = new Windows.UI.Xaml.Thickness(1.0, 0.0, -1.0, 0.0);
            this._rightRect.Fill = BorderGapBrush;
            this._glyphGrid.Children.Add(this._rightRect);
            Grid.SetColumn(this._rightRect, 2);
            Grid.SetRowSpan(this._rightRect, 3);
            this._bottomRect = new Rectangle();
            this._bottomRect.Margin = new Windows.UI.Xaml.Thickness(0.0, 1.0, 0.0, -1.0);
            this._bottomRect.Fill = BorderGapBrush;
            this._glyphGrid.Children.Add(this._bottomRect);
            Grid.SetRow(this._bottomRect, 2);
            Grid.SetColumnSpan(this._bottomRect, 3);
            this._topLeftRect = new Image();
            this._topLeftRect.Width = 7.0;
            this._topLeftRect.Height = 7.0;
            this._topLeftRect.Source = Dt.Cells.UI.SR.GetImage("TopLeftGrip.png");
            this._glyphGrid.Children.Add(this._topLeftRect);
            Grid.SetRow(this._topLeftRect, 0);
            Grid.SetColumn(this._topLeftRect, 0);
            this._topCentertRect = new Image();
            this._topCentertRect.Width = 15.0;
            this._topCentertRect.Height = 3.0;
            this._topCentertRect.Source = Dt.Cells.UI.SR.GetImage("TopCenterGrip.png");
            this._topCentertRect.VerticalAlignment = VerticalAlignment.Top;
            this._glyphGrid.Children.Add(this._topCentertRect);
            Grid.SetRow(this._topCentertRect, 0);
            Grid.SetColumn(this._topCentertRect, 1);
            this._topRightRect = new Image();
            this._topRightRect.Width = 7.0;
            this._topRightRect.Height = 7.0;
            this._topRightRect.Source = Dt.Cells.UI.SR.GetImage("TopRightGrip.png");
            this._glyphGrid.Children.Add(this._topRightRect);
            Grid.SetRow(this._topRightRect, 0);
            Grid.SetColumn(this._topRightRect, 2);
            this._middleLeftRect = new Image();
            this._middleLeftRect.Width = 3.0;
            this._middleLeftRect.Height = 15.0;
            this._middleLeftRect.Source = Dt.Cells.UI.SR.GetImage("MiddleLeftGrip.png");
            this._middleLeftRect.HorizontalAlignment = HorizontalAlignment.Left;
            this._glyphGrid.Children.Add(this._middleLeftRect);
            Grid.SetRow(this._middleLeftRect, 1);
            Grid.SetColumn(this._middleLeftRect, 0);
            this._middleRightRect = new Image();
            this._middleRightRect.Width = 3.0;
            this._middleRightRect.Height = 15.0;
            this._middleRightRect.Source = Dt.Cells.UI.SR.GetImage("MiddleRightGrip.png");
            this._middleRightRect.HorizontalAlignment = HorizontalAlignment.Right;
            this._glyphGrid.Children.Add(this._middleRightRect);
            Grid.SetRow(this._middleRightRect, 1);
            Grid.SetColumn(this._middleRightRect, 2);
            this._bottomLeftRect = new Image();
            this._bottomLeftRect.Width = 7.0;
            this._bottomLeftRect.Height = 7.0;
            this._bottomLeftRect.Source = Dt.Cells.UI.SR.GetImage("BottomLeftGrip.png");
            this._glyphGrid.Children.Add(this._bottomLeftRect);
            Grid.SetRow(this._bottomLeftRect, 2);
            Grid.SetColumn(this._bottomLeftRect, 0);
            this._bottomCenterRect = new Image();
            this._bottomCenterRect.Width = 15.0;
            this._bottomCenterRect.Height = 3.0;
            this._bottomCenterRect.Source = Dt.Cells.UI.SR.GetImage("BottomCenterGrip.png");
            this._bottomCenterRect.VerticalAlignment = VerticalAlignment.Bottom;
            this._glyphGrid.Children.Add(this._bottomCenterRect);
            Grid.SetRow(this._bottomCenterRect, 2);
            Grid.SetColumn(this._bottomCenterRect, 1);
            this._bottomRightRect = new Image();
            this._bottomRightRect.Width = 7.0;
            this._bottomRightRect.Height = 7.0;
            this._bottomRightRect.Source = Dt.Cells.UI.SR.GetImage("BottomRightGrip.png");
            this._glyphGrid.Children.Add(this._bottomRightRect);
            Grid.SetRow(this._bottomRightRect, 2);
            Grid.SetColumn(this._bottomRightRect, 2);
            this._innerBorder = new Border();
            this._innerBorder.BorderBrush = BorderBrush;
            this._innerBorder.BorderThickness = new Windows.UI.Xaml.Thickness(2.0);
            this._innerBorder.CornerRadius = new Windows.UI.Xaml.CornerRadius(10.0);
            base.Children.Add(this._innerBorder);
            this._contentPanel = new Border();
            this._contentPanel.Background = new SolidColorBrush(Colors.Transparent);
            base.Children.Add(this._contentPanel);
            this._defaultContent = new Border();
            this._contentPanel.Child = this._defaultContent;
            this.UpdateElements(false);
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this._outBorder.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            Windows.Foundation.Point location = new Windows.Foundation.Point(3.0, 3.0);
            Windows.Foundation.Size size = new Windows.Foundation.Size(finalSize.Width - 6.0, finalSize.Height - 6.0);
            this._glyphGrid.Arrange(new Windows.Foundation.Rect(location, size));
            Windows.Foundation.Point point2 = new Windows.Foundation.Point(6.0, 6.0);
            Windows.Foundation.Size size2 = new Windows.Foundation.Size(finalSize.Width - 12.0, finalSize.Height - 12.0);
            this._innerBorder.Arrange(new Windows.Foundation.Rect(point2, size2));
            Windows.Foundation.Point point3 = new Windows.Foundation.Point(7.0, 7.0);
            Windows.Foundation.Size size3 = new Windows.Foundation.Size(finalSize.Width - 14.0, finalSize.Height - 14.0);
            this._contentPanel.Arrange(new Windows.Foundation.Rect(point3, size3));
            return base.ArrangeOverride(finalSize);
        }

        private void FloatingObjectContainer_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsSelected = this._floatingObject.IsSelected;
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._outBorder.Measure(availableSize);
            Windows.Foundation.Size size = new Windows.Foundation.Size(availableSize.Width - 6.0, availableSize.Height - 6.0);
            this._glyphGrid.Measure(size);
            Windows.Foundation.Size size2 = new Windows.Foundation.Size(availableSize.Width - 12.0, availableSize.Height - 12.0);
            this._innerBorder.Measure(size2);
            Windows.Foundation.Size size3 = new Windows.Foundation.Size(availableSize.Width - 14.0, availableSize.Height - 14.0);
            this._contentPanel.Measure(size3);
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
                    base.Visibility = this._floatingObject.Visible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        internal void RefreshIsSelected()
        {
            if (this.FloatingObject != null)
            {
                this.IsSelected = this.FloatingObject.IsSelected;
            }
        }

        private void UpdateElements(bool isSelected)
        {
            if (isSelected)
            {
                this._outBorder.Background = new SolidColorBrush(Colors.Transparent);
                this._outBorder.BorderBrush = BorderBrush;
                this._contentPanel.CornerRadius = new Windows.UI.Xaml.CornerRadius(10.0);
                this._contentPanel.Background = new SolidColorBrush(Colors.Transparent);
                this._topLeftRect.Visibility = Visibility.Visible;
                this._topCentertRect.Visibility = Visibility.Visible;
                this._topRightRect.Visibility = Visibility.Visible;
                this._middleLeftRect.Visibility = Visibility.Visible;
                this._middleRightRect.Visibility = Visibility.Visible;
                this._bottomLeftRect.Visibility = Visibility.Visible;
                this._bottomCenterRect.Visibility = Visibility.Visible;
                this._bottomRightRect.Visibility = Visibility.Visible;
                this._innerBorder.Visibility = Visibility.Visible;
                this._leftRect.Visibility = Visibility.Visible;
                this._topRect.Visibility = Visibility.Visible;
                this._rightRect.Visibility = Visibility.Visible;
                this._bottomRect.Visibility = Visibility.Visible;
            }
            else
            {
                this._outBorder.Background = new SolidColorBrush(Colors.Transparent);
                this._outBorder.BorderBrush = null;
                this._contentPanel.CornerRadius = new Windows.UI.Xaml.CornerRadius(0.0);
                this._contentPanel.Background = null;
                this._topLeftRect.Visibility = Visibility.Collapsed;
                this._topCentertRect.Visibility = Visibility.Collapsed;
                this._topRightRect.Visibility = Visibility.Collapsed;
                this._middleLeftRect.Visibility = Visibility.Collapsed;
                this._middleRightRect.Visibility = Visibility.Collapsed;
                this._bottomLeftRect.Visibility = Visibility.Collapsed;
                this._bottomCenterRect.Visibility = Visibility.Collapsed;
                this._bottomRightRect.Visibility = Visibility.Collapsed;
                this._innerBorder.Visibility = Visibility.Collapsed;
                this._leftRect.Visibility = Visibility.Collapsed;
                this._topRect.Visibility = Visibility.Collapsed;
                this._rightRect.Visibility = Visibility.Collapsed;
                this._bottomRect.Visibility = Visibility.Collapsed;
            }
        }

        public FrameworkElement Content
        {
            get { return  (this._contentPanel.Child as FrameworkElement); }
            set
            {
                if (value != null)
                {
                    if ((value.Parent != null) && (value.Parent is Border))
                    {
                        (value.Parent as Border).Child = null;
                    }
                    this._contentPanel.Child = value;
                }
            }
        }

        internal Dt.Cells.Data.FloatingObject FloatingObject
        {
            get { return  this._floatingObject; }
        }

        public bool IsSelected
        {
            get { return  this._isSelected; }
            set
            {
                if (value != this.IsSelected)
                {
                    this._isSelected = value;
                    this.UpdateElements(this._isSelected);
                }
            }
        }

        internal GcViewport ParentViewport { get; set; }

        internal virtual SpreadTheme Theme { get; set; }
    }
}

