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
using Dt.Pdf.Exceptions;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The Page tree object of Pdf
    /// </summary>
    public class PdfPageTree : PdfDictionary
    {
        /// <summary>
        /// Waring: can be null.
        /// </summary>
        private readonly PdfDocument pdf;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageTree" /> class.
        /// </summary>
        public PdfPageTree()
        {
            base.Add(PdfName.Type, PdfName.Pages);
            base.Add(PdfName.Kids, new PdfArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageTree" /> class.
        /// </summary>
        /// <param name="pdf">The PDF.</param>
        internal PdfPageTree(PdfDocument pdf) : this()
        {
            this.pdf = pdf;
        }

        /// <summary>
        /// Adds the new page.
        /// </summary>
        /// <returns></returns>
        public PdfPage AddNewPage()
        {
            return this.AddNewPage(600f, 800f);
        }

        /// <summary>
        /// Adds the new page.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public PdfPage AddNewPage(float width, float height)
        {
            PdfPage dic = new PdfPage(width, height, this.pdf);
            this.AddPageOrPageTree(dic);
            return dic;
        }

        /// <summary>
        /// Adds the page or page tree.
        /// </summary>
        /// <param name="dic">The dic.</param>
        public void AddPageOrPageTree(PdfDictionary dic)
        {
            if (dic == null)
            {
                throw new PdfArgumentNullException("dic");
            }
            if (!dic.IsTypeOf(PdfName.Page) && !dic.IsTypeOf(PdfName.Pages))
            {
                throw new PdfArgumentException("dic");
            }
            if (!base.isLabeled)
            {
                base.SetLabelAndFix(true, true);
            }
            dic[PdfName.Parent] = this;
            dic.SetLabelAndFix(true, true);
            this.Kids.Add(dic);
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Count] = new PdfNumber((double) this.Kids.Count);
            base.ToPdf(writer);
        }

        /// <summary>
        /// Gets the kids.
        /// </summary>
        /// <value>The kids.</value>
        public PdfArray Kids
        {
            get { return  (PdfArray) base[PdfName.Kids]; }
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        /// <value>The page count.</value>
        public int PageCount
        {
            get { return  this.Kids.Count; }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public PdfPageTree Parent
        {
            get { return  (PdfPageTree) base[PdfName.Parent]; }
            set
            {
                if (value == null)
                {
                    base.Remove(PdfName.Parent);
                }
                else
                {
                    value.SetLabelAndFix(true, true);
                    if (base.ContainsKey(PdfName.Parent))
                    {
                        base[PdfName.Parent] = value;
                    }
                    else
                    {
                        base.Add(PdfName.Parent, value);
                    }
                }
            }
        }
    }
}

