#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
using Serilog.Extensions.ElapsedTime;
using Serilog.Formatting.Compact;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 启动微服务
    /// </summary>
    public static partial class Launcher
    {
        /// <summary>
        /// 初始化服务，任一环节失败即启动失败
        /// + 确定基础路径
        /// + 创建日志对象
        /// + 读取配置
        /// + 整理服务存根
        /// + 启动http服务器
        /// 此方法不可异步，否则启动有问题！！！
        /// </summary>
        /// <param name="p_args">命令行参数，基础路径，空时dll所在路径为基础路径</param>
        /// <param name="p_allStubs">所有可用的服务存根对象</param>
        public static void Run(string[] p_args, Dictionary<string, Stub> p_allStubs)
        {
            var starter = new WebHostStarter(p_args);
            starter.SetPathBase();
            CreateLogger();
            LoadConfig();
            if ("InitDb".Equals(Kit.GetCfg<string>("SvcName"), StringComparison.OrdinalIgnoreCase))
            {
                // 进入初始化数据库模式
                starter.RunInitMode();
            }
            else
            {
                LoadSvcs(p_allStubs);
                starter.Run();
            }
            Log.CloseAndFlush();
        }

        /// <summary>
        /// 创建日志对象
        /// </summary>
        static void CreateLogger()
        {
            try
            {
                // 支持动态调整
                var cfg = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("logger.json", false, true)
                    .Build();

                // 日志文件命名：
                // log-服务实例ID-日期.txt，避免部署在k8s挂载宿主目录时文件名重复
                string path = Path.Combine(Kit.PathBase, "etc/log", $"log-{Kit.SvcID}-.txt");
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(cfg)
                    .WriteTo.Html()
                    // 输出json文件，默认最大1G、最多保存31个文件、实时写、文件独占方式
                    .WriteTo.File(
                        new CompactJsonFormatter(),
                        path,
                        rollingInterval: RollingInterval.Day, // 文件名末尾加日期
                        rollOnFileSizeLimit: true) // 超过1G时新文件名末尾加序号
                    .WriteTo.Console()
                    .WithElapsed<Kit>()
                    .CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine($"创建日志对象失败：{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        static void LoadConfig()
        {
            try
            {
                var cfg = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("service.json", false, true)
                    .AddJsonFile("global.json", false, true)
                    .Build();
                Kit.InitConfig(cfg);
                Log.Information("读取配置成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取配置失败！");
                throw;
            }
        }

        /// <summary>
        /// 加载服务
        /// </summary>
        /// <param name="p_allStubs">所有可用的服务存根对象</param>
        static void LoadSvcs(Dictionary<string, Stub> p_allStubs)
        {
            if (p_allStubs == null || p_allStubs.Count == 0)
            {
                LogException("未提供任何服务存根对象！");
            }
            
            // 服务名
            var svcName = Kit.GetCfg<string>("SvcName", "").Trim().ToLower();

            var svcs = new SvcList();
            if (svcName != "")
            {
                // 外部微服务
                if (!p_allStubs.TryGetValue(svcName, out var stub))
                {
                    LogException($"服务 {svcName} 不存在！");
                }
                svcs.Add(new SvcInfo(svcName, stub));
            }
            else
            {
                // 单体服务模式，所有微服务聚集成一个服务
                foreach (var item in p_allStubs)
                {
                    svcs.Add(new SvcInfo(item.Key, item.Value));
                }
            }
            Kit.Svcs = svcs;

            // 完善每个微服务的数据源jian
            foreach (var svc in Kit.Svcs)
            {
                string file = svc.SvcName + ".json";
                string path = Path.Combine(Kit.PathBase, "etc/config", file);

                if (!File.Exists(path))
                {
                    // 无配置文件，使用默认数据源
                    svc.DbInfo = Kit.DefaultDbInfo;
                    continue;
                }

                string dbKey = null;
                try
                {
                    var cfg = new ConfigurationBuilder()
                        .AddJsonFile(path, false, false)
                        .Build();
                    dbKey = cfg["DbKey"];
                }
                catch
                {
                    svc.DbInfo = Kit.DefaultDbInfo;
                    continue;
                }

                if (string.IsNullOrEmpty(dbKey))
                {
                    // 无 DbKey 配置，使用默认数据源
                    svc.DbInfo = Kit.DefaultDbInfo;
                }
                else if (Kit.AllDbInfo.TryGetValue(dbKey, out var di))
                {
                    svc.DbInfo = di;
                }
                else
                {
                    svc.DbInfo = Kit.DefaultDbInfo;
                    Log.Warning($"{file} 中的数据源键名 {dbKey} 无连接串配置，使用默认数据源！");
                }
            }

            // 单体不启用 RabbitMQ
            Kit.EnableRabbitMQ = Kit.Svcs.Count == 1;
            Log.Information($"启动 {Kit.AllSvcName}，版本：{Kit.GetSysVersion()}");
        }
        
        static void LogException(string p_msg)
        {
            Log.Fatal(p_msg);
            throw new Exception(p_msg);
        }
    }
}