#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Data.Common;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Oracle数据库访问类，基于ODP.Net core，不用安装Oracle客户端
    /// </summary>
    class OracleAccess : DataAccess
    {
        #region 构造方法
        public OracleAccess(DbInfo p_info)
            : base(p_info)
        {
        }
        #endregion

        #region 重写
        protected override DbConnection CreateConnection()
            => new OracleConnection(DbInfo.ConnStr);

        protected override Type GetColumnType(DbColumn p_col)
        {
            /* number类型的c#类型的对应关系
             * number(1-4)   Int16/shot
             * number(5-9)   Int32/int
             * number(10-19) Int64/long 但新版 ODP.Net 将19位映射成 decimal
             * number(20-)   decimal
             */

            if (p_col.DataType == typeof(decimal))
            {
                // ODP.Net 原来对于 number(19) 映射为long，新版变成 decimal，无用
                // 无法判断number的位数，只能将19及以上的都映射为 long，此处有坑！！！
                return (p_col.AllowDBNull.HasValue && p_col.AllowDBNull.Value) ? typeof(long?) : typeof(long);
            }

            // char(1) 不一定是bool，放在客户端反序列化时根据Entity处理
            //if (p_col.DataType == typeof(string)
            //    && p_col.ColumnSize.HasValue
            //    && p_col.ColumnSize.Value == 1)
            //{
            //    // char(1)
            //    return (p_col.AllowDBNull.HasValue && p_col.AllowDBNull.Value) ? typeof(bool?) : typeof(bool);
            //}

            if (p_col.AllowDBNull.HasValue
                && p_col.AllowDBNull.Value
                && p_col.DataType.IsValueType)
            {
                // 可为null的值类型
                return typeof(Nullable<>).MakeGenericType(p_col.DataType);
            }

            return p_col.DataType;
        }

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
        const string _sqlAllTbls = "select table_name from user_tables union select view_name from user_views";
        const string _sqlCols = "select * from \"{0}\" where 1!=1";
        const string _sqlDef = "select column_name,data_default from user_tab_columns where table_name = '{0}'";
        const string _sqlComment = "select column_name,comments from user_col_comments where table_name = '{0}'";
        const string _sqlPk = "select cu.column_name from user_cons_columns cu, user_constraints au where cu.constraint_name = au.constraint_name AND au.constraint_type = 'P' AND cu.table_name='{0}'";

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

                    Log.Information("共 {0} 张表或视图", tbls.Count);

                    // 表结构
                    for (int i = 0; i < tbls.Count; i++)
                    {
                        var tbl = tbls[i];
                        Log.Information("导出 {0} {1}/{2}", tbl, i + 1, tbls.Count);
                        var vs = await GetTblOrViewSchema(tbl, cmd);
                        if (vs != null)
                        {
                            schema[tbl] = vs;
                        }
                    }

                    Log.Information("实际导出 {0} 张", schema.Count);
                }
                return schema;
            }
            catch (Exception ex)
            {
                throw new Exception("OracleAccess.GetDbSchema异常：\r\n" + ex.Message);
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
                throw new Exception("OracleAccess.GetTableSchema异常：\r\n" + ex.Message);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        public override async Task SyncDbTime()
        {
            Kit.Now = await GetScalar<DateTime>("select sysdate from dual");
        }

        async Task<TableSchema> GetTblOrViewSchema(string p_tblName, DbCommand p_cmd)
        {
            DbDataReader reader = null;
            TableSchema tblCols = null;

            try
            {
                // 表注释
                p_cmd.CommandText = $"select table_name,comments from user_tab_comments where LOWER(table_name)='{p_tblName.ToLower()}'";
                using (reader = await p_cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        // 原始名称
                        if (!reader.IsDBNull(0))
                            tblCols = new TableSchema(reader.GetString(0), DatabaseType.Oracle);

                        // 注释
                        if (!reader.IsDBNull(1))
                            tblCols.Comments = reader.GetString(1);
                    }
                }
                if (tblCols == null)
                    return null;

                // 所有列
                // 建表时的SQL中表名用引号括起来了，则此处报 Oracle ORA-00942: table or view does not exist
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

                // 字段默认值
                Dictionary<string, string> defVals = new Dictionary<string, string>();
                p_cmd.CommandText = string.Format(_sqlDef, tblCols.Name);
                using (reader = await p_cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                                defVals.Add(reader.GetString(0), reader.GetString(1));
                        }
                    }
                }

                // 字段注释
                Dictionary<string, string> comments = new Dictionary<string, string>();
                p_cmd.CommandText = string.Format(_sqlComment, tblCols.Name);
                using (reader = await p_cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                                comments.Add(reader.GetString(0), reader.GetString(1));
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

                    // 默认值
                    if (defVals.TryGetValue(colSchema.ColumnName, out var def))
                        col.Default = def;

                    // 字段注释
                    if (comments.TryGetValue(colSchema.ColumnName, out var cmts))
                    {
                        col.Comments = cmts;

                        // oracle char(1) 注释以 #bool# 开头时，bool类型
                        if (!string.IsNullOrEmpty(cmts)
                            && cmts.StartsWith("#bool#", StringComparison.OrdinalIgnoreCase)
                            && col.Type == typeof(string)
                            && col.Length == 1)
                        {
                            col.Type = typeof(bool);
                        }
                    }

                    // 是否为主键
                    if (pk.Contains(colSchema.ColumnName))
                        tblCols.PrimaryKey.Add(col);
                    else
                        tblCols.Columns.Add(col);
                }
            }
            catch (Exception ex)
            {
                // 出现异常，重置null，可能视图编译错误等
                tblCols = null;
                Log.Error(ex, $"获取{p_tblName}的表结构异常！");
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }
            return tblCols;
        }
        #endregion
    }
}