#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent the parse exception which contains error found in parsing.
    /// </summary>
    public class CalcUnParseException : Exception
    {
        private CalcExpression _errorExpression;

        /// <summary>
        /// Initialize a new <see cref="T:Dt.CalcEngine.CalcParseException" /> with specified error information.
        /// </summary>
        /// <param name="message">The message for exception.</param>
        /// <param name="errorExpression">The number of offset to an error in string formula.</param>
        public CalcUnParseException(string message, CalcExpression errorExpression) : base(message)
        {
            this._errorExpression = errorExpression;
        }

        /// <summary>
        /// Gets the error expression.
        /// </summary>
        /// <value>
        /// An error expression.
        /// </value>
        public CalcExpression ErrorExpression
        {
            get
            {
                return this._errorExpression;
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
                return (base.Message + "\nError expressio: " + this._errorExpression.ToString());
            }
        }
    }
}

