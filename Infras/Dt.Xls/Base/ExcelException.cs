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
    internal class ExcelException : Exception
    {
        private ExcelExceptionCode code;

        /// <summary>
        /// Default constructor - internal use
        /// </summary>
        internal ExcelException()
        {
        }

        /// <summary>
        /// Constructor - internal use
        /// </summary>
        /// <param name="message"></param>
        internal ExcelException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor - internal use
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        internal ExcelException(string message, ExcelExceptionCode code) : base(message)
        {
            this.code = code;
        }

        /// <summary>
        /// Constructor - internal use
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        internal ExcelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets the ExcelExceptionCode enumeration member set for this Excel exception.
        /// </summary>
        public ExcelExceptionCode Code
        {
            get { return  this.code; }
        }

        /// <summary>
        /// Gets the text message associated with this Excel exception.
        /// </summary>
        public override string Message
        {
            get { return  base.Message; }
        }
    }
}

