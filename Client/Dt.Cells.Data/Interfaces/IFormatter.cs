#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Interface that defines the methods and properties required by objects used as formatters.
    /// </summary>
    /// <remarks>
    /// Formatters are responsible for formatting cell contents.
    /// </remarks>
    public interface IFormatter
    {
        /// <summary>
        /// Formats the specified object as a string.
        /// </summary>
        /// <param name="obj">The object with cell data to format.</param>
        /// <returns>Returns the formatted string.</returns>
        string Format(object obj);
        /// <summary>
        /// Parses the specified string into the required object.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Returns the parsed object.</returns>
        object Parse(string str);

        /// <summary>
        /// The expression that is used to format and parse.
        /// </summary>
        string FormatString { get; }
    }
}

