#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.IO.Compression;
#endregion

namespace Dt.Xls.OOXml
{
    internal class ZipHelper
    {
        public static CompressionLevel COMPRESS_LEVEL = ((CompressionLevel) CompressionLevel.Fastest);

        /// <summary>
        /// Closes the zip output stream.
        /// </summary>
        /// <param name="zipOutputStream">zip output stream</param>
        /// <returns>successful or not</returns>
        public static bool CloseZipOutputStream(ZipArchive zipOutputStream)
        {
            if (zipOutputStream == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Compresses the file.
        /// </summary>
        /// <param name="zipArhive">The zip arhive.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>successful or not</returns>
        internal static bool CompressFile(ZipArchive zipArhive, string fileName, Stream fileStream)
        {
            if (((zipArhive == null) || (fileName == null)) || ((fileName.Length == 0) || (fileStream == null)))
            {
                return false;
            }
            ZipArchiveEntry entry = zipArhive.CreateEntry(fileName, COMPRESS_LEVEL);
            entry.LastWriteTime = (DateTimeOffset) DateTime.Now;
            using (Stream stream = entry.Open())
            {
                fileStream.CopyTo(stream);
            }
            return true;
        }

        /// <summary>
        /// Compresses the files.
        /// </summary>
        /// <param name="mFolder">m folder</param>
        /// <param name="targetStream">target stream</param>
        /// <returns>successful or not</returns>
        public static bool CompressFiles(MemoryFolder mFolder, Stream targetStream)
        {
            if (((mFolder == null) || (mFolder.disk == null)) || ((targetStream == null) || !targetStream.CanWrite))
            {
                return false;
            }
            ZipArchive zipOutputStream = GetZipOutputStream(targetStream);
            if (zipOutputStream == null)
            {
                return false;
            }
            using (zipOutputStream)
            {
                foreach (string str in mFolder.disk.Keys)
                {
                    Stream file = mFolder.GetFile(str);
                    if (file != null)
                    {
                        CompressFile(zipOutputStream, str.Replace('\\', '/'), file);
                    }
                    else
                    {
                        zipOutputStream.CreateEntry(str.Replace('\\', '/'));
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Extracts the zip.
        /// </summary>
        /// <param name="zipStream">zip stream</param>
        /// <returns>Instance of MemoryFolder</returns>
        public static MemoryFolder ExtractZip(Stream zipStream)
        {
            if ((zipStream == null) || !zipStream.CanRead)
            {
                return null;
            }
            MemoryFolder folder = new MemoryFolder();
            ZipArchive archive = new ZipArchive(zipStream);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string fullName = entry.FullName;
                using (Stream stream = entry.Open())
                {
                    MemoryStream stream2 = new MemoryStream();
                    int count = 0x800;
                    byte[] buffer = new byte[count];
                    while (true)
                    {
                        count = stream.Read(buffer, 0, buffer.Length);
                        if (count <= 0)
                        {
                            break;
                        }
                        stream2.Write(buffer, 0, count);
                    }
                    stream2.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    folder.CreateMemoryFile(fullName, (Stream) stream2);
                }
            }
            return folder;
        }

        /// <summary>
        /// Gets the zip output stream.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>the instance of ZipOutputStream</returns>
        public static ZipArchive GetZipOutputStream(Stream stream)
        {
            if (stream == null)
            {
                return null;
            }
            return new ZipArchive(stream, ZipArchiveMode.Create, true, null);
        }
    }
}

