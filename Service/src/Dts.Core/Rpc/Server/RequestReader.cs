#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 读取客户端请求流
    /// </summary>
    public class RequestReader
    {
        LobContext _lc;
        string _originalVal;

        internal RequestReader(LobContext p_lc)
        {
            _lc = p_lc;
        }

        public async Task<bool> MoveNext()
        {
            var reader = _lc.Context.Request.BodyReader;
            byte[] data = null;
            while (true)
            {
                var completeMessage = false;
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;

                try
                {
                    if (result.IsCanceled)
                        return false;

                    if (!buffer.IsEmpty)
                    {
                        if (ParseData(ref buffer, out data))
                        {
                            completeMessage = true;
                            break;
                        }
                    }

                    if (result.IsCompleted)
                    {
                        if (buffer.Length == 0)
                            return false;

                    }
                }
                finally
                {
                    if (completeMessage)
                    {
                        // ParseData中已经切掉读取过的包，只需前移到buffer的起点
                        reader.AdvanceTo(buffer.Start);
                    }
                    else
                    {
                        // 当未读取完整的一帧时，标志已检查到buffer.End
                        reader.AdvanceTo(buffer.Start, buffer.End);
                    }
                }
            }
            if (data == null || data.Length == 0)
                return false;

            _originalVal = Encoding.UTF8.GetString(data);
            return true;
        }

        public T GetVal<T>()
        {
            throw new NotImplementedException();
        }

        public string GetOriginalVal()
        {
            return _originalVal;
        }

        bool ParseData(ref ReadOnlySequence<byte> p_buffer, out byte[] p_data)
        {
            // 1字节压缩标志 + 4字节内容长度
            bool compressed;
            int length;
            if (p_buffer.First.Length >= RpcKit.HeaderSize)
            {
                var headerData = p_buffer.First.Span.Slice(0, RpcKit.HeaderSize);
                compressed = headerData[0] == 1;
                length = (int)BinaryPrimitives.ReadUInt32BigEndian(headerData.Slice(1));
            }
            else
            {
                Span<byte> headerData = stackalloc byte[RpcKit.HeaderSize];
                p_buffer.Slice(0, RpcKit.HeaderSize).CopyTo(headerData);
                compressed = headerData[0] == 1;
                length = (int)BinaryPrimitives.ReadUInt32BigEndian(headerData.Slice(1));
            }

            // 长度不足
            if (p_buffer.Length < RpcKit.HeaderSize + length)
            {
                p_data = null;
                return false;
            }

            p_data = p_buffer.Slice(RpcKit.HeaderSize, length).ToArray();
            if (compressed)
            {
                // 先解压
                var ms = new MemoryStream();
                using (GZipStream zs = new GZipStream(new MemoryStream(p_data), CompressionMode.Decompress))
                {
                    zs.CopyTo(ms);
                }
                p_data = ms.ToArray();
            }

            // 切掉以上已读取的数据包
            p_buffer = p_buffer.Slice(RpcKit.HeaderSize + length);
            return true;
        }
    }
}
