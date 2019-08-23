#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using MySql.Data.MySqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Model
{
    /// <summary>
    /// Sqlite模型文件生成类
    /// </summary>
    public static class SqliteModel
    {
        static readonly Dictionary<Type, string> _cacheDict;
        static string _path;
        static bool _refreshing;

        static SqliteModel()
        {
            _cacheDict = new Dictionary<Type, string>();
            _cacheDict[typeof(OmMenu)] = @"SELECT
	                                            id,
	                                            parentid,
	                                            name,
	                                            IF (isgroup='1',1,0) AS isgroup,
	                                            viewname,
	                                            params,
	                                            icon,
	                                            srvname,
	                                            note,
	                                            dispidx 
                                            FROM
	                                            dt_menu 
                                            WHERE
	                                            islocked = '0' 
                                            ORDER BY
	                                            dispidx";
            _cacheDict[typeof(OmBaseCode)] = "select id,grp from dt_res order by dispidx";
            _cacheDict[typeof(OmReport)] = "select id,name,define from dt_rpt";
            _cacheDict[typeof(RoleMenu)] = "select a.* from dt_rolemenu a,dt_menu b where a.menuid=b.id and b.islocked='0'";
            _cacheDict[typeof(RolePrv)] = "select * from dt_roleprv";
        }

        /// <summary>
        /// 获取模型文件版本号
        /// </summary>
        public static string Version { get; private set; }

        /// <summary>
        /// 获取模型文件缓存
        /// </summary>
        public static byte[] ModelData { get; private set; }

        /// <summary>
        /// 初始化SQLite模型文件
        /// </summary>
        internal static async Task Init()
        {
            _path = Path.Combine(Directory.GetCurrentDirectory(), "etc/model");
            DirectoryInfo dir = new DirectoryInfo(_path);
            if (!dir.Exists)
                dir.Create();

            FileInfo fi = dir.EnumerateFiles("*.gz", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (fi == null)
            {
                bool trace = Db.TraceSql;
                Db.TraceSql = false;
                Dict dt = await Refresh();
                Db.TraceSql = trace;
                if ((bool)dt["suc"])
                    Log.Information("创建模型文件成功");
                else
                    Log.Error($"创建模型文件失败\r\n【{dt["msg"]}】");
            }
            else
            {
                Version = fi.Name.Substring(0, fi.Name.Length - 3);
                LoadModelFile();
                Log.Information("缓存模型文件成功");
            }
        }

        /// <summary>
        /// 刷新模型版本
        /// </summary>
        /// <returns>刷新结果</returns>
        internal static async Task<Dict> Refresh()
        {
            Dict dt;
            if (_refreshing)
            {
                dt = new Dict();
                dt["suc"] = false;
                dt["msg"] = "模型刷新进行中...";
                return dt;
            }

            // 刷新标志，避免重复刷新
            _refreshing = true;
            string id = Guid.NewGuid().ToString().Substring(0, 8);
            StringBuilder sb = new StringBuilder();
            sb.Append("※ ");
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.AppendLine(" (刷新模型)");
            sb.AppendFormat("模型版本 {0}\r\n", id);

            // 刷新表结构缓存
            Stopwatch watch = new Stopwatch();
            watch.Start();
            sb.AppendLine(DbSchema.LoadSchema());

            // 创建Data目录
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
            string dbFile = Path.Combine(_path, id + ".db");
            try
            {
                Db db = new Db();
                using (SqliteConnectionEx conn = new SqliteConnectionEx($"Data Source={dbFile}"))
                {
                    conn.Open();
                    sb.AppendLine("开始导出模型及数据");
                    using (var tran = conn.BeginTransaction())
                    {
                        foreach (var item in _cacheDict)
                        {
                            conn.CreateTable(item.Key);
                            sb.AppendFormat("创建表{0}成功，", item.Key.Name);
                            Table tbl = await db.Table(item.Value);
                            if (tbl.Count > 0)
                            {
                                conn.BatchInsert(item.Key, tbl);
                                sb.AppendFormat("导出{0}行\r\n", tbl.Count);
                            }
                            else
                            {
                                sb.AppendLine("无数据");
                            }
                        }

                        conn.CreateTable(typeof(OmColumn));
                        List<OmColumn> cols = new List<OmColumn>();
                        LoadSchema(cols);
                        conn.BatchInsert(cols);
                        sb.AppendFormat("创建表OmColumn成功，导出{0}行\r\n", cols.Count);

                        tran.Commit();
                    }
                }

                // 将结果模型文件内容压缩
                sb.Append("生成压缩文件...");
                using (FileStream inFile = File.OpenRead(dbFile))
                {
                    using (FileStream outFile = File.Create(Path.Combine(_path, id + ".gz")))
                    {
                        using (GZipStream gzStream = new GZipStream(outFile, CompressionMode.Compress))
                        {
                            inFile.CopyTo(gzStream);
                        }
                    }
                }
                sb.AppendLine("成功！");

                // 更新缓存、通知版本变化
                Version = id;
                LoadModelFile();
                sb.AppendLine("缓存模型文件成功！");

                watch.Stop();
                sb.AppendLine($"创建结束！用时{watch.ElapsedMilliseconds}毫秒");

                dt = new Dict();
                dt["suc"] = true;
                dt["msg"] = sb.ToString();
            }
            catch (Exception ex)
            {
                watch.Stop();
                throw ex;
            }
            finally
            {
                // 移除标志
                _refreshing = false;
            }

            // 删除历史文件
            try
            {
                DirectoryInfo dir = new DirectoryInfo(_path);
                foreach (FileInfo fi in dir.EnumerateFiles())
                {
                    if (!fi.Name.StartsWith(id))
                    {
                        RemoveReadOnlyFlag(fi.FullName);
                        fi.Delete();
                    }
                }
            }
            catch { }

            return dt;
        }

        /// <summary>
        /// 读取模型文件到缓存
        /// </summary>
        static void LoadModelFile()
        {
            string gzFile = Path.Combine(_path, Version + ".gz");
            using (FileStream fs = new FileStream(gzFile, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    ModelData = new byte[fs.Length];
                    reader.Read(ModelData, 0, (int)fs.Length);
                }
            }
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
        /// <param name="p_cols"></param>
        static void LoadSchema(List<OmColumn> p_cols)
        {
            foreach (var item in DbSchema.Schema)
            {
                foreach (var row in item.Value.PrimaryKey)
                {
                    OmColumn col = CreateCol(row, true);
                    col.TabName = item.Key;
                    p_cols.Add(col);
                }

                foreach (var row in item.Value.Columns)
                {
                    OmColumn col = CreateCol(row, false);
                    col.TabName = item.Key;
                    p_cols.Add(col);
                }
            }

            // 加载DbList中库的表结构
            foreach (var item in Glb.Config.GetSection("DbList").GetChildren())
            {
                LoadItemSchema(item.Value, p_cols);
            }
        }

        static void LoadItemSchema(string p_connStr, List<OmColumn> p_cols)
        {
            MySqlDataReader reader = null;
            using (MySqlConnection conn = new MySqlConnection(p_connStr))
            {
                conn.Open();
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
                                OmColumn col = new OmColumn();
                                col.TabName = reader.GetString(0).ToLower();
                                col.ColName = reader.GetString(1).ToLower();
                                col.DbType = DbTypeConverter.GetTypeNameByDbTypeName(reader.GetString(2));
                                
                                // 是否为主键
                                if (!reader.IsDBNull(3) && reader.GetString(3) == "PRI")
                                    col.IsPrimary = true;
                                else
                                    col.IsPrimary = false;

                                // character_maximum_length
                                if (!reader.IsDBNull(4))
                                    col.Length = (int)reader.GetInt64(4);

                                // is_nullable
                                if (!reader.IsDBNull(5))
                                    col.Nullable = reader.GetString(5).ToLower() == "yes";

                                // column_comment
                                if (!reader.IsDBNull(6))
                                    col.Comments = reader.GetString(6);
                                p_cols.Add(col);
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
                }
            }
        }

        static OmColumn CreateCol(TableCol row, bool p_isPrimaryKey)
        {
            OmColumn col = new OmColumn();
            col.ColName = row.Name;
            col.DbType = row.TypeName;
            col.IsPrimary = p_isPrimaryKey;
            col.Length = (int)row.Length;
            col.Nullable = row.Nullable;
            col.Comments = row.Comments;
            return col;
        }
    }
}
