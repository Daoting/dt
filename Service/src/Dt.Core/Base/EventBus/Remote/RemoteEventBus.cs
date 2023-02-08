#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-10 ����
******************************************************************************/
#endregion

#region ��������
using Dt.Core.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// ����RabbitMQ���¼�����
    /// </summary>
    [Service(ServiceLifetime.Singleton)]
    public sealed class RemoteEventBus
    {
        #region ��Ա����
        readonly RabbitMQCenter _mq;
        #endregion

        #region ���췽��
        public RemoteEventBus(RabbitMQCenter p_mq)
        {
            _mq = p_mq;
        }
        #endregion

        /// <summary>
        /// ��Ϊ�¼��������ƣ�ֵΪIRemoteHandler����
        /// </summary>
        internal static readonly Dictionary<string, Type> Events = new Dictionary<string, Type>();

        /// <summary>
        /// ��Ӧ���ڵ����з�����й㲥
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        /// <param name="p_isAllSvcInst">true��ʾ���з�������и�����false��ʾ�������ж������ʱֻͶ�ݸ�����һ��</param>
        public void Broadcast(IEvent p_event, bool p_isAllSvcInst = true)
        {
            if (p_event == null)
                return;

            List<string> svcs = Kit.GetAllSvcs(false);
            foreach (var svc in svcs)
            {
                if (p_isAllSvcInst)
                {
                    // ���з�������и���������ڶ�����
                    Publish(p_event, $"{svc}.All", true);
                }
                else
                {
                    // ÿ������ֻͶ�ݸ�����һ�������������һ����
                    Publish(p_event, svc, false);
                }
            }
        }

        /// <summary>
        /// ��Ӧ���ڵĶ��������й㲥
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        /// <param name="p_svcs">�����б�</param>
        /// <param name="p_isAllSvcInst">true��ʾ���з�������и�����false��ʾ�������ж������ʱֻͶ�ݸ�����һ��</param>
        public void Broadcast(IEvent p_event, List<string> p_svcs, bool p_isAllSvcInst = true)
        {
            if (p_event == null || p_svcs == null || p_svcs.Count == 0)
                return;

            if (p_isAllSvcInst)
            {
                // ���з�������и���������ڶ�����
                foreach (var svc in p_svcs)
                {
                    if (!string.IsNullOrEmpty(svc))
                        Publish(p_event, $"{Kit.AppName}.{svc.ToLower()}.All", true);
                }
            }
            else
            {
                // ÿ������ֻͶ�ݸ�����һ�������������һ����
                foreach (var svc in p_svcs)
                {
                    if (!string.IsNullOrEmpty(svc))
                        Publish(p_event, $"{Kit.AppName}.{svc.ToLower()}", false);
                }
            }
        }

        /// <summary>
        /// ��ĳ����������з��񸱱������鲥
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        /// <param name="p_svcName">�������ƣ�null��ʾ��ǰ����</param>
        public void Multicast(IEvent p_event, string p_svcName = null)
        {
            // ����ڶ�����
            if (p_event != null)
            {
                if (string.IsNullOrEmpty(p_svcName))
                    p_svcName = Kit.Stubs[0].SvcName;
                Publish(p_event, $"{Kit.AppName}.{p_svcName.ToLower()}.All", true);
            }
        }

        /// <summary>
        /// ��ĳ�����񷢲��¼����ж�����񸱱�ʱ���þ����㷨����ϢͶ�ݸ�����һ��
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        /// <param name="p_svcName">��������</param>
        public void Push(IEvent p_event, string p_svcName)
        {
            // �����һ����
            if (p_event != null && !string.IsNullOrEmpty(p_svcName))
                Publish(p_event, $"{Kit.AppName}.{p_svcName.ToLower()}", false);
        }

        /// <summary>
        /// ��ĳ������Ĺ̶����������¼���ʹ�ó����٣�������������Ϣ����ͻ������ӵĸ�����ͬ
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        /// <param name="p_svcID">���񸱱�ID</param>
        public void PushFixed(IEvent p_event, string p_svcID)
        {
            // ����ڶ����У�ʹ����������ʱ��ƥ�����и�����
            if (p_event != null && !string.IsNullOrEmpty(p_svcID))
                Publish(p_event, $".{p_svcID}", true);
        }

        /// <summary>
        /// ����Զ���¼�
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_routingKey"></param>
        /// <param name="p_bindExchange"></param>
        void Publish(IEvent p_event, string p_routingKey, bool p_bindExchange)
        {
            // δ����RabbitMQ���շ���Ϣ���磺�������Boot����
            if (!Kit.EnableRabbitMQ)
                return;

            // ���л�
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