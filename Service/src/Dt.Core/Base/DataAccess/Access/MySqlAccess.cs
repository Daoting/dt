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

        protected override string GetPageSql(int p_starRow, int p_pageSize, string p_keyOrSql)
        {
            return $"select * from ({GetSql(p_keyOrSql)}) a limit {p_starRow},{p_pageSize}";
        }
        #endregion

        #region 表结构
        const string _sqlAllTbls = "SELECT table_name FROM information_schema.tables WHERE table_schema='{0}'";
        const string _sqlCols = "select * from `{0}` where 1!=1";
        const string _sqlComment = "SELECT column_default,column_comment FROM information_schema.columns WHERE table_schema='{0}' and table_name='{1}' and column_name='{2}'";

        /// <summary>
        /// 获取数据库所有表结构信息（已调整到最优）
        /// </summary>
        /// <returns>返回加载结果信息</returns>
        public override IReadOnlyDictionary<string, TableSchema> GetDbSchema()
        {
            var schema = new Dictionary<string, TableSchema>(StringComparer.OrdinalIgnoreCase);
            using (MySqlConnection conn = new MySqlConnection(DbInfo.ConnStr))
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                // 原来通过系统表information_schema.columns获取结构，为准确获取与c#的映射类型采用当前方式

                // 所有表名
                cmd.CommandText = string.Format(_sqlAllTbls, conn.Database);
                List<string> tbls = new List<string>();
                MySqlDataReader reader;
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
                    TableSchema tblCols = new TableSchema(tbl, DatabaseType.MySql);
                    cmd.CommandText = string.Format(_sqlCols, tbl);
                    ReadOnlyCollection<DbColumn> cols;
                    using (reader = cmd.ExecuteReader())
                    {
                        cols = reader.GetColumnSchema();
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
                        cmd.CommandText = string.Format(_sqlComment, conn.Database, tbl, colSchema.ColumnName);
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
                        if (colSchema.IsKey.HasValue && colSchema.IsKey.Value)
                            tblCols.PrimaryKey.Add(col);
                        else
                            tblCols.Columns.Add(col);
                    }
                    schema[tbl] = tblCols;
                }

                // 取Db时间
                cmd.CommandText = "select now()";
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
            using (MySqlConnection conn = new MySqlConnection(DbInfo.ConnStr))
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                // 所有表名
                cmd.CommandText = string.Format(_sqlAllTbls, conn.Database);
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

            using (MySqlConnection conn = new MySqlConnection(DbInfo.ConnStr))
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                MySqlDataReader reader;
                ReadOnlyCollection<DbColumn> cols;
                TableSchema tblCols = null;

                // 表注释
                cmd.CommandText = $"SELECT table_name,table_comment FROM information_schema.tables WHERE table_schema='{conn.Database}' and table_name='{p_tblName}'";
                using (reader = cmd.ExecuteReader())
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
                cmd.CommandText = string.Format(_sqlCols, tblCols.Name);
                using (reader = cmd.ExecuteReader())
                {
                    cols = reader.GetColumnSchema();
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
                    cmd.CommandText = string.Format(_sqlComment, conn.Database, tblCols.Name, colSchema.ColumnName);
                    using (reader = cmd.ExecuteReader())
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

            CommandDefinition cmd;
            try
            {
                await OpenConnection();
                cmd = CreateCommand($"SELECT count(*) FROM information_schema.tables WHERE table_schema='{_conn.Database}' and lower(table_name)='{p_tblName.ToLower()}'", null, false);
                var cnt = await _conn.ExecuteScalarAsync<int>(cmd);
                return cnt > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("MySqlAccess.ExistTable异常：\r\n" + ex.Message);
            }
            finally
            {
                ReleaseConnection();
            }
        }
        #endregion
    }
}