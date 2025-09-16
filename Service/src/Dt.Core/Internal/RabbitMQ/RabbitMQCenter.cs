#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2021-01-17 ����
******************************************************************************/
#endregion

#region ��������
using Dt.Core.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
#endregion

namespace Dt.Core.RabbitMQ
{
    /// <summary>
    /// RabbitMQ �ŵ������С�������Ϣ����
    /// </summary>
    [Service(ServiceLifetime.Singleton)]
    public sealed class RabbitMQCenter
    {
        #region ��Ա����
        // ����������
        readonly string _exchangeName = Kit.AppName;
        readonly RabbitMQConnection _conn;
        readonly AsyncLock _mutex;
        IChannel _chPublish;
        #endregion

        #region ���췽��
        public RabbitMQCenter()
        {
            // δ����RabbitMQ���磺�������Boot����
            if (!Kit.EnableRabbitMQ)
                return;

            _conn = new RabbitMQConnection();
            _mutex = new AsyncLock();
            _ = Init();
        }
        #endregion

        #region ����
        /// <summary>
        /// ���� RabbitMQ ��Ϣ
        /// </summary>
        /// <param name="p_data"></param>
        /// <param name="p_routingKey"></param>
        /// <param name="p_bindExchange"></param>
        /// <param name="p_correlationId"></param>
        /// <param name="p_replyTo"></param>
        public async void Publish(
            byte[] p_data,
            string p_routingKey,
            bool p_bindExchange,
            string p_correlationId = null,
            string p_replyTo = null)
        {
            // IModelʵ����֧�ֶ���߳�ͬʱʹ��
            using (await _mutex.LockAsync())
            {
                if (!_conn.IsConnected)
                    await _conn.TryConnect();

                if (_chPublish == null)
                {
                    _chPublish = await _conn.CreateChannel();
                    _chPublish.ChannelShutdownAsync += (s, e) =>
                    {
                        _chPublish.Dispose();
                        _chPublish = null;
                        return Task.CompletedTask;
                    };
                }

                var props = new BasicProperties();
                if (!string.IsNullOrEmpty(p_correlationId))
                    props.CorrelationId = p_correlationId;
                if (!string.IsNullOrEmpty(p_replyTo))
                    props.ReplyTo = p_replyTo;

                await _chPublish.BasicPublishAsync(
                    exchange: p_bindExchange ? _exchangeName : "",
                    routingKey: p_routingKey,
                    mandatory: false,
                    basicProperties: props,
                    body: p_data);
            }
        }
        #endregion

        #region ����
        /// <summary>
        /// ��ʼ�� RabbitMQ �ŵ�������
        /// </summary>
        /// <param name="p_provider"></param>
        internal static void Subscribe(IServiceProvider p_provider)
        {
            // ��������ʵ����ʱ���ж���
            p_provider.GetRequiredService<RabbitMQCenter>();
        }

        async Task Init()
        {
            if (!_conn.IsConnected)
                await _conn.TryConnect();

            // ����������Ϣ��ͨ��
            _chPublish = await _conn.CreateChannel();
            _chPublish.ChannelShutdownAsync += (s, e) =>
            {
                _chPublish.Dispose();
                _chPublish = null;
                return Task.CompletedTask;
            };

            // ������������·�ɹ���
            // direct�����͸�ͬһ���������µ�ӵ����ӦRoutingKey�Ķ���
            // fanout�����͸�ͬһ���������µ����ж���
            // topic�����͸�ͬһ���������µİ�������ʽ��RoutingKeyƥ��Ķ���
            // headers�����͸�ͬһ���������µ�ӵ����ӦRoutingKey����headers�Ķ���
            await _chPublish.ExchangeDeclareAsync(
                _exchangeName,      // ����Ӧ���������ֽ�����
                "topic",            // ��������ʽƥ�����
                durable: true,      // �־û�
                autoDelete: false); // �Ƿ��Զ�ɾ��

            var name = Kit.Svcs[0].SvcName;
            // ÿ��΢�����������������߶���
            // 1. ��dt.cm�����յ�����ʱ��ֱ��Ͷ�� �� ������񸱱�ʱ���þ����㷨Ͷ�ݸ�����һ�������
            await CreateWorkConsumer(name);
            // 2. ��dt.cm.xxx�����ն����и����㲥�򰴷����鲥���������ÿ������id��ͬ�����в����Զ�ɾ��ģʽ
            await CreateTopicConsumer(name);
            // 3. ���Ķ��б仯�¼�(queue.*)������׼ȷ��ȡ����΢����ĸ�������
            // ��ҪRabbitMQ�����¼�֪ͨ�����rabbitmq-plugins enable rabbitmq_event_exchange
            await CreateQueueChangeConsumer(name);
        }

