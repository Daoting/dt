#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.File;
using Dt.Pdf.Object;
using Dt.Pdf.Text;
using Dt.Pdf.Utility;
using System;
using System.IO;
#endregion

namespace Dt.Pdf
{
    /// <summary>
    /// The Pdf writer
    /// </summary>
    public class PdfWriter
    {
        private readonly PdfFileCrossRefTable crossTable = new PdfFileCrossRefTable();
        private const string indirectR = "R";
        private int objectIndex;
        private static readonly byte[] objPrefix = PdfASCIIEncoding.Instance.GetBytes("obj");
        private readonly Sequence<PdfObjectBase> objSequence = new Sequence<PdfObjectBase>();
        private static readonly byte[] objSuffix = PdfASCIIEncoding.Instance.GetBytes("endobj");
        private static readonly byte[] PDF_ = PdfASCIIEncoding.Instance.GetBytes("PDF-");
        private readonly PdfStreamWriter psw;
        private bool refreshVersion;
        private PdfVersion version = PdfVersion.PDF1_0;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.PdfWriter" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfWriter(System.IO.Stream stream)
        {
            this.psw = new PdfStreamWriter(stream, PdfASCIIEncoding.Instance);
        }

        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void AddObject(PdfObjectBase obj)
        {
            if (!this.crossTable.Contains(obj) && !this.objSequence.Contains(obj))
            {
                obj.ObjectIndex = this.GetNextObjectIndex();
                this.objSequence.In(obj);
            }
        }

        /// <summary>
        /// Gets the index of the next object.
        /// </summary>
        /// <returns></returns>
        public int GetNextObjectIndex()
        {
            this.objectIndex++;
            return this.objectIndex;
        }

        /// <summary>
        /// Starts the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void Start(PdfDocumentCatalog obj)
        {
            this.objectIndex = 0;
            this.objSequence.Clear();
            this.crossTable.Clear();
            long offset = 0L;
            if (this.psw.Stream.CanSeek)
            {
                offset = this.psw.Stream.Position;
            }
            this.WritePdfHeader();
            obj.SetLabelAndFix(true, obj.IsFix);
            this.AddObject(obj);
            obj.Info.CreationDate = new PdfDate(DateTime.Now);
            obj.Info.ModDate = new PdfDate(DateTime.Now);
            this.AddObject(obj.Info);
            while (this.objSequence.Count > 0)
            {
                this.psw.WriteLineEnd();
                this.WriteLabeledObject(this.objSequence.Out());
            }
            PdfFileTrailer trailer = new PdfFileTrailer {
                Offset = (int) this.psw.Position,
                Root = obj,
                Size = this.GetNextObjectIndex(),
                Info = obj.Info
            };
            this.crossTable.Add(new PdfFileCrossRef(null, 0L, false));
            this.crossTable.ToPdf(this);
            trailer.ToPdf(this);
            if (this.refreshVersion)
            {
                this.psw.Stream.Seek(offset, SeekOrigin.Begin);
                this.WritePdfHeader();
                this.psw.Stream.Seek(0L, SeekOrigin.End);
            }
        }

        /// <summary>
        /// Writes the labeled object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void WriteLabeledObject(PdfObjectBase obj)
        {
            if (!this.crossTable.Contains(obj))
            {
                if (obj is IVersionDepend)
                {
                    PdfVersion version = ((IVersionDepend) obj).Version();
                    if (version > this.version)
                    {
                        this.version = version;
                        this.refreshVersion = true;
                    }
                }
                this.crossTable.Add(new PdfFileCrossRef(obj, this.psw.Position, true));
                this.psw.WriteString(string.Format("{0} {1} ", new object[] { (int) obj.ObjectIndex, (int) obj.Generation }));
                this.psw.WriteBytes(objPrefix);
                obj.ToPdf(this);
                this.psw.WriteBytes(objSuffix);
            }
        }

        /// <summary>
        /// Writes the labeled object ref.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="renderRef">if set to <c>true</c> [render ref].</param>
        protected void WriteLabeledObjectRef(PdfObjectBase obj, bool renderRef)
        {
            this.AddObject(obj);
            if (renderRef)
            {
                this.psw.WriteString(string.Format("{0} {1} {2}", new object[] { (int) obj.ObjectIndex, (int) obj.Generation, "R" }));
            }
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void WriteObject(PdfObjectBase obj)
        {
            this.WriteObject(obj, true);
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="renderRef">if set to <c>true</c> [render ref].</param>
        public void WriteObject(PdfObjectBase obj, bool renderRef)
        {
            if (obj == null)
            {
                obj = PdfNull.Null;
            }
            if (obj.IsLabeled)
            {
                this.WriteLabeledObjectRef(obj, renderRef);
            }
            else
            {
                obj.ToPdf(this);
            }
        }

        /// <summary>
        /// Writes the PDF header.
        /// </summary>
        private void WritePdfHeader()
        {
            this.psw.WriteByte(0x25);
            this.psw.WriteBytes(PDF_);
            this.psw.WriteInt((int)((int)this.version / (int)PdfVersion.PDF1_0));
            this.psw.WriteByte(0x2e);
            this.psw.WriteInt((int)((int)this.version % (int)PdfVersion.PDF1_0));
        }

        /// <summary>
        /// Gets the PSW.
        /// </summary>
        /// <value>The PSW.</value>
        internal PdfStreamWriter Psw
        {
            get { return  this.psw; }
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public System.IO.Stream Stream
        {
            get { return  this.psw.Stream; }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public PdfVersion Version
        {
            get { return  this.version; }
            set { this.version = value; }
        }
    }
}

