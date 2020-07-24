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
        Windows.Foundation.Point _relativePoint;
        const double BackgroundSize = 10000.0;

        public PopupHelper(Windows.UI.Xaml.Controls.Primitives.Popup popup)
        {
            _popup = popup;
            _popupChildGrid = new Grid();
            _popup.Child = _popupChildGrid;
            _popupOutsideCanvas = new Canvas();
            _popupChildGrid.Children.Add(_popupOutsideCanvas);
            _popupContentHost = new Border();
            _popupContentHost.Style = null;
            _popupChildGrid.Children.Add(_popupContentHost);
            Canvas canvas = _popupOutsideCanvas;
            canvas.PointerPressed += OnOutsideCanvasMouseLeftButtonDown;
            Border border = _popupContentHost;
            border.SizeChanged += OnPopupContentHostSizeChanged;
        }

        Windows.Foundation.Point CalcPlacementPosition(Windows.Foundation.Point basePoint, Windows.Foundation.Size popSize, PopupDirection popDirection)
        {
            Windows.Foundation.Point point = new Windows.Foundation.Point();
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
            Windows.Foundation.Point basePoint = _relativePoint;
            basePoint = CalcPlacementPosition(basePoint, new Windows.Foundation.Size(_popupContentHost.ActualWidth, _popupContentHost.ActualHeight), _popDirection);
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

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Windows.Foundation.Point relativePoint)
        {
            ShowAsModal(placeElement, contentElement, relativePoint, PopupDirection.BottomLeft);
        }

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Windows.Foundation.Point relativePoint, PopupDirection popDirection)
        {
            ShowAsModal(placeElement, contentElement, relativePoint, popDirection, true, true);
        }

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Windows.Foundation.Point relativePoint, PopupDirection popDirection, bool capture, bool contentFocus)
        {
            _popupContentHost.Child = contentElement;
            _placeElement = placeElement;
            _relativePoint = relativePoint;
            _popDirection = popDirection;
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate
            {
                _popupContentHost.Background = new SolidColorBrush(Colors.Transparent);
                _popupOutsideCanvas.Background = new SolidColorBrush(Colors.Transparent);
            });
            _popupOutsideCanvas.Margin = new Windows.UI.Xaml.Thickness(-10000.0);
            placeElement.ReleasePointerCaptures();
            _popup.IsOpen = true;
            if (contentFocus)
            {
                contentElement.Focus(FocusState.Programmatic);
            }
        }

        internal Windows.Foundation.Point Location
        {
            get { return new Windows.Foundation.Point(_popup.HorizontalOffset, _popup.VerticalOffset); }
        }

        internal Windows.Foundation.Size Size
        {
            get
            {
                Control child = _popupContentHost.Child as Control;
                return new Windows.Foundation.Size(child.ActualWidth, child.ActualHeight);
            }
        }
    }
}

