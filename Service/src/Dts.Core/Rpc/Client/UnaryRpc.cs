#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 基于Http2的请求/响应模式的远程调用
    /// </summary>
    public class UnaryRpc
    {
        #region 成员变量
        // 每种服务缓存一个HttpClient，HttpClient所有异步方法都是多线程安全！
        static readonly ConcurrentDictionary<string, HttpClient> _clients = new ConcurrentDictionary<string, HttpClient>();

        protected readonly HttpClient _client;
        protected readonly string _methodName;
        protected readonly byte[] _data;
        protected bool _isCompressed;
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        public UnaryRpc(string p_serviceName, string p_methodName, params object[] p_params)
        {
            if (string.IsNullOrEmpty(p_serviceName) || string.IsNullOrEmpty(p_methodName))
                throw new InvalidOperationException("Rpc调用时需要指定服务名称和API名称！");

            _client = GetHttpClient(p_serviceName);
            _methodName = p_methodName;
            _data = GetRpcData(p_methodName, p_params);
        }

        /// <summary>
        /// 发送json格式的Http Rpc远程调用
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <returns>返回远程调用结果</returns>
        public async Task<T> Call<T>()
        {
            // 远程请求
            Stream stream = null;
            try
            {
                using (var content = new ByteArrayContent(_data))
                {
                    if (_isCompressed)
                        content.Headers.ContentEncoding.Add("gzip");
                    var response = await _client.PostAsync(default(Uri), content).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    stream = await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"调用【{_methodName}】时服务器连接失败！\r\n{ex.Message}");
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
                        throw new Exception($"调用【{_methodName}】对返回结果类型转换时异常：\r\n {result.Value.GetType()}-->{typeof(T)}：{convExp.Message}");
                    }
                }
            }
            else
            {
                throw new Exception($"调用【{_methodName}】异常：\r\n{result.Info}");
            }
            return val;
        }

        HttpClient GetHttpClient(string p_serviceName)
        {
            HttpClient client;
            if (!_clients.TryGetValue(p_serviceName, out client))
            {
                try
                {
                    // 自动GZip解压
                    client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
                    // 此处设置无效！在HttpRequestMessage设置
                    //client.DefaultRequestVersion = new Version(2, 0);
                    // 部署在k8s时内部DNS通过服务名即可
                    string uri = "https://localhost:50002/.c"; //Glb.IsInDocker ? $"https://{p_serviceName}/.c" : $"https://localhost/{Glb.AppName}/{p_serviceName}/.c";
                    client.BaseAddress = new Uri(uri, UriKind.Absolute);
                    _clients.TryAdd(p_serviceName, client);
                }
                catch
                {
                    throw new Exception($"获取【{p_serviceName}】服务的HttpClient失败！");
                }
            }
            return client;
        }

        /// <summary>
        /// 序列化RPC调用，按需压缩
        /// </summary>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数</param>
        /// <returns></returns>
        byte[] GetRpcData(string p_methodName, ICollection<object> p_params)
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
            var data = Encoding.UTF8.GetBytes(sb.ToString());

            // 超过长度限制时执行压缩
            if (data.Length > RpcKit.MinCompressLength)
            {
                _isCompressed = true;
                var ms = new MemoryStream();
                using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress))
                {
                    zs.Write(data, 0, data.Length);
                }
                data = ms.ToArray();
            }
            return data;
        }
    }
}