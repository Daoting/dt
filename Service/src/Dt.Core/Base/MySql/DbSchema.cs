#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Domain;
using MySql.Data.MySqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        static readonly Dictionary<Type, string> _entityInsert = new Dictionary<Type, string>();
        static readonly Dictionary<Type, string> _entityUpdate = new Dictionary<Type, string>();

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
            throw new Exception($"未找到表{p_tblName}的结构信息！");
        }

        /// <summary>
        /// 获取表的完整insert语句模板
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        public static string GetFullInsertSql(string p_tblName)
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
        public static string GetFullUpdateSql(string p_tblName)
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

        #region 实体
        /// <summary>
        /// 获取实体类型对应的insert语句模板
        /// </summary>
        /// <param name="p_entityType">实体类型</param>
        /// <returns></returns>
        public static string GetEntityInsertSql(Type p_entityType)
        {
            string sql;
            if (_entityInsert.TryGetValue(p_entityType, out sql))
                return sql;

            string tblName = GetEntityTblName(p_entityType);
            StringBuilder insertCol = new StringBuilder();
            StringBuilder insertVal = new StringBuilder();
            var schema = GetTableSchema(tblName);
            foreach (var col in schema.PrimaryKey.Concat(schema.Columns))
            {
                // 忽略没有的属性列
                if (p_entityType.GetProperty(col.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null)
                {
                    // 自动设置创建时间和修改时间
                    if (col.Name == "ctime" || col.Name == "mtime")
                    {
                        insertCol.Append(col.Name);
                        insertCol.Append(",");

                        insertVal.Append("now(),");
                    }
                    continue;
                }

                insertCol.Append(col.Name);
                insertCol.Append(",");

                insertVal.Append("@");
                insertVal.Append(col.Name);
                insertVal.Append(",");
            }
            sql = $"insert into `{tblName}` ({insertCol.ToString().TrimEnd(',')}) values ({insertVal.ToString().TrimEnd(',')})";
            _entityInsert[p_entityType] = sql;
            return sql;
        }

        /// <summary>
        /// 获取实体类型对应的update语句模板
        /// </summary>
        /// <param name="p_entityType">实体类型</param>
        /// <returns></returns>
        public static string GetEntityUpdateSql(Type p_entityType)
        {
            string sql;
            if (_entityUpdate.TryGetValue(p_entityType, out sql))
                return sql;

            string tblName = GetEntityTblName(p_entityType);
            StringBuilder updateVal = new StringBuilder();
            StringBuilder whereVal = new StringBuilder();
            var schema = GetTableSchema(tblName);
            foreach (var col in schema.PrimaryKey)
            {
                if (p_entityType.GetProperty(col.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null)
                    throw new Exception($"实体类型{p_entityType.Name}中缺少主键列{col.Name}！");

                if (whereVal.Length > 0)
                    whereVal.Append(" and ");
                whereVal.Append(col.Name);
                whereVal.Append("=@");
                whereVal.Append(col.Name);
            }
            foreach (var col in schema.Columns)
            {
                // 忽略没有的属性列
                if (p_entityType.GetProperty(col.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null)
                {
                    // 自动设置修改时间
                    if (col.Name == "mtime")
                    {
                        if (updateVal.Length > 0)
                            updateVal.Append(", ");
                        updateVal.Append("mtime=now()");
                    }
                    continue;
                }

                if (updateVal.Length > 0)
                    updateVal.Append(", ");
                updateVal.Append(col.Name);
                updateVal.Append("=@");
                updateVal.Append(col.Name);
            }
            sql = $"update `{tblName}` set {updateVal} where {whereVal}";
            _entityUpdate[p_entityType] = sql;
            return sql;
        }

        /// <summary>
        /// 获取实体类型对应的表名
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static string GetEntityTblName(Type p_type)
        {
            string tblName = p_type.Name.ToLower();
            var tag = p_type.GetCustomAttribute<TagAttribute>(false);
            if (tag != null && !string.IsNullOrEmpty(tag.TblName))
                tblName = tag.TblName.ToLower();
            return tblName;
        }
        #endregion

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
