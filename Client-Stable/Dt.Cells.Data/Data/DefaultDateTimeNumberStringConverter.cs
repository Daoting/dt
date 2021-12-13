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
    /// Represents a default date time converter class that converts number strings to Japanese or Chinese. 
    /// </summary>
    internal class DefaultDateTimeNumberStringConverter : INumberStringConverter
    {
        /// <summary>
        /// Converts a specified number string to another representation.
        /// </summary>
        /// <param name="number">The formatted data string.</param>
        /// <param name="value">The original data value.</param>
        /// <param name="isGeneralNumber">Whether the number is a general format number.</param>
        /// <param name="locale">A <see cref="T:Dt.Cells.Data.LocaleIDFormatPart" /> object that specifies the locale information.</param>
        /// <param name="dbNumber">A <see cref="T:Dt.Cells.Data.DBNumberFormatPart" /> object that specifies the number format information.</param>
        /// <returns>
        /// Returns the string that represents the original data value.
        /// </returns>
        public string ConvertTo(string number, object value, bool isGeneralNumber, LocaleIDFormatPart locale, DBNumberFormatPart dbNumber)
        {
            string s = number;
            if (((locale != null) && (dbNumber != null)) && (value is DateTime))
            {
                DBNumber dBNumber = locale.GetDBNumber(dbNumber.Type);
                DateTime time = (DateTime) value;
                DateTime time2 = (DateTime) value;
                s = dbNumber.ReplaceNumberString(s, dBNumber, true).Replace(DefaultTokens.ReplacePlaceholder + NumberFormatDateTime.YearFourDigit, time.ToString(NumberFormatDateTime.YearFourDigit)).Replace(DefaultTokens.ReplacePlaceholder + NumberFormatDateTime.YearTwoDigit, time2.ToString(NumberFormatDateTime.YearTwoDigit));
                s = dbNumber.ReplaceNumberString(s, dBNumber, false);
            }
            return s;
        }
    }
}

