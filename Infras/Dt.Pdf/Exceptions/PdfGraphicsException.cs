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
    /// Graphics Exception of Pdf
    /// </summary>
    public class PdfGraphicsException : PdfException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfGraphicsException" /> class.
        /// </summary>
        public PdfGraphicsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfGraphicsException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PdfGraphicsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfGraphicsException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfGraphicsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

