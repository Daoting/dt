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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 读取服务器的返回流
    /// </summary>
    public class ResponseReader
    {
        const string _errReading = "上次读取未结束，禁止继续读取！";
        readonly StreamRpc _call;
        readonly object _moveNextLock;
        Task<bool> _moveNextTask;
        HttpResponseMessage _httpResponse;
        Stream _responseStream;
        string _originalVal;

        public ResponseReader(StreamRpc p_call)
        {
            _call = p_call;
            _moveNextLock = new object();
        }

        public Task<bool> MoveNext()
        {
            // HTTP响应已结束
            if (_call.ResponseFinished)
                return Task.FromResult(false);

            lock (_moveNextLock)
            {
                // 上次读取未完成
                if (IsMoveNextInProgress)
                {
                    Log.Error(_errReading);
                    return Task.FromException<bool>(new InvalidOperationException(_errReading));
                }

                // 保存任务用来检查是否完成
                _moveNextTask = MoveNextCore();
            }
            return _moveNextTask;
        }

        public T GetVal<T>()
        {
            throw new NotImplementedException();
        }

        public string GetOriginalVal()
        {
            return _originalVal;
        }

        async Task<bool> MoveNextCore()
        {
            try
            {
                _call.CancellationToken.ThrowIfCancellationRequested();
                if (_httpResponse == null)
                {
                    // 等待发送完毕
                    await _call.SendTask.ConfigureAwait(false);
                    _httpResponse = _call.HttpResponse;
                }
                if (_responseStream == null)
                    _responseStream = await _httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

                // 无内容时表示响应已结束
                if (_responseStream.Length == 0)
                {
                    _originalVal = null;
                    _call.FinishResponse();
                    return false;
                }

                int received = 0;
                int read;
                byte[] data = new byte[_responseStream.Length];
                while ((read = await _responseStream.ReadAsync(data, received, data.Length - received, _call.CancellationToken).ConfigureAwait(false)) > 0)
                {
                    received += read;
                    if (received == data.Length)
                        break;
                }
                _originalVal = Encoding.UTF8.GetString(data, 0, data.Length);
                return true;
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 上次异步读取是否结束
        /// </summary>
        bool IsMoveNextInProgress
        {
            get { return _moveNextTask != null && !_moveNextTask.IsCompleted; }
        }
    }
}
