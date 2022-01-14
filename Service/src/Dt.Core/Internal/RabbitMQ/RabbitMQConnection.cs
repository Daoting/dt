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
using Serilog;
using System;
using System.IO;
using System.Net.Sockets;
#endregion

namespace Dt.Core.RabbitMQ
{
    /// <summary>
    /// ������RabbitMQ�����ӣ�����
    /// </summary>
    class RabbitMQConnection : IDisposable
    {
        #region ��Ա����
        readonly IConnectionFactory _connectionFactory;
        readonly object _connSignal = new object();
        IConnection _connection;
        bool _disposed;
        #endregion

        #region ���췽��
        public RabbitMQConnection()
        {
            var cfg = Kit.Config.GetSection("RabbitMq");
            if (!cfg.Exists())
                throw new InvalidOperationException("δ�ҵ�RabbitMq���ýڣ�");

            // RabbitMQ��������
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
                        (ex, time) => Log.Warning(ex.Message));

                _connection = policy.Execute(() => _connectionFactory.CreateConnection());

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    Log.Information("RabbitMQ ���ӳɹ�");
                    return true;
                }

                Log.Error("����5�Σ����� RabbitMQ ʧ�ܣ�");
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
            catch (Exception ex)
            {
                Log.Error(ex, "RabbitMQ �ر������쳣");
            }
        }

        void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            Log.Warning("RabbitMQ ���ӹرգ���������...");
            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            Log.Warning("RabbitMQ �����쳣����������...");
            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            Log.Warning("RabbitMQ ���ӹرգ���������...");
            TryConnect();
        }
    }
}