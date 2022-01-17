#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
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
    /// RabbitMQ 信道、队列、发布消息管理
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public sealed class RabbitMQCenter
    {
        #region 成员变量
        // 交换机名称
        readonly string _exchangeName = Kit.AppName;
        readonly RabbitMQConnection _conn;
        readonly AsyncLock _mutex;
        IModel _chPublish;
        #endregion

        #region 构造方法
        public RabbitMQCenter()
        {
            _conn = new RabbitMQConnection();
            _mutex = new AsyncLock();
            Init();
        }
        #endregion

        #region 发布
        /// <summary>
        /// 发布 RabbitMQ 消息
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
            // IModel实例不支持多个线程同时使用
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
                        props.CorrelationId = p_correlationId;
                    if (!string.IsNullOrEmpty(p_replyTo))
                        props.ReplyTo = p_replyTo;

                    _chPublish.BasicPublish(
                        p_bindExchange ? _exchangeName : "",
                        p_routingKey,
                        props,
                        p_data);
                });
            }
        }
        #endregion

        #region 订阅
        /// <summary>
        /// 初始化 RabbitMQ 信道、队列
        /// </summary>
        /// <param name="p_provider"></param>
        internal static void Subscribe(IServiceProvider p_provider)
        {
            // 单例对象，实例化时进行订阅
            p_provider.GetRequiredService<RabbitMQCenter>();
        }

        void Init()
        {
            // 单体服务不收发消息
            if (Kit.IsSingletonSvc)
                return;

            if (!_conn.IsConnected)
                _conn.TryConnect();

            // 负责生产消息的通道
            _chPublish = _conn.CreateModel();
            _chPublish.ModelShutdown += (s, e) =>
            {
                _chPublish.Dispose();
                _chPublish = null;
            };

            // 声明交换机，路由规则：
            // direct：发送给同一个交换机下的拥有相应RoutingKey的队列
            // fanout：发送给同一个交换机下的所有队列
            // topic：发送给同一个交换机下的按正则表达式对RoutingKey匹配的队列
            // headers：发送给同一个交换机下的拥有相应RoutingKey或者headers的队列
            _chPublish.ExchangeDeclare(
                _exchangeName,      // 采用应用名称区分交换机
                "topic",            // 按正则表达式匹配队列
                durable: true,      // 持久化
                autoDelete: false); // 是否自动删除

            // 每个微服务声明三个消费者队列
            // 1. 订阅队列变化事件(queue.*)，用来准确获取所有微服务的副本个数，先订阅为了保证首次更新列表
            // 需要RabbitMQ启用事件通知插件：rabbitmq-plugins enable rabbitmq_event_exchange
            CreateQueueChangeConsumer();
            // 2. 如dt.cm，接收单副本时的直接投递 或 多个服务副本时采用均衡算法投递给其中一个的情况
            CreateWorkConsumer(Kit.Stubs[0].SvcName);
            // 3. 如dt.cm.xxx，接收对所有副本广播或按服务组播的情况，因每次重启id不同，队列采用自动删除模式
            CreateTopicConsumer(Kit.Stubs[0].SvcName);
        }

        /// <summary>
        /// 声明消费者队列 AppName.SvcName，work模式，未绑定交换机，只支持和队列名称完全匹配时投递
        /// 用于接收单副本时的直接投递 或 多个服务副本时采用均衡算法投递给其中一个的情况
        /// </summary>
        /// <param name="p_svcName"></param>
        void CreateWorkConsumer(string p_svcName)
        {
            string queueName = $"{Kit.AppName}.{p_svcName}";
            IModel channel = _conn.CreateModel();

            // 声明队列
            channel.QueueDeclare(
                queueName,         // 队列名称
                durable: false,    // 是否持久化
                exclusive: false,  // 是否为排他队列，若排他则只首次连接可见，连接断开时删除
                autoDelete: true); // true时若没有任何订阅者的话，该队列会被自动删除，这种队列适用于临时队列

            // 创建消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, e) =>
            {
                OnConsumeMessage(e);
                channel.BasicAck(e.DeliveryTag, false);
            };

            // 限流的设置
            // 参数一： 0表消息的大小不做任何限制
            // 参数二： 1表服务器给的最大的消息数，这里是一条一条的消费，如果消费者没有确认消费，将不会接受新消息
            // 参数三： false级别为consumer 
            channel.BasicQos(0, 1, false);

            // 要想做限流必须将autoAck设置为false
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            // 异常处理
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
                    Log.Error(ex, $"重建RabbitMQ队列{queueName}时异常！");
                }
            };
        }

        /// <summary>
        /// 声明消费者队列 AppName.SvcName.SvcID，绑定交换机，topic模式，支持按正则表达式匹配队列
        /// AppName.SvcName.*  接收对服务所有副本的投递
        /// #.SvcID  接收对当前副本的投递
        /// </summary>
        /// <param name="p_svcName"></param>
        void CreateTopicConsumer(string p_svcName)
        {
            string queueName = $"{Kit.AppName}.{p_svcName}.{Kit.SvcID}";
            IModel channel = _conn.CreateModel();

            // 声明队列
            channel.QueueDeclare(
                queueName,         // 队列名称
                durable: false,    // 是否持久化
                exclusive: false,  // 是否为排他队列，若排他则只首次连接可见，连接断开时删除
                autoDelete: true); // true时若没有任何订阅者的话，该队列会被自动删除，这种队列适用于临时队列

            // 绑定队列
            channel.QueueBind(
                queue: queueName,          // 队列名称
                exchange: _exchangeName,   // 绑定的交换机
                routingKey: $"{Kit.AppName}.{p_svcName}.*"); // 路由名称
            channel.QueueBind(
                queue: queueName,           // 队列名称
                exchange: _exchangeName,    // 绑定的交换机
                routingKey: $"#.{Kit.SvcID}"); // 路由名称

            // 创建消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, e) =>
            {
                OnConsumeMessage(e);
                channel.BasicAck(e.DeliveryTag, false);
            };

            // 限流的设置
            // 参数一： 0表消息的大小不做任何限制
            // 参数二： 1表服务器给的最大的消息数，这里是一条一条的消费，如果消费者没有确认消费，将不会接受新消息
            // 参数三： false级别为consumer 
            channel.BasicQos(0, 1, false);

            // 要想做限流必须将autoAck设置为false
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            // 异常处理
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
                    Log.Error(ex, $"重建RabbitMQ队列{queueName}时异常！");
                }
            };
        }

        /// <summary>
        /// 订阅系统队列变化事件(queue.*)，用来准确获取所有微服务的副本个数
        /// </summary>
        void CreateQueueChangeConsumer()
        {
            IModel channel = _conn.CreateModel();

            // 创建一个由RabbitMQ 命名的、排他的、自动删除的、非持久化的队列
            var queueName = channel.QueueDeclare().QueueName;

            // 绑定queue.*队列
            channel.QueueBind(
               queue: queueName,
               exchange: "amq.rabbitmq.event",
               routingKey: "queue.*");

            // 队列变化时更新微服务列表
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, e) => Kit.UpdateSvcList();

            channel.BasicConsume(queue: queueName, true, consumer: consumer);
        }

        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="p_args"></param>
        void OnConsumeMessage(BasicDeliverEventArgs p_args)
        {
            if (!string.IsNullOrEmpty(p_args.BasicProperties.CorrelationId))
            {
                // Rpc
                if (!string.IsNullOrEmpty(p_args.BasicProperties.ReplyTo))
                {
                    // 接收Rpc调用
                    new RabbitMQRpcHandler().Process(p_args);
                }
                else
                {
                    // 接收Rpc返回的结果
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