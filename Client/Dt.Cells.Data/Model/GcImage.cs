#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Drawing;
using System;
using System.ComponentModel;
using System.Xml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an image control.
    /// </summary>
    internal class GcImage : GcPrintableControl
    {
        Brush background;
        Dt.Pdf.Drawing.Image image;

        /// <summary>
        /// Creates a new image of the control.
        /// </summary>
        public GcImage()
        {
        }

        /// <summary>
        /// Creates a new image of the control using image data.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        public GcImage(byte[] imageData)
        {
            if ((imageData == null) || (imageData.Length < 4))
            {
                throw new ArgumentNullException("imageData");
            }
            this.image = Dt.Pdf.Drawing.Image.GetInstance(imageData);
            base.width = (int) this.image.Width;
            base.height = (int) this.image.Height;
        }

        /// <summary>
        /// Creates a new image of the control using an image object.
        /// </summary>
        /// <param name="image">The image object.</param>
        public GcImage(Dt.Pdf.Drawing.Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            this.image = image;
            base.width = (int) image.Width;
            base.height = (int) image.Height;
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
        /// Gets or sets the background for the control.
        /// </summary>
        /// <value>
        /// A <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that specifies the background for the control.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        public Brush Background
        {
            get { return  this.background; }
            set { this.background = value; }
        }

        /// <summary>
        /// Overrides the CanGrow property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property is read-only.
        /// </remarks>
        public override bool CanGrow
        {
            get { return  false; }
            set
            {
            }
        }

        /// <summary>
        /// Overrides the CanShrink property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property is read-only.
        /// </remarks>
        public override bool CanShrink
        {
            get { return  false; }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the image of this control.
        /// </summary>
        /// <value>The image object.</value>
        [DefaultValue((string) null)]
        public Dt.Pdf.Drawing.Image Image
        {
            get { return  this.image; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                this.image = value;
            }
        }
    }
}

