#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class AxisUtility
    {
        public static double CalculateLabelUnit(double minimum, double maximum, double majorUnit)
        {
            return majorUnit;
        }

        public static double CalculateMajorUnit(double minimum, double maximum, bool autoMinorUnit, double minorUnit, bool logarithmic, double logarithmicBase)
        {
            double a = Math.Abs((double) (maximum - minimum));
            if (logarithmic)
            {
                return logarithmicBase;
            }
            double num2 = Math.Pow(10.0, Math.Floor(Math.Log(a, 10.0)));
            if ((a / num2) <= 1.6)
            {
                num2 /= 5.0;
            }
            else if ((a / num2) <= 4.0)
            {
                num2 /= 2.0;
            }
            else if ((a / num2) > 8.0)
            {
                num2 *= 2.0;
            }
            if (!autoMinorUnit)
            {
                num2 = Math.Max(num2, minorUnit);
            }
            return num2;
        }

        public static double CalculateMaximum(double maximum, bool tickBetween)
        {
            if (tickBetween)
            {
                maximum += 0.5;
            }
            return maximum;
        }

        public static double CalculateMaximum(double minimum, double maximum, bool logarithmic, double logarithmicBase)
        {
            if (logarithmic)
            {
                if ((minimum < 1.0) && (maximum < 1.0))
                {
                    maximum = 1.0;
                }
                return maximum;
            }
            if ((minimum < 0.0) && (maximum < 0.0))
            {
                if ((1.2 * maximum) <= minimum)
                {
                    maximum += 0.5 * (maximum - minimum);
                    return maximum;
                }
                maximum = 0.0;
            }
            return maximum;
        }

        public static double CalculateMaximum(double minimum, double maximum, double majorUnit, bool logarithmic, double logarithmicBase)
        {
            return CalculateMaximum(minimum, maximum, majorUnit, logarithmic, logarithmicBase, false);
        }

        public static double CalculateMaximum(double minimum, double maximum, double majorUnit, bool logarithmic, double logarithmicBase, bool baseOne)
        {
            if (logarithmic)
            {
                maximum = Math.Pow(majorUnit, Math.Ceiling(Math.Log(maximum, majorUnit)));
            }
            else
            {
                maximum = Math.Ceiling((double) (maximum / majorUnit)) * majorUnit;
            }
            if (baseOne)
            {
                maximum++;
            }
            return maximum;
        }

        public static double CalculateMinimum(double minimum, bool tickBetween)
        {
            if (tickBetween)
            {
                minimum -= 0.5;
            }
            return minimum;
        }

        public static double CalculateMinimum(double minimum, double maximum, bool logarithmic, double logarithmicBase)
        {
            if (logarithmic)
            {
                if ((1.0 < minimum) && (1.0 < maximum))
                {
                    minimum = 1.0;
                }
                return minimum;
            }
            if ((0.0 < minimum) && (0.0 < maximum))
            {
                if (maximum <= (1.2 * minimum))
                {
                    minimum -= 0.5 * (maximum - minimum);
                    return minimum;
                }
                minimum = 0.0;
            }
            return minimum;
        }

        public static double CalculateMinimum(double minimum, double maximum, double majorUnit, bool logarithmic, double logarithmicBase)
        {
            if (logarithmic)
            {
                minimum = Math.Pow(logarithmicBase, Math.Floor(Math.Log(minimum, logarithmicBase)));
                return minimum;
            }
            minimum = Math.Floor((double) (minimum / majorUnit)) * majorUnit;
            return minimum;
        }

        public static double CalculateMinorUnit(double minimum, double maximum, double majorUnit, bool logarithmic)
        {
            if (logarithmic)
            {
                return majorUnit;
            }
            return (majorUnit / 5.0);
        }

        public static double CalculateValidLabelUnit(double labelUnit, bool logarithmic, double logarithmicBase)
        {
            if (logarithmic)
            {
                labelUnit = Math.Max(labelUnit, logarithmicBase);
            }
            return labelUnit;
        }

        public static double CalculateValidMajorUnit(double majorUnit, bool logarithmic, double logarithmicBase)
        {
            if (logarithmic)
            {
                majorUnit = Math.Max(majorUnit, logarithmicBase);
            }
            return majorUnit;
        }

        public static double CalculateValidMaximum(double minimum, double maximum, bool tickBetween)
        {
            if (maximum <= minimum)
            {
                maximum = minimum + (tickBetween ? 0.0 : 0.5);
            }
            return maximum;
        }

        public static double CalculateValidMaximum(double minimum, double maximum, bool logarithmic, double logarithmicBase)
        {
            if (logarithmic)
            {
                if (maximum <= minimum)
                {
                    if (minimum < 1.0)
                    {
                        maximum = 1.0;
                        return maximum;
                    }
                    if (minimum == 1.0)
                    {
                        maximum = logarithmicBase;
                        return maximum;
                    }
                    if (1.0 < minimum)
                    {
                        maximum = minimum * logarithmicBase;
                    }
                }
                return maximum;
            }
            if (maximum <= minimum)
            {
                if (minimum < 0.0)
                {
                    maximum = 0.0;
                    return maximum;
                }
                if (minimum == 0.0)
                {
                    maximum = 1.0;
                    return maximum;
                }
                if (0.0 < minimum)
                {
                    maximum = 2.0 * minimum;
                }
            }
            return maximum;
        }

        public static double CalculateValidMinimum(double minimum, double maximum, bool tickBetween)
        {
            if (minimum == double.MaxValue)
            {
                if (maximum == double.MinValue)
                {
                    minimum = 0.0;
                    return minimum;
                }
                minimum = maximum - (tickBetween ? 0.0 : 0.5);
            }
            return minimum;
        }

        public static double CalculateValidMinimum(double minimum, double maximum, bool logarithmic, double logarithmicBase, bool autoMinimum, bool autoMaximum)
        {
            if (logarithmic)
            {
                if ((minimum == double.MaxValue) || (minimum <= 0.0))
                {
                    if ((maximum == double.MinValue) || (maximum <= 0.0))
                    {
                        minimum = 1.0;
                        return minimum;
                    }
                    if (maximum < 1.0)
                    {
                        minimum = maximum / logarithmicBase;
                        return minimum;
                    }
                    if (maximum == 1.0)
                    {
                        minimum = 1.0 / logarithmicBase;
                        return minimum;
                    }
                    if (1.0 < maximum)
                    {
                        minimum = 1.0;
                    }
                    return minimum;
                }
                if (((maximum <= minimum) && (maximum != double.MinValue)) && (maximum > 0.0))
                {
                    if (((maximum < 1.0) && autoMinimum) && !autoMaximum)
                    {
                        minimum = maximum / logarithmicBase;
                        return minimum;
                    }
                    if (((maximum == 1.0) && autoMinimum) && !autoMaximum)
                    {
                        minimum = 1.0 / logarithmicBase;
                        return minimum;
                    }
                    if ((1.0 >= maximum) || (!autoMinimum && autoMaximum))
                    {
                        return minimum;
                    }
                    minimum = 1.0;
                }
                return minimum;
            }
            if (minimum == double.MaxValue)
            {
                if (maximum == double.MinValue)
                {
                    minimum = 0.0;
                    return minimum;
                }
                if (maximum < 0.0)
                {
                    minimum = 2.0 * maximum;
                    return minimum;
                }
                if (maximum == 0.0)
                {
                    minimum = -1.0;
                    return minimum;
                }
                if (0.0 < maximum)
                {
                    minimum = 0.0;
                }
                return minimum;
            }
            if ((maximum <= minimum) && (maximum != double.MinValue))
            {
                if (((maximum < 0.0) && autoMinimum) && !autoMaximum)
                {
                    minimum = 2.0 * maximum;
                    return minimum;
                }
                if (((maximum == 0.0) && autoMinimum) && !autoMaximum)
                {
                    minimum = -1.0;
                    return minimum;
                }
                if ((0.0 >= maximum) || (!autoMinimum && autoMaximum))
                {
                    return minimum;
                }
                minimum = 0.0;
            }
            return minimum;
        }

        public static double CalculateValidMinorUnit(double minorUnit, bool logarithmic, double logarithmicBase)
        {
            if (logarithmic)
            {
                minorUnit = Math.Max(minorUnit, logarithmicBase);
            }
            return minorUnit;
        }
    }
}

