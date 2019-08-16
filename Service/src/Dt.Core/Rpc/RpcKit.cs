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
using System.IO;
using System.IO.Compression;
using System.Text;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Rpc工具类
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
        public const int MinCompressLength = 4096;

        /// <summary>
        /// 心跳内容：***
        /// </summary>
        public static byte[] ShakeHands = new byte[] { 42, 42, 42 };

        /// <summary>
        /// 对象序列化，超过长度限制时执行压缩
        /// </summary>
        /// <param name="p_message"></param>
        /// <param name="p_data"></param>
        /// <returns>true 已压缩</returns>
        public static bool SerializeObj(object p_message, out byte[] p_data)
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
    }
}
