#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DrawRectInfo
    {
        public Windows.Foundation.Rect rect;
        public Brush brush;
    }
}

