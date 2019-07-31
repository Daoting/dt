#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
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
        readonly StreamRpc _call;
        readonly object _writeLock;
        Task _writeTask;

        public RequestWriter(StreamRpc p_call)
        {
            _call = p_call;
            _writeLock = new object();
        }

        public Task Write(object p_message)
        {
            if (p_message == null)
                throw new ArgumentNullException(nameof(p_message));

            lock (_writeLock)
            {
                // 请求流已关闭
                if (_call.RequestCompleted)
                {
                    Log.Error(_errClosed);
                    return Task.FromException(new InvalidOperationException(_errClosed));
                }

                // 上次写入未结束时禁止再写
                if (IsWriteInProgress)
                {
                    Log.Error(_errWriting);
                    return Task.FromException(new InvalidOperationException(_errWriting));
                }

                // 保存任务用来检查是否完成
                _writeTask = WriteCore(p_message);
            }
            return _writeTask;
        }


        public Task Complete()
        {
            lock (_writeLock)
            {
                // 写入未结束时禁止结束
                if (IsWriteInProgress)
                {
                    Log.Error(_errWriting);
                    return Task.FromException(new InvalidOperationException(_errWriting));
                }
                _call.FinishRequest();
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
                _call.CancellationToken.ThrowIfCancellationRequested();
                // 等到启动请求流，调用 PushStreamContent.SerializeToStreamAsync 时启动
                var writeStream = await _call.GetRequestStream().ConfigureAwait(false);
                // 写入完整Frame内容
                await writeStream.WriteAsync(RpcKit.GetObjData(p_message), _call.CancellationToken).ConfigureAwait(false);
                // 传输数据，清除本地缓存
                await writeStream.FlushAsync(_call.CancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                
            }
        }
    }
}
