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
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies a set of strings used for a chart.
    /// </summary>
    public class StringLiteralData
    {
        private List<string> _literalDatas = new List<string>();

        internal void ReadXml(XElement element)
        {
            int num = (int) ((int) element.GetChildElementAttributeValueOrDefault<int>("ptCount", "val"));
            if (num > 0)
            {
                string[] strArray = new string[num];
                foreach (XElement element2 in element.Elements())
                {
                    int index = element2.GetAttributeValueOrDefaultOfInt32Type("idx", 0);
                    strArray[index] = element2.GetChildElementValue("v");
                }
                this._literalDatas = new List<string>(strArray);
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if (this._literalDatas.Count > 0)
            {
                writer.WriteStartElement("c", "strLit", null);
                writer.WriteStartElement("c", "ptCount", null);
                writer.WriteAttributeString("val", null, ((int) this._literalDatas.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteEndElement();
                for (int i = 0; i < this._literalDatas.Count; i++)
                {
                    writer.WriteStartElement("c", "pt", null);
                    writer.WriteAttributeString("idx", null, ((int) i).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    writer.WriteElementString("c", "v", null, this._literalDatas[i]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// The count of strings this collection contains.
        /// </summary>
        public int Count
        {
            get { return  this.StringLiteralDatas.Count; }
        }

        /// <summary>
        /// Represents a string collections which this container contains.
        /// </summary>
        public List<string> StringLiteralDatas
        {
            get { return  this._literalDatas; }
        }
    }
}

