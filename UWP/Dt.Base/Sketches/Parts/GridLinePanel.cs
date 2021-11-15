#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 背景网格面板
    /// </summary>
    public partial class GridLinePanel : Panel
    {
        // 线间距
        const double _padding = 20.0;
        readonly List<Line> _horLines = new List<Line>();
        readonly List<Line> _verLines = new List<Line>();

        public GridLinePanel()
        {
            // 快很多
            CacheMode = new BitmapCache();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = finalSize.Width;
            double height = finalSize.Height;
            if (!double.IsInfinity(width)
                && !double.IsInfinity(height)
                && width > 0
                && height > 0)
            {
                Rect rc = new Rect(0, 0, width, height);
                bool horChanged = PrepareLines(rc, true);
                bool verChanged = PrepareLines(rc, false);
                int horCount = _horLines.Count;
                int verCount = _verLines.Count;
                if (horChanged && verCount > 0)
                {
                    for (int i = 0; i < verCount; i++)
                    {
                        _verLines[i].Y2 = height;
                    }
                }
                if (verChanged && horCount > 0)
                {
                    for (int i = 0; i < horCount; i++)
                    {
                        _horLines[i].X2 = width;
                    }
                }
            }
            return finalSize;
        }

        /// <summary>
        /// 准备网格线
        /// </summary>
        /// <param name="p_rc"></param>
        /// <param name="p_isHor"></param>
        bool PrepareLines(Rect p_rc, bool p_isHor)
        {   
            int count;
            List<Line> lines;
            if (p_isHor)
            {
                lines = _horLines;
                count = (int)Math.Ceiling(p_rc.Height / _padding) - 1;
            }
            else
            {
                lines = _verLines;
                count = (int)Math.Ceiling(p_rc.Width / _padding) - 1;
            }

            int newCount = count - lines.Count;
            if (newCount > 0)
            {
                // 原有子元素不够
                int start = lines.Count;
                for (int i = 0; i < newCount; i++)
                {
                    int index = i + start + 1;
                    Line line = new Line();
                    if (index % 5 == 0)
                    {
                        line.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xD6, 0xD6));
                        line.StrokeThickness = 1;
                    }
                    else
                    {
                        line.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xC3, 0xC3, 0xC3));
                        line.StrokeDashArray = new DoubleCollection() { 6.0, 6.0 };
                        line.StrokeThickness = 0.5;
                    }

                    double dist = index * _padding;
                    if (p_isHor)
                    {
                        line.X2 = p_rc.Width;
                        line.Y1 = line.Y2 = dist;
                    }
                    else
                    {
                        line.X1 = line.X2 = dist;
                        line.Y2 = p_rc.Height;
                    }
                    lines.Add(line);
                    Children.Add(line);
                }
            }
            else if (newCount < 0)
            {
                // 移除多余的子元素
                for (int i = 0; i < -newCount; i++)
                {
                    int index = lines.Count - 1;
                    Line line = lines[index];
                    Children.Remove(line);
                    lines.RemoveAt(index);
                }
            }
            return newCount != 0;
        }
    }
}
