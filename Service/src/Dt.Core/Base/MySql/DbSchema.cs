#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using MySqlConnector;
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
        /// <summary>
        /// 默认库的所有表结构，键名为小写表名
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
                Log.Fatal(e, "同步时间失败！");
                throw;
            }
        }

        /// <summary>
        /// 加载表结构信息（已调整到最优）
        /// </summary>
        /// <returns>返回加载结果信息</returns>
        public static string LoadSchema()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var schema = new Dictionary<string, TableSchema>();
            using (MySqlConnection conn = new MySqlConnection(MySqlAccess.DefaultConnStr))
            {
                conn.Open();
                Database = conn.Database;
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    // 原来通过系统表information_schema.columns获取结构，为准确获取与c#的映射类型采用当前方式

                    // 所有表名
                    cmd.CommandText = $"SELECT table_name FROM information_schema.tables WHERE table_schema='{conn.Database}'";
                    List<string> tbls = new List<string>();
                    MySqlDataReader reader;
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
                        TableSchema tblCols = new TableSchema(tbl);
                        cmd.CommandText = $"SELECT * FROM {tbl} WHERE false";
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
                            cmd.CommandText = $"SELECT column_default,column_comment FROM information_schema.columns WHERE table_schema='{conn.Database}' and table_name='{tbl}' and column_name='{colSchema.ColumnName}'";
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
                        schema[tbl] = tblCols;
                    }

                    // 取Db时间
                    cmd.CommandText = "select now()";
                    Kit.Now = (DateTime)cmd.ExecuteScalar();
                }
            }
            stopwatch.Stop();
            Schema = schema;
            return $"加载 {Database} 的表结构用时 {stopwatch.ElapsedMilliseconds} 毫秒";
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
}
