#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;
#endregion

namespace Dt.Core.EventBus
{
    /// <summary>
    /// 管理与RabbitMQ的连接，单例
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public class RabbitMQConnection : IDisposable
    {
        #region 成员变量
        readonly IConnectionFactory _connectionFactory;
        readonly ILogger<RabbitMQConnection> _log;
        readonly object _connSignal = new object();
        IConnection _connection;
        bool _disposed;
        #endregion

        #region 构造方法
        public RabbitMQConnection(ILogger<RabbitMQConnection> p_log)
        {
            _log = p_log;

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
        public bool TryConnect()
        {
            lock (_connSignal)
            {
                // 出现异常时重试5次，每次间隔时间增加
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(
                        5,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) => _log.LogWarning(ex.Message));

                _connection = policy.Execute(() => _connectionFactory.CreateConnection());

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _log.LogInformation("RabbitMQ Connected");
                    return true;
                }

                _log.LogCritical("重试5次，连接 RabbitMQ 失败！");
                return false;
            }
        }

        /// <summary>
        /// 创建通道
        /// </summary>
        /// <returns></returns>
        public IModel CreateModel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("未创建RabbitMQ连接！");
            return _connection.CreateModel();
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
            catch (IOException ex)
            {
                _log.LogCritical(ex.ToString());
            }
        }

        void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _log.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _log.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _log.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }
    }
}