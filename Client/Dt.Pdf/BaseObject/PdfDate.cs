#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using System;
using System.Globalization;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf Date type
    /// </summary>
    public class PdfDate : PdfString
    {
        private DateTime date;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfDate" /> class.
        /// </summary>
        public PdfDate()
        {
            this.date = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfDate" /> class.
        /// </summary>
        /// <param name="date">The date.</param>
        public PdfDate(DateTime date)
        {
            this.date = DateTime.Now;
            this.date = date;
        }

        /// <summary>
        /// Toes the PDF.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void ToPdf(PdfWriter writer)
        {
            string str = "D:" + this.date.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            TimeSpan baseUtcOffset = TimeZoneInfo.Local.BaseUtcOffset;
            if ((baseUtcOffset.Hours == 0) && (baseUtcOffset.Minutes == 0))
            {
                str = str + "Z";
            }
            else if ((baseUtcOffset.Hours < 0) || (baseUtcOffset.Minutes < 0))
            {
                string str2 = str;
                str = string.Concat((string[]) new string[] { str2, "-", ((int) Math.Abs(baseUtcOffset.Hours)).ToString("00"), "'", ((int) Math.Abs(baseUtcOffset.Minutes)).ToString("00"), "'" });
            }
            else
            {
                string str3 = str;
                str = string.Concat((string[]) new string[] { str3, "+", ((int) baseUtcOffset.Hours).ToString("00"), "'", ((int) baseUtcOffset.Minutes).ToString("00"), "'" });
            }
            writer.Psw.WriteByte(40);
            writer.Psw.WriteString(str);
            writer.Psw.WriteByte(0x29);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date
        {
            get { return  this.date; }
            set { this.date = value; }
        }
    }
}

