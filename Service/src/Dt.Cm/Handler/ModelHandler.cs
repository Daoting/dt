#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 处理模型文件下载请求
    /// </summary>
    internal static class ModelHandler
    {
        /// <summary>
        /// 获取缓存中的模型文件内容
        /// </summary>
        /// <param name="p_context"></param>
        /// <returns></returns>
        public static async Task GetFile(HttpContext p_context)
        {
            p_context.Response.ContentType = "text/xml";
            p_context.Response.Headers["content-encoding"] = "gzip";
            await p_context.Response.Body.WriteAsync(SqliteModel.ModelData, 0, SqliteModel.ModelData.Length);
        }
    }
}
