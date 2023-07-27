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

        protected override string GetPageSql(int p_starRow, int p_pageSize, string p_sql)
        {
            // SQL2012以上的版本才支持，前段sql应有order by，否则出错！
            var sql = p_sql;
            if (!sql.Contains(" order ", StringComparison.OrdinalIgnoreCase))
            {
                // 添加无用的order by
                sql += " ORDER BY(SELECT NULL)";
            }

            return $"{sql} offset {p_starRow} rows fetch next {p_pageSize} rows only";
        }

        public override Task<int> NewSequence(string p_seqName)
        {
            if (!string.IsNullOrEmpty(p_seqName))
                return GetScalar<int>($"SELECT NEXT VALUE FOR [{p_seqName}]");
            return Task.FromResult(0);
        }
        #endregion

        #region 表结构
        const string _sqlAllTbls = "select name from sysobjects where xtype='U' or xtype='V'";
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

        public override async Task<IReadOnlyDictionary<string, TableSchema>> GetDbSchema()
        {
            try
            {
                var schema = new Dictionary<string, TableSchema>(StringComparer.OrdinalIgnoreCase);
                await OpenConnection();
                using (var cmd = _conn.CreateCommand())
                {
                    // 所有表名
                    cmd.CommandText = _sqlAllTbls;
                    List<string> tbls = new List<string>();
                    DbDataReader reader;
                    using (reader = await cmd.ExecuteReaderAsync())
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
                        schema[tbl] = await GetTblOrViewSchema(tbl, cmd);
                    }
                }
                return schema;
            }
            catch (Exception ex)
            {
                throw new Exception("SqlServerAccess.GetDbSchema异常：\r\n" + ex.Message);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        public override Task<List<string>> GetAllTableNames()
        {
            return FirstCol<string>(_sqlAllTbls);
        }

        public override async Task<TableSchema> GetTableSchema(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            try
            {
                await OpenConnection();
                using (var cmd = _conn.CreateCommand())
                {
                    return await GetTblOrViewSchema(p_tblName, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SqlServerAccess.GetTableSchema异常：\r\n" + ex.Message);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        public override async Task SyncDbTime()
        {
            Kit.Now = await GetScalar<DateTime>("select getdate()");
        }

        async Task<TableSchema> GetTblOrViewSchema(string p_tblName, DbCommand p_cmd)
        {
            DbDataReader reader;
            TableSchema tblCols = null;

            // 表注释
            string sql = @"select a.name,g.[value] from
	( select object_id, name from sys.tables union select object_id, name from sys.views ) a
	left join sys.extended_properties g on ( a.object_id = g.major_id AND g.minor_id = 0 ) 
where a.name= '{0}'";

            p_cmd.CommandText = string.Format(sql, p_tblName);
            using (reader = await p_cmd.ExecuteReaderAsync())
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
            p_cmd.CommandText = string.Format(_sqlCols, tblCols.Name);
            ReadOnlyCollection<DbColumn> cols;
            using (reader = await p_cmd.ExecuteReaderAsync())
            {
                cols = reader.GetColumnSchema();
            }

            // 表的主键
            p_cmd.CommandText = string.Format(_sqlPk, tblCols.Name);
            List<string> pk = new List<string>();
            using (reader = await p_cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        pk.Add(reader.GetString(0));
                    }
                }
                else
                {
                    // 视图无法查找主键，id默认为主键
                    var idCol = (from c in cols
                                 where c.ColumnName.Equals("id", StringComparison.OrdinalIgnoreCase)
                                 select c).FirstOrDefault();
                    if (idCol != null)
                    {
                        pk.Add(idCol.ColumnName);
                    }
                }
            }

            foreach (var colSchema in cols)
            {
                TableCol col = new TableCol(tblCols);
                col.Name = colSchema.ColumnName;
                col.Type = GetColumnType(colSchema);

                // character_maximum_length
                if (colSchema.ColumnSize.HasValue)
                    col.Length = colSchema.ColumnSize.Value;

                if (colSchema.AllowDBNull.HasValue)
                    col.Nullable = colSchema.AllowDBNull.Value;

                // 读取列结构
                p_cmd.CommandText = string.Format(_sqlComment, tblCols.Name, colSchema.ColumnName);
                using (reader = await p_cmd.ExecuteReaderAsync())
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
        #endregion
    }
}