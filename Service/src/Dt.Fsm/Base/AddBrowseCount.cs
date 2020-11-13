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
    /// 增加浏览次数
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
            string path = p_context.Request.Path.Value;
            if (!path.EndsWith("-t.jpg"))
            {
                var str = Path.Combine(Cfg.Root, path.TrimStart('/'));
                if (File.Exists(str))
                    await new MySqlAccess().Exec("增加下载次数", new { path = path });
            }
            await _next(p_context);
        }
    }
}
