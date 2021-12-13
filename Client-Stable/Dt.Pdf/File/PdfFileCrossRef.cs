#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Pdf.File
{
    /// <summary>
    /// Pdf file cross reference
    /// </summary>
    public class PdfFileCrossRef : IComparable<PdfFileCrossRef>
    {
        private bool inUse;
        private PdfObjectBase objectBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.File.PdfFileCrossRef" /> class.
        /// </summary>
        public PdfFileCrossRef()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.File.PdfFileCrossRef" /> class.
        /// </summary>
        /// <param name="objectBase">The object base.</param>
        public PdfFileCrossRef(PdfObjectBase objectBase)
        {
            this.objectBase = objectBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.File.PdfFileCrossRef" /> class.
        /// </summary>
        /// <param name="objectBase">The object base.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="inUse">if set to <c>true</c> [in use].</param>
        public PdfFileCrossRef(PdfObjectBase objectBase, long offset, bool inUse)
        {
            this.objectBase = objectBase;
            this.Offset = offset;
            this.inUse = inUse;
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other" /> parameter.
        /// Zero
        /// This object is equal to <paramref name="other" />.
        /// Greater than zero
        /// This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(PdfFileCrossRef other)
        {
            if (other == null)
            {
                throw new PdfArgumentNullException("other");
            }
            return (this.ObjectIndex - other.ObjectIndex);
        }

        /// <summary>
        /// Gets the generation.
        /// </summary>
        /// <value>The generation.</value>
        public int Generation
        {
            get
            {
                if (this.objectBase != null)
                {
                    return this.objectBase.Generation;
                }
                return 0xffff;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [in use].
        /// </summary>
        /// <value><c>true</c> if [in use]; otherwise, <c>false</c>.</value>
        public bool InUse
        {
            get { return  ((this.ObjectIndex != 0) && this.inUse); }
            set { this.inUse = value; }
        }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>The object.</value>
        public PdfObjectBase Object
        {
            get { return  this.objectBase; }
            set { this.objectBase = value; }
        }

        /// <summary>
        /// Gets the index of the object.
        /// </summary>
        /// <value>The index of the object.</value>
        public int ObjectIndex
        {
            get
            {
                if (this.objectBase != null)
                {
                    return this.objectBase.ObjectIndex;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public long Offset { get; set; }
    }
}

