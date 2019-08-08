#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
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

        public Stream ResponseStream { get; private set; }

        /// <summary>
        /// 启动Http2协议的远程调用，客户端发送一个请求，服务端返回数据流响应
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseReader> Call()
        {
            try
            {
                using (var request = CreateRequestMessage())
                using (var content = new ByteArrayContent(_data))
                {
                    if (_isCompressed)
                        content.Headers.ContentEncoding.Add("gzip");
                    request.Content = content;
                    var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                    response.EnsureSuccessStatusCode();
                    ResponseStream = await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"调用【{_methodName}】时服务器连接失败！\r\n{ex.Message}");
            }
            return new ResponseReader(this);
        }
    }
}
