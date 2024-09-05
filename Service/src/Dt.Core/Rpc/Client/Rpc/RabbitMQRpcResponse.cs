#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using RabbitMQ.Client.Events;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// RabbitMQ远程调用的结果处理
    /// </summary>
    class RabbitMQRpcResponse
    {
        
        public void Process(BasicDeliverEventArgs p_args)
        {
            if (RabbitMQRpc.RunningCalls.TryGetValue(p_args.BasicProperties.CorrelationId, out var tcs))
            {
                tcs.TrySetResult(p_args);
                RabbitMQRpc.RunningCalls.Remove(p_args.BasicProperties.CorrelationId, out _);
            }
        }
    }
}