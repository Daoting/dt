#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.RabbitMQ
{
    /// <summary>
    /// 基于Http2的请求/响应模式的远程调用
    /// </summary>
    class RabbitMQRpcHandler
    {
        public void Process(BasicDeliverEventArgs p_args)
        {

        }
    }
}