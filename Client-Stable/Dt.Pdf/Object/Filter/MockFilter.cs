#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.Object.Filter
{
    /// <summary>
    /// Mock filter. This filter didn't convert any byte.
    /// </summary>
    public class MockFilter : PdfFilter
    {
        public static PdfFilter CCITTFaxFilter = new MockFilter(PdfName.CCITTFaxDecode);
        public static PdfFilter DCTFilter = new MockFilter(PdfName.DCTDecode);
        new public static PdfFilter FlateFilter = new MockFilter(PdfName.FlateDecode);
        public static PdfFilter JPXFilter = new MockFilter(PdfName.JPXDecode);
        private PdfName name;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Filter.MockFilter" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MockFilter(PdfName name)
        {
            if (name == null)
            {
                throw new PdfArgumentNullException("name");
            }
            this.name = name;
        }

        /// <summary>
        /// Decodes the inner.
        /// </summary>
        /// <param name="compressedData">The compressed data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        protected override void DecodeInner(Stream compressedData, Stream result, Dictionary<PdfName, PdfObjectBase> options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encodes the inner.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        protected override void EncodeInner(Stream rawData, Stream result, Dictionary<PdfName, PdfObjectBase> options)
        {
            int num;
            byte[] buffer = new byte[0x400];
            while ((num = rawData.Read(buffer, 0, buffer.Length)) > 0)
            {
                result.Write(buffer, 0, num);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override PdfName GetName()
        {
            return this.name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public PdfName Name
        {
            get { return  this.name; }
            set { this.name = value; }
        }
    }
}

