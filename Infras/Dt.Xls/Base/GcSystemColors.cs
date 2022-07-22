#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Contains system colors.
    /// </summary>
    public static class GcSystemColors
    {
        private static uint[] _systemColors = new uint[] { 
            0xffb4b4b4, 0xff99b4d1, 0xff000000, 0xffababab, 0xfff0f0f0, 0xffa0a0a0, 0xff696969, 0xffe3e3e3, uint.MaxValue, 0xff000000, 0xff000000, 0xffb9d1ea, 0xffd7e4f2, 0xff6d6d6d, 0xff3399ff, uint.MaxValue, 
            0xff0066cc, 0xfff4f7fc, 0xffbfcddb, 0xff434e54, 0xffffffe1, 0xff000000, 0xfff0f0f0, 0xfff0f0f0, 0xff3399ff, 0xff000000, 0xffc8c8c8, uint.MaxValue, 0xff646464, 0xff000000, 0xffa0a0a0, 0xffe3e3e3, 
            0xfff0f0f0, uint.MaxValue, 0xffa0a0a0, 0xff000000
         };

        /// <summary>
        /// Get the system color at the specified system color index
        /// </summary>
        /// <param name="index">The index used to refer the GcSystemColor</param>
        /// <returns>The system color at the specified color index</returns>
        public static GcColor GetSystemColor(GcSystemColorIndex index)
        {
            return GcColor.FromArgb(_systemColors[(int) index]);
        }

        /// <summary>
        /// update the built-in system color specified by the index.
        /// </summary>
        /// <param name="index">The index used to refer the GcSystemColor</param>
        /// <param name="color">The new system color</param>
        public static void UpdateColor(GcSystemColorIndex index, GcColor color)
        {
            _systemColors[(int) index] = color.ToArgb();
        }
    }
}

