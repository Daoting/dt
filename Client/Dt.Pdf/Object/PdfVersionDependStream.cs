#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Object.Filter;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The version depended stream
    /// </summary>
    public class PdfVersionDependStream : PdfStream, IVersionDepend
    {
        protected PdfVersion version;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfVersionDependStream" /> class.
        /// </summary>
        public PdfVersionDependStream()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfVersionDependStream" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfVersionDependStream(PdfStream stream) : base(stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfVersionDependStream" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public PdfVersionDependStream(PdfFilter filter) : base(filter)
        {
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

