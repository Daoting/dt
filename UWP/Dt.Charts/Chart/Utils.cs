#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    internal class Utils
    {
        public const double DegToRad = 0.017453292519943295;
        public static Size LargeSize = new Size(10000.0, 10000.0);
        public const double Pi2 = 6.2831853071795862;
        public const double PiHalf = 1.5707963267948966;
        public const double RadToDeg = 57.295779513082323;
        public static Visibility VisHidden = Visibility.Collapsed;

        Utils()
        {
        }

        public static Brush Clone(Brush brush)
        {
            return brush;
        }

        public static DoubleCollection Clone(DoubleCollection dc)
        {
            DoubleCollection doubles = dc;
            if ((dc != null) && (dc.Count > 0))
            {
                doubles = new DoubleCollection();
                for (int i = 0; i < dc.Count; i++)
                {
                    doubles.Add(dc[i]);
                }
            }
            return doubles;
        }

        public static Line Clone(Line line)
        {
            Line line2 = new Line();
            line2.Stroke = line.Stroke;
            line2.StrokeDashCap = line.StrokeDashCap;
            line2.StrokeDashOffset = line.StrokeDashOffset;
            line2.StrokeEndLineCap = line.StrokeEndLineCap;
            line2.StrokeLineJoin = line.StrokeLineJoin;
            line2.StrokeMiterLimit = line.StrokeMiterLimit;
            line2.StrokeStartLineCap = line.StrokeStartLineCap;
            line2.StrokeThickness = line.StrokeThickness;
            line2.RenderTransform = line.RenderTransform;
            line2.RenderTransformOrigin = line.RenderTransformOrigin;
            return line2;
        }

        public static object[] CreateArray(IEnumerable list)
        {
            List<object> list2 = null;
            if (list != null)
            {
                list2 = new List<object>();
                IEnumerator enumerator = list.GetEnumerator();
                DataUtils.TryReset(enumerator);
                while (enumerator.MoveNext())
                {
                    list2.Add(enumerator.Current);
                }
                if (list2.Count == 0)
                {
                    list2 = null;
                }
            }
            if (list2 != null)
            {
                return list2.ToArray();
            }
            return null;
        }

        public static int FindIntersection(Point start1, Point end1, Point start2, Point end2)
        {
            Point point = new Point(end1.X - start1.X, end1.Y - start1.Y);
            Point point2 = new Point(end2.X - start2.X, end2.Y - start2.Y);
            double num = -point.Y;
            double x = point.X;
            double num3 = -((num * start1.X) + (x * start1.Y));
            double num4 = -point2.Y;
            double num5 = point2.X;
            double num6 = -((num4 * start2.X) + (num5 * start2.Y));
            double num7 = ((num4 * start1.X) + (num5 * start1.Y)) + num6;
            double num8 = ((num4 * end1.X) + (num5 * end1.Y)) + num6;
            double num9 = ((num * start2.X) + (x * start2.Y)) + num3;
            double num10 = ((num * end2.X) + (x * end2.Y)) + num3;
            if (((num7 * num8) <= 0.0) && ((num9 * num10) <= 0.0))
            {
                return -Math.Sign(num6);
            }
            return 0;
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T: DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                string str = (string) (child.GetValue(FrameworkElement.NameProperty) as string);
                if (str == name)
                {
                    return (T)child;
                }
                T local = FindVisualChildByName<T>(child, name);
                if (local != null)
                {
                    return local;
                }
            }
            return default(T);
        }

        public static bool GetIsClustered(bool defval, object sym)
        {
            IRenderVisual visual = sym as IRenderVisual;
            if (visual != null)
            {
                return visual.IsClustered;
            }
            IPlotElement element = sym as IPlotElement;
            if (element != null)
            {
                return element.IsClustered;
            }
            return defval;
        }

        public static double GetMajorUnit(double range, IAxis ax, double delta = 0.0)
        {
            double majorUnit = ax.MajorUnit;
            if (double.IsNaN(majorUnit))
            {
                int num2 = NicePrecision(range);
                double x = range / ((double) ax.GetAnnoNumber());
                if (delta == 0.0)
                {
                    delta = NiceNumber(2.0 * x, -num2, true);
                    if (delta < x)
                    {
                        delta = NiceNumber(x, -num2 + 1, false);
                    }
                    if (delta < x)
                    {
                        delta = NiceTickNumber(x);
                    }
                }
                return delta;
            }
            delta = majorUnit;
            return delta;
        }

        public static Point GetNormal(Point tan)
        {
            return new Point(tan.Y, -tan.X);
        }

        public static Size GetSize(UIElement el)
        {
            FrameworkElement element = el as FrameworkElement;
            if (((element != null) && (element.ActualWidth > 0.0)) && (element.ActualHeight > 0.0))
            {
                return new Size(element.ActualWidth, element.ActualHeight);
            }
            el.Measure(LargeSize);
            return el.DesiredSize;
        }

        internal static Rect InflateRect(Rect rect, double width, double height)
        {
            if (rect.IsEmpty)
            {
                throw new InvalidOperationException("Rect_CannotCallMethod");
            }
            double x = rect.X - width;
            double y = rect.Y - height;
            double num3 = (rect.Width + width) + width;
            double num4 = (rect.Height + height) + height;
            if ((num3 < 0.0) || (num4 < 0.0))
            {
                rect = Rect.Empty;
                return rect;
            }
            rect = new Rect(x, y, num3, num4);
            return rect;
        }

        public static double NiceNumber(double x, int exp, bool round)
        {
            if (x == 0.0)
            {
                return x;
            }
            if (x < 0.0)
            {
                x = -x;
            }
            double num = x / Math.Pow(10.0, (double) exp);
            double num2 = 10.0;
            if (round)
            {
                if (num < 1.5)
                {
                    num2 = 1.0;
                }
                else if (num < 3.0)
                {
                    num2 = 2.0;
                }
                else if (num < 4.5)
                {
                    num2 = 4.0;
                }
                else if (num < 7.0)
                {
                    num2 = 5.0;
                }
            }
            else if (num <= 1.0)
            {
                num2 = 1.0;
            }
            else if (num <= 2.0)
            {
                num2 = 2.0;
            }
            else if (num <= 5.0)
            {
                num2 = 5.0;
            }
            return (num2 * Math.Pow(10.0, (double) exp));
        }

        public static int NicePrecision(double range)
        {
            if ((range <= 0.0) || double.IsNaN(range))
            {
                return 0;
            }
            int num2 = (int) SignedFloor(Math.Log10(range));
            double num3 = range / Math.Pow(10.0, (double) num2);
            if (num3 < 3.0)
            {
                num2 = -num2 + 1;
                num3 = range / Math.Pow(10.0, (double) num2);
                if (num3 < 3.0)
                {
                    num2++;
                }
            }
            return num2;
        }

        public static double NiceTickNumber(double x)
        {
            if (x == 0.0)
            {
                return x;
            }
            if (x < 0.0)
            {
                x = -x;
            }
            int num2 = (int) SignedFloor(Math.Log10(x));
            double num3 = x / Math.Pow(10.0, (double) num2);
            double num4 = 10.0;
            if (num3 <= 1.0)
            {
                num4 = 1.0;
            }
            else if (num3 <= 2.0)
            {
                num4 = 2.0;
            }
            else if (num3 <= 5.0)
            {
                num4 = 5.0;
            }
            return (num4 * Math.Pow(10.0, (double) num2));
        }

        public static double PrecCeil(int prec, double value)
        {
            double num = Math.Pow(10.0, (double) prec);
            double a = value / num;
            return (Math.Ceiling(a) * num);
        }

        public static double PrecFloor(int prec, double value)
        {
            double num = Math.Pow(10.0, (double) prec);
            double d = value / num;
            return (Math.Floor(d) * num);
        }

        public static DependencyProperty RegisterAttachedProperty(string name, Type propertyType, Type ownerType, PropertyChangedCallback propertyChangedCallback, object defaultValue)
        {
            return DependencyProperty.RegisterAttached(name, propertyType, ownerType, new PropertyMetadata(defaultValue, propertyChangedCallback));
        }

        public static DependencyProperty RegisterProperty(string name, Type propertyType, Type ownerType)
        {
            return RegisterProperty(name, propertyType, ownerType, null, null);
        }

        public static DependencyProperty RegisterProperty(string name, Type propertyType, Type ownerType, PropertyChangedCallback propertyChangedCallback)
        {
            return RegisterProperty(name, propertyType, ownerType, propertyChangedCallback, null);
        }

        public static DependencyProperty RegisterProperty(string name, Type propertyType, Type ownerType, PropertyChangedCallback propertyChangedCallback, object defaultValue)
        {
            return DependencyProperty.Register(name, propertyType, ownerType, new PropertyMetadata(defaultValue, propertyChangedCallback));
        }

        static double SignedFloor(double val)
        {
            if (val < 0.0)
            {
                return Math.Ceiling(val);
            }
            return Math.Floor(val);
        }

        public static PointCollection ToCollection(Point[] thisPointArray)
        {
            PointCollection points = new PointCollection();
            int length = thisPointArray.Length;
            for (int i = 0; i < length; i++)
            {
                points.Add(thisPointArray[i]);
            }
            return points;
        }
    }
}

