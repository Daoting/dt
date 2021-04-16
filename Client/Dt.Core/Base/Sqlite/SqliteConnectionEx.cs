#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2017-12-06 ����
******************************************************************************/
#endregion

#region ��������
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// SqliteConnection������չ��
    /// </summary>
    public class SqliteConnectionEx : SqliteConnection
    {
        const string _errQuery = "��ѯ��䲻��Ϊ��";
        const string _lastRowid = "select last_insert_rowid()";
        static ConcurrentDictionary<string, TableMapping> _mappings = null;

        #region ���췽��
        public SqliteConnectionEx()
        {
        }

        public SqliteConnectionEx(string p_connectStr)
            : base(p_connectStr)
        {
        }
        #endregion

        #region ����
        /// <summary>
        /// �����ݿ��в���ĳ��������ݡ�
        /// </summary>
        /// <param name="p_obj">���ݶ���</param>
        /// <param name="p_autoUpdate">bool ���ͣ�Ϊtrueʱ��������sql���� or replace </param>
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

                // ������
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
        /// �����ڲ���������
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

                        // ������
                        if (map.HasAutoIncPK)
                        {
                            // ����ʹ��cmd��
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
        #endregion

        #region ��ѯ
        /// <summary>
        /// SQL��ѯ�����ض����б�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_sql">sql���</param>
        /// <param name="p_params">����ֵ�б�</param>
        /// <returns>�����б�</returns>
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
        /// SQL��ѯ������Table
        /// </summary>
        /// <param name="p_sql">sql���</param>
        /// <param name="p_params">����ֵ�б�</param>
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
        /// ��ѯ�����ؿ�ö�ٶ����б�
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
        /// ��ѯ����ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string p_sql, object p_params = null)
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
        /// ���ط��������ĵ�һ�����ݣ���ת��Ϊָ������
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
        /// �Բ���ֵ��ʽִ��Sql��䣬���ص�һ��ö�٣�������
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
        /// ��ȡ���е����б���
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
        /// ��������ִ�й���
        /// </summary>
        /// <param name="p_action"></param>
        public void RunInTransaction(Action p_action)
        {
#if WASM
            // 5.0.3�汾�� BeginTransaction �쳣��
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

        /// <summary>
        /// ִ�в�ѯ����������Ӱ�����������
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
        #endregion

        #region ��ʼ��״̬��
        /// <summary>
        /// ��ʼ�����ṹ
        /// </summary>
        /// <param name="p_types">ӳ������</param>
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
        /// �������ʹ�����ṹ
        /// </summary>
        /// <param name="p_type"></param>
        void CreateTable(Type p_type)
        {
            TableMapping map = GetMapping(p_type);

            // ��ѯ����������ԭ��ʹ�� pragma table_info(tblName)
            var cols = GetFirstCol<string>($"select name from pragma_table_info('{p_type.Name}')");
            if (cols.Count > 0)
            {
                // ������ڣ��ж��Ƿ���Ҫ������
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
                // ������
                Execute(map.GetCreateSql());
            }

            // ����
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

        #region �ڲ�����
        /// <summary>
        /// �������ͻ�ȡTableMapping
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
        /// ��ȡ��ֵ
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
                // bool���⴦��1
                string b = cell.Val.ToString().ToLower();
                val = (b == "1" || b == "true");
            }
            else
            {
                // ���Ͳ�ͬ�Ľ�ֵת��
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
        /// ���°�װsqlitecommand�Ĳ���
        /// </summary>
        /// <param name="p_collection"></param>
        /// <param name="p_param"></param>
        void WrapParams(SqliteParameterCollection p_collection, object p_param)
        {
            if (p_param is Dict dt && dt.Count > 0)
            {
                foreach (var item in dt)
                {
                    p_collection.AddWithValue(item.Key, item.Value);
                }
            }
            else if (p_param != null)
            {
                // ���������޷���GetPropertyʱָ��BindingFlags��
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