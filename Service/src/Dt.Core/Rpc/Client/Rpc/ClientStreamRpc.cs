#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 客户端发送请求数据流的远程调用
    /// </summary>
    public class ClientStreamRpc : BaseRpc
    {
        TaskCompletionSource<Stream> _writeStreamTcs;
        TaskCompletionSource<bool> _writeCompleteTcs;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        public ClientStreamRpc(string p_serviceName, string p_methodName, params object[] p_params)
            : base(p_serviceName, p_methodName, p_params)
        { }

        /// <summary>
        /// 启动Http2协议的远程调用，客户端发送请求数据流
        /// </summary>
        /// <returns></returns>
        public async Task<RequestWriter> Call()
        {
            var request = CreateRequestMessage();
            var writer = CreateWriter(request);
            _ = _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
            RequestStream = await _writeStreamTcs.Task;
            return writer;
        }

        /// <summary>
        /// 请求流
        /// </summary>
        internal Stream RequestStream { get; private set; }

        /// <summary>
        /// 请求流是否已关闭
        /// </summary>
        internal bool RequestCompleted
        {
            get
            {
                if (_writeCompleteTcs != null)
                    return _writeCompleteTcs.Task.IsCompleted || _writeCompleteTcs.Task.IsCanceled;
                return false;
            }
        }

        /// <summary>
        /// 请求流发送结束
        /// </summary>
        internal void FinishRequest()
        {
            _writeCompleteTcs?.TrySetResult(true);
        }

        protected RequestWriter CreateWriter(HttpRequestMessage p_request)
        {
            _writeStreamTcs = new TaskCompletionSource<Stream>(TaskCreationOptions.RunContinuationsAsynchronously);
            _writeCompleteTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            p_request.Content = new PushStreamContent(async (stream) =>
            {
                // 不同平台stream类型不同，造成uwp、android、ios客户端调用stream.FlushAsync()时并未发送！
                // asp.net core：System.Net.Http.Http2Connection.Http2Stream.Http2WriteStream
                // uwp：System.Net.Http.HttpContent.LimitMemoryStream
                // mono：System.Net.Http.HttpContent.FixedMemoryStream

                // 先发送调用帧
                await RpcClientKit.WriteFrame(stream, _data, _isCompressed).ConfigureAwait(false);
                // 允许自定义写入
                _writeStreamTcs.TrySetResult(stream);
                // 控制发送任务不结束，未结束前一直可发送
                await _writeCompleteTcs.Task;
            });
            return new RequestWriter(this);
        }
    }
}
