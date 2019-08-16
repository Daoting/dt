#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// IsSelected -> 字符图标
    /// </summary>
    class IsSelectedIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "\uE06A" : "\uE068";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// IsSelected -> 背景
    /// </summary>
    class SelectedBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? AtRes.暗遮罩 : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// IsSelected -> 背景
    /// </summary>
    class HeaderBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? AtRes.黄色背景 : AtRes.浅灰背景;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Dot根据内容类型转换
    /// </summary>
    class DotContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return null;

            Dot dot = (Dot)parameter;

            // 取列值
            object val = null;
            if (value is Row dr && dr.Contains(dot.ID))
            {
                // 从Row取
                val = dr[dot.ID];
            }
            else
            {
                // 对象属性
                var pi = value.GetType().GetProperty(dot.ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                    val = pi.GetValue(value);
            }
            if (val == null || val.ToString() == "")
                return null;

            // 根据类型生成UI
            TextBlock tb;
            if (dot.To == DotContentType.Icon)
            {
                // 图标
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    FontFamily = AtRes.IconFont,
                    TextAlignment = TextAlignment.Center,
                };

                if (val is int)
                    tb.Text = AtRes.GetIconChar((Icons)val);
                else
                    tb.Text = AtRes.ParseIconChar(val.ToString());
                return tb;
            }

            if (dot.To == DotContentType.CheckBox)
            {
                // 字符模拟CheckBox
                tb = new TextBlock
                {
                    Style = AtRes.LvTextBlock,
                    FontFamily = AtRes.IconFont,
                    TextAlignment = TextAlignment.Center,
                };

                bool b;
                if (val is bool)
                {
                    b = (bool)val;
                }
                else
                {
                    string temp = val.ToString().ToLower();
                    b = (temp == "1" || temp == "true");
                }
                tb.Text = b ? "\uE06A" : "\uE068";
                return tb;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
