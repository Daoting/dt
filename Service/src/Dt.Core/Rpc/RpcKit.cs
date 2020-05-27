#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
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
            p_data = GetObjectBytes(p_message);
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

        /// <summary>
        /// 获取Rpc调用时json格式的字节数组
        /// </summary>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数</param>
        /// <param name="p_indented">序列化json时是否含有缩进</param>
        /// <returns>字节数组</returns>
        public static byte[] GetCallBytes(string p_methodName, ICollection<object> p_params, bool p_indented = false)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, p_indented ? JsonOptions.IndentedWriter : JsonOptions.UnsafeWriter))
                {
                    writer.WriteStartArray();
                    writer.WriteStringValue(p_methodName);
                    if (p_params != null && p_params.Count > 0)
                    {
                        foreach (var par in p_params)
                        {
                            JsonRpcSerializer.Serialize(par, writer);
                        }
                    }
                    writer.WriteEndArray();
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 获取Rpc调用时json字符串
        /// </summary>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数</param>
        /// <param name="p_indented">序列化json时是否含有缩进</param>
        /// <returns>json串</returns>
        public static string GetCallString(string p_methodName, ICollection<object> p_params, bool p_indented = false)
        {
            return Encoding.UTF8.GetString(GetCallBytes(p_methodName, p_params, p_indented));
        }

        /// <summary>
        /// 获取通过网络传输对象时的json格式的字节数组
        /// </summary>
        /// <param name="p_obj">待传输对象</param>
        /// <returns>字节数组</returns>
        public static byte[] GetObjectBytes(object p_obj)
        {
            if (p_obj == null)
                throw new ArgumentNullException(nameof(p_obj));

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
                {
                    JsonRpcSerializer.Serialize(p_obj, writer);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 获取通过网络传输对象时的json串
        /// </summary>
        /// <param name="p_obj">待传输对象</param>
        /// <returns></returns>
        public static string GetObjectString(object p_obj)
        {
            return Encoding.UTF8.GetString(GetObjectBytes(p_obj));
        }

        /// <summary>
        /// json串解析成内部支持的对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="p_json">json串</param>
        /// <returns></returns>
        public static T ParseString<T>(string p_json)
        {
            Throw.IfNullOrEmpty(p_json);
            return ParseBytes<T>(Encoding.UTF8.GetBytes(p_json));
        }

        /// <summary>
        /// json字节数组解析成内部支持的对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static T ParseBytes<T>(byte[] p_data)
        {
            if (p_data == null || p_data.Length == 0)
                return default;

            // Utf8JsonReader不能用在异步方法内！
            Utf8JsonReader reader = new Utf8JsonReader(p_data);
            reader.Read();
            return JsonRpcSerializer.Deserialize<T>(ref reader);
        }
    }
}
