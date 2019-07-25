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
using System.Diagnostics;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// MySql默认数据库的表结构
    /// </summary>
    public static class DbSchema
    {
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
                    cmd.CommandText = "SELECT\n" +
                                      "	table_name,\n" +
                                      "	column_name,\n" +
                                      "	data_type,\n" +
                                      "	column_key,\n" +
                                      "	character_maximum_length,\n" +
                                      "	is_nullable,\n" +
                                      "	column_comment \n" +
                                      "FROM\n" +
                                      "	information_schema.columns \n" +
                                      "WHERE\n" +
                                      "	table_schema = '" + conn.Database + "'";
                    try
                    {
                        reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string tblName = reader.GetString(0).ToLower();
                                string colName = reader.GetString(1).ToLower();
                                if (!schema.ContainsKey(tblName))
                                    schema[tblName] = new TableSchema();

                                TableSchema cols = schema[tblName];
                                TableCol col = new TableCol();
                                col.Name = colName;
                                col.DbTypeName = reader.GetString(2);

                                // 是否为主键
                                if (!reader.IsDBNull(3) && reader.GetString(3) == "PRI")
                                    cols.PrimaryKey.Add(col);
                                else
                                    cols.Columns.Add(col);

                                // character_maximum_length
                                if (!reader.IsDBNull(4))
                                    col.Length = reader.GetInt64(4);

                                // is_nullable
                                if (!reader.IsDBNull(5))
                                    col.Nullable = reader.GetString(5).ToLower() == "yes";

                                // column_comment
                                if (!reader.IsDBNull(6))
                                    col.Comments = reader.GetString(6);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Close();
                    }

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
            throw new Exception(string.Format("未找到表{0}的结构信息！", p_tblName));
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
        /// mysql中数据类型名称
        /// </summary>
        public string DbTypeName { get; set; }

        /// <summary>
        /// 列长度，只字符类型有效
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// 列是否允许为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 列类型
        /// </summary>
        public Type Type
        {
            get { return DbTypeConverter.GetTypeByDbTypeName(DbTypeName); }
        }

        /// <summary>
        /// 列类型名称
        /// </summary>
        public string TypeName
        {
            get { return DbTypeConverter.GetTypeNameByDbTypeName(DbTypeName); }
        }
    }
}
