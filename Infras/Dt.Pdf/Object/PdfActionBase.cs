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
    /// The Pdf action base
    /// </summary>
    public abstract class PdfActionBase : PdfDictionary, IVersionDepend
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfActionBase" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        protected PdfActionBase(PdfName type)
        {
            base.isLabeled = true;
            base[PdfName.Type] = PdfName.Action;
            base[PdfName.S] = type;
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public virtual PdfVersion Version()
        {
            return PdfVersion.PDF1_1;
        }
    }
}

