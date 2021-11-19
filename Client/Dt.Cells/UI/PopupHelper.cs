#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal class PopupHelper
    {
        PopupDirection _popDirection;
        Microsoft.UI.Xaml.Controls.Primitives.Popup _popup;
        Grid _popupChildGrid;
        Border _popupContentHost;
        Canvas _popupOutsideCanvas;
        Point _relativePoint;
        Excel _excel;

        public PopupHelper(Microsoft.UI.Xaml.Controls.Primitives.Popup popup, Excel p_excel)
        {
            _popup = popup;
            _excel = p_excel;
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

            // WinUI
            double num = (basePoint.X + _popupContentHost.ActualWidth) - _excel.ActualWidth;
            if (num > 0.0)
            {
                basePoint.X -= num;
                if (basePoint.X < 0.0)
                {
                    basePoint.X = 0.0;
                }
            }
            double num2 = (basePoint.Y + _popupContentHost.ActualHeight) - _excel.ActualHeight;
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

