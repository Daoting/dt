#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// A CIDFont program contains glyph descriptions that are accessed using a CID as the character selector.
    /// </summary>
    public class PdfCIDFont : PdfFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCIDFont" /> class.
        /// </summary>
        /// <param name="fontType">Type of the font.</param>
        /// <param name="baseFont">The base font.</param>
        public PdfCIDFont(PdfName fontType, BaseFont baseFont) : base(fontType, baseFont)
        {
            if (baseFont == null)
            {
                throw new PdfArgumentNullException("baseFont");
            }
            switch (baseFont.Encoding)
            {
                case "Identity-H":
                case "Identity-V":
                    base[PdfName.CIDSystemInfo] = CIDSystemInfo.Identity;
                    break;

                case "UniGB-UCS2-H":
                    base[PdfName.CIDSystemInfo] = CIDSystemInfo.GB1;
                    break;

                case "UniCNS-UCS2-H":
                    base[PdfName.CIDSystemInfo] = CIDSystemInfo.CNS1;
                    break;

                case "UniJIS-UCS2-H":
                    base[PdfName.CIDSystemInfo] = CIDSystemInfo.Japan1;
                    break;

                case "UniKS-UCS2-H":
                    base[PdfName.CIDSystemInfo] = CIDSystemInfo.Korea1;
                    break;

                case "WinAnsiEncoding":
                    throw new PdfArgumentException("wrong encoding type");

                default:
                    throw new PdfNotSupportedEncodingException(baseFont.Encoding + " not supported.");
            }
            base.baseFont = baseFont;
            this.InitPdfFontDescriptor();
            base.isLabeled = true;
        }

        /// <summary>
        /// Inits the PDF font descriptor.
        /// </summary>
        private void InitPdfFontDescriptor()
        {
            base[PdfName.FontDescriptor] = new PdfFontDescriptor(base.baseFont, base.FontType);
        }

        /// <summary>
        /// Called when [base font changing].
        /// </summary>
        /// <param name="bf">The bf.</param>
        /// <returns></returns>
        protected override bool OnBaseFontChanging(BaseFont bf)
        {
            this.InitPdfFontDescriptor();
            return base.OnBaseFontChanging(bf);
        }

        /// <summary>
        /// Sets the widths.
        /// </summary>
        /// <param name="chars">The chars.</param>
        public void SetWidths(Dictionary<char, int> chars)
        {
            if ((chars != null) && (chars.Count > 0))
            {
                bool isAdobeCMap = base.baseFont.IsAdobeCMap;
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                List<int> list = new List<int>();
                foreach (KeyValuePair<char, int> pair in chars)
                {
                    int item = isAdobeCMap ? AdobeCMap.GetCode(pair.Key, base.baseFont.Encoding) : pair.Value;
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                        if (isAdobeCMap)
                        {
                            dictionary[item] = pair.Value;
                        }
                    }
                }
                list.Sort();
                float defaultWidth = base.baseFont.GetDefaultWidth();
                PdfArray array = new PdfArray();
                int index = list[0];
                List<float> list2 = new List<float> {
                    base.BaseFont.GetWidth(index)
                };
                bool flag2 = false;
                for (int i = 1; i <= list.Count; i++)
                {
                    int num5 = 0;
                    float width = 0f;
                    if (i < list.Count)
                    {
                        num5 = list[i];
                        width = base.BaseFont.GetWidth(isAdobeCMap ? dictionary[num5] : num5);
                        if (num5 == (index + list2.Count))
                        {
                            if ((width == list2[list2.Count - 1]) == flag2)
                            {
                                list2.Add(width);
                                continue;
                            }
                            if (list2.Count <= 1)
                            {
                                flag2 = !flag2;
                                list2.Add(width);
                                continue;
                            }
                        }
                    }
                    if (flag2)
                    {
                        array.Add(new PdfNumber((double) index));
                        array.Add(new PdfNumber((double) ((index + list2.Count) - 1)));
                        array.Add(new PdfNumber((double) list2[0]));
                    }
                    else if ((list2.Count != 1) || (list2[0] != defaultWidth))
                    {
                        array.Add(new PdfNumber((double) index));
                        array.Add(PdfArray.Convert(list2.ToArray()));
                    }
                    index = num5;
                    list2.Clear();
                    list2.Add(width);
                    flag2 = false;
                }
                base[PdfName.W] = array;
            }
        }

        /// <summary>
        /// Toes the PDF.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void ToPdf(PdfWriter writer)
        {
            base[PdfName.DW] = new PdfNumber((double) base.baseFont.GetDefaultWidth());
            base.ToPdf(writer);
        }
    }
}

