#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
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
                return new MySqlAccess(this);

            return new MySqlAccess(this);
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
