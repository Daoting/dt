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
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The dictionary form of file specification provides more flexibility than 
    /// the string form, allowing different files to be specified for different 
    /// file systems or platforms, or for file systems other than the standard 
    /// ones (DOS/Windows, Mac OS, and UNIX).
    /// </summary>
    public class PdfFileSpecification : PdfDictionary
    {
        /// <summary>
        /// file name
        /// </summary>
        private string fileName;
        /// <summary>
        /// file stream
        /// </summary>
        private readonly PdfEmbeddedFileStream fileStream = new PdfEmbeddedFileStream();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFileSpecification" /> class.
        /// </summary>
        public PdfFileSpecification()
        {
            base[PdfName.Type] = PdfName.Filespec;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            PdfString str = new PdfString(this.fileName) {
                IsHexMode = true
            };
            base[PdfName.F] = str;
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Add(PdfName.F, this.fileStream);
            base[PdfName.EF] = dictionary;
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return  this.fileName; }
            set { this.fileName = value; }
        }

        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <value>The file stream.</value>
        public PdfEmbeddedFileStream FileStream
        {
            get { return  this.fileStream; }
        }
    }
}

