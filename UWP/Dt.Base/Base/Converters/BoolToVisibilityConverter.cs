#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 表示将布尔值与 Visibility 枚举值相互转换的转换器。
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
                return b ? Visibility.Visible : Visibility.Collapsed;

            if (value is IConvertible)
                return (bool)System.Convert.ChangeType(value, typeof(bool)) ? Visibility.Visible : Visibility.Collapsed;

            throw new Exception($"{value}不是bool类型");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((value is Visibility) && (((Visibility) value) == Visibility.Visible));
        }
    }
}

