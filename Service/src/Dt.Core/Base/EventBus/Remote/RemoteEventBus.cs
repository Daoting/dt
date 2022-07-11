#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 基于RabbitMQ的事件总线
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public sealed class RemoteEventBus
    {
        #region 成员变量
        readonly RabbitMQCenter _mq;
        #endregion

        #region 构造方法
        public RemoteEventBus(RabbitMQCenter p_mq)
        {
            _mq = p_mq;
        }
        #endregion

        /// <summary>
        /// 键为事件类型名称，值为IRemoteHandler泛型
        /// </summary>
        internal static readonly Dictionary<string, Type> Events = new Dictionary<string, Type>();

        /// <summary>
        /// 向应用内的所有服务进行广播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_isAllSvcInst">true表示所有服务的所有副本，false表示当服务有多个副本时只投递给其中一个</param>
        public void Broadcast(IEvent p_event, bool p_isAllSvcInst = true)
        {
            if (p_event == null)
                return;

            List<string> svcs = Kit.GetAllSvcs(false);
            foreach (var svc in svcs)
            {
                if (p_isAllSvcInst)
                {
                    // 所有服务的所有副本，进入第二队列
                    Publish(p_event, $"{svc}.All", true);
                }
                else
                {
                    // 每个服务只投递给其中一个副本，进入第一队列
                    Publish(p_event, svc, false);
                }
            }
        }

        /// <summary>
        /// 向应用内的多个服务进行广播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcs">服务列表</param>
        /// <param name="p_isAllSvcInst">true表示所有服务的所有副本，false表示当服务有多个副本时只投递给其中一个</param>
        public void Broadcast(IEvent p_event, List<string> p_svcs, bool p_isAllSvcInst = true)
        {
            if (p_event == null || p_svcs == null || p_svcs.Count == 0)
                return;

            if (p_isAllSvcInst)
            {
                // 所有服务的所有副本，进入第二队列
                foreach (var svc in p_svcs)
                {
                    if (!string.IsNullOrEmpty(svc))
                        Publish(p_event, $"{Kit.AppName}.{svc.ToLower()}.All", true);
                }
            }
            else
            {
                // 每个服务只投递给其中一个副本，进入第一队列
                foreach (var svc in p_svcs)
                {
                    if (!string.IsNullOrEmpty(svc))
                        Publish(p_event, $"{Kit.AppName}.{svc.ToLower()}", false);
                }
            }
        }

        /// <summary>
        /// 向某个服务的所有服务副本进行组播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcName">服务名称，null表示当前服务</param>
        public void Multicast(IEvent p_event, string p_svcName = null)
        {
            // 进入第二队列
            if (p_event != null)
            {
                if (string.IsNullOrEmpty(p_svcName))
                    p_svcName = Kit.Stubs[0].SvcName;
                Publish(p_event, $"{Kit.AppName}.{p_svcName.ToLower()}.All", true);
            }
        }

        /// <summary>
        /// 向某个服务发布事件，有多个服务副本时采用均衡算法将消息投递给其中一个
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcName">服务名称</param>
        public void Push(IEvent p_event, string p_svcName)
        {
            // 进入第一队列
            if (p_event != null && !string.IsNullOrEmpty(p_svcName))
                Publish(p_event, $"{Kit.AppName}.{p_svcName.ToLower()}", false);
        }

        /// <summary>
        /// 向某个服务的固定副本发布事件，使用场景少，如在线推送消息，因客户端连接的副本不同
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcID">服务副本ID</param>
        public void PushFixed(IEvent p_event, string p_svcID)
        {
            // 进入第二队列，使用完整名称时会匹配所有副本！
            if (p_event != null && !string.IsNullOrEmpty(p_svcID))
                Publish(p_event, $".{p_svcID}", true);
        }

        /// <summary>
        /// 发布远程事件
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_routingKey"></param>
        /// <param name="p_bindExchange"></param>
        void Publish(IEvent p_event, string p_routingKey, bool p_bindExchange)
        {
            // 未启用RabbitMQ不收发消息，如：单体服务、Boot服务
            if (!Kit.EnableRabbitMQ)
                return;

            // 序列化
            EventWrapper body = new EventWrapper
            {
                EventName = p_event.GetType().Name,
                Data = JsonSerializer.Serialize(p_event, p_event.GetType(), JsonOptions.UnsafeSerializer)
            };
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body, JsonOptions.UnsafeSerializer));
            _mq.Publish(data, p_routingKey, p_bindExchange);
        }
    }
}