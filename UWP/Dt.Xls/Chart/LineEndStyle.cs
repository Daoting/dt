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
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represent the line end style
    /// </summary>
    public class LineEndStyle
    {
        internal void ReadXml(XElement node)
        {
            string str = node.GetAttributeValueOrDefaultOfStringType("len", "lg");
            string str2 = node.GetAttributeValueOrDefaultOfStringType("type", "none");
            string str3 = node.GetAttributeValueOrDefaultOfStringType("w", "lg");
            string str4 = str;
            if (str4 != null)
            {
                if (str4 == "lg")
                {
                    this.Length = LineSize.Large;
                }
                else if (str4 == "med")
                {
                    this.Length = LineSize.Medium;
                }
                else if (str4 == "sm")
                {
                    this.Length = LineSize.Small;
                }
            }
            switch (str2)
            {
                case "arrow":
                    this.Type = LineEndType.ArrowHead;
                    break;

                case "diamond":
                    this.Type = LineEndType.Diamond;
                    break;

                case "none":
                    this.Type = LineEndType.None;
                    break;

                case "oval":
                    this.Type = LineEndType.Oval;
                    break;

                case "stealth":
                    this.Type = LineEndType.StealthArrow;
                    break;

                case "triangle":
                    this.Type = LineEndType.Traingle;
                    break;
            }
            string str6 = str3;
            if (str6 != null)
            {
                if (str6 != "lg")
                {
                    if (str6 != "med")
                    {
                        if (str6 == "sm")
                        {
                            this.Width = LineSize.Small;
                        }
                        return;
                    }
                }
                else
                {
                    this.Width = LineSize.Large;
                    return;
                }
                this.Width = LineSize.Medium;
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            switch (this.Type)
            {
                case LineEndType.Traingle:
                    writer.WriteAttributeString("type", "triangle");
                    break;

                case LineEndType.ArrowHead:
                    writer.WriteAttributeString("type", "arrow");
                    break;

                case LineEndType.StealthArrow:
                    writer.WriteAttributeString("type", "stealth");
                    break;

                case LineEndType.Diamond:
                    writer.WriteAttributeString("type", "diamond");
                    break;

                case LineEndType.Oval:
                    writer.WriteAttributeString("type", "oval");
                    break;
            }
            switch (this.Width)
            {
                case LineSize.Large:
                    writer.WriteAttributeString("w", "lg");
                    break;

                case LineSize.Medium:
                    writer.WriteAttributeString("w", "med");
                    break;

                case LineSize.Small:
                    writer.WriteAttributeString("w", "sm");
                    break;
            }
            switch (this.Length)
            {
                case LineSize.Large:
                    writer.WriteAttributeString("len", "lg");
                    return;

                case LineSize.Medium:
                    writer.WriteAttributeString("len", "med");
                    return;

                case LineSize.Small:
                    writer.WriteAttributeString("len", "sm");
                    return;
            }
        }

        /// <summary>
        /// Specified the line length.
        /// </summary>
        public LineSize Length { get; set; }

        /// <summary>
        /// Specifies the line end decoration.
        /// </summary>
        public LineEndType Type { get; set; }

        /// <summary>
        /// Specified the line width.
        /// </summary>
        public LineSize Width { get; set; }
    }
}

