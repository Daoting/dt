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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 基于Http2的流模式远程调用
    /// </summary>
    public class StreamRpc : UnaryRpc, IDisposable
    {
        #region 成员变量
        readonly CancellationTokenSource _callCts;
        TaskCompletionSource<Stream> _writeStreamTcs;
        TaskCompletionSource<bool> _writeCompleteTcs;
        RequestWriter _requestWriter;
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        public StreamRpc(string p_serviceName, string p_methodName, params object[] p_params)
            : base(p_serviceName, p_methodName, p_params)
        {
            _callCts = new CancellationTokenSource();
        }

        /// <summary>
        /// 可取消令牌
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return _callCts.Token; }
        }

        /// <summary>
        /// 请求流是否已关闭
        /// </summary>
        public bool RequestCompleted
        {
            get
            {
                if (_writeCompleteTcs != null)
                    return _writeCompleteTcs.Task.IsCompleted || _writeCompleteTcs.Task.IsCanceled;
                return false;
            }
        }

        public bool ResponseFinished { get; private set; }

        public Task SendTask { get; private set; }

        public HttpResponseMessage HttpResponse { get; private set; }

        public bool Disposed { get; private set; }

        /// <summary>
        /// 启动Http2协议的远程调用，客户端发送一个请求，服务端返回数据流响应
        /// </summary>
        /// <returns></returns>
        public ResponseReader StartServerStream()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new PushStreamContent((stream) => stream.WriteAsync(_data, 0, _data.Length))
            };
            _ = StartAsync(request);
            return new ResponseReader(this);
        }

        /// <summary>
        /// 启动Http2协议的远程调用，客户端发送请求数据流，服务端返回一个响应
        /// </summary>
        /// <returns></returns>
        public RequestWriter StartClientStream()
        {
            var request = new HttpRequestMessage { Method = HttpMethod.Post };
            _requestWriter = CreateWriter(request);
            _ = StartAsync(request);
            return _requestWriter;
        }

        /// <summary>
        /// 启动Http2协议的远程调用，客户端发送请求数据流，服务端返回数据流响应
        /// </summary>
        /// <returns></returns>
        public DuplexStream StartDuplexStream()
        {
            var request = new HttpRequestMessage { Method = HttpMethod.Post };
            _requestWriter = CreateWriter(request);
            _ = StartAsync(request);
            return new DuplexStream(_requestWriter, new ResponseReader(this));
        }

        /// <summary>
        /// 获取请求流
        /// </summary>
        /// <returns></returns>
        internal Task<Stream> GetRequestStream()
        {
            return _writeStreamTcs?.Task;
        }

        /// <summary>
        /// 请求流发送结束
        /// </summary>
        internal void FinishRequest()
        {
            _writeCompleteTcs?.TrySetResult(true);
        }

        internal void FinishResponse()
        {
            ResponseFinished = true;
            Dispose();
        }

        async Task StartAsync(HttpRequestMessage p_request)
        {
            var task = _client.SendAsync(p_request, _callCts.Token);
            SendTask = task;

            try
            {
                HttpResponse = await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
        }


        RequestWriter CreateWriter(HttpRequestMessage p_request)
        {
            _writeStreamTcs = new TaskCompletionSource<Stream>(TaskCreationOptions.RunContinuationsAsynchronously);
            _writeCompleteTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            p_request.Content = new PushStreamContent(
                (stream) =>
                {
                    // 发送准备就绪
                    _writeStreamTcs.TrySetResult(stream);
                    // 控制发送任务不结束，未结束前一直可发送
                    return _writeCompleteTcs.Task;
                });
            return new RequestWriter(this);
        }


        public void Dispose()
        {
            if (Disposed)
                return;

            lock (this)
            {
                Disposed = true;

                if (!ResponseFinished)
                {
                    // If the response is not finished then cancel any pending actions:
                    // 1. Call HttpClient.SendAsync
                    // 2. Response Stream.ReadAsync
                    // 3. Client stream
                    //    - Getting the Stream from the Request.HttpContent
                    //    - Holding the Request.HttpContent.SerializeToStream open
                    //    - Writing to the client stream
                    CancelCall();
                }
                else
                {
                    _writeStreamTcs?.TrySetCanceled();
                    _writeCompleteTcs?.TrySetCanceled();
                }

                HttpResponse?.Dispose();
                // To avoid racing with Dispose, skip disposing the call CTS
                // This avoid Dispose potentially calling cancel on a disposed CTS
                // The call CTS is not exposed externally and all dependent registrations
                // are cleaned up
            }
        }

        void CancelCall()
        {
            // Checking if cancellation has already happened isn't threadsafe
            // but there is no adverse effect other than an extra log message
            if (!_callCts.IsCancellationRequested)
            {
                _callCts.Cancel();

                // Canceling call will cancel pending writes to the stream
                _writeCompleteTcs?.TrySetCanceled();
                _writeStreamTcs?.TrySetCanceled();
            }
        }
    }
}
