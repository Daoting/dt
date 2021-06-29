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
using Dt.Core.Rpc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
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
        MySqlAccess _db;

        public Uploader(HttpContext p_context)
        {
            _context = p_context;
            _result = new List<string>();
        }

        /************************ Section 结构 ************************
        Content-Length: 60408
        Content-Type:multipart/form-data; boundary=ZnGpDtePMx0KrHh_G0X99Yef9r8JZsRJSXC

        --ZnGpDtePMx0KrHh_G0X99Yef9r8JZsRJSXC
        Content-Disposition: form-data; name="fixedvolume"

        photo
        --ZnGpDtePMx0KrHh_G0X99Yef9r8JZsRJSXC
        Content-Disposition: form-data;name="72 x 72 (.png)"; filename="photo.jpg"
        Content-Type: application/octet-stream
        Content-Transfer-Encoding: binary

        ... binary data of the jpg ...
        --ZnGpDtePMx0KrHh_G0X99Yef9r8JZsRJSXC
        Content-Disposition: form-data;name="thumbnail"; filename="thumbnail.jpg"
        Content-Type: application/octet-stream
        Content-Transfer-Encoding: binary

        ... binary data of the jpg ...
        --ZnGpDtePMx0KrHh_G0X99Yef9r8JZsRJSXC
        Content-Disposition: form-data;name="00:00:06 (480 x 288)"; filename="ImageStabilization.wmv"
        Content-Type: application/octet-stream
        Content-Transfer-Encoding: binary

        ... binary data ...
        --ZnGpDtePMx0KrHh_G0X99Yef9r8JZsRJSXC--
        ****************************/

        public async Task Handle()
        {
            // 读取boundary
            var boundary = _context.Request.GetMultipartBoundary();
            if (string.IsNullOrEmpty(boundary))
            {
                // 不支持的媒体类型
                _context.Response.StatusCode = 415;
                Log.Information("无Boundary，上传失败");
                return;
            }

            SortedSetCache cache = null;
            _db = new MySqlAccess(false);

            // 记录已成功接收的文件，以备后续异常时删除这些文件
            List<FileDesc> sucFiles = new List<FileDesc>();
            try
            {
                var reader = new MultipartReader(boundary, _context.Request.Body);
                var section = await reader.ReadNextSectionAsync(_context.RequestAborted);

                var fmSection = section.AsFormDataSection();
                if (fmSection != null && fmSection.Name == "fixedvolume")
                {
                    // 选择固定卷
                    _volume = (await fmSection.GetValueAsync()).ToLower();
                    if (!Cfg.FixedVolumes.Contains(_volume))
                    {
                        // 无此固定卷，状态码：未满足前提条件
                        _context.Response.StatusCode = 412;
                        Log.Information("不存在固定卷 {0}，上传失败！", _volume);
                        return;
                    }
                    section = await reader.ReadNextSectionAsync(_context.RequestAborted);
                }
                else
                {
                    // 选择正在使用数最低的卷
                    cache = new SortedSetCache(Cfg.VolumeKey);
                    _volume = await cache.GetMin();
                    // 该卷使用数加1
                    await cache.Increment(_volume);
                }

                // Section顺序：固定路径(可无)，文件，文件，缩略图，文件
                // 缩略图由客户端生成，因提取视频帧耗资源，属于前面的文件
                FileDesc lastFile = null;
                while (section != null)
                {
                    var fileSection = section.AsFileSection();
                    if (fileSection != null)
                    {
                        if (fileSection.Name == "thumbnail")
                        {
                            if (lastFile != null)
                            {
                                // 将缩略图复制到同路径
                                // 命名规则：原文件名添加后缀"-t.jpg"，如 143203944767549440.wmv-t.jpg
                                string fullPath = Path.Combine(Cfg.Root, lastFile.Path + Cfg.ThumbPostfix);
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
                            lastFile = await ReceiveFile(fileSection);
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
                        await _db.Exec($"delete from fsm_file where id in ({sb.ToString().TrimStart(',')})");
                    }
                    catch { }
                }
            }
            finally
            {
                await _db.Close(true);
                // 卷使用数减1
                if (cache != null)
                    await cache.Decrement(_volume);
            }

            await Response();
        }

        /// <summary>
        /// 接收文件、保存记录
        /// </summary>
        /// <param name="p_section"></param>
        /// <returns></returns>
        async Task<FileDesc> ReceiveFile(FileMultipartSection p_section)
        {
            FileDesc desc = new FileDesc();
            desc.ID = Kit.NewID;
            desc.Name = p_section.FileName;
            if (long.TryParse(_context.Request.Headers["uid"], out var id))
                desc.Uploader = id;
            desc.Info = p_section.Name;

            // 扩展名
            int pt = desc.Name.LastIndexOf('.');
            string ext = pt > -1 ? desc.Name.Substring(pt).ToLower() : "";

            // 根据文件名获取两级目录
            string dir = GetDir(desc.Name);
            desc.Path = Path.Combine(_volume, dir, desc.ID + ext).Replace('\\', '/');
            EnsurePathExist(dir);
            string fullPath = Path.Combine(Cfg.Root, _volume, dir, desc.ID + ext);
            _result.Add(desc.Path);

            try
            {
                using (var writeStream = File.Create(fullPath))
                {
                    // FileStream 类型为 MultipartReaderStream
                    await p_section.FileStream.CopyToAsync(writeStream, _bufferSize, _context.RequestAborted);
                    desc.Size = writeStream.Length;
                }
                await _db.Exec("上传文件", desc);
            }
            catch
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
                throw;
            }
            return desc;
        }

        async Task Response()
        {
            if (_result.Count == 0)
                return;

            _context.Response.BodyWriter.Write(RpcKit.GetObjectBytes(_result));
            await _context.Response.BodyWriter.FlushAsync();
            Log.Information("接收 {0} 个文件", _result.Count);
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
        /// <summary>
        /// 文件标识
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 存放路径：卷/两级目录/id.ext
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 文件长度
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 上传人id
        /// </summary>
        public long Uploader { get; set; }

        /// <summary>
        /// 文件描述
        /// </summary>
        public string Info { get; set; }
    }
}
