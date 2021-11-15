#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    internal class ThemeConverter : IValueConverter
    {
        static Brush DefaultForeground = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            ResourceDictionary dictionary = value as ResourceDictionary;
            object[] objArray = parameter as object[];
            if ((objArray == null) || (objArray.Length < 2))
            {
                return null;
            }
            string str = (string) (objArray[0] as string);
            if ((value != null) && (str != null))
            {
                if (dictionary.ContainsKey(str))
                {
                    value = dictionary[str];
                }
                else
                {
                    if (str.EndsWith("Brush"))
                    {
                        return null;
                    }
                    if ((str.EndsWith("Thickness") || str.EndsWith("Padding")) || str.EndsWith("Margin"))
                    {
                        return new Windows.UI.Xaml.Thickness();
                    }
                    if (str.EndsWith("CornerRadius"))
                    {
                        return new Windows.UI.Xaml.CornerRadius();
                    }
                }
                string s = (string) (value as string);
                if (s == null)
                {
                    return value;
                }
                if (str.EndsWith("CornerRadius"))
                {
                    return ParseCornerRadius(s);
                }
                if ((!str.EndsWith("Thickness") && !str.EndsWith("Padding")) && !str.EndsWith("Margin"))
                {
                    return value;
                }
                return ParseThickness(s);
            }
            if (str != null)
            {
                if (str.EndsWith("Foreground_Brush"))
                {
                    return FindParentForeground(objArray[1] as FrameworkElement);
                }
                if (str.EndsWith("Brush"))
                {
                    return null;
                }
                if ((str.EndsWith("Thickness") || str.EndsWith("Padding")) || str.EndsWith("Margin"))
                {
                    return new Windows.UI.Xaml.Thickness();
                }
                if (str.EndsWith("CornerRadius"))
                {
                    return new Windows.UI.Xaml.CornerRadius();
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }

        static Brush FindParentForeground(FrameworkElement fe)
        {
            Brush foreground = null;
            while (fe != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(fe);
                Control control = parent as Control;
                if (control != null)
                {
                    foreground = control.Foreground;
                    break;
                }
                fe = parent as FrameworkElement;
            }
            if (foreground == null)
            {
                return DefaultForeground;
            }
            return foreground;
        }

        static Windows.UI.Xaml.CornerRadius ParseCornerRadius(string s)
        {
            Windows.UI.Xaml.CornerRadius radius = new Windows.UI.Xaml.CornerRadius();
            if (string.IsNullOrEmpty(s))
            {
                return radius;
            }
            string[] strArray = s.Split(new char[] { ',', ' ' });
            if (strArray.Length == 1)
            {
                return new Windows.UI.Xaml.CornerRadius(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (strArray.Length != 4)
            {
                throw new FormatException(string.Format("String {0} does not represent CornerRadius", (object[]) new object[] { s }));
            }
            return new Windows.UI.Xaml.CornerRadius(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture));
        }

        static Windows.UI.Xaml.Thickness ParseThickness(string s)
        {
            Windows.UI.Xaml.Thickness thickness = new Windows.UI.Xaml.Thickness();
            if (string.IsNullOrEmpty(s))
            {
                return thickness;
            }
            string[] strArray = s.Split(new char[] { ',', ' ' });
            if (strArray.Length == 1)
            {
                return new Windows.UI.Xaml.Thickness(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (strArray.Length != 4)
            {
                throw new FormatException(string.Format("String {0} does not represent Thickness", (object[]) new object[] { s }));
            }
            return new Windows.UI.Xaml.Thickness(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture));
        }
    }
}

