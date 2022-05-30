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
    /// Pdf filter
    /// </summary>
    public abstract class PdfFilter
    {
        private static readonly Dictionary<PdfName, PdfFilter> filterCache = new Dictionary<PdfName, PdfFilter>();

        protected PdfFilter()
        {
        }

        /// <summary>
        /// Decodes the specified compressed data.
        /// </summary>
        /// <param name="compressedData">The compressed data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        public void Decode(Stream compressedData, Stream result, Dictionary<PdfName, PdfObjectBase> options)
        {
            if ((compressedData == null) || !compressedData.CanRead)
            {
                throw new PdfArgumentException("compressedData");
            }
            if ((result == null) || !result.CanWrite)
            {
                throw new PdfArgumentException("result");
            }
            this.DecodeInner(compressedData, result, options);
        }

        /// <summary>
        /// Decodes the inner.
        /// </summary>
        /// <param name="compressedData">The compressed data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        protected abstract void DecodeInner(Stream compressedData, Stream result, Dictionary<PdfName, PdfObjectBase> options);
        /// <summary>
        /// Encodes the specified raw data.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        public void Encode(Stream rawData, Stream result, Dictionary<PdfName, PdfObjectBase> options)
        {
            if ((rawData == null) || !rawData.CanRead)
            {
                throw new PdfArgumentException("rawData");
            }
            if ((result == null) || !result.CanWrite)
            {
                throw new PdfArgumentException("result");
            }
            this.EncodeInner(rawData, result, options);
        }

        /// <summary>
        /// Encodes the inner.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        protected abstract void EncodeInner(Stream rawData, Stream result, Dictionary<PdfName, PdfObjectBase> options);
        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static PdfFilter GetFilter(PdfName name)
        {
            PdfFilter filter;
            if (name == null)
            {
                throw new PdfArgumentNullException("name");
            }
            if (filterCache.ContainsKey(name))
            {
                return filterCache[name];
            }
            if (name.Equals(PdfName.ASCIIHexDecode))
            {
                filter = new Filter.ASCIIHexFilter();
            }
            else if (name.Equals(PdfName.ASCII85Decode))
            {
                filter = new Filter.ASCII85Filter();
            }
            else if (name.Equals(PdfName.LZWDecode))
            {
                filter = new Filter.LZWFilter();
            }
            else
            {
                if (!name.Equals(PdfName.FlateDecode))
                {
                    throw new PdfNotSupportedFilterException();
                }
                filter = new Filter.FlateFilter();
            }
            filterCache.Add(name, filter);
            return filter;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public abstract PdfName GetName();

        /// <summary>
        /// Gets the ASCII85 filter.
        /// </summary>
        /// <value>The ASCII85 filter.</value>
        public static PdfFilter ASCII85Filter
        {
            get { return  GetFilter(PdfName.ASCII85Decode); }
        }

        /// <summary>
        /// Gets the ASCIIHex filter.
        /// </summary>
        /// <value>The ASCIIHex filter.</value>
        public static PdfFilter ASCIIHexFilter
        {
            get { return  GetFilter(PdfName.ASCIIHexDecode); }
        }

        /// <summary>
        /// Gets the Flate filter.
        /// </summary>
        /// <value>The Flate filter.</value>
        public static PdfFilter FlateFilter
        {
            get { return  GetFilter(PdfName.FlateDecode); }
        }

        /// <summary>
        /// Gets the LZW filter.
        /// </summary>
        /// <value>The LZW filter.</value>
        public static PdfFilter LZWFilter
        {
            get { return  GetFilter(PdfName.LZWDecode); }
        }
    }
}

