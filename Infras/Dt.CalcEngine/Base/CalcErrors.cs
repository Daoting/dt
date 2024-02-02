#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent a <see cref="T:Dt.CalcEngine.CalcErrors" /> which list all supported errors.
    /// </summary>
    public static class CalcErrors
    {
        internal const int DIVIDE_BY_ERROR = 7;
        /// <summary>
        /// Indicates a divide by zero error.
        /// </summary>
        public static readonly CalcError DivideByZero = new CalcDivideByZeroError();
        /// <summary>
        /// Indicates a name error.
        /// </summary>
        public static readonly CalcError Name = new CalcNameError();
        internal const int NAME = 0x1d;
        internal const int NOT_AVAILABLE = 0x2a;
        /// <summary>
        /// Indicates a not available error.
        /// </summary>
        public static readonly CalcError NotAvailable = new CalcNotAvailableError();
        /// <summary>
        /// Indicates a null error.
        /// </summary>
        public static readonly CalcError Null = new CalcNullError();
        internal const int NULL = 0;
        /// <summary>
        /// Indicates a number error.
        /// </summary>
        public static readonly CalcError Number = new CalcNumberError();
        internal const int NUMBER = 0x24;
        /// <summary>
        /// Indicates a reference error.
        /// </summary>
        public static readonly CalcError Reference = new CalcReferenceError();
        internal const int REFERENCE = 0x17;
        internal static readonly Dictionary<string, CalcError> Tables = new Dictionary<string, CalcError>();
        internal const int UNKNOWN_ERROR_CODE = -1;
        /// <summary>
        /// Indicates a value error.
        /// </summary>
        public static readonly CalcError Value = new CalcValueError();
        internal const int VALUE = 15;

        static CalcErrors()
        {
            Tables.Add("#NULL!", Null);
            Tables.Add("#DIV/0!", DivideByZero);
            Tables.Add("#VALUE!", Value);
            Tables.Add("#REF!", Reference);
            Tables.Add("#NAME?", Name);
            Tables.Add("#N/A", NotAvailable);
            Tables.Add("#NUM!", Number);
        }

        /// <summary>
        /// Parses the specified error from string.
        /// </summary>
        /// <param name="error">The error string.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>A <see cref="T:Dt.CalcEngine.CalcError" /> indicates the known error. if not supported, return <see langword="null" />.</returns>
        public static CalcError Parse(string error, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(error))
            {
                CalcError error2;
                if (Tables.TryGetValue(error, out error2))
                {
                    return error2;
                }
                error = error.ToUpper((culture != null) ? culture : CultureInfo.InvariantCulture);
                if (Tables.TryGetValue(error, out error2))
                {
                    return error2;
                }
            }
            return null;
        }
    }
}

