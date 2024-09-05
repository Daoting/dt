#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 水平布局面板，可指定 宽度、star、auto，配合附加依赖属性 a:Ex.Width 使用
    /// </summary>
    public partial class HorPanel : Panel
    {
        // star对应的宽度
        double _unit;

        protected override Size MeasureOverride(Size availableSize)
        {
            double height = 0;
            double totalStar = 0;
            double leaveWidth = double.IsInfinity(availableSize.Width) ? Kit.ViewWidth : availableSize.Width;
            List<(UIElement, double)> ls = new List<(UIElement, double)>();

            foreach (var elem in Children)
            {
                var str = Ex.GetWidth(elem);

                // 指定宽度
                double width = 0;
                if (string.IsNullOrEmpty(str)
                    || double.TryParse(str, out width))
                {
                    if (string.IsNullOrEmpty(str))
                        width = 110;

                    width = leaveWidth > width ? width : leaveWidth;
                    elem.Measure(new Size(width, availableSize.Height));
                    if (elem.DesiredSize.Height > height)
                        height = elem.DesiredSize.Height;
                    leaveWidth = Math.Max(0, leaveWidth - width);
                    continue;
                }

                // star
                if (str.EndsWith('*'))
                {
                    if (str.Length == 1)
                    {
                        totalStar += 1;
                        ls.Add((elem, 1));
                    }
                    else if (double.TryParse(str.TrimEnd('*'), out var per))
                    {
                        totalStar += per;
                        ls.Add((elem, per));
                    }
                    continue;
                }

                // auto
                if (str.Equals("auto", StringComparison.OrdinalIgnoreCase))
                {
                    elem.Measure(new Size(leaveWidth, availableSize.Height));
                    if (elem.DesiredSize.Height > height)
                        height = elem.DesiredSize.Height;
                    leaveWidth = Math.Max(0, leaveWidth - elem.DesiredSize.Width);
                }
            }

            // 测量star的元素
            if (ls.Count > 0 && totalStar > 0)
            {
                _unit = leaveWidth > 0 ? leaveWidth / totalStar : 0;
                foreach (var item in ls)
                {
                    item.Item1.Measure(new Size(Math.Floor(_unit * item.Item2), availableSize.Height));
                    if (item.Item1.DesiredSize.Height > height)
                        height = item.Item1.DesiredSize.Height;
                }
            }

            return new Size(availableSize.Width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double left = 0;
            foreach (var elem in Children)
            {
                var str = Ex.GetWidth(elem);
                double width = 0;

                if (string.IsNullOrEmpty(str))
                {
                    width = 100;
                }
                else if (str.EndsWith('*'))
                {
                    if (str.Length == 1)
                    {
                        width = _unit;
                    }
                    else if (double.TryParse(str.TrimEnd('*'), out var per))
                    {
                        width = per * _unit;
                    }
                }
                else if (str.Equals("auto", StringComparison.OrdinalIgnoreCase))
                {
                    width = elem.DesiredSize.Width;
                }
                else
                {
                    double.TryParse(str, out width);
                }

                elem.Arrange(new Rect(left, 0, width, finalSize.Height));
                left += width;
            }
            return finalSize;
        }
    }
}