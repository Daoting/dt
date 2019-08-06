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
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
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
        readonly StreamRpc _call;
        HttpResponseMessage _httpResponse;
        Stream _responseStream;
        string _originalVal;

        public ResponseReader(StreamRpc p_call)
        {
            _call = p_call;
        }

        public async Task<bool> MoveNext()
        {
            // HTTP响应已结束
            if (_call.ResponseFinished)
                return false;

            try
            {
                _call.CancellationToken.ThrowIfCancellationRequested();
                if (_httpResponse == null)
                {
                    // 等待请求发送完毕
                    await _call.SendTask.ConfigureAwait(false);
                    _httpResponse = _call.HttpResponse;
                }
                if (_responseStream == null)
                    _responseStream = await _httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);

                int received = 0;
                int read;

                // 包头
                // 1字节压缩标志 + 4字节内容长度
                byte[] header = new byte[RpcKit.HeaderSize];
                while ((read = await _responseStream.ReadAsync(header, received, header.Length - received, _call.CancellationToken).ConfigureAwait(false)) > 0)
                {
                    received += read;
                    if (received == header.Length)
                        break;
                }

                if (received < header.Length)
                {
                    if (received == 0)
                    {
                        // 结束
                        _originalVal = null;
                        return false;
                    }
                    throw new InvalidDataException("数据包头错误");
                }

                // 读取内容
                byte[] data;
                var length = BinaryPrimitives.ReadUInt32BigEndian(header.AsSpan(1));
                if (length > int.MaxValue)
                    throw new InvalidDataException("消息超长");
                if (length > 0)
                {
                    received = 0;
                    data = new byte[length];
                    while ((read = await _responseStream.ReadAsync(data, received, data.Length - received, _call.CancellationToken).ConfigureAwait(false)) > 0)
                    {
                        received += read;
                        if (received == data.Length)
                            break;
                    }
                }
                else
                {
                    data = Array.Empty<byte>();
                }

                if (header[0] == 1)
                {
                    // 先解压
                    var ms = new MemoryStream();
                    using (GZipStream zs = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
                    {
                        zs.CopyTo(ms);
                    }
                    data = ms.ToArray();
                }
                _originalVal = Encoding.UTF8.GetString(data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T GetVal<T>()
        {
            throw new NotImplementedException();
        }

        public string GetOriginalVal()
        {
            return _originalVal;
        }
    }
}
