#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The version depended dictionary
    /// </summary>
    public class PdfVersionDependDictionary : PdfDictionary, IVersionDepend
    {
        protected PdfVersion version;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfVersionDependDictionary" /> class.
        /// </summary>
        public PdfVersionDependDictionary()
        {
            base.isLabeled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfVersionDependDictionary" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfVersionDependDictionary(PdfDictionary value) : base(value)
        {
            base.isLabeled = true;
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            return this.version;
        }

        /// <summary>
        /// Gets or sets the PDF version.
        /// </summary>
        /// <value>The PDF version.</value>
        public PdfVersion PdfVersion
        {
            get { return  this.version; }
            internal set { this.version = value; }
        }
    }
}

