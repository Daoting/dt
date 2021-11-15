#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the dimensions of the printed page margins.
    /// </summary>
    public class Margins : IXmlSerializable
    {
        int bottom;
        int footer;
        int header;
        int left;
        int right;
        int top;

        /// <summary>
        /// Creates new margin settings.
        /// </summary>
        public Margins()
        {
            this.Init();
        }

        /// <summary>
        /// Creates new margin settings, using the specified settings for the top, bottom, left, and right margins.
        /// </summary>
        /// <param name="top">The top margin, in hundredths of an inch. </param>
        /// <param name="bottom">The bottom margin, in hundredths of an inch. </param>
        /// <param name="left">The left margin, in hundredths of an inch. </param>
        /// <param name="right">The right margin, in hundredths of an inch. </param>
        public Margins(int top, int bottom, int left, int right)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Creates new margin settings, using the specified settings for the top, bottom, left, and right margins, and for the header and footer offsets.
        /// </summary>
        /// <param name="top">The top margin, in hundredths of an inch. </param>
        /// <param name="bottom">The bottom margin, in hundredths of an inch. </param>
        /// <param name="left">The left margin, in hundredths of an inch. </param>
        /// <param name="right">The right margin, in hundredths of an inch. </param>
        /// <param name="header">The header margin, in hundredths of an inch. </param>
        /// <param name="footer">The footer margin, in hundredths of an inch. </param>
        public Margins(int top, int bottom, int left, int right, int header, int footer) : this(top, bottom, left, right)
        {
            this.header = header;
            this.footer = footer;
        }

        /// <summary>
        /// Determines whether these margin settings are equal to another Margin object's settings.
        /// </summary>
        /// <param name="other">The other Margin object.</param>
        /// <returns><c>true</c> if the Margin objects are equal.</returns>
        public bool Equals(Margins other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }
            return (object.ReferenceEquals(this, other) || (((((other.top == this.top) && (other.bottom == this.bottom)) && ((other.left == this.left) && (other.right == this.right))) && (other.header == this.header)) && (other.footer == this.footer)));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj" /> parameter is null.
        /// </exception>
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
            if (obj.GetType() != typeof(Margins))
            {
                return false;
            }
            return this.Equals((Margins) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            int num = (this.top * 0x18d) ^ this.bottom;
            num = (num * 0x18d) ^ this.left;
            num = (num * 0x18d) ^ this.right;
            num = (num * 0x18d) ^ this.header;
            return ((num * 0x18d) ^ this.footer);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Init()
        {
            this.top = 0x4b;
            this.bottom = 0x4b;
            this.left = 70;
            this.right = 70;
            this.header = 30;
            this.footer = 30;
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
                if (str != "top")
                {
                    if (str != "bottom")
                    {
                        if (str == "left")
                        {
                            this.left = Serializer.ReadAttributeInt("value", 70, reader);
                            return;
                        }
                        if (str == "right")
                        {
                            this.right = Serializer.ReadAttributeInt("value", 70, reader);
                            return;
                        }
                        if (str == "header")
                        {
                            this.header = Serializer.ReadAttributeInt("value", 30, reader);
                            return;
                        }
                        if (str == "footer")
                        {
                            this.footer = Serializer.ReadAttributeInt("value", 30, reader);
                        }
                        return;
                    }
                }
                else
                {
                    this.top = Serializer.ReadAttributeInt("value", 0x4b, reader);
                    return;
                }
                this.bottom = Serializer.ReadAttributeInt("value", 0x4b, reader);
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
            if (this.top != 0x4b)
            {
                Serializer.SerializeObj((int) this.top, "top", false, writer);
            }
            if (this.bottom != 0x4b)
            {
                Serializer.SerializeObj((int) this.bottom, "bottom", false, writer);
            }
            if (this.left != 70)
            {
                Serializer.SerializeObj((int) this.left, "left", false, writer);
            }
            if (this.right != 70)
            {
                Serializer.SerializeObj((int) this.right, "right", false, writer);
            }
            if (this.header != 30)
            {
                Serializer.SerializeObj((int) this.header, "header", false, writer);
            }
            if (this.footer != 30)
            {
                Serializer.SerializeObj((int) this.footer, "footer", false, writer);
            }
        }

        /// <summary>
        /// Gets or sets the bottom margin, in hundredths of an inch. 
        /// </summary>
        /// <value>The bottom margin.</value>
        public int Bottom
        {
            get { return  this.bottom; }
            set { this.bottom = value; }
        }

        /// <summary>
        /// Gets or sets the footer offset, in hundredths of an inch. 
        /// </summary>
        /// <value>The footer offset.</value>
        public int Footer
        {
            get { return  this.footer; }
            set { this.footer = value; }
        }

        /// <summary>
        /// Gets or sets the header offset, in hundredths of an inch. 
        /// </summary>
        /// <value>The header offset.</value>
        public int Header
        {
            get { return  this.header; }
            set { this.header = value; }
        }

        /// <summary>
        /// Gets or sets the left margin, in hundredths of an inch. 
        /// </summary>
        /// <value>The left margin.</value>
        public int Left
        {
            get { return  this.left; }
            set { this.left = value; }
        }

        /// <summary>
        /// Gets or sets the right margin, in hundredths of an inch. 
        /// </summary>
        /// <value>The right margin.</value>
        public int Right
        {
            get { return  this.right; }
            set { this.right = value; }
        }

        /// <summary>
        /// Gets or sets the top margin, in hundredths of an inch. 
        /// </summary>
        /// <value>The top margin.</value>
        public int Top
        {
            get { return  this.top; }
            set { this.top = value; }
        }
    }
}

