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
    /// 纸张边距，单位：0.01英寸
    /// </summary>
    public class Margins : IXmlSerializable
    {
        int _bottom;
        int _footer;
        int _header;
        int _left;
        int _right;
        int _top;

        /// <summary>
        /// 纸张边距，单位：0.01英寸
        /// </summary>
        public Margins()
        {
            Init();
        }

        /// <summary>
        /// 纸张边距，单位：0.01英寸
        /// </summary>
        /// <param name="top">The top margin, in hundredths of an inch. </param>
        /// <param name="bottom">The bottom margin, in hundredths of an inch. </param>
        /// <param name="left">The left margin, in hundredths of an inch. </param>
        /// <param name="right">The right margin, in hundredths of an inch. </param>
        public Margins(int top, int bottom, int left, int right)
        {
            _top = top;
            _bottom = bottom;
            _left = left;
            _right = right;
        }

        /// <summary>
        /// 纸张边距，单位：0.01英寸
        /// </summary>
        /// <param name="top">The top margin, in hundredths of an inch. </param>
        /// <param name="bottom">The bottom margin, in hundredths of an inch. </param>
        /// <param name="left">The left margin, in hundredths of an inch. </param>
        /// <param name="right">The right margin, in hundredths of an inch. </param>
        /// <param name="header">The header margin, in hundredths of an inch. </param>
        /// <param name="footer">The footer margin, in hundredths of an inch. </param>
        public Margins(int top, int bottom, int left, int right, int header, int footer) : this(top, bottom, left, right)
        {
            _header = header;
            _footer = footer;
        }

        /// <summary>
        /// 页眉高度，单位：0.01英寸，默认0
        /// </summary>
        public int Header
        {
            get { return _header; }
            set { _header = value; }
        }

        /// <summary>
        /// 页眉高度，单位：dpi像素数
        /// </summary>
        public int PxHeader => (int)Math.Round(_header * UnitManager.Dpi / 100);
        
        /// <summary>
        /// 页脚高度，单位：0.01英寸，默认0
        /// </summary>
        public int Footer
        {
            get { return _footer; }
            set { _footer = value; }
        }

        /// <summary>
        /// 页脚高度，单位：dpi像素数
        /// </summary>
        public int PxFooter => (int)Math.Round(_footer * UnitManager.Dpi / 100);

        /// <summary>
        /// 左边距，单位：0.01英寸，默认70
        /// </summary>
        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// 左边距，单位：dpi像素数
        /// </summary>
        public int PxLeft => (int)Math.Round(_left * UnitManager.Dpi / 100);

        /// <summary>
        /// 右边距，单位：0.01英寸，默认70
        /// </summary>
        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }

        /// <summary>
        /// 右边距，单位：dpi像素数
        /// </summary>
        public int PxRight => (int)Math.Round(_right * UnitManager.Dpi / 100);

        /// <summary>
        /// 上边距，单位：0.01英寸，默认100
        /// </summary>
        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        /// <summary>
        /// 上边距，单位：dpi像素数
        /// </summary>
        public int PxTop => (int)Math.Round(_top * UnitManager.Dpi / 100);

        /// <summary>
        /// 下边距，单位：0.01英寸，默认100
        /// </summary>
        public int Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        /// <summary>
        /// 下边距，单位：dpi像素数
        /// </summary>
        public int PxBottom => (int)Math.Round(_bottom * UnitManager.Dpi / 100);

        void Init()
        {
            _top = 100;
            _bottom = 100;
            _left = 70;
            _right = 70;
            _header = 0;
            _footer = 0;
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
                            _left = Serializer.ReadAttributeInt("value", 70, reader);
                            return;
                        }
                        if (str == "right")
                        {
                            _right = Serializer.ReadAttributeInt("value", 70, reader);
                            return;
                        }
                        if (str == "header")
                        {
                            _header = Serializer.ReadAttributeInt("value", 0, reader);
                            return;
                        }
                        if (str == "footer")
                        {
                            _footer = Serializer.ReadAttributeInt("value", 0, reader);
                        }
                        return;
                    }
                }
                else
                {
                    _top = Serializer.ReadAttributeInt("value", 100, reader);
                    return;
                }
                _bottom = Serializer.ReadAttributeInt("value", 100, reader);
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
            Init();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    ReadXmlBase(reader);
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
            WriteXmlBase(writer);
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
            if (_top != 100)
            {
                Serializer.SerializeObj((int) _top, "top", false, writer);
            }
            if (_bottom != 100)
            {
                Serializer.SerializeObj((int) _bottom, "bottom", false, writer);
            }
            if (_left != 70)
            {
                Serializer.SerializeObj((int) _left, "left", false, writer);
            }
            if (_right != 70)
            {
                Serializer.SerializeObj((int) _right, "right", false, writer);
            }
            if (_header != 0)
            {
                Serializer.SerializeObj((int) _header, "header", false, writer);
            }
            if (_footer != 0)
            {
                Serializer.SerializeObj((int) _footer, "footer", false, writer);
            }
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
            return (object.ReferenceEquals(this, other) || (((((other._top == _top) && (other._bottom == _bottom)) && ((other._left == _left) && (other._right == _right))) && (other._header == _header)) && (other._footer == _footer)));
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
            return Equals((Margins)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            int num = (_top * 0x18d) ^ _bottom;
            num = (num * 0x18d) ^ _left;
            num = (num * 0x18d) ^ _right;
            num = (num * 0x18d) ^ _header;
            return ((num * 0x18d) ^ _footer);
        }
    }
}

