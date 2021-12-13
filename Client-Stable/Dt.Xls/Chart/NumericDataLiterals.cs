#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies a set of numbers used for the parent element.
    /// </summary>
    public class NumericDataLiterals
    {
        private List<ExcelNumberPoint> _numberPoints = new List<ExcelNumberPoint>();

        internal void ReadXml(XElement node)
        {
            ExcelNumberPoint[] pointArray = null;
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "formatCode")
                {
                    this.FormatCode = element.Value;
                }
                else if (element.Name.LocalName == "ptCount")
                {
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("val", -1);
                    if (num < 0)
                    {
                        break;
                    }
                    pointArray = new ExcelNumberPoint[num];
                }
                else if ((element.Name.LocalName == "pt") && (pointArray != null))
                {
                    ExcelNumberPoint point = new ExcelNumberPoint();
                    point.ReadXml(element);
                    pointArray[point.Index] = point;
                }
            }
            if (pointArray != null)
            {
                this._numberPoints = new List<ExcelNumberPoint>(pointArray);
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "numLit", null);
            if (!string.IsNullOrWhiteSpace(this.FormatCode))
            {
                writer.WriteElementString("c", "formatCode", null, this.FormatCode);
            }
            else
            {
                writer.WriteElementString("c", "formatCode", null, "General");
            }
            if (this.NumberPoints.Count > 0)
            {
                using (writer.WriteElement("ptCount", null, "c"))
                {
                    writer.WriteAttributeString("val", ((int) this.NumberPoints.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                using (List<ExcelNumberPoint>.Enumerator enumerator = this.NumberPoints.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.WriteXml(writer, mFolder, chartFile);
                    }
                }
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Sepcifies a string representing the format code to apply
        /// </summary>
        public string FormatCode { get; set; }

        /// <summary>
        /// Represents a collection of number points which the container contains.
        /// </summary>
        public List<ExcelNumberPoint> NumberPoints
        {
            get { return  this._numberPoints; }
        }
    }
}

