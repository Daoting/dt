#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前数据访问对象，静态类方便使用
    /// </summary>
    public static partial class At
    {
        /// <summary>
        /// 系统架构：使用dt服务的多层架构、直连数据库的两层架构、单机
        /// </summary>
        public static AccessType Framework
        {
            get
            {
                if (_originAI != null)
                    return _originAI.Type;
                return AccessType.Local;
            }
        }

        /// <summary>
        /// 当前实体系统使用的服务 或 直连数据库的信息
        /// </summary>
        public static IAccessInfo AccessInfo => _currentAI;

        /// <summary>
        /// 获取实体系统当前使用的服务名
        /// </summary>
        public static string Svc
        {
            get
            {
                if (_currentAI is SvcAccessInfo di)
                    return di.Name;
                return null;
            }
            set
            {
                if (_svcUrlInfo == null)
                    Throw.Msg("当前未使用服务架构！");
                _currentAI = GetAccessInfo(AccessType.Service, value);
            }
        }

        /// <summary>
        /// 设置实体系统当前直连的数据库
        /// </summary>
        /// <param name="p_dbKey"></param>
        public static void SetDb(string p_dbKey)
        {
            _currentAI = GetAccessInfo(AccessType.Database, p_dbKey);
        }

        /// <summary>
        /// 重置实体系统的默认服务 或 默认数据库
        /// </summary>
        public static void Reset()
        {
            _currentAI = _originAI;
        }

        /// <summary>
        /// 获取服务地址
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetSvcUrl(string p_svcName)
        {
            return _svcUrlInfo?.GetSvcUrl(p_svcName);
        }

        /// <summary>
        /// 初始化所有微服务地址
        /// </summary>
        /// <param name="p_svcUrls"></param>
        /// <param name="p_entitySvcName">实体系统使用的默认服务名</param>
        public static void InitSvcUrls(Dict p_svcUrls, string p_entitySvcName)
        {
            _svcUrlInfo.InitSvcUrls(p_svcUrls);
            _originAI = _currentAI = GetAccessInfo(AccessType.Service, p_entitySvcName);
        }

        /// <summary>
        /// 获取指定的IAccessInfo
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_name"></param>
        /// <returns></returns>
        public static IAccessInfo GetAccessInfo(AccessType p_type, string p_name)
        {
            if (p_type == AccessType.Service)
            {
                if (_svcs.TryGetValue(p_name, out var m))
                    return m;

                var ai = new SvcAccessInfo(p_name);
                _svcs[p_name] = ai;
                return ai;
            }

            if (p_type == AccessType.Database)
            {
                if (_dbs.TryGetValue(p_name, out var m))
                    return m;
                Throw.Msg($"直连数据库未配置{p_name}的连接信息！");
            }

            if (_sqlites.TryGetValue(p_name, out var s))
                return s;

            var sai = new SqliteAccessInfo(p_name);
            _sqlites[p_name] = sai;
            return sai;
        }

        internal static void InitConfig()
        {
#if DOTNET
            // wasm不支持直连数据库，不使用Config.json，在Config.js配置
            if (Kit.AppType == AppType.Wasm)
            {
                InitWasmConfig();
                return;
            }
#endif

            string config;
#if ANDROID
            try
            {
                using (var sr = new StreamReader(Android.App.Application.Context.Assets.Open("Config.json")))
                {
                    config = sr.ReadToEnd();
                }
            }
            catch
            {
                Kit.Debug("缺少Config.json文件！");
                return;
            }
#else
            var path = Path.Combine(AppContext.BaseDirectory, "Config.json");
            if (!File.Exists(path))
            {
                // throw时无提示信息
                Kit.Debug("缺少Config.json文件！");
                return;
            }
            config = File.ReadAllText(path);
#endif

            string dbKey = null;
            try
            {
                var r = new Utf8JsonReader(Encoding.UTF8.GetBytes(config), new JsonReaderOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true, });
                // {
                r.Read();
                while (r.Read())
                {
                    if (r.TokenType != JsonTokenType.PropertyName)
                        break;

                    string key = r.GetString().ToLower();
                    if (key == "title")
                    {
                        Kit.Title = r.ReadAsString();
                    }
                    else if (key == "server")
                    {
                        var str = r.ReadAsString();
                        if (Regex.IsMatch(str, @"^http[s]?://[^\s/]+"))
                        {
                            _svcUrlInfo = new SvcUrlInfo(str);
                            _originAI = _currentAI = GetAccessInfo(AccessType.Service, "cm");
                        }
                        else
                        {
                            dbKey = str;
                        }
                    }
                    else if (key == "database")
                    {
                        // {
                        r.Read();

                        while (r.Read() && r.TokenType != JsonTokenType.EndObject)
                        {
                            string dbName = r.GetString();
                            // {
                            r.Read();

                            string connStr = null;
                            DatabaseType? tp = null;
                            while (r.Read() && r.TokenType != JsonTokenType.EndObject)
                            {
                                var name = r.GetString().ToLower();
                                if (name == "connstr")
                                {
                                    connStr = r.ReadAsString();
                                }
                                else if (name == "dbtype")
                                {
                                    var dbType = r.ReadAsString().ToLower();
                                    if (dbType == "mysql")
                                        tp = DatabaseType.MySql;
                                    else if (dbType == "oracle")
                                        tp = DatabaseType.Oracle;
                                    else if (dbType == "sqlserver")
                                        tp = DatabaseType.SqlServer;
                                    else if (dbType == "postgresql")
                                        tp = DatabaseType.PostgreSql;
                                }
                            }
                            if (!string.IsNullOrEmpty(connStr) && tp != null)
                            {
                                _dbs[dbName] = new DbAccessInfo(dbName, connStr, tp.Value);
                            }
                        }
                    }
                    else
                    {
                        r.Read();
                        r.TrySkip();
                    }
                }
            }
            catch (Exception ex)
            {
                // throw时无提示信息
                Kit.Debug("读取 Config.json 时出错！" + ex.Message);
                return;
            }

            if (dbKey != null)
            {
                if (_dbs.TryGetValue(dbKey, out var info))
                {
                    _originAI = _currentAI = info;
                }
                else
                {
                    Kit.Debug($"Config.json 中未设置键名为 {dbKey} 的数据库连接串！");
                }
            }

#if DEBUG
            string fw = _originAI == null ? "单机架构" : (_originAI.Type == AccessType.Service ? "多层服务架构" : $"两层架构({((DbAccessInfo)_originAI).DbType})");
            Kit.Debug(fw);
#endif
        }

        static void InitWasmConfig()
        {
#if DOTNET
            // wasm不支持直连数据库，因启动时读取配置，不使用Config.json，使用Config.js
            // UnoAppManifest.displayName 在AppManifest.js中设置
            // DtConfig.server 在Config.js配置，AppManifest.js每次会动态生成，无法在其设置
            Kit.Title = Kit.InvokeJS("UnoAppManifest.displayName");
            var server = Kit.InvokeJS("DtConfig.server");
            if (!string.IsNullOrEmpty(server))
            {
                _svcUrlInfo = new SvcUrlInfo(server);
                _originAI = _currentAI = GetAccessInfo(AccessType.Service, "cm");
            }
            Console.WriteLine($"标题：{(Kit.Title == null ? "" : Kit.Title)}\r\n服务：{(server == null ? "" : server)}");
#endif
        }

        static SvcUrlInfo _svcUrlInfo;
        static IAccessInfo _originAI;
        static IAccessInfo _currentAI;

        static readonly Dictionary<string, SvcAccessInfo> _svcs = new Dictionary<string, SvcAccessInfo>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, DbAccessInfo> _dbs = new Dictionary<string, DbAccessInfo>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, SqliteAccessInfo> _sqlites = new Dictionary<string, SqliteAccessInfo>(StringComparer.OrdinalIgnoreCase);
    }
}
