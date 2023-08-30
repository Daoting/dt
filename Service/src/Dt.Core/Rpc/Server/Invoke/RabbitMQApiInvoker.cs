#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using System;
using System.Buffers.Binary;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 处理接收到的 RabbitMQ Rpc消息
    /// </summary>
    class RabbitMQApiInvoker : ApiInvoker
    {
        BasicDeliverEventArgs _args;

        /// <summary>
        /// 固定用户标识
        /// </summary>
        public override long UserID => 111;

        /// <summary>
        /// 客户端ip
        /// </summary>
        public override string ClientIP => "RabbitMQ";

        /// <summary>
        /// 取消请求的令牌
        /// </summary>
        public override CancellationToken RequestAborted => CancellationToken.None;

        public async Task Process(BasicDeliverEventArgs p_args)
        {
            _args = p_args;

            // 读取RabbitMQ消息内容，已自动解压
            byte[] data = RpcServerKit.ReadRabbitMQMessage(p_args);

            // 解析rpc参数
            if (!await ParseParams(data))
                return;

            // 不校验授权

            if (Api.CallMode != ApiCallMode.Unary)
            {
                await Response(ApiResponseType.Error, 0, "RabbitMQ Rpc不支持流模式");
                return;
            }

            await new UnaryHandler(this).Call();
        }

        protected override Task WriteResponse(byte[] p_data, bool p_compress)
        {
            byte[] result = new byte[p_data.Length + RpcKit.HeaderSize];
            // Frame头：1字节压缩标志 + 4字节内容长度
            result[0] = p_compress ? (byte)1 : (byte)0;
            BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(1), (uint)p_data.Length);
            p_data.CopyTo(result, 5);

            // 发送给固定副本
            var mq = Kit.GetService<RabbitMQCenter>();
            mq.Publish(
                result,
                _args.BasicProperties.ReplyTo,
                true,
                _args.BasicProperties.CorrelationId);

            return Task.CompletedTask;
        }
    }
}