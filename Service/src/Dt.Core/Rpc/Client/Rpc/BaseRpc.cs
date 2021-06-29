#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 远程调用基类
    /// </summary>
    public abstract class BaseRpc
    {
        #region 成员变量
        protected readonly static HttpClient _client;
        protected readonly string _svcName;
        protected readonly string _methodName;
        protected readonly byte[] _data;
        protected bool _isCompressed;
        #endregion

        static BaseRpc()
        {
#if SERVER || UWP
            _client = new HttpClient(new HttpClientHandler
            {
                // 验证时服务端证书始终有效！
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });
#if SERVER
            // 内部用户标识
            _client.DefaultRequestHeaders.Add("uid", "110");
#endif
#elif ANDROID || IOS
            _client = new HttpClient(new NativeMessageHandler());
#elif WASM
            _client = new HttpClient();
#endif
        }

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

            _svcName = p_serviceName;
            _methodName = p_methodName;
            _data = GetRpcData(p_methodName, p_params);
        }

        /// <summary>
        /// 获取单例HttpClient
        /// </summary>
        public static HttpClient Client => _client;

#if !SERVER
        /// <summary>
        /// 刷新HttpClient头的用户信息
        /// </summary>
        internal static void RefreshHeader()
        {
            var header = _client.DefaultRequestHeaders;
            header.Remove("uid");
            if (Kit.IsLogon)
            {
                header.Add("uid", Kit.UserID.ToString());
            }
        }
#endif

        /// <summary>
        /// 创建http2协议的Request
        /// </summary>
        /// <returns></returns>
        protected HttpRequestMessage CreateRequestMessage()
        {
            // 使用http2协议Post方法
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
#if SERVER
                // 部署在k8s时内部DNS通过服务名即可
                RequestUri = new Uri(Kit.IsInDocker ? $"https://{Kit.AppName}-{_svcName}/.c" : $"https://localhost/{Kit.AppName}/{_svcName}/.c"),
#else
                RequestUri = new Uri($"{Kit.Stub.ServerUrl}/{_svcName}/.c"),
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
            byte[] data = RpcKit.GetCallBytes(p_methodName, p_params);

#if !SERVER
            // 输出监视信息
            Kit.Trace(TraceOutType.RpcCall, p_methodName, Encoding.UTF8.GetString(data), _svcName);
#endif

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