#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-06-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数值显示转换类
    /// </summary>
    public class NumericConverter : IValueConverter
    {
        int _scale;
        bool _isPercentage;

        /// <summary>
        /// 获取设置保留小数位
        /// </summary>
        public int Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        /// <summary>
        /// 获取设置是否显示为百分数
        /// </summary>
        public bool IsPercentage
        {
            get { return _isPercentage; }
            set { _isPercentage = value; }
        }

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
                return "";

            string format = "";
            var info = CultureInfo.CurrentCulture.NumberFormat;
            info.NumberGroupSeparator = ",";
            if (_isPercentage)
            {
                format = "P";
                info.PercentDecimalDigits = _scale;
            }
            else
            {
                format = "N";
                info.NumberDecimalDigits = _scale;
            }

            double val = 0;
            try
            {
                val = (double)System.Convert.ChangeType(value, typeof(double));
            }
            catch
            { }
            return val.ToString(format, info);
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
            throw new NotImplementedException();
        }
    }
}
