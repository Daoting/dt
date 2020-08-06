#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Defines a abstract class for report sections that can contain objects of the <see cref="T:Dt.Cells.Data.GcControl" /> class and its derived classes. 
    /// </summary>
    internal abstract class GcSection : IXmlSerializable
    {
        bool canGrow;
        bool canShrink;
        ControlCollection controls;
        int height;

        /// <summary>
        /// Creates a new section.
        /// </summary>
        internal GcSection()
        {
            this.Init();
        }

        /// <summary>
        /// Creates the state of the build in control.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal virtual object CreateBuildInControlState(GcReportContext context)
        {
            return null;
        }

        /// <summary>
        /// Gets all blocks.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal List<GcBlock> GetAllBlocks(GcReportContext context)
        {
            List<GcBlock> list = new List<GcBlock>();
            this.controls.Sort();
            foreach (GcPrintableControl control in this.controls.PrintableControls)
            {
                if (control.Visible)
                {
                    list.Add(control.GetBlock(context));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets all sizes.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal Windows.Foundation.Size GetAllSize(GcReportContext context)
        {
            List<GcBlock> allBlocks = this.GetAllBlocks(context);
            return this.GetAllSize((IEnumerable<GcBlock>) allBlocks);
        }

        /// <summary>
        /// Gets all sizes.
        /// </summary>
        /// <param name="cache">The cache</param>
        /// <returns></returns>
        internal Windows.Foundation.Size GetAllSize(GcSectionCache cache)
        {
            if (cache.AllSize.IsEmpty)
            {
                return this.GetAllSize((IEnumerable<GcBlock>) cache.AllBlocks);
            }
            return cache.AllSize;
        }

        /// <summary>
        /// Gets all sizes.
        /// </summary>
        /// <param name="list">The list</param>
        /// <returns></returns>
        Windows.Foundation.Size GetAllSize(IEnumerable<GcBlock> list)
        {
            Windows.Foundation.Size size = new Windows.Foundation.Size(0.0, 0.0);
            foreach (GcBlock block in list)
            {
                if (block.Right > size.Width)
                {
                    size.Width = block.Right;
                }
                if (block.Bottom > size.Height)
                {
                    size.Height = block.Bottom;
                }
            }
            Windows.Foundation.Rect buildInControlSize = this.GetBuildInControlSize();
            if (buildInControlSize.Right > size.Width)
            {
                size.Width = buildInControlSize.Right;
            }
            if (buildInControlSize.Bottom > size.Height)
            {
                size.Height = buildInControlSize.Bottom;
            }
            if (((size.Height > this.Height) && !this.CanGrow) || ((size.Height < this.Height) && !this.CanShrink))
            {
                size.Height = this.Height;
            }
            return size;
        }

        /// <summary>
        /// Gets the build in control range.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="buildInControlState">State of the build in control</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <param name="continuePage">If set to <c>true</c>, [continue page]</param>
        /// <returns></returns>
        internal virtual List<GcRangeBlock> GetBuildInControlRange(GcReportContext context, int x, int y, int width, int high, object buildInControlState, bool horizontal, bool continuePage)
        {
            return null;
        }

        /// <summary>
        /// Gets the size of the built-in control.
        /// </summary>
        /// <returns></returns>
        protected virtual Windows.Foundation.Rect GetBuildInControlSize()
        {
            return Windows.Foundation.Rect.Empty;
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal GcSectionCache GetCache(GcReportContext context)
        {
            List<GcBlock> allBlocks = this.GetAllBlocks(context);
            return new GcSectionCache(this.GetAllSize((IEnumerable<GcBlock>) allBlocks), allBlocks);
        }

        /// <summary>
        /// Gets the range.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="cache">The cache</param>
        /// <param name="buildInControlState">State of the build in control</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <param name="continuePage">If set to <c>true</c>, [continue page]</param>
        /// <returns></returns>
        internal GcRangeBlock GetRange(GcReportContext context, int x, int y, int width, int high, GcSectionCache cache, object buildInControlState, bool horizontal, bool continuePage)
        {
            return this.GetRange(context, x, y, width, high, cache.AllBlocks, cache.AllSize, buildInControlState, horizontal, continuePage);
        }

        /// <summary>
        /// Gets the range.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        /// <param name="width">The width</param>
        /// <param name="high">The height</param>
        /// <param name="blocks">The blocks</param>
        /// <param name="allSize">All sizes</param>
        /// <param name="buildInControlState">State of the build in control</param>
        /// <param name="horizontal">If set to <c>true</c>, [horizontal]</param>
        /// <param name="continuePage">If set to <c>true</c>, [continue page]</param>
        /// <returns></returns>
        internal virtual GcRangeBlock GetRange(GcReportContext context, int x, int y, int width, int high, List<GcBlock> blocks, Windows.Foundation.Size allSize, object buildInControlState, bool horizontal, bool continuePage)
        {
            List<GcRangeBlock> list = this.GetBuildInControlRange(context, x, y, width, high, buildInControlState, horizontal, continuePage);
            Windows.Foundation.Size size = allSize;
            Windows.Foundation.Rect rect = new Windows.Foundation.Rect(0.0, 0.0, size.Width, size.Height);
            if ((list != null) && (list.Count > 0))
            {
                foreach (GcRangeBlock block in list)
                {
                    rect.Union(new Windows.Foundation.Rect(block.X, block.Y, block.Width, block.Height));
                }
            }
            Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect((double) x, (double) y, (double) width, (double) high);
            if (!IsIntersect(rect, rect2))
            {
                return new GcRangeBlock(0.0, 0.0, 0.0, 0.0);
            }
            rect.Intersect(rect2);
            GcRangeBlock block2 = new GcRangeBlock(0.0, 0.0, (double) ((int) Math.Ceiling(rect.Width)), (double) ((int) Math.Ceiling(rect.Height))) {
                OffsetX = (int) rect.X,
                OffsetY = (int) rect.Y
            };
            if ((list != null) && (list.Count > 0))
            {
                foreach (GcRangeBlock block3 in list)
                {
                    block2.Blocks.Add(block3);
                }
            }
            foreach (GcBlock block4 in blocks)
            {
                if (block4.IntersectWith((double) ((int) rect.X), (double) ((int) rect.Y), (double) ((int) rect.Width), (double) ((int) rect.Height)))
                {
                    block2.Blocks.Add(block4.Clone());
                }
            }
            return block2;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Init()
        {
            this.height = 200;
            this.canShrink = true;
            this.canGrow = true;
            this.controls = new ControlCollection();
        }

        /// <summary>
        /// Determines whether the specified rect1 is intersect.
        /// </summary>
        /// <param name="rect1">The rect1.</param>
        /// <param name="rect2">The rect2.</param>
        /// <returns>
        /// <c>true</c> if the specified rect1 is intersect; otherwise, <c>false</c>.
        /// </returns>
        static bool IsIntersect(Windows.Foundation.Rect rect1, Windows.Foundation.Rect rect2)
        {
            return ((((rect2.X < (rect1.X + rect1.Width)) && (rect1.X < (rect2.X + rect2.Width))) && (rect2.Y < (rect1.Y + rect1.Height))) && (rect1.Y < (rect2.Y + rect2.Height)));
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadXmlBase(XmlReader reader)
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
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            this.Init();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    this.ReadXmlBase(reader);
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.WriteXmlBase(writer);
        }

        /// <summary>
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void WriteXmlBase(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the height can increase to display the content.
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
        /// Gets or sets a value that indicates whether the height can decrease if its contents do not completely fill the section. 
        /// </summary>
        /// <value>
        /// <c>true</c> if the height can decrease in order to remove unused space; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public virtual bool CanShrink
        {
            get { return  this.canShrink; }
            set { this.canShrink = value; }
        }

        /// <summary>
        /// Gets the collection of controls contained in a section.
        /// </summary>
        /// <value>An object of the <see cref="T:Dt.Cells.Data.ControlCollection" /> class that represents the collection of contained controls.</value>
        public virtual ControlCollection Controls
        {
            get { return  this.controls; }
        }

        /// <summary>
        /// Gets or sets the section's height, in hundredths of an inch.
        /// </summary>
        /// <value>The height. The default value is 200, which is 2 inches.</value>
        [DefaultValue(200)]
        public virtual int Height
        {
            get { return  this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Internal only.
        /// GcSectionCache
        /// </summary>
        internal class GcSectionCache
        {
            List<GcBlock> allBlocks;
            Windows.Foundation.Size allSize;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSection.GcSectionCache" /> class.
            /// </summary>
            public GcSectionCache()
            {
                this.allSize = Windows.Foundation.Size.Empty;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.GcSection.GcSectionCache" /> class.
            /// </summary>
            /// <param name="allSize">All sizes.</param>
            /// <param name="allBlocks">All blocks.</param>
            public GcSectionCache(Windows.Foundation.Size allSize, List<GcBlock> allBlocks)
            {
                this.allSize = Windows.Foundation.Size.Empty;
                this.allSize = allSize;
                this.allBlocks = allBlocks;
            }

            /// <summary>
            /// Gets or sets all blocks.
            /// </summary>
            /// <value>All blocks.</value>
            public List<GcBlock> AllBlocks
            {
                get { return  this.allBlocks; }
                set { this.allBlocks = value; }
            }

            /// <summary>
            /// Gets or sets all sizes.
            /// </summary>
            /// <value>All sizes.</value>
            public Windows.Foundation.Size AllSize
            {
                get { return  this.allSize; }
                set { this.allSize = value; }
            }
        }
    }
}

