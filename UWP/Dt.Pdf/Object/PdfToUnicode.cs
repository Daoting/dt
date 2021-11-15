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
using Dt.Pdf.Object.Filter;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.Object
{
    public class PdfToUnicode : PdfStream
    {
        private readonly Dictionary<char, int> usedChars;

        public PdfToUnicode(Dictionary<char, int> usedChars)
        {
            if (usedChars == null)
            {
                throw new PdfArgumentNullException("usedChars");
            }
            this.usedChars = usedChars;
            base.Filters.Enqueue(PdfFilter.FlateFilter);
        }

        public override void ToPdf(PdfWriter writer)
        {
            if (this.usedChars.Count > 0)
            {
                PdfStreamWriter psw = base.Psw;
                psw.WriteString("/CIDInit /ProcSet findresource begin 12 dict begin begincmap /CMapType 2 def").WriteLineEnd();
                psw.WriteString("1 begincodespacerange <0000> <FFFF> endcodespacerange").WriteLineEnd();
                psw.WriteString(((int) this.usedChars.Count) + " beginbfchar").WriteLineEnd();
                foreach (KeyValuePair<char, int> pair in this.usedChars)
                {
                    psw.WriteString(string.Format("<{0:x4}><{1:x4}>", new object[] { (int) pair.Value, (int) pair.Key })).WriteLineEnd();
                }
                psw.WriteString("endbfchar").WriteLineEnd().WriteString("endcmap CMapName currentdict /CMap defineresource pop end end").WriteLineEnd();
            }
            base.ToPdf(writer);
        }
    }
}

