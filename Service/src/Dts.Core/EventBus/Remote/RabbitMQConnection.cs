#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-10 ����
******************************************************************************/
#endregion

#region ��������
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

namespace Dts.Core.EventBus
{
    /// <summary>
    /// ������RabbitMQ�����ӣ�����
    /// </summary>
    [Svc(ServiceLifetime.Singleton)]
    public class RabbitMQConnection : IDisposable
    {
        #region ��Ա����
        readonly IConnectionFactory _connectionFactory;
        readonly ILogger<RabbitMQConnection> _log;
        readonly object _connSignal = new object();
        IConnection _connection;
        bool _disposed;
        #endregion

        #region ���췽��
        public RabbitMQConnection(ILogger<RabbitMQConnection> p_log)
        {
            _log = p_log;

            var cfg = Glb.GetCfg<string>("rabbitmq");
            if (string.IsNullOrWhiteSpace(cfg))
                throw new InvalidOperationException("δ�ҵ�RabbitMq�����ã�");

            // RabbitMQ��������
            var fac = new ConnectionFactory();
            var arr = cfg.Split(',');
            foreach (var paddedOption in arr)
            {
                var option = paddedOption.Trim();
                if (string.IsNullOrWhiteSpace(option))
                    continue;

                int idx = option.IndexOf('=');
                if (idx <= 0)
                    continue;

                var key = option.Substring(0, idx).Trim().ToLower();
                var value = option.Substring(idx + 1).Trim();
                switch (key)
                {
                    case "hostname":
                        fac.HostName = value;
                        break;
                    case "username":
                        fac.UserName = value;
                        break;
                    case "password":
                        fac.Password = value;
                        break;
                    case "port":
                        if (int.TryParse(value, out int port))
                            fac.Port = port;
                        break;
                }
            }
            _connectionFactory = fac;
        }
        #endregion

        /// <summary>
        /// �Ƿ�������RabbitMQ
        /// </summary>
        public bool IsConnected
        {
            get { return _connection != null && _connection.IsOpen && !_disposed; }
        }

        /// <summary>
        /// ִ������
        /// </summary>
        /// <returns></returns>
        public bool TryConnect()
        {
            lock (_connSignal)
            {
                // �����쳣ʱ����5�Σ�ÿ�μ��ʱ������
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

                    _log.LogInformation("����RabbitMQ�ɹ�");
                    return true;
                }

                _log.LogCritical("����5�Σ�����RabbitMQʧ�ܣ�");
                return false;
            }
        }

        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <returns></returns>
        public IModel CreateModel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("δ����RabbitMQ���ӣ�");
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