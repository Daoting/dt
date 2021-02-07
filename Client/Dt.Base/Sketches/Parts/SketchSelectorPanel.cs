#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class SketchSelectorPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FrameworkElement element in Children)
            {
                if (element is Rectangle)
                {
                    element.Arrange(new Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
                    continue;
                }

                Size size = new Size(Math.Ceiling(element.DesiredSize.Width), Math.Ceiling(element.DesiredSize.Height));
                if (element.Name == "PART_Link")
                {
                    element.Arrange(new Rect(-50 - (size.Width / 2.0), finalSize.Height - (size.Height / 2.0), size.Width, size.Height));
                    continue;
                }

                double x = 0.0;
                double y = 0.0;
                switch (element.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        x = -size.Width / 2.0;
                        break;

                    case HorizontalAlignment.Center:
                        x = (finalSize.Width / 2.0) - (size.Width / 2.0);
                        break;

                    case HorizontalAlignment.Right:
                        x = finalSize.Width - (size.Width / 2.0);
                        break;
                }

                switch (element.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        y = -size.Height / 2.0;
                        break;

                    case VerticalAlignment.Center:
                        y = (finalSize.Height / 2.0) - (size.Height / 2.0);
                        break;

                    case VerticalAlignment.Bottom:
                        y = finalSize.Height - (size.Height / 2.0);
                        break;
                }

                if (element.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    size.Width = finalSize.Width;
                }
                if (element.VerticalAlignment == VerticalAlignment.Stretch)
                {
                    size.Height = finalSize.Height;
                }
                element.Arrange(new Rect(x, y, size.Width, size.Height));
            }
            return finalSize;
        }
    }
}
