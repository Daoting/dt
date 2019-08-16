#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-08-05 创建
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
    /// 将object与 Visibility 枚举值相互转换的转换器。
    /// </summary>
    public class ObjectToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将object转成Visibility枚举值，字符串"1"或"true"返回Visible，其它情况Collapsed
        /// </summary>
        /// <param name="value">正传递到目标的源数据</param>
        /// <param name="targetType">目标依赖项属性需要的数据的 Type</param>
        /// <param name="parameter">要在转换器逻辑中使用的可选参数</param>
        /// <param name="language">语言</param>
        /// <returns>要传递到目标依赖项属性的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str;
            if (value == null || string.IsNullOrEmpty(str = value.ToString().ToLower()))
                return Visibility.Collapsed;
            return (str == "1" || str == "true") ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 将Visibility枚举值转成布尔值
        /// </summary>
        /// <param name="value">正传递到源的目标数据</param>
        /// <param name="targetType">源对象需要的数据的 Type</param>
        /// <param name="parameter">要在转换器逻辑中使用的可选参数</param>
        /// <param name="language">语言</param>
        /// <returns>要传递到源对象的值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将object取反与 Visibility 枚举值相互转换的转换器。
    /// </summary>
    public class RObjectToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将object转成Visibility枚举值，字符串"0"或"false"返回Visible，其它情况Collapsed
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str;
            if (value == null || string.IsNullOrEmpty(str = value.ToString().ToLower()))
                return Visibility.Collapsed;
            return (str == "0" || str == "false") ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 将Visibility枚举值转成布尔值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

