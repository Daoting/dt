#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 切换垂直显示的文本
    /// </summary>
    public class VerticalTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = "";
            if (value != null)
            {
                string text = value.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    // 每个字符为独立的行
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < text.Length; i++)
                    {
                        sb.AppendLine(text.Substring(i, 1));
                    }
                    result = sb.ToString().Substring(0, sb.Length - 2);
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
