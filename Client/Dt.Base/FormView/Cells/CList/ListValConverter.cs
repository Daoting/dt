#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 源Cell.Val，目标CList.Value
    /// </summary>
    class ListValConverter : IFvCall
    {
        public object Get(Mid m)
        {
            return m.Val;
        }

        public object Set(Mid m)
        {
            if (m.Val == null)
            {
                if (m.ValType.IsEnum)
                    return default(Enum);
                return null;
            }

            // 支持可null类型
            Type tp = m.ValType;
            if (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
                tp = tp.GetGenericArguments()[0];

            // 可能为子类，如值为 RuntimeType 时
            Type tpVal = m.Val.GetType();
            if (tpVal == tp || tpVal.IsSubclassOf(tp))
                return m.Val;

            // 从ListDlg选择对象后回填数据时
            if (m.ValType == typeof(string))
                return m.Val.ToString();

            object result = null;
            try
            {
                result = System.Convert.ChangeType(m.Val, m.ValType);
            }
            catch { }
            return result;
        }
    }

    /// <summary>
    /// 源CList.Value，目标TextBox.Text
    /// </summary>
    partial class ListTextConverter : IValueConverter
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

            if (!string.IsNullOrEmpty(_owner.Ex) && _owner.Ex.StartsWith("EnumData#"))
            {
                // 将byte int等数值类型转成枚举类型，显示枚举项
                Type type = Type.GetType(_owner.Ex.Substring(9), false, true);
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