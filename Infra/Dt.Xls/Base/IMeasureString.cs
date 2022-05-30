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
    /// Defines a generalized method that a value or class implements to create a type-specified method for measuring strings.
    /// </summary>
    public interface IMeasureString
    {
        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="text">The string to be measured.</param>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="wordWrap">if set to <see langword="true" />, word-wrap.</param>
        /// <param name="width">The width of the word-wrap rectangle.</param>
        /// <returns>Returns the size of the string.</returns>
        GcSize MeasureString(string text, string fontName, double fontSize, bool wordWrap, double width);
    }
}

