#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    internal static class Extension
    {
        static Windows.UI.Xaml.CornerRadius DefaultCornerRadius = new Windows.UI.Xaml.CornerRadius();
        static Windows.UI.Xaml.Thickness DefaultThickness = new Windows.UI.Xaml.Thickness();

        public static bool IsDefault(this Windows.UI.Xaml.CornerRadius cr)
        {
            return (cr == DefaultCornerRadius);
        }

        public static bool IsDefault(this Windows.UI.Xaml.Thickness cr)
        {
            return (cr == DefaultThickness);
        }

        public static bool IsDefaultForeground(this object value, string key, object lvalue)
        {
            return (key.Contains("Foreground") && !(lvalue is SolidColorBrush));
        }
    }
}

