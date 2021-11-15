#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 特殊处理CNum与Cell之间null的转换
    /// </summary>
    public class NumValConverter : IValueConverter
    {
        /// <summary>
        /// 在将源数据传递到目标以在 UI 中显示之前，对源数据进行修改
        /// </summary>
        /// <param name="value">正传递到目标的源数据</param>
        /// <param name="targetType">目标依赖项属性需要的数据的 Type</param>
        /// <param name="parameter">要在转换器逻辑中使用的可选参数</param>
        /// <param name="language">语言</param>
        /// <returns>要传递到目标依赖项属性的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return double.NaN;
            return value;
        }

        /// <summary>
        /// 在将目标数据传递到源对象之前，对目标数据进行修改
        /// </summary>
        /// <param name="value">正传递到源的目标数据</param>
        /// <param name="targetType">源对象需要的数据的 Type</param>
        /// <param name="parameter">要在转换器逻辑中使用的可选参数</param>
        /// <param name="language">语言</param>
        /// <returns>要传递到源对象的值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (double.IsNaN((double)value))
                return null;
            return value;
        }
    }
}
