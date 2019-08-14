#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Cm.Sqlite
{
    public class SqliteConnectionEx : SqliteConnection
    {
        static ConcurrentDictionary<string, TableMapping> _mappings = null;

        public SqliteConnectionEx()
        {
        }

        public SqliteConnectionEx(string p_connectStr)
            : base(p_connectStr)
        {
        }

        /// <summary>
        /// 根据类型创建表
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public int CreateTable(Type p_type)
        {
            TableMapping map = GetMapping(p_type);
            string sql = map.GetCreateSql();
            int count = 0;
            using (SqliteCommand cmd = CreateCommand())
            {
                // 创建表
                cmd.CommandText = sql;
                count = cmd.ExecuteNonQuery();

                // 整理索引项
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

                // 创建索引
                foreach (var indexName in indexes.Keys)
                {
                    var index = indexes[indexName];
                    var columns = string.Join("\",\"", index.Columns.OrderBy(i => i.Order).Select(i => i.ColumnName).ToArray());
                    sql = $"create {(index.Unique ? "unique" : "")} index if not exists \"{indexName}\" on \"{index.TableName}\"(\"{columns}\")";
                    cmd.CommandText = sql;
                    count += cmd.ExecuteNonQuery();
                }
            }
            return count;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="ty"></param>
        /// <param name="p_tbl"></param>
        public void BatchInsert(Type ty, Table p_tbl)
        {
            var map = GetMapping(ty);
            var cols = map.InsertColumns;

            using (SqliteCommand cmd = CreateCommand())
            {
                cmd.CommandText = map.GetInsertSql();
                foreach (var col in cols)
                {
                    cmd.Parameters.Add(col.Name, ToDbType(col.ColumnType));
                }
                foreach (var row in p_tbl)
                {
                    foreach (var col in cols)
                    {
                        cmd.Parameters[col.Name].Value = row[col.Name];
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_list"></param>
        public void BatchInsert<T>(IEnumerable<T> p_list)
        {
            var map = GetMapping(typeof(T));
            var cols = map.InsertColumns;

            using (SqliteCommand cmd = CreateCommand())
            {
                cmd.CommandText = map.GetInsertSql();
                foreach (var col in cols)
                {
                    cmd.Parameters.Add(col.Name, ToDbType(col.ColumnType));
                }
                foreach (var item in p_list)
                {
                    foreach (var col in cols)
                    {
                        cmd.Parameters[col.Name].Value = col.GetValue(item);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        TableMapping GetMapping(Type type)
        {
            if (_mappings == null)
                _mappings = new ConcurrentDictionary<string, TableMapping>();
            return _mappings.GetOrAdd(type.FullName, (name) => new TableMapping(type));
        }

        struct IndexedColumn
        {
            public int Order;
            public string ColumnName;
        }

        struct IndexInfo
        {
            public string IndexName;
            public string TableName;
            public bool Unique;
            public List<IndexedColumn> Columns;
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

        public static SqliteType ToDbType(Type p_type)
        {
            if (_sqliteTypeMapping.TryGetValue(p_type, out var sqliteType))
                return sqliteType;
            throw new InvalidOperationException("UnknownDataType "+ p_type.FullName);
        }
    }
}
