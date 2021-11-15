#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
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
        double _minWidth;
        Windows.UI.Xaml.Controls.Primitives.Popup _popup;
        TooltipControl _toolTipBlock;
        Grid _tooltipFocusableElement;

        public TooltipPopupHelper(Excel p_excel, double minWidth = -1.0)
        {
            _popup = p_excel.ToolTipPopup;
            _tooltipFocusableElement = new Grid();
            new Border();
            new LinearGradientBrush().StartPoint = new Point();
            _toolTipBlock = new TooltipControl();
            _toolTipBlock.Margin = new Thickness(0.0, 0.0, 5.0, 5.0);
            _popup.Child = _tooltipFocusableElement;
            _tooltipFocusableElement.Children.Add(_toolTipBlock);
            _minWidth = minWidth;
            Grid grid = _tooltipFocusableElement;
            grid.SizeChanged += _tooltipFocusableElement_SizeChanged;
            if (_minWidth > 0.0)
            {
                _toolTipBlock.MinWidth = minWidth;
            }
        }

        void _tooltipFocusableElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        public void CloseTooltip()
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
            }
        }

        public void ShowTooltip(string text, double offsetX, double offsetY)
        {
            if (!string.IsNullOrEmpty(text) && _popup.IsOpen)
            {
                _toolTipBlock.Text = text;
                _popup.HorizontalOffset = offsetX;
                _popup.VerticalOffset = offsetY;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                _toolTipBlock.Text = text;
                _popup.HorizontalOffset = offsetX;
                _popup.VerticalOffset = offsetY;
                _popup.IsOpen = true;
            }
            else if (_popup.IsOpen)
            {
                _popup.IsOpen = false;
            }
        }
    }
}

