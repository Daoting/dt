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
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The outline item of Pdf file
    /// </summary>
    public class PdfOutlineItem : PdfDictionary
    {
        private string bakTitle;
        private readonly List<PdfOutlineItem> childItems;
        private const string defaultTitle = "Bookmark";
        private PdfDestination dest;
        private PdfOutlineItem next;
        private PdfDictionary parent;
        private PdfOutlineItem prev;
        private string title;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOutlineItem" /> class.
        /// </summary>
        public PdfOutlineItem()
        {
            this.childItems = new List<PdfOutlineItem>();
            base.isLabeled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOutlineItem" /> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PdfOutlineItem(PdfPage page) : this(null, page)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOutlineItem" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        public PdfOutlineItem(string title) : this()
        {
            this.title = title;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOutlineItem" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="dest">The dest.</param>
        public PdfOutlineItem(string title, PdfDestination dest) : this(title)
        {
            this.dest = dest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOutlineItem" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="page">The page.</param>
        public PdfOutlineItem(string title, PdfPage page) : this(title, new PdfXYZDestination(page, 0f, page.MediaBox.Height, 0f))
        {
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.childItems.Count > 0)
            {
                for (int i = 0; i < this.childItems.Count; i++)
                {
                    PdfOutlineItem item = this.childItems[i];
                    item.Parent = this;
                    item.BakTitle = this.bakTitle + "." + ((int) (i + 1));
                    if (i > 0)
                    {
                        item.Prev = this.childItems[i - 1];
                    }
                    if (i < (this.childItems.Count - 1))
                    {
                        item.Next = this.childItems[i + 1];
                    }
                    writer.WriteObject(item, false);
                }
                base[PdfName.First] = this.childItems[0];
                base[PdfName.Last] = this.childItems[this.childItems.Count - 1];
                base[PdfName.Count] = new PdfNumber((double) this.childItems.Count);
            }
            if (string.IsNullOrEmpty(this.title))
            {
                base[PdfName.Title] = new PdfString("Bookmark " + this.bakTitle);
            }
            else
            {
                base[PdfName.Title] = new PdfString(this.title);
            }
            base[PdfName.Parent] = this.parent;
            if (this.prev != null)
            {
                base[PdfName.Prev] = this.prev;
            }
            if (this.next != null)
            {
                base[PdfName.Next] = this.next;
            }
            if (this.dest.TargetPage != null)
            {
                base[PdfName.Dest] = this.dest;
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets or sets the bak title.
        /// </summary>
        /// <value>The bak title.</value>
        internal string BakTitle
        {
            get { return  this.bakTitle; }
            set { this.bakTitle = value; }
        }

        /// <summary>
        /// Gets the child items.
        /// </summary>
        /// <value>The child items.</value>
        public List<PdfOutlineItem> ChildItems
        {
            get { return  this.childItems; }
        }

        /// <summary>
        /// Gets or sets the dest.
        /// </summary>
        /// <value>The dest.</value>
        public PdfDestination Dest
        {
            get { return  this.dest; }
            set { this.dest = value; }
        }

        /// <summary>
        /// Sets the next.
        /// </summary>
        /// <value>The next.</value>
        internal PdfOutlineItem Next
        {
            set { this.next = value; }
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        internal PdfDictionary Parent
        {
            set { this.parent = value; }
        }

        /// <summary>
        /// Sets the prev.
        /// </summary>
        /// <value>The prev.</value>
        internal PdfOutlineItem Prev
        {
            set { this.prev = value; }
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

