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
    /// Represents icon criteria.
    /// </summary>
    public sealed class IconCriterion : IXmlSerializable
    {
        object iconValue;
        Dt.Cells.Data.IconValueType iconValueType;
        bool isGreaterThanOrEqualTo;

        /// <summary>
        /// Creates new icon criteria.
        /// </summary>
        public IconCriterion() : this(true, Dt.Cells.Data.IconValueType.Percent, null)
        {
        }

        /// <summary>
        /// Creates new icon criteria with the specified operator, scale value, and type.
        /// </summary>
        /// <param name="isGreaterThanOrEqualTo">If set to <c>true</c> use the greater than and equal to operator to calculate the value.</param>
        /// <param name="type">Type of scale value.</param>
        /// <param name="value">Scale value.</param>
        /// <remarks>
        /// If the <paramref name="isGreaterThanOrEqualTo" /> parameter is set to <c>true</c>, 
        /// use the greater than and equal to operator to calculate the value.
        /// </remarks>
        public IconCriterion(bool isGreaterThanOrEqualTo, Dt.Cells.Data.IconValueType type, object value)
        {
            this.isGreaterThanOrEqualTo = isGreaterThanOrEqualTo;
            this.iconValueType = type;
            this.iconValue = value;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "IsGreaterThanOrEqualTo")
                    {
                        if (str == "IconValueType")
                        {
                            goto Label_005E;
                        }
                        if (str == "IconValue")
                        {
                            goto Label_007B;
                        }
                    }
                    else
                    {
                        this.isGreaterThanOrEqualTo = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    }
                }
                continue;
            Label_005E:
                this.iconValueType = (Dt.Cells.Data.IconValueType) Serializer.DeserializeObj(typeof(Dt.Cells.Data.IconValueType), reader);
                continue;
            Label_007B:
                this.iconValue = Serializer.DeserializeObj(null, reader);
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.SerializeObj((bool) this.isGreaterThanOrEqualTo, "IsGreaterThanOrEqualTo", writer);
            Serializer.SerializeObj(this.iconValueType, "IconValueType", writer);
            Serializer.WriteStartObj("IconValue", writer);
            Serializer.WriteTypeAttr(this.iconValue, writer);
            Serializer.SerializeObj(this.iconValue, null, writer);
            Serializer.WriteEndObj(writer);
        }

        /// <summary>
        /// Gets or sets the scale value type.
        /// </summary>
        /// <value>
        /// A value that specifies the type of scale value.
        /// The default value is <see cref="T:Dt.Cells.Data.ScaleValueType">Percent</see>.
        /// </value>
        public Dt.Cells.Data.IconValueType IconValueType
        {
            get { return  this.iconValueType; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to use the greater than and equal to operator.
        /// </summary>
        /// <value>
        /// <c>true</c> if the criteria use the greater than and equal to operator; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        public bool IsGreaterThanOrEqualTo
        {
            get { return  this.isGreaterThanOrEqualTo; }
        }

        /// <summary>
        /// Gets or sets the original scale value.
        /// </summary>
        /// <value>The original scale value. The default value is null.</value>
        public object Value
        {
            get { return  this.iconValue; }
        }
    }
}

