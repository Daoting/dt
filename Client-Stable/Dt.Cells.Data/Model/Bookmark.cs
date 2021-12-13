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
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a bookmark for the report.
    /// </summary>
    internal class Bookmark : IXmlSerializable
    {
        int index;
        Bookmark parent;
        int parentIndex;
        internal static int SIndex;
        string text;

        /// <summary>
        /// Creates a new bookmark.
        /// </summary>
        public Bookmark()
        {
            this.Init();
            this.index = SIndex;
            SIndex++;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Init()
        {
            this.text = string.Empty;
            this.parent = null;
            this.index = -1;
            this.parentIndex = -1;
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadXmlBase(XmlReader reader)
        {
            string str;
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
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "Text")
                {
                    if (str != "Index")
                    {
                        if (str == "ParentIndex")
                        {
                            this.parentIndex = Serializer.ReadAttributeInt("value", -1, reader);
                        }
                        return;
                    }
                }
                else
                {
                    this.text = Serializer.ReadAttribute("value", reader);
                    return;
                }
                this.index = Serializer.ReadAttributeInt("value", -1, reader);
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
        /// Internal only.
        /// Gets the index.
        /// </summary>
        /// <value>The index</value>
        internal int Index
        {
            get { return  this.index; }
        }

        /// <summary>
        /// Internal only.
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c></value>
        internal bool IsEmpty
        {
            get { return  string.IsNullOrEmpty(this.text); }
        }

        /// <summary>
        /// Gets or sets the parent of the bookmark.
        /// </summary>
        /// <value>The parent <see cref="T:Dt.Cells.Data.Bookmark" /> instance. The default value is null.</value>
        [DefaultValue((string) null)]
        public Bookmark Parent
        {
            get { return  this.parent; }
            set
            {
                if (value != null)
                {
                    this.parentIndex = value.index;
                }
                else
                {
                    this.parentIndex = -1;
                }
                this.parent = value;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets the index of the parent.
        /// </summary>
        /// <value>The index of the parent</value>
        internal int ParentIndex
        {
            get { return  this.parentIndex; }
        }

        /// <summary>
        /// Gets or sets the text of the bookmark.
        /// </summary>
        /// <value>The text. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Text
        {
            get { return  this.text; }
            set { this.text = value; }
        }
    }
}

