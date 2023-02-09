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
    abstract class BaseRpc
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
#if SERVER || WIN
            _client = new HttpClient(new HttpClientHandler
            {
                // 验证时服务端证书始终有效！
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });

#elif ANDROID || IOS
            _client = new HttpClient(new NativeMessageHandler());

#elif WASM
            _client = new HttpClient();
#endif


#if SERVER
            // 内部用户标识
            _client.DefaultRequestHeaders.Add("uid", "110");

#elif WASM
            // 识别wasm客户端，允许跨域请求
            _client.DefaultRequestHeaders.Add("dt-wasm", "");
#endif

            // 默认使用http2协议，避免像 _client.GetAsync 方法使用 1.1
            _client.DefaultRequestVersion = new Version(2, 0);
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

            _svcName = p_serviceName.ToLower();
            _methodName = p_methodName;
            _data = GetRpcData(p_methodName, p_params);
        }

        /// <summary>
        /// 获取单例HttpClient，当服务端DNS更新时可能产生无法解析的异常，HttpClientFactory能解决，不急暂缓
        /// </summary>
        public static HttpClient Client => _client;

        /// <summary>
        /// 创建http2协议的Request
        /// </summary>
        /// <returns></returns>
        protected HttpRequestMessage CreateRequestMessage()
        {
            // 使用http2协议Post方法，Version默认1.1，虽DefaultRequestVersion已设2！
            // 只在启用https时采用http2，否则虽然已设置http2但仍采用http1.1协议！！！
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
                RequestUri = new Uri($"{Kit.GetSvcUrl(_svcName)}/.c"),
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
            // 输出日志信息
            string id = TraceLogs.AddRpcJson(data);
            _logCall.ForContext("Detail", id)
                    .Debug($"{_svcName}.{p_methodName}");
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

        static ILogger _logCall = Log.ForContext("Kind", "Call");
    }
}