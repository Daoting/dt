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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents report controls that are printable.
    /// </summary>
    internal abstract class GcPrintableControl : GcControl
    {
        protected Dt.Cells.Data.Bookmark bookmark;
        protected IBorder border;
        protected bool canGrow;
        protected bool canShrink;
        protected Dt.Cells.Data.Font font;
        protected int height;
        protected Uri navigationUri;
        protected PaddingInfo padding;
        protected double rotationAngle;
        protected bool visible;
        protected int width;
        protected int zIndex;

        /// <summary>
        /// Creates a new GcPrintable control.
        /// </summary>
        protected GcPrintableControl()
        {
            this.padding = PaddingInfo.Empty;
            this.Init();
        }

        /// <summary>
        /// Creates a new GcPrintable control, with the specified <i>x</i> and <i>y</i> values.
        /// </summary>
        /// <param name="x">The <i>x</i> value, in hundredths of an inch.</param>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        protected GcPrintableControl(int x, int y) : base(x, y)
        {
            this.padding = PaddingInfo.Empty;
            this.Init();
        }

        /// <summary>
        /// Creates a new GcPrintable control, with the specified <i>x</i> and <i>y</i> values, and width and height.
        /// </summary>
        /// <param name="x">The <i>x</i> value, in hundredths of an inch.</param>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        /// <param name="width">The width, in hundredths of an inch.</param>
        /// <param name="height">The height, in hundredths of an inch.</param>
        protected GcPrintableControl(int x, int y, int width, int height)
        {
            this.padding = PaddingInfo.Empty;
            this.Init();
            base.x = x;
            base.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Creates a new GcPrintable control, with the specified <i>x</i> and <i>y</i> values, width and height, and size flags.
        /// </summary>
        /// <param name="x">The <i>x</i> value, in hundredths of an inch.</param>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        /// <param name="width">The width, in hundredths of an inch.</param>
        /// <param name="height">The height, in hundredths of an inch.</param>
        /// <param name="canShrink">Whether the size of the control can decrease if contents do not fill the control.</param>
        /// <param name="canGrow">Whether the size of the control can increase if contents are larger than the control.</param>
        protected GcPrintableControl(int x, int y, int width, int height, bool canShrink, bool canGrow)
        {
            this.padding = PaddingInfo.Empty;
            this.Init();
            base.x = x;
            base.y = y;
            this.width = width;
            this.height = height;
            this.canShrink = canShrink;
            this.canGrow = canGrow;
        }

        /// <summary>
        /// Gets the actual size.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal virtual Windows.Foundation.Size GetActualSize(GcReportContext context)
        {
            int height = this.Height;
            if (this.CanGrow || this.CanShrink)
            {
                height = (int) Math.Ceiling(this.GetPreferredSize(context).Height);
                if (((height > this.Height) && !this.CanGrow) || ((height < this.Height) && !this.CanShrink))
                {
                    height = this.Height;
                }
            }
            return new Windows.Foundation.Size((double) this.Width, (double) height);
        }

        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        internal virtual GcBlock GetBlock(GcReportContext context)
        {
            Windows.Foundation.Size actualSize = this.GetActualSize(context);
            if (this.RotationAngle != 0.0)
            {
                Windows.Foundation.Rect rect = Shape.GetRotatedBounds(0.0, 0.0, actualSize.Width, actualSize.Height, this.RotationAngle, actualSize.Width / 2.0, actualSize.Height / 2.0);
                actualSize = new Windows.Foundation.Size(rect.Width, rect.Height);
            }
            return new GcBlock((double) this.X, (double) this.Y, actualSize.Width, actualSize.Height, this);
        }

        /// <summary>
        /// Gets the boundary, in hundredths of an inch.
        /// </summary>
        /// <returns>A <see cref="T:Windows.Foundation.Size" /> object.</returns>
        public virtual Windows.Foundation.Size GetBounds()
        {
            Windows.Foundation.Rect rect = Shape.GetRotatedBounds(0.0, 0.0, (double) this.Width, (double) this.Height, this.RotationAngle, ((double) this.Width) / 2.0, ((double) this.Height) / 2.0);
            return new Windows.Foundation.Size(rect.Width, rect.Height);
        }

        /// <summary>
        /// Gets the preferred size.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal virtual Windows.Foundation.Size GetPreferredSize(GcReportContext context)
        {
            return this.GetBounds();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.border = null;
            this.font = null;
            this.canGrow = true;
            this.canShrink = false;
            this.height = 200;
            this.width = 300;
            this.rotationAngle = 0.0;
            this.bookmark = new Dt.Cells.Data.Bookmark();
            this.navigationUri = null;
            this.visible = true;
            this.zIndex = 0;
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
        /// Gets the bookmark of the control.
        /// </summary>
        /// <value>The <see cref="P:Dt.Cells.Data.GcPrintableControl.Bookmark" /> object.</value>
        public Dt.Cells.Data.Bookmark Bookmark
        {
            get { return  this.bookmark; }
        }

        /// <summary>
        /// Gets or sets the border for a control.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Cells.Data.IBorder" /> object that specifies the border for the control.
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        public virtual IBorder Border
        {
            get { return  this.border; }
            set { this.border = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the height can increase to display the entire content.
        /// </summary>
        /// <value>
        /// <c>true</c> if the height can increase to display all the contents; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public virtual bool CanGrow
        {
            get { return  this.canGrow; }
            set { this.canGrow = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the height can decrease if its contents do not completely fill the control. 
        /// </summary>
        /// <value>
        /// <c>true</c> if the height can decrease to remove unused space; otherwise, <c>false</c>. 
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public virtual bool CanShrink
        {
            get { return  this.canShrink; }
            set { this.canShrink = value; }
        }

        /// <summary>
        /// Gets or sets the font of the control.
        /// </summary>
        /// <value>The <see cref="P:Dt.Cells.Data.GcPrintableControl.Font" /> object. The default value is null.</value>
        [DefaultValue((string) null)]
        public virtual Dt.Cells.Data.Font Font
        {
            get { return  this.font; }
            set { this.font = value; }
        }

        /// <summary>
        /// Gets or sets the height, in hundredths of an inch.
        /// </summary>
        /// <value>The height. The default value is 200, which is 2 inches.</value>
        [DefaultValue(200)]
        public virtual int Height
        {
            get { return  this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Gets or sets the navigation uniform resource identifier (URI) for the control.
        /// </summary>
        /// <value>The navigation <see cref="T:System.Uri" /> object. The default value is null.</value>
        /// <remarks>Use this property to add a hyperlink for the control.</remarks>
        [DefaultValue((string) null)]
        public Uri NavigationUri
        {
            get { return  this.navigationUri; }
            set { this.navigationUri = value; }
        }

        /// <summary>
        /// Gets or sets the padding values, in hundredths of an inch.
        /// </summary>
        /// <value>The <see cref="T:Dt.Cells.Data.PaddingInfo" /> object.</value>
        public PaddingInfo Padding
        {
            get { return  this.padding; }
            set { this.padding = value; }
        }

        /// <summary>
        /// Gets or sets the rotation angle of the control.
        /// </summary>
        /// <value>The rotation angle of the control in degrees. The default value is 0.0 degrees.</value>
        [DefaultValue((double) 0.0)]
        public virtual double RotationAngle
        {
            get { return  this.rotationAngle; }
            set { this.rotationAngle = value; }
        }

        /// <summary>
        /// Gets or sets whether this control is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if the control is visible; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool Visible
        {
            get { return  this.visible; }
            set { this.visible = value; }
        }

        /// <summary>
        /// Gets or sets the width, in hundredths of an inch.
        /// </summary>
        /// <value>The width. The default value is 300, which is 3 inches.</value>
        [DefaultValue(300)]
        public virtual int Width
        {
            get { return  this.width; }
            set { this.width = value; }
        }

        /// <summary>
        /// Gets or sets the <i>z</i>-index of control.
        /// </summary>
        /// <value>The <i>z</i>-index. The default value is 0.</value>
        /// <remarks>
        /// The <i>z</i>-index is the location of the control in the window in relation to other controls along the <i>z</i>-axis. 
        /// That is, whether it is on top of or behind other objects.
        /// </remarks>
        [DefaultValue(0)]
        public int ZIndex
        {
            get { return  this.zIndex; }
            set { this.zIndex = value; }
        }
    }
}

