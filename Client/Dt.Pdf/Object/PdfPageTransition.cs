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
    /// PdfPageTransition class
    /// </summary>
    public class PdfPageTransition : PdfDictionary, IVersionDepend
    {
        /// <summary>
        /// transitionType
        /// </summary>
        private PageTransitionType transitionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageTransition" /> class.
        /// </summary>
        public PdfPageTransition()
        {
            base.Add(PdfName.Type, PdfName.Trans);
            base.isLabeled = true;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            switch (this.transitionType)
            {
                case PageTransitionType.Default:
                    base[PdfName.S] = PdfName.R;
                    break;

                case PageTransitionType.Split:
                    base[PdfName.S] = PdfName.Split;
                    break;

                case PageTransitionType.Blinds:
                    base[PdfName.S] = PdfName.Blinds;
                    break;

                case PageTransitionType.Box:
                    base[PdfName.S] = PdfName.Box;
                    break;

                case PageTransitionType.Wipe:
                    base[PdfName.S] = PdfName.Wipe;
                    break;

                case PageTransitionType.Dissolve:
                    base[PdfName.S] = PdfName.Dissolve;
                    break;

                case PageTransitionType.Glitter:
                    base[PdfName.S] = PdfName.Glitter;
                    break;

                case PageTransitionType.Fly:
                    base[PdfName.S] = PdfName.Fly;
                    break;

                case PageTransitionType.Push:
                    base[PdfName.S] = PdfName.Push;
                    break;

                case PageTransitionType.Cover:
                    base[PdfName.S] = PdfName.Cover;
                    break;

                case PageTransitionType.Uncover:
                    base[PdfName.S] = PdfName.Uncover;
                    break;

                case PageTransitionType.Fade:
                    base[PdfName.S] = PdfName.Fade;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            switch (this.transitionType)
            {
                case PageTransitionType.Default:
                case PageTransitionType.Split:
                case PageTransitionType.Blinds:
                case PageTransitionType.Box:
                case PageTransitionType.Wipe:
                case PageTransitionType.Dissolve:
                case PageTransitionType.Glitter:
                    return PdfVersion.PDF1_1;

                case PageTransitionType.Fly:
                case PageTransitionType.Push:
                case PageTransitionType.Cover:
                case PageTransitionType.Uncover:
                case PageTransitionType.Fade:
                    return PdfVersion.PDF1_5;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Gets or sets the type of the transition.
        /// </summary>
        /// <value>The type of the transition.</value>
        public PageTransitionType TransitionType
        {
            get { return  this.transitionType; }
            set { this.transitionType = value; }
        }
    }
}

