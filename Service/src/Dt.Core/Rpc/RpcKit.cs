#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Rpc工具方法
    /// </summary>
    public static class RpcKit
    {
        /// <summary>
        /// 数据包头长度，1字节压缩标志 + 4字节内容长度
        /// </summary>
        public const int HeaderSize = 5;

        /// <summary>
        /// 进行内容压缩的最小长度
        /// </summary>
        public const int MinCompressLength = 1024;

        /// <summary>
        /// 心跳内容：***
        /// </summary>
        public static byte[] ShakeHands = new byte[] { 42, 42, 42 };

        /// <summary>
        /// 将对象打包成一帧写入管道
        /// </summary>
        /// <param name="p_writer"></param>
        /// <param name="p_message"></param>
        /// <returns></returns>
        public static Task WriteFrame(PipeWriter p_writer, object p_message)
        {
            bool compress = SerializeObj(p_message, out var content);
            return WriteFrame(p_writer, content, compress);
        }

        /// <summary>
        /// 向管道写入心跳帧
        /// </summary>
        /// <param name="p_writer"></param>
        /// <returns></returns>
        public static Task WriteHeartbeat(PipeWriter p_writer)
        {
            return WriteFrame(p_writer, ShakeHands, false);
        }

        /// <summary>
        /// 封装Frame写入管道
        /// </summary>
        /// <param name="p_writer"></param>
        /// <param name="p_data"></param>
        /// <param name="p_compress"></param>
        /// <returns></returns>
        public static Task WriteFrame(PipeWriter p_writer, byte[] p_data, bool p_compress)
        {
            // Frame头：1字节压缩标志 + 4字节内容长度
            var header = p_writer.GetSpan(HeaderSize);
            header[0] = p_compress ? (byte)1 : (byte)0;
            BinaryPrimitives.WriteUInt32BigEndian(header.Slice(1), (uint)p_data.Length);
            p_writer.Advance(HeaderSize);

            // Frame内容
            p_writer.Write(p_data);

            // 查看源码了解到，FlushAsync不支持多次awaiter，当上次调用未结束时再次调用不会附加到上次任务之后执行！！！
            // FlushAsync两个作用：
            // 1. 传输数据，唤醒 PipeReader.ReadAsync 或 Stream.ReadAsync 方法继续读取
            // 2. 如果writer快过reader，如pipe中充满了没被reader清除的数据，会挂起writer等待清除后重新激活
            var flushTask = p_writer.FlushAsync();
            if (flushTask.IsCompletedSuccessfully)
            {
                flushTask.GetAwaiter().GetResult();
                return Task.CompletedTask;
            }
            return flushTask.AsTask();
        }

        /// <summary>
        /// 将对象打包成一帧写入流
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_message"></param>
        /// <returns></returns>
        public static Task WriteFrame(Stream p_stream, object p_message)
        {
            bool compress = SerializeObj(p_message, out var content);
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
            byte[] header = new byte[HeaderSize];
            header[0] = p_compress ? (byte)1 : (byte)0;
            BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(1), (uint)p_data.Length);
            await p_stream.WriteAsync(header, 0, header.Length).ConfigureAwait(false);

            // Frame内容
            await p_stream.WriteAsync(p_data).ConfigureAwait(false);
            // 传输数据，清除本地缓存
            await p_stream.FlushAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 从管道中读取一帧，返回的数据不包括Frame头，已自动解压
        /// </summary>
        /// <param name="p_reader"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadFrame(PipeReader p_reader)
        {
            while (true)
            {
                var completeMessage = false;
                var result = await p_reader.ReadAsync();
                var buffer = result.Buffer;

                try
                {
                    if (result.IsCanceled)
                        return Array.Empty<byte>();

                    if (!buffer.IsEmpty)
                    {
                        if (ParseFrame(ref buffer, out var data))
                        {
                            // 读取完整一帧
                            completeMessage = true;
                            return data;
                        }
                    }

                    // 不会再有数据写入
                    if (result.IsCompleted)
                        return Array.Empty<byte>();
                }
                finally
                {
                    if (completeMessage)
                    {
                        // ParseFrame中已经切掉读取过的包，只需前移到buffer的起点
                        p_reader.AdvanceTo(buffer.Start);
                    }
                    else
                    {
                        // 当未读取完整的一帧时，标志已检查到buffer.End
                        p_reader.AdvanceTo(buffer.Start, buffer.End);
                    }
                }
            }
        }

        /// <summary>
        /// 从流中读取一帧，返回的数据不包括Frame头，已自动解压
        /// </summary>
        /// <param name="p_stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadFrame(Stream p_stream)
        {
            int received = 0;
            int read;

            // 包头
            // 1字节压缩标志 + 4字节内容长度
            byte[] header = new byte[HeaderSize];
            while ((read = await p_stream.ReadAsync(header, received, header.Length - received).ConfigureAwait(false)) > 0)
            {
                received += read;
                if (received == header.Length)
                    break;
            }

            if (received < header.Length)
                throw new InvalidDataException("Frame头错误");

            // 读取内容
            byte[] data;
            var length = BinaryPrimitives.ReadUInt32BigEndian(header.AsSpan(1));
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
            return data;
        }

        /// <summary>
        /// 从流中读取心跳帧
        /// </summary>
        /// <param name="p_stream"></param>
        /// <returns></returns>
        public static async Task ReadHeartbeat(Stream p_stream)
        {
            byte[] data = await ReadFrame(p_stream);
            if (data.Length != ShakeHands.Length)
                throw new Exception("心跳信息错误！");
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_val"></param>
        /// <returns></returns>
        public static T GetVal<T>(object p_val)
        {
            T val = default;
            if (p_val == null)
            {
                // 空值
            }
            else if (typeof(T) == p_val.GetType())
            {
                // 结果对象与给定对象类型相同时
                val = (T)p_val;
            }
            else
            {
                // 特殊处理结果对象与给定对象类型不相同时
                try
                {
                    val = (T)Convert.ChangeType(p_val, typeof(T));
                }
                catch (Exception convExp)
                {
                    throw new Exception($"类型转换时异常：\r\n {p_val.GetType()}-->{typeof(T)}：{convExp.Message}");
                }
            }
            return val;
        }

        /// <summary>
        /// 读取Frame的内容，不包括Frame头，自动解压
        /// </summary>
        /// <param name="p_buffer"></param>
        /// <param name="p_data"></param>
        /// <returns></returns>
        static bool ParseFrame(ref ReadOnlySequence<byte> p_buffer, out byte[] p_data)
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

        /// <summary>
        /// 对象序列化，超过长度限制时执行压缩
        /// </summary>
        /// <param name="p_message"></param>
        /// <param name="p_data"></param>
        /// <returns>true 已压缩</returns>
        static bool SerializeObj(object p_message, out byte[] p_data)
        {
            if (p_message == null)
                throw new ArgumentNullException(nameof(p_message));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sr))
            {
                JsonRpcSerializer.Serialize(p_message, writer);
                writer.Flush();
            }
            p_data = Encoding.UTF8.GetBytes(sb.ToString());
            bool compress = p_data.Length > MinCompressLength;

            // 超过长度限制时执行压缩
            if (compress)
            {
                var ms = new MemoryStream();
                using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress))
                {
                    zs.Write(p_data, 0, p_data.Length);
                }
                p_data = ms.ToArray();
            }
            return compress;
        }
    }
}
