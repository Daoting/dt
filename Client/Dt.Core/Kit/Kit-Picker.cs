#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using Windows.Storage.Pickers;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 获取 FileOpenPicker, FileSavePicker, FolderPicker 对象，升级WinUI后增加Window句柄
    /// </summary>
    public partial class Kit
    {
        public static FileOpenPicker GetFileOpenPicker()
        {
            var picker = new FileOpenPicker();
#if WIN
            Init(picker);
#endif
            return picker;
        }

        public static FileSavePicker GetFileSavePicker()
        {
            var picker = new FileSavePicker();
#if WIN
            Init(picker);
#endif
            return picker;
        }

        public static FolderPicker GetFolderPicker()
        {
            var picker = new FolderPicker();
#if WIN
            Init(picker);
#endif
            return picker;
        }

#if WIN
        /// <summary>
        /// 绑定Window句柄，操
        /// </summary>
        /// <param name="p_picker"></param>
        static void Init(object p_picker)
        {
            var initializeWithWindowWrapper = WinRT.CastExtensions.As<IInitializeWithWindow>(p_picker);
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
#endif
    }
}