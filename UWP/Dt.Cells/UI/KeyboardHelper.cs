#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    internal class KeyboardHelper
    {
        KeyboardHelper()
        {
        }

        public static void GetMetaKeyState(out bool shift, out bool ctrl)
        {
            CoreVirtualKeyStates keyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            ctrl = (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            CoreVirtualKeyStates states2 = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
            shift = (states2 & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        public static void GetMetaKeyState(out bool shift, out bool ctrl, out bool alt)
        {
            GetMetaKeyState(out shift, out ctrl);
            CoreVirtualKeyStates keyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu);
            alt = (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}

