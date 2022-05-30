#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal static class RectHelper
    {
        public static Rect Expand(this Rect rect, int width, int height)
        {
            if (rect.IsEmpty)
            {
                return rect;
            }
            return new Rect { X = ((rect.X - width) > 0.0) ? (rect.X - width) : 0.0, Y = ((rect.Y - height) > 0.0) ? (rect.Y - height) : 0.0, Width = rect.Width + (2 * width), Height = rect.Height + (2 * height) };
        }
    }
}

