#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-28 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自适应时间转换器，如 昨天，09:13, 2015-04-09
    /// </summary>
    public class AutoDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// 将布尔值转成Visibility枚举值
        /// </summary>
        /// <param name="value">正传递到目标的源数据</param>
        /// <param name="targetType">目标依赖项属性需要的数据的 Type</param>
        /// <param name="parameter">要在转换器逻辑中使用的可选参数</param>
        /// <param name="language">语言</param>
        /// <returns>要传递到目标依赖项属性的值</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";

            DateTime dt;
            if (value.GetType() == typeof(DateTime))
            {
                dt = (DateTime)value;
            }
            else
            {
                try
                {
                    dt = (DateTime)System.Convert.ChangeType(value, typeof(DateTime));
                }
                catch
                {
                    return "";
                }
            }

            string result;
            TimeSpan ts = DateTime.Now.Date - dt.Date;
            switch (ts.Days)
            {
                case 0:
                    result = dt.ToString("HH:mm:ss");
                    break;
                case 1:
                    result = "昨天";
                    break;
                case -1:
                    result = "明天";
                    break;
                default:
                    result = dt.ToString("yyyy-MM-dd");
                    break;
            }
            return result;
        }

        /// <summary>
        /// 单向绑定
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

