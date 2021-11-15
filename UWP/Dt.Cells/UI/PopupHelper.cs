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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal class PopupHelper
    {
        FrameworkElement _placeElement;
        PopupDirection _popDirection;
        Windows.UI.Xaml.Controls.Primitives.Popup _popup;
        Grid _popupChildGrid;
        Border _popupContentHost;
        Canvas _popupOutsideCanvas;
        Point _relativePoint;
        const double BackgroundSize = 10000.0;

        public PopupHelper(Windows.UI.Xaml.Controls.Primitives.Popup popup)
        {
            _popup = popup;
            _popupChildGrid = new Grid();

            _popupOutsideCanvas = new Canvas();
            _popupOutsideCanvas.PointerPressed += OnOutsideCanvasMouseLeftButtonDown;
            _popupChildGrid.Children.Add(_popupOutsideCanvas);

            _popupContentHost = new Border();
            _popupContentHost.SizeChanged += OnPopupContentHostSizeChanged;
            _popupChildGrid.Children.Add(_popupContentHost);

            _popup.Child = _popupChildGrid;
        }

        Point CalcPlacementPosition(Point basePoint, Size popSize, PopupDirection popDirection)
        {
            Point point = new Point();
            switch (popDirection)
            {
                case PopupDirection.UpperLeft:
                    point.X = basePoint.X - popSize.Width;
                    point.Y = basePoint.Y - popSize.Height;
                    return point;

                case PopupDirection.UpperRight:
                    point.X = basePoint.X;
                    point.Y = basePoint.Y - popSize.Height;
                    return point;

                case PopupDirection.BottomLeft:
                    point.X = basePoint.X - popSize.Width;
                    point.Y = basePoint.Y;
                    return point;

                case PopupDirection.BottomRight:
                    point.X = basePoint.X;
                    point.Y = basePoint.Y;
                    return point;
            }
            return point;
        }

        public void Close()
        {
            _popupContentHost.Child = null;
            _popup.IsOpen = false;
        }

        void OnOutsideCanvasMouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            if (object.ReferenceEquals(e.OriginalSource, _popupOutsideCanvas))
            {
                Close();
            }
        }

        void OnPopupContentHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Point basePoint = _relativePoint;
            basePoint = CalcPlacementPosition(basePoint, new Size(_popupContentHost.ActualWidth, _popupContentHost.ActualHeight), _popDirection);
            basePoint.X += 5.0;
            if (basePoint.X < 0.0)
            {
                basePoint.X = 0.0;
            }
            if (basePoint.Y < 0.0)
            {
                basePoint.Y = 0.0;
            }
            double num = (basePoint.X + _popupContentHost.ActualWidth) - Window.Current.Bounds.Width;
            if (num > 0.0)
            {
                basePoint.X -= num;
                if (basePoint.X < 0.0)
                {
                    basePoint.X = 0.0;
                }
            }
            double num2 = (basePoint.Y + _popupContentHost.ActualHeight) - Window.Current.Bounds.Height;
            if (num2 > 0.0)
            {
                basePoint.Y -= num2;
                if (basePoint.Y < 0.0)
                {
                    basePoint.Y = 0.0;
                }
            }
            _popup.HorizontalOffset = Math.Floor(basePoint.X);
            _popup.VerticalOffset = Math.Floor(basePoint.Y);
        }

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Point relativePoint)
        {
            ShowAsModal(placeElement, contentElement, relativePoint, PopupDirection.BottomLeft);
        }

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Point relativePoint, PopupDirection popDirection)
        {
            _popupContentHost.Child = contentElement;
            _placeElement = placeElement;
            // 相对整个视图的位置
            _relativePoint = placeElement.TransformToVisual(null).Transform(relativePoint);
            _popDirection = popDirection;
            _popupContentHost.Background = new SolidColorBrush(Colors.Transparent);
            _popupOutsideCanvas.Background = new SolidColorBrush(Colors.Transparent);
            _popupOutsideCanvas.Margin = new Thickness(-10000.0);
            placeElement.ReleasePointerCaptures();
            _popup.IsOpen = true;
        }

        internal Point Location
        {
            get { return new Point(_popup.HorizontalOffset, _popup.VerticalOffset); }
        }

        internal Size Size
        {
            get
            {
                Control child = _popupContentHost.Child as Control;
                return new Size(child.ActualWidth, child.ActualHeight);
            }
        }
    }
}

