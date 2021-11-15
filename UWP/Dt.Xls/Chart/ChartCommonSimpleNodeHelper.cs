#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Utils;
using System;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    internal static class ChartCommonSimpleNodeHelper
    {
        internal static NumberFormat ReadNumberFormatNode(XElement element)
        {
            return new NumberFormat { NumberFormatCode = element.GetAttributeValueOrDefaultOfStringType("formatCode", "General"), LinkToSource = element.GetAttributeValueOrDefaultOfBooleanType("sourceLinked", true) };
        }

        internal static void WriteNummberFormatNode(XmlWriter writer, NumberFormat numberFormat)
        {
            if (((numberFormat.NumberFormatCode != "General") && !string.IsNullOrWhiteSpace(numberFormat.NumberFormatCode)) || !numberFormat.LinkToSource)
            {
                writer.WriteStartElement("c", "numFmt", null);
                writer.WriteAttributeString("formatCode", numberFormat.NumberFormatCode);
                writer.WriteAttributeString("sourceLinked", numberFormat.LinkToSource ? "1" : "0");
                writer.WriteEndElement();
            }
        }
    }
}

