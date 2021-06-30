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
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统相关
    /// </summary>
    public partial class Kit
    {
        #region 成员变量
        static ISvcStub _stub;
        static TimeSpan _timeSpan;
        static IConfiguration _config;
        static IDisposable _cfgCallback;
        static IServiceProvider _svcProvider;
        static IHttpContextAccessor _accessor;
        #endregion

        #region 属性
        /// <summary>
        /// 获取服务名称，取自服务程序集命名空间的末尾段，小写
        /// </summary>
        public static string SvcName { get; private set; }

        /// <summary>
        /// 获取服务实例ID，k8s部署在同一Node上多个Pod副本时区分用，每次启动生成新ID，终生不变
        /// </summary>
        public static string SvcID { get; } = Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// 获取服务存根
        /// </summary>
        public static ISvcStub Stub
        {
            get { return _stub; }
            internal set
            {
                _stub = value;
                // 服务名称
                var ns = _stub.GetType().Namespace;
                SvcName = ns.Substring(ns.LastIndexOf(".") + 1).ToLower();
            }
        }

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
        /// 查询Sql语句，默认从缓存字典中查询，service.json中CacheSql为false时直接从表xxx_sql查询！
        /// <para>输入参数为键名(无空格) 或 Sql语句，含空格时不需查询，直接返回Sql语句</para>
        /// </summary>
        public static Func<string, string> Sql { get; internal set; }

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
        /// 服务是否运行在Docker容器
        /// </summary>
        public static bool IsInDocker
        {
            get { return RuntimeInformation.IsOSPlatform(OSPlatform.Linux); }
        }
        #endregion

        #region 系统配置
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
            Silo.OnConfigChanged();
            ApplyConfig();

            // 每次修改后需要重新注册，立即注册又会连续触发两次！
            _cfgCallback?.Dispose();
            Task.Delay(2000).ContinueWith((s) => _cfgCallback = _config.GetReloadToken().RegisterChangeCallback(OnConfigChanged, null));
        }

        static void ApplyConfig()
        {
            MySqlAccess.DefaultConnStr = _config["MySql:" + _config["DbConn"]];
            MySqlAccess.TraceSql = _config.GetValue("TraceSql", false);
            RpcHandler.TraceRpc = _config.GetValue("TraceRpc", false);
        }
        #endregion

        #region 全局服务对象
        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时异常
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务对象</returns>
        public static T GetObj<T>()
        {
            return _svcProvider.GetService<T>();
        }

        /// <summary>
        /// 在全局服务容器中获取指定类型的服务对象，服务类型不存在时异常
        /// </summary>
        /// <param name="p_svcType"></param>
        /// <returns>服务对象</returns>
        public static object GetObj(Type p_svcType)
        {
            return _svcProvider.GetService(p_svcType);
        }

        /// <summary>
        /// 在全局服务容器中获取指定类型的所有服务对象
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>所有服务对象</returns>
        public static IEnumerable<T> GetObjs<T>()
        {
            return _svcProvider.GetServices<T>();
        }

        /// <summary>
        /// 在全局服务容器中获取指定类型的所有服务对象
        /// </summary>
        /// <param name="p_svcType">服务类型</param>
        /// <returns>所有服务对象</returns>
        public static IEnumerable<object> GetObjs(Type p_svcType)
        {
            return _svcProvider.GetServices(p_svcType);
        }
        #endregion

        #region Startup
        internal static void ConfigureServices(IServiceCollection p_services)
        {
            // 外部
            _stub.ConfigureServices(p_services);

            // 以便访问当前的HttpContext
            p_services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        internal static void Configure(IApplicationBuilder p_app)
        {
            // 全局服务容器
            _svcProvider = p_app.ApplicationServices;
            _accessor = _svcProvider.GetRequiredService<IHttpContextAccessor>();
            _stub.Configure(p_app, DtMiddleware.RequestHandlers);
        }
        #endregion
    }
}
