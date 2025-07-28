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
using Windows.Storage;
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
        /// 设置实体系统当前的本地sqlite库
        /// </summary>
        /// <param name="p_dbSqlite"></param>
        public static void SetSqliteDb(string p_dbSqlite)
        {
            _currentAI = GetAccessInfo(AccessType.Local, p_dbSqlite);
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
            string dbKey = null;
            string sqliteKey = null;

#if WASM
            if (Regex.IsMatch(GlobalConfig.WasmServer, @"^http[s]?://[^\s/]+"))
            {
                _svcUrlInfo = new SvcUrlInfo(GlobalConfig.WasmServer);
                _originAI = _currentAI = GetAccessInfo(AccessType.Service, "cm");
            }
#else
            if (Regex.IsMatch(GlobalConfig.Server, @"^http[s]?://[^\s/]+"))
            {
                _svcUrlInfo = new SvcUrlInfo(GlobalConfig.Server);
                _originAI = _currentAI = GetAccessInfo(AccessType.Service, "cm");
            }
            else if (GlobalConfig.Server.StartsWith("sqlite/", StringComparison.OrdinalIgnoreCase))
            {
                sqliteKey = GlobalConfig.Server.Substring(7);
            }
            else
            {
                dbKey = GlobalConfig.Server;
            }
#endif

#if WASM
            if (_originAI == null)
            {
                throw new Exception("Config.json 缺少键名为 WasmServer 的服务地址！");
            }
#else
            if (dbKey != null)
            {
                if (_dbs.TryGetValue(dbKey, out var info))
                {
                    _originAI = _currentAI = info;
                }
                else
                {
                    throw new Exception($"Config.json 中未设置键名为 {dbKey} 的数据库连接串！");
                }
            }
            else if (sqliteKey != null)
            {
                SqliteAccessInfo info;
                if (!_sqlites.TryGetValue(sqliteKey, out info))
                {
                    info = new SqliteAccessInfo(sqliteKey);
                    _sqlites[sqliteKey] = info;
                }
                _originAI = _currentAI = info;
            }
#endif

#if DEBUG
            string fw = "单机架构";
            if (_originAI != null)
            {
                if (_originAI.Type == AccessType.Service)
                {
                    fw = "多层服务架构";
                }
                else if (_originAI.Type == AccessType.Database && dbKey != null)
                {
                    fw = $"两层架构({((DbAccessInfo)_originAI).DbType})";
                }
                else if (sqliteKey != null)
                {
                    fw = $"单机架构({sqliteKey})";
                }
            }
            Kit.Trace(fw);
#endif
        }

        static SvcUrlInfo _svcUrlInfo;
        static IAccessInfo _originAI;
        static IAccessInfo _currentAI;

        static readonly Dictionary<string, SvcAccessInfo> _svcs = new Dictionary<string, SvcAccessInfo>(StringComparer.OrdinalIgnoreCase);
        static Dictionary<string, DbAccessInfo> _dbs => GlobalConfig._dbs;
        static readonly Dictionary<string, SqliteAccessInfo> _sqlites = new Dictionary<string, SqliteAccessInfo>(StringComparer.OrdinalIgnoreCase);
    }
}
