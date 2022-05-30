#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an actual value object.
    /// </summary>
    public interface IActualValue
    {
        /// <summary>
        /// Gets the color of the background.
        /// </summary>
        /// <returns>The color of the background.</returns>
        Color? GetBackgroundColor();
        /// <summary>
        /// Gets the default color of the background.
        /// </summary>
        /// <returns>The default color of the background.</returns>
        Color? GetDefaultBackgroundColor();
        /// <summary>
        /// Gets the default color of the foreground.
        /// </summary>
        /// <returns>The default color of the foreground.</returns>
        Color? GetDefaultForegroundColor();
        /// <summary>
        /// Gets the color of the foreground.
        /// </summary>
        /// <returns>The color of the foreground.</returns>
        Color? GetForegroundColor();
        /// <summary>
        /// Gets the string representation of the value.
        /// </summary>
        /// <returns>The string representation of the value.</returns>
        string GetText();
        /// <summary>
        /// Gets the real value.
        /// </summary>
        /// <returns>The real value.</returns>
        object GetValue();
        /// <summary>
        /// Gets the real value as a date.
        /// </summary>
        /// <param name="type">The type of the value</param>
        /// <returns>The date time value.</returns>
        object GetValue(NumberFormatType type);
        /// <summary>
        /// Gets the value with a specified row and column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        object GetValue(int row, int column);
    }
}

