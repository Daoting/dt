#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2019-06-10 ����
******************************************************************************/
#endregion

#region ��������
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
    /// ������RabbitMQ�����ӣ�����
    /// </summary>
    class RabbitMQConnection : IDisposable
    {
        #region ��Ա����
        readonly IConnectionFactory _connectionFactory;
        readonly AsyncLock _mutex = new AsyncLock();
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
        public async Task<bool> TryConnect()
        {
            using (await _mutex.LockAsync())
            {
                // �����쳣ʱ����5�Σ�ÿ�μ��ʱ������
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
                            Log.Warning($"RabbitMQ�� {args.AttemptNumber + 1} ������ʧ�ܣ�{ex.Message}");
                            if (args.AttemptNumber + 1 == 3)
                                Log.Error("����3�Σ����� RabbitMQ ʧ�ܣ�");
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

                    Log.Information("RabbitMQ ���ӳɹ�");
                    return true;
                }

                return false;
            }
        }
        
        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <returns></returns>
        public Task<IChannel> CreateChannel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("δ����RabbitMQ���ӣ�");
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
                Log.Error(ex, "RabbitMQ �ر������쳣");
            }
        }

        async Task OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (!_disposed)
            {
                Log.Warning("RabbitMQ ���ӹرգ���������...");
                await TryConnect();
            }
        }

        async Task OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (!_disposed)
            {
                Log.Warning("RabbitMQ �����쳣����������...");
                await TryConnect();
            }
        }

        async Task OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (!_disposed)
            {
                Log.Warning("RabbitMQ ���ӹرգ���������...");
                await TryConnect();
            }
        }
    }
}