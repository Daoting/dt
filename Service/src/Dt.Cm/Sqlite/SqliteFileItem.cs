#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
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
    /// Sqlite文件项
    /// </summary>
    class SqliteFileItem
    {
        IConfigurationRoot _root;
        IConfigurationSection _sect;
        byte[] _data;

        public SqliteFileItem(IConfigurationRoot p_cfg, IConfigurationSection p_item)
        {
            _root = p_cfg;
            _sect = p_item;

            var name = p_item.Key;
            FileInfo fi = SqliteFileHandler.Path.EnumerateFiles(name + "_*.gz", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (fi != null)
            {
                Version = fi.Name.Substring(0, fi.Name.Length - 3);
                Log.Information("sqlite文件 {0}", Version);
            }
            else
            {
                Log.Error("缺少sqlite文件 {0}，客户端连接前务必创建！！！", name + "_*");
            }
        }

        /// <summary>
        /// 是否正在刷新中
        /// </summary>
        public bool IsRefreshing { get; set; }

        /// <summary>
        /// sqlite文件名作为版本号
        /// </summary>
        public string Version { get; private set; }

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
                Log.Warning("更新sqlite文件 {0}_* 进行中，无法重复更新！", _sect.Key);
                return;
            }

            // 刷新标志，避免重复刷新
            IsRefreshing = true;
            var newVer = _sect.Key + "_" + Guid.NewGuid().ToString().Substring(0, 8);
            Log.Information("准备更新 {0}", newVer);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            string dbFile = System.IO.Path.Combine(SqliteFileHandler.Path.FullName, newVer + ".db");

            bool trace = Kit.TraceSql;
            Kit.TraceSql = false;
            int sum = 0;

            try
            {
                using (var conn = new SqliteConnection($"Data Source={dbFile}"))
                {
                    conn.Open();
                    using var tran = conn.BeginTransaction();

                    foreach (var item in _sect.GetChildren())
                    {
                        var arr = _root.GetSection($"{item.Path}:Create").Get<string[]>();
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

                        // 导入数据
                        int cnt = await ImportData(item, conn);
                        if (cnt == 0)
                            Log.Information("创建表 {0}，无数据", item.Key);
                        else
                            Log.Information("创建表 {0}，导出 {1} 行", item.Key, cnt);
                        sum++;
                    }

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
                Log.Information("生成 {0}，{1} 张表，{1} 毫秒", gzFile, sum, watch.ElapsedMilliseconds);
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
                    if (fi.Name.StartsWith(_sect.Key + "_")
                        && !fi.Name.StartsWith(Version))
                    {
                        ModelFileItem.RemoveReadOnlyFlag(fi.FullName);
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

        async Task<int> ImportData(IConfigurationSection p_item, SqliteConnection p_conn)
        {
            var dbConn = _root.GetValue<string>($"{p_item.Path}:DbKey");
            var select = _root.GetValue<string>($"{p_item.Path}:Data");
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
                string tblName = p_item.Key;

                // 查询表的所有列
                using (var cmdCol = p_conn.CreateCommand())
                {
                    cmdCol.CommandText = $"SELECT c.name, c.type FROM sqlite_master AS t, pragma_table_info(t.name) AS c WHERE t.type = 'table' and t.name='{tblName}'";
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

                cmd.CommandText = $"insert into {tblName} ({colNames}) values ({paraNames})";
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

    }
}
