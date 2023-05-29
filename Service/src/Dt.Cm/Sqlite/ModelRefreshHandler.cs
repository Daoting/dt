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

namespace Dt.Cm
{
    /// <summary>
    /// Sqlite模型文件生成类
    /// </summary>
    public class ModelRefreshHandler : IRemoteEventHandler<ModelRefreshEvent>
    {
        #region 成员变量
        // 表名及列的数据少，不需要索引！！！
        const string _createOmTable = "CREATE TABLE OmTable (\n" +
                                            "ID integer primary key not null,\n" +
                                            "Name text,\n" +
                                            "Type integer,\n" +
                                            "DbKey text)";
        const string _insertOmTable = "insert into OmTable (ID,Name,Type,DbKey) values (:ID,:Name,:Type,:DbKey)";

        const string _createOmColumn = "CREATE TABLE OmColumn (\n" +
                                            "ID integer primary key autoincrement not null,\n" +
                                            "TableID integer,\n" +
                                            "ColName text,\n" +
                                            "DbType text,\n" +
                                            "IsPrimary integer,\n" +
                                            "Length integer,\n" +
                                            "Nullable integer)";
        const string _insertOmColumn = "insert into OmColumn (TableID,ColName,DbType,IsPrimary,Length,Nullable) values (:TableID,:ColName,:DbType,:IsPrimary,:Length,:Nullable)";
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
            DbSchema.RefreshSchema();
            sb.AppendLine($"刷新表结构缓存 {watch.ElapsedMilliseconds} 毫秒");

            // 创建Data目录
            string path = SqliteModelHandler.ModelPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string dbFile = System.IO.Path.Combine(path, p_event.Version + ".db");
            var handler = Kit.GetService<SqliteModelHandler>();

            bool trace = Kit.TraceSql;
            Kit.TraceSql = false;

