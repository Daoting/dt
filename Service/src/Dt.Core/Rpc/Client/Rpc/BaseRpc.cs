#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 远程调用基类
    /// </summary>
    public abstract class BaseRpc
    {
        #region 成员变量
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
        public BaseRpc(string p_serviceName, string p_methodName, params object[] p_params)
        {
            if (string.IsNullOrEmpty(p_serviceName) || string.IsNullOrEmpty(p_methodName))
                throw new InvalidOperationException("Rpc调用时需要指定服务名称和API名称！");

#if SERVER
            _client = GetHttpClient(p_serviceName);
#else
            _svcName = p_serviceName;
#endif
            _methodName = p_methodName;
            _data = GetRpcData(p_methodName, p_params);
        }

        protected HttpRequestMessage CreateRequestMessage()
        {
            // 使用http2协议Post方法
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
#if !SERVER
                RequestUri = new Uri(_svcName + "/.c", UriKind.Relative),
#endif
            };
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

#if SERVER
        // 每种服务缓存一个HttpClient，HttpClient所有异步方法都是多线程安全！
        static readonly ConcurrentDictionary<string, HttpClient> _clients = new ConcurrentDictionary<string, HttpClient>();

        protected readonly HttpClient _client;

        /// <summary>
        /// 每种服务缓存一个HttpClient
        /// </summary>
        /// <param name="p_serviceName"></param>
        /// <returns></returns>
        HttpClient GetHttpClient(string p_serviceName)
        {
            HttpClient client;
            if (!_clients.TryGetValue(p_serviceName, out client))
            {
                try
                {
                    // 部署在k8s时内部DNS通过服务名即可
                    string uri = Glb.IsInDocker ? $"https://{Glb.AppName}-{p_serviceName}/.c" : $"https://localhost/{Glb.AppName}/{p_serviceName}/.c";

                    // 验证时服务端证书始终有效！
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                    client = new HttpClient(handler);
                    client.BaseAddress = new Uri(uri, UriKind.Absolute);
                    // 此处设置无效！在HttpRequestMessage设置
                    //client.DefaultRequestVersion = new Version(2, 0);

                    _clients.TryAdd(p_serviceName, client);
                }
                catch
                {
                    throw new Exception($"获取【{p_serviceName}】服务的HttpClient失败！");
                }
            }
            return client;
        }
#else
        protected static HttpClient _client;
        readonly string _svcName;

        /// <summary>
        /// 初始化HttpClient
        /// </summary>
        /// <param name="p_url">服务器地址</param>
        internal static void Init(string p_url)
        {
            // 验证时服务端证书始终有效！
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri(p_url.TrimEnd('/') + "/", UriKind.Absolute);
        }

        /// <summary>
        /// 刷新HttpClient头的访问令牌信息
        /// </summary>
        /// <param name="p_token">访问令牌，null时清除认证信息</param>
        internal static void SetToken(string p_token)
        {
            if (string.IsNullOrEmpty(p_token))
                _client.DefaultRequestHeaders.Authorization = null;
            else
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", p_token);
        }
#endif
    }
}