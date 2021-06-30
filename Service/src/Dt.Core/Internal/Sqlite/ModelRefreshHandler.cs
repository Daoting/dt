#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// Sqlite模型文件生成类
    /// </summary>
    public class ModelRefreshHandler : IRemoteHandler<ModelRefreshEvent>
    {
        #region 成员变量
        const string _createOmColumn = "CREATE TABLE OmColumn (\n" +
                                            "ID integer primary key autoincrement not null,\n" +
                                            "TabName text,\n" +
                                            "ColName text,\n" +
                                            "DbType text,\n" +
                                            "IsPrimary integer,\n" +
                                            "Length integer,\n" +
                                            "Nullable integer,\n" +
                                            "Comments text)";
        const string _insertOmColumn = "insert into OmColumn (TabName,ColName,DbType,IsPrimary,Length,Nullable,Comments) values (:TabName,:ColName,:DbType,:IsPrimary,:Length,:Nullable,:Comments)";
        #endregion

        public async Task Handle(ModelRefreshEvent p_event)
        {
            if (SqliteModelHandler.Refreshing)
            {
                Log.Warning("模型刷新进行中，无法重复刷新！");
                return;
            }

            // 刷新标志，避免重复刷新
            SqliteModelHandler.Refreshing = true;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("准备更新模型...");
            sb.AppendFormat("模型版本 {0}\r\n", p_event.Version);

            // 刷新表结构缓存
            Stopwatch watch = new Stopwatch();
            watch.Start();
            sb.AppendLine(DbSchema.LoadSchema());

            // 创建Data目录
            string path = SqliteModelHandler.ModelPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string dbFile = System.IO.Path.Combine(path, p_event.Version + ".db");
            var handler = Kit.GetObj<SqliteModelHandler>();

            bool trace = MySqlAccess.TraceSql;
            MySqlAccess.TraceSql = false;
            try
            {
                using (var conn = new SqliteConnection($"Data Source={dbFile}"))
                {
                    conn.Open();
                    sb.AppendLine("开始导出模型及数据");

                    using var tran = conn.BeginTransaction();

                    // 加载global.json中MySql配置节的所有库的表结构
                    // 创建OmColumn表结构
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = _createOmColumn;
                        cmd.ExecuteNonQuery();
                    }
                    List<OmColumn> cols = new List<OmColumn>();
                    foreach (var item in Kit.Config.GetSection("MySql").GetChildren())
                    {
                        LoadSchema(item.Value, cols);
                    }
                    InsertSchema(conn, cols);
                    sb.AppendFormat("创建表OmColumn成功，导出{0}行\r\n", cols.Count);

                    // 加载service.json配置节 SqliteModel 的缓存数据
                    foreach (var item in Kit.Config.GetSection("SqliteModel").GetChildren())
                    {
                        var arr = Kit.Config.GetSection($"SqliteModel:{item.Key}:Create").Get<string[]>();
                        if (arr == null || arr.Length == 0)
                            continue;

                        // 创建表及索引
                        using (var cmd = conn.CreateCommand())
                        {
                            foreach (var sql in arr)
                            {
                                if (!string.IsNullOrEmpty(sql))
                                {
                                    cmd.CommandText = sql;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        sb.AppendFormat("创建表{0}成功，", item.Key);

                        // 导入数据
                        int cnt = await ImportData(item.Key, conn);
                        if (cnt == 0)
                            sb.AppendLine("无数据");
                        else
                            sb.AppendFormat("导出{0}行\r\n", cnt);
                    }

                    tran.Commit();
                }

                // 将结果模型文件内容压缩
                sb.Append("生成压缩文件...");
                using (FileStream inFile = File.OpenRead(dbFile))
                {
                    using (FileStream outFile = File.Create(System.IO.Path.Combine(path, p_event.Version + ".gz")))
                    {
                        using (GZipStream gzStream = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            inFile.CopyTo(gzStream);
                        }
                    }
                }
                sb.AppendLine("成功！");

                // 更新缓存、通知版本变化
                handler.SetVersion(p_event.Version);
                handler.LoadModelFile();
                sb.AppendLine("缓存模型文件成功！");

                watch.Stop();
                sb.AppendLine($"创建结束！用时{watch.ElapsedMilliseconds}毫秒");

                Log.Information(sb.ToString());
            }
            catch
            {
                watch.Stop();
                throw;
            }
            finally
            {
                // 移除标志
                SqliteModelHandler.Refreshing = false;
                MySqlAccess.TraceSql = trace;
            }

            // 删除历史文件
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo fi in dir.EnumerateFiles())
                {
                    if (!fi.Name.StartsWith(p_event.Version))
                    {
                        RemoveReadOnlyFlag(fi.FullName);
                        fi.Delete();
                    }
                }
            }
            catch { }
        }

        async Task<int> ImportData(string p_tblName, SqliteConnection p_conn)
        {
            var dbConn = Kit.Config.GetValue<string>($"SqliteModel:{p_tblName}:DbConn");
            var select = Kit.Config.GetValue<string>($"SqliteModel:{p_tblName}:Data");
            if (string.IsNullOrEmpty(select))
                return 0;

            Table tbl = await new MySqlAccess(dbConn).Query(select);
            if (tbl.Count == 0)
                return 0;

            using (var cmd = p_conn.CreateCommand())
            {
                List<string> columns = new List<string>();
                string colNames = null;
                string paraNames = null;

                // 查询表的所有列
                using (var cmdCol = p_conn.CreateCommand())
                {
                    cmdCol.CommandText = $"SELECT c.name, c.type FROM sqlite_master AS t, pragma_table_info(t.name) AS c WHERE t.type = 'table' and t.name='{p_tblName}'";
                    var colReader = cmdCol.ExecuteReader();
                    if (colReader != null && colReader.FieldCount == 2)
                    {
                        while (colReader.Read())
                        {
                            var colName = colReader[0].ToString();
                            columns.Add(colName);
                            if (colNames == null)
                                colNames = colName;
                            else
                                colNames += "," + colName;
                            if (paraNames == null)
                                paraNames = ":" + colName;
                            else
                                paraNames += ",:" + colName;

                            SqliteType tp = SqliteType.Text;
                            var colType = colReader[1].ToString().ToLower();
                            if (colType == "integer")
                                tp = SqliteType.Integer;
                            else if (colType == "real")
                                tp = SqliteType.Real;
                            cmd.Parameters.Add(colName, tp);
                        }
                    }
                }

                cmd.CommandText = $"insert into {p_tblName} ({colNames}) values ({paraNames})";
                foreach (var row in tbl)
                {
                    foreach (var col in columns)
                    {
                        var obj = row[col];
                        cmd.Parameters[col].Value = (obj == null ? DBNull.Value : obj);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            return tbl.Count;
        }

        /// <summary>
        /// 移除文件的只读属性
        /// </summary>
        /// <param name="p_filePath"></param>
        static void RemoveReadOnlyFlag(string p_filePath)
        {
            FileAttributes fileAttributes = File.GetAttributes(p_filePath);
            if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                fileAttributes &= ~FileAttributes.ReadOnly;
                File.SetAttributes(p_filePath, fileAttributes);
            }
        }

        /// <summary>
        /// 构造表结构信息
        /// </summary>
        /// <param name="p_connStr"></param>
        /// <param name="p_cols"></param>
        static void LoadSchema(string p_connStr, List<OmColumn> p_cols)
        {
            using (MySqlConnection conn = new MySqlConnection(p_connStr))
            {
                conn.Open();
                using (MySqlCommand cmd = conn.CreateCommand())
                {
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
                                tbls.Add(reader.GetString(0).ToLower());
                            }
                        }
                    }

                    // 表结构
                    foreach (var tbl in tbls)
                    {
                        cmd.CommandText = $"SELECT * FROM {tbl} WHERE false";
                        ReadOnlyCollection<DbColumn> cols;
                        using (reader = cmd.ExecuteReader())
                        {
                            cols = reader.GetColumnSchema();
                        }

                        foreach (var colSchema in cols)
                        {
                            OmColumn col = new OmColumn();
                            col.TabName = tbl;
                            col.ColName = colSchema.ColumnName;

                            // 可为null的值类型
                            if (colSchema.AllowDBNull.HasValue && colSchema.AllowDBNull.Value && colSchema.DataType.IsValueType)
                                col.DbType = Table.GetColTypeAlias(typeof(Nullable<>).MakeGenericType(colSchema.DataType));
                            else
                                col.DbType = Table.GetColTypeAlias(colSchema.DataType);

                            // 是否为主键
                            if (colSchema.IsKey.HasValue && colSchema.IsKey.Value)
                                col.IsPrimary = true;

                            // character_maximum_length
                            if (colSchema.ColumnSize.HasValue)
                                col.Length = colSchema.ColumnSize.Value;

                            if (colSchema.AllowDBNull.HasValue)
                                col.Nullable = colSchema.AllowDBNull.Value;

                            // 字段注释
                            cmd.CommandText = $"SELECT column_comment FROM information_schema.columns WHERE table_schema='{conn.Database}' and table_name='{tbl}' and column_name='{colSchema.ColumnName}'";
                            col.Comments = (string)cmd.ExecuteScalar();

                            p_cols.Add(col);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 批量插入表结构信息
        /// </summary>
        /// <param name="p_conn"></param>
        /// <param name="p_cols"></param>
        static void InsertSchema(SqliteConnection p_conn, List<OmColumn> p_cols)
        {
            using (var cmd = p_conn.CreateCommand())
            {
                cmd.Parameters.Add("TabName", SqliteType.Text);
                cmd.Parameters.Add("ColName", SqliteType.Text);
                cmd.Parameters.Add("DbType", SqliteType.Text);
                cmd.Parameters.Add("IsPrimary", SqliteType.Integer);
                cmd.Parameters.Add("Length", SqliteType.Integer);
                cmd.Parameters.Add("Nullable", SqliteType.Integer);
                cmd.Parameters.Add("Comments", SqliteType.Text);

                cmd.CommandText = _insertOmColumn;
                foreach (var col in p_cols)
                {
                    cmd.Parameters["TabName"].Value = col.TabName;
                    cmd.Parameters["ColName"].Value = col.ColName;
                    cmd.Parameters["DbType"].Value = col.DbType;
                    cmd.Parameters["IsPrimary"].Value = col.IsPrimary;
                    cmd.Parameters["Length"].Value = col.Length;
                    cmd.Parameters["Nullable"].Value = col.Nullable;
                    cmd.Parameters["Comments"].Value = col.Comments;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        class OmColumn
        {
            /// <summary>
            /// 主键
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// 所属表名
            /// </summary>
            public string TabName { get; set; }

            /// <summary>
            /// 列名
            /// </summary>
            public string ColName { get; set; }

            /// <summary>
            /// 数据类型
            /// </summary>
            public string DbType { get; set; }

            /// <summary>
            /// 是否为主键
            /// </summary>
            public bool IsPrimary { get; set; }

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
        }
    }
}
