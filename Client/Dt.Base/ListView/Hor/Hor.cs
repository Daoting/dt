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
    /// 表格式列表的布局面板
    /// </summary>
    public partial class Hor : Panel
    {
        #region 静态内容
        public readonly static DependencyProperty ItemMarginProperty = DependencyProperty.Register(
            "ItemMargin",
            typeof(Thickness),
            typeof(Hor),
            new PropertyMetadata(new Thickness(10, 4, 10, 4)));

        static Thickness _rightLine = new Thickness(0, 0, 1, 0);
        #endregion

        // star对应的宽度
        double _unit;

        /// <summary>
        /// 获取设置项的边距，默认(10, 4, 10, 4)
        /// </summary>
        public Thickness ItemMargin
        {
            get { return (Thickness)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }

        
        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            double height = 0;
            double totalStar = 0;
            double leaveWidth = availableSize.Width;
            List<(UIElement, double)> ls = new List<(UIElement, double)>();

            foreach (var elem in Children)
            {
                if (elem is Dot dot)
                {
                    if (dot.ReadLocalValue(Dot.BorderBrushProperty) == DependencyProperty.UnsetValue)
                    {
                        dot.BorderBrush = Res.浅灰2;
                        dot.BorderThickness = _rightLine;
                    }

                    if (dot.ReadLocalValue(Dot.PaddingProperty) == DependencyProperty.UnsetValue)
                    {
                        dot.Padding = ItemMargin;
                    }
                }
                else if (elem is StackPanel sp)
                {
                }

                var str = Ex.GetWidth(elem);

                // star
                if (!string.IsNullOrEmpty(str) && str.EndsWith('*'))
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

                double width;
                if (string.IsNullOrEmpty(str))
                {
                    width = 100;
                }
                else if (!double.TryParse(str, out width))
                {
                    continue;
                }

                width = leaveWidth > width ? width : leaveWidth;
                elem.Measure(new Size(width, availableSize.Height));
                if (elem.DesiredSize.Height > height)
                    height = elem.DesiredSize.Height;
                leaveWidth = Math.Max(0, leaveWidth - width);
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
                else
                {
                    double.TryParse(str, out width);
                }

                elem.Arrange(new Rect(left, 0, width, finalSize.Height));
                left += width;
            }
            return finalSize;
        }
        #endregion
    }
}