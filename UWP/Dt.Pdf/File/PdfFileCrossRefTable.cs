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
using System.Globalization;
#endregion

namespace Dt.Pdf.File
{
    /// <summary>
    /// Pdf file cross reference table
    /// </summary>
    public class PdfFileCrossRefTable : List<PdfFileCrossRef>
    {
        private static readonly byte[] nf = PdfASCIIEncoding.Instance.GetBytes("nf");
        private static readonly byte[] xref = PdfASCIIEncoding.Instance.GetBytes("xref");

        /// <summary>
        /// Determines whether [contains] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified obj]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(PdfObjectBase obj)
        {
            if (obj != null)
            {
                if (!obj.IsLabeled)
                {
                    return false;
                }
                foreach (PdfFileCrossRef ref2 in this)
                {
                    if (obj.Equals(ref2.Object))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Toes the PDF.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void ToPdf(PdfWriter writer)
        {
            this.ValidateEntries();
            base.Sort();
            PdfStreamWriter psw = writer.Psw;
            psw.WriteLineEnd();
            psw.WriteBytes(xref).WriteLineEnd();
            int index = 0;
            int count = 1;
            int num3 = 1;
            while (num3 < base.Count)
            {
                if ((base[index].ObjectIndex + count) != base[num3].ObjectIndex)
                {
                    this.WriteCrossBlock(index, count, psw);
                    index = num3;
                    count = 0;
                }
                num3++;
                count++;
            }
            this.WriteCrossBlock(index, count, psw);
        }

        /// <summary>
        /// Validates the entries.
        /// </summary>
        private void ValidateEntries()
        {
            foreach (PdfFileCrossRef ref2 in this)
            {
                if (ref2.ObjectIndex < 0)
                {
                    throw new PdfException("All references in cross table should be assign to a indirect object.");
                }
            }
        }

        /// <summary>
        /// Writes the cross block.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="psw">The PSW.</param>
        private void WriteCrossBlock(int index, int count, PdfStreamWriter psw)
        {
            psw.WriteInt(base[index].ObjectIndex).WriteSpace().WriteInt(count);
            for (int i = index; i < count; i++)
            {
                psw.WriteLineEnd();
                psw.WriteString(((long) base[i].Offset).ToString("0000000000", CultureInfo.InvariantCulture.NumberFormat));
                psw.WriteSpace();
                psw.WriteString(((int) base[i].Generation).ToString("00000", CultureInfo.InvariantCulture.NumberFormat));
                psw.WriteSpace();
                psw.WriteByte(base[i].InUse ? nf[0] : nf[1]);
            }
        }
    }
}

