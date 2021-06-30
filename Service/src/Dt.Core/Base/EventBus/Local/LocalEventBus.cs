#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-14 ����
******************************************************************************/
#endregion

#region ��������
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// �����¼�����
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
        /// ��Ϊ�¼��������ƣ�ֵΪILocalHandler����
        /// </summary>
        internal static readonly Dictionary<string, Type> NoticeEvents = new Dictionary<string, Type>();

        /// <summary>
        /// ��Ϊ�¼����ͣ�ֵΪHandler����
        /// </summary>
        internal static readonly Dictionary<Type, Type> RequestEvents = new Dictionary<Type, Type>();

        /// <summary>
        /// ���������¼������ȴ�
        /// </summary>
        /// <param name="p_event">�¼�����</param>
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
                    // ��˳�����
                    await (Task)mi.Invoke(h, new object[] { p_event });
                }
                catch (Exception e)
                {
                    _log.LogWarning(e, $"{h.GetType().Name}�������¼�ʱ�쳣��");
                }
            }
        }

        /// <summary>
        /// ��������/��Ӧģʽ���¼�
        /// </summary>
        /// <typeparam name="TResponse">��������</typeparam>
        /// <param name="p_request">��������</param>
        /// <returns>������Ӧֵ</returns>
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
                _log.LogWarning(e, $"{tp.Name}�������¼�ʱ�쳣��");
                // �쳣ʱδ�ٴ��׳�
            }
            return Task.FromResult(default(TResponse));
        }
    }
}