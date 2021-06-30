#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-10 ����
******************************************************************************/
#endregion

#region ��������
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Nito.AsyncEx;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// ����RabbitMQ���¼�����
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public sealed class RemoteEventBus
    {
        #region ��Ա����
        /// <summary>
        /// ��Ϊ�¼��������ƣ�ֵΪIRemoteHandler����
        /// </summary>
        internal static readonly Dictionary<string, Type> Events = new Dictionary<string, Type>();

        // ����������
        readonly string _exchangeName = Kit.AppName;
        readonly RabbitMQConnection _conn;
        readonly ILogger<RemoteEventBus> _log;
        readonly AsyncLock _mutex = new AsyncLock();
        IModel _chPublish;
        #endregion

        #region ���췽��
        public RemoteEventBus(RabbitMQConnection p_conn, ILogger<RemoteEventBus> p_log)
        {
            _conn = p_conn;
            _log = p_log;
            Init();
        }
        #endregion

        #region ����
        /// <summary>
        /// ��Ӧ���ڵ����з�����й㲥
        /// </summary>
        /// <param name="p_event">�¼�����</param>
        /// <param name="p_isAllSvcInst">true��ʾ���з�������и�����false��ʾ�������ж������ʱֻͶ�ݸ�����һ��</param>
        public async void Broadcast(IEvent p_event, bool p_isAllSvcInst = true)
        {
            if (p_event == null)
                return;

            List<string> svcs = await Kit.GetAllSvcs(false);
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
                    p_svcName = Kit.SvcName;
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
        async void Publish(IEvent p_event, string p_routingKey, bool p_bindExchange)
        {
            // IModelʵ����֧�ֶ���߳�ͬʱʹ��
            using (await _mutex.LockAsync())
            {
                await Task.Run(() =>
                {
                    if (!_conn.IsConnected)
                        _conn.TryConnect();

                    if (_chPublish == null)
                    {
                        _chPublish = _conn.CreateModel();
                        _chPublish.ModelShutdown += (s, e) =>
                        {
                            _chPublish.Dispose();
                            _chPublish = null;
                        };
                    }

                    // ���л�
                    EventWrapper body = new EventWrapper
                    {
                        EventName = p_event.GetType().Name,
                        Data = JsonSerializer.Serialize(p_event, p_event.GetType(), JsonOptions.UnsafeSerializer)
                    };
                    var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body, JsonOptions.UnsafeSerializer));

                    var properties = _chPublish.CreateBasicProperties();
                    properties.Persistent = true;
                    _chPublish.BasicPublish(
                        p_bindExchange ? _exchangeName : "",
                        routingKey: p_routingKey,
                        basicProperties: properties,
                        body: data);
                });
            }
        }
        #endregion

        #region ����
        internal static void Subscribe(IServiceProvider p_provider)
        {
            // ��������ʵ����ʱ���ж���
            p_provider.GetRequiredService<RemoteEventBus>();
        }

        void Init()
        {
            if (!_conn.IsConnected)
                _conn.TryConnect();

            // ����������Ϣ��ͨ��
            _chPublish = _conn.CreateModel();
            _chPublish.ModelShutdown += (s, e) =>
            {
                _chPublish.Dispose();
                _chPublish = null;
            };

            // ������������·�ɹ���
            // direct�����͸�ͬһ���������µ�ӵ����ӦRoutingKey�Ķ���
            // fanout�����͸�ͬһ���������µ����ж���
            // topic�����͸�ͬһ���������µİ�������ʽ��RoutingKeyƥ��Ķ���
            // headers�����͸�ͬһ���������µ�ӵ����ӦRoutingKey����headers�Ķ���
            _chPublish.ExchangeDeclare(
                _exchangeName,      // ����Ӧ���������ֽ�����
                "topic",            // ��������ʽƥ�����
                durable: true,      // �־û�
                autoDelete: false); // �Ƿ��Զ�ɾ��

            // �������������߶���
            // 1. ��dt.cm�����յ�����ʱ��ֱ��Ͷ�� �� ������񸱱�ʱ���þ����㷨Ͷ�ݸ�����һ�������
            CreateWorkConsumer();
            // 2. ��dt.cm.xxx�����ն����и����㲥�򰴷����鲥���������ÿ������id��ͬ�����в����Զ�ɾ��ģʽ
            CreateTopicConsumer();
        }

        /// <summary>
        /// ���������߶��� AppName.SvcName��workģʽ��δ�󶨽�������ֻ֧�ֺͶ���������ȫƥ��ʱͶ��
        /// ���ڽ��յ�����ʱ��ֱ��Ͷ�� �� ������񸱱�ʱ���þ����㷨Ͷ�ݸ�����һ�������
        /// </summary>
        void CreateWorkConsumer()
        {
            string queueName = $"{Kit.AppName}.{Kit.SvcName}";
            IModel channel = _conn.CreateModel();

            // ��������
            channel.QueueDeclare(
                queueName,         // ��������
                durable: false,    // �Ƿ�־û�
                exclusive: false,  // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: true); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // ����������
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (s, e) =>
            {
                await ProcessEvent(e);
                channel.BasicAck(e.DeliveryTag, false);
            };

            // ����������
            // ����һ�� 0����Ϣ�Ĵ�С�����κ�����
            // �������� 1�����������������Ϣ����������һ��һ�������ѣ����������û��ȷ�����ѣ��������������Ϣ
            // �������� false����Ϊconsumer 
            channel.BasicQos(0, 1, false);

            // Ҫ�����������뽫autoAck����Ϊfalse
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            // �쳣����
            channel.CallbackException += (s, e) =>
            {
                try
                {
                    channel.Dispose();
                    channel = null;

                    if (!_conn.IsConnected)
                        _conn.TryConnect();
                    if (_conn.IsConnected)
                        CreateWorkConsumer();
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"�ؽ�RabbitMQ����{queueName}ʱ�쳣��");
                }
            };
        }

        /// <summary>
        /// ���������߶��� AppName.SvcName.SvcID���󶨽�������topicģʽ��֧�ְ�������ʽƥ�����
        /// AppName.SvcName.*  ���նԷ������и�����Ͷ��
        /// #.SvcID  ���նԵ�ǰ������Ͷ��
        /// </summary>
        void CreateTopicConsumer()
        {
            string queueName = $"{Kit.AppName}.{Kit.SvcName}.{Kit.SvcID}";
            IModel channel = _conn.CreateModel();

            // ��������
            channel.QueueDeclare(
                queueName,         // ��������
                durable: false,    // �Ƿ�־û�
                exclusive: false,  // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: true); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // �󶨶���
            channel.QueueBind(
                queue: queueName,        // ��������
                exchange: _exchangeName,   // �󶨵Ľ�����
                routingKey: $"{Kit.AppName}.{Kit.SvcName}.*"); // ·������
            channel.QueueBind(
                queue: queueName,           // ��������
                exchange: _exchangeName,    // �󶨵Ľ�����
                routingKey: $"#.{Kit.SvcID}"); // ·������

            // ����������
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (s, e) =>
            {
                await ProcessEvent(e);
                channel.BasicAck(e.DeliveryTag, false);
            };

            // ����������
            // ����һ�� 0����Ϣ�Ĵ�С�����κ�����
            // �������� 1�����������������Ϣ����������һ��һ�������ѣ����������û��ȷ�����ѣ��������������Ϣ
            // �������� false����Ϊconsumer 
            channel.BasicQos(0, 1, false);

            // Ҫ�����������뽫autoAck����Ϊfalse
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            // �쳣����
            channel.CallbackException += (s, e) =>
            {
                try
                {
                    channel.Dispose();
                    channel = null;

                    if (!_conn.IsConnected)
                        _conn.TryConnect();
                    if (_conn.IsConnected)
                        CreateTopicConsumer();
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"�ؽ�RabbitMQ����{queueName}ʱ�쳣��");
                }
            };
        }

        async Task ProcessEvent(BasicDeliverEventArgs p_args)
        {
            EventWrapper body;
            try
            {
                body = JsonSerializer.Deserialize<EventWrapper>(p_args.Body.Span, JsonOptions.UnsafeSerializer);
            }
            catch (Exception e)
            {
                _log.LogWarning(e, "Զ���¼������л�ʱ�쳣��");
                return;
            }

            // ��ȡIRemoteHandler���ͣ������ڱ�ʾ��Handler
            Type hType;
            if (string.IsNullOrEmpty(body.EventName)
                || !Events.TryGetValue(body.EventName, out hType))
                return;

            // �����л��¼�����
            object eventObj;
            try
            {
                eventObj = JsonSerializer.Deserialize(body.Data, hType.GetGenericArguments()[0], JsonOptions.UnsafeSerializer);
            }
            catch (Exception e)
            {
                _log.LogWarning(e, "Զ���¼������л�ʱ�쳣��");
                return;
            }

            // �鲥ʱ�ų��ĸ���
            if (eventObj is ExcludeEvent ee && ee.ExcludeSvcID == Kit.SvcID)
                return;

            // ʵ��������Handler
            var handlers = Kit.GetObjs(hType);
            var mi = hType.GetMethod("Handle");
            foreach (var h in handlers)
            {
                try
                {
                    await (Task)mi.Invoke(h, new object[] { eventObj });
                }
                catch (Exception e)
                {
                    _log.LogWarning(e, $"{h.GetType().Name}����Զ���¼�ʱ�쳣��");
                }
            }
        }
        #endregion
    }
}