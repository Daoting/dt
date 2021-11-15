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
    /// Represents report controls used for creating reports. 
    /// </summary>
    internal abstract class GcControl : IXmlSerializable
    {
        protected int x;
        protected int y;

        /// <summary>
        /// Creates a new control.
        /// </summary>
        protected GcControl()
        {
            this.Init();
        }

        /// <summary>
        /// Creates a new control with the specified <i>x</i> and <i>y</i> values.
        /// </summary>
        /// <param name="x">The <i>x</i> value.</param>
        /// <param name="y">The <i>y</i> value.</param>
        protected GcControl(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Init()
        {
            this.x = 0;
            this.y = 0;
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
                if (str != "left")
                {
                    if (str != "top")
                    {
                        return;
                    }
                }
                else
                {
                    this.x = Serializer.ReadAttributeInt("value", 0, reader);
                    return;
                }
                this.y = Serializer.ReadAttributeInt("value", 0, reader);
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
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized</param>
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
        /// Gets or sets the <i>x</i> value, in hundredths of an inch.
        /// </summary>
        /// <value>The <i>x</i> value. The default value is 0.</value>
        [DefaultValue(0)]
        public virtual int X
        {
            get { return  this.x; }
            set { this.x = value; }
        }

        /// <summary>
        /// Gets or sets the <i>y</i> value, in hundredths of an inch.
        /// </summary>
        /// <value>The <i>y</i> value. The default value is 0.</value>
        [DefaultValue(0)]
        public virtual int Y
        {
            get { return  this.y; }
            set { this.y = value; }
        }
    }
}

