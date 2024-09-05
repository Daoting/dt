#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统相关
    /// </summary>
    public partial class Kit
    {
        #region 成员变量
        static TimeSpan _timeSpan;
        static IConfiguration _config;
        static IDisposable _cfgCallback;
        static IServiceProvider _svcProvider;
        static IHttpContextAccessor _accessor;
        #endregion

        #region 属性
        /// <summary>
        /// 获取所有服务存根
        /// </summary>
        public static Stub[] Stubs { get; set; }

        /// <summary>
        /// 获取服务实例ID，k8s部署在同一Node上多个Pod副本时区分用，每次启动生成新ID，终生不变
        /// </summary>
        public static string SvcID { get; } = Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// 获取所有服务名称，小写
        /// </summary>
        public static IEnumerable<string> SvcNames => from stub in Stubs
                                                      select stub.SvcName;

        /// <summary>
        /// 是否为单体服务(所有微服务合并成一个服务)
        /// </summary>
        public static bool IsSingletonSvc => Stubs.Length > 1;

        /// <summary>
        /// 获取系统配置
        /// </summary>
        public static IConfiguration Config
        {
            get { return _config; }
            internal set
            {
                if (value != null)
                {
                    _config = value;
                    ApplyConfig();
                    _cfgCallback = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, null);
                }
            }
        }

        /// <summary>
        /// 获取当前请求的HttpContext
        /// </summary>
        public static HttpContext HttpContext => _accessor.HttpContext;

        /// <summary>
        /// 获取数据库服务器的当前时间，根据时差计算所得
        /// </summary>
        public static DateTime Now
        {
            get { return DateTime.Now + _timeSpan; }
            internal set { _timeSpan = value - DateTime.Now; }
        }

        /// <summary>
        /// 获取应用名称
        /// </summary>
        public static string AppName
        {
            get { return _config.GetValue("App", "dt"); }
        }

        /// <summary>
        /// 是否启用RabbitMQ
        /// </summary>
        public static bool EnableRabbitMQ { get; internal set; }

        /// <summary>
        /// 服务是否运行在Docker容器
        /// </summary>
        public static bool IsInDocker
        {
            get { return RuntimeInformation.IsOSPlatform(OSPlatform.Linux); }
        }

        /// <summary>
        /// 是否输出所有运行中的Sql语句
        /// </summary>
        public static bool TraceSql { get; set; }
        #endregion

        #region 系统配置
        /// <summary>
        /// 系统配置变化事件
        /// </summary>
        public static event Action ConfigChanged;

        /// <summary>
        /// 获取系统配置中指定键的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="p_key">键名</param>
        /// <param name="p_defaultValue">键不存在时的默认值</param>
        /// <returns></returns>
        public static T GetCfg<T>(string p_key, T p_defaultValue)
        {
            return _config.GetValue<T>(p_key, p_defaultValue);
        }

        /// <summary>
        /// 获取系统配置中指定键的值
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="p_key">键名</param>
        /// <returns></returns>
        public static T GetCfg<T>(string p_key)
        {
            return _config.GetValue<T>(p_key);
        }

        /// <summary>
        /// 系统配置(json文件)修改事件
        /// </summary>
        /// <param name="p_state"></param>
        static void OnConfigChanged(object p_state)
        {
            ApplyConfig();
            ConfigChanged?.Invoke();

            // 每次修改后需要重新注册，立即注册又会连续触发两次！
            _cfgCallback?.Dispose();
            Task.Delay(2000).ContinueWith((s) => _cfgCallback = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, null));
        }

        static void ApplyConfig()
        {
            InitDbInfo();
            TraceSql = _config.GetValue("TraceSql", false);
            RpcHandler.TraceRpc = _config.GetValue("TraceRpc", false);
        }

        /// <summary>
        /// 设置Boot服务的配置
        /// </summary>
        /// <param name="p_config"></param>
        internal static void SetBootConfig(IConfigurationRoot p_config)
        {
            _config = p_config;
        }
        #endregion

        #region 全局服务对象
        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时返回null，不抛异常
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务对象</returns>
        public static T GetService<T>() => _svcProvider.GetService<T>();

        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时抛异常
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务对象</returns>
        public static T GetRequiredService<T>() => _svcProvider.GetRequiredService<T>();

        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时返回null，不抛异常
        /// </summary>
        /// <param name="p_svcType"></param>
        /// <returns>服务对象</returns>
        public static object GetService(Type p_svcType) => _svcProvider.GetService(p_svcType);

        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时抛异常
        /// </summary>
        /// <param name="p_svcType"></param>
        /// <returns>服务对象</returns>
        public static object GetRequiredService(Type p_svcType) => _svcProvider.GetRequiredService(p_svcType);

        /// <summary>
        /// 在全局服务容器中获取指定类型的所有服务对象
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>所有服务对象</returns>
        public static IEnumerable<T> GetServices<T>() => _svcProvider.GetServices<T>();

        /// <summary>
        /// 在全局服务容器中获取指定类型的所有服务对象
        /// </summary>
        /// <param name="p_svcType">服务类型</param>
        /// <returns>所有服务对象</returns>
        public static IEnumerable<object> GetServices(Type p_svcType) => _svcProvider.GetServices(p_svcType);
        #endregion

        #region Startup
        internal static void ConfigureServices(IServiceCollection p_services)
        {
            // 外部
            if (Stubs != null)
            {
                foreach (var stub in Stubs)
                {
                    stub.ConfigureServices(p_services);
                }
            }

            // 以便访问当前的HttpContext
            p_services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        internal static void Configure(IApplicationBuilder p_app)
        {
            // 全局服务容器
            _svcProvider = p_app.ApplicationServices;
            _accessor = _svcProvider.GetRequiredService<IHttpContextAccessor>();

            if (Stubs != null)
            {
                foreach (var stub in Stubs)
                {
                    stub.Configure(p_app, DtMiddleware.RequestHandlers);
                }
            }
        }
        #endregion
    }
}
