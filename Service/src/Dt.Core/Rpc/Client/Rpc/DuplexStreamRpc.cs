#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 客户端发送请求数据流，服务端返回数据流响应的远程调用
    /// </summary>
    public class DuplexStreamRpc : ClientStreamRpc
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        public DuplexStreamRpc(string p_serviceName, string p_methodName, params object[] p_params)
            : base(p_serviceName, p_methodName, p_params)
        { }

        /// <summary>
        /// 启动Http2协议的远程调用，客户端与服务端双工流
        /// </summary>
        /// <returns></returns>
        new public async Task<DuplexStream> Call()
        {
            var request = CreateRequestMessage();
            var writer = CreateWriter(request);
            var response = await SendRequest(request);
            var reader = new ResponseReader(response);
            await reader.InitStream();
            return new DuplexStream(writer, reader);
        }

        async Task<HttpResponseMessage> SendRequest(HttpRequestMessage p_request)
        {
            try
            {
                var response = await _client.SendAsync(p_request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"调用【{_methodName}】时服务器连接失败！\r\n{ex.Message}");
            }
        }
    }
}
