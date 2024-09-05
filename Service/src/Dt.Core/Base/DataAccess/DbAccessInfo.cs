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
    public class DbAccessInfo
#if !SERVER
        : IAccessInfo
#endif
    {
        public DbAccessInfo(string p_name, string p_connStr, DatabaseType p_type)
        {
            Name = p_name;
            ConnStr = p_connStr;
            DbType = p_type;
        }

#if !SERVER
        public AccessType Type => AccessType.Database;
#endif
        /// <summary>
        /// 数据源键名，在配置文件中可根据键名获取连接串
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnStr { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType { get; }
        
        /// <summary>
        /// 创建数据访问对象
        /// </summary>
        /// <returns></returns>
        public IDataAccess GetDa()
        {
            if (DbType == DatabaseType.MySql)
                return new MySqlAccess(this);

            if (DbType == DatabaseType.Oracle)
                return new OracleAccess(this);

            if (DbType == DatabaseType.SqlServer)
                return new SqlServerAccess(this);

            if (DbType == DatabaseType.PostgreSql)
                return new PostgreSqlAccess(this);

            throw new Exception($"无法创建[{DbType}]类型的数据访问对象！");
        }
    }
}
