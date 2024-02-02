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
    /// A dictionary containing entries that define 
    /// the character collection of the CIDFont.
    /// </summary>
    public class CIDSystemInfo : PdfDictionary, IVersionDepend
    {
        public static CIDSystemInfo CNS1 = new CIDSystemInfo("CNS1", 3);
        public static CIDSystemInfo GB1 = new CIDSystemInfo("GB1", 4);
        public static CIDSystemInfo Identity = new CIDSystemInfo("Indentity", 0);
        public static CIDSystemInfo Japan1 = new CIDSystemInfo("Japan1", 4);
        public static CIDSystemInfo Korea1 = new CIDSystemInfo("Korea1", 1);
        private readonly string ordering;
        private readonly string registry;
        private readonly int supplement;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CIDSystemInfo" /> class.
        /// </summary>
        /// <param name="ordering">The ordering.</param>
        /// <param name="supplement">The supplement.</param>
        public CIDSystemInfo(string ordering, int supplement)
        {
            this.registry = "Adobe";
            this.ordering = ordering;
            this.supplement = supplement;
            base.isLabeled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CIDSystemInfo" /> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="ordering">The ordering.</param>
        /// <param name="supplement">The supplement.</param>
        public CIDSystemInfo(string registry, string ordering, int supplement) : this(ordering, supplement)
        {
            this.registry = registry;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Registry] = new PdfString(this.registry);
            base[PdfName.Ordering] = new PdfString(this.ordering);
            base[PdfName.Supplement] = new PdfNumber((double) this.supplement);
            base.ToPdf(writer);
        }

        /// <summary>
        /// Versions this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            return PdfVersion.PDF1_4;
        }
    }
}

