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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a Page Info control.
    /// </summary>
    internal class GcPageInfo : GcLabel
    {
        int currentHPage = 1;
        int currentPage = 1;
        int currentVPage = 1;
        System.DateTime dateTime = System.DateTime.Now;
        string format = string.Empty;
        int pageCount = 1;
        PageInfoType type = PageInfoType.CurrentOfTotal;

        /// <summary>
        /// Creates a new page information object.
        /// </summary>
        public GcPageInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.type = PageInfoType.CurrentOfTotal;
            this.format = string.Empty;
            this.currentPage = 1;
            this.currentHPage = 1;
            this.currentVPage = 1;
            this.pageCount = 1;
            this.dateTime = System.DateTime.Now;
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
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "Type":
                        Serializer.ReadAttributeEnum<PageInfoType>("value", PageInfoType.CurrentOfTotal, reader);
                        return;

                    case "FormatString":
                        Serializer.ReadAttribute("value", reader);
                        return;
                }
                base.ReadXmlBase(reader);
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
        /// Internal only.
        /// Gets or sets the current H page.
        /// </summary>
        /// <value>The current H page</value>
        internal int CurrentHPage
        {
            get { return  this.currentHPage; }
            set { this.currentHPage = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page</value>
        internal int CurrentPage
        {
            get { return  this.currentPage; }
            set { this.currentPage = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the current V page.
        /// </summary>
        /// <value>The current V page</value>
        internal int CurrentVPage
        {
            get { return  this.currentVPage; }
            set { this.currentVPage = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time</value>
        internal System.DateTime DateTime
        {
            get { return  this.dateTime; }
            set { this.dateTime = value; }
        }

        /// <summary>
        /// Gets or sets the format string for the page information.
        /// </summary>
        /// <remarks>
        /// The required format string is dependent on the type of page information, as set by the <see cref="P:Dt.Cells.Data.GcPageInfo.Type" /> property.
        /// The following table explains the type settings and parameters.
        /// <list type="table">
        /// <listheader><term>Type</term><description>Parameters and Example</description></listheader>
        /// <item><term>PageInfoType.None</term>
        /// <description>No parameters.</description></item>
        /// <item><term>PageInfoType.CurrentOfTotal</term>
        /// <description>The current page number and the page count. 
        /// Example: "1 page / 12 pages"</description></item>
        /// <item><term>PageInfoType.TwoDimensionPageNumber</term>
        /// <description>The current page number, the horizontal page number, and the vertical page number. 
        /// Example: 101[12-27]</description></item>
        /// <item><term>All other types</term>
        /// <description>One parameter (varies by type)</description></item>
        /// </list>
        /// For more information, see <see cref="T:Dt.Cells.Data.PageInfoType" />.
        /// </remarks>
        /// <value>The format. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Format
        {
            get { return  this.format; }
            set { this.format = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the page count.
        /// </summary>
        /// <value>The page count</value>
        internal int PageCount
        {
            get { return  this.pageCount; }
            set { this.pageCount = value; }
        }

        /// <summary>
        /// Gets the text for the page information.
        /// </summary>
        /// <value>The text.</value>
        /// <remarks>This property is read-only.</remarks>
        public override string Text
        {
            get
            {
                string format = this.format;
                if (string.IsNullOrEmpty(format))
                {
                    switch (this.type)
                    {
                        case PageInfoType.None:
                            format = "";
                            goto Label_006C;

                        case PageInfoType.PageNumber:
                        case PageInfoType.PageCount:
                            format = "{0}";
                            goto Label_006C;

                        case PageInfoType.TwoDimensionPageNumber:
                            format = "{0}[{1}-{2}]";
                            goto Label_006C;

                        case PageInfoType.CurrentOfTotal:
                            format = "{0}/{1}";
                            goto Label_006C;

                        case PageInfoType.DateTime:
                        case PageInfoType.Date:
                        case PageInfoType.Time:
                            format = "{0}";
                            goto Label_006C;
                    }
                    throw new ArgumentOutOfRangeException();
                }
            Label_006C:
                switch (this.type)
                {
                    case PageInfoType.None:
                        return string.Empty;

                    case PageInfoType.PageNumber:
                        return string.Format(format, (object[]) new object[] { ((int) this.currentPage) });

                    case PageInfoType.PageCount:
                        return string.Format(format, (object[]) new object[] { ((int) this.pageCount) });

                    case PageInfoType.TwoDimensionPageNumber:
                        return string.Format(format, (object[]) new object[] { ((int) this.currentPage), ((int) this.currentVPage), ((int) this.currentHPage) });

                    case PageInfoType.CurrentOfTotal:
                        return string.Format(format, (object[]) new object[] { ((int) this.currentPage), ((int) this.pageCount) });

                    case PageInfoType.DateTime:
                        return string.Format(format, (object[]) new object[] { this.dateTime });

                    case PageInfoType.Date:
                        return string.Format(format, (object[]) new object[] { this.dateTime.ToShortDateString() });

                    case PageInfoType.Time:
                        return string.Format(format, (object[]) new object[] { this.dateTime.ToShortTimeString() });
                }
                throw new ArgumentOutOfRangeException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the page information type.
        /// </summary>
        /// <value>The type. The default value is <see cref="T:Dt.Cells.Data.PageInfoType">CurrentOfTotal</see>.</value>
        [DefaultValue(4)]
        public PageInfoType Type
        {
            get { return  this.type; }
            set { this.type = value; }
        }
    }
}

