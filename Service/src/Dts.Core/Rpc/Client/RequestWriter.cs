#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 向服务器的写入流
    /// </summary>
    public class RequestWriter
    {
        const string _errClosed = "请求流已关闭，消息内容无法写入！";
        const string _errWriting = "上次写入未结束时禁止再写入或关闭！";
        readonly ClientStreamRpc _rpc;
        readonly object _writeLock;
        Task _writeTask;

        public RequestWriter(ClientStreamRpc p_rpc)
        {
            _rpc = p_rpc;
            _writeLock = new object();
        }

        /// <summary>
        /// 向服务端写入一帧
        /// </summary>
        /// <param name="p_message">支持序列化的对象</param>
        /// <returns></returns>
        public Task Write(object p_message)
        {
            if (p_message == null)
                throw new ArgumentNullException(nameof(p_message));

            lock (_writeLock)
            {
                // 请求流已关闭
                if (_rpc.RequestStream == null || _rpc.RequestCompleted)
                    return Task.FromException(new InvalidOperationException(_errClosed));

                // 上次写入未结束时禁止再写
                if (IsWriteInProgress)
                    return Task.FromException(new InvalidOperationException(_errWriting));

                // 保存任务用来检查是否完成
                _writeTask = WriteCore(p_message);
            }
            return _writeTask;
        }

        /// <summary>
        /// 请求流结束
        /// </summary>
        /// <returns></returns>
        public Task Complete()
        {
            lock (_writeLock)
            {
                // 写入未结束时禁止结束
                if (IsWriteInProgress)
                    return Task.FromException(new InvalidOperationException(_errWriting));

                _rpc.FinishRequest();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 上次异步写入是否结束
        /// </summary>
        bool IsWriteInProgress
        {
            get { return _writeTask != null && !_writeTask.IsCompleted; }
        }

        async Task WriteCore(object p_message)
        {
            try
            {
                // 写入完整Frame内容
                await _rpc.RequestStream.WriteAsync(RpcKit.GetObjData(p_message)).ConfigureAwait(false);
                // 传输数据，清除本地缓存
                await _rpc.RequestStream.FlushAsync().ConfigureAwait(false);
            }
            catch
            { }
        }
    }
}
