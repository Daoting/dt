#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.IO;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the document attachment for the PDF document.
    /// </summary>
    public class DocumentAttachment
    {
        bool compress = true;
        string contentType;
        string description;
        string fileName;
        MemoryStream fileStream;

        /// <summary>
        /// Creates a new document attachment for the specified file.
        /// </summary>
        /// <param name="name">Name of the Attachment.</param>
        public DocumentAttachment(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="T:Dt.Cells.Data.DocumentAttachment" /> is compressed.
        /// </summary>
        /// <value><c>true</c> if compressed; otherwise, <c>false</c>. The default value is <c>true</c>.</value>
        [DefaultValue(true)]
        public bool Compress
        {
            get { return  this.compress; }
            set { this.compress = value; }
        }

        /// <summary>
        /// Gets or sets the content type.
        /// </summary>
        /// <value>The type of the content. The default value is an empty string.</value>
        [DefaultValue("")]
        public string ContentType
        {
            get { return  this.contentType; }
            set { this.contentType = value; }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Description
        {
            get { return  this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Gets or sets the file stream.
        /// </summary>
        /// <value>The file stream. The default value is null.</value>
        [DefaultValue((string) null)]
        public Stream FileStream
        {
            get { return  (Stream) this.fileStream; }
            set
            {
                if ((value != null) && value.CanRead)
                {
                    byte[] buffer = new byte[value.Length];
                    value.Read(buffer, 0, buffer.Length);
                    if (this.fileStream != null)
                    {
                        ((Stream) this.fileStream).Close();
                    }
                    this.fileStream = new MemoryStream();
                    this.fileStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    this.fileStream = null;
                }
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets the file stream inner.
        /// </summary>
        /// <value>The file stream inner</value>
        internal MemoryStream FileStreamInner
        {
            get { return  this.fileStream; }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file. The default value is an empty string.</value>
        [DefaultValue("")]
        public string Name
        {
            get { return  this.fileName; }
            set
            {
                this.fileName = value;
                if (!string.IsNullOrEmpty(this.fileName) && string.IsNullOrEmpty(this.contentType))
                {
                    this.contentType = this.fileName.Substring(this.fileName.LastIndexOf('.') + 1);
                }
            }
        }
    }
}

