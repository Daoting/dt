#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.Exceptions;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Object base of all Pdf objects.
    /// </summary>
    public abstract class PdfObjectBase
    {
        private bool isFix;
        protected bool isLabeled;
        private int objectIndex = -1;

        protected PdfObjectBase()
        {
        }

        /// <summary>
        /// Get bytes of object
        /// </summary>
        public abstract byte[] GetBytes();
        /// <summary>
        /// Sets the label and fix.
        /// </summary>
        /// <param name="labeled">if set to <c>true</c> [labeled].</param>
        /// <param name="fix">if set to <c>true</c> [fix].</param>
        internal void SetLabelAndFix(bool labeled, bool fix)
        {
            this.isLabeled = labeled;
            this.isFix = fix;
        }

        /// <summary>
        /// Read data from Pdf reader
        /// </summary>
        /// <param name="reader">Pdf Reader</param>
        public abstract void ToObject(IPdfReader reader);
        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public abstract void ToPdf(PdfWriter writer);

        /// <summary>
        /// Gets or sets the generation.
        /// </summary>
        /// <value>The generation.</value>
        internal int Generation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance able to change Label or not.
        /// </summary>
        /// <value><c>true</c> if this instance is fix; otherwise, <c>false</c>.</value>
        public bool IsFix
        {
            get { return  this.isFix; }
            set { this.isFix = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is labeled.
        /// About Label reference Pdf Ref v1.7-3.2.9
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is labeled; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsLabeled
        {
            get { return  this.isLabeled; }
            set
            {
                if (this.isFix)
                {
                    throw new PdfObjectInternalException(PdfObjectInternalException.PdfObjectExceptionType.ChangeFixedObjectLabel);
                }
                this.isLabeled = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the object.
        /// </summary>
        /// <value>The index of the object.</value>
        internal int ObjectIndex
        {
            get { return  this.objectIndex; }
            set { this.objectIndex = value; }
        }
    }
}

