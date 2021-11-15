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
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type String
    /// </summary>
    public class PdfString : PdfObjectBase
    {
        protected byte[] byteValue;
        public static PdfString Empty = new PdfString(string.Empty);
        private string encoding;
        private bool isHexMode;
        protected const byte prefix = 40;
        private const byte prefixHex = 60;
        protected const byte suffix = 0x29;
        private const byte suffixHex = 0x3e;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfString" /> class.
        /// </summary>
        public PdfString()
        {
            this.encoding = "us-ascii";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfString" /> class.
        /// </summary>
        /// <param name="pdfString">The PDF string.</param>
        public PdfString(PdfString pdfString)
        {
            this.encoding = "us-ascii";
            if (pdfString == null)
            {
                throw new PdfArgumentNullException("pdfString");
            }
            this.byteValue = new byte[pdfString.byteValue.Length];
            Array.Copy(pdfString.byteValue, this.byteValue, pdfString.byteValue.Length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfString" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfString(string value)
        {
            this.encoding = "us-ascii";
            this.byteValue = (value != null) ? this.PdfEncoding.GetBytes(value) : ((byte[]) new byte[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfString" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfString(byte[] value)
        {
            this.encoding = "us-ascii";
            if (value == null)
            {
                throw new PdfArgumentNullException("value");
            }
            this.byteValue = new byte[value.Length];
            Array.Copy(value, this.byteValue, value.Length);
            this.IsHexMode = true;
        }

        /// <summary>
        /// Escapes the string.
        /// </summary>
        /// <param name="buf">The buf.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        internal static byte[] EscapeString(byte[] buf, PdfEncodingBase encoding)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < buf.Length; i++)
            {
                byte item = buf[i];
                switch (item)
                {
                    case 8:
                    {
                        list.AddRange(encoding.GetBytes(@"\b"));
                        continue;
                    }
                    case 9:
                    {
                        list.AddRange(encoding.GetBytes(@"\t"));
                        continue;
                    }
                    case 10:
                    {
                        list.AddRange(encoding.GetBytes(@"\n"));
                        continue;
                    }
                    case 12:
                    {
                        list.AddRange(encoding.GetBytes(@"\f"));
                        continue;
                    }
                    case 13:
                    {
                        list.AddRange(encoding.GetBytes(@"\r"));
                        continue;
                    }
                    case 40:
                    case 0x29:
                    case 0x5c:
                    {
                        list.AddRange(encoding.GetBytes(@"\"));
                        list.Add(item);
                        continue;
                    }
                }
                list.Add(item);
            }
            return list.ToArray();
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
        /// Gets the escaped bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] GetEscapedBytes()
        {
            return EscapeString(this.byteValue, this.PdfEncoding);
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
            this.WriteTo(writer.Psw);
        }

        /// <summary>
        /// Writes to.
        /// </summary>
        /// <param name="psw">The PSW.</param>
        internal void WriteTo(PdfStreamWriter psw)
        {
            if (this.isHexMode)
            {
                psw.WriteByte(60);
                byte[] byteValue = this.byteValue;
                for (int i = 0; i < byteValue.Length; i++)
                {
                    psw.WriteString(((byte) byteValue[i]).ToString("X2"));
                }
                psw.WriteByte(0x3e);
            }
            else
            {
                psw.WriteByte(40);
                psw.WriteBytes(this.GetEscapedBytes());
                psw.WriteByte(0x29);
            }
        }

        /// <summary>
        /// Gets or sets the bytes.
        /// </summary>
        /// <value>The bytes.</value>
        public byte[] Bytes
        {
            get { return  this.byteValue; }
            set { this.byteValue = value ?? new byte[0]; }
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public string Encoding
        {
            get { return  this.encoding; }
            set { this.encoding = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hex mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hex mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsHexMode
        {
            get { return  this.isHexMode; }
            set { this.isHexMode = value; }
        }

        /// <summary>
        /// Gets the PDF encoding.
        /// </summary>
        /// <value>The PDF encoding.</value>
        internal PdfEncodingBase PdfEncoding
        {
            get { return  (PdfEncodingBase.GetEncoding(this.encoding) as PdfEncodingBase); }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return  this.PdfEncoding.GetString(this.byteValue, 0, this.byteValue.Length); }
        }
    }
}

