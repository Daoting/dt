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
    public class PdfExtGraphicState : PdfDictionary, IVersionDepend
    {
        private float fillAlpha = -1f;
        private float strokAlpha = -1f;

        public bool Equals(PdfExtGraphicState other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }
            return (object.ReferenceEquals(this, other) || ((other.strokAlpha == this.strokAlpha) && (other.fillAlpha == this.fillAlpha)));
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(PdfExtGraphicState))
            {
                return false;
            }
            return this.Equals((PdfExtGraphicState) obj);
        }

        public override int GetHashCode()
        {
            return ((((float) this.strokAlpha).GetHashCode() * 0x18d) ^ ((float) this.fillAlpha).GetHashCode());
        }

        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Type] = PdfName.ExtGState;
            if (this.strokAlpha != -1f)
            {
                base[PdfName.CA] = new PdfNumber((double) this.strokAlpha);
            }
            if (this.fillAlpha != -1f)
            {
                base[PdfName.ca] = new PdfNumber((double) this.fillAlpha);
            }
            base.ToPdf(writer);
        }

        public PdfVersion Version()
        {
            return PdfVersion.PDF1_2;
        }

        public float FillAlpha
        {
            get
            {
                if (this.fillAlpha != -1f)
                {
                    return this.fillAlpha;
                }
                return 1f;
            }
            set
            {
                if ((value < 0f) || (value > 1f))
                {
                    throw new PdfArgumentOutOfRangeException("Between 0 and 1.");
                }
                this.fillAlpha = value;
            }
        }

        public float StrokAlpha
        {
            get
            {
                if (this.strokAlpha != -1f)
                {
                    return this.strokAlpha;
                }
                return 1f;
            }
            set
            {
                if ((value < 0f) || (value > 1f))
                {
                    throw new PdfArgumentOutOfRangeException("Between 0 and 1.");
                }
                this.strokAlpha = value;
            }
        }
    }
}

