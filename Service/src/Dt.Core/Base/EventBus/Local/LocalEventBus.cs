#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 本地事件总线
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public sealed class LocalEventBus
    {
        readonly ILogger<LocalEventBus> _log;

        public LocalEventBus(ILogger<LocalEventBus> p_log)
        {
            _log = p_log;
        }

        /// <summary>
        /// 键为事件类型名称，值为ILocalHandler泛型
        /// </summary>
        internal static readonly Dictionary<string, Type> NoticeEvents = new Dictionary<string, Type>();

        /// <summary>
        /// 键为事件类型，值为Handler类型
        /// </summary>
        internal static readonly Dictionary<Type, Type> RequestEvents = new Dictionary<Type, Type>();

        /// <summary>
        /// 发布本地事件，不等待
        /// </summary>
        /// <param name="p_event">事件内容</param>
        public async void Publish(IEvent p_event)
        {
            Type tp;
            if (p_event == null || !NoticeEvents.TryGetValue(p_event.GetType().Name, out tp))
                return;

            var mi = tp.GetMethod("Handle");
            foreach (var h in Kit.GetObjs(tp))
            {
                try
                {
                    // 按顺序调用
                    await (Task)mi.Invoke(h, new object[] { p_event });
                }
                catch (Exception e)
                {
                    _log.LogWarning(e, $"{h.GetType().Name}处理本地事件时异常！");
                }
            }
        }

        /// <summary>
        /// 发布请求/响应模式的事件
        /// </summary>
        /// <typeparam name="TResponse">返回类型</typeparam>
        /// <param name="p_request">请求内容</param>
        /// <returns>返回响应值</returns>
        public Task<TResponse> Call<TResponse>(IRequest<TResponse> p_request)
        {
            Type tp;
            if (p_request == null || !RequestEvents.TryGetValue(p_request.GetType(), out tp))
                return Task.FromResult(default(TResponse));

            object handler = Kit.GetObj(tp);
            var mi = tp.GetMethod("Handle");
            try
            {
                return (Task<TResponse>)mi.Invoke(handler, new object[] { p_request });
            }
            catch (Exception e)
            {
                _log.LogWarning(e, $"{tp.Name}处理本地事件时异常！");
                // 异常时未再次抛出
            }
            return Task.FromResult(default(TResponse));
        }
    }
}