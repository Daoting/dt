#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// Websocket Rpc请求处理类
    /// </summary>
    public class WsRpcHandler : RpcHandler
    {
        /// <summary>
        /// WebSocket缓冲区大小
        /// </summary>
        public const int WsMaxBufferSize = 1024;
        WebSocket _socket;

        public WsRpcHandler(HttpContext p_context, WebSocket p_socket)
            : base(p_context)
        {
            _socket = p_socket;
        }

        /// <summary>
        /// 执行Websocket Rpc调用
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public async Task Call(byte[] p_data)
        {
            _error = null;
            _args = null;
            _result = null;
            _elapsed = 0;

            // 解析调用参数
            using (StreamReader sr = new StreamReader(new MemoryStream(p_data)))
            {
                ParseParams(sr);
                if (!string.IsNullOrEmpty(_error))
                {
                    await Answer(false);
                    return;
                }
            }

            // 调用目标方法
            await CallMethod();
            if (!string.IsNullOrEmpty(_error))
            {
                await Answer(_isMessage);
                return;
            }

            // 输出结果
            string strResult = GetResult();
            if (!string.IsNullOrEmpty(_error))
            {
                await Answer(false);
                return;
            }

            // 若发送异常，则跳入外层catch
            byte[] result = Encoding.UTF8.GetBytes(strResult);
            if (result.Length <= WsMaxBufferSize)
            {
                await _socket.SendAsync(
                    new ArraySegment<byte>(result),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            else
            {
                int num = (int)Math.Ceiling((double)result.Length / WsMaxBufferSize);
                for (var i = 0; i < num; i++)
                {
                    int offset = WsMaxBufferSize * i;
                    int count = WsMaxBufferSize;
                    if (count * (i + 1) > result.Length)
                        count = result.Length - offset;
                    await _socket.SendAsync(
                        new ArraySegment<byte>(result, offset, count),
                        WebSocketMessageType.Text,
                        (i + 1) == num,
                        CancellationToken.None);
                }
            }
        }

        /// <summary>
        /// 向Websocket终端输出错误或警告信息
        /// </summary>
        /// <param name="p_isMessage"></param>
        /// <returns></returns>
        async Task Answer(bool p_isMessage)
        {
            // 若发送异常，则跳入外层catch
            ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(GetFaultResult(p_isMessage)));
            await _socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}