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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public static class Extensions
    {
        internal static Rect EmptyRect = new Rect(3.4028234663852886E+38, 3.4028234663852886E+38, 3.4028234663852886E+38, 3.4028234663852886E+38);

        internal static long DoubleDateToTicks(double value)
        {
            if ((value >= 2958466.0) || (value <= -657435.0))
            {
                throw new ArgumentException("Arg_OleAutDateInvalid");
            }
            long num = (long)((value * 86400000.0) + ((value >= 0.0) ? 0.5 : -0.5));
            if (num < 0L)
            {
                num -= (num % 0x5265c00L) * 2L;
            }
            num += 0x3680b5e1fc00L;
            if ((num < 0L) || (num >= 0x11efae44cb400L))
            {
                throw new ArgumentException("Arg_OleAutDateScale");
            }
            return (num * 0x2710L);
        }

        internal static Binding From<T>(this Binding binding, Expression<Func<T, object>> expr)
        {
            binding.Path = new PropertyPath(GetPropertyPath<T>(expr));
            return binding;
        }

        internal static Binding From<T>(this Binding binding, T source, Expression<Func<T, object>> expr)
        {
            binding.Source = source;
            binding.Path = new PropertyPath(GetPropertyPath<T>(expr));
            return binding;
        }

        public static DateTime FromOADate(this double d)
        {
            return new DateTime(DoubleDateToTicks(d), (DateTimeKind) DateTimeKind.Unspecified);
        }

        internal static Point GetPosition(this PointerRoutedEventArgs e, UIElement element)
        {
            return e.GetCurrentPoint(element).Position;
        }

        internal static string GetPropertyPath<T>(Expression<Func<T, object>> expression)
        {
            MemberExpression expression3;
            IList<string> list = (IList<string>) new List<string>();
            for (Expression expression2 = expression.Body; expression2.NodeType != ((ExpressionType) ((int) ExpressionType.Parameter)); expression2 = expression3.Expression)
            {
                ExpressionType type = expression2.NodeType;
                if (type == ((ExpressionType) ((int) ExpressionType.Call)))
                {
                    MethodCallExpression expression4 = (MethodCallExpression) expression2;
                    throw new InvalidOperationException("The member '" + expression4.Method.Name + "' is a method call but a Property or Field was expected.");
                }
                if ((type != ((ExpressionType) ((int) ExpressionType.Convert))) && (type != ((ExpressionType) ((int) ExpressionType.MemberAccess))))
                {
                    throw new InvalidOperationException("The expression NodeType '" + expression2.NodeType.ToString() + "' is not supported, expected MemberAccess, Convert, or Call.");
                }
                expression3 = (expression2.NodeType == ((ExpressionType) ((int) ExpressionType.MemberAccess))) ? ((MemberExpression) expression2) : ((MemberExpression) ((UnaryExpression) expression2).Operand);
                if (!(expression3.Member is PropertyInfo) && !(expression3.Member is FieldInfo))
                {
                    throw new InvalidOperationException("The member '" + expression3.Member.Name + "' is not a field or property");
                }
                list.Add(expression3.Member.Name);
            }
            return string.Join(".", Enumerable.ToArray<string>(Enumerable.Reverse<string>((IEnumerable<string>) list)));
        }

        internal static Rect IntersectRect(this Rect rect1, Rect rect2)
        {
            if (!IntersectsWith(rect1, rect2))
            {
                return EmptyRect;
            }
            double num = Math.Max(rect1.Left, rect2.Left);
            double num2 = Math.Max(rect1.Top, rect2.Top);
            rect1.Width = Math.Max((double) (Math.Min(rect1.Right, rect2.Right) - num), (double) 0.0);
            rect1.Height = Math.Max((double) (Math.Min(rect1.Bottom, rect2.Bottom) - num2), (double) 0.0);
            rect1.X = num;
            rect1.Y = num2;
            return rect1;
        }

        static bool IntersectsWith(Rect rect1, Rect rect2)
        {
            return ((((!rect1.IsEmptyRect() && !rect2.IsEmptyRect()) && ((rect2.Left <= rect1.Right) && (rect2.Right >= rect1.Left))) && (rect2.Top <= rect1.Bottom)) && (rect2.Bottom >= rect1.Top));
        }

        internal static bool IsEmptyRect(this Rect rect)
        {
            return (rect == EmptyRect);
        }

        internal static bool IsInVisualTree(this DependencyObject element)
        {
            return IsInVisualTree(element, Window.Current.Content);
        }

        static bool IsInVisualTree(DependencyObject element, DependencyObject ancestor)
        {
            for (DependencyObject obj2 = element; obj2 != null; obj2 = VisualTreeHelper.GetParent(obj2))
            {
                if (obj2 == ancestor)
                {
                    return true;
                }
            }
            return false;
        }

        static double TicksToOADate(long value)
        {
            if (value == 0L)
            {
                return 0.0;
            }
            if (value < 0xc92a69c000L)
            {
                value += 0x85103c0cb83c000L;
            }
            if (value < 0x6efdddaec64000L)
            {
                throw new OverflowException("Arg_OleAutDateInvalid");
            }
            long num = (value - 0x85103c0cb83c000L) / 0x2710L;
            if (num < 0L)
            {
                long num2 = num % 0x5265c00L;
                if (num2 != 0L)
                {
                    num -= (0x5265c00L + num2) * 2L;
                }
            }
            return (((double) num) / 86400000.0);
        }

        internal static DoubleCollection ToCollection(this double[] thisPointArray)
        {
            DoubleCollection doubles = new DoubleCollection();
            for (int i = 0; i < thisPointArray.Length; i++)
            {
                doubles.Add(thisPointArray[i]);
            }
            return doubles;
        }

        internal static PointCollection ToCollection(this Point[] thisPointArray)
        {
            PointCollection points = new PointCollection();
            int length = thisPointArray.Length;
            for (int i = 0; i < length; i++)
            {
                points.Add(thisPointArray[i]);
            }
            return points;
        }

        public static double ToOADate(this DateTime date)
        {
            return TicksToOADate(date.Ticks);
        }
    }
}

