#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent the parse exception which contains error found in parsing.
    /// </summary>
    public class CalcParseException : Exception
    {
        private int _errorOffset;

        /// <summary>
        /// Initialize a new <see cref="T:Dt.CalcEngine.CalcParseException" /> with specified error information.
        /// </summary>
        /// <param name="message">The message for exception.</param>
        /// <param name="errorOffset">The number of offset to an error in string formula.</param>
        public CalcParseException(string message, int errorOffset) : base(message)
        {
            this._errorOffset = errorOffset;
        }

        /// <summary>
        /// Gets a number of offset to an error in string formula.
        /// </summary>
        public int ErrorOffset
        {
            get
            {
                return this._errorOffset;
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value>The error message that explains the reason for the exception, or an empty string("").</value>
        public override string Message
        {
            get
            {
                return (base.Message + "\n" + "Exceptions.ErrorOffset" + ((int) this._errorOffset).ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}

