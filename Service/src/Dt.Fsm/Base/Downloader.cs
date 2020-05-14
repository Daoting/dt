#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Net;
using System.Threading.Tasks;
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 处理文件下载
    /// </summary>
    public class Downloader
    {
        HttpContext _context;

        public Downloader(HttpContext p_context)
        {
            _context = p_context;
        }

        public Task Handle()
        {
            // 截取路径
            string path = _context.Request.Path.Value.Substring(4);
            if (path.EndsWith("-t.jpg"))
                return DownloadThumbnail(path);
            return DownloadFile(path);
        }

        async Task DownloadFile(string p_path)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(Cfg.Root, p_path));
            if (!fileInfo.Exists)
            {
                _context.Response.Headers["error"] = WebUtility.UrlEncode("😢下载失败，文件不存在！");
                Log.Information("文件不存在：" + p_path);
                return;
            }

            Log.Information("下载：" + p_path);
            await new Db().Exec("增加下载次数", new { path = p_path });

            var response = _context.Response;
            response.Headers["Content-Type"] = "application/octet-stream";
            response.Headers["Content-Transfer-Encoding"] = "binary";
            response.Headers["Content-Length"] = fileInfo.Length.ToString();
            // 不以附件形式下载
            //response.Headers["Content-Disposition"] = "attachment;filename=" + path.Substring(path.LastIndexOf('/') + 1);

            try
            {
                await response.SendFileAsync(Path.Combine(Cfg.Root, p_path), _context.RequestAborted);
            }
            catch { }
        }

        async Task DownloadThumbnail(string p_path)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(Cfg.Root, p_path));
            if (!fileInfo.Exists)
            {
                // 未找到缩略图，取原图或视频
                int index = p_path.LastIndexOf('/');
                if (index > 0)
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(Path.Combine(Cfg.Root, p_path.Substring(0, index)));
                        if (di.Exists)
                        {
                            // 同目录下id相同的文件
                            string fileName = p_path.Substring(index + 1, p_path.Length - index - 7);
                            var files = di.GetFiles(fileName + ".*");
                            if (files != null && files.Length > 0)
                            {
                                // 下载原图或视频
                                await DownloadFile(p_path.Substring(0, index + 1) + files[0].Name);
                                return;
                            }
                        }
                    }
                    catch { }
                }

                _context.Response.Headers["error"] = WebUtility.UrlEncode("😢下载缩略图失败！");
                Log.Information("缩略图不存在：" + p_path);
                return;
            }

            // 不记录下载次数
            Log.Information("下载缩略图：" + p_path);

            var response = _context.Response;
            response.Headers["Content-Type"] = "application/octet-stream";
            response.Headers["Content-Transfer-Encoding"] = "binary";
            response.Headers["Content-Length"] = fileInfo.Length.ToString();
            try
            {
                await response.SendFileAsync(Path.Combine(Cfg.Root, p_path), _context.RequestAborted);
            }
            catch { }
        }
    }
}
