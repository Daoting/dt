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

namespace Dt.Fsm
{
    /// <summary>
    /// Msix文件配置相关
    /// </summary>
    public static class MsixCfg
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
        /// 初始Msix文件配置
        /// </summary>
        public static void Init()
        {
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "etc/config/msix.json")))
            {
                Log.Warning("缺少msix.json文件，无msix安装包版本信息");
                WinAppVer = new Dict { { "ver", "0.0.0.0" }, { "force", false }, { "file", "Dt.Shell.Win" } };
                return;
            }

            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "etc/config"))
                    .AddJsonFile("msix.json", false, true)
                    .Build();

                // json文件修改后重新加载
                _cfgCallback = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, null);
                ApplyConfig();
                Log.Information("读取msix安装包信息成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "读取 msix.json 失败！");
                throw;
            }
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
                { "ver", _config.GetValue("Version", "0.0.0.0") },
                { "force", _config.GetValue("ForceUpdate", false) },
                { "file", _config.GetValue("File", "Dt") }
            };
        }
    }
}
