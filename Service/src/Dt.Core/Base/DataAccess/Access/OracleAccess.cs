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
            if (p_col.DataType == typeof(decimal))
            {
                // number(19)
                return (p_col.AllowDBNull.HasValue && p_col.AllowDBNull.Value) ? typeof(long?) : typeof(long);
            }

            if (p_col.DataType == typeof(string)
                && p_col.ColumnSize.HasValue
                && p_col.ColumnSize.Value == 1)
            {
                // char(1)
                return (p_col.AllowDBNull.HasValue && p_col.AllowDBNull.Value) ? typeof(bool?) : typeof(bool);
            }

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
        const string _sqlAllTbls = "select table_name from user_tables";
        const string _sqlCols = "select * from \"{0}\" where 1!=1";
        const string _sqlComment = "select a.data_default, b.comments from user_tab_columns a, user_col_comments b where a.table_name=b.table_name AND a.column_name=b.column_name AND a.table_name='{0}' AND a.column_name='{1}'";
        const string _sqlPk = "select cu.column_name from user_cons_columns cu, user_constraints au where cu.constraint_name = au.constraint_name AND au.constraint_type = 'P' AND cu.table_name='{0}'";

        /// <summary>
        /// 获取数据库所有表结构信息
        /// </summary>
        /// <returns>返回加载结果信息</returns>
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
                        // 建表时的SQL中表名用引号括起来了，则此处报 Oracle ORA-00942: table or view does not exist
                        cmd.CommandText = string.Format(_sqlCols, tbl);

                        TableSchema tblCols = new TableSchema(tbl, DatabaseType.Oracle);
                        ReadOnlyCollection<DbColumn> cols;
                        using (reader = await cmd.ExecuteReaderAsync())
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

                            // 读取列结构
                            cmd.CommandText = string.Format(_sqlComment, tbl, colSchema.ColumnName);
                            using (reader = await cmd.ExecuteReaderAsync())
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
                    cmd.CommandText = "select sysdate from dual";
                    Kit.Now = (DateTime)await cmd.ExecuteScalarAsync();
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

        /// <summary>
        /// 获取数据库的所有表名
        /// </summary>
        /// <returns></returns>
        public override Task<List<string>> GetAllTableNames()
        {
            return FirstCol<string>(_sqlAllTbls);
        }

        /// <summary>
        /// 获取单个表结构信息
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
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
                    cmd.CommandText = $"select table_name,comments from user_tab_comments where LOWER(table_name)='{p_tblName.ToLower()}'";
                    using (reader = await cmd.ExecuteReaderAsync())
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
                    ReadOnlyCollection<DbColumn> cols;
                    cmd.CommandText = string.Format(_sqlCols, tblCols.Name);
                    using (reader = await cmd.ExecuteReaderAsync())
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

                        // 读取列结构
                        cmd.CommandText = string.Format(_sqlComment, tblCols.Name, colSchema.ColumnName);
                        using (reader = await cmd.ExecuteReaderAsync())
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
            catch (Exception ex)
            {
                throw new Exception("OracleAccess.GetTableSchema异常：\r\n" + ex.Message);
            }
            finally
            {
                ReleaseConnection();
            }
        }
        #endregion
    }
}