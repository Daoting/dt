#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-08-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Devices.Input;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// PointerRoutedEventArgs扩展类
    /// </summary>
    public static class PointerRoutedEventArgsExt
    {
        /// <summary>
        /// 是否为鼠标左键
        /// </summary>
        /// <param name="p_args"></param>
        /// <returns></returns>
        public static bool IsLeftButton(this PointerRoutedEventArgs p_args)
        {
            return p_args != null
                && p_args.Pointer.PointerDeviceType == PointerDeviceType.Mouse
                && p_args.GetCurrentPoint(null).Properties.IsLeftButtonPressed;
        }

        /// <summary>
        /// 是否为鼠标右键
        /// </summary>
        /// <param name="p_args"></param>
        /// <returns></returns>
        public static bool IsRightButton(this PointerRoutedEventArgs p_args)
        {
            return p_args != null
                && p_args.Pointer.PointerDeviceType == PointerDeviceType.Mouse
                && p_args.GetCurrentPoint(null).Properties.IsRightButtonPressed;
        }

        /// <summary>
        /// 是否为鼠标操作，false时为触摸或触摸笔
        /// </summary>
        /// <param name="p_args"></param>
        /// <returns></returns>
        public static bool IsMouse(this PointerRoutedEventArgs p_args)
        {
            return p_args != null && p_args.Pointer.PointerDeviceType == PointerDeviceType.Mouse;
        }

        /// <summary>
        /// 是否为触摸模式，false时为鼠标操作
        /// </summary>
        /// <param name="p_args"></param>
        /// <returns></returns>
        public static bool IsTouch(this PointerRoutedEventArgs p_args)
        {
            return p_args != null && p_args.Pointer.PointerDeviceType == PointerDeviceType.Touch;
        }
    }
}
