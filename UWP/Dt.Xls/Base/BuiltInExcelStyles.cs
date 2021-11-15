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
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents all excel built-in styles.
    /// </summary>
    public sealed class BuiltInExcelStyles
    {
        private static List<ExcelStyle> _builtInStyls = new List<ExcelStyle>();

        /// <summary>
        /// Get all excel built-in styles
        /// </summary>
        /// <returns>A collection of <see cref="T:Dt.Xls.IExcelStyle" /> represents all built-in styles used in excel</returns>
        public List<IExcelStyle> GetBuiltInStyls()
        {
            if (_builtInStyls.Count != 0x2f)
            {
                this.InitBuiltInExcelStyleCollections().Wait();
            }
            List<IExcelStyle> list = new List<IExcelStyle>();
            foreach (ExcelStyle style in _builtInStyls)
            {
                list.Add(style.Copy());
            }
            return list;
        }

        internal static ExcelStyle GetNormalStyle()
        {
            ExtendedFormat format = new ExtendedFormat {
                Font = new ExcelFont()
            };
            format.Font.FontColor = new ExcelColor(ExcelColorType.Theme, 1, 0.0);
            format.Font.FontSize = 11.0;
            format.Font.FontName = "Calibri";
            return new ExcelStyle { Name = "Normal", BuiltInStyle = BuiltInStyleIndex.Normal, Format = format };
        }

        private Task InitBuiltInExcelStyleCollections()
        {
            _builtInStyls.Clear();
            return Task.Run(()=>
            {
                // hdt，内容转为嵌入资源
                using (Stream s = typeof(BuiltInExcelStyles).Assembly.GetManifestResourceStream("Dt.Xls.BuiltInStyles.xml"))
                {
                    s.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    XDocument document = XDocument.Load(s);
                    List<ExcelFont> fonts = new List<ExcelFont>();
                    List<Tuple<FillPatternType, ExcelColor, ExcelColor>> fills = new List<Tuple<FillPatternType, ExcelColor, ExcelColor>>();
                    List<ExcelBorder> borders = new List<ExcelBorder>();
                    List<ExtendedFormat> list4 = new List<ExtendedFormat>();
                    Dictionary<int, ExcelNumberFormat> dictionary = new Dictionary<int, ExcelNumberFormat>();
                    using (IEnumerator<XElement> enumerator = document.Elements().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            XElement element = enumerator.Current;
                            if (element.Name.LocalName == "styleSheet")
                            {
                                using (IEnumerator<XElement> enumerator2 = element.Elements().GetEnumerator())
                                {
                                    while (enumerator2.MoveNext())
                                    {
                                        XElement element2 = enumerator2.Current;
                                        if (element2.Name.LocalName == "numFmts")
                                        {
                                            using (IEnumerator<XElement> enumerator3 = element2.Elements().GetEnumerator())
                                            {
                                                while (enumerator3.MoveNext())
                                                {
                                                    XElement element3 = enumerator3.Current;
                                                    int id = element3.GetAttributeValueOrDefaultOfInt32Type("numFmtId", -2147483648);
                                                    if (id >= 0)
                                                    {
                                                        string code = element3.GetAttributeValueOrDefaultOfStringType("formatCode", null);
                                                        dictionary.Add(id, new ExcelNumberFormat(id, code));
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (element2.Name.LocalName == "fonts")
                                        {
                                            using (IEnumerator<XElement> enumerator4 = element2.Elements().GetEnumerator())
                                            {
                                                while (enumerator4.MoveNext())
                                                {
                                                    XElement child = enumerator4.Current;
                                                    if (child.Name.LocalName == "font")
                                                    {
                                                        fonts.Add(XlsxReader.ReadFont(child));
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (element2.Name.LocalName == "fills")
                                        {
                                            using (IEnumerator<XElement> enumerator5 = element2.Elements().GetEnumerator())
                                            {
                                                while (enumerator5.MoveNext())
                                                {
                                                    XElement element5 = enumerator5.Current;
                                                    if (element5.Name.LocalName == "fill")
                                                    {
                                                        fills.Add(XlsxReader.ReadFillPattern(element5.Element(XlsxReader.XNames.patternFill)));
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (element2.Name.LocalName == "borders")
                                        {
                                            using (IEnumerator<XElement> enumerator6 = element2.Elements().GetEnumerator())
                                            {
                                                while (enumerator6.MoveNext())
                                                {
                                                    XElement element6 = enumerator6.Current;
                                                    if (element6.HasElements)
                                                    {
                                                        ExcelBorder border = new ExcelBorder {
                                                            Left = XlsxReader.GetBorder(element6, "left"),
                                                            Right = XlsxReader.GetBorder(element6, "right"),
                                                            Top = XlsxReader.GetBorder(element6, "top"),
                                                            Bottom = XlsxReader.GetBorder(element6, "bottom")
                                                        };
                                                        borders.Add(border);
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (element2.Name.LocalName == "cellStyleXfs")
                                        {
                                            using (IEnumerator<XElement> enumerator7 = element2.Elements().GetEnumerator())
                                            {
                                                while (enumerator7.MoveNext())
                                                {
                                                    XElement node = enumerator7.Current;
                                                    if (node.Name.LocalName == "xf")
                                                    {
                                                        list4.Add(XlsxReader.ReadXF(node, fonts, fills, borders, true));
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (element2.Name.LocalName == "cellStyles")
                                        {
                                            using (IEnumerator<XElement> enumerator8 = element2.Elements().GetEnumerator())
                                            {
                                                while (enumerator8.MoveNext())
                                                {
                                                    XElement element8 = enumerator8.Current;
                                                    string str2 = element8.GetAttributeValueOrDefaultOfStringType("name", null);
                                                    int num2 = element8.GetAttributeValueOrDefaultOfInt32Type("xfId", 0);
                                                    ExtendedFormat format = list4[num2];
                                                    format.IsStyleFormat = true;
                                                    BuiltInStyleIndex index = (BuiltInStyleIndex) element8.GetAttributeValueOrDefaultOfInt32Type("builtinId", 0);
                                                    ExcelStyle style = new ExcelStyle {
                                                        Name = str2,
                                                        Format = format.Clone()
                                                    };
                                                    if ((style.Format.NumberFormat == null) && dictionary.ContainsKey(style.Format.NumberFormatIndex))
                                                    {
                                                        style.Format.NumberFormat = dictionary[style.Format.NumberFormatIndex];
                                                        style.Format.NumberFormatIndex = 0;
                                                    }
                                                    style.IsCustomBuiltIn = false;
                                                    style.BuiltInStyle = index;
                                                    style.Category = style.GetBuiltInStyleCategory();
                                                    _builtInStyls.Add(style);
                                                }
                                                continue;
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}

