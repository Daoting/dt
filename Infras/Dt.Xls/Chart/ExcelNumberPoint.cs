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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies data for a particular data point.
    /// </summary>
    public class ExcelNumberPoint
    {
        internal void ReadXml(XElement node)
        {
            this.Value = node.GetChildElementValue("v");
            this.FormatCode = node.GetAttributeValueOrDefaultOfStringType("formatCode", null);
            this.Index = node.GetAttributeValueOrDefaultOfInt32Type("idx", 0);
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if (writer != null)
            {
                writer.WriteStartElement("c", "pt", null);
                writer.WriteAttributeString("idx", ((int) this.Index).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                if (!string.IsNullOrWhiteSpace(this.FormatCode))
                {
                    writer.WriteAttributeString("c", "formatCode", null, this.FormatCode);
                }
                writer.WriteElementString("c", "v", null, this.Value);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// A string representing the format code to apply.
        /// </summary>
        public string FormatCode { get; set; }

        /// <summary>
        /// The index of the series in the collections.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The number point value
        /// </summary>
        public string Value { get; set; }
    }
}

