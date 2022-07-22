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
    /// Interface that defines the methods that convert a number string to another number string.
    /// </summary>
    internal interface INumberStringConverter
    {
        /// <summary>
        /// Converts a specified number string to a different representation.
        /// </summary>
        /// <param name="number">The formatted data string.</param>
        /// <param name="value">The original data value.</param>
        /// <param name="isGeneralNumber">Whether the number is a general format number.</param>
        /// <param name="locale">The locale information.</param>
        /// <param name="dbNumber">The number format information.</param>
        /// <returns>
        /// Returns the string that represents the original data value.
        /// </returns>
        string ConvertTo(string number, object value, bool isGeneralNumber, LocaleIDFormatPart locale, DBNumberFormatPart dbNumber);
    }
}

