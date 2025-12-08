#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using Dt.Core.Rpc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 内部使用的全局静态类
    /// </summary>
    internal static class Silo
    {
        #region 构造方法
        static Silo()
        {
            // Api字典
            Methods = new Dictionary<string, ApiMethod>();
            GroupMethods = new Dictionary<string, List<string>>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取Api字典
        /// </summary>
        public static Dictionary<string, ApiMethod> Methods { get; }

        /// <summary>
        /// 获取Api分组列表
        /// </summary>
        public static Dictionary<string, List<string>> GroupMethods { get; }
        #endregion

        #region Api
        /// <summary>
        /// 获取Api描述
        /// </summary>
        /// <param name="p_methodName">Api名称</param>
        /// <returns>Api描述</returns>
        public static ApiMethod GetMethod(string p_methodName)
        {
            ApiMethod sm;
            if (Methods.TryGetValue(p_methodName, out sm))
                return sm;
            return null;
        }

        /// <summary>
        /// 根据Api名称构造调用时的json串
        /// </summary>
        /// <param name="p_methodName">Api名称</param>
        /// <returns></returns>
        public static string GetMethodCall(string p_methodName)
        {
            ApiMethod sm = GetMethod(p_methodName);
            if (sm == null)
                return null;

            List<object> funParams = new List<object>();
            foreach (ParameterInfo param in sm.Method.GetParameters())
            {
                Type type = param.ParameterType;
                if (type == typeof(string))
                {
                    funParams.Add("");
                }
                else if (type == typeof(Dict) || type == typeof(object))
                {
                    Dict dict = new Dict();
                    dict["键"] = "值";
                    funParams.Add(dict);
                }
                else if (type == typeof(Table))
                {
                    Table tbl = new Table { { "列名1" } };
                    tbl.NewRow("");
                    funParams.Add(tbl);
                }
                else if (type == typeof(bool))
                {
                    funParams.Add(true);
                }
                else if (type == typeof(Int32))
                {
                    funParams.Add(0);
                }
                else if (type == typeof(Int64))
                {
                    funParams.Add(0L);
                }
                else if (type == typeof(double))
                {
                    funParams.Add(0d);
                }
                else if (type == typeof(DateTime))
                {
                    funParams.Add(DateTime.Now);
                }

                else if (type == typeof(byte[]))
                {
                    funParams.Add(new byte[0]);
                }
                else if (type == typeof(string[]))
                {
                    funParams.Add(new string[1] { "" });
                }
                else if (type == typeof(List<string>))
                {
                    List<string> ls = new List<string>();
                    ls.Add("");
                    funParams.Add(ls);
                }
                else if (type == typeof(List<int>))
                {
                    funParams.Add(new List<int>() { 1, 2, 3, 4 });
                }
                else if (type == typeof(List<double>))
                {
                    funParams.Add(new List<double>() { 200.0d, 100d, 50.123d, 123.45d });
                }
                else if (type.IsArray)
                {
                    funParams.Add(Activator.CreateInstance(type, 1));
                }
                else
                {
                    funParams.Add(Activator.CreateInstance(type));
                }
            }

            // 序列化Json串 RPC 调用请求，含有缩进
            return RpcKit.GetCallString(sm.SvcName, p_methodName, funParams, true);
        }
        #endregion

        #region Startup
        /// <summary>
        /// 注入服务，提取程序集中的Api列表、事件处理类型、服务列表、可序列化类型列表，注册服务
        /// </summary>
        /// <param name="p_services"></param>
        /// <returns></returns>
        public static void ConfigureServices(IServiceCollection p_services)
        {
            // 提取微服务和Dt.Core程序集
            LoadAssembly(typeof(Silo).Assembly, p_services, "公共");
            foreach (var svc in Kit.Svcs)
            {
                LoadAssembly(svc.Stub.GetType().Assembly, p_services, svc.SvcName);
            }

            // 内部服务管理Api
            ExtractApi(typeof(Admin), null, p_services, null);
            Log.Information("注入服务成功");
        }

        static void LoadAssembly(Assembly p_asm, IServiceCollection p_services, string p_svcName)
        {
            // 过滤有用类型，排序为了admin页面显示顺序
            var ls = from tp in p_asm.GetTypes()
                     where tp != null && tp.IsClass && tp.IsPublic && !tp.IsAbstract
                     orderby tp.Name
                     select tp;

            foreach (Type type in ls)
            {
                // 提取Api
                if (type.IsSubclassOf(typeof(DomainSvc)))
                {
                    ApiAttribute rpcAttr = type.GetCustomAttribute<ApiAttribute>(false);
                    if (rpcAttr != null)
                    {
                        ExtractApi(type, rpcAttr, p_services, rpcAttr.IsTest ? "测试" : p_svcName);
                        continue;
                    }
                }

                // 注册事件处理
                if (IsEventHandler(type, p_services))
                    continue;

                // 注册服务，支持继承的 ServiceAttribute
                ServiceAttribute svcAttr = type.GetCustomAttribute<ServiceAttribute>(true);
                if (svcAttr != null)
                {
                    if (svcAttr.Lifetime == ServiceLifetime.Singleton)
                    {
                        p_services.AddSingleton(type);
                    }
                    else if (svcAttr.Lifetime == ServiceLifetime.Scoped)
                    {
                        p_services.AddScoped(type);
                    }
                    else
                    {
                        p_services.AddTransient(type);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// 提取类型中的Api，注册服务
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_apiAttr"></param>
        /// <param name="p_services"></param>
        /// <param name="p_groupName">分组名称</param>
        static void ExtractApi(Type p_type, ApiAttribute p_apiAttr, IServiceCollection p_services, string p_groupName)
        {
            // 分组列表
            List<string> grpMethods = null;
            if (!string.IsNullOrEmpty(p_groupName))
            {
                if (GroupMethods.TryGetValue(p_groupName, out List<string> ls))
                {
                    grpMethods = ls;
                }
                else
                {
                    grpMethods = new List<string>();
                    GroupMethods[p_groupName] = grpMethods;
                }
            }

            // 服务名
            string svcName;
            if (p_apiAttr == null
                || p_apiAttr.IsTest
                || string.IsNullOrEmpty(p_groupName)
                || p_groupName == "公共")
            {
                svcName = Kit.Svcs[0].SvcName;
            }
            else
            {
                svcName = p_groupName;
            }

            ApiCallMode callMode;
            var clsAuth = p_type.GetCustomAttribute<AuthAttribute>(false);
            MethodInfo[] methods = p_type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo mi in methods)
            {
                // 是否为流模式方法，流模式返回值始终为Task
                callMode = ApiCallMode.Unary;
                if (mi.ReturnType == typeof(Task))
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    if (pis.Length > 0)
                    {
                        if (pis[pis.Length - 1].ParameterType == typeof(RequestReader))
                        {
                            callMode = ApiCallMode.ClientStream;
                        }
                        else if (pis.Length > 1
                            && pis[pis.Length - 2].ParameterType == typeof(RequestReader)
                            && pis[pis.Length - 1].ParameterType == typeof(ResponseWriter))
                        {
                            callMode = ApiCallMode.DuplexStream;
                        }
                        else if (pis[pis.Length - 1].ParameterType == typeof(ResponseWriter))
                        {
                            callMode = ApiCallMode.ServerStream;
                        }
                    }
                }

                var methodAuth = mi.GetCustomAttribute<AuthAttribute>(false);
                string name = $"{p_type.Name}.{mi.Name}";
                Methods[name] = new ApiMethod(mi, callMode, methodAuth ?? clsAuth, svcName);
                if (grpMethods != null)
                    grpMethods.Add(name);
            }
            
            // 注册Api服务
            p_services.AddTransient(p_type);
        }

        /// <summary>
        /// 注册事件处理
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_services"></param>
        /// <returns></returns>
        static bool IsEventHandler(Type p_type, IServiceCollection p_services)
        {
            // 不含标签的排除
            if (p_type.GetCustomAttribute<EventHandlerAttribute>(false) == null)
                return false;

            // 获取泛型接口类型，一个事件处理可以同时支持本地和远程
            var types = p_type.GetInterfaces().Where(t => t.IsGenericType);
            bool isHandler = false;
            foreach (var iType in types)
            {
                Type eventType = iType.GetGenericArguments()[0];
                Type genType = iType.GetGenericTypeDefinition();
                if (genType == typeof(IRemoteEventHandler<>))
                {
                    Type tgtType = typeof(IRemoteEventHandler<>).MakeGenericType(eventType);
                    p_services.AddTransient(tgtType, p_type);
                    RemoteEventBus.Events[eventType.Name] = tgtType;
                    isHandler = true;
                }
                else if (genType == typeof(IEventHandler<>))
                {
                    Type tgtType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    p_services.AddTransient(tgtType, p_type);
                    LocalEventBus.EventHandlerTypes[eventType.Name] = tgtType;
                    isHandler = true;
                }
            }
            return isHandler;
        }
        #endregion
    }
}
