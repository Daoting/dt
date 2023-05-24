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
    class OracleAccess //: IDataAccess
    {
        #region 构造方法
        public OracleAccess(DbInfo p_info)
        {
            DbInfo = p_info;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 调用每个公共方法后是否自动关闭连接，默认true，false时切记最后手动关闭！
        /// </summary>
        public bool AutoClose { get; set; } = true;

        /// <summary>
        /// 数据库描述信息
        /// </summary>
        public DbInfo DbInfo { get; }
        #endregion


        #region 表结构
        /// <summary>
        /// 获取数据库所有表结构信息
        /// </summary>
        /// <returns>返回加载结果信息</returns>
        public IReadOnlyDictionary<string, TableSchema> GetDbSchema()
        {
            var schema = new Dictionary<string, TableSchema>();
            using (OracleConnection conn = new OracleConnection(DbInfo.ConnStr))
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                OracleDataReader reader = null;

                // 所有主键信息
                cmd.CommandText = "select cu.table_name,cu.column_name from user_cons_columns cu, user_constraints au where cu.constraint_name = au.constraint_name AND au.constraint_type = 'P'";
                Dictionary<string, string> primaryKeys = new Dictionary<string, string>();
                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string tblKey = reader.GetString(reader.GetOrdinal("table_name")).ToLower();
                            string col = reader.GetString(reader.GetOrdinal("column_name"));
                            if (primaryKeys.ContainsKey(tblKey))
                            {
                                primaryKeys[tblKey] += "+" + col;
                            }
                            else
                            {
                                primaryKeys[tblKey] = col;
                            }
                        }
                    }
                }

                // 所有表名
                cmd.CommandText = "SELECT table_name FROM user_tables";
                List<string> tbls = new List<string>();
                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // 表名小写
                            tbls.Add(reader.GetString(0).ToLower());
                        }
                    }
                }

                // 表结构
                foreach (var tbl in tbls)
                {
                    // 建表时的SQL中表名用引号括起来了，则此处报 Oracle ORA-00942: table or view does not exist
                    cmd.CommandText = $"SELECT * FROM {tbl} WHERE 1!=1";

                    TableSchema tblCols = new TableSchema(tbl);
                    ReadOnlyCollection<DbColumn> cols;
                    using (reader = cmd.ExecuteReader())
                    {
                        cols = reader.GetColumnSchema();
                    }

                    // 取表主键串
                    primaryKeys.TryGetValue(tbl, out var priKey);

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
                        cmd.CommandText = $"SELECT a.data_default, b.comments FROM user_tab_columns a, user_col_comments b WHERE a.table_name=b.table_name AND a.column_name=b.column_name AND a.table_name='{tbl.ToUpper()}' AND a.column_name='{colSchema.ColumnName}'";
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
                        if (!string.IsNullOrEmpty(priKey) && priKey.Contains(colSchema.ColumnName))
                            tblCols.PrimaryKey.Add(col);
                        else
                            tblCols.Columns.Add(col);
                    }
                    schema[tbl] = tblCols;
                }

                // 取Db时间
                cmd.CommandText = "select sysdate from dual";
                Kit.Now = (DateTime)cmd.ExecuteScalar();
            }
            return schema;
        }

        /// <summary>
        /// 获取数据库的所有表名
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllTableNames()
        {
            using (OracleConnection conn = new OracleConnection(DbInfo.ConnStr))
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT table_name FROM user_tables";
                List<string> tbls = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // 表名小写
                            tbls.Add(reader.GetString(0).ToLower());
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
        public TableSchema GetTableSchema(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                return null;

            using (OracleConnection conn = new OracleConnection(DbInfo.ConnStr))
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                OracleDataReader reader = null;

                // 主键
                cmd.CommandText = $"select cu.column_name from user_cons_columns cu, user_constraints au where cu.constraint_name = au.constraint_name AND au.constraint_type = 'P' AND cu.table_name='{p_tblName.ToUpper()}'";
                string primaryKey = null;
                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string col = reader.GetString(0);
                            if (primaryKey == null)
                            {
                                primaryKey = col;
                            }
                            else
                            {
                                primaryKey += "+" + col;
                            }
                        }
                    }
                }

                // 所有列
                TableSchema tblCols = new TableSchema(p_tblName);
                ReadOnlyCollection<DbColumn> cols;
                cmd.CommandText = $"SELECT * FROM {p_tblName} WHERE 1!=1";
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
                    cmd.CommandText = $"SELECT a.data_default, b.comments FROM user_tab_columns a, user_col_comments b WHERE a.table_name=b.table_name AND a.column_name=b.column_name AND a.table_name='{p_tblName.ToUpper()}' AND a.column_name='{colSchema.ColumnName}'";
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
                    if (!string.IsNullOrEmpty(primaryKey) && primaryKey.Contains(colSchema.ColumnName))
                        tblCols.PrimaryKey.Add(col);
                    else
                        tblCols.Columns.Add(col);
                }

                return tblCols;
            }
        }
        #endregion
    }
}