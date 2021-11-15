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
    /// The outline object of Pdf
    /// </summary>
    public class PdfOutline : PdfDictionary
    {
        private readonly List<PdfOutlineItem> items = new List<PdfOutlineItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfOutline" /> class.
        /// </summary>
        public PdfOutline()
        {
            base[PdfName.Type] = PdfName.Outlines;
            base.isLabeled = true;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            if (this.items.Count > 0)
            {
                for (int i = 0; i < this.items.Count; i++)
                {
                    PdfOutlineItem item = this.items[i];
                    item.Parent = this;
                    int num2 = i + 1;
                    item.BakTitle = ((int) num2).ToString();
                    if (i > 0)
                    {
                        item.Prev = this.items[i - 1];
                    }
                    if (i < (this.items.Count - 1))
                    {
                        item.Next = this.items[i + 1];
                    }
                    writer.WriteObject(item, false);
                }
                base[PdfName.First] = this.items[0];
                base[PdfName.Last] = this.items[this.items.Count - 1];
                base[PdfName.Count] = new PdfNumber((double) this.items.Count);
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public List<PdfOutlineItem> Items
        {
            get { return  this.items; }
        }
    }
}

