#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
#endregion

namespace Dt.App
{
    /// <summary>
    /// Msix文件配置相关
    /// </summary>
    public static class Cfg
    {
        #region 成员变量
        static IConfiguration _config;
        static IDisposable _cfgCallback;
        #endregion

        /// <summary>
        /// windows应用msix安装包的版本信息
        /// </summary>
        public static Dict WinAppVer { get; set; }

        /// <summary>
        /// app安装包存放目录
        /// </summary>
        public static string PackageDir { get; private set; }

        /// <summary>
        /// wasm app的虚拟目录
        /// </summary>
        public const string WasmVirPath = "/app";

        /// <summary>
        /// wasm app网站静态文件存放目录
        /// </summary>
        public static string WasmDir { get; private set; }

        /// <summary>
        /// 初始化服务配置
        /// </summary>
        public static void Init()
        {
            if (!File.Exists(Path.Combine(Kit.PathBase, "etc/config/app.json")))
            {
                var ex = new Exception("缺少 app.json 文件！");
                Log.Fatal(ex, "app服务启动出错");
                throw ex;
            }

            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Kit.PathBase, "etc/config"))
                    .AddJsonFile("app.json", false, true)
                    .Build();

                // json文件修改后重新加载
                _cfgCallback = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, null);
                ApplyConfig();
                Log.Information("读取 app.json 成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取 app.json 失败！");
                throw;
            }

            PackageDir = Path.Combine(Kit.PathBase, "package");
            if (!Directory.Exists(PackageDir))
                Directory.CreateDirectory(PackageDir);

            WasmDir = _config.GetValue<string>("WasmPath");
            if (!Path.IsPathRooted(WasmDir))
            {
                // 相对路径
                WasmDir = Path.Combine(Kit.PathBase, WasmDir);
            }
            if (!Directory.Exists(WasmDir))
                Directory.CreateDirectory(WasmDir);
        }

        /// <summary>
        /// 系统配置(json文件)修改事件
        /// </summary>
        /// <param name="p_state"></param>
        static void OnConfigChanged(object p_state)
        {
            ApplyConfig();

            // 每次修改后需要重新注册，立即注册又会连续触发两次！
            _cfgCallback?.Dispose();
            Task.Delay(2000).ContinueWith((s) => _cfgCallback = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, null));
        }

        static void ApplyConfig()
        {
            WinAppVer = new Dict
            {
                { "ver", _config.GetValue("WinApp:Version", "0.0.0.0") },
                { "force", _config.GetValue("WinApp:ForceUpdate", false) },
                { "file", _config.GetValue("WinApp:File", "Dt") }
            };
        }
    }
}
