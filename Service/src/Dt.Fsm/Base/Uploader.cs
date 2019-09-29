#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 处理文件上传
    /// </summary>
    public class Uploader
    {
        // 10k
        const int _bufferSize = 10240;
        readonly HttpContext _context;
        readonly List<string> _result;
        string _volume;

        public Uploader(HttpContext p_context)
        {
            _context = p_context;
            _result = new List<string>();
        }

        public async Task Handle()
        {
            // 读取boundary
            var boundary = _context.Request.GetMultipartBoundary();
            if (string.IsNullOrEmpty(boundary))
            {
                // 不支持的媒体类型
                _context.Response.StatusCode = 415;
                return;
            }

            Db db = new Db(false);
            List<FileDesc> sucFiles = new List<FileDesc>();
            SortedSetCache cache = new SortedSetCache(Cfg.VolumeKey);
            try
            {
                // 选择使用率最低的卷
                _volume = await cache.GetMin();
                // 增加使用次数
                await cache.Increment(_volume);

                // Section顺序：文件，文件，缩略图，文件
                // 缩略图由客户端生成，因提取视频帧耗资源，属于前面的文件
                FileDesc lastFile = null;
                var reader = new MultipartReader(boundary, _context.Request.Body);
                var section = await reader.ReadNextSectionAsync(_context.RequestAborted);
                while (section != null)
                {
                    var fileSection = section.AsFileSection();
                    if (fileSection != null)
                    {
                        if (fileSection.Name == "thumbnail")
                        {
                            // 将缩略图复制到同路径，命名：xxx-t.jpg
                            int index;
                            if (lastFile != null && (index = lastFile.Path.LastIndexOf('.')) > 0)
                            {
                                string fullPath = Path.Combine(Cfg.Root, lastFile.Path.Substring(0, index) + Cfg.ThumbPostfix);
                                try
                                {
                                    using (var writeStream = File.Create(fullPath))
                                    {
                                        await fileSection.FileStream.CopyToAsync(writeStream, _bufferSize, _context.RequestAborted);
                                    }
                                }
                                catch { }
                                lastFile = null;
                            }
                        }
                        else
                        {
                            lastFile = await ReceiveFile(fileSection, db);
                            sucFiles.Add(lastFile);
                        }
                    }
                    section = await reader.ReadNextSectionAsync(_context.RequestAborted);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "处理上传文件异常");
                _result.Clear();

                if (sucFiles.Count > 0)
                {
                    // 删除已上传成功的文件和记录
                    StringBuilder sb = new StringBuilder();
                    foreach (var desc in sucFiles)
                    {
                        FileInfo fi = new FileInfo(Path.Combine(Cfg.Root, desc.Path));
                        if (fi.Exists)
                        {
                            try
                            {
                                fi.Delete();
                            }
                            catch { }
                        }
                        sb.Append(',');
                        sb.Append(desc.ID);
                    }

                    try
                    {
                        await db.Exec($"delete from fsm_file where id in ({sb.ToString().TrimStart(',')})");
                    }
                    catch { }
                }
            }
            finally
            {
                await db.Close(true);
                // 移除使用标志
                await cache.Decrement(_volume);
            }

            await Response();
        }

        /// <summary>
        /// 接收文件、保存记录
        /// </summary>
        /// <param name="p_section"></param>
        /// <param name="p_db"></param>
        /// <returns></returns>
        async Task<FileDesc> ReceiveFile(FileMultipartSection p_section, Db p_db)
        {
            FileDesc desc = new FileDesc();
            desc.ID = Id.New();
            desc.Name = p_section.FileName;
            if (long.TryParse(_context.Request.Headers["uid"], out var id))
                desc.Uploader = id;
            desc.UserType = 3;
            desc.Info = p_section.Name;

            // 根据文件名获取两级目录
            string dir = GetDir(desc.Name);
            int pt = desc.Name.LastIndexOf('.');
            string ext = pt > -1 ? desc.Name.Substring(pt) : "";
            desc.Path = Path.Combine(_volume, dir, desc.ID + ext).Replace('\\', '/');
            _result.Add(desc.Path);
            EnsurePathExist(dir);

            string fullPath = Path.Combine(Cfg.Root, _volume, dir, desc.ID + ext);
            try
            {
                using (var writeStream = File.Create(fullPath))
                {
                    // FileStream 类型为 MultipartReaderStream
                    await p_section.FileStream.CopyToAsync(writeStream, _bufferSize, _context.RequestAborted);
                    desc.Size = writeStream.Length;
                }
                await p_db.Exec("上传文件", desc);
            }
            catch (Exception ex)
            {
                FileInfo fi = new FileInfo(fullPath);
                if (fi.Exists)
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch { }
                }
                throw ex;
            }
            return desc;
        }

        async Task Response()
        {
            if (_result.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();
            using (var sr = new StringWriter(sb))
            using (var writer = new JsonTextWriter(sr))
            {
                JsonRpcSerializer.Serialize(_result, writer);
                writer.Flush();
            }
            var data = Encoding.UTF8.GetBytes(sb.ToString());
            _context.Response.BodyWriter.Write(data);
            await _context.Response.BodyWriter.FlushAsync();
        }

        /// <summary>
        /// 根据文件名获取两级目录
        /// </summary>
        /// <param name="p_fileName"></param>
        /// <returns></returns>
        string GetDir(string p_fileName)
        {
            uint hash = 0;
            // BKDRHash算法
            foreach (var ch in p_fileName)
            {
                // 种子131
                hash = hash * 131 + ch;
            }
            return Path.Combine((hash & 0xFF).ToString("X2"), ((hash & 0xFF00) >> 8).ToString("X2"));
        }

        /// <summary>
        /// 确保路径存在
        /// </summary>
        /// <param name="p_dir"></param>
        void EnsurePathExist(string p_dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(Cfg.Root, _volume, p_dir));
            if (!dirInfo.Exists)
            {
                try
                {
                    // 可能并发创建
                    dirInfo.Create();
                }
                catch { }
            }
        }
    }

    public class FileDesc
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public long Uploader { get; set; }

        public int UserType { get; set; }

        public string Info { get; set; }
    }
}
