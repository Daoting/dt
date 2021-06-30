#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-10 创建
******************************************************************************/
#endregion

#region 引用命名
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
    /// 基于RabbitMQ的事件总线
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public sealed class RemoteEventBus
    {
        #region 成员变量
        /// <summary>
        /// 键为事件类型名称，值为IRemoteHandler泛型
        /// </summary>
        internal static readonly Dictionary<string, Type> Events = new Dictionary<string, Type>();

        // 交换机名称
        readonly string _exchangeName = Kit.AppName;
        readonly RabbitMQConnection _conn;
        readonly ILogger<RemoteEventBus> _log;
        readonly AsyncLock _mutex = new AsyncLock();
        IModel _chPublish;
        #endregion

        #region 构造方法
        public RemoteEventBus(RabbitMQConnection p_conn, ILogger<RemoteEventBus> p_log)
        {
            _conn = p_conn;
            _log = p_log;
            Init();
        }
        #endregion

        #region 发布
        /// <summary>
        /// 向应用内的所有服务进行广播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_isAllSvcInst">true表示所有服务的所有副本，false表示当服务有多个副本时只投递给其中一个</param>
        public async void Broadcast(IEvent p_event, bool p_isAllSvcInst = true)
        {
            if (p_event == null)
                return;

            List<string> svcs = await Kit.GetAllSvcs(false);
            foreach (var svc in svcs)
            {
                if (p_isAllSvcInst)
                {
                    // 所有服务的所有副本，进入第二队列
                    Publish(p_event, $"{svc}.All", true);
                }
                else
                {
                    // 每个服务只投递给其中一个副本，进入第一队列
                    Publish(p_event, svc, false);
                }
            }
        }

        /// <summary>
        /// 向应用内的多个服务进行广播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcs">服务列表</param>
        /// <param name="p_isAllSvcInst">true表示所有服务的所有副本，false表示当服务有多个副本时只投递给其中一个</param>
        public void Broadcast(IEvent p_event, List<string> p_svcs, bool p_isAllSvcInst = true)
        {
            if (p_event == null || p_svcs == null || p_svcs.Count == 0)
                return;

            if (p_isAllSvcInst)
            {
                // 所有服务的所有副本，进入第二队列
                foreach (var svc in p_svcs)
                {
                    if (!string.IsNullOrEmpty(svc))
                        Publish(p_event, $"{Kit.AppName}.{svc.ToLower()}.All", true);
                }
            }
            else
            {
                // 每个服务只投递给其中一个副本，进入第一队列
                foreach (var svc in p_svcs)
                {
                    if (!string.IsNullOrEmpty(svc))
                        Publish(p_event, $"{Kit.AppName}.{svc.ToLower()}", false);
                }
            }
        }

        /// <summary>
        /// 向某个服务的所有服务副本进行组播
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcName">服务名称，null表示当前服务</param>
        public void Multicast(IEvent p_event, string p_svcName = null)
        {
            // 进入第二队列
            if (p_event != null)
            {
                if (string.IsNullOrEmpty(p_svcName))
                    p_svcName = Kit.SvcName;
                Publish(p_event, $"{Kit.AppName}.{p_svcName.ToLower()}.All", true);
            }
        }

        /// <summary>
        /// 向某个服务发布事件，有多个服务副本时采用均衡算法将消息投递给其中一个
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcName">服务名称</param>
        public void Push(IEvent p_event, string p_svcName)
        {
            // 进入第一队列
            if (p_event != null && !string.IsNullOrEmpty(p_svcName))
                Publish(p_event, $"{Kit.AppName}.{p_svcName.ToLower()}", false);
        }

        /// <summary>
        /// 向某个服务的固定副本发布事件，使用场景少，如在线推送消息，因客户端连接的副本不同
        /// </summary>
        /// <param name="p_event">事件内容</param>
        /// <param name="p_svcID">服务副本ID</param>
        public void PushFixed(IEvent p_event, string p_svcID)
        {
            // 进入第二队列，使用完整名称时会匹配所有副本！
            if (p_event != null && !string.IsNullOrEmpty(p_svcID))
                Publish(p_event, $".{p_svcID}", true);
        }

        /// <summary>
        /// 发布远程事件
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_routingKey"></param>
        /// <param name="p_bindExchange"></param>
        async void Publish(IEvent p_event, string p_routingKey, bool p_bindExchange)
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

                    // 序列化
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

        #region 订阅
        internal static void Subscribe(IServiceProvider p_provider)
        {
            // 单例对象，实例化时进行订阅
            p_provider.GetRequiredService<RemoteEventBus>();
        }

        void Init()
        {
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

            // 声明两个消费者队列
            // 1. 如dt.cm，接收单副本时的直接投递 或 多个服务副本时采用均衡算法投递给其中一个的情况
            CreateWorkConsumer();
            // 2. 如dt.cm.xxx，接收对所有副本广播或按服务组播的情况，因每次重启id不同，队列采用自动删除模式
            CreateTopicConsumer();
        }

        /// <summary>
        /// 声明消费者队列 AppName.SvcName，work模式，未绑定交换机，只支持和队列名称完全匹配时投递
        /// 用于接收单副本时的直接投递 或 多个服务副本时采用均衡算法投递给其中一个的情况
        /// </summary>
        void CreateWorkConsumer()
        {
            string queueName = $"{Kit.AppName}.{Kit.SvcName}";
            IModel channel = _conn.CreateModel();

            // 声明队列
            channel.QueueDeclare(
                queueName,         // 队列名称
                durable: false,    // 是否持久化
                exclusive: false,  // 是否为排他队列，若排他则只首次连接可见，连接断开时删除
                autoDelete: true); // true时若没有任何订阅者的话，该队列会被自动删除，这种队列适用于临时队列

            // 创建消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (s, e) =>
            {
                await ProcessEvent(e);
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
                        CreateWorkConsumer();
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"重建RabbitMQ队列{queueName}时异常！");
                }
            };
        }

        /// <summary>
        /// 声明消费者队列 AppName.SvcName.SvcID，绑定交换机，topic模式，支持按正则表达式匹配队列
        /// AppName.SvcName.*  接收对服务所有副本的投递
        /// #.SvcID  接收对当前副本的投递
        /// </summary>
        void CreateTopicConsumer()
        {
            string queueName = $"{Kit.AppName}.{Kit.SvcName}.{Kit.SvcID}";
            IModel channel = _conn.CreateModel();

            // 声明队列
            channel.QueueDeclare(
                queueName,         // 队列名称
                durable: false,    // 是否持久化
                exclusive: false,  // 是否为排他队列，若排他则只首次连接可见，连接断开时删除
                autoDelete: true); // true时若没有任何订阅者的话，该队列会被自动删除，这种队列适用于临时队列

            // 绑定队列
            channel.QueueBind(
                queue: queueName,        // 队列名称
                exchange: _exchangeName,   // 绑定的交换机
                routingKey: $"{Kit.AppName}.{Kit.SvcName}.*"); // 路由名称
            channel.QueueBind(
                queue: queueName,           // 队列名称
                exchange: _exchangeName,    // 绑定的交换机
                routingKey: $"#.{Kit.SvcID}"); // 路由名称

            // 创建消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (s, e) =>
            {
                await ProcessEvent(e);
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
                        CreateTopicConsumer();
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"重建RabbitMQ队列{queueName}时异常！");
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
                _log.LogWarning(e, "远程事件反序列化时异常！");
                return;
            }

            // 获取IRemoteHandler泛型，不存在表示无Handler
            Type hType;
            if (string.IsNullOrEmpty(body.EventName)
                || !Events.TryGetValue(body.EventName, out hType))
                return;

            // 反序列化事件对象
            object eventObj;
            try
            {
                eventObj = JsonSerializer.Deserialize(body.Data, hType.GetGenericArguments()[0], JsonOptions.UnsafeSerializer);
            }
            catch (Exception e)
            {
                _log.LogWarning(e, "远程事件反序列化时异常！");
                return;
            }

            // 组播时排除的副本
            if (eventObj is ExcludeEvent ee && ee.ExcludeSvcID == Kit.SvcID)
                return;

            // 实例化所有Handler
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
                    _log.LogWarning(e, $"{h.GetType().Name}处理远程事件时异常！");
                }
            }
        }
        #endregion
    }
}