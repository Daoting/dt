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
using Dt.Pdf.Drawing;
using Dt.Pdf.Exceptions;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The Pdf Annotation base
    /// </summary>
    public abstract class PdfAnnotationBase : PdfDictionary, IVersionDepend
    {
        private double borderSize = 1.0;
        private string contents = string.Empty;
        private Flag flags;
        private DateTime modifiedDate;
        private PdfRectangle rectangle = new PdfRectangle(0f, 0f, 200f, 200f);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfAnnotationBase" /> class.
        /// </summary>
        /// <param name="subType">Type of the sub.</param>
        protected PdfAnnotationBase(PdfName subType)
        {
            base.isLabeled = true;
            base.Add(PdfName.Type, PdfName.Annot);
            base.Add(PdfName.Subtype, subType);
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns></returns>
        protected virtual PdfVersion GetVersion()
        {
            return PdfVersion.PDF1_0;
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.Rect] = this.rectangle;
            if (this.flags != Flag.NotSet)
            {
                base[PdfName.F] = new PdfNumber((double) this.flags);
            }
            this.WriteProperties(writer);
            if (this.borderSize != 1.0)
            {
                PdfArray array = new PdfArray {
                    new PdfNumber(0.0),
                    new PdfNumber(0.0),
                    new PdfNumber(this.borderSize)
                };
                base[PdfName.Border] = array;
            }
            if (!string.IsNullOrEmpty(this.contents))
            {
                base[PdfName.Contents] = new PdfString(this.contents);
            }
            if (this.modifiedDate.Ticks <= 0L)
            {
                this.modifiedDate = DateTime.Now;
            }
            base[PdfName.M] = new PdfDate(this.modifiedDate);
            base.ToPdf(writer);
        }

        /// <summary>
        /// Version of this instance.
        /// </summary>
        /// <returns></returns>
        public PdfVersion Version()
        {
            PdfVersion version = PdfVersion.PDF1_0;
            if (this.flags >= Flag.LockedContents)
            {
                version = PdfVersion.PDF1_7;
            }
            else if (this.flags >= Flag.ToggleNoView)
            {
                version = PdfVersion.PDF1_5;
            }
            else if (this.flags >= Flag.NoRotate)
            {
                version = PdfVersion.PDF1_3;
            }
            else if (this.flags >= Flag.Hidden)
            {
                version = PdfVersion.PDF1_2;
            }
            PdfVersion version2 = this.GetVersion();
            if (version2 > version)
            {
                version = version2;
            }
            if (base.ContainsKey(PdfName.AP) && (version < PdfVersion.PDF1_2))
            {
                version = PdfVersion.PDF1_2;
            }
            return version;
        }

        /// <summary>
        /// Writes the properties.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected abstract void WriteProperties(PdfWriter writer);

        /// <summary>
        /// Gets or sets the size of the border.
        /// </summary>
        /// <value>The size of the border.</value>
        public double BorderSize
        {
            get { return  this.borderSize; }
            set { this.borderSize = value; }
        }

        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        /// <value>The contents.</value>
        public string Contents
        {
            get { return  this.contents; }
            set { this.contents = value; }
        }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>The flags.</value>
        public Flag Flags
        {
            get { return  this.flags; }
            set { this.flags = value; }
        }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>The modified date.</value>
        public DateTime ModifiedDate
        {
            get { return  this.modifiedDate; }
            set { this.modifiedDate = value; }
        }

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public PdfRectangle Rectangle
        {
            get { return  this.rectangle; }
            set
            {
                if (value == null)
                {
                    throw new PdfArgumentNullException("Rectange");
                }
                this.rectangle = value;
            }
        }

        /// <summary>
        /// Gets the type of the sub.
        /// </summary>
        /// <value>The type of the sub.</value>
        public PdfName SubType
        {
            get { return  (base[PdfName.Subtype] as PdfName); }
        }

        /// <summary>
        /// The value of the annotation dictionary’s F entry is an unsigned 32-bit integer containing flags specifying various characteristics of the annotation.
        /// </summary>
        [Flags]
        public enum Flag
        {
            /// <summary>
            /// If set, do not display or print the annotation or allow it to interact 
            /// with the user, regardless of its annotation type or whether an 
            /// annotation handler is available. In cases where screen space is 
            /// limited, the ability to hide and show annotations selectively can 
            /// be used in combination with appearance streams to display auxiliary
            /// pop-up information similar in function to online help systems.
            /// </summary>
            Hidden = 2,
            /// <summary>
            /// If set, do not display the annotation if it does not belong to one of 
            /// the standard annotation types and no annotation handler 
            /// is available. If clear, display such an unknown annotation using 
            /// an appearance stream specified by its appearance dictionary, if any.
            /// </summary>
            Invisible = 1,
            /// <summary>
            /// If set, do not allow the annotation to be deleted or its properties 
            /// (including position and size) to be modified by the user. However, 
            /// this flag does not restrict changes to the annotation’s contents, such 
            /// as the value of a form field.
            /// </summary>
            Locked = 0x80,
            /// <summary>
            /// If set, do not allow the contents of the annotation to be modified by 
            /// the user. This flag does not restrict deletion of the annotation or 
            /// changes to other annotation properties, such as position and size.
            /// </summary>
            LockedContents = 0x200,
            /// <summary>
            /// If set, do not rotate the annotation’s appearance to match the rotation
            /// of the page. The upper-left corner of the annotation rectangle remains
            /// in a fixed location on the page, regardless of the page rotation.
            /// </summary>
            NoRotate = 0x10,
            /// <summary>
            /// Default value.
            /// </summary>
            NotSet = 0,
            /// <summary>
            /// If set, do not display the annotation on the screen or allow it to 
            /// interact with the user. The annotation may be printed (depending on 
            /// the setting of the Print flag) but should be considered hidden for 
            /// purposes of on-screen display and user interaction.
            /// </summary>
            NoView = 0x20,
            /// <summary>
            /// If set, do not scale the annotation’s appearance to match the 
            /// magnification of the page. The location of the annotation 
            /// on the page (defined by the upper-left corner of its annotation 
            /// rectangle) remains fixed, regardless of the page magnification. 
            /// </summary>
            NoZoom = 8,
            /// <summary>
            /// If set, print the annotation when the page is printed. If clear, 
            /// never print the annotation, regardless of whether it is displayed on 
            /// the screen. This can be useful, for example, for annotations 
            /// representing interactive pushbuttons, which would serve no meaningful 
            /// purpose on the printed page.
            /// </summary>
            Print = 4,
            /// <summary>
            /// If set, do not allow the annotation to interact with the user. The 
            /// annotation may be displayed or printed (depending on the settings of 
            /// the NoView and Print flags) but should not respond to mouse clicks or 
            /// change its appearance in response to mouse motions.
            /// </summary>
            ReadOnly = 0x40,
            /// <summary>
            /// If set, invert the interpretation of the NoView flag for certain events.
            /// A typical use is to have an annotation that appears only when a mouse cursor 
            /// is held over it.
            /// </summary>
            ToggleNoView = 0x100
        }
    }
}

