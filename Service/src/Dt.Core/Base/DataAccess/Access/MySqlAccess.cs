#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dapper;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data.Common;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// MySql数据库访问类，全部采用异步操作
    /// 基于开源项目 MySqlConnector 和 Dapper
    /// </summary>
    class MySqlAccess : DataAccess
    {
        #region 构造方法
        public MySqlAccess(DbInfo p_info)
            : base(p_info)
        {
        }
        #endregion

        #region 重写
        protected override DbConnection CreateConnection()
            => new MySqlConnection(DbInfo.ConnStr);

        protected override string GetPageSql(int p_starRow, int p_pageSize, string p_sql)
        {
            return $"select * from ({p_sql}) a limit {p_starRow},{p_pageSize}";
        }

        public override Task<int> NewSequence(string p_seqName)
        {
            if (!string.IsNullOrEmpty(p_seqName))
                return GetScalar<int>($"select nextval('{p_seqName}')");
            return Task.FromResult(0);
        }
        #endregion

        #region 表结构
        const string _sqlAllTbls = "select table_name from information_schema.tables where table_schema='{0}'";
        const string _sqlCols = "select * from `{0}` where 1!=1";
        const string _sqlComment = "select column_default,column_comment from information_schema.columns where table_schema='{0}' and table_name='{1}' and column_name='{2}'";

        public override async Task<IReadOnlyDictionary<string, TableSchema>> GetDbSchema()
        {
            try
            {
                var schema = new Dictionary<string, TableSchema>(StringComparer.OrdinalIgnoreCase);
                await OpenConnection();
                using (var cmd = _conn.CreateCommand())
                {
                    // 原来通过系统表information_schema.columns获取结构，为准确获取与c#的映射类型采用当前方式

                    // 所有表名包括视图
                    cmd.CommandText = string.Format(_sqlAllTbls, _conn.Database);
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
                throw new Exception("MySqlAccess.GetDbSchema异常：\r\n" + ex.Message);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        public override async Task<List<string>> GetAllTableNames()
        {
            await OpenConnection();
            return await FirstCol<string>(string.Format(_sqlAllTbls, _conn.Database));
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
                throw new Exception("MySqlAccess.GetTableSchema异常：\r\n" + ex.Message);
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
            ReadOnlyCollection<DbColumn> cols;
            TableSchema tblCols = null;

            // 表注释
            p_cmd.CommandText = $"SELECT table_name,table_comment FROM information_schema.tables WHERE table_schema='{_conn.Database}' and table_name='{p_tblName}'";
            using (reader = await p_cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows && reader.Read())
                {
                    // 原始名称
                    if (!reader.IsDBNull(0))
                        tblCols = new TableSchema(reader.GetString(0), DatabaseType.MySql);

                    // 注释
                    if (!reader.IsDBNull(1))
                        tblCols.Comments = reader.GetString(1);
                }
            }
            if (tblCols == null)
                return null;

            // 表结构
            p_cmd.CommandText = string.Format(_sqlCols, tblCols.Name);
            using (reader = await p_cmd.ExecuteReaderAsync())
            {
                cols = reader.GetColumnSchema();
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
                p_cmd.CommandText = string.Format(_sqlComment, _conn.Database, tblCols.Name, colSchema.ColumnName);
                using (reader = await p_cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        // 默认值
                        if (!reader.IsDBNull(0))
                            col.Default = reader.GetString(0);
                        // 字段注释
                        col.Comments = reader.GetString(1);
                    }
                }

                // 是否为主键
                if (colSchema.IsKey.HasValue && colSchema.IsKey.Value)
                    tblCols.PrimaryKey.Add(col);
                else
                    tblCols.Columns.Add(col);
            }
            return tblCols;
        }
        #endregion
    }
}