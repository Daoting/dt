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
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;
#endregion

namespace Dts.Core.Rpc
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
        /// 获取对象序列化后的完整数据包，1字节压缩标志 + 4字节内容长度 + 内容
        /// </summary>
        /// <param name="p_obj"></param>
        /// <returns></returns>
        public static byte[] GetObjData(object p_obj)
        {
            if (p_obj == null)
                throw new InvalidDataException(nameof(p_obj));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sr))
            {
                writer.WriteStartArray();
                JsonRpcSerializer.Serialize(p_obj, writer);
                writer.WriteEndArray();
                writer.Flush();
            }
            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
            bool isCompress = data.Length > MinCompressLength;

            // 超过长度限制时执行压缩
            if (isCompress)
            {
                var ms = new MemoryStream();
                using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress))
                {
                    zs.Write(data, 0, data.Length);
                }
                data = ms.ToArray();
            }

            byte[] result = new byte[data.Length + HeaderSize];
            // 压缩标志位
            result[0] = (byte)(isCompress ? 1 : 0);
            // 内容长度
            BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(1), (uint)data.Length);
            // 内容
            Array.Copy(data, 0, result, HeaderSize, data.Length);
            return result;
        }
    }
}