        /// <summary>
        /// ���������߶��� AppName.SvcName��workģʽ��δ�󶨽�������ֻ֧�ֺͶ���������ȫƥ��ʱͶ��
        /// ���ڽ��յ�����ʱ��ֱ��Ͷ�� �� ������񸱱�ʱ���þ����㷨Ͷ�ݸ�����һ�������
        /// </summary>
        /// <param name="p_svcName"></param>
        async Task CreateWorkConsumer(string p_svcName)
        {
            string queueName = $"{Kit.AppName}.{p_svcName}";
            var channel = await _conn.CreateChannel();

            // ��������
            await channel.QueueDeclareAsync(
                queueName,         // ��������
                durable: false,    // �Ƿ�־û�
                exclusive: false,  // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: true); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // ����������
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (s, e) =>
            {
                OnConsumeMessage(e);
                await channel.BasicAckAsync(e.DeliveryTag, false);
            };

            // ����������
            // ����һ�� 0����Ϣ�Ĵ�С�����κ�����
            // �������� 1�����������������Ϣ����������һ��һ�������ѣ����������û��ȷ�����ѣ��������������Ϣ
            // �������� false����Ϊconsumer 
            await channel.BasicQosAsync(0, 1, false);

            // Ҫ�����������뽫autoAck����Ϊfalse
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            // �쳣����
            channel.CallbackExceptionAsync += async (s, e) =>
            {
                try
                {
                    channel.Dispose();
                    channel = null;

                    if (!_conn.IsConnected)
                        await _conn.TryConnect();
                    if (_conn.IsConnected)
                        await CreateWorkConsumer(p_svcName);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"�ؽ�RabbitMQ����{queueName}ʱ�쳣��");
                }
            };
        }

        /// <summary>
        /// ���������߶��� AppName.SvcName.SvcID���󶨽�������topicģʽ��֧�ְ�������ʽƥ�����
        /// AppName.SvcName.*  ���նԷ������и�����Ͷ��
        /// #.SvcID  ���նԵ�ǰ������Ͷ��
        /// </summary>
        /// <param name="p_svcName"></param>
        async Task CreateTopicConsumer(string p_svcName)
        {
            string queueName = $"{Kit.AppName}.{p_svcName}.{Kit.SvcID}";
            var channel = await _conn.CreateChannel();

            // ��������
            await channel.QueueDeclareAsync(
                queueName,         // ��������
                durable: false,    // �Ƿ�־û�
                exclusive: false,  // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: true); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // �󶨶���
            await channel.QueueBindAsync(
                queue: queueName,          // ��������
                exchange: _exchangeName,   // �󶨵Ľ�����
                routingKey: $"{Kit.AppName}.{p_svcName}.*"); // ·������
            await channel.QueueBindAsync(
                queue: queueName,           // ��������
                exchange: _exchangeName,    // �󶨵Ľ�����
                routingKey: $"#.{Kit.SvcID}"); // ·������

            // ����������
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (s, e) =>
            {
                OnConsumeMessage(e);
                await channel.BasicAckAsync(e.DeliveryTag, false);
            };

            // ����������
            // ����һ�� 0����Ϣ�Ĵ�С�����κ�����
            // �������� 1�����������������Ϣ����������һ��һ�������ѣ����������û��ȷ�����ѣ��������������Ϣ
            // �������� false����Ϊconsumer 
            await channel.BasicQosAsync(0, 1, false);

            // Ҫ�����������뽫autoAck����Ϊfalse
            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            // �쳣����
            channel.CallbackExceptionAsync += async (s, e) =>
            {
                try
                {
                    channel.Dispose();
                    channel = null;

                    if (!_conn.IsConnected)
                        await _conn.TryConnect();
                    if (_conn.IsConnected)
                        await CreateTopicConsumer(p_svcName);
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
        async Task CreateQueueChangeConsumer(string p_svcName)
        {
            // ��'-'����Ϊ�˺��������������֣������ȡ�ķ����б����
            string queueName = $"{Kit.AppName}-{p_svcName}-{Kit.SvcID}-queue";
            var channel = await _conn.CreateChannel();

            // ����һ�������ġ��Զ�ɾ���ġ��ǳ־û��Ķ���
            await channel.QueueDeclareAsync(
                queueName,         // ��������
                durable: false,    // �Ƿ�־û�
                exclusive: true,   // �Ƿ�Ϊ�������У���������ֻ�״����ӿɼ������ӶϿ�ʱɾ��
                autoDelete: true); // trueʱ��û���κζ����ߵĻ����ö��лᱻ�Զ�ɾ�������ֶ�����������ʱ����

            // ��queue.*����
            await channel.QueueBindAsync(
               queue: queueName,
               exchange: "amq.rabbitmq.event",
               routingKey: "queue.*");

            // ���б仯ʱ����΢�����б�
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (s, e) =>
            {
                Kit.UpdateSvcList();
                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queue: queueName, true, consumer: consumer);

            // ��֤�״θ����б�
            Kit.UpdateSvcList();
        }

        /// <summary>
        /// ������յ�����Ϣ
        /// </summary>
        /// <param name="p_args"></param>
        void OnConsumeMessage(BasicDeliverEventArgs p_args)
        {
            if (!string.IsNullOrEmpty(p_args.BasicProperties.CorrelationId))
            {
                // Rpc
                if (!string.IsNullOrEmpty(p_args.BasicProperties.ReplyTo))
                {
                    // ����Rpc����
                    _ = new RabbitMQApiInvoker().Process(p_args);
                }
                else
                {
                    // ����Rpc���صĽ��
                    new RabbitMQRpcResponse().Process(p_args);
                }
            }
            else
            {
                _ = new RemoteEventHandler().Process(p_args);
            }
        }
        #endregion
    }
}