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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 服务之间的Http远程调用，Json格式和客户端相同
    /// </summary>
    public static class ServiceRpc
    {
        // 每种服务缓存一个HttpClient，HttpClient所有异步方法都是多线程安全！
        static readonly ConcurrentDictionary<string, HttpClient> _clients = new ConcurrentDictionary<string, HttpClient>();

        /// <summary>
        /// 发送json格式的Http Rpc远程调用
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回远程调用结果</returns>
        public static async Task<T> Call<T>(string p_serviceName, string p_methodName, params object[] p_params)
        {
            // 获取服务地址
            HttpClient client;
            if (!_clients.TryGetValue(p_serviceName, out client))
            {
                try
                {
                    client = CreateClient(p_serviceName);
                    _clients.TryAdd(p_serviceName, client);
                }
                catch
                {
                    throw new Exception($"调用【{p_methodName}】时获取【{p_serviceName}】服务的HttpClient失败！");
                }
            }

            // 远程请求
            Stream stream = null;
            try
            {
                using (var content = new StringContent(GetRpcJson(p_methodName, p_params)))
                {
                    // 默认编码utf8
                    var response = await client.PostAsync(default(Uri), content);
                    response.EnsureSuccessStatusCode();
                    stream = await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"调用【{p_methodName}】时服务器连接失败！\r\n{ex.Message}");
            }

            // 解析结果
            RpcResult result = new RpcResult();
            using (StreamReader sr = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                try
                {
                    // [
                    reader.Read();
                    // 0成功，1错误，2警告提示
                    result.ResultType = (RpcResultType)reader.ReadAsInt32();
                    // 耗时，非调试状态为0
                    result.Elapsed = reader.ReadAsString();
                    if (result.ResultType == RpcResultType.Value)
                    {
                        reader.Read();
                        result.Value = JsonRpcSerializer.Deserialize(reader);
                    }
                    else
                    {
                        // 错误或提示信息
                        result.Info = reader.ReadAsString();
                    }
                }
                catch
                {
                    result.ResultType = RpcResultType.Error;
                    result.Info = "返回Json内容结构不正确！";
                }
            }
            stream.Close();

            // 返回结果
            T val = default(T);
            if (result.ResultType == RpcResultType.Value)
            {
                if (result.Value == null)
                {
                    // 空值
                }
                else if (typeof(T) == result.Value.GetType())
                {
                    // 结果对象与给定对象类型相同时
                    val = (T)result.Value;
                }
                else
                {
                    // 特殊处理结果对象与给定对象类型不相同时
                    try
                    {
                        val = (T)Convert.ChangeType(result.Value, typeof(T));
                    }
                    catch (Exception convExp)
                    {
                        throw new Exception($"调用【{p_methodName}】对返回结果类型转换时异常：\r\n {result.Value.GetType()}-->{typeof(T)}：{convExp.Message}");
                    }
                }
            }
            else
            {
                throw new Exception($"调用【{p_methodName}】异常：\r\n{result.Info}");
            }
            return val;
        }

        /// <summary>
        /// 序列化RPC调用
        /// </summary>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数</param>
        /// <returns></returns>
        public static string GetRpcJson(string p_methodName, ICollection<object> p_params)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sr))
            {
                writer.WriteStartArray();
                writer.WriteValue(p_methodName);
                if (p_params != null && p_params.Count > 0)
                {
                    foreach (var par in p_params)
                    {
                        JsonRpcSerializer.Serialize(par, writer);
                    }
                }
                writer.WriteEndArray();
                writer.Flush();
            }
            return sb.ToString();
        }

        static HttpClient CreateClient(string p_serviceName)
        {
            // 自动GZip解压
            HttpClient client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
            // 保持TCP连接
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            // 部署在k8s时内部DNS通过服务名即可
            string uri = Glb.IsInDocker ? $"http://{p_serviceName}/.c" : $"http://localhost/{Glb.AppName}/{p_serviceName}/.c";
            client.BaseAddress = new Uri(uri, UriKind.Absolute);
            return client;
        }
    }

    /// <summary>
    /// 远程回调结果包装类
    /// </summary>
    internal class RpcResult
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        public RpcResultType ResultType { get; set; }

        /// <summary>
        /// 结果值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        public string Elapsed { get; set; }

        /// <summary>
        /// 监控结果内容
        /// </summary>
        public string Trace { get; set; }
    }

    /// <summary>
    /// 反序列化结果的种类
    /// </summary>
    internal enum RpcResultType
    {
        /// <summary>
        /// 普通结果值
        /// </summary>
        Value,

        /// <summary>
        /// 服务端错误信息
        /// </summary>
        Error,

        /// <summary>
        /// 业务警告信息
        /// </summary>
        Message
    }
}
