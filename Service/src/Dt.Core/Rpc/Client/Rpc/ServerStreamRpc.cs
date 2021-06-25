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
    /// 客户端发送一个请求，服务端返回数据流响应的远程调用
    /// </summary>
    public class ServerStreamRpc : BaseRpc
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        public ServerStreamRpc(string p_serviceName, string p_methodName, params object[] p_params)
            : base(p_serviceName, p_methodName, p_params)
        { }

        /// <summary>
        /// 启动Http2协议的远程调用，客户端发送一个请求，服务端返回数据流响应
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseReader> Call()
        {
            try
            {
                using (var request = CreateRequestMessage())
                using (var content = new PushStreamContent((ws) => RpcClientKit.WriteFrame(ws, _data, _isCompressed)))
                {
                    // 必须设置 WebAssemblyEnableStreamingResponse 为 true，WASM 才会返回 stream 类型的 response；
                    // 如果不设置，则只在整个response结束时收到一次，无法实现服务器推送流模式！！！参见：
                    // https://github.com/grpc/grpc-dotnet/blob/master/src/Grpc.Net.Client.Web/GrpcWebHandler.cs#L137
                    //
                    // 此限制和服务端无关，只是WASM客户端在收到服务器推送后的处理方式不同，默认response 非 stream模式，参见：
                    // https://github.com/mono/mono/blob/a0d69a4e876834412ba676f544d447ec331e7c01/sdks/wasm/framework/src/System.Net.Http.WebAssemblyHttpHandler/WebAssemblyHttpHandler.cs#L149
                    //
                    // https://github.com/mono/mono/issues/18718
#if WASM
#pragma warning disable CS0618
                    request.Properties["WebAssemblyEnableStreamingResponse"] = true;
#pragma warning restore CS0618
#endif

                    request.Content = content;
                    // 一定是ResponseHeadersRead，否则只在结束时收到一次！！！众里寻他千百度
                    var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                    response.EnsureSuccessStatusCode();

                    var reader = new ResponseReader(response);
                    // wasm中增加上述设置后返回 stream 正常！
                    await reader.InitStream();
                    return reader;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"调用【{_methodName}】时服务器连接失败！\r\n{ex.Message}");
            }
        }
    }
}
