#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    [Flags]
    internal enum GestureType
    {
        DoubleTap = 2,
        DragComplete = 0x100,
        FreeDrag = 0x20,
        Hold = 4,
        HorizontalDrag = 8,
        None = 0,
        Pinch = 0x40,
        PinchComplete = 0x200,
        Tap = 1,
        VerticalDrag = 0x10
    }
}

