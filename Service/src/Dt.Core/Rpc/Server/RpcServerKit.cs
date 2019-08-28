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
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Rpc服务端工具方法
    /// </summary>
    public static class RpcServerKit
    {
        /// <summary>
        /// 将对象打包成一帧写入管道
        /// </summary>
        /// <param name="p_writer"></param>
        /// <param name="p_message"></param>
        /// <returns></returns>
        public static Task WriteFrame(PipeWriter p_writer, object p_message)
        {
            bool compress = RpcKit.SerializeObj(p_message, out var content);
            return WriteFrame(p_writer, content, compress);
        }

        /// <summary>
        /// 向管道写入心跳帧
        /// </summary>
        /// <param name="p_writer"></param>
        /// <returns></returns>
        public static Task WriteHeartbeat(PipeWriter p_writer)
        {
            return WriteFrame(p_writer, RpcKit.ShakeHands, false);
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
            var header = p_writer.GetSpan(RpcKit.HeaderSize);
            header[0] = p_compress ? (byte)1 : (byte)0;
            BinaryPrimitives.WriteUInt32BigEndian(header.Slice(1), (uint)p_data.Length);
            p_writer.Advance(RpcKit.HeaderSize);

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
                catch (Exception ex)
                {
                    Log.Error(ex, "从管道读取帧异常");
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
    }
}
