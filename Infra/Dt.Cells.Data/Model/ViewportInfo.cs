#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a ViewportInfo object.
    /// </summary>
    public class ViewportInfo : IXmlSerializable
    {
        int _columnViewportCount;
        int[] _leftColumns;
        int _rowViewportCount;
        int[] _topRows;
        double[] _viewportHeight;
        double[] _viewportWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public ViewportInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ViewportInfo" /> class.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="rowViewportCount">The row viewport count.</param>
        /// <param name="columnViewportCount">The column viewport count.</param>
        public ViewportInfo(Worksheet sheet, int rowViewportCount, int columnViewportCount)
        {
            this._rowViewportCount = rowViewportCount;
            this._columnViewportCount = columnViewportCount;
            this._topRows = new int[rowViewportCount];
            this._leftColumns = new int[columnViewportCount];
            this._viewportHeight = new double[rowViewportCount];
            this._viewportWidth = new double[columnViewportCount];
            for (int i = 0; i < rowViewportCount; i++)
            {
                this._topRows[i] = sheet.FrozenRowCount;
                this._viewportHeight[i] = -1.0;
            }
            for (int j = 0; j < columnViewportCount; j++)
            {
                this._leftColumns[j] = sheet.FrozenColumnCount;
                this._viewportWidth[j] = -1.0;
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            int num = int.Parse(Serializer.ReadAttribute("rowPaneCount", reader));
            this._rowViewportCount = num;
            int num2 = int.Parse(Serializer.ReadAttribute("columnPaneCount", reader));
            this._columnViewportCount = num2;
            while (reader.Read())
            {
                XmlReader reader3;
                XmlReader reader4;
                XmlReader reader5;
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "ViewportLeftColumns")
                    {
                        if (str == "ViewportTopRows")
                        {
                            goto Label_00C0;
                        }
                        if (str == "ViewportWidths")
                        {
                            goto Label_00EE;
                        }
                        if (str == "ViewportHeights")
                        {
                            goto Label_011C;
                        }
                    }
                    else
                    {
                        XmlReader reader2 = Serializer.ExtractNode(reader);
                        List<int> list = new List<int>();
                        Serializer.DeserializeList((IList) list, reader2);
                        reader2.Close();
                        this._leftColumns = list.ToArray();
                    }
                }
                continue;
            Label_00C0:
                reader3 = Serializer.ExtractNode(reader);
                List<int> list2 = new List<int>();
                Serializer.DeserializeList((IList) list2, reader3);
                this._topRows = list2.ToArray();
                reader3.Close();
                continue;
            Label_00EE:
                reader4 = Serializer.ExtractNode(reader);
                List<double> list3 = new List<double>();
                Serializer.DeserializeList((IList) list3, reader4);
                this._viewportWidth = list3.ToArray();
                reader4.Close();
                continue;
            Label_011C:
                reader5 = Serializer.ExtractNode(reader);
                List<double> list4 = new List<double>();
                Serializer.DeserializeList((IList) list4, reader5);
                this._viewportHeight = list4.ToArray();
                reader5.Close();
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            Serializer.WriteAttribute("rowPaneCount", this._rowViewportCount, writer);
            Serializer.WriteAttribute("columnPaneCount", this._columnViewportCount, writer);
            Serializer.WriteStartObj("ViewportLeftColumns", writer);
            Serializer.SerializeList(this.LeftColumns, writer);
            Serializer.WriteEndObj(writer);
            Serializer.WriteStartObj("ViewportTopRows", writer);
            Serializer.SerializeList(this.TopRows, writer);
            Serializer.WriteEndObj(writer);
            Serializer.WriteStartObj("ViewportWidths", writer);
            Serializer.SerializeList(this.ViewportWidth, writer);
            Serializer.WriteEndObj(writer);
            Serializer.WriteStartObj("ViewportHeights", writer);
            Serializer.SerializeList(this.ViewportHeight, writer);
            Serializer.WriteEndObj(writer);
        }

        /// <summary>
        /// Gets or sets the column index of the active viewport.
        /// </summary>
        /// <value>The column index of the active viewport.</value>
        public int ActiveColumnViewport { get; set; }

        /// <summary>
        /// Gets or sets the row index of the active viewport.
        /// </summary>
        /// <value>The row index of the active viewport.</value>
        public int ActiveRowViewport { get; set; }

        /// <summary>
        /// Gets the column viewport count.
        /// </summary>
        public int ColumnViewportCount
        {
            get { return  this._columnViewportCount; }
        }

        /// <summary>
        /// Gets the left columns of the viewports.
        /// </summary>
        public int[] LeftColumns
        {
            get { return  this._leftColumns; }
        }

        /// <summary>
        /// Gets the row viewport count.
        /// </summary>
        public int RowViewportCount
        {
            get { return  this._rowViewportCount; }
        }

        /// <summary>
        /// Gets the top rows of the viewports.
        /// </summary>
        public int[] TopRows
        {
            get { return  this._topRows; }
        }

        /// <summary>
        /// Gets the height of all the viewports.
        /// </summary>
        public double[] ViewportHeight
        {
            get { return  this._viewportHeight; }
        }

        /// <summary>
        /// Gets the width of all the viewports.
        /// </summary>
        public double[] ViewportWidth
        {
            get { return  this._viewportWidth; }
        }
    }
}

