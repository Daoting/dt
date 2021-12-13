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
    /// Object internal Exception
    /// </summary>
    public class PdfObjectInternalException : PdfException
    {
        private PdfObjectExceptionType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfObjectInternalException" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public PdfObjectInternalException(PdfObjectExceptionType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Object exception type
        /// </summary>
        public enum PdfObjectExceptionType
        {
            Unknown,
            ChangeFixedObjectLabel,
            Cast
        }
    }
}

