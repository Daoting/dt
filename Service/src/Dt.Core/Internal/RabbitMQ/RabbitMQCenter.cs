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
using Serilog;
#endregion

namespace Dt.Core.RabbitMQ
{
    /// <summary>
    /// RabbitMQ �ŵ����й���
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public sealed class RabbitMQCenter
    {
        #region ��Ա����
        // ����������
        readonly string _exchangeName = Kit.AppName;
        readonly RabbitMQConnection _conn;
        readonly AsyncLock _mutex;
        IModel _chPublish;
        #endregion

        #region ���췽��
        public RabbitMQCenter()
        {
            _conn = new RabbitMQConnection();
            _mutex = new AsyncLock();
            Init();
        }
        #endregion

        /// <summary>
        /// ����Զ���¼�
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_routingKey"></param>
        /// <param name="p_bindExchange"></param>
        public async void Publish(byte[] p_data, string p_routingKey, bool p_bindExchange, string p_correlationId = null)
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

                    var props = _chPublish.CreateBasicProperties();
                    if (!string.IsNullOrEmpty(p_correlationId))
                    {
                        props.CorrelationId = p_correlationId;
                        props.ReplyTo = "." + Kit.SvcID;
                    }

                    _chPublish.BasicPublish(
                        p_bindExchange ? _exchangeName : "",
                        p_routingKey,
                        props,
                        p_data);
                });
            }
        }

        internal static void Subscribe(IServiceProvider p_provider)
        {
            // ��������ʵ����ʱ���ж���
            p_provider.GetRequiredService<RabbitMQCenter>();
        }

        #region ����
        void Init()
        {
            // ��������շ���Ϣ
            if (Kit.IsSingletonSvc)
                return;

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
            // topic�����͸�ͬһ���������µİ��������ʽ��RoutingKeyƥ��Ķ���
            // headers�����͸�ͬһ���������µ�ӵ����ӦRoutingKey����headers�Ķ���
            _chPublish.ExchangeDeclare(
                _exchangeName,      // ����Ӧ���������ֽ�����
                "topic",            // ���������ʽƥ�����
                durable: true,      // �־û�
                autoDelete: false); // �Ƿ��Զ�ɾ��

            // ÿ��΢�����������������߶���
            // 1. ���Ķ��б仯�¼�(queue.*)������׼ȷ��ȡ����΢����ĸ����������ȶ���Ϊ�˱�֤�״θ����б�
            // ��ҪRabbitMQ�����¼�֪ͨ�����rabbitmq-plugins enable rabbitmq_event_exchange
            CreateQueueChangeConsumer();
            // 2. ��dt.cm�����յ�����ʱ��ֱ��Ͷ�� �� ������񸱱�ʱ���þ����㷨Ͷ�ݸ�����һ�������
            CreateWorkConsumer(Kit.Stubs[0].SvcName);
            // 3. ��dt.cm.xxx�����ն����и����㲥�򰴷����鲥���������ÿ������id��ͬ�����в����Զ�ɾ��ģʽ
            CreateTopicConsumer(Kit.Stubs[0].SvcName);
        }

        /// <summary>
        /// ���������߶��� AppName.SvcName��workģʽ��δ�󶨽�������ֻ֧�ֺͶ���������ȫƥ��ʱͶ��
        /// ���ڽ��յ�����ʱ��ֱ��Ͷ�� �� ������񸱱�ʱ���þ����㷨Ͷ�ݸ�����һ�������
        /// </summary>
        /// <param name="p_svcName"></param>
        void CreateWorkConsumer(string p_svcName)
        {
            string queueName = $"{Kit.AppName}.{p_svcName}";
            IModel channel = _conn.CreateModel();

            // ��������
            channel.QueueDeclare(
                queueName,         // ��������
                durable: false,    // �Ƿ�־û�
                exclusive: false,  // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: true); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // ����������
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, e) =>
            {
                OnConsumeMessage(e);
                channel.BasicAck(e.DeliveryTag, false);
            };

            // ����������
            // ����һ�� 0����Ϣ�Ĵ�С�����κ�����
            // �������� 1������������������Ϣ����������һ��һ�������ѣ����������û��ȷ�����ѣ��������������Ϣ
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
                        CreateWorkConsumer(p_svcName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"�ؽ�RabbitMQ����{queueName}ʱ�쳣��");
                }
            };
        }

        /// <summary>
        /// ���������߶��� AppName.SvcName.SvcID���󶨽�������topicģʽ��֧�ְ��������ʽƥ�����
        /// AppName.SvcName.*  ���նԷ������и�����Ͷ��
        /// #.SvcID  ���նԵ�ǰ������Ͷ��
        /// </summary>
        /// <param name="p_svcName"></param>
        void CreateTopicConsumer(string p_svcName)
        {
            string queueName = $"{Kit.AppName}.{p_svcName}.{Kit.SvcID}";
            IModel channel = _conn.CreateModel();

            // ��������
            channel.QueueDeclare(
                queueName,         // ��������
                durable: false,    // �Ƿ�־û�
                exclusive: false,  // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: true); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // �󶨶���
            channel.QueueBind(
                queue: queueName,          // ��������
                exchange: _exchangeName,   // �󶨵Ľ�����
                routingKey: $"{Kit.AppName}.{p_svcName}.*"); // ·������
            channel.QueueBind(
                queue: queueName,           // ��������
                exchange: _exchangeName,    // �󶨵Ľ�����
                routingKey: $"#.{Kit.SvcID}"); // ·������

            // ����������
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, e) =>
            {
                OnConsumeMessage(e);
                channel.BasicAck(e.DeliveryTag, false);
            };

            // ����������
            // ����һ�� 0����Ϣ�Ĵ�С�����κ�����
            // �������� 1������������������Ϣ����������һ��һ�������ѣ����������û��ȷ�����ѣ��������������Ϣ
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
                        CreateTopicConsumer(p_svcName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"�ؽ�RabbitMQ����{queueName}ʱ�쳣��");
                }
            };
        }

        /// <summary>
        /// ����ϵͳ���б仯�¼�(queue.*)������׼ȷ��ȡ����΢����ĸ�������
        /// </summary>
        void CreateQueueChangeConsumer()
        {
            IModel channel = _conn.CreateModel();

            // ����һ����RabbitMQ �����ġ������ġ��Զ�ɾ���ġ��ǳ־û��Ķ���
            var queueName = channel.QueueDeclare().QueueName;

            // ��queue.*����
            channel.QueueBind(
               queue: queueName,
               exchange: "amq.rabbitmq.event",
               routingKey: "queue.*");

            // ���б仯ʱ����΢�����б�
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, e) => Kit.UpdateSvcList();

            channel.BasicConsume(queue: queueName, true, consumer: consumer);
        }

        void OnConsumeMessage(BasicDeliverEventArgs p_args)
        {
            if (!string.IsNullOrEmpty(p_args.BasicProperties.CorrelationId))
                new RabbitMQRpcHandler().Process(p_args);
            else
                _ = new RemoteEventHandler().Process(p_args);
        }
        #endregion
    }
}