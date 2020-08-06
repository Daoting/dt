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
    /// Defines an abstract class for an appendix report section.
    /// </summary>
    internal abstract class GcAppendixSection : GcSection
    {
        bool horizontalExtend;

        /// <summary>
        /// Creates a new appendix section.
        /// </summary>
        internal GcAppendixSection()
        {
            this.Init();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.horizontalExtend = false;
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
                string str;
                if (((str = reader.Name) != null) && (str == "HorizontalExtend"))
                {
                    this.horizontalExtend = Serializer.ReadAttributeBoolean("value", false, reader);
                }
                else
                {
                    base.ReadXmlBase(reader);
                }
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
        /// Gets or sets a value that indicates whether the controls of this section can extend to the next horizontal page.
        /// </summary>
        /// <value>
        /// <c>true</c> if the controls of this section can extend to the next horizontal page; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool HorizontalExtend
        {
            get { return  this.horizontalExtend; }
            set { this.horizontalExtend = value; }
        }
    }
}

