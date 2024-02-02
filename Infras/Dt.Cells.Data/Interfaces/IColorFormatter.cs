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
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Interface that defines the methods and properties required by objects used to format foreground values.
    /// </summary>
    public interface IColorFormatter
    {
        /// <summary>
        /// Formats the specified object as a string with a condition foreground.
        /// </summary>
        /// <param name="obj">The object with cell data to format.</param>
        /// <param name="conditionalForeColor">The foreground of the formatted string.</param>
        /// <returns>Returns the formatted string.</returns>
        string Format(object obj, out Color? conditionalForeColor);

        /// <summary>
        /// Gets whether this formatted text contains a foreground color.
        /// </summary>
        /// <remarks>
        /// <c>true</c> if this formatted text contains a foreground color; otherwise, <c>false</c>.
        /// </remarks>
        bool HasFormatedColor { get; }
    }
}

