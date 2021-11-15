#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using System;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type Null. Only has one instance.
    /// </summary>
    public class PdfNull : PdfObjectBase
    {
        public static readonly PdfNull Null = new PdfNull();
        private const string NullString = "null";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfNull" /> class.
        /// </summary>
        private PdfNull()
        {
        }

        /// <summary>
        /// Get bytes of object
        /// </summary>
        /// <returns></returns>
        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read data from Pdf reader
        /// </summary>
        /// <param name="reader">Pdf Reader</param>
        public override void ToObject(IPdfReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            writer.Psw.WriteString("null");
        }
    }
}

