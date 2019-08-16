using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Dt.Base
{
    /// <summary>
    /// menu下面的功能信息提示信息转换器，在dt.core中只返回string类型的提示信息，要转成textblock。
    /// </summary>
    public class MenuNoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value.GetType() == typeof(string))
            {
                TextBlock tb = new TextBlock();
                tb.Text = value as string;
                tb.FontSize = 12;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.TextTrimming = TextTrimming.CharacterEllipsis;
                return tb;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