            try
            {
                using (var conn = new SqliteConnection($"Data Source={dbFile}"))
                {
                    conn.Open();
                    sb.AppendLine("开始导出模型及数据");

                    using var tran = conn.BeginTransaction();

                    #region 表结构模型
                    // 加载global.json配置中 ExportToModel=true 的所有库的表结构

                    // 创建 OmTable OmColumn 表结构
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = _createOmTable;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = _createOmColumn;
                        cmd.ExecuteNonQuery();
                    }

                    List<OmTable> tbls = new List<OmTable>();
                    List<OmColumn> cols = new List<OmColumn>();

                    // 加载默认库表结构
                    LoadSchema(DbSchema.Schema, tbls, cols, Kit.DefaultDbInfo);

                    // 默认库键名
                    var dbConn = Kit.Config["DbKey"];
                    foreach (var item in Kit.AllDbInfo)
                    {
                        // 排除默认库表结构 和 不需要导出的库
                        if (dbConn.Equals(item.Key, StringComparison.OrdinalIgnoreCase)
                            || !item.Value.ExportToModel)
                            continue;

                        var schema = Kit.NewDataAccess(item.Key).GetDbSchema();
                        LoadSchema(schema, tbls, cols, item.Value);
                    }

                    InsertSchema(conn, tbls, cols);
                    sb.AppendFormat("导出表结构：{0}张表，{1}个字段\r\n", tbls.Count, cols.Count);
                    #endregion

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
                using (FileStream inFile = File.Open(dbFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                handler.Version = p_event.Version;
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
                Kit.TraceSql = trace;
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
            var dbConn = Kit.Config.GetValue<string>($"SqliteModel:{p_tblName}:DbKey");
            var select = Kit.Config.GetValue<string>($"SqliteModel:{p_tblName}:Data");
            if (string.IsNullOrEmpty(select))
                return 0;

            // 连接不同的库
            var da = Kit.NewDataAccess(dbConn);

            Table tbl = await da.Query(select);
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
        /// <param name="p_schema"></param>
        /// <param name="p_tbls"></param>
        /// <param name="p_cols"></param>
        /// <param name="p_dbInfo"></param>
        static void LoadSchema(IReadOnlyDictionary<string, TableSchema> p_schema, List<OmTable> p_tbls, List<OmColumn> p_cols, DbInfo p_dbInfo)
        {
            OmColumn col;
            foreach (var item in p_schema.Values)
            {
                var tbl = new OmTable();
                tbl.ID = p_tbls.Count + 1;
                tbl.Name = item.Name;
                tbl.Type = (int)p_dbInfo.Type;
                tbl.DbKey = p_dbInfo.Key;

                p_tbls.Add(tbl);

                foreach (var cs in item.PrimaryKey)
                {
                    col = new OmColumn();
                    col.TableID = tbl.ID;
                    col.ColName = cs.Name;
                    col.DbType = Table.GetColTypeAlias(cs.Type);
                    col.IsPrimary = true;
                    col.Length = cs.Length;
                    col.Nullable = cs.Nullable;

                    p_cols.Add(col);
                }

                foreach (var cs in item.Columns)
                {
                    col = new OmColumn();
                    col.TableID = tbl.ID;
                    col.ColName = cs.Name;
                    col.DbType = Table.GetColTypeAlias(cs.Type);
                    col.IsPrimary = false;
                    col.Length = cs.Length;
                    col.Nullable = cs.Nullable;

                    p_cols.Add(col);
                }
            }
        }

        /// <summary>
        /// 批量插入表结构信息
        /// </summary>
        /// <param name="p_conn"></param>
        /// <param name="p_tbls"></param>
        /// <param name="p_cols"></param>
        static void InsertSchema(SqliteConnection p_conn, List<OmTable> p_tbls, List<OmColumn> p_cols)
        {
            using (var cmd = p_conn.CreateCommand())
            {
                cmd.Parameters.Add("ID", SqliteType.Integer);
                cmd.Parameters.Add("Name", SqliteType.Text);
                cmd.Parameters.Add("Type", SqliteType.Integer);
                cmd.Parameters.Add("DbKey", SqliteType.Text);

                cmd.CommandText = _insertOmTable;
                foreach (var tbl in p_tbls)
                {
                    cmd.Parameters["ID"].Value = tbl.ID;
                    cmd.Parameters["Name"].Value = tbl.Name;
                    cmd.Parameters["Type"].Value = tbl.Type;
                    cmd.Parameters["DbKey"].Value = tbl.DbKey;
                    cmd.ExecuteNonQuery();
                }
            }

            using (var cmd = p_conn.CreateCommand())
            {
                cmd.Parameters.Add("TableID", SqliteType.Integer);
                cmd.Parameters.Add("ColName", SqliteType.Text);
                cmd.Parameters.Add("DbType", SqliteType.Text);
                cmd.Parameters.Add("IsPrimary", SqliteType.Integer);
                cmd.Parameters.Add("Length", SqliteType.Integer);
                cmd.Parameters.Add("Nullable", SqliteType.Integer);

                cmd.CommandText = _insertOmColumn;
                foreach (var col in p_cols)
                {
                    cmd.Parameters["TableID"].Value = col.TableID;
                    cmd.Parameters["ColName"].Value = col.ColName;
                    cmd.Parameters["DbType"].Value = col.DbType;
                    cmd.Parameters["IsPrimary"].Value = col.IsPrimary;
                    cmd.Parameters["Length"].Value = col.Length;
                    cmd.Parameters["Nullable"].Value = col.Nullable;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        class OmTable
        {
            /// <summary>
            /// 主键
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// 表名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 数据库类型
            /// </summary>
            public int Type { get; set; }

            /// <summary>
            /// 数据源键名
            /// </summary>
            public string DbKey { get; set; }
        }

        class OmColumn
        {
            /// <summary>
            /// 主键
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// 所属表ID
            /// </summary>
            public int TableID { get; set; }

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
        }
    }
}
