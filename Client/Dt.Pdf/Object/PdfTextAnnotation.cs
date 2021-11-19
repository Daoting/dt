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
    /// Pdf Text Annotation
    /// </summary>
    public class PdfTextAnnotation : PdfAnnotationBase
    {
        private IconType icon;
        private bool isOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTextAnnotation" /> class.
        /// </summary>
        public PdfTextAnnotation() : base(PdfName.Text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTextAnnotation" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public PdfTextAnnotation(string text) : this()
        {
            base.Contents = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfTextAnnotation" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="isOpen">if set to <c>true</c> [is open].</param>
        public PdfTextAnnotation(string text, IconType icon, bool isOpen) : this(text)
        {
            this.icon = icon;
            this.isOpen = isOpen;
        }

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteProperties(PdfWriter writer)
        {
            if (this.isOpen)
            {
                base[PdfName.Open] = PdfBool.TRUE;
            }
            if (this.icon != IconType.Note)
            {
                base[PdfName.Name_] = new PdfName(this.icon.ToString(), true);
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public IconType Icon
        {
            get { return  this.icon; }
            set { this.icon = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return  this.isOpen; }
            set { this.isOpen = value; }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return  base.Contents; }
            set { base.Contents = value; }
        }

        /// <summary>
        /// Icons
        /// </summary>
        public enum IconType
        {
            Note,
            Comment,
            Key,
            Help,
            NewParagraph,
            Paragraph,
            Insert
        }
    }
}

