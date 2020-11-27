#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// SqliteConnection功能扩展类
    /// </summary>
    public class SqliteConnectionEx : SqliteConnection
    {
        const string _errQuery = "查询语句不能为空";
        const string _lastRowid = "select last_insert_rowid()";
        static ConcurrentDictionary<string, TableMapping> _mappings = null;

        #region 构造方法
        public SqliteConnectionEx()
        {
        }

        public SqliteConnectionEx(string p_connectStr)
            : base(p_connectStr)
        {
        }
        #endregion

        #region 插入
        /// <summary>
        /// 向数据库中插入某对象的数据。
        /// </summary>
        /// <param name="p_obj">数据对象</param>
        /// <param name="p_autoUpdate">bool 类型，为true时，产生的sql语句带 or replace </param>
        /// <returns></returns>
        public int Insert(object p_obj, bool p_autoUpdate)
        {
            if (p_obj == null)
                return 0;

            var map = GetMapping(p_obj.GetType());
            var cols = p_autoUpdate ? map.Columns : map.InsertColumns;
            int cnt = 0;

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = map.GetInsertSql(p_autoUpdate);
                foreach (var col in cols)
                {
                    cmd.Parameters.Add(col.Name, ToDbType(col.ColumnType)).Value = col.GetValue(p_obj);
                }
                cnt = cmd.ExecuteNonQuery();

                // 自增列
                if (map.HasAutoIncPK)
                {
                    cmd.CommandText = _lastRowid;
                    cmd.Parameters.Clear();
                    long id = (long)cmd.ExecuteScalar();
                    map.SetAutoIncPK(p_obj, id);
                }
            }
            return cnt;
        }

        /// <summary>
        /// 事务内插入多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_list"></param>
        /// <param name="p_autoUpdate"></param>
        /// <returns></returns>
        public int BatchInsert<T>(IEnumerable<T> p_list, bool p_autoUpdate)
        {
            if (p_list == null || p_list.Count() == 0)
                return 0;

            var map = GetMapping(typeof(T));
            var cols = p_autoUpdate ? map.Columns : map.InsertColumns;
            int cnt = 0;

            RunInTransaction(() =>
            {
                using (var cmd = CreateCmd())
                {
                    cmd.CommandText = map.GetInsertSql(p_autoUpdate);
                    foreach (var col in cols)
                    {
                        cmd.Parameters.Add(col.Name, ToDbType(col.ColumnType));
                    }

                    SqliteCommand idCmd = null;
                    foreach (var r in p_list)
                    {
                        if (r == null)
                            continue;

                        foreach (var col in cols)
                        {
                            cmd.Parameters[col.Name].Value = col.GetValue(r);
                        }
                        cnt += cmd.ExecuteNonQuery();

                        // 自增列
                        if (map.HasAutoIncPK)
                        {
                            // 不能使用cmd！
                            if (idCmd == null)
                                idCmd = new SqliteCommand(_lastRowid, this, Transaction);
                            long id = (long)idCmd.ExecuteScalar();
                            map.SetAutoIncPK(r, id);
                        }
                    }

                    if (idCmd != null)
                        idCmd.Dispose();
                }
            });
            return cnt;
        }

        /// <summary>
        /// 插入或自动更新一条记录
        /// </summary>
        /// <param name="p_row">数据</param>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        internal int InsertRow(Row p_row, string p_tblName)
        {
            if (p_row == null || string.IsNullOrEmpty(p_tblName))
                throw new Exception("待保存的数据和表名不可为空！");

            int cnt = 0;
            TableMapping map = GetTblMapping(p_tblName);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = map.GetInsertSql(true);
                foreach (var col in map.Columns)
                {
                    cmd.Parameters.Add(col.Name, ToDbType(col.ColumnType)).Value = GetColVal(p_row, col);
                }
                cnt = cmd.ExecuteNonQuery();

                // 自增列
                if (p_row.IsAdded && map.HasAutoIncPK)
                {
                    cmd.CommandText = _lastRowid;
                    cmd.Parameters.Clear();
                    long id = (long)cmd.ExecuteScalar();
                    p_row[map.PK.Name] = id;
                }
            }
            return cnt;
        }

        /// <summary>
        /// 事务内插入或更新Table
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        internal int InsertTable(Table p_tbl, string p_tblName)
        {
            if (p_tbl == null || string.IsNullOrEmpty(p_tblName))
                throw new Exception("待保存的数据和表名不可为空！");

            int cnt = 0;
            TableMapping map = GetTblMapping(p_tblName);

            RunInTransaction(() =>
            {
                using (var cmd = CreateCmd())
                {
                    cmd.CommandText = map.GetInsertSql(true);
                    foreach (var col in map.Columns)
                    {
                        cmd.Parameters.Add(col.Name, ToDbType(col.ColumnType));
                    }

                    SqliteCommand idCmd = null;
                    foreach (var r in p_tbl)
                    {
                        foreach (var col in map.Columns)
                        {
                            cmd.Parameters[col.Name].Value = GetColVal(r, col);
                        }
                        cnt += cmd.ExecuteNonQuery();

                        // 自增列
                        if (r.IsAdded && map.HasAutoIncPK)
                        {
                            if (idCmd == null)
                                idCmd = new SqliteCommand(_lastRowid, this, Transaction);
                            long id = (long)idCmd.ExecuteScalar();
                            r[map.PK.Name] = id;
                        }
                    }

                    if (idCmd != null)
                        idCmd.Dispose();
                }
            });
            return cnt;
        }
        #endregion

        #region 查询
        /// <summary>
        /// SQL查询，返回对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>对象列表</returns>
        public List<T> Query<T>(string p_sql, Dict p_params = null) where T : class
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.ExecuteQuery<T>();
            }
        }

        /// <summary>
        /// SQL查询，返回Table
        /// </summary>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>Table</returns>
        public Table Query(string p_sql, Dict p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.ExecuteQuery();
            }
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string p_sql, Dict p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.ExecuteScalar<T>();
            }
        }

        /// <summary>
        /// 查询，返回可枚举对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public IEnumerable<T> DeferredQuery<T>(string p_sql, Dict p_params = null) where T : class
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.DeferredQuery<T>();
            }
        }

        /// <summary>
        /// SQL查询，只返回第一行
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public Row GetRow(string p_sql, Dict p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.GetFirstRow();
            }
        }

        /// <summary>
        /// 获取库中的所有表名
        /// </summary>
        /// <returns></returns>
        internal Table QueryTblsName()
        {
            using (var cmd = CreateCmd())
            {
                cmd.CommandText = "select name from sqlite_master where type='table' order by name";
                return cmd.ExecuteQuery();
            }
        }

        /// <summary>
        /// 在事务中执行过程
        /// </summary>
        /// <param name="p_action"></param>
        public void RunInTransaction(Action p_action)
        {
            using (var trans = BeginTransaction())
            {
                try
                {
                    p_action();
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
        }

        /// <summary>
        /// 执行查询操作，返回影响的数据行数
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public int Execute(string p_sql, Dict p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region 初始化状态库
        /// <summary>
        /// 初始化库表结构
        /// </summary>
        internal void InitStateDb()
        {
            var tps = AtSys.Stub.StateTbls;
            if (tps == null || tps.Count == 0)
                return;

            foreach (var item in tps)
            {
                CreateStateTable(item.Value);
            }
        }

        /// <summary>
        /// 创建状态库表
        /// </summary>
        /// <param name="p_type"></param>
        void CreateStateTable(Type p_type)
        {
            TableMapping map = GetMapping(p_type);
            List<ColumnInfo> cols = GetTableInfo(p_type.Name);

            if (cols.Count > 0)
            {
                // 若表存在，判断是否需要增加列
                var toBeAdded = new List<TableMapping.Column>();
                foreach (var p in map.Columns)
                {
                    var found = false;
                    foreach (var c in cols)
                    {
                        found = (string.Compare(p.Name, c.Name, StringComparison.OrdinalIgnoreCase) == 0);
                        if (found)
                            break;
                    }
                    if (!found)
                    {
                        toBeAdded.Add(p);
                    }
                }

                foreach (var p in toBeAdded)
                {
                    var addCol = "alter table \"" + map.TableName + "\" add column " + TableMapping.SqlDecl(p);
                    Execute(addCol);
                }
            }
            else
            {
                // 创建表
                Execute(map.GetCreateSql());
            }

            // 索引
            var indexes = new Dictionary<string, IndexInfo>();
            foreach (var c in map.Columns)
            {
                foreach (var i in c.Indices)
                {
                    var iname = i.Name ?? map.TableName + "_" + c.Name;
                    IndexInfo iinfo;
                    if (!indexes.TryGetValue(iname, out iinfo))
                    {
                        iinfo = new IndexInfo
                        {
                            IndexName = iname,
                            TableName = map.TableName,
                            Unique = i.Unique,
                            Columns = new List<IndexedColumn>()
                        };
                        indexes.Add(iname, iinfo);
                    }

                    if (i.Unique != iinfo.Unique)
                        throw new Exception("All the columns in an index must have the same value for their Unique property");

                    iinfo.Columns.Add(new IndexedColumn
                    {
                        Order = i.Order,
                        ColumnName = c.Name
                    });
                }
            }
        }

        /// <summary>
        /// 根据表名获得表信息（列信息集合）
        /// </summary>
        /// <param name="p_tableName"></param>
        /// <returns></returns>
        List<ColumnInfo> GetTableInfo(string p_tableName)
        {
            var query = "pragma table_info(\"" + p_tableName + "\")";
            return Query<ColumnInfo>(query);
        }

        public struct IndexInfo
        {
            public List<IndexedColumn> Columns;
            public string IndexName;
            public string TableName;
            public bool Unique;
        }

        public struct IndexedColumn
        {
            public string ColumnName;
            public int Order;
        }

        public class ColumnInfo
        {
            public ColumnInfo()
            {
            }

            [Column("name")]
            public string Name { get; set; }
            public int notnull { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 根据表名获取TableMapping
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        internal TableMapping GetTblMapping(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                throw new Exception("获取本地库映射未提供表名！");

            // 表名->类型->获取映射
            Type tp;
            var tps = AtSys.Stub.StateTbls;
            if (tps != null && tps.TryGetValue(p_tblName.ToLower(), out tp))
                return GetMapping(tp);
            throw new Exception(string.Format("本地库无【{0}】！", p_tblName));
        }

        /// <summary>
        /// 根据类型获取TableMapping
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        TableMapping GetMapping(Type type)
        {
            if (_mappings == null)
                _mappings = new ConcurrentDictionary<string, TableMapping>();
            return _mappings.GetOrAdd(type.FullName, (name) => new TableMapping(type));
        }

        /// <summary>
        /// 获取列值
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        object GetColVal(Row p_row, TableMapping.Column p_col)
        {
            object val = DBNull.Value;
            if (!p_row.Contains(p_col.Name))
                return val;

            Cell cell = p_row.Cells[p_col.Name];
            if (cell.Val == null)
                return val;

            if (cell.Type == p_col.ColumnType)
            {
                val = cell.Val;
            }
            else if (p_col.ColumnType == typeof(bool))
            {
                // bool特殊处理1
                string b = cell.Val.ToString().ToLower();
                val = (b == "1" || b == "true");
            }
            else
            {
                // 类型不同的将值转型
                try
                {
                    val = Convert.ChangeType(cell.Val, p_col.ColumnType);
                }
                catch { }
            }
            return val;
        }

        SqliteCommandEx CreateCmd()
        {
            return new SqliteCommandEx { Connection = this, Transaction = Transaction };
        }

        /// <summary>
        /// 重新包装sqlitecommand的参数
        /// </summary>
        /// <param name="p_collection"></param>
        /// <param name="p_param"></param>
        void WrapParams(SqliteParameterCollection p_collection, Dict p_param)
        {
            if (p_param != null && p_param.Count > 0)
            {
                foreach (var item in p_param)
                {
                    p_collection.AddWithValue(item.Key, item.Value);
                }
            }
        }

        static readonly Dictionary<Type, SqliteType> _sqliteTypeMapping =
            new Dictionary<Type, SqliteType>()
            {
                {typeof(bool), SqliteType.Integer},
                {typeof(byte),SqliteType.Integer},
                {typeof(byte[]), SqliteType.Blob},
                {typeof(char),SqliteType.Integer},
                {typeof(DateTime), SqliteType.Text},
                {typeof(DateTimeOffset), SqliteType.Text},
                {typeof(DBNull), SqliteType.Text},
                {typeof(decimal),SqliteType.Text},
                {typeof(double), SqliteType.Real},
                {typeof(float), SqliteType.Real},
                {typeof(Guid), SqliteType.Blob},
                {typeof(int), SqliteType.Integer},
                {typeof(long), SqliteType.Integer},
                {typeof(sbyte),SqliteType.Integer},
                {typeof(short), SqliteType.Integer},
                {typeof(string), SqliteType.Text},
                {typeof(TimeSpan), SqliteType.Text},
                {typeof(uint), SqliteType.Integer},
                {typeof(ulong), SqliteType.Integer},
                {typeof(ushort), SqliteType.Integer},
            };

        static SqliteType ToDbType(Type p_type)
        {
            if (_sqliteTypeMapping.TryGetValue(p_type, out var sqliteType))
                return sqliteType;
            if (p_type.IsEnum)
                return SqliteType.Integer;
            throw new InvalidOperationException("UnknownDataType " + p_type.FullName);
        }
        #endregion
    }
}