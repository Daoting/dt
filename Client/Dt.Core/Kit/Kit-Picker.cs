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
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWin);
            WinRT.Interop.InitializeWithWindow.Initialize(p_picker, hWnd);
        }
#endif
    }
}