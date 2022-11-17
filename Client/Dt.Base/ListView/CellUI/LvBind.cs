#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 简洁版bind，DataContext为ViewItem
    /// </summary>
    public class LvBind : Binding
    {
        
        public LvBind(Env p_env, Func<ConverterArgs, object> p_convert)
        {
            if (p_env != null && p_convert != null)
            {
                Converter = new FreeConverter(p_env.Dot, p_convert);
                // 自动置绑定标志
                p_env.IsBinding = true;
            }
        }
        internal LvBind(Dot p_dot, Func<ConverterArgs, object> p_convert)
        {
            if (p_dot != null && p_convert != null)
                Converter = new FreeConverter(p_dot, p_convert);
        }
    }

    class FreeConverter : IValueConverter
    {
        Dot _dot;
        Func<ConverterArgs, object> _convert;

        public FreeConverter(Dot p_dot, Func<ConverterArgs, object> p_convert)
        {
            _dot = p_dot;
            _convert = p_convert;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ViewItem vi)
            {
                return _convert.Invoke(new ConverterArgs(vi, _dot));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}