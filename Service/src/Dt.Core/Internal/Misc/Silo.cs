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
using System.Reflection;
using System.Text;
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
        static readonly Dictionary<string, Type> _entityDict;
        #endregion

        #region 构造方法
        static Silo()
        {
            // Api字典
            Methods = new Dictionary<string, ApiMethod>();
            GroupMethods = new Dictionary<string, List<string>>();
            // Sql缓存字典
            _sqlDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _entityDict = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
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
            return RpcKit.GetCallString(p_methodName, funParams, true);
        }
        #endregion

        #region Sql字典
        /// <summary>
        /// 缓存当前所有服务的所有Sql语句，表名xxx_sql
        /// </summary>
        public static void LoadCacheSql()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var name in Kit.SvcNames)
                {
                    if (sb.Length > 0)
                        sb.Append(" union ");
                    sb.Append($"select id,`sql` from {name}_sql");
                }
                var ls = new MySqlAccess().Each(sb.ToString()).Result;
                foreach (Row item in ls)
                {
                    _sqlDict[item.Str("id")] = item.Str("sql");
                }
                Log.Information("缓存Sql成功");
            }
            catch (Exception e)
            {
                Log.Fatal(e, "缓存Sql失败！");
                throw;
            }
        }

        /// <summary>
        /// 缓存Sql串
        /// </summary>
        public static void CacheSql()
        {
            if (Kit.GetCfg("CacheSql", true))
            {
                Kit.Sql = GetDictSql;
                LoadCacheSql();
            }
            else
            {
                Kit.Sql = GetDebugSql;

                // 生成查询sql
                if (Kit.Stubs.Length > 1)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var name in Kit.SvcNames)
                    {
                        string tbl = name + "_sql";
                        if (DbSchema.Schema.ContainsKey(tbl))
                        {
                            if (sb.Length > 0)
                                sb.Append(" union ");
                            sb.Append($"select `sql` from {tbl} where id=@id");
                        }
                    }
                    _debugSql = $"select * from ({sb}) a LIMIT 1";
                }
                else
                {
                    _debugSql = $"select `sql` from {Kit.Stubs[0].SvcName}_sql where id=@id";
                }
                Log.Information("未缓存Sql, 调试状态");
            }
        }

        /// <summary>
        /// 系统配置(json文件)修改事件
        /// </summary>
        public static void OnConfigChanged()
        {
            if (Kit.GetCfg("CacheSql", true))
            {
                Kit.Sql = GetDictSql;
                LoadCacheSql();
                Log.Information("切换到Sql缓存模式");
            }
            else if (Kit.Sql != GetDictSql)
            {
                if (Kit.Sql != GetDebugSql)
                {
                    Kit.Sql = GetDebugSql;
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

        static string _debugSql;
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

            return new MySqlAccess().GetScalar<string>(_debugSql, new { id = p_keyOrSql }).Result;
        }

        #endregion

        #region 实体类型
        /// <summary>
        /// 获取实体类型，不存在时抛出异常
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns>实体类型</returns>
        public static Type GetEntityType(string p_tblName)
        {
            if (_entityDict.TryGetValue(p_tblName, out var type))
                return type;
            throw new Exception($"表{p_tblName}不存在实体类型！");
        }
        #endregion

        #region Startup
        /// <summary>
        /// 注入服务，提取程序集中的Api列表、事件处理类型、服务列表、可序列化类型列表，注册服务，添加拦截
        /// </summary>
        /// <param name="p_builder"></param>
        /// <returns></returns>
        public static void ConfigureContainer(ContainerBuilder p_builder)
        {
            // 提取微服务和Dt.Core程序集
            LoadAssembly(typeof(Silo).Assembly, p_builder, "公共");
            foreach (var stub in Kit.Stubs)
            {
                LoadAssembly(stub.GetType().Assembly, p_builder, stub.SvcName);
            }

            // 内部服务管理Api
            ExtractApi(typeof(Admin), null, p_builder, null);
            Log.Information("注入服务成功");
        }

        static void LoadAssembly(Assembly p_asm, ContainerBuilder p_builder, string p_svcName)
        {
            // 过滤有用类型，排序为了admin页面显示顺序
            var ls = from tp in p_asm.GetTypes()
                     where tp != null && tp.IsClass && tp.IsPublic && !tp.IsAbstract
                     orderby tp.Name
                     select tp;

            foreach (Type type in ls)
            {
                // 提取Api
                if (type.IsSubclassOf(typeof(BaseApi)))
                {
                    ApiAttribute rpcAttr = type.GetCustomAttribute<ApiAttribute>(false);
                    if (rpcAttr != null)
                    {
                        ExtractApi(type, rpcAttr, p_builder, rpcAttr.IsTest ? "测试" : p_svcName);
                        continue;
                    }
                }
                
                // 注册事件处理
                if (IsEventHandler(type, p_builder))
                    continue;

                // 注册服务，支持继承的SvcAttribute
                SvcAttribute svcAttr = type.GetCustomAttribute<SvcAttribute>(true);
                if (svcAttr != null)
                {
                    var itps = type.GetInterfaces();
                    if (itps.Length > 0)
                    {
                        // 注册接口类型
                        p_builder
                            .RegisterType(type)
                            .As(type)
                            .As(itps)
                            .ConfigureLifecycle(svcAttr.Lifetime, null);
                    }
                    else
                    {
                        p_builder
                            .RegisterType(type)
                            .ConfigureLifecycle(svcAttr.Lifetime, null);
                    }
                    continue;
                }

                // 实体类型字典
                if (type.IsSubclassOf(typeof(Entity)))
                {
                    var tbl = type.GetCustomAttribute<TblAttribute>(false);
                    if (tbl != null && !string.IsNullOrEmpty(tbl.Name))
                        _entityDict[tbl.Name.ToLower()] = type;
                }
            }
        }

        /// <summary>
        /// 提取类型中的Api，注册服务，添加拦截
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_apiAttr"></param>
        /// <param name="p_builder"></param>
        /// <param name="p_groupName">分组名称</param>
        static void ExtractApi(Type p_type, ApiAttribute p_apiAttr, ContainerBuilder p_builder, string p_groupName)
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

                // 是否启用事务
                bool isTran = mi.GetCustomAttribute<TransactionAttribute>(false) != null;

                var methodAuth = mi.GetCustomAttribute<AuthAttribute>(false);
                string name = $"{p_type.Name}.{mi.Name}";
                Methods[name] = new ApiMethod(mi, callMode, methodAuth ?? clsAuth, isTran);
                if (grpMethods != null)
                    grpMethods.Add(name);
            }

            if (p_apiAttr != null
                && p_apiAttr.Interceptors != null
                && p_apiAttr.Interceptors.Length > 0)
            {
                // 将拦截器添加到容器
                p_builder.RegisterTypes(p_apiAttr.Interceptors);
                // 注册服务，添加拦截
                p_builder
                    .RegisterType(p_type)
                    .InstancePerDependency()
                    .EnableClassInterceptors()
                    .InterceptedBy(p_apiAttr.Interceptors);
            }
            else
            {
                // 注册服务
                p_builder.RegisterType(p_type);
            }
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

            // 获取泛型接口类型，一个事件处理可以同时支持本地和远程
            var types = p_type.GetInterfaces().Where(t => t.IsGenericType);
            bool isHandler = false;
            foreach (var iType in types)
            {
                Type eventType = iType.GetGenericArguments()[0];
                Type genType = iType.GetGenericTypeDefinition();
                if (genType == typeof(IRemoteHandler<>))
                {
                    Type tgtType = typeof(IRemoteHandler<>).MakeGenericType(eventType);
                    p_builder.RegisterType(p_type).As(tgtType);
                    RemoteEventBus.Events[eventType.Name] = tgtType;
                    isHandler = true;
                }
                else if (genType == typeof(ILocalHandler<>))
                {
                    Type tgtType = typeof(ILocalHandler<>).MakeGenericType(eventType);
                    p_builder.RegisterType(p_type).As(tgtType);
                    LocalEventBus.NoticeEvents[eventType.Name] = tgtType;
                    isHandler = true;
                }
                else if (genType == typeof(IRequestHandler<,>))
                {
                    p_builder.RegisterType(p_type);
                    LocalEventBus.RequestEvents[eventType] = p_type;
                    isHandler = true;
                }
            }
            return isHandler;
        }
        #endregion
    }
}
