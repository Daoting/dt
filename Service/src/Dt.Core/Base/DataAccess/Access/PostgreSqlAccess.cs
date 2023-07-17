#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Npgsql;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// PostgreSql数据库访问类，基于Npgsql，不用安装客户端
    /// </summary>
    class PostgreSqlAccess : DataAccess
    {
        #region 构造方法
        public PostgreSqlAccess(DbInfo p_info)
            : base(p_info)
        {
            // Npgsql真变态，必须为 KeyInfo 才能查询到列结构信息！否则AllowDBNull始终null
            // 速度变慢
            _cmdBehavior = CommandBehavior.KeyInfo;
        }
        #endregion

        #region 重写
        protected override DbConnection CreateConnection()
            => new NpgsqlConnection(DbInfo.ConnStr);

        protected override string GetPageSql(int p_starRow, int p_pageSize, string p_sql)
        {
            return $"select * from (select a.*,rownum rn from ({p_sql}) a where rownum <= {p_starRow + p_pageSize}) where rn > {p_starRow}";
        }

        public override Task<int> NewSequence(string p_seqName)
        {
            if (!string.IsNullOrEmpty(p_seqName))
                return GetScalar<int>($"select {p_seqName}.nextval from dual");
            return Task.FromResult(0);
        }
        #endregion

        #region 表结构
        const string _sqlAllTbls = "select tablename from pg_tables where schemaname='public'";
        const string _sqlCols = "select * from \"{0}\" where 1!=1";
        const string _sqlDefaultVal = @"select column_default from information_schema.columns where (table_schema, table_name, column_name) = ('public', '{0}', '{1}')";

        const string _sqlComment = @"
select col_description(a.attrelid,a.attnum)
from pg_class as c,pg_attribute as a 
where a.attrelid = c.oid
      and a.attnum>0 
      and c.relname = '{0}'
      and a.attname='{1}'";

        const string _sqlPk = @"
select attr.attname as colname from pg_constraint 
inner join pg_class on pg_constraint.conrelid = pg_class.oid 
inner join pg_attribute attr on attr.attrelid = pg_class.oid and attr.attnum = pg_constraint.conkey[1] 
where pg_class.relname = '{0}' and pg_constraint.contype='p'";

        public override async Task<IReadOnlyDictionary<string, TableSchema>> GetDbSchema()
        {
            try
            {
                var schema = new Dictionary<string, TableSchema>(StringComparer.OrdinalIgnoreCase);
                await OpenConnection();
                using (var cmd = _conn.CreateCommand())
                {
                    DbDataReader reader = null;

                    // 所有表名
                    cmd.CommandText = _sqlAllTbls;
                    List<string> tbls = new List<string>();
                    using (reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // 原始表名
                                tbls.Add(reader.GetString(0));
                            }
                        }
                    }

                    // 表结构
                    foreach (var tbl in tbls)
                    {
                        // 建表时的SQL中表名用引号括起来了，pg不外加引号都为小写，Oracle不外加引号都为大写
                        cmd.CommandText = string.Format(_sqlCols, tbl);

                        TableSchema tblCols = new TableSchema(tbl, DatabaseType.PostgreSql);
                        ReadOnlyCollection<DbColumn> cols;
                        using (reader = await cmd.ExecuteReaderAsync(_cmdBehavior))
                        {
                            cols = reader.GetColumnSchema();
                        }

                        // 表的主键
                        cmd.CommandText = string.Format(_sqlPk, tbl);
                        List<string> pk = new List<string>();
                        using (reader = await cmd.ExecuteReaderAsync())
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
                            TableCol col = new TableCol(tblCols);
                            col.Name = colSchema.ColumnName;
                            col.Type = GetColumnType(colSchema);

                            // character_maximum_length
                            if (colSchema.ColumnSize.HasValue)
                                col.Length = colSchema.ColumnSize.Value;

                            if (colSchema.AllowDBNull.HasValue)
                                col.Nullable = colSchema.AllowDBNull.Value;

                            // 列默认值
                            cmd.CommandText = string.Format(_sqlDefaultVal, tbl, colSchema.ColumnName);
                            using (reader = await cmd.ExecuteReaderAsync())
                            {
                                if (reader.HasRows && reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        col.Default = reader.GetString(0);
                                }
                            }

                            // 列注释
                            cmd.CommandText = string.Format(_sqlComment, tbl, colSchema.ColumnName);
                            using (reader = await cmd.ExecuteReaderAsync())
                            {
                                if (reader.HasRows && reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        col.Comments = reader.GetString(0);
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
                }
                return schema;
            }
            catch (Exception ex)
            {
                throw new Exception("PostgreSqlAccess.GetDbSchema异常：\r\n" + ex.Message);
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
                    DbDataReader reader;
                    TableSchema tblCols = null;

                    // 表注释
                    cmd.CommandText = string.Format(@"
select tb.table_name,d.description
from information_schema.tables tb
     join pg_class c on c.relname = tb.table_name
     left join pg_description d on d.objoid = c.oid and d.objsubid = '0'
where tb.table_schema = 'public' and lower(tb.table_name)='{0}'", p_tblName.ToLower());

                    using (reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            // 原始名称
                            if (!reader.IsDBNull(0))
                                tblCols = new TableSchema(reader.GetString(0), DatabaseType.PostgreSql);

                            // 注释
                            if (!reader.IsDBNull(1))
                                tblCols.Comments = reader.GetString(1);
                        }
                    }
                    if (tblCols == null)
                        return null;

                    // 所有列
                    ReadOnlyCollection<DbColumn> cols;
                    cmd.CommandText = string.Format(_sqlCols, tblCols.Name);
                    using (reader = await cmd.ExecuteReaderAsync(_cmdBehavior))
                    {
                        cols = reader.GetColumnSchema();
                    }

                    // 表的主键
                    cmd.CommandText = string.Format(_sqlPk, tblCols.Name);
                    List<string> pk = new List<string>();
                    using (reader = await cmd.ExecuteReaderAsync())
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
                        TableCol col = new TableCol(tblCols);
                        col.Name = colSchema.ColumnName;
                        col.Type = GetColumnType(colSchema);

                        // character_maximum_length
                        if (colSchema.ColumnSize.HasValue)
                            col.Length = colSchema.ColumnSize.Value;

                        if (colSchema.AllowDBNull.HasValue)
                            col.Nullable = colSchema.AllowDBNull.Value;

                        // 列默认值
                        cmd.CommandText = string.Format(_sqlDefaultVal, tblCols.Name, colSchema.ColumnName);
                        using (reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    col.Default = reader.GetString(0);
                            }
                        }

                        // 列注释
                        cmd.CommandText = string.Format(_sqlComment, tblCols.Name, colSchema.ColumnName);
                        using (reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                    col.Comments = reader.GetString(0);
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
            catch (Exception ex)
            {
                throw new Exception("PostgreSqlAccess.GetTableSchema异常：\r\n" + ex.Message);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        public override async Task SyncDbTime()
        {
            Kit.Now = await GetScalar<DateTime>("select now()");
        }
        #endregion
    }
}