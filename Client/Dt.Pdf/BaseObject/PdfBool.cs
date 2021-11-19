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
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type Boolean
    /// </summary>
    public class PdfBool : PdfObjectBase
    {
        public static readonly PdfBool FALSE = new PdfBool(false);
        public const string FALSESTRING = "false";
        private bool rawValue;
        public static readonly PdfBool TRUE = new PdfBool(true);
        public const string TRUESTRING = "true";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfBool" /> class.
        /// </summary>
        public PdfBool()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfBool" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfBool(PdfBool value) : this(value.rawValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfBool" /> class.
        /// </summary>
        /// <param name="value">The Value</param>
        public PdfBool(bool value)
        {
            this.RawValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfBool" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfBool(string value)
        {
            this.RawValue = !string.IsNullOrEmpty(value) && value.ToLower().Equals("true");
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj is PdfBool)
            {
                return (this.RawValue == ((PdfBool) obj).RawValue);
            }
            return base.Equals(obj);
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
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
            writer.Psw.WriteString(this.rawValue ? "true" : "false");
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            if (!this.RawValue)
            {
                return "false";
            }
            return "true";
        }

        /// <summary>
        /// Gets or sets a value.
        /// </summary>
        public bool RawValue
        {
            get { return  this.rawValue; }
            set { this.rawValue = value; }
        }
    }
}

