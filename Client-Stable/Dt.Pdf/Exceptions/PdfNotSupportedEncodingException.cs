#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Pdf.Exceptions
{
    /// <summary>
    /// Not supported encoding Exception
    /// </summary>
    public class PdfNotSupportedEncodingException : PdfNotSupportedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfNotSupportedEncodingException" /> class.
        /// </summary>
        public PdfNotSupportedEncodingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfNotSupportedEncodingException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PdfNotSupportedEncodingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfNotSupportedEncodingException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfNotSupportedEncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

