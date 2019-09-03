#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Dt.Core.EventBus;
using Dt.Core.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 内部使用的全局静态类
    /// </summary>
    internal static class Silo
    {
        #region 成员变量
        static readonly Dictionary<string, string> _sqlDict;
        #endregion

        #region 构造方法
        static Silo()
        {
            // Api字典
            Methods = new Dictionary<string, ApiMethod>();
            GroupMethods = new Dictionary<string, List<string>>();
            // Sql缓存字典
            _sqlDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
            StringBuilder json = new StringBuilder();
            using (StringWriter sw = new StringWriter(json))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                writer.WriteStartArray();
                writer.WriteValue(p_methodName);
                foreach (object item in funParams)
                {
                    JsonRpcSerializer.Serialize(item, writer);
                }
                writer.WriteEndArray();
                writer.Flush();
            }
            return json.ToString();
        }
        #endregion

        #region Sql字典
        /// <summary>
        /// 缓存当前服务的所有Sql语句，表名xxx_sql
        /// </summary>
        public static void LoadCacheSql()
        {
            try
            {
                var ls = new Db().ForEach($"select id,`sql` from {Glb.SvcName}_sql").Result;
                foreach (var item in ls)
                {
                    _sqlDict[item.id] = item.sql;
                }
                Log.Information("缓存Sql成功");
            }
            catch (Exception e)
            {
                Log.Fatal($"缓存Sql失败：{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 缓存Sql串
        /// </summary>
        public static void CacheSql()
        {
            if (Glb.GetCfg("CacheSql", true))
            {
                Glb.Sql = GetDictSql;
                LoadCacheSql();
            }
            else
            {
                Glb.Sql = GetDebugSql;
                Log.Information("未缓存Sql, 调试状态");
            }
        }

        /// <summary>
        /// 系统配置(json文件)修改事件
        /// </summary>
        public static void OnConfigChanged()
        {
            if (Glb.GetCfg("CacheSql", true))
            {
                Glb.Sql = GetDictSql;
                LoadCacheSql();
                Log.Information("切换到Sql缓存模式");
            }
            else if (Glb.Sql != GetDictSql)
            {
                if (Glb.Sql != GetDebugSql)
                {
                    Glb.Sql = GetDebugSql;
                    Log.Information("切换到Sql调试模式");
                }
            }
        }

        /// <summary>
        /// 查询缓存中的Sql语句
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <returns>Sql语句</returns>
        static string GetDictSql(string p_keyOrSql)
        {
            if (string.IsNullOrEmpty(p_keyOrSql))
                throw new Exception("Sql键名不可为空！");

            // Sql语句中包含空格
            if (p_keyOrSql.IndexOf(' ') != -1)
                return p_keyOrSql;

            // 键名不包含空格！！！
            string sql;
            if (!_sqlDict.TryGetValue(p_keyOrSql, out sql))
                throw new Exception($"不存在键名为[{p_keyOrSql}]的Sql语句！");
            return sql;
        }

        /// <summary>
        /// 直接从库中查询Sql语句，只在调试时单机用！
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <returns>Sql语句</returns>
        static string GetDebugSql(string p_keyOrSql)
        {
            if (string.IsNullOrEmpty(p_keyOrSql))
                throw new Exception("Sql键名不可为空！");

            // Sql语句中包含空格
            if (p_keyOrSql.IndexOf(' ') != -1)
                return p_keyOrSql;

            // 键名不包含空格！！！
            if (!string.IsNullOrEmpty(p_keyOrSql))
                return new Db().Scalar<string>($"select `sql` from {Glb.SvcName}_sql where id='{p_keyOrSql}'").Result;
            return null;
        }

        #endregion

        #region Startup
        /// <summary>
        /// 注入服务，提取程序集中的Api列表、可序列化类型列表、领域服务列表，注册服务，添加拦截
        /// </summary>
        /// <param name="p_services"></param>
        /// <returns></returns>
        public static IServiceProvider ConfigureServices(IServiceCollection p_services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(p_services);

            // 内置拦截
            builder.RegisterType(typeof(LobInterceptor));

            //// 提取有用类型，程序集包括Dt.Core、微服务、插件(以.Addin.dll结尾)
            //List<Assembly> asms = new List<Assembly> { Glb.Stub.GetType().Assembly, typeof(Silo).Assembly };
            //asms.AddRange(Directory
            //    .EnumerateFiles(Path.GetDirectoryName(typeof(Silo).Assembly.Location), "*.Addin.dll", SearchOption.TopDirectoryOnly)
            //    .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath));

            // 提取微服务和Dt.Core程序集
            var types = Glb.Stub.GetType().Assembly.GetTypes()
                .Concat(typeof(Silo).Assembly.GetTypes())
                .Where(type => type != null && type.IsClass && type.IsPublic && !type.IsAbstract);

            foreach (Type type in types)
            {
                // 提取Api
                ApiAttribute rpcAttr = type.GetCustomAttribute<ApiAttribute>(false);
                if (rpcAttr != null && typeof(BaseApi).IsAssignableFrom(type))
                {
                    ExtractApi(type, rpcAttr, builder);
                    continue;
                }

                // 注册事件处理
                if (IsEventHandler(type, builder))
                    continue;

                // 注册服务，支持继承的SvcAttribute
                SvcAttribute svcAttr = type.GetCustomAttribute<SvcAttribute>(true);
                if (svcAttr != null)
                {
                    var itps = type.GetInterfaces();
                    if (itps.Length > 0)
                    {
                        // 注册接口类型
                        builder
                            .RegisterType(type)
                            .As(type)
                            .As(itps)
                            .ConfigureLifecycle(svcAttr.Lifetime, null);
                    }
                    else
                    {
                        builder
                            .RegisterType(type)
                            .ConfigureLifecycle(svcAttr.Lifetime, null);
                    }
                    continue;
                }

                // 自定义json序列化对象
                JsonObjAttribute serAttr = type.GetCustomAttribute<JsonObjAttribute>(false);
                if (serAttr != null)
                    SerializeTypeAlias.Add(serAttr.Alias, type);
            }

            // 内部服务管理Api
            ExtractApi(typeof(Admin), null, builder);
            Log.Information("注入服务成功");

            return new AutofacServiceProvider(builder.Build());
        }

        /// <summary>
        /// 提取类型中的Api，注册服务，添加拦截
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_apiAttr"></param>
        /// <param name="p_builder"></param>
        static void ExtractApi(Type p_type, ApiAttribute p_apiAttr, ContainerBuilder p_builder)
        {
            // 分组列表
            List<string> grpMethods = null;
            if (p_apiAttr != null)
            {
                string grpName = string.IsNullOrEmpty(p_apiAttr.Group) ? "基础API" : p_apiAttr.Group;
                if (GroupMethods.TryGetValue(grpName, out List<string> ls))
                {
                    grpMethods = ls;
                }
                else
                {
                    grpMethods = new List<string>();
                    GroupMethods[grpName] = grpMethods;
                }
            }

            ApiCallMode callMode;
            bool isTran;
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

                // 是否启用事务
                var transAttr = mi.GetCustomAttribute<TransactionAttribute>(false);
                if (transAttr != null)
                    isTran = transAttr.IsTransactional;
                else if (p_apiAttr != null)
                    isTran = p_apiAttr.IsTransactional;
                else
                    isTran = false;

                var methodAuth = mi.GetCustomAttribute<AuthAttribute>(false);
                string name = $"{p_type.Name}.{mi.Name}";
                Methods[name] = new ApiMethod(mi, callMode, methodAuth?? clsAuth, isTran);
                if (grpMethods != null)
                    grpMethods.Add(name);
            }

            // 注册服务，添加拦截
            var b = p_builder
                .RegisterType(p_type)
                .InstancePerDependency()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(LobInterceptor));
        }

        /// <summary>
        /// 注册事件处理
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_builder"></param>
        /// <returns></returns>
        static bool IsEventHandler(Type p_type, ContainerBuilder p_builder)
        {
            if (p_type.GetInterface("IEventHandler") == null)
                return false;

            // 获取泛型接口类型
            Type iType = p_type.GetInterfaces().FirstOrDefault(t => t.IsGenericType);
            if (iType == null)
                return false;

            Type eventType = iType.GetGenericArguments()[0];
            Type genType = iType.GetGenericTypeDefinition();
            if (genType == typeof(IRemoteHandler<>))
            {
                Type tgtType = typeof(IRemoteHandler<>).MakeGenericType(eventType);
                p_builder.RegisterType(p_type).As(tgtType);
                RemoteEventBus.Events[eventType.Name] = tgtType;
                return true;
            }

            if (genType == typeof(ILocalHandler<>))
            {
                Type tgtType = typeof(ILocalHandler<>).MakeGenericType(eventType);
                p_builder.RegisterType(p_type).As(tgtType);
                LocalEventBus.NoticeEvents[eventType.Name] = tgtType;
                return true;
            }

            if (genType == typeof(IRequestHandler<,>))
            {
                p_builder.RegisterType(p_type);
                LocalEventBus.RequestEvents[eventType] = p_type;
                return true;
            }
            return false;
        }
        #endregion
    }
}
