#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.Exceptions;
using System;
using System.Globalization;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type Number.
    /// </summary>
    public class PdfNumber : PdfObjectBase
    {
        /// <summary>
        /// PdfNumber 4
        /// </summary>
        public static readonly PdfNumber Four = new PdfNumber(4.0);
        /// <summary>
        /// PdfNumber 1
        /// </summary>
        public static readonly PdfNumber One = new PdfNumber(1.0);
        private double rawValue;
        /// <summary>
        /// PdfNumber 3
        /// </summary>
        public static readonly PdfNumber Three = new PdfNumber(3.0);
        /// <summary>
        /// PdfNumber 2
        /// </summary>
        public static readonly PdfNumber Two = new PdfNumber(2.0);
        /// <summary>
        /// PdfNumber 0
        /// </summary>
        public static readonly PdfNumber Zero = new PdfNumber(0.0);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfNumber" /> class.
        /// </summary>
        public PdfNumber()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfNumber" /> class.
        /// </summary>
        /// <param name="number">The number.</param>
        public PdfNumber(PdfNumber number)
        {
            if (number == null)
            {
                throw new PdfArgumentNullException("number");
            }
            this.rawValue = number.rawValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfNumber" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfNumber(double value)
        {
            this.rawValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfNumber" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfNumber(string value)
        {
            double num = double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            this.rawValue = num;
        }

        /// <summary>
        /// Get bytes of object
        /// </summary>
        /// <returns></returns>
        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read data from Pdf reader
        /// </summary>
        /// <param name="reader">Pdf Reader</param>
        public override void ToObject(IPdfReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            writer.Psw.WriteDouble(this.rawValue);
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get { return  this.rawValue; }
            set { this.rawValue = value; }
        }

        /// <summary>
        /// Gets or sets the value int.
        /// </summary>
        /// <value>The value int.</value>
        public int ValueInt
        {
            get { return  (int) this.rawValue; }
            set { this.rawValue = value; }
        }
    }
}

