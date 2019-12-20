#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class HeaderFooterPanel : Panel
    {
        /// <summary>
        /// 依次水平摆放
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0.0;
            double height = 0.0;
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement element = (UIElement)Children[i];
                element.Measure(availableSize);
                width += element.DesiredSize.Width;
                height = Math.Max(height, element.DesiredSize.Height);
            }
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double x = 0.0;
            for (int i = 0; i < Children.Count; i++)
            {
                Point pt = new Point(x, 0.0);
                ((UIElement)Children[i]).Arrange(new Rect(pt.X, pt.Y, arrangeSize.Width, arrangeSize.Height));
                x += ((UIElement)Children[i]).DesiredSize.Width;
            }
            return base.ArrangeOverride(arrangeSize);
        }
    }
}

