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
    /// Image Exception
    /// </summary>
    public class PdfImageException : PdfException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfImageException" /> class.
        /// </summary>
        public PdfImageException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfImageException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PdfImageException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfImageException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfImageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

