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
        readonly string _exchangeName = Glb.AppName;
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
        public void Broadcast(IEvent p_event, bool p_isAllSvcInst = true)
        {
            if (p_isAllSvcInst)
            {
                // 所有服务的所有副本，进入第二队列
                Publish(p_event, Glb.AppName + ".*.*");
            }
            else
            {
                // 每个服务只投递给其中一个副本，进入第一队列
                Publish(p_event, Glb.AppName + ":*");
            }
        }

        /// <summary>
        /// 向某个服务的所有服务副本进行组播
        /// </summary>
        public void Multicast(IEvent p_event, string p_svcName)
        {
            // 进入第二队列
            if (!string.IsNullOrEmpty(p_svcName))
                Publish(p_event, $"{Glb.AppName}.{p_svcName.ToLower()}.*");
        }

        /// <summary>
        /// 向某个服务发布事件，有多个服务副本时采用均衡算法将消息投递给其中一个
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_svcName"></param>
        public void Push(IEvent p_event, string p_svcName)
        {
            // 进入第一队列
            if (!string.IsNullOrEmpty(p_svcName))
                Publish(p_event, $"{Glb.AppName}:{p_svcName.ToLower()}");
        }

        /// <summary>
        /// 发布远程事件
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

                    // 序列化
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
            // 1. 如dt:cm，接收单副本时的直接投递 或 多个服务副本时采用均衡算法投递给其中一个的情况
            CreateConsumerChannel($"{Glb.AppName}:{Glb.SvcName}", false, $"{Glb.AppName}:*");
            // 2. 如dt.cm.xxx，接收对所有副本广播或按服务组播的情况，因每次重启id不同，队列采用自动删除模式
            CreateConsumerChannel($"{Glb.AppName}.{Glb.SvcName}.{Glb.ID}", true, $"{Glb.AppName}.*.*");
        }

        void CreateConsumerChannel(string p_queueName, bool p_autoDelete, string p_routingKey)
        {
            IModel channel = _conn.CreateModel();

            // 声明队列
            channel.QueueDeclare(
                p_queueName,        // 队列名称
                durable: true,      // 是否持久化
                exclusive: false,   // 是否为排他队列，若排他则只首次连接可见，连接断开时删除
                autoDelete: p_autoDelete); // true时若没有任何订阅者的话，该队列会被自动删除，这种队列适用于临时队列

            // 绑定队列
            channel.QueueBind(
                queue: p_queueName,        // 队列名称
                exchange: _exchangeName,   // 绑定的交换机
                routingKey: p_routingKey); // 路由名称

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
            channel.BasicConsume(queue: p_queueName, autoAck: false, consumer: consumer);

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
                        CreateConsumerChannel(p_queueName, p_autoDelete, p_routingKey);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "重建RabbitMQ消费者通道时异常！");
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
                eventObj = JsonConvert.DeserializeObject(body.Data, hType.GetGenericArguments()[0]);
            }
            catch (Exception e)
            {
                _log.LogWarning(e, "远程事件反序列化时异常！");
                return;
            }

            // 实例化所有Handler
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
                    _log.LogWarning(e, $"{h.GetType().Name}处理远程事件时异常！");
                }
            }
        }
        #endregion
    }
}