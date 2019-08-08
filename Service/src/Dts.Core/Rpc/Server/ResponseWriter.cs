#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Buffers;
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

        /// <summary>
        /// 向客户端写入一帧响应流
        /// </summary>
        /// <param name="p_message">支持序列化的对象</param>
        /// <returns></returns>
        public Task Write(object p_message)
        {
            if (p_message == null)
                throw new ArgumentNullException(nameof(p_message));

            var writer = _lc.Context.Response.BodyWriter;
            // 写入完整Frame内容：1字节压缩标志 + 4字节内容长度 + 内容
            writer.Write(RpcKit.GetObjData(p_message));

            // 查看源码了解到，FlushAsync不支持多次awaiter，当上次调用未结束时再次调用不会附加到上次任务之后执行！！！
            // FlushAsync两个作用：
            // 1. 传输数据，唤醒 PipeReader.ReadAsync 或 Stream.ReadAsync 方法继续读取
            // 2. 如果writer快过reader，如pipe中充满了没被reader清除的数据，会挂起writer等待清除后重新激活
            var flushTask = writer.FlushAsync();
            if (flushTask.IsCompletedSuccessfully)
            {
                flushTask.GetAwaiter().GetResult();
                return Task.CompletedTask;
            }
            return flushTask.AsTask();
        }
    }
}
