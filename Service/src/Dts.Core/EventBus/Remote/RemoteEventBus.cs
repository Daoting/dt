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
using Newtonsoft.Json;
using Nito.AsyncEx;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.EventBus
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
        readonly string _exchangeName = Glb.AppName;
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
        public void Broadcast(IEvent p_event, bool p_isAllSvcInst = true)
        {
            if (p_isAllSvcInst)
            {
                // ���з�������и���������ڶ�����
                Publish(p_event, Glb.AppName + ".*.*");
            }
            else
            {
                // ÿ������ֻͶ�ݸ�����һ�������������һ����
                Publish(p_event, Glb.AppName + ":*");
            }
        }

        /// <summary>
        /// ��ĳ����������з��񸱱������鲥
        /// </summary>
        public void Multicast(IEvent p_event, string p_svcName)
        {
            // ����ڶ�����
            if (!string.IsNullOrEmpty(p_svcName))
                Publish(p_event, $"{Glb.AppName}.{p_svcName.ToLower()}.*");
        }

        /// <summary>
        /// ��ĳ�����񷢲��¼����ж�����񸱱�ʱ���þ����㷨����ϢͶ�ݸ�����һ��
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_svcName"></param>
        public void Push(IEvent p_event, string p_svcName)
        {
            // �����һ����
            if (!string.IsNullOrEmpty(p_svcName))
                Publish(p_event, $"{Glb.AppName}:{p_svcName.ToLower()}");
        }

        /// <summary>
        /// ����Զ���¼�
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_routingKey"></param>
        async void Publish(IEvent p_event, string p_routingKey)
        {
            if (p_event == null)
                return;

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
                        Data = JsonConvert.SerializeObject(p_event)
                    };
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));

                    var properties = _chPublish.CreateBasicProperties();
                    properties.Persistent = true;
                    _chPublish.BasicPublish(
                        _exchangeName,
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
            // 1. ��dt:cm�����յ�����ʱ��ֱ��Ͷ�� �� ������񸱱�ʱ���þ����㷨Ͷ�ݸ�����һ�������
            CreateConsumerChannel($"{Glb.AppName}:{Glb.SvcName}", false, $"{Glb.AppName}:*");
            // 2. ��dt.cm.xxx�����ն����и����㲥�򰴷����鲥���������ÿ������id��ͬ�����в����Զ�ɾ��ģʽ
            CreateConsumerChannel($"{Glb.AppName}.{Glb.SvcName}.{Glb.ID}", true, $"{Glb.AppName}.*.*");
        }

        void CreateConsumerChannel(string p_queueName, bool p_autoDelete, string p_routingKey)
        {
            IModel channel = _conn.CreateModel();

            // ��������
            channel.QueueDeclare(
                p_queueName,        // ��������
                durable: true,      // �Ƿ�־û�
                exclusive: false,   // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: p_autoDelete); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // �󶨶���
            channel.QueueBind(
                queue: p_queueName,        // ��������
                exchange: _exchangeName,   // �󶨵Ľ�����
                routingKey: p_routingKey); // ·������

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
            channel.BasicConsume(queue: p_queueName, autoAck: false, consumer: consumer);

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
                        CreateConsumerChannel(p_queueName, p_autoDelete, p_routingKey);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "�ؽ�RabbitMQ������ͨ��ʱ�쳣��");
                }
            };
        }

        async Task ProcessEvent(BasicDeliverEventArgs p_args)
        {
            EventWrapper body;
            try
            {
                var message = Encoding.UTF8.GetString(p_args.Body);
                body = JsonConvert.DeserializeObject<EventWrapper>(message);
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
                eventObj = JsonConvert.DeserializeObject(body.Data, hType.GetGenericArguments()[0]);
            }
            catch (Exception e)
            {
                _log.LogWarning(e, "Զ���¼������л�ʱ�쳣��");
                return;
            }

            // ʵ��������Handler
            var handlers = Glb.GetSvcs(hType);
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