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
    [Service(ServiceLifetime.Singleton)]
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
        internal static readonly Dictionary<string, Type> EventHandlerTypes = new Dictionary<string, Type>();

        /// <summary>
        /// ���������¼�
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        public async Task Publish(IEvent p_event)
        {
            Type tp;
            if (p_event == null || !EventHandlerTypes.TryGetValue(p_event.GetType().Name, out tp))
                return;

            var mi = tp.GetMethod("Handle");
            foreach (var h in Kit.GetServices(tp))
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
    }
}