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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 通过 RabbitMQ 获取微服务的方法
    /// </summary>
    public partial class Kit
    {
        static HttpClient _mqClient;

        /// <summary>
        /// 通过RabbitMQ队列，实时获取应用内正在运行的所有微服务
        /// </summary>
        /// <param name="p_isSvcInst">true表示所有微服务副本实例，false表示所有微服务</param>
        /// <returns>微服务列表</returns>
        public static async Task<List<string>> GetAllSvcs(bool p_isSvcInst)
        {
            if (_mqClient == null)
            {
                var cfg = Config.GetSection("RabbitMq");
                if (!cfg.Exists())
                    throw new InvalidOperationException("未找到RabbitMq配置节！");

                _mqClient = new HttpClient();
                // 必须base64编码
                string bsc = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{cfg["UserName"]}:{cfg["Password"]}"));
                _mqClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", bsc);
                // 获取所有队列 /api/queues/
                _mqClient.BaseAddress = new Uri($"http://{cfg["HostName"]}:{cfg["HttpPort"]}/api/queues/");
            }

            List<string> ls = new List<string>();
            try
            {
                using (var response = await _mqClient.GetAsync(default(Uri)))
                {
                    response.EnsureSuccessStatusCode();
                    var data = await response.Content.ReadAsByteArrayAsync();
                    var root = JsonSerializer.Deserialize<JsonElement>(data);
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var elem in root.EnumerateArray())
                        {
                            if (elem.ValueKind == JsonValueKind.Object
                                && elem.TryGetProperty("name", out var name)
                                && name.ValueKind == JsonValueKind.String)
                            {
                                string val = name.GetString();
                                string[] parts = val.Split('.');
                                // 属于当前应用
                                if (parts.Length > 1 && parts[0] == AppName)
                                {
                                    if (parts.Length == 2 && !p_isSvcInst)
                                        ls.Add(val);
                                    else if (parts.Length == 3 && p_isSvcInst)
                                        ls.Add(val);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "获取RabbitMQ所有队列时异常！");
            }
            return ls;
        }
    }
}
