#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-12-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// PhonePage附加依赖属性
    /// </summary>
    internal static class PhonePageEx
    {
        /// <summary>
        /// 所属页面
        /// </summary>
        public static readonly DependencyProperty ParentPageProperty = DependencyProperty.RegisterAttached(
            "ParentPage",
            typeof(PhonePage),
            typeof(PhonePageEx),
            new PropertyMetadata(null));

        public static PhonePage GetParentPage(FrameworkElement element)
        {
            return (PhonePage)element.GetValue(ParentPageProperty);
        }

        public static void SetParentPage(FrameworkElement element, PhonePage value)
        {
            element.SetValue(ParentPageProperty, value);
        }
    }
}