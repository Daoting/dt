#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据库描述信息
    /// </summary>
    public class DbInfo
    {
        public DbInfo(string p_key, string p_connStr, DatabaseType p_type)
        {
            Key = p_key;
            ConnStr = p_connStr;
            Type = p_type;

            if (p_type == DatabaseType.MySql)
                ParseMySql();
            else if (p_type == DatabaseType.Oracle)
                ParseOracle();
            else
                ParseSqlServer();
        }

        /// <summary>
        /// 数据源键名，在global.json可根据键名获取连接串
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnStr { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType Type { get; }

        /// <summary>
        /// 当前数据库连接用户名
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 数据库主机
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DbName { get; private set; }

        /// <summary>
        /// 根据数据源键名获取数据库描述信息
        /// </summary>
        /// <param name="p_dbKey">数据源键名</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DbInfo ReadConfig(string p_dbKey)
        {
            if (string.IsNullOrWhiteSpace(p_dbKey))
                throw new Exception("数据源键名不可为空！");

            var info = Kit.Config["MySql:" + p_dbKey];
            if (!string.IsNullOrEmpty(info))
                return new DbInfo(p_dbKey, info, DatabaseType.MySql);

            info = Kit.Config["Oracle:" + p_dbKey];
            if (!string.IsNullOrEmpty(info))
                return new DbInfo(p_dbKey, info, DatabaseType.Oracle);

            info = Kit.Config["SqlServer:" + p_dbKey];
            if (!string.IsNullOrEmpty(info))
                return new DbInfo(p_dbKey, info, DatabaseType.SqlServer);

            throw new Exception($"数据源键名[{p_dbKey}]在global.json无配置！");
        }

        /// <summary>
        /// 创建数据访问对象
        /// </summary>
        /// <returns></returns>
        public IDataAccess NewDataAccess()
        {
            if (Type == DatabaseType.MySql)
                return new MySqlAccess(this);

            if (Type == DatabaseType.Oracle)
                return new OracleAccess(this);

            return new MySqlAccess(this);
        }

        void ParseOracle()
        {
            string[] parts = ConnStr.Split(';');
            foreach (string part in parts)
            {
                int index = part.IndexOf('=');
                if (index == -1)
                    continue;

                string key = part.Substring(0, index).Trim().ToLower();
                if (key == "data source")
                {
                    string temp = part.Substring(index + 1).Trim();
                    Match match = Regex.Match(temp, @"\([\s]*HOST[\s]*=[\s]*([^\)^\s)]+)");
                    if (match != null && match.Groups.Count == 2)
                        Host = match.Groups[1].Value;
                    match = Regex.Match(temp, @"\([\s]*SERVICE_NAME[\s]*=[\s]*([^\)^\s)]+)");
                    if (match != null && match.Groups.Count == 2)
                        DbName = match.Groups[1].Value;
                }
                else if (key == "user id")
                {
                    UserName = part.Substring(index + 1).Trim().ToUpper();
                }

                if (DbName != null && UserName != null)
                    break;
            }
        }

        void ParseMySql()
        {
            string[] parts = ConnStr.Split(';');
            foreach (string part in parts)
            {
                int index = part.IndexOf('=');
                if (index == -1)
                    continue;

                string key = part.Substring(0, index).Trim().ToLower();
                string val = part.Substring(index + 1).Trim();
                if (key == "server")
                {
                    Host = val;
                }
                else if (key == "uid")
                {
                    UserName = val;
                }
                else if (key == "database")
                {
                    DbName = val;
                }
            }
        }

        void ParseSqlServer()
        {

        }
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        MySql,
        Oracle,
        SqlServer
    }
}
