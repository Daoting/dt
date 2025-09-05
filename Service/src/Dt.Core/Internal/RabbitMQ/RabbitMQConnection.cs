#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
using Nito.AsyncEx;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
#endregion

namespace Dt.Core.RabbitMQ
{
    /// <summary>
    /// 管理与RabbitMQ的连接，单例
    /// </summary>
    class RabbitMQConnection : IDisposable
    {
        #region 成员变量
        readonly IConnectionFactory _connectionFactory;
        readonly AsyncLock _mutex = new AsyncLock();
        IConnection _connection;
        bool _disposed;
        #endregion

        #region 构造方法
        public RabbitMQConnection()
        {
            var cfg = Kit.Config.GetSection("RabbitMq");
            if (!cfg.Exists())
                throw new InvalidOperationException("未找到RabbitMq配置节！");

            // RabbitMQ连接配置
            _connectionFactory = new ConnectionFactory()
            {
                HostName = cfg["HostName"],
                UserName = cfg["UserName"],
                Password = cfg["Password"],
                Port = cfg.GetValue<int>("Port"),
            };
        }
        #endregion

        /// <summary>
        /// 是否已连接RabbitMQ
        /// </summary>
        public bool IsConnected
        {
            get { return _connection != null && _connection.IsOpen && !_disposed; }
        }

        /// <summary>
        /// 执行连接
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryConnect()
        {
            using (await _mutex.LockAsync())
            {
                // 出现异常时重试5次，每次间隔时间增加
                var options = new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<SocketException>().Handle<BrokerUnreachableException>(),
                    MaxRetryAttempts = 3,
                    DelayGenerator = static args =>
                    {
                        return new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(Math.Pow(2, args.AttemptNumber)));
                    },
                    OnRetry = args =>
                    {
                        if (args.Outcome.Exception is Exception ex)
                        {
                            Log.Warning($"RabbitMQ第 {args.AttemptNumber + 1} 次连接失败：{ex.Message}");
                            if (args.AttemptNumber + 1 == 3)
                                Log.Error("重试3次，连接 RabbitMQ 失败！");
                        }
                        return ValueTask.CompletedTask;
                    }
                };
                var pipeline = new ResiliencePipelineBuilder()
                    .AddRetry(options)
                    .Build();

                _connection = await pipeline.ExecuteAsync(async token => await _connectionFactory.CreateConnectionAsync());
                
                if (IsConnected)
                {
                    _connection.ConnectionShutdownAsync += OnConnectionShutdown;
                    _connection.CallbackExceptionAsync += OnCallbackException;
                    _connection.ConnectionBlockedAsync += OnConnectionBlocked;

                    Log.Information("RabbitMQ 连接成功");
                    return true;
                }

                return false;
            }
        }
        
        /// <summary>
        /// 创建通道
        /// </summary>
        /// <returns></returns>
        public Task<IChannel> CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("未创建RabbitMQ连接！");
            return _connection.CreateChannelAsync();;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                _disposed = true;
                _connection.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RabbitMQ 关闭连接异常");
            }
        }

        async Task OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (!_disposed)
            {
                Log.Warning("RabbitMQ 连接关闭，正在重连...");
                await TryConnect();
            }
        }

        async Task OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (!_disposed)
            {
                Log.Warning("RabbitMQ 连接异常，正在重连...");
                await TryConnect();
            }
        }

        async Task OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (!_disposed)
            {
                Log.Warning("RabbitMQ 连接关闭，正在重连...");
                await TryConnect();
            }
        }
    }
}