#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text.Json;
#endregion

namespace Dt.Core.RabbitMQ
{
    /// <summary>
    /// 处理接收到的 RabbitMQ 远程事件消息
    /// </summary>
    class RemoteEventHandler
    {
        public async Task Process(BasicDeliverEventArgs p_args)
        {
            EventWrapper body;
            try
            {
                body = Kit.Deserialize<EventWrapper>(p_args.Body.Span);
            }
            catch (Exception e)
            {
                Log.Warning(e, "远程事件反序列化时异常！");
                return;
            }

            // 获取IRemoteHandler泛型，不存在表示无Handler
            Type hType;
            if (string.IsNullOrEmpty(body.EventName)
                || !RemoteEventBus.Events.TryGetValue(body.EventName, out hType))
                return;

            // 反序列化事件对象
            object eventObj;
            try
            {
                eventObj = Kit.Deserialize(body.Data, hType.GetGenericArguments()[0]);
            }
            catch (Exception e)
            {
                Log.Warning(e, "远程事件反序列化时异常！");
                return;
            }

            // 组播时排除的副本
            if (eventObj is ExcludeEvent ee && ee.ExcludeSvcID == Kit.SvcID)
                return;

            // 实例化所有Handler
            var handlers = Kit.GetServices(hType);
            var mi = hType.GetMethod("Handle");
            foreach (var h in handlers)
            {
                try
                {
                    await (Task)mi.Invoke(h, new object[] { eventObj });
                }
                catch (Exception e)
                {
                    Log.Warning(e, $"{h.GetType().Name}处理远程事件时异常！");
                }
            }
        }
    }
}