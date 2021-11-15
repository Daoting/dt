#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 源CDate.Data，目标CDate.Value
    /// </summary>
    class DateValConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return default(DateTime);

            if (value.GetType() == typeof(DateTime))
                return (DateTime)value;

            try
            {
                return System.Convert.ToDateTime(value);
            }
            catch { }
            return default(DateTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Type dataType = (Type)parameter;
            if (dataType == typeof(DateTime))
                return value;

            object result = null;
            try
            {
                result = System.Convert.ChangeType(value, dataType);
            }
            catch { }
            return result;
        }
    }

    /// <summary>
    /// 源CDate.Value，目标TextBlock
    /// </summary>
    class DateValUIConverter : IValueConverter
    {
        CDate _owner;

        public DateValUIConverter(CDate p_owner)
        {
            _owner = p_owner;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToString(_owner.Format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 源CDate.Value，目标MaskBox
    /// </summary>
    class ValMaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return default(DateTime);
            return value;
        }
    }
}