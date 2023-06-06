#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-05-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data.Common;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Sql Server数据库访问类，不用安装客户端软件
    /// </summary>
    class SqlServerAccess : DataAccess
    {
        #region 构造方法
        public SqlServerAccess(DbInfo p_info)
            : base(p_info)
        {
        }
        #endregion

        #region 重写
        protected override DbConnection CreateConnection()
            => new SqlConnection(DbInfo.ConnStr);

        protected override string GetPageSql(int p_starRow, int p_pageSize, string p_keyOrSql)
        {
            // SQL2012以上的版本才支持，前段sql应有order by，否则出错！
            var sql = GetSql(p_keyOrSql);
            if (!sql.Contains(" order ", StringComparison.OrdinalIgnoreCase))
            {
                // 添加无用的order by
                sql += " ORDER BY(SELECT NULL)";
            }

            return $"{sql} offset {p_starRow} rows fetch next {p_pageSize} rows only";
        }
        #endregion

        #region 表结构
        const string _sqlAllTbls = "select name from sysobjects where xtype='u'";
        const string _sqlCols = "select * from [{0}] where 1!=1";
        const string _sqlComment =
"SELECT d.text def, c.value comment 　　 \n" +
"FROM\n" +
"	sys.tables a 　　\n" +
"	INNER JOIN sys.columns b ON b.object_id = a.object_id 　　\n" +
"	LEFT JOIN sys.extended_properties c ON ( c.major_id = b.object_id AND c.minor_id = b.column_id )\n" +
"	LEFT JOIN dbo.syscomments d ON b.default_object_id = d.id\n" +
"WHERE a.name = '{0}' AND b.name= '{1}'";

        const string _sqlPk =
"select col_name(object_id('{0}'),c.colid)\n" +
"from sysobjects a,sysindexes b,sysindexkeys c\n" +
"where a.name=b.name and b.id=c.id and b.indid=c.indid\n" +
"and a.xtype='PK' and a.parent_obj=object_id('{0}') and c.id=object_id('{0}')";

        /// <summary>
        /// 获取数据库所有表结构信息
        /// </summary>
        /// <returns>返回加载结果信息</returns>
        public override IReadOnlyDictionary<string, TableSchema> GetDbSchema()
        {
            var schema = new Dictionary<string, TableSchema>(StringComparer.OrdinalIgnoreCase);
            using (var conn = new SqlConnection(DbInfo.ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                // 所有表名
                cmd.CommandText = _sqlAllTbls;
                List<string> tbls = new List<string>();
                SqlDataReader reader;
                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tbls.Add(reader.GetString(0));
                        }
                    }
                }

                // 表结构
                foreach (var tbl in tbls)
                {
                    TableSchema tblCols = new TableSchema(tbl, DatabaseType.SqlServer);
                    cmd.CommandText = string.Format(_sqlCols, tbl);
                    ReadOnlyCollection<DbColumn> cols;
                    using (reader = cmd.ExecuteReader())
                    {
                        cols = reader.GetColumnSchema();
                    }

                    // 表的主键
                    cmd.CommandText = string.Format(_sqlPk, tbl);
                    List<string> pk = new List<string>();
                    using (reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                pk.Add(reader.GetString(0));
                            }
                        }
                    }

                    foreach (var colSchema in cols)
                    {
                        TableCol col = new TableCol();
                        col.Name = colSchema.ColumnName;

                        // 可为null的值类型
                        if (colSchema.AllowDBNull.HasValue && colSchema.AllowDBNull.Value && colSchema.DataType.IsValueType)
                            col.Type = typeof(Nullable<>).MakeGenericType(colSchema.DataType);
                        else
                            col.Type = colSchema.DataType;

                        // character_maximum_length
                        if (colSchema.ColumnSize.HasValue)
                            col.Length = colSchema.ColumnSize.Value;

                        if (colSchema.AllowDBNull.HasValue)
                            col.Nullable = colSchema.AllowDBNull.Value;

                        // 读取列结构
                        cmd.CommandText = string.Format(_sqlComment, tbl, colSchema.ColumnName);
                        using (reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                // 默认值
                                if (!reader.IsDBNull(0))
                                    col.Default = reader.GetString(0);
                                // 字段注释
                                if (!reader.IsDBNull(1))
                                    col.Comments = reader.GetString(1);
                            }
                        }

                        // 是否为主键
                        if (pk.Contains(colSchema.ColumnName))
                            tblCols.PrimaryKey.Add(col);
                        else
                            tblCols.Columns.Add(col);
                    }
                    schema[tbl] = tblCols;
                }

                // 取Db时间
                cmd.CommandText = "select getdate()";
                Kit.Now = (DateTime)cmd.ExecuteScalar();
            }
            return schema;
        }

        /// <summary>
        /// 获取数据库的所有表名
        /// </summary>
        /// <returns></returns>
        public override List<string> GetAllTableNames()
        {
            using (var conn = new SqlConnection(DbInfo.ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = _sqlAllTbls;
                List<string> tbls = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tbls.Add(reader.GetString(0));
                        }
                    }
                }
                return tbls;
            }
        }

        /// <summary>
        /// 获取单个表结构信息
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public override TableSchema GetTableSchema(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            using (var conn = new SqlConnection(DbInfo.ConnStr))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                SqlDataReader reader;
                TableSchema tblCols = null;

                // 表注释
                cmd.CommandText = $"select a.name,g.[value] from sys.tables a left join sys.extended_properties g on (a.object_id = g.major_id AND g.minor_id = 0) where a.name='{p_tblName}'";
                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        // 原始名称
                        if (!reader.IsDBNull(0))
                            tblCols = new TableSchema(reader.GetString(0), DatabaseType.SqlServer);

                        // 注释
                        if (!reader.IsDBNull(1))
                            tblCols.Comments = reader.GetString(1);
                    }
                }
                if (tblCols == null)
                    return null;

                // 表结构
                cmd.CommandText = string.Format(_sqlCols, tblCols.Name);
                ReadOnlyCollection<DbColumn> cols;
                using (reader = cmd.ExecuteReader())
                {
                    cols = reader.GetColumnSchema();
                }

                // 表的主键
                cmd.CommandText = string.Format(_sqlPk, tblCols.Name);
                List<string> pk = new List<string>();
                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            pk.Add(reader.GetString(0));
                        }
                    }
                }

                foreach (var colSchema in cols)
                {
                    TableCol col = new TableCol();
                    col.Name = colSchema.ColumnName;

                    // 可为null的值类型
                    if (colSchema.AllowDBNull.HasValue && colSchema.AllowDBNull.Value && colSchema.DataType.IsValueType)
                        col.Type = typeof(Nullable<>).MakeGenericType(colSchema.DataType);
                    else
                        col.Type = colSchema.DataType;

                    // character_maximum_length
                    if (colSchema.ColumnSize.HasValue)
                        col.Length = colSchema.ColumnSize.Value;

                    if (colSchema.AllowDBNull.HasValue)
                        col.Nullable = colSchema.AllowDBNull.Value;

                    // 读取列结构
                    cmd.CommandText = string.Format(_sqlComment, tblCols.Name, colSchema.ColumnName);
                    using (reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            // 默认值
                            if (!reader.IsDBNull(0))
                                col.Default = reader.GetString(0);
                            // 字段注释
                            if (!reader.IsDBNull(1))
                                col.Comments = reader.GetString(1);
                        }
                    }

                    // 是否为主键
                    if (pk.Contains(colSchema.ColumnName))
                        tblCols.PrimaryKey.Add(col);
                    else
                        tblCols.Columns.Add(col);
                }

                return tblCols;
            }
        }

        /// <summary>
        /// 数据库中是否存在指定的表
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public override async Task<bool> ExistTable(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return false;

            // 大小写不敏感
            return await GetScalar<int>($"select count(*) from sysobjects where xtype='u' and name='{p_tblName}'") > 0;
        }
        #endregion
    }
}