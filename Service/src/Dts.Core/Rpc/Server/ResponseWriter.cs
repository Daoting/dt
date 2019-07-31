#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 返回客户端的响应流
    /// </summary>
    public class ResponseWriter
    {
        LobContext _lc;

        internal ResponseWriter(LobContext p_lc)
        {
            _lc = p_lc;
        }

        public async Task Write(object p_message)
        {
            if (p_message == null)
                throw new ArgumentNullException(nameof(p_message));

            var response = _lc.Context.Response;
            if (!response.HasStarted)
            {
                response.ContentType = "text/plain";
                response.Headers["content-encoding"] = "gzip";
                await response.StartAsync();
            }

            // 写入数据，作为完整Frame内容
            await response.BodyWriter.WriteAsync(RpcKit.GetObjData(p_message));

            // FlushAsync两个作用：
            // 1. 传输数据，唤醒 PipeReader.ReadAsync 或 Stream.ReadAsync 方法继续读取
            // 2. 如果writer快过reader，如pipe中充满了没被reader清除的数据，会挂起writer等待清除后重新激活
            await response.BodyWriter.FlushAsync();
        }
    }
}
