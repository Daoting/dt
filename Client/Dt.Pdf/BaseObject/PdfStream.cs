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
using Dt.Pdf.Object.Filter;
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type Stream
    /// </summary>
    public class PdfStream : PdfObjectBase
    {
        private readonly MemoryStream data;
        private Queue<PdfFilter> filters;
        private readonly PdfDictionary properties;
        private PdfStreamWriter psw;
        private static readonly byte[] streamPrefix = PdfASCIIEncoding.Instance.GetBytes("stream");
        private static readonly byte[] streamSuffix = PdfASCIIEncoding.Instance.GetBytes("endstream");

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfStream" /> class.
        /// </summary>
        public PdfStream()
        {
            this.data = new MemoryStream();
            PdfDictionary dictionary = new PdfDictionary {
                IsLabeled = false,
                IsFix = true
            };
            this.properties = dictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfStream" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfStream(PdfStream stream)
        {
            this.data = new MemoryStream();
            PdfDictionary dictionary = new PdfDictionary {
                IsLabeled = false,
                IsFix = true
            };
            this.properties = dictionary;
            if (stream == null)
            {
                throw new PdfArgumentNullException("stream");
            }
            stream.data.WriteTo(this.data);
            foreach (KeyValuePair<PdfName, PdfObjectBase> pair in stream.properties)
            {
                this.properties.Add(pair);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfStream" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public PdfStream(PdfFilter filter)
        {
            this.data = new MemoryStream();
            PdfDictionary dictionary = new PdfDictionary {
                IsLabeled = false,
                IsFix = true
            };
            this.properties = dictionary;
            if (filter == null)
            {
                throw new PdfArgumentNullException("filter");
            }
            this.Filters.Enqueue(filter);
        }

        /// <summary>
        /// Get bytes of object
        /// </summary>
        /// <returns></returns>
        public override byte[] GetBytes()
        {
            return this.data.ToArray();
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
            MemoryStream data = this.data;
            if ((this.filters != null) && (this.filters.Count > 0))
            {
                data = new MemoryStream();
                this.data.WriteTo(data);
                PdfArray array = new PdfArray();
                while (this.filters.Count > 0)
                {
                    using (MemoryStream stream2 = new MemoryStream())
                    {
                        data.Seek(0L, SeekOrigin.Begin);
                        data.WriteTo(stream2);
                        stream2.Seek(0L, SeekOrigin.Begin);
                        data.Dispose();
                        data.Dispose();
                        data = new MemoryStream();
                        PdfFilter filter = this.filters.Dequeue();
                        filter.Encode(stream2, data, null);
                        array.Insert(0, filter.GetName());
                        continue;
                    }
                }
                if (array.Count == 1)
                {
                    this.properties.Add(PdfName.Filter, array[0]);
                }
                else
                {
                    this.properties.Add(PdfName.Filter, array);
                }
            }
            this.properties[PdfName.Length] = new PdfNumber((double) data.Length);
            PdfStreamWriter psw = writer.Psw;
            this.properties.ToPdf(writer);
            psw.WriteLineEnd();
            psw.WriteBytes(streamPrefix).WriteLineEnd();
            data.WriteTo(psw.Stream);
            psw.WriteLineEnd().WriteBytes(streamSuffix);
            psw.WriteLineEnd();
            if (data != this.data)
            {
                data.Dispose();
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public MemoryStream Data
        {
            get { return  this.data; }
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public Queue<PdfFilter> Filters
        {
            get
            {
                if (this.filters == null)
                {
                    this.filters = new Queue<PdfFilter>();
                }
                return this.filters;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is labeled.
        /// About Label reference Pdf Ref v1.7-3.2.9
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is labeled; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLabeled
        {
            get { return  true; }
            set { throw new PdfObjectInternalException(PdfObjectInternalException.PdfObjectExceptionType.ChangeFixedObjectLabel); }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public long Length
        {
            get { return  this.data.Length; }
        }

        /// <summary>
        /// Gets the properties dictionary of PdfStream.
        /// </summary>
        /// <value>The properties.</value>
        public PdfDictionary Properties
        {
            get { return  this.properties; }
        }

        /// <summary>
        /// Gets the PSW.
        /// </summary>
        /// <value>The PSW.</value>
        internal PdfStreamWriter Psw
        {
            get
            {
                if (this.psw == null)
                {
                    this.psw = new PdfStreamWriter(this.data, PdfASCIIEncoding.Instance);
                }
                return this.psw;
            }
        }
    }
}

