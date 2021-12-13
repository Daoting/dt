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
    /// The information of Pdf file
    /// </summary>
    public class PdfInfo : PdfDictionary, IVersionDepend
    {
        private string author;
        private PdfDate creationDate = new PdfDate();
        private string creator;
        private string keywords;
        private PdfDate modDate = new PdfDate();
        private string producer = "Daoting Pdf Library";
        private string subject;
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfInfo" /> class.
        /// </summary>
        public PdfInfo()
        {
            base.isLabeled = true;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (!string.IsNullOrEmpty(this.title))
            {
                base[PdfName.Title] = new PdfString(this.title);
            }
            if (!string.IsNullOrEmpty(this.author))
            {
                base[PdfName.Author] = new PdfString(this.author);
            }
            if (!string.IsNullOrEmpty(this.subject))
            {
                base[PdfName.Subject] = new PdfString(this.subject);
            }
            if (!string.IsNullOrEmpty(this.keywords))
            {
                base[PdfName.Keywords] = new PdfString(this.keywords);
            }
            base[PdfName.Creator] = new PdfString(this.creator);
            base[PdfName.Producer] = new PdfString(this.producer);
            base[PdfName.CreationDate] = this.creationDate;
            base[PdfName.ModDate] = this.modDate;
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            return PdfVersion.PDF1_1;
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        public string Author
        {
            get { return  this.author; }
            set { this.author = value; }
        }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>The creation date.</value>
        internal PdfDate CreationDate
        {
            get { return  this.creationDate; }
            set { this.creationDate = value; }
        }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        public string Creator
        {
            get { return  this.creator; }
            set { this.creator = value; }
        }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public string Keywords
        {
            get { return  this.keywords; }
            set { this.keywords = value; }
        }

        /// <summary>
        /// Gets or sets the mod date.
        /// </summary>
        /// <value>The mod date.</value>
        internal PdfDate ModDate
        {
            get { return  this.modDate; }
            set { this.modDate = value; }
        }

        /// <summary>
        /// Gets or sets the producer.
        /// </summary>
        /// <value>The producer.</value>
        internal string Producer
        {
            get { return  this.producer; }
            set { this.producer = value; }
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get { return  this.subject; }
            set { this.subject = value; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return  this.title; }
            set { this.title = value; }
        }
    }
}

