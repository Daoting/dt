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
    /// Unrecognized font Exception
    /// </summary>
    public class PdfUnrecognizedFontException : PdfException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfUnrecognizedFontException" /> class.
        /// </summary>
        public PdfUnrecognizedFontException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfUnrecognizedFontException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PdfUnrecognizedFontException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfUnrecognizedFontException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfUnrecognizedFontException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

