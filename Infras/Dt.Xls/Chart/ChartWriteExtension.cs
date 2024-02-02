#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.OOXml;
using System;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    internal static class ChartWriteExtension
    {
        internal static IFillFormat ReadFillFormat(this XElement item, MemoryFolder mFolder, XFile xFile)
        {
            if (item != null)
            {
                if (item.Name.LocalName == "solidFill")
                {
                    SolidFillFormat format = new SolidFillFormat();
                    format.ReadXml(item);
                    return format;
                }
                if (item.Name.LocalName == "pattFill")
                {
                    PatternFill fill = new PatternFill();
                    fill.ReadXml(item);
                    return fill;
                }
                if (item.Name.LocalName == "noFill")
                {
                    NoFillFormat format2 = new NoFillFormat();
                    format2.ReadXml(item);
                    return format2;
                }
                if (item.Name.LocalName == "gradFill")
                {
                    GradientFillFormat format3 = new GradientFillFormat();
                    format3.ReadXml(item);
                    return format3;
                }
                if (item.Name.LocalName == "blipFill")
                {
                    BlipFillFormat format4 = new BlipFillFormat();
                    format4.ReadXml(item, mFolder, xFile);
                    return format4;
                }
            }
            return null;
        }

        internal static void WriteXml(this IFillFormat fillFormat, XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            if (fillFormat != null)
            {
                if (fillFormat is NoFillFormat)
                {
                    (fillFormat as NoFillFormat).WriteXml(writer, mFolder, chartFile);
                }
                else if (fillFormat is SolidFillFormat)
                {
                    (fillFormat as SolidFillFormat).WriteXml(writer, mFolder, chartFile);
                }
                else if (fillFormat is PatternFill)
                {
                    (fillFormat as PatternFill).WriteXml(writer, mFolder, chartFile);
                }
                else if (fillFormat is GradientFillFormat)
                {
                    (fillFormat as GradientFillFormat).WriteXml(writer, mFolder, chartFile);
                }
                else if (fillFormat is BlipFillFormat)
                {
                    (fillFormat as BlipFillFormat).WriteXml(writer, mFolder, chartFile);
                }
            }
        }
    }
}

