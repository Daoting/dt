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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// SqliteConnection功能扩展类
    /// </summary>
    public class SqliteConnectionEx : SqliteConnection
    {
        #region 成员变量
        const string _errQuery = "查询语句不能为空";
        const string _lastRowid = "select last_insert_rowid()";
        static readonly ConcurrentDictionary<string, TableMapping> _mappings = new ConcurrentDictionary<string, TableMapping>();
        #endregion

        #region 构造方法
        public SqliteConnectionEx()
        {
        }

        public SqliteConnectionEx(string p_connectStr)
            : base(p_connectStr)
        {
        }
        #endregion

        #region 查询
        /// <summary>
        /// SQL查询，返回Table
        /// </summary>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>Table</returns>
        public Table Query(string p_sql, object p_params = null)
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
        /// SQL查询，返回对象列表
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="p_sql">sql语句</param>
        /// <param name="p_params">参数值列表</param>
        /// <returns>对象列表</returns>
        public Table<TEntity> Query<TEntity>(string p_sql, object p_params = null)
            where TEntity : Entity
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.ExecuteQuery<TEntity>();
            }
        }

        /// <summary>
        /// 查询，返回可枚举对象列表
        /// </summary>
        /// <typeparam name="TRow"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public IEnumerable<TRow> ForEach<TRow>(string p_sql, object p_params = null)
            where TRow : Row
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.ForEach<TRow>();
            }
        }

        /// <summary>
        /// 查询单个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public T GetScalar<T>(string p_sql, object p_params = null)
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
        /// 返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public List<T> GetFirstCol<T>(string p_sql, object p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.GetFirstCol<T>();
            }
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一列枚举，高性能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public IEnumerable<T> EachFirstCol<T>(string p_sql, object p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.EachFirstCol<T>();
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
                cmd.CommandText = "select name from sqlite_master where type='table' and name<>'sqlite_sequence' order by name";
                return cmd.ExecuteQuery();
            }
        }
        #endregion

        #region 执行
        /// <summary>
        /// 执行查询操作，返回影响的数据行数
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public int Execute(string p_sql, object p_params = null)
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

        /// <summary>
        /// 批量执行
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_list"></param>
        /// <returns></returns>
        public int BatchExecute(string p_sql, List<Dict> p_list)
        {
            if (p_list == null || p_list.Count == 0)
                throw new Exception(_errQuery);

            int cnt = 0;
            RunInTransaction(() =>
            {
                using (var cmd = CreateCmd())
                {
                    cmd.CommandText = p_sql;
                    foreach (var item in p_list[0])
                    {
                        cmd.Parameters.Add(item.Key, item.Value == null ? SqliteType.Text : ToDbType(item.Value.GetType()));
                    }

                    foreach (var dt in p_list)
                    {
                        foreach (var item in dt)
                        {
                            cmd.Parameters[item.Key].Value = item.Value;
                        }
                        cnt += cmd.ExecuteNonQuery();
                    }
                }
            });
            return cnt;
        }

        /// <summary>
        /// 在事务中执行过程
        /// </summary>
        /// <param name="p_action"></param>
        public void RunInTransaction(Action p_action)
        {
#if WASM
            // 5.0.3版本中 BeginTransaction 异常！
            p_action();
#else
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
#endif
        }
        #endregion

        #region 初始化状态库
        /// <summary>
        /// 初始化库表结构
        /// </summary>
        /// <param name="p_types">映射类型</param>
        internal void InitDb(IList<Type> p_types)
        {
            if (p_types == null || p_types.Count == 0)
                return;

            foreach (var item in p_types)
            {
                CreateTable(item);
            }
        }

        /// <summary>
        /// 根据类型创建表结构
        /// </summary>
        /// <param name="p_type"></param>
        void CreateTable(Type p_type)
        {
            TableMapping map = GetMapping(p_type);

            // 查询所有列名，原来使用 pragma table_info(tblName)
            var cols = GetFirstCol<string>($"select name from pragma_table_info('{p_type.Name}')");
            if (cols.Count > 0)
            {
                // 若表存在，判断是否需要增加列
                var toBeAdded = new List<TableMapping.Column>();
                foreach (var p in map.Columns)
                {
                    var found = false;
                    foreach (var col in cols)
                    {
                        found = (string.Compare(p.Name, col, StringComparison.OrdinalIgnoreCase) == 0);
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
        #endregion

        #region 内部方法
        /// <summary>
        /// 根据类型获取TableMapping
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static TableMapping GetMapping(Type type)
        {
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
        void WrapParams(SqliteParameterCollection p_collection, object p_param)
        {
            if (p_param is Dict dt)
            {
                if (dt.Count > 0)
                {
                    foreach (var item in dt)
                    {
                        p_collection.AddWithValue(item.Key, item.Value);
                    }
                }
            }
            else if (p_param != null)
            {
                // 匿名对象无法在GetProperty时指定BindingFlags！
                foreach (var prop in p_param.GetType().GetProperties())
                {
                    p_collection.AddWithValue(prop.Name, prop.GetValue(p_param));
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