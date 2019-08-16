#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 将颜色值转换为画刷类
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 将颜色值转换为画刷
        /// </summary>
        /// <param name="value">正传递到目标的源数据</param>
        /// <param name="targetType">目标依赖项属性需要的数据的 Type</param>
        /// <param name="parameter">要在转换器逻辑中使用的可选参数</param>
        /// <param name="language">语言</param>
        /// <returns>要传递到目标依赖项属性的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush brush = new SolidColorBrush();
            if (value != null)
            {
                brush.Color = (Color) value;
            }
            return brush;
        }

        /// <summary>
        /// 未实现
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
}

