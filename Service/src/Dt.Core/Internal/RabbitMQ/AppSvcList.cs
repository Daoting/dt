#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Timer = System.Timers.Timer;
#endregion

namespace Dt.Core.RabbitMQ
{
    /// <summary>
    /// 通过 RabbitMQ 获取微服务列表
    /// </summary>
    class AppSvcList
    {
        #region 成员变量
        readonly List<string> _allSvcs;
        readonly List<string> _allSvcInsts;
        readonly Timer _timer;
        HttpClient _mqClient;
        bool _updating;
        #endregion

        public AppSvcList()
        {
            if (!Kit.EnableRabbitMQ)
                throw new Exception("未启用 RabbitMQ，无法创建 AppSvcList");
            
            _allSvcs = new List<string>();
            _allSvcInsts = new List<string>();
            _timer = new Timer(5000);
            _timer.Elapsed += OnUpdateSvcList;
        }

        /// <summary>
        /// 通过RabbitMQ队列，获取应用内正在运行的所有微服务列表
        /// </summary>
        /// <param name="p_isSvcInst">true表示所有微服务副本实例，false表示所有微服务</param>
        /// <returns>微服务列表</returns>
        public List<string> GetAllSvcs(bool p_isSvcInst)
        {
            return p_isSvcInst ? _allSvcInsts : _allSvcs;
        }

        /// <summary>
        /// 获取当前服务的其他副本实例的id列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetOtherReplicaIDs()
        {
            string queueName = $"{Kit.AppName}.{Kit.Svcs[0].SvcName}";
            return from name in _allSvcInsts
                   where name.StartsWith(queueName) && !name.EndsWith(Kit.SvcID)
                   select name.Substring(queueName.Length + 1);
        }

        /// <summary>
        /// 获取某微服务的所有副本实例的id列表
        /// </summary>
        /// <param name="p_svcName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetReplicaIDs(string p_svcName)
        {
            if (string.IsNullOrEmpty(p_svcName))
                p_svcName = Kit.Svcs[0].SvcName;

            string queueName = $"{Kit.AppName}.{p_svcName}";
            return from name in _allSvcInsts
                   where name.StartsWith(queueName)
                   select name.Substring(queueName.Length + 1);
        }

        /// <summary>
        /// 通过RabbitMQ队列，获取应用内正在运行的某微服务的副本个数
        /// </summary>
        /// <param name="p_svcName">服务名称，null表示当前服务</param>
        /// <returns>副本个数</returns>
        public int GetReplicaCount(string p_svcName)
        {
            if (string.IsNullOrEmpty(p_svcName))
                p_svcName = Kit.Svcs[0].SvcName;

            string queueName = $"{Kit.AppName}.{p_svcName}";
            return (from name in _allSvcInsts
                    where name.StartsWith(queueName)
                    select name).Count();
        }

        /// <summary>
        /// 更新微服务列表
        /// </summary>
        public void Update()
        {
            // 5秒之内多次调用时，前面的调用取消，从最后一次调用重新计时
            // 避免冗余的连续更新，因每个服务声明两个队列！
            _timer.Stop();
            _timer.Start();
        }

        async void OnUpdateSvcList(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            if (_updating)
                return;

            Log.Information("已更新服务列表 — RabbitMQ");
            _updating = true;
            if (_mqClient == null)
            {
                var cfg = Kit.Config.GetSection("RabbitMq");
                if (!cfg.Exists())
                    throw new InvalidOperationException("未找到RabbitMq配置节！");

                _mqClient = new HttpClient();
                // 必须base64编码
                string bsc = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{cfg["UserName"]}:{cfg["Password"]}"));
                _mqClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", bsc);
                // 获取所有队列 /api/queues/
                _mqClient.BaseAddress = new Uri($"http://{cfg["HostName"]}:{cfg["HttpPort"]}/api/queues/");
            }

            _allSvcs.Clear();
            _allSvcInsts.Clear();

            try
            {
                byte[] data;
                using (var response = await _mqClient.GetAsync(default(Uri)))
                {
                    response.EnsureSuccessStatusCode();
                    data = await response.Content.ReadAsByteArrayAsync();
                }

                var root = JsonSerializer.Deserialize<JsonElement>(data);
                if (root.ValueKind != JsonValueKind.Array)
                    throw new Exception("RabbitMQ返回的队列json反序列化异常！");

                foreach (var elem in root.EnumerateArray())
                {
                    if (elem.ValueKind == JsonValueKind.Object
                        && elem.TryGetProperty("name", out var name)
                        && name.ValueKind == JsonValueKind.String)
                    {
                        string val = name.GetString();
                        string[] parts = val.Split('.');
                        // 属于当前应用
                        if (parts.Length > 1 && parts[0] == Kit.AppName)
                        {
                            if (parts.Length == 2)
                                _allSvcs.Add(val);
                            else if (parts.Length == 3)
                                _allSvcInsts.Add(val);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "获取RabbitMQ所有队列时异常！");
            }
            finally
            {
                _updating = false;
            }
        }
    }
}
