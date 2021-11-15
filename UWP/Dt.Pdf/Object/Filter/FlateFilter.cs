#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Utility.zlib;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.Object.Filter
{
    /// <summary>
    /// Flate filter for Pdf stream
    /// </summary>
    public class FlateFilter : PdfFilter
    {
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
            ZDeflaterOutputStream stream = new ZDeflaterOutputStream(result);
            byte[] buffer = new byte[0x400];
            int count = 0;
            while ((count = rawData.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, count);
            }
            stream.Finish();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override PdfName GetName()
        {
            return PdfName.FlateDecode;
        }
    }
}

