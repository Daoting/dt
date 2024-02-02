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
    /// Internal Exception
    /// </summary>
    public class PdfInternalException : PdfException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfInternalException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PdfInternalException(string message) : base(message)
        {
        }
    }
}

