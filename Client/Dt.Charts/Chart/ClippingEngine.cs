#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    internal class ClippingEngine
    {
        public static Point[] ClipSegmentCS(Rect rect, Point[] pts)
        {
            double num7 = 0.0;
            double num8 = 0.0;
            bool flag = false;
            Point pt = pts[0];
            Point point2 = pts[1];
            int num = CScode(rect, pt);
            int num2 = CScode(rect, point2);
            double num5 = point2.X - pt.X;
            double num6 = point2.Y - pt.Y;
            if (num5 != 0.0)
            {
                num8 = num6 / num5;
            }
            else if (num6 == 0.0)
            {
                if ((num == 0) && (num2 == 0))
                {
                    return new Point[] { pt, point2 };
                }
                return null;
            }
            if (num6 != 0.0)
            {
                num7 = num5 / num6;
            }
            int num3 = 4;
            do
            {
                if ((num & num2) > 0)
                {
                    break;
                }
                if ((num == 0) && (num2 == 0))
                {
                    flag = true;
                    break;
                }
                if (num == 0)
                {
                    int num4 = num;
                    num = num2;
                    num2 = num4;
                    double x = pt.X;
                    pt.X = point2.X;
                    point2.X = x;
                    x = pt.Y;
                    pt.Y = point2.Y;
                    point2.Y = x;
                }
                if ((num & 1) > 0)
                {
                    pt.Y += num8 * (rect.Left - pt.X);
                    pt.X = rect.Left;
                }
                else if ((num & 2) > 0)
                {
                    pt.Y += num8 * (rect.Right - pt.X);
                    pt.X = rect.Right;
                }
                else if ((num & 4) > 0)
                {
                    pt.X += num7 * (rect.Bottom - pt.Y);
                    pt.Y = rect.Bottom;
                }
                else if ((num & 8) > 0)
                {
                    pt.X += num7 * (rect.Top - pt.Y);
                    pt.Y = rect.Top;
                }
                num = CScode(rect, pt);
            }
            while (--num3 >= 0);
            if (flag)
            {
                return new Point[] { pt, point2 };
            }
            return null;
        }

        public static bool ClipSegmentCS(Rect rect, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            double num7 = 0.0;
            double num8 = 0.0;
            bool flag = false;
            double x = x1;
            double y = y1;
            double num12 = x2;
            double num13 = y2;
            int num = CScode(rect, x, y);
            int num2 = CScode(rect, num12, num13);
            double num5 = num12 - x;
            double num6 = num13 - y;
            if (num5 != 0.0)
            {
                num8 = num6 / num5;
            }
            else if (num6 == 0.0)
            {
                if ((num == 0) && (num2 == 0))
                {
                    x1 = x;
                    y1 = y;
                    x2 = num12;
                    y2 = num13;
                    return true;
                }
                return false;
            }
            if (num6 != 0.0)
            {
                num7 = num5 / num6;
            }
            int num3 = 4;
            do
            {
                if ((num & num2) > 0)
                {
                    break;
                }
                if ((num == 0) && (num2 == 0))
                {
                    flag = true;
                    break;
                }
                if (num == 0)
                {
                    int num4 = num;
                    num = num2;
                    num2 = num4;
                    double num9 = x;
                    x = num12;
                    num12 = num9;
                    num9 = y;
                    y = num13;
                    num13 = num9;
                }
                if ((num & 1) > 0)
                {
                    y += num8 * (rect.Left - x);
                    x = rect.Left;
                }
                else if ((num & 2) > 0)
                {
                    y += num8 * (rect.Right - x);
                    x = rect.Right;
                }
                else if ((num & 4) > 0)
                {
                    x += num7 * (rect.Bottom - y);
                    y = rect.Bottom;
                }
                else if ((num & 8) > 0)
                {
                    x += num7 * (rect.Top - y);
                    y = rect.Top;
                }
                num = CScode(rect, x, y);
            }
            while (--num3 >= 0);
            if (flag)
            {
                x1 = x;
                y1 = y;
                x2 = num12;
                y2 = num13;
                return true;
            }
            return false;
        }

        public static PathFigure[] CreateClippedLines(Point[] pts, Rect r)
        {
            List<PathFigure> list = new List<PathFigure>();
            int length = pts.Length;
            for (int i = 1; i < length; i++)
            {
                Point[] pointArray2 = new Point[] { pts[i - 1], pts[i] };
                Point[] pointArray = ClipSegmentCS(r, pointArray2);
                if (pointArray != null)
                {
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = pointArray[0];
                    figure.IsClosed = false;
                    figure.IsFilled = true;
                    LineSegment segment = new LineSegment();
                    segment.Point = pointArray[1];
                    figure.Segments.Add(segment);
                    list.Add(figure);
                }
            }
            return list.ToArray();
        }

        static int CScode(Rect rect, Point pt)
        {
            int num = 0;
            if (pt.X < rect.Left)
            {
                num++;
            }
            else if (pt.X > rect.Right)
            {
                num += 2;
            }
            if (pt.Y > rect.Bottom)
            {
                return (num + 4);
            }
            if (pt.Y < rect.Top)
            {
                num += 8;
            }
            return num;
        }

        static int CScode(Rect rect, double x, double y)
        {
            int num = 0;
            if (x < rect.Left)
            {
                num++;
            }
            else if (x > rect.Right)
            {
                num += 2;
            }
            if (y > rect.Bottom)
            {
                return (num + 4);
            }
            if (y < rect.Top)
            {
                num += 8;
            }
            return num;
        }
    }
}

