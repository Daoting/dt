#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using MySql.Data.MySqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// MySql默认数据库的表结构
    /// </summary>
    public static class DbSchema
    {
        static readonly Dictionary<string, string> _sqlInsert = new Dictionary<string, string>();
        static readonly Dictionary<string, string> _sqlUpdate = new Dictionary<string, string>();
        static readonly Dictionary<string, string> _sqlDel = new Dictionary<string, string>();

        /// <summary>
        /// 默认库的所有表结构
        /// </summary>
        public static IReadOnlyDictionary<string, TableSchema> Schema { get; private set; }

        /// <summary>
        /// 默认库名
        /// </summary>
        public static string Database { get; private set; }

        /// <summary>
        /// 加载默认库的所有表结构，初次Mysql连接
        /// </summary>
        internal static void Init()
        {
            try
            {
                LoadSchema();
                Log.Information("同步时间成功");
            }
            catch (Exception e)
            {
                Log.Fatal($"同步时间失败：{e.Message}");
                throw;
            }
        }

        /// <summary>
        /// 加载系统用户和应用用户的表结构信息（已调整到最优）
        /// </summary>
        /// <returns>返回加载结果信息</returns>
        public static string LoadSchema()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var schema = new Dictionary<string, TableSchema>(StringComparer.OrdinalIgnoreCase);
            MySqlDataReader reader = null;
            using (MySqlConnection conn = new MySqlConnection(Glb.Config["Db"]))
            {
                conn.Open();
                Database = conn.Database;
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    // 原来通过系统表information_schema.columns获取结构，为准确获取与c#的映射类型采用当前方式

                    // 所有表名
                    cmd.CommandText = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{conn.Database}'";
                    List<string> tbls = new List<string>();
                    using (reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                tbls.Add(reader.GetString(0).ToLower());
                            }
                        }
                    }

                    // 表结构
                    foreach (var tbl in tbls)
                    {
                        TableSchema tblCols = new TableSchema();
                        cmd.CommandText = $"SELECT * FROM {tbl} WHERE false";
                        ReadOnlyCollection<DbColumn> cols;
                        using (reader = cmd.ExecuteReader())
                        {
                            cols = reader.GetColumnSchema();
                        }

                        foreach (var colSchema in cols)
                        {
                            TableCol col = new TableCol();
                            col.Name = colSchema.ColumnName.ToLower();

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

                            // 字段注释
                            cmd.CommandText = $"SELECT column_comment FROM information_schema.columns WHERE table_schema='{conn.Database}' and table_name='{tbl}' and column_name='{colSchema.ColumnName}'";
                            col.Comments = (string)cmd.ExecuteScalar();

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
                    Glb.Now = (DateTime)cmd.ExecuteScalar();
                }
            }
            stopwatch.Stop();
            Schema = schema;
            return $"加载[{Database}]表结构用时 {stopwatch.ElapsedMilliseconds} 毫秒";
        }

        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static TableSchema GetTableSchema(string p_tblName)
        {
            TableSchema schema;
            if (Schema.TryGetValue(p_tblName.ToLower(), out schema))
                return schema;
            throw new Exception($"未找到表{p_tblName}的结构信息！");
        }

        /// <summary>
        /// 获取表的完整insert语句模板
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static string GetInsertSql(string p_tblName)
        {
            Check.NotNullOrEmpty(p_tblName);
            string tblName = p_tblName.ToLower();
            string sql;
            if (_sqlInsert.TryGetValue(tblName, out sql))
                return sql;

            StringBuilder insertCol = new StringBuilder();
            StringBuilder insertVal = new StringBuilder();
            var schema = GetTableSchema(tblName);
            foreach (var col in schema.PrimaryKey.Concat(schema.Columns))
            {
                insertCol.Append(col.Name);
                insertCol.Append(",");

                insertVal.Append("@");
                insertVal.Append(col.Name);
                insertVal.Append(",");
            }
            sql = $"insert into `{tblName}` ({insertCol.ToString().TrimEnd(',')}) values ({insertVal.ToString().TrimEnd(',')})";
            _sqlInsert[tblName] = sql;
            return sql;
        }

        /// <summary>
        /// 获取表的完整update语句模板
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static string GetUpdateSql(string p_tblName)
        {
            Check.NotNullOrEmpty(p_tblName);
            string tblName = p_tblName.ToLower();
            string sql;
            if (_sqlUpdate.TryGetValue(tblName, out sql))
                return sql;

            StringBuilder updateVal = new StringBuilder();
            StringBuilder whereVal = new StringBuilder();
            var schema = GetTableSchema(tblName);
            foreach (var col in schema.PrimaryKey)
            {
                if (whereVal.Length > 0)
                    whereVal.Append(" and ");
                whereVal.Append(col.Name);
                whereVal.Append("=@");
                whereVal.Append(col.Name);
            }
            foreach (var col in schema.Columns)
            {
                if (updateVal.Length > 0)
                    updateVal.Append(", ");
                updateVal.Append(col.Name);
                updateVal.Append("=@");
                updateVal.Append(col.Name);
            }
            sql = $"update `{tblName}` set {updateVal} where {whereVal}";
            _sqlUpdate[tblName] = sql;
            return sql;
        }

        /// <summary>
        /// 获取表的delete语句模板
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static string GetDeleteSql(string p_tblName)
        {
            Check.NotNullOrEmpty(p_tblName);
            string tblName = p_tblName.ToLower();
            string sql;
            if (_sqlDel.TryGetValue(tblName, out sql))
                return sql;

            StringBuilder whereVal = new StringBuilder();
            var schema = GetTableSchema(tblName);
            foreach (var col in schema.PrimaryKey)
            {
                if (whereVal.Length > 0)
                    whereVal.Append(" and ");
                whereVal.Append(col.Name);
                whereVal.Append("=@");
                whereVal.Append(col.Name);
            }
            sql = $"delete from `{tblName}` where {whereVal}";
            _sqlDel[tblName] = sql;
            return sql;
        }

        /// <summary>
        /// 关闭MySql连接池，释放资源
        /// </summary>
        internal static void Close()
        {
            try
            {
                MySqlConnection.ClearAllPools();
            }
            catch { }
        }
    }

    /// <summary>
    /// 存储表结构信息，分主键列列表和普通列列表
    /// </summary>
    public class TableSchema
    {
        /// <summary>
        /// 主键列列表
        /// </summary>
        public List<TableCol> PrimaryKey { get; } = new List<TableCol>();

        /// <summary>
        /// 普通列列表
        /// </summary>
        public List<TableCol> Columns { get; } = new List<TableCol>();
    }

    /// <summary>
    /// 列描述类
    /// </summary>
    public class TableCol
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 列类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 列长度，只字符类型有效
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 列是否允许为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 列类型名称
        /// </summary>
        public string TypeName
        {
            get { return TableKit.GetColTypeAlias(Type); }
        }
    }
}
