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
        private FrameworkElement _placeElement;
        private PopupDirection _popDirection;
        private Windows.UI.Xaml.Controls.Primitives.Popup _popup;
        private Grid _popupChildGrid;
        private Border _popupContentHost;
        private Canvas _popupOutsideCanvas;
        private Windows.Foundation.Point _relativePoint;
        private const double BackgroundSize = 10000.0;

        public PopupHelper(Windows.UI.Xaml.Controls.Primitives.Popup popup)
        {
            this._popup = popup;
            this._popupChildGrid = new Grid();
            this._popup.Child = this._popupChildGrid;
            this._popupOutsideCanvas = new Canvas();
            this._popupChildGrid.Children.Add(this._popupOutsideCanvas);
            this._popupContentHost = new Border();
            this._popupContentHost.Style = null;
            this._popupChildGrid.Children.Add(this._popupContentHost);
            Canvas canvas = this._popupOutsideCanvas;
            canvas.PointerPressed += OnOutsideCanvasMouseLeftButtonDown;
            Border border = this._popupContentHost;
            border.SizeChanged += OnPopupContentHostSizeChanged;
        }

        private Windows.Foundation.Point CalcPlacementPosition(Windows.Foundation.Point basePoint, Windows.Foundation.Size popSize, PopupDirection popDirection)
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
            this._popupContentHost.Child = null;
            this._popup.IsOpen = false;
        }

        private void OnOutsideCanvasMouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            if (object.ReferenceEquals(e.OriginalSource, this._popupOutsideCanvas))
            {
                this.Close();
            }
        }

        private void OnPopupContentHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Windows.Foundation.Point basePoint = this._relativePoint;
            basePoint = this.CalcPlacementPosition(basePoint, new Windows.Foundation.Size(this._popupContentHost.ActualWidth, this._popupContentHost.ActualHeight), this._popDirection);
            basePoint.X += 5.0;
            if (basePoint.X < 0.0)
            {
                basePoint.X = 0.0;
            }
            if (basePoint.Y < 0.0)
            {
                basePoint.Y = 0.0;
            }
            double num = (basePoint.X + this._popupContentHost.ActualWidth) - Window.Current.Bounds.Width;
            if (num > 0.0)
            {
                basePoint.X -= num;
                if (basePoint.X < 0.0)
                {
                    basePoint.X = 0.0;
                }
            }
            double num2 = (basePoint.Y + this._popupContentHost.ActualHeight) - Window.Current.Bounds.Height;
            if (num2 > 0.0)
            {
                basePoint.Y -= num2;
                if (basePoint.Y < 0.0)
                {
                    basePoint.Y = 0.0;
                }
            }
            this._popup.HorizontalOffset = Math.Floor(basePoint.X);
            this._popup.VerticalOffset = Math.Floor(basePoint.Y);
        }

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Windows.Foundation.Point relativePoint)
        {
            this.ShowAsModal(placeElement, contentElement, relativePoint, PopupDirection.BottomLeft);
        }

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Windows.Foundation.Point relativePoint, PopupDirection popDirection)
        {
            this.ShowAsModal(placeElement, contentElement, relativePoint, popDirection, true, true);
        }

        public void ShowAsModal(FrameworkElement placeElement, Control contentElement, Windows.Foundation.Point relativePoint, PopupDirection popDirection, bool capture, bool contentFocus)
        {
            this._popupContentHost.Child = contentElement;
            this._placeElement = placeElement;
            this._relativePoint = relativePoint;
            this._popDirection = popDirection;
            Dt.Cells.Data.UIAdaptor.InvokeSync(delegate
            {
                this._popupContentHost.Background = new SolidColorBrush(Colors.Transparent);
                this._popupOutsideCanvas.Background = new SolidColorBrush(Colors.Transparent);
            });
            this._popupOutsideCanvas.Margin = new Windows.UI.Xaml.Thickness(-10000.0);
            placeElement.ReleasePointerCaptures();
            this._popup.IsOpen = true;
            if (contentFocus)
            {
                contentElement.Focus(FocusState.Programmatic);
            }
        }

        internal Windows.Foundation.Point Location
        {
            get { return new Windows.Foundation.Point(this._popup.HorizontalOffset, this._popup.VerticalOffset); }
        }

        internal Windows.Foundation.Size Size
        {
            get
            {
                Control child = this._popupContentHost.Child as Control;
                return new Windows.Foundation.Size(child.ActualWidth, child.ActualHeight);
            }
        }
    }
}

