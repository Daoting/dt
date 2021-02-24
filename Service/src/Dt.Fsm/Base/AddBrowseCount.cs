#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-05-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 增加浏览次数，无缩略图时自动取原图
    /// </summary>
    public class AddBrowseCount
    {
        readonly RequestDelegate _next;

        public AddBrowseCount(RequestDelegate p_next)
        {
            _next = p_next ?? throw new ArgumentNullException(nameof(p_next));
        }

        public async Task Invoke(HttpContext p_context)
        {
            string path = p_context.Request.Path.Value.TrimStart('/');
            FileInfo fileInfo = new FileInfo(Path.Combine(Cfg.Root, path));

            // 缩略图
            bool isThumb = false;
            if (path.EndsWith(Cfg.ThumbPostfix))
            {
                if (fileInfo.Exists)
                {
                    isThumb = true;
                }
                else
                {
                    // 未找到缩略图，取原图，视频不处理
                    string originPath = path.Substring(0, path.Length - Cfg.ThumbPostfix.Length);
                    int index = originPath.LastIndexOf('.');
                    if (index > -1)
                    {
                        string ext = originPath.Substring(index + 1).ToLower();
                        if (ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "bmp" || ext == "gif" || ext == "tif")
                        {
                            // 取原图
                            path = originPath;
                            p_context.Request.Path = new PathString("/" + path);
                            fileInfo = new FileInfo(Path.Combine(Cfg.Root, path));
                        }
                    }
                }
            }

            if (!fileInfo.Exists)
            {
                p_context.Response.StatusCode = 404;
                return;
            }

            if (!isThumb)
                await new MySqlAccess().Exec("增加下载次数", new { path = path });
            await _next(p_context);
        }
    }
}
