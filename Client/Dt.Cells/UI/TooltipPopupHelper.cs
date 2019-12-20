#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal class TooltipPopupHelper
    {
        private double _minWidth;
        private Windows.UI.Xaml.Controls.Primitives.Popup _popup;
        private TooltipControl _toolTipBlock;
        private Grid _tooltipFocusableElement;

        public TooltipPopupHelper(SheetView sheetView, double minWidth = -1.0)
        {
            this._popup = sheetView.ToolTipPopup;
            this._tooltipFocusableElement = new Grid();
            new Border();
            new LinearGradientBrush().StartPoint = new Windows.Foundation.Point();
            this._toolTipBlock = new TooltipControl();
            this._toolTipBlock.Margin = new Windows.UI.Xaml.Thickness(0.0, 0.0, 5.0, 5.0);
            this._popup.Child = this._tooltipFocusableElement;
            this._tooltipFocusableElement.Children.Add(this._toolTipBlock);
            this._minWidth = minWidth;
            Grid grid = this._tooltipFocusableElement;
            grid.SizeChanged += _tooltipFocusableElement_SizeChanged;
            if (this._minWidth > 0.0)
            {
                this._toolTipBlock.MinWidth = minWidth;
            }
        }

        private void _tooltipFocusableElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        public void CloseTooltip()
        {
            if (this._popup != null)
            {
                this._popup.IsOpen = false;
            }
        }

        public void ShowTooltip(string text, double offsetX, double offsetY)
        {
            if (!string.IsNullOrEmpty(text) && this._popup.IsOpen)
            {
                this._toolTipBlock.Text = text;
                this._popup.HorizontalOffset = offsetX;
                this._popup.VerticalOffset = offsetY;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                this._toolTipBlock.Text = text;
                this._popup.HorizontalOffset = offsetX;
                this._popup.VerticalOffset = offsetY;
                this._popup.IsOpen = true;
            }
            else if (this._popup.IsOpen)
            {
                this._popup.IsOpen = false;
            }
        }
    }
}

