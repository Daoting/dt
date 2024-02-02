#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据访问对象
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 获取当前http请求上下文的数据访问对象，无http请求上下文时返回新对象！AutoClose为true
        /// <para>如：本地定时器调用或RabbitMQ消息产生的调用无http请求上下文</para>
        /// </summary>
        public static IDataAccess DataAccess
        {
            get
            {
                if (HttpContext != null)
                    return ((Bag)HttpContext.Items[ContextItemName]).DataAccess;
                return NewDataAccess();
            }
        }

        /// <summary>
        /// 获取新的数据访问对象
        /// </summary>
        /// <param name="p_dbKey">数据源键名，在global.json可根据键名获取连接串，null时使用默认数据源</param>
        /// <returns>返回数据访问对象</returns>
        public static IDataAccess NewDataAccess(string p_dbKey = null)
        {
            // 默认数据源
            if (string.IsNullOrEmpty(p_dbKey) || _defaultDbInfo.Name.Equals(p_dbKey, StringComparison.OrdinalIgnoreCase))
                return _defaultDbInfo.GetDa();

            // 其它数据源
            if (_dbAll.TryGetValue(p_dbKey, out var dbInfo))
                return dbInfo.GetDa();

            throw new Exception($"数据源键名[{p_dbKey}]在global.json无配置！");
        }

        /// <summary>
        /// 所有数据库描述信息
        /// </summary>
        public static IReadOnlyDictionary<string, DbAccessInfo> AllDbInfo => _dbAll;

        /// <summary>
        /// 默认数据库描述信息
        /// </summary>
        public static DbAccessInfo DefaultDbInfo => _defaultDbInfo;

        static void InitDbInfo()
        {
            var defDbKey = _config["DbKey"];
            if (string.IsNullOrEmpty(defDbKey))
                throw new Exception("service.json中缺少默认数据源键名的配置！");

            _dbAll = new Dictionary<string, DbAccessInfo>(StringComparer.OrdinalIgnoreCase);
            var sect = _config.GetSection("Database");
            foreach (var item in sect.GetChildren())
            {
                var connStr = sect.GetValue<string>(item.Key + ":ConnStr");
                if (string.IsNullOrEmpty(connStr))
                    continue;

                var dbType = sect.GetValue(item.Key + ":DbType", "mysql").ToLower();
                DatabaseType tp;
                if (dbType == "mysql")
                    tp = DatabaseType.MySql;
                else if (dbType == "oracle")
                    tp = DatabaseType.Oracle;
                else if (dbType == "sqlserver")
                    tp = DatabaseType.SqlServer;
                else if (dbType == "postgresql")
                    tp = DatabaseType.PostgreSql;
                else
                    continue;

                var di = new DbAccessInfo(item.Key, connStr, tp);
                _dbAll[item.Key] = di;

                if (item.Key.Equals(defDbKey, StringComparison.OrdinalIgnoreCase))
                    _defaultDbInfo = di;
            }

            if (_defaultDbInfo == null)
                throw new Exception($"默认数据源键名[{defDbKey}]在global.json无配置！");
        }

        /// <summary>
        /// 单体服务时所有服务的数据库描述信息，所有服务使用同一库时为null
        /// </summary>
        internal static Dictionary<string, DbAccessInfo> SingletonSvcDbs { get; set; }

        static Dictionary<string, DbAccessInfo> _dbAll;
        static DbAccessInfo _defaultDbInfo;
    }
}
