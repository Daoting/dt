#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.BaseObject;
using Dt.Pdf.Text;
using System;
#endregion

namespace Dt.Pdf.File
{
    /// <summary>
    /// Pdf file trailer
    /// </summary>
    public class PdfFileTrailer
    {
        private readonly PdfDictionary dic = new PdfDictionary();
        private static readonly byte[] eof = PdfASCIIEncoding.Instance.GetBytes("%%EOF");
        private int offset = -1;
        private static readonly byte[] startxref = PdfASCIIEncoding.Instance.GetBytes("startxref");
        private static readonly byte[] trailer = PdfASCIIEncoding.Instance.GetBytes("trailer");

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.File.PdfFileTrailer" /> class.
        /// </summary>
        public PdfFileTrailer()
        {
            this.dic.Add(PdfName.Size, new PdfNumber());
        }

        /// <summary>
        /// Toes the PDF.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void ToPdf(PdfWriter writer)
        {
            PdfArray array2 = new PdfArray();
            PdfString item = new PdfString(Guid.NewGuid().ToString()) {
                IsHexMode = true
            };
            array2.Add(item);
            PdfString str2 = new PdfString(Guid.NewGuid().ToString()) {
                IsHexMode = true
            };
            array2.Add(str2);
            PdfArray array = array2;
            this.dic[PdfName.ID] = array;
            PdfStreamWriter psw = writer.Psw;
            psw.WriteLineEnd();
            psw.WriteBytes(trailer).WriteLineEnd();
            writer.WriteObject(this.dic);
            psw.WriteLineEnd();
            psw.WriteBytes(startxref).WriteLineEnd();
            psw.WriteInt(this.Offset).WriteLineEnd();
            psw.WriteBytes(eof).WriteLineEnd();
        }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        /// <value>The info.</value>
        public PdfDictionary Info
        {
            get { return  (this.dic[PdfName.Info] as PdfDictionary); }
            set { this.dic[PdfName.Info] = value; }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public int Offset
        {
            get { return  this.offset; }
            set { this.offset = value; }
        }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        public PdfDictionary Root
        {
            get { return  (this.dic[PdfName.Root] as PdfDictionary); }
            set { this.dic[PdfName.Root] = value; }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size
        {
            get { return  ((PdfNumber) this.dic[PdfName.Size]).ValueInt; }
            set { ((PdfNumber) this.dic[PdfName.Size]).ValueInt = value; }
        }
    }
}

