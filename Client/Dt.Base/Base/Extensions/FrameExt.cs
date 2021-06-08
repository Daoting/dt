#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-05-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Frame扩展类
    /// </summary>
    public static class FrameExt
    {
        /// <summary>
        /// 在Frame页面导航时自动设置可水平滑屏
        /// </summary>
        /// <param name="p_frame"></param>
        public static void AllowTranslateX(this Frame p_frame)
        {
            if (p_frame != null)
                p_frame.Navigated += OnFrameNavigated;
        }

        /// <summary>
        /// 完成导航时对内容附加事件中设置可水平滑屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            FrameworkElement elem = e.Content as FrameworkElement;
            if (elem != null)
                elem.Loaded += OnContentLoaded;
        }

        /// <summary>
        /// 内容加载时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void OnContentLoaded(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            elem.Loaded -= OnContentLoaded;
            Kit.RunAsync(() => elem.AllowTranslateX());
        }
    }
}
