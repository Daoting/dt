#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Defines an abstract class for report sections that can display in multiple pages.
    /// </summary>
    internal abstract class GcMultiplePageSection : GcSection
    {
        BreakType pageBreak;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcMultiplePageSection" /> class.
        /// </summary>
        internal GcMultiplePageSection()
        {
            this.Init();
        }

        /// <summary>
        /// Generates the pages.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="continuePage">If set to <c>true</c>, [continue page]</param>
        /// <param name="offset">The offset</param>
        /// <param name="pageHeaderOffset">The page header offset</param>
        /// <returns></returns>
        internal virtual bool GeneratePages(GcReportContext context, bool continuePage, ref double offset, ref double pageHeaderOffset)
        {
            int num;
            GcPageHeaderSection pageHeader = context.PageHeader;
            GcPageFooterSection pageFooter = context.PageFooter;
            GcSection.GcSectionCache pageHeaderCache = context.PageHeaderCache;
            GcSection.GcSectionCache pageFooterCache = context.PageFooterCache;
            List<int> horizontalPageBreaks = this.GetHorizontalPageBreaks();
            GcSection.GcSectionCache cache = base.GetCache(context);
            Windows.Foundation.Size allSize = base.GetAllSize(cache);
            PaperCutter cutter = new PaperCutter((int) context.PageRects.CropRectangle.Height, (int) allSize.Height, (IEnumerable<int>) horizontalPageBreaks);
            bool flag = this is IGcAllowAppendixSection;
            if (flag)
            {
                if (pageHeader != null)
                {
                    cutter.MaxLength -= (int) pageHeader.GetAllSize(pageHeaderCache).Height;
                }
                if (pageFooter != null)
                {
                    cutter.MaxLength -= (int) pageFooter.GetAllSize(pageFooterCache).Height;
                }
            }
            if (this.PageBreakBefore || (offset >= cutter.MaxLength))
            {
                continuePage = false;
            }
            if (!continuePage)
            {
                offset = 0.0;
                pageHeaderOffset = 0.0;
            }
            List<GcPageBlock> list2 = (continuePage && (context.Pages.Count > 0)) ? context.Pages[context.Pages.Count - 1] : new List<GcPageBlock>();
            if (continuePage)
            {
                cutter.AddBreakIfFirst((int) (cutter.MaxLength - offset));
            }
            object buildInControlState = this.CreateBuildInControlState(context);
            double num2 = offset;
            bool horizontal = false;
            int num3 = 0;
            while (cutter.Next(out num) || this.HasMorePage(buildInControlState, horizontal))
            {
                int num6;
                List<int> verticalPageBreaks = this.GetVerticalPageBreaks();
                int width = (int) allSize.Width;
                if (flag)
                {
                    width = Math.Max(width, context.PageHeaderFooterMaxWidth);
                }
                PaperCutter cutter2 = new PaperCutter((int) context.PageRects.CropRectangle.Width, width, (IEnumerable<int>) verticalPageBreaks);
                int num5 = 0;
                while (cutter2.Next(out num6) || this.HasMorePage(buildInControlState, horizontal))
                {
                    if (num5 >= list2.Count)
                    {
                        list2.Add(context.CreateNewPage());
                    }
                    GcPageBlock block = list2[num5];
                    int x = cutter2.Current - num6;
                    int y = cutter.Current - num;
                    int height = 0;
                    if (flag)
                    {
                        if ((pageHeader != null) && (block.PageHeader == null))
                        {
                            height = (int) pageHeader.GetAllSize(pageHeaderCache).Height;
                        }
                        if (pageFooter != null)
                        {
                            num3 = (int) pageFooter.GetAllSize(pageFooterCache).Height;
                        }
                        if ((height + num3) >= num)
                        {
                            num3 = 0;
                        }
                        if ((height + num3) >= num)
                        {
                            height = 0;
                        }
                    }
                    else if ((pageFooter != null) && (block.PageFooter != null))
                    {
                        num3 = (int) pageFooter.GetAllSize(pageFooterCache).Height;
                        if ((height + num3) >= num)
                        {
                            num3 = 0;
                        }
                    }
                    GcRangeBlock block2 = base.GetRange(context, x, y, num6, num, cache, buildInControlState, horizontal, continuePage);
                    if (flag)
                    {
                        if (((pageHeader != null) && (block.PageHeader == null)) && (height > 0))
                        {
                            block.PageHeader = pageHeader.GetRange(context, pageHeader.HorizontalExtend ? x : 0, 0, (int) context.PageRects.PageHeaderRectangle.Width, (int) context.PageRects.PageHeaderRectangle.Height, pageHeaderCache, buildInControlState, horizontal, continuePage);
                            if (continuePage)
                            {
                                GcRangeBlock block1 = block.PageHeader;
                                block1.Y += pageHeaderOffset;
                            }
                        }
                        if (((pageFooter != null) && (block.PageFooter == null)) && (num3 > 0))
                        {
                            block.PageFooter = pageFooter.GetRange(context, pageFooter.HorizontalExtend ? x : 0, 0, (int) context.PageRects.PageFooterRectangle.Width, (int) context.PageRects.PageFooterRectangle.Height, pageFooterCache, buildInControlState, horizontal, continuePage);
                            block.PageFooter.Y = ((int) context.PageRects.CropRectangle.Height) - num3;
                        }
                    }
                    if ((context.TopMargin != null) && (block.TopMargin == null))
                    {
                        block.TopMargin = context.TopMargin.GetRange(context, context.TopMargin.HorizontalExtend ? x : 0, 0, (int) context.PageRects.TopMarginRectangle.Width, (int) context.PageRects.TopMarginRectangle.Height, context.TopMarginCache, buildInControlState, horizontal, continuePage);
                        block.TopMargin.X = context.PageRects.TopMarginRectangle.X;
                        block.TopMargin.Y = context.PageRects.TopMarginRectangle.Y;
                    }
                    if ((context.BottomMargin != null) && (block.BottomMargin == null))
                    {
                        block.BottomMargin = context.BottomMargin.GetRange(context, context.BottomMargin.HorizontalExtend ? x : 0, 0, (int) context.PageRects.BottomMarginRectangle.Width, (int) context.PageRects.BottomMarginRectangle.Height, context.BottomMarginCache, buildInControlState, horizontal, continuePage);
                        block.BottomMargin.X = context.PageRects.BottomMarginRectangle.X;
                        block.BottomMargin.Y = context.PageRects.BottomMarginRectangle.Y;
                    }
                    block2.Y += height;
                    if (continuePage)
                    {
                        block2.Y += num2;
                    }
                    if ((!flag && (pageFooter != null)) && (block.PageFooter != null))
                    {
                        block.PageFooter.Y = block2.Y;
                        block2.Y += num3;
                    }
                    block.Blocks.Add(block2);
                    if (!horizontal)
                    {
                        offset = (num2 + block2.Height) + height;
                    }
                    num5++;
                    horizontal = true;
                }
                pageHeaderOffset = 0.0;
                num2 = 0.0;
                horizontal = false;
                continuePage = false;
                if (!context.Pages.Contains(list2))
                {
                    context.Pages.Add(list2);
                }
                list2 = new List<GcPageBlock>();
            }
            continuePage = !this.PageBreakAfter;
            if (continuePage && (offset >= (context.PageRects.CropRectangle.Height - num3)))
            {
                continuePage = !continuePage;
            }
            if (!continuePage)
            {
                offset = 0.0;
            }
            pageHeaderOffset = flag ? 0.0 : offset;
            return continuePage;
        }

        /// <summary>
        /// Gets the horizontal page breaks.
        /// </summary>
        /// <returns></returns>
        internal virtual List<int> GetHorizontalPageBreaks()
        {
            List<GcHorizontalPageBreak> controls = this.Controls.GetControls<GcHorizontalPageBreak>();
            List<int> list2 = new List<int>();
            foreach (GcHorizontalPageBreak @break in controls)
            {
                list2.Add(@break.Y);
            }
            list2.Sort();
            return list2;
        }

        /// <summary>
        /// Gets the vertical page breaks.
        /// </summary>
        /// <returns></returns>
        internal virtual List<int> GetVerticalPageBreaks()
        {
            List<GcVerticalPageBreak> controls = this.Controls.GetControls<GcVerticalPageBreak>();
            List<int> list2 = new List<int>();
            foreach (GcVerticalPageBreak @break in controls)
            {
                list2.Add(@break.X);
            }
            list2.Sort();
            return list2;
        }

        /// <summary>
        /// Determines whether [has more page] [the specified build in control state].
        /// </summary>
        /// <param name="buildInControlState">State of the built-in control.</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <returns>
        /// <c>true</c> if [has more page] [the specified build in control state]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool HasMorePage(object buildInControlState, bool horizontal)
        {
            return false;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.pageBreak = BreakType.None;
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void ReadXmlBase(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                string str;
                if (((str = reader.Name) != null) && (str == "PageBreak"))
                {
                    this.pageBreak = Serializer.ReadAttributeEnum<BreakType>("value", BreakType.None, reader);
                }
                else
                {
                    base.ReadXmlBase(reader);
                }
            }
        }

        /// <summary>
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteXmlBase(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            base.WriteXmlBase(writer);
        }

        /// <summary>
        /// Gets or sets a value that determines where to make a page break. 
        /// </summary>
        /// <value>A value that specifies where to make a page break. The default value is <see cref="T:Dt.Cells.Data.BreakType">None</see>.</value>
        [DefaultValue(0)]
        public BreakType PageBreak
        {
            get { return  this.pageBreak; }
            set { this.pageBreak = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets a value indicating whether [page break after].
        /// </summary>
        /// <value><c>true</c> if [page break after]; otherwise, <c>false</c></value>
        internal bool PageBreakAfter
        {
            get
            {
                if (this.pageBreak != BreakType.After)
                {
                    return (this.pageBreak == BreakType.BeforeAndAfter);
                }
                return true;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets a value indicating whether [page break before].
        /// </summary>
        /// <value><c>true</c> if [page break before]; otherwise, <c>false</c></value>
        internal bool PageBreakBefore
        {
            get
            {
                if (this.pageBreak != BreakType.Before)
                {
                    return (this.pageBreak == BreakType.BeforeAndAfter);
                }
                return true;
            }
        }
    }
}

