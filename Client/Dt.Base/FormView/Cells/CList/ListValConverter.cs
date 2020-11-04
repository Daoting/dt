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
    /// 源Cell.Val，目标CList.Value
    /// </summary>
    class ListValConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Type dataType = (Type)parameter;
            if (value == null)
            {
                if (dataType.IsEnum)
                    return default(Enum);
                return null;
            }

            if (value.GetType() == dataType)
                return value;

            // 从ListDlg选择对象后回填数据时
            if (dataType == typeof(string))
                return value.ToString();

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
    /// 源CList.Value，目标TextBox.Text
    /// </summary>
    class ListTextConverter : IValueConverter
    {
        readonly CList _owner;

        public ListTextConverter(CList p_owner)
        {
            _owner = p_owner;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";

            if (value is string)
                return value;

            if (!string.IsNullOrEmpty(_owner.Enum))
            {
                // 将byte int等数值类型转成枚举类型，显示枚举项
                Type type = Type.GetType(_owner.Enum, false, true);
                if (type != null)
                {
                    try
                    {
                        return Enum.ToObject(type, value).ToString();
                    }
                    catch { }
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}