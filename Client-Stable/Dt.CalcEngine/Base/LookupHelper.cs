#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
#endregion

namespace Dt.CalcEngine
{
    internal class LookupHelper
    {
        private static bool _bstarted;
        private static bool _bsup;
        private static int _mcurrent;
        private static int _mhigh;
        private static int _mlow;
        private static int _morig;

        internal static string CreateStringcomparisonRegexPattern(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            StringBuilder builder = new StringBuilder(s);
            builder.Replace("^", @"\^");
            builder.Replace("$", @"\$");
            builder.Replace("(", @"\(");
            builder.Replace(")", @"\)");
            builder.Replace("[", @"\[");
            builder.Replace("]", @"\]");
            builder.Replace("{", @"\{");
            builder.Replace("}", @"\}");
            builder.Replace(".", @"\.");
            builder.Replace("+", @"\+");
            builder.Replace("|", @"\|");
            builder.Replace("~?", "{113E2532-EAF5-444c-A5CB-3D7446971C4D}");
            builder.Replace("~*", "{E21523B3-0F1F-458f-B547-23D25713D0EC}");
            builder.Replace("?", ".");
            builder.Replace("*", @"((.|\n)*)");
            builder.Replace("{113E2532-EAF5-444c-A5CB-3D7446971C4D}", @"\?");
            builder.Replace("{E21523B3-0F1F-458f-B547-23D25713D0EC}", @"\*");
            return builder.ToString();
        }

        internal static int find_bound_walk(int l, int h, int start, bool up, bool reset)
        {
            if (l < 0)
            {
                return -1;
            }
            if (h < 0)
            {
                return -1;
            }
            if (h < l)
            {
                return -1;
            }
            if (start < l)
            {
                return -1;
            }
            if (start > h)
            {
                return -1;
            }
            if (reset)
            {
                _mlow = l;
                _mhigh = h;
                _mcurrent = start;
                _morig = start;
                _bsup = up;
                _bstarted = up;
                return _mcurrent;
            }
            if (_bsup)
            {
                _mcurrent++;
                if ((_mcurrent <= _mhigh) || (_bsup != _bstarted))
                {
                    if ((_mcurrent > _mhigh) && (_bsup != _bstarted))
                    {
                        return -1;
                    }
                }
                else
                {
                    _mcurrent = _morig - 1;
                    _bsup = false;
                }
            }
            else
            {
                _mcurrent--;
                if ((_mcurrent < _mlow) && (_bsup == _bstarted))
                {
                    _mcurrent = _morig + 1;
                    _bsup = true;
                }
                else if ((_mcurrent < _mlow) && (_bsup != _bstarted))
                {
                    return -1;
                }
            }
            return _mcurrent;
        }

        internal static bool find_compare_type_valid(object find, object val)
        {
            if (val == null)
            {
                return false;
            }
            return ((CalcConvert.IsNumber(find) && CalcConvert.IsNumber(val)) || (((find is bool) && (val is bool)) || ((find is string) && (val is string))));
        }

        internal static int find_index_bisection(object find, object data, int type, bool height)
        {
            int rowCount;
            int num = -1;
            int l = 0;
            int num4 = -1;
            int start = -1;
            if (height)
            {
                rowCount = ArrayHelper.GetRowCount(data, 0);
            }
            else
            {
                rowCount = ArrayHelper.GetColumnCount(data, 0);
            }
            rowCount--;
            if (rowCount >= l)
            {
                while (l <= rowCount)
                {
                    object val = null;
                    if ((type >= 1) != (num == 2))
                    {
                        num4 = start;
                    }
                    start = (l + rowCount) / 2;
                    start = find_bound_walk(l, rowCount, start, type >= 0, true);
                    int num6 = start;
                    object obj3 = find;
                    if (find is CalcArray)
                    {
                        obj3 = ((CalcArray) find).GetValue(0, 0);
                    }
                    else if (find is SheetRangeReference)
                    {
                        obj3 = ((SheetRangeReference) find).GetValue(0, 0, 0, 0);
                    }
                    else if (find is CalcReference)
                    {
                        obj3 = ((CalcReference) find).GetValue(0, 0, 0);
                    }
                    while (!find_compare_type_valid(obj3, val) && (start != -1))
                    {
                        bool flag = false;
                        if (height)
                        {
                            val = value_area_get_x_y(data, 0, start);
                        }
                        else
                        {
                            val = value_area_get_x_y(data, start, 0);
                        }
                        if (find_compare_type_valid(obj3, val))
                        {
                            break;
                        }
                        start = find_bound_walk(0, 0, 0, false, false);
                        if ((!flag && (type >= 0)) && (start < num6))
                        {
                            rowCount = start;
                            flag = true;
                        }
                        else if ((!flag && (type < 0)) && (start > num6))
                        {
                            l = start;
                            flag = true;
                        }
                    }
                    if ((start == -1) && ((type >= 1) != (num == 2)))
                    {
                        return num4;
                    }
                    if (start == -1)
                    {
                        return -1;
                    }
                    num = value_compare(obj3, val, false);
                    if ((type >= 1) && (num == 1))
                    {
                        l = start + 1;
                    }
                    else if ((type >= 1) && (num == 2))
                    {
                        rowCount = start - 1;
                    }
                    else if ((type <= -1) && (num == 1))
                    {
                        rowCount = start - 1;
                    }
                    else if ((type <= -1) && (num == 2))
                    {
                        l = start + 1;
                    }
                    else if (num == 0)
                    {
                        while (((type <= -1) && (start > l)) || ((type >= 0) && (start < rowCount)))
                        {
                            int y = 0;
                            if (type >= 0)
                            {
                                y = start + 1;
                            }
                            else
                            {
                                y = start - 1;
                            }
                            if (height)
                            {
                                val = value_area_fetch_x_y(data, 0, y);
                            }
                            else
                            {
                                val = value_area_fetch_x_y(data, y, 0);
                            }
                            if (val == null)
                            {
                                return -1;
                            }
                            if (!find_compare_type_valid(obj3, val) || (value_compare(obj3, val, false) != 0))
                            {
                                return start;
                            }
                            start = y;
                        }
                        return start;
                    }
                }
                if ((type >= 1) != (num == 2))
                {
                    return start;
                }
                return num4;
            }
            return -1;
        }

        internal static int find_index_linear(object find, object data, int type, bool height)
        {
            object b = null;
            int rowCount;
            int num4 = -1;
            if (height)
            {
                rowCount = ArrayHelper.GetRowCount(data, 0);
            }
            else
            {
                rowCount = ArrayHelper.GetColumnCount(data, 0);
            }
            for (int i = 0; i < rowCount; i++)
            {
                object obj3;
                if (height)
                {
                    obj3 = value_area_fetch_x_y(data, 0, i);
                }
                else
                {
                    obj3 = value_area_fetch_x_y(data, i, 0);
                }
                if (obj3 == null)
                {
                    return -1;
                }
                object obj4 = find;
                if (find is CalcArray)
                {
                    obj4 = ((CalcArray) find).GetValue(0, 0);
                }
                else if (find is CalcReference)
                {
                    obj4 = ((CalcReference) find).GetValue(0, 0, 0);
                }
                if (find_compare_type_valid(obj4, obj3))
                {
                    int num = value_compare(obj4, obj3, false);
                    if ((type >= 1) && (num == 1))
                    {
                        num = -1;
                        if (num4 >= 0)
                        {
                            num = value_compare(obj3, b, false);
                        }
                        if ((num4 < 0) || ((num4 >= 0) && (num == 1)))
                        {
                            num4 = i;
                            b = obj3;
                        }
                    }
                    else if ((type <= -1) && (num == 2))
                    {
                        num = -1;
                        if (num4 >= 0)
                        {
                            num = value_compare(obj3, b, false);
                        }
                        if ((num4 < 0) || ((num4 >= 0) && (num == 2)))
                        {
                            num4 = i;
                            b = obj3;
                        }
                    }
                    else if (num == 0)
                    {
                        return i;
                    }
                }
            }
            return num4;
        }

        internal static object value_area_fetch_x_y(object v, int x, int y)
        {
            object obj2 = value_area_get_x_y(v, x, y);
            if (obj2 != null)
            {
                return obj2;
            }
            return (int) 0;
        }

        internal static object value_area_get_x_y(object v, int x, int y)
        {
            if (v == null)
            {
                return null;
            }
            if (!ArrayHelper.IsArrayOrRange(v))
            {
                return v;
            }
            if (x > ArrayHelper.GetColumnCount(v, 0))
            {
                int num = x;
                x = y;
                y = num;
            }
            int i = (y * ArrayHelper.GetColumnCount(v, 0)) + x;
            return ArrayHelper.GetValue(v, i, 0);
        }

        internal static int value_compare(object a, object b, bool case_sensitive)
        {
            if (a == b)
            {
                return 0;
            }
            if (a is string)
            {
                if ((b == null) && (a.ToString().Length == 0))
                {
                    return 0;
                }
                if (CalcConvert.IsNumber(b))
                {
                    return 1;
                }
                if (!(b is bool))
                {
                    DateTime time;
                    if (b is string)
                    {
                        int num = string.Compare(a.ToString(), b.ToString(), case_sensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
                        if (num == 0)
                        {
                            return 0;
                        }
                        if (num > 0)
                        {
                            return 1;
                        }
                        return 2;
                    }
                    if (!(b is DateTime))
                    {
                        return -1;
                    }
                    if (!DateTime.TryParse((string) ((string) a), out time))
                    {
                        return 1;
                    }
                    int num2 = DateTime.Compare(time, (DateTime) b);
                    if (num2 == 0)
                    {
                        return 0;
                    }
                    if (num2 > 0)
                    {
                        return 1;
                    }
                }
                return 2;
            }
            if (b is string)
            {
                if ((a == null) && (b.ToString().Length == 0))
                {
                    return 0;
                }
                if (!CalcConvert.IsNumber(a))
                {
                    DateTime time2;
                    if (a is bool)
                    {
                        return 1;
                    }
                    if (!(a is DateTime))
                    {
                        return -1;
                    }
                    if (!DateTime.TryParse((string) ((string) b), out time2))
                    {
                        return 2;
                    }
                    int num3 = DateTime.Compare((DateTime) a, time2);
                    if (num3 == 0)
                    {
                        return 0;
                    }
                    if (num3 > 0)
                    {
                        return 1;
                    }
                }
                return 2;
            }
            if (!(a is bool) || !CalcConvert.IsNumber(b))
            {
                if ((b is bool) && CalcConvert.IsNumber(a))
                {
                    return 2;
                }
                double num4 = CalcConvert.ToDouble(a);
                double num5 = CalcConvert.ToDouble(b);
                if (num4 == num5)
                {
                    return 0;
                }
                if (num4 < num5)
                {
                    return 2;
                }
            }
            return 1;
        }
    }
}

