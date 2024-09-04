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
    /// Incorrect font data Exception
    /// </summary>
    public class PdfIncorrectFontData : PdfException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfIncorrectFontData" /> class.
        /// </summary>
        public PdfIncorrectFontData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfIncorrectFontData" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PdfIncorrectFontData(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfIncorrectFontData" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfIncorrectFontData(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

