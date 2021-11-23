#if WIN
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using WinRT;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 升级到WinUI3的扩展方法
    /// </summary>
    public static class WinUIExt
    {
        /// <summary>
        /// 绑定Window句柄，操
        /// </summary>
        /// <param name="p_picker"></param>
        public static void Init(this IWinRTObject p_picker)
        {
            var initializeWithWindowWrapper = p_picker.As<IInitializeWithWindow>();
            var hwnd = GetActiveWindow();
            initializeWithWindowWrapper.Initialize(hwnd);
        }

        [ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize([In] IntPtr hwnd);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetActiveWindow();
    }
}
#endif