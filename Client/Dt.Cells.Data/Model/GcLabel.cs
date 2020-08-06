#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Xml;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a label control.
    /// </summary>
    internal class GcLabel : GcPrintableControl
    {
        ContentAlignment alignment;
        Brush background;
        Brush foreground;
        string text;

        /// <summary>
        /// Creates a new label.
        /// </summary>
        public GcLabel()
        {
            this.text = string.Empty;
            this.Init();
        }

        /// <summary>
        /// Creates a new label with the specified text.
        /// </summary>
        /// <param name="text">The text of the label.</param>
        public GcLabel(string text) : this()
        {
            this.text = text;
        }

        /// <summary>
        /// Creates a new label with the specified <i>x</i> and <i>y</i> values, and width and height.
        /// </summary>
        /// <param name="x">The <i>x</i> value, in hundredths of an inch.</param>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        /// <param name="width">The width, in hundredths of an inch.</param>
        /// <param name="height">The height, in hundredths of an inch.</param>
        public GcLabel(int x, int y, int width, int height) : base(x, y, width, height)
        {
            this.text = string.Empty;
        }

        /// <summary>
        /// Creates a new label with the specified <i>x</i> and <i>y</i> values, width and height, and size flags.
        /// </summary>
        /// <param name="x">The <i>x</i> value, in hundredths of an inch.</param>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        /// <param name="width">The width, in hundredths of an inch.</param>
        /// <param name="height">The height, in hundredths of an inch.</param>
        /// <param name="canShrink">Whether the label decreases in size if the text does not fill the label.</param>
        /// <param name="canGrow">Whether the label increases in size if the text is larger than the label.</param>
        public GcLabel(int x, int y, int width, int height, bool canShrink, bool canGrow) : base(x, y, width, height, canShrink, canGrow)
        {
            this.text = string.Empty;
        }

        /// <summary>
        /// Creates a new label with the specified <i>x</i> and <i>y</i> values, width and height, size flags, and text.
        /// </summary>
        /// <param name="x">The <i>x</i> value, in hundredths of an inch.</param>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        /// <param name="width">The width, in hundredths of an inch.</param>
        /// <param name="height">The height, in hundredths of an inch.</param>
        /// <param name="canShrink">Whether the label decreases in size if the text does not fill the label.</param>
        /// <param name="canGrow">Whether the label increases in size if the text is larger than the label.</param>
        /// <param name="text">The text for the label.</param>
        public GcLabel(int x, int y, int width, int height, bool canShrink, bool canGrow, string text) : this(x, y, width, height, canShrink, canGrow)
        {
            this.text = text;
        }

        /// <summary>
        /// Changes the size automatically.
        /// </summary>
        /// <param name="context">The context.</param>
        internal void AutoSize(GcReportContext context)
        {
            Windows.Foundation.Size preferredSize = this.GetPreferredSize(context);
            this.Width = (int) Math.Ceiling(preferredSize.Width);
            this.Height = (int) Math.Ceiling(preferredSize.Height);
        }

        /// <summary>
        /// Gets the preferred size.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal override Windows.Foundation.Size GetPreferredSize(GcReportContext context)
        {
            int num = base.width - this.alignment.TextIndent;
            num = Math.Max(0, num);
            if (this.padding.Horizontal > 0)
            {
                num -= this.padding.Horizontal;
            }
            if (num < 0)
            {
                num = 0;
            }
            Windows.Foundation.Size size = Utilities.MeasureStringByAlignment(context, this.Text, base.font, this.alignment, num);
            if (!size.IsEmpty && (this.padding.Vertical > 0))
            {
                size.Height += this.padding.Vertical;
            }
            return size;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.text = string.Empty;
            this.background = null;
            this.foreground = FillEffects.Black;
            this.alignment = new ContentAlignment();
            this.alignment.WordWrap = true;
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader</param>
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
        /// Gets the label alignment.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.ContentAlignment" /> object that specifies the alignment for the label.</value>
        public ContentAlignment Alignment
        {
            get { return  this.alignment; }
        }

        /// <summary>
        /// Gets or sets the background color of the label.
        /// </summary>
        /// <value>
        /// A <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that specifies the background color for the label.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        public Brush Background
        {
            get { return  this.background; }
            set { this.background = value; }
        }

        /// <summary>
        /// Gets or sets the foreground color for the label.
        /// </summary>
        /// <value>
        /// A <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that specifies the foreground color for the label.
        /// The default value is black.
        /// </value>
        public Brush Foreground
        {
            get { return  this.foreground; }
            set
            {
                if (value == null)
                {
                    this.foreground = FillEffects.Black;
                }
                this.foreground = value;
            }
        }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>The text. The default value is an empty string.</value>
        [DefaultValue("")]
        public virtual string Text
        {
            get { return  this.text; }
            set { this.text = value; }
        }
    }
}

