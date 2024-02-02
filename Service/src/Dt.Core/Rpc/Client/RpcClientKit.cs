#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Rpc客户端工具方法
    /// </summary>
    public static class RpcClientKit
    {
        /// <summary>
        /// 将对象打包成一帧写入流
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_message"></param>
        /// <returns></returns>
        public static Task WriteFrame(Stream p_stream, object p_message)
        {
            bool compress = RpcKit.SerializeObj(p_message, out var content);
            return WriteFrame(p_stream, content, compress);
        }

        /// <summary>
        /// 封装Frame写入流
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_data"></param>
        /// <param name="p_compress"></param>
        /// <returns></returns>
        public static async Task WriteFrame(Stream p_stream, byte[] p_data, bool p_compress)
        {
            // Frame头：1字节压缩标志 + 4字节内容长度
            byte[] header = new byte[RpcKit.HeaderSize];
            header[0] = p_compress ? (byte)1 : (byte)0;

            // uno的ios版BinaryPrimitives程序集冲突，提了不改，操蛋
#if IOS
            BitConverter.GetBytes((uint)p_data.Length).Take(4).Reverse().ToArray().CopyTo(header, 1);
#else
            BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(1), (uint)p_data.Length);
#endif
            await p_stream.WriteAsync(header, 0, header.Length).ConfigureAwait(false);

            // Frame内容
            await p_stream.WriteAsync(p_data, 0, p_data.Length).ConfigureAwait(false);
            // 传输数据，清除本地缓存
            await p_stream.FlushAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 从流中读取一帧，返回的数据不包括Frame头，已解压，自动过滤心跳帧
        /// </summary>
        /// <param name="p_stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadFrame(Stream p_stream)
        {
            byte[] data;
            while (true)
            {
                int received = 0;
                int read;

                // 包头
                // 1字节压缩标志 + 4字节内容长度
                byte[] header = new byte[RpcKit.HeaderSize];
                while ((read = await p_stream.ReadAsync(header, received, header.Length - received).ConfigureAwait(false)) > 0)
                {
                    received += read;
                    if (received == header.Length)
                        break;
                }

                if (received < header.Length)
                    throw new InvalidDataException("Frame头错误");

                // 读取内容，uno操蛋
#if IOS
                byte[] arrLength = new byte[RpcKit.HeaderSize - 1];
                Array.Copy(header, 1, arrLength, 0, arrLength.Length);
                uint length = BitConverter.ToUInt32(arrLength.Reverse().ToArray(), 0);
#else
                var length = BinaryPrimitives.ReadUInt32BigEndian(header.AsSpan(1));
#endif
                if (length > int.MaxValue)
                    throw new InvalidDataException("消息超长");
                if (length > 0)
                {
                    received = 0;
                    data = new byte[length];
                    while ((read = await p_stream.ReadAsync(data, received, data.Length - received).ConfigureAwait(false)) > 0)
                    {
                        received += read;
                        if (received == data.Length)
                            break;
                    }

                    // 过滤心跳帧
                    if (length == RpcKit.ShakeHands.Length && data[0] == RpcKit.ShakeHands[0])
                        continue;
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
                break;
            }
            return data;
        }
    }
}
