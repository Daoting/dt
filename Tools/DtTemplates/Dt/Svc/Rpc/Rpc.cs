using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dt.Core
{
    class Rpc
    {
        #region 成员变量
        readonly static HttpClient _client;
        byte[] _data;
        bool _isCompressed;
        #endregion

        static Rpc()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                // 验证时服务端证书始终有效！
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });
            // 内部用户标识
            _client.DefaultRequestHeaders.Add("uid", "110");
        }

        /// <summary>
        /// 发送json格式的Http Rpc远程调用
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <returns>返回远程调用结果</returns>
        public async Task<T> Call<T>(string p_svcUrl, string p_svcName, string p_methodName, params object[] p_params)
        {
            _data = GetRpcData(p_svcName, p_methodName, p_params);

            byte[] data = null;
            using (var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{p_svcUrl}/.c"),
            })
            using (var content = new PushStreamContent((ws) => RpcClientKit.WriteFrame(ws, _data, _isCompressed)))
            {
                request.Content = content;
                HttpResponseMessage response;
                try
                {
                    response = await _client.SendAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Kit.Output($"服务器连接失败 {p_svcUrl}\r\n{ex.Message}");
                    return default(T);
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Kit.Output($"服务器返回状态码：{response.StatusCode}");
                    return default(T);
                }

                var stream = await response.Content.ReadAsStreamAsync();
                data = await RpcClientKit.ReadFrame(stream);
                response.Dispose();
            }
            return ParseResult<T>(data);
        }

        byte[] GetRpcData(string p_svcName, string p_methodName, ICollection<object> p_params)
        {
            byte[] data = RpcKit.GetCallBytes(p_svcName, p_methodName, p_params);

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

        T ParseResult<T>(byte[] p_data)
        {
            T val = default(T);
            RpcResult result = new RpcResult();
            Utf8JsonReader reader = new Utf8JsonReader(p_data);

            try
            {
                // [
                reader.Read();

                // 0成功，1错误，2警告提示
                result.ResultType = (RpcResultType)reader.ReadAsInt();

                // 耗时，非调试状态为0
                reader.Read();
                result.Elapsed = reader.GetInt32().ToString();

                reader.Read();
                if (result.ResultType == RpcResultType.Value)
                {
                    // 结果
                    val = JsonRpcSerializer.Deserialize<T>(ref reader);
                }
                else
                {
                    // 错误或提示信息
                    result.Info = reader.GetString();
                }
            }
            catch
            {
                result.ResultType = RpcResultType.Error;
                result.Info = "返回Json内容结构不正确！";
            }

            if (result.ResultType == RpcResultType.Value)
                return val;

            Kit.Output("服务器异常：" + result.Info);
            return default(T);
        }
    }
}
