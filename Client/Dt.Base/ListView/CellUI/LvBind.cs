#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 简洁版bind，DataContext为ViewItem
    /// </summary>
    class LvBind : Binding
    {
        public LvBind(Dot p_dot, Func<CallArgs, object> p_convert)
        {
            Converter = new FreeConverter(p_dot, p_convert);
            // Dot及内部元素的所有绑定都为 OneTime ，靠切换 DataContext 更新Dot！！！
            Mode = BindingMode.OneTime;
        }
    }

    class FreeConverter : IValueConverter
    {
        Dot _dot;
        Func<CallArgs, object> _convert;

        public FreeConverter(Dot p_dot, Func<CallArgs, object> p_convert)
        {
            _dot = p_dot;
            _convert = p_convert;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ViewItem vi)
            {
                return _convert.Invoke(new CallArgs(vi, _dot));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}