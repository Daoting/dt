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
            return $"select * from ({p_sql}) a limit {p_pageSize} offset {p_starRow}";
        }

        public override Task<int> NewSequence(string p_seqName)
        {
            if (!string.IsNullOrEmpty(p_seqName))
                return GetScalar<int>($"select nextval('{p_seqName}')");
            return Task.FromResult(0);
        }
        #endregion

        #region 表结构
        const string _sqlAllTbls = "select tablename from pg_tables where schemaname='public' union select viewname from pg_views where schemaname='public'";
        const string _sqlCols = "select * from \"{0}\" where 1!=1";
        const string _sqlDefaultVal = @"select column_default from information_schema.columns where (table_schema, table_name, column_name) = ('public', '{0}', '{1}')";

        const string _sqlComment = @"
select col_description(a.attrelid,a.attnum)
from pg_class as c,pg_attribute as a 
where a.attrelid = c.oid
      and a.attnum>0 
      and c.relname = '{0}'
      and a.attname='{1}'
";

        const string _sqlPk = @"
select kcu.column_name
from information_schema.table_constraints tco
join information_schema.key_column_usage kcu 
     on kcu.constraint_name = tco.constraint_name
     and kcu.constraint_schema = tco.constraint_schema
     and kcu.constraint_name = tco.constraint_name
where tco.constraint_type = 'PRIMARY KEY'
      and kcu.table_schema='public'
      and kcu.table_name='{0}'
";

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
                        schema[tbl] = await GetTblOrViewSchema(tbl, cmd);
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
                    return await GetTblOrViewSchema(p_tblName, cmd);
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

        async Task<TableSchema> GetTblOrViewSchema(string p_tblName, DbCommand p_cmd)
        {
            DbDataReader reader;
            TableSchema tblCols = null;

            // 表注释
            p_cmd.CommandText = string.Format(@"
select tb.table_name,d.description,tb.table_type
from information_schema.tables tb
     join pg_class c on c.relname = tb.table_name
     left join pg_description d on d.objoid = c.oid and d.objsubid = '0'
where tb.table_schema = 'public' and lower(tb.table_name)='{0}'", p_tblName.ToLower());

            bool isView = false;
            using (reader = await p_cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows && reader.Read())
                {
                    // 原始名称
                    if (!reader.IsDBNull(0))
                        tblCols = new TableSchema(reader.GetString(0), DatabaseType.PostgreSql);

                    // 注释
                    if (!reader.IsDBNull(1))
                        tblCols.Comments = reader.GetString(1);

                    if (!reader.IsDBNull(2))
                        isView = reader.GetString(2) == "VIEW";
                }
            }
            if (tblCols == null)
                return null;

            // 所有列
            // 建表时的SQL中表名用引号括起来了，pg不外加引号都为小写，Oracle不外加引号都为大写
            p_cmd.CommandText = string.Format(_sqlCols, tblCols.Name);

            ReadOnlyCollection<DbColumn> cols;
            using (reader = await p_cmd.ExecuteReaderAsync(_cmdBehavior))
            {
                cols = reader.GetColumnSchema();
            }

            // 表的主键
            List<string> pk = new List<string>();
            if (!isView)
            {
                p_cmd.CommandText = string.Format(_sqlPk, tblCols.Name);
                using (reader = await p_cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            pk.Add(reader.GetString(0));
                        }
                    }
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

            foreach (var colSchema in cols)
            {
                TableCol col = new TableCol(tblCols);
                col.Name = colSchema.ColumnName;
                col.Type = isView ? colSchema.DataType : GetColumnType(colSchema);

                // character_maximum_length
                if (colSchema.ColumnSize.HasValue)
                    col.Length = colSchema.ColumnSize.Value;

                if (colSchema.AllowDBNull.HasValue)
                    col.Nullable = colSchema.AllowDBNull.Value;

                // 列默认值
                p_cmd.CommandText = string.Format(_sqlDefaultVal, tblCols.Name, colSchema.ColumnName);
                using (reader = await p_cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            col.Default = reader.GetString(0);
                    }
                }

                // 列注释
                p_cmd.CommandText = string.Format(_sqlComment, tblCols.Name, colSchema.ColumnName);
                using (reader = await p_cmd.ExecuteReaderAsync())
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
        #endregion
    }
}