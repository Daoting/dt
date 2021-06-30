#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 两种EventBus
    /// </summary>
    public partial class Kit
    {
        #region RemoteEventBus
        /// <summary>
        /// 向应用内的所有服务进行广播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_isAllSvcInst">true表示所有服务的所有副本，false表示当服务有多个副本时只投递给其中一个</param>
        public static void RemoteBroadcast(IEvent p_event, bool p_isAllSvcInst = true)
        {
            GetObj<RemoteEventBus>().Broadcast(p_event, p_isAllSvcInst);
        }

        /// <summary>
        /// 向应用内的多个服务进行广播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcs">服务列表</param>
        /// <param name="p_isAllSvcInst">true表示所有服务的所有副本，false表示当服务有多个副本时只投递给其中一个</param>
        public static void RemoteBroadcast(IEvent p_event, List<string> p_svcs, bool p_isAllSvcInst = true)
        {
            GetObj<RemoteEventBus>().Broadcast(p_event, p_svcs, p_isAllSvcInst);
        }

        /// <summary>
        /// 向某个服务的所有服务副本进行组播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcName">服务名称，null表示当前服务</param>
        public static void RemoteMulticast(IEvent p_event, string p_svcName = null)
        {
            GetObj<RemoteEventBus>().Multicast(p_event, p_svcName);
        }

        /// <summary>
        /// 向某个服务发布事件，有多个服务副本时采用均衡算法将消息投递给其中一个
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcName">服务名称</param>
        public static void RemotePush(IEvent p_event, string p_svcName)
        {
            GetObj<RemoteEventBus>().Push(p_event, p_svcName);
        }

        /// <summary>
        /// 向某个服务的固定副本发布事件，使用场景少，如在线推送消息，因客户端连接的副本不同
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcID">服务副本ID</param>
        public static void RemotePushFixed(IEvent p_event, string p_svcID)
        {
            GetObj<RemoteEventBus>().PushFixed(p_event, p_svcID);
        }
        #endregion

        #region LocalEventBus
        /// <summary>
        /// 发布本地事件，不等待
        /// </summary>
        /// <param name="p_event">事件内容</param>
        public static void LocalPublish(IEvent p_event)
        {
            GetObj<LocalEventBus>().Publish(p_event);
        }

        /// <summary>
        /// 发布请求/响应模式的事件
        /// </summary>
        /// <typeparam name="TResponse">返回类型</typeparam>
        /// <param name="p_request">请求内容</param>
        /// <returns>返回响应值</returns>
        public static Task<TResponse> LocalCall<TResponse>(IRequest<TResponse> p_request)
        {
            return GetObj<LocalEventBus>().Call(p_request);
        }
        #endregion
    }
}
