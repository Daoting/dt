#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 通过 RabbitMQ 获取微服务列表
    /// </summary>
    class AppSvcList
    {
        const int _minInterval = 180;
        readonly AsyncLocker _locker = new AsyncLocker();
        readonly List<string> _allSvcs = new List<string>();
        readonly List<string> _allSvcInsts = new List<string>();
        HttpClient _mqClient;
        DateTime _lastUpateSvcTime;

        /// <summary>
        /// 通过RabbitMQ队列，获取应用内正在运行的所有微服务列表
        /// </summary>
        /// <param name="p_isSvcInst">true表示所有微服务副本实例，false表示所有微服务</param>
        /// <returns>微服务列表</returns>
        public async Task<List<string>> GetAllSvcs(bool p_isSvcInst)
        {
            await UpdateSvcList();
            return p_isSvcInst ? _allSvcInsts : _allSvcs;
        }

        /// <summary>
        /// 通过RabbitMQ队列，获取应用内正在运行的某微服务的副本个数
        /// </summary>
        /// <param name="p_svcName">服务名称，null表示当前服务</param>
        /// <returns>副本个数</returns>
        public async Task<int> GetReplicaCount(string p_svcName)
        {
            await UpdateSvcList();

            if (string.IsNullOrEmpty(p_svcName))
                p_svcName = Kit.SvcName;
            string queueName = $"{Kit.AppName}.{p_svcName}";
            return (from name in _allSvcInsts
                    where name.StartsWith(queueName)
                    select name).Count();
        }

        async Task UpdateSvcList()
        {
            // 超过最小间隔刷新，RabbitMQ队列个数变化时无法同步获取
            // 实时刷新又太耗资源，当前方式为折中方案，可能造成列表不准确！
            if ((DateTime.Now - _lastUpateSvcTime).TotalSeconds < _minInterval)
                return;

            using (await _locker.LockAsync())
            {
                // 可能刚刷新完毕！
                if ((DateTime.Now - _lastUpateSvcTime).TotalSeconds < _minInterval)
                    return;

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
                    _lastUpateSvcTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "获取RabbitMQ所有队列时异常！");
                }
            }
        }
    }
}
