#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Castle.Core.Configuration;
using Dt.Core.EventBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 模型文件处理类
    /// </summary>
    class ModelFileItem
    {
        #region 成员变量
        // 表名及列的数据少，不需要索引！！！
        const string _createOmTable = "CREATE TABLE OmTable (\n" +
                                            "ID integer primary key not null,\n" +
                                            "Name text COLLATE NOCASE,\n" +
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

        List<string> _exportDbs;
        byte[] _data;
        #endregion

        /// <summary>
        /// 是否正在刷新中
        /// </summary>
        public bool IsRefreshing { get; set; }

        /// <summary>
        /// sqlite文件名作为版本号
        /// </summary>
        public string Version { get; private set; }

        public void Init(IConfigurationSection p_cfg)
        {
            _exportDbs = (from item in p_cfg.GetSection("ExportToModel").GetChildren()
                          select item.Value).ToList();

            FileInfo fi = SqliteFileHandler.Path.EnumerateFiles("model_*.gz", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (fi != null)
            {
                Version = fi.Name.Substring(0, fi.Name.Length - 3);
                Log.Information("sqlite文件 {0} ", Version);
            }
            else
            {
                Log.Error("缺少sqlite文件 {0}，客户端连接前务必创建！！！", "model_*");
            }
        }

        public byte[] GetData()
        {
            if (_data == null)
            {
                LoadFile();
            }
            return _data;
        }

        public async Task Refresh()
        {
            if (IsRefreshing)
            {
                Log.Warning("更新模型文件进行中，无法重复更新！");
                return;
            }

            // 刷新标志，避免重复刷新
            IsRefreshing = true;
            var newVer = "model_" + Guid.NewGuid().ToString().Substring(0, 8);
            Log.Information("准备更新 {0}", newVer);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            // 刷新表结构缓存
            Log.Information("开始刷新 {0} 库表结构...", Kit.DefaultDbInfo.Name);
            var defSchema = await Kit.NewDataAccess().GetDbSchema();
            Log.Information("刷新 {0} {1} 毫秒", Kit.DefaultDbInfo.Name, watch.ElapsedMilliseconds);

            string dbFile = System.IO.Path.Combine(SqliteFileHandler.Path.FullName, newVer + ".db");

            bool trace = Kit.TraceSql;
            Kit.TraceSql = false;
            List<OmTable> tbls = new List<OmTable>();
            List<OmColumn> cols = new List<OmColumn>();

            try
            {
                using (var conn = new SqliteConnection($"Data Source={dbFile}"))
                {
                    conn.Open();
                    using var tran = conn.BeginTransaction();

                    // 创建 OmTable OmColumn 表结构
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = _createOmTable;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = _createOmColumn;
                        cmd.ExecuteNonQuery();
                    }

                    // 加载默认库表结构
                    LoadSchema(defSchema, tbls, cols, Kit.DefaultDbInfo);

                    if (_exportDbs != null && _exportDbs.Count > 0)
                    {
                        // 默认库键名
                        var dbConn = Kit.DefaultDbInfo.Name;
                        foreach (var dbKey in _exportDbs)
                        {
                            // 排除默认库、不存在的库
                            if (string.IsNullOrEmpty(dbKey)
                                || dbConn.Equals(dbKey, StringComparison.OrdinalIgnoreCase)
                                || !Kit.AllDbInfo.ContainsKey(dbKey))
                                continue;

                            Log.Information("开始刷新 {0} 库表结构...", dbKey);
                            var startSec = watch.ElapsedMilliseconds;
                            var schema = await Kit.NewDataAccess(dbKey).GetDbSchema();
                            Log.Information("刷新 {0} {1} 毫秒", dbKey, watch.ElapsedMilliseconds - startSec);

                            LoadSchema(schema, tbls, cols, Kit.AllDbInfo[dbKey]);
                        }
                    }

                    InsertSchema(conn, tbls, cols);
                    tran.Commit();
                }

                // 将结果模型文件内容压缩
                string gzFile = newVer + ".gz";
                using (FileStream inFile = File.Open(dbFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (FileStream outFile = File.Create(System.IO.Path.Combine(SqliteFileHandler.Path.FullName, gzFile)))
                using (GZipStream gzStream = new GZipStream(outFile, CompressionMode.Compress))
                {
                    inFile.CopyTo(gzStream);
                }

                // 更新版本
                Version = newVer;
                watch.Stop();
                Log.Information("生成 {0}，{1} 张表，{2} 个字段，{3} 毫秒", gzFile, tbls.Count, cols.Count, watch.ElapsedMilliseconds);
                _data = null;
            }
            catch (Exception ex)
            {
                watch.Stop();
                Log.Error(ex, "更新 {0} 文件失败！", newVer);
                throw;
            }
            finally
            {
                // 移除标志
                IsRefreshing = false;
                Kit.TraceSql = trace;
            }

            // 删除历史文件
            try
            {
                foreach (FileInfo fi in SqliteFileHandler.Path.EnumerateFiles())
                {
                    if (fi.Name.StartsWith("model_")
                        && !fi.Name.StartsWith(Version))
                    {
                        RemoveReadOnlyFlag(fi.FullName);
                        fi.Delete();
                    }
                }
            }
            catch { }
        }

        void LoadFile()
        {
            string gzFile = Path.Combine(SqliteFileHandler.Path.FullName, Version + ".gz");
            using (FileStream fs = new FileStream(gzFile, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                _data = new byte[fs.Length];
                reader.Read(_data, 0, (int)fs.Length);
            }
        }

        /// <summary>
        /// 构造表结构信息
        /// </summary>
        /// <param name="p_schema"></param>
        /// <param name="p_tbls"></param>
        /// <param name="p_cols"></param>
        /// <param name="p_dbInfo"></param>
        static void LoadSchema(IReadOnlyDictionary<string, TableSchema> p_schema, List<OmTable> p_tbls, List<OmColumn> p_cols, DbAccessInfo p_dbInfo)
        {
            OmColumn col;
            foreach (var item in p_schema.Values)
            {
                var tbl = new OmTable();
                tbl.ID = p_tbls.Count + 1;
                tbl.Name = item.Name;
                tbl.Type = (int)p_dbInfo.DbType;
                tbl.DbKey = p_dbInfo.Name;

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

        /// <summary>
        /// 移除文件的只读属性
        /// </summary>
        /// <param name="p_filePath"></param>
        internal static void RemoveReadOnlyFlag(string p_filePath)
        {
            FileAttributes fileAttributes = File.GetAttributes(p_filePath);
            if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                fileAttributes &= ~FileAttributes.ReadOnly;
                File.SetAttributes(p_filePath, fileAttributes);
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
