#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
#endregion

namespace Dt.Xls.OOXml
{
    internal class MemoryFolder : IDisposable
    {
        private string _currentPath = "";
        private List<Tuple<IExcelImage, XFile>> _images;
        internal Dictionary<string, Stream> disk;
        internal int ImageCounter;

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the cloned memory folder.</returns>
        public MemoryFolder Clone()
        {
            MemoryFolder folder = new MemoryFolder {
                disk = new Dictionary<string, Stream>()
            };
            foreach (string str in this.disk.Keys)
            {
                folder.disk.Add(str, this.disk[str]);
            }
            folder._currentPath = this._currentPath;
            return folder;
        }

        /// <summary>
        /// Creates the memory file.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="stream">stream</param>
        /// <returns>successful or not</returns>
        public bool CreateMemoryFile(string fileName, Stream stream)
        {
            if (((fileName == null) || (fileName.Length == 0)) || (stream == null))
            {
                return false;
            }
            if (!stream.CanRead && !stream.CanWrite)
            {
                return false;
            }
            if (this.disk == null)
            {
                this.disk = new Dictionary<string, Stream>();
            }
            fileName = this.FixFileName(fileName);
            string str = fileName.ToLower();
            if (this.disk.ContainsKey(str))
            {
                this.disk[str] = stream;
            }
            else
            {
                this.disk.Add(str, stream);
            }
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Fixes the name of the file.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>file name</returns>
        private string FixFileName(string fileName)
        {
            if ((fileName != null) && (fileName.Length != 0))
            {
                fileName = fileName.Replace('/', '\\');
                while (fileName.IndexOf('\\') == 0)
                {
                    fileName = fileName.Substring(1);
                }
            }
            return fileName;
        }

        internal XFile GetExcelImageIdex(IExcelImage excelImage)
        {
            if ((excelImage == null) || (this.Images.Count == 0))
            {
                return null;
            }
            for (int i = 0; i < this.Images.Count; i++)
            {
                IExcelImage image = this.Images[i].Item1;
                if (((image.ImageType == excelImage.ImageType) && (image.SourceArray.Length == excelImage.SourceArray.Length)) && Enumerable.SequenceEqual<byte>(image.SourceArray, excelImage.SourceArray))
                {
                    return this.Images[i].Item2;
                }
            }
            string str = "jpg";
            switch (excelImage.ImageType)
            {
                case ImageType.JPG:
                    str = "jpg";
                    break;

                case ImageType.PNG:
                    str = "png";
                    break;

                case ImageType.Bitmap:
                    str = "bmp";
                    break;

                case ImageType.Gif:
                    str = "gif";
                    break;
            }
            string str2 = @"\xl";
            XFile file = new XFile(string.Concat((string[]) new string[] { str2, @"\media\image", ((int) this.ImageCounter).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat), ".", str }), "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image");
            string str3 = string.Format("image{0}.{1}", (object[]) new object[] { ((int) this.ImageCounter).ToString((IFormatProvider) CultureInfo.InvariantCulture), str });
            file.Target = "../media/" + str3;
            MemoryStream stream = new MemoryStream(excelImage.SourceArray);
            stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            this.CreateMemoryFile(file.FileName, (Stream) stream);
            this.Images.Add(new Tuple<IExcelImage, XFile>(excelImage, file));
            return file;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>stream about file</returns>
        public Stream GetFile(string fileName)
        {
            if (((fileName != null) && (fileName.Length != 0)) && (this.disk != null))
            {
                fileName = this.FixFileName(fileName);
                string str = fileName.ToLower();
                if (this.disk.ContainsKey(str))
                {
                    return this.disk[str];
                }
            }
            return null;
        }

        internal void RemoveFile(string fileName)
        {
            if (((fileName != null) && (fileName.Length != 0)) && (this.disk != null))
            {
                fileName = this.FixFileName(fileName);
                string str = fileName.ToLower();
                if (this.disk.ContainsKey(str))
                {
                    this.disk.Remove(str);
                }
            }
        }

        internal void Reset()
        {
            this.ImageCounter = 0;
            this.Images.Clear();
        }

        /// <summary>
        /// Gets or sets the current path.
        /// </summary>
        /// <value>The current path.</value>
        public string CurrentPath
        {
            get { return  this._currentPath; }
            set { this._currentPath = value; }
        }

        internal List<Tuple<IExcelImage, XFile>> Images
        {
            get
            {
                if (this._images == null)
                {
                    this._images = new List<Tuple<IExcelImage, XFile>>();
                }
                return this._images;
            }
        }
    }
}

