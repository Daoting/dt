#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.Object.Filter
{
    /// <summary>
    /// ASCII85 filter for Pdf stream
    /// </summary>
    public class ASCII85Filter : PdfFilter
    {
        private const byte ascii33 = 0x21;

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
            byte[] buffer = new byte[4];
            while (((num = rawData.Read(buffer, 0, buffer.Length)) != -1) && (num > 0))
            {
                if (num < buffer.Length)
                {
                    for (int i = num; i < 4; i++)
                    {
                        buffer[i] = buffer[num - 1];
                    }
                }
                long num3 = (((buffer[0] << 0x18) | (buffer[1] << 0x10)) | (buffer[2] << 8)) | buffer[3];
                if (num3 == 0L)
                {
                    result.WriteByte(0x7a);
                }
                else
                {
                    for (long j = 0x31c84b1L; j > 0L; j /= 0x55L)
                    {
                        long num5 = num3 / j;
                        result.WriteByte((byte) (num5 + 0x21L));
                        num3 -= num5 * j;
                    }
                }
            }
            result.WriteByte(0x7e);
            result.WriteByte(0x3e);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override PdfName GetName()
        {
            return PdfName.ASCII85Decode;
        }
    }
}

