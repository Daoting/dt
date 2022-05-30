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
    /// Represents axis information for columns and rows.
    /// </summary>
    internal sealed class AxisInfo : IXmlSerializable, ICloneable, IsEmptySupport
    {
        bool resizable;
        bool resizableSet;
        double size;
        bool sizeSet;
        object tag;
        bool tagSet;
        bool visible;
        bool visibleSet;

        /// <summary>
        /// Creates new axis information for the style model.
        /// </summary>
        public AxisInfo()
        {
            this.Init();
        }

        /// <summary>
        /// Composes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        public void Compose(AxisInfo source)
        {
            if (source != null)
            {
                if (source.sizeSet)
                {
                    this.size = source.size;
                    this.sizeSet = true;
                }
                if (source.visibleSet)
                {
                    this.visible = source.visible;
                    this.visibleSet = true;
                }
                if (source.resizableSet)
                {
                    this.resizable = source.resizable;
                    this.resizableSet = true;
                }
                if (source.tagSet)
                {
                    this.tag = Worksheet.CloneObject(source.tag);
                    this.tagSet = true;
                }
            }
        }

        /// <summary>
        /// Copies properties from another instance.
        /// </summary>
        /// <param name="info">The axis information source to copy.</param>
        public void CopyFrom(AxisInfo info)
        {
            if (info != null)
            {
                this.size = info.size;
                this.visible = info.visible;
                this.resizable = info.resizable;
                this.tag = Worksheet.CloneObject(info.tag);
                this.sizeSet = info.sizeSet;
                this.visibleSet = info.visibleSet;
                this.resizableSet = info.resizableSet;
                this.tagSet = info.tagSet;
            }
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns>The schema XML string.</returns>
        public XmlSchema GetSchema()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            AxisInfo info = new AxisInfo();
            if (this.resizableSet)
            {
                info.Resizable = this.Resizable;
                info.resizableSet = this.resizableSet;
            }
            if (this.sizeSet)
            {
                info.Size = this.Size;
                info.sizeSet = this.sizeSet;
            }
            if (this.visibleSet)
            {
                info.Visible = this.Visible;
                info.visibleSet = this.visibleSet;
            }
            if (this.tagSet)
            {
                info.Tag = Worksheet.CloneObject(this.Tag);
                info.tagSet = this.tagSet;
            }
            return info;
        }

        void Init()
        {
            this.size = -1.0;
            this.sizeSet = false;
            this.visible = true;
            this.visibleSet = false;
            this.resizable = true;
            this.resizableSet = false;
            this.tag = null;
            this.tagSet = false;
        }

        /// <summary>
        /// Determines whether the Resizable property is set.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the Resizable property is set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsResizableSet()
        {
            return this.resizableSet;
        }

        /// <summary>
        /// Determines whether the Size property is set.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the Size property is set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSizeSet()
        {
            return this.sizeSet;
        }

        /// <summary>
        /// Determines whether the Tag property is set.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the Tag property is set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTagSet()
        {
            return this.tagSet;
        }

        /// <summary>
        /// Determines whether the Visible property is set.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the Visible property is set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsVisibleSet()
        {
            return this.visibleSet;
        }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            this.Init();
            while (reader.Read())
            {
                Type type;
                string str2;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str2 = reader.Name) != null))
                {
                    if (str2 != "Size")
                    {
                        if (str2 == "IsVisible")
                        {
                            goto Label_0087;
                        }
                        if (str2 == "CanUserResize")
                        {
                            goto Label_00AB;
                        }
                        if (str2 == "Tag")
                        {
                            goto Label_00CF;
                        }
                    }
                    else
                    {
                        this.size = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                        this.sizeSet = true;
                    }
                }
                continue;
            Label_0087:
                this.visible = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                this.visibleSet = true;
                continue;
            Label_00AB:
                this.resizable = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                this.resizableSet = true;
                continue;
            Label_00CF:
                type = Serializer.CreateType(Serializer.ReadAttribute("type", reader), string.Empty);
                this.tag = Serializer.DeserializeObj(type, reader);
                this.tagSet = true;
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.ResetSize();
            this.ResetVisible();
            this.ResetResizable();
            this.ResetTag();
        }

        /// <summary>
        /// Resets the value of the Resizable property.
        /// </summary>
        public void ResetResizable()
        {
            this.resizable = true;
            this.resizableSet = false;
        }

        /// <summary>
        /// Resets the value of the Size property.
        /// </summary>
        public void ResetSize()
        {
            this.size = -1.0;
            this.sizeSet = false;
        }

        /// <summary>
        /// Resets the value of the Tag property.
        /// </summary>
        public void ResetTag()
        {
            this.tag = null;
            this.tagSet = false;
        }

        /// <summary>
        /// Resets the value of the Visible property.
        /// </summary>
        public void ResetVisible()
        {
            this.visible = true;
            this.visibleSet = false;
        }

        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.sizeSet)
            {
                Serializer.SerializeObj((double) this.size, "Size", writer);
            }
            if (this.visibleSet)
            {
                Serializer.SerializeObj((bool) this.visible, "IsVisible", writer);
            }
            if (this.resizableSet)
            {
                Serializer.SerializeObj((bool) this.resizable, "CanUserResize", writer);
            }
            if (this.tagSet)
            {
                writer.WriteStartElement("Tag");
                Serializer.WriteTypeAttr(this.tag, writer);
                Serializer.SerializeObj(this.tag, null, false, writer);
                writer.WriteEndElement();
            }
        }

        bool IsEmptySupport.IsEmpty
        {
            get { return  (((!this.sizeSet && !this.visibleSet) && !this.resizableSet) && !this.tagSet); }
        }

        /// <summary>
        /// Gets a value that indicates whether the sheet axis can be resized.
        /// </summary>
        /// <value><c>true</c> if the sheet axis can be resized; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Resizable
        {
            get { return  this.resizable; }
            internal set
            {
                this.resizable = value;
                this.resizableSet = true;
            }
        }

        /// <summary>
        /// Gets the cell width in this sheet axis.
        /// </summary>
        /// <value>The cell width in this sheet axis.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The specified width is out of range (must be between -1 and 9,999,999 pixels).
        /// </exception>
        [DefaultValue(-1)]
        public double Size
        {
            get { return  this.size; }
            internal set
            {
                this.size = value;
                this.sizeSet = true;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the sheet axis tag.
        /// </summary>
        [DefaultValue((string) null)]
        public object Tag
        {
            get { return  this.tag; }
            internal set
            {
                this.tag = value;
                this.tagSet = true;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the sheet axis is visible.
        /// </summary>
        /// <value><c>true</c> if the sheet axis is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Visible
        {
            get { return  this.visible; }
            internal set
            {
                this.visible = value;
                this.visibleSet = true;
            }
        }
    }
}

