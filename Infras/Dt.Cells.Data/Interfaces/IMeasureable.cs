#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// IMeasureable Interface
    /// </summary>
    internal interface IMeasureable
    {
        /// <summary>
        /// Measures the string when it is not wrapped.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        Windows.Foundation.Size MeasureNoWrapString(string str, Font font);
        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <param name="allowWrap">If set to <c>true</c> [allow wrap].</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        Windows.Foundation.Size MeasureString(string str, Font font, bool allowWrap, int width);

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <value>The default font.</value>
        Font DefaultFont { get; }
    }
}

