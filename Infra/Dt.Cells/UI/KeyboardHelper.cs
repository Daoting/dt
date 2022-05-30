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
using Microsoft.UI.Xaml;
using Microsoft.UI.Input;
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
            // WinUI
            var keyState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
            ctrl = (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            var states2 = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
            shift = (states2 & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        public static void GetMetaKeyState(out bool shift, out bool ctrl, out bool alt)
        {
            // WinUI
            GetMetaKeyState(out shift, out ctrl);
            var keyState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu);
            alt = (keyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}

