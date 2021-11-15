#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    internal class SplineNew
    {
        Point[] _a;
        Point[] _b;
        Point[] _c;
        Point[] _d;
        int _len;
        Point[] _pts;
        static double[,] m = new double[,] { { -0.5, 1.5, -1.5, 0.5 }, { 1.0, -2.5, 2.0, -0.5 }, { -0.5, 0.0, 0.5, 0.0 }, { 0.0, 1.0, 0.0, 0.0 } };

        public SplineNew(Point[] pts)
        {
            _pts = pts;
            _len = pts.Length;
            _a = new Point[_len - 1];
            _b = new Point[_len - 1];
            _c = new Point[_len - 1];
            _d = new Point[_len - 1];
            for (int i = 0; i < (_len - 1); i++)
            {
                Point point = (i == 0) ? pts[i] : pts[i - 1];
                Point point2 = pts[i];
                Point point3 = pts[i + 1];
                Point point4 = (i == (_len - 2)) ? pts[i + 1] : pts[i + 2];
                _a[i].X = (((point.X * m[0, 0]) + (point2.X * m[0, 1])) + (point3.X * m[0, 2])) + (point4.X * m[0, 3]);
                _b[i].X = (((point.X * m[1, 0]) + (point2.X * m[1, 1])) + (point3.X * m[1, 2])) + (point4.X * m[1, 3]);
                _c[i].X = (((point.X * m[2, 0]) + (point2.X * m[2, 1])) + (point3.X * m[2, 2])) + (point4.X * m[2, 3]);
                _d[i].X = (((point.X * m[3, 0]) + (point2.X * m[3, 1])) + (point3.X * m[3, 2])) + (point4.X * m[3, 3]);
                _a[i].Y = (((point.Y * m[0, 0]) + (point2.Y * m[0, 1])) + (point3.Y * m[0, 2])) + (point4.Y * m[0, 3]);
                _b[i].Y = (((point.Y * m[1, 0]) + (point2.Y * m[1, 1])) + (point3.Y * m[1, 2])) + (point4.Y * m[1, 3]);
                _c[i].Y = (((point.Y * m[2, 0]) + (point2.Y * m[2, 1])) + (point3.Y * m[2, 2])) + (point4.Y * m[2, 3]);
                _d[i].Y = (((point.Y * m[3, 0]) + (point2.Y * m[3, 1])) + (point3.Y * m[3, 2])) + (point4.Y * m[3, 3]);
            }
        }

        public Point[] Calculate()
        {
            Point point2;
            List<Point> list = new List<Point>();
            Point point = Calculate(0.0);
            list.Add(point);
            double num = _len * 0.002;
            double val = num;
        Label_0031:
            point2 = Calculate(val);
            if ((Math.Abs((double) (point.X - point2.X)) >= 1.0) || (Math.Abs((double) (point.Y - point2.Y)) >= 1.0))
            {
                list.Add(point2);
                point = point2;
            }
            if (val <= (_len - 1))
            {
                val += num;
                goto Label_0031;
            }
            return list.ToArray();
        }

        public Point Calculate(double val)
        {
            int index = (int) val;
            if (index < 0)
            {
                index = 0;
            }
            if (index > (_len - 2))
            {
                index = _len - 2;
            }
            Point point = new Point();
            double num2 = val - index;
            point.X = (((((_a[index].X * num2) + _b[index].X) * num2) + _c[index].X) * num2) + _d[index].X;
            point.Y = (((((_a[index].Y * num2) + _b[index].Y) * num2) + _c[index].Y) * num2) + _d[index].Y;
            return point;
        }

        public PointCollection CalculateCollection()
        {
            Point point2;
            PointCollection points = new PointCollection();
            Point point = Calculate(0.0);
            points.Add(point);
            double num = _len * 0.002;
            double val = num;
        Label_0031:
            point2 = Calculate(val);
            if ((Math.Abs((double) (point.X - point2.X)) >= 1.0) || (Math.Abs((double) (point.Y - point2.Y)) >= 1.0))
            {
                points.Add(point2);
                point = point2;
            }
            if (val <= (_len - 1))
            {
                val += num;
                goto Label_0031;
            }
            return points;
        }

        public Point[] Points
        {
            get { return  _pts; }
        }
    }
}

