#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Icons枚举值转unicode字符
    /// </summary>
    public class IconToUnicodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                try
                {
                    Icons icon = (Icons)value;
                    return Res.GetIconChar(icon);
                }
                catch { }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Icons枚举值名称转unicode字符
    /// </summary>
    public class IconNameToUnicodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Icons icon = Icons.None;
            if (value != null)
                Enum.TryParse<Icons>(value.ToString(), true, out icon);
            return Res.GetIconChar(icon);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 用于显示icon的name
    /// </summary>
    public class IconToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                try
                {
                    Icons icon = (Icons)value;
                    return icon.ToString();
                }
                catch { }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

