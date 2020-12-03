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

        /// <summary>
        /// ������Զ�����һ����¼
        /// </summary>
        /// <param name="p_row">����</param>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        internal int InsertRow(Row p_row, string p_tblName)
        {
            if (p_row == null || string.IsNullOrEmpty(p_tblName))
                throw new Exception("����������ݺͱ�������Ϊ�գ�");

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

                // ������
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
        /// �����ڲ�������Table
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <param name="p_tblName"></param>
        /// <returns></returns>
        internal int InsertTable(Table p_tbl, string p_tblName)
        {
            if (p_tbl == null || string.IsNullOrEmpty(p_tblName))
                throw new Exception("����������ݺͱ�������Ϊ�գ�");

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

                        // ������
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

        #region ��ѯ
        /// <summary>
        /// SQL��ѯ�����ض����б�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="p_sql">sql���</param>
        /// <param name="p_params">����ֵ�б�</param>
        /// <returns>�����б�</returns>
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
        /// SQL��ѯ������Table
        /// </summary>
        /// <param name="p_sql">sql���</param>
        /// <param name="p_params">����ֵ�б�</param>
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
        /// ��ѯ����ֵ
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
        /// ��ѯ�����ؿ�ö�ٶ����б�
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
        /// SQL��ѯ��ֻ���ص�һ��
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

        public List<T> GetFirstCol<T>(string p_sql, Dict p_params = null)
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
        /// ִ�в�ѯ����������Ӱ�����������
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
        /// ����״̬���
        /// </summary>
        /// <param name="p_type"></param>
        void CreateStateTable(Type p_type)
        {
            TableMapping map = GetMapping(p_type);
            List<ColumnInfo> cols = GetTableInfo(p_type.Name);

            if (cols.Count > 0)
            {
                // ������ڣ��ж��Ƿ���Ҫ������
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

        /// <summary>
        /// ���ݱ�����ñ���Ϣ������Ϣ���ϣ�
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

        #region �ڲ�����
        /// <summary>
        /// ���ݱ�����ȡTableMapping
        /// </summary>
        /// <param name="p_tblName">����</param>
        /// <returns></returns>
        internal TableMapping GetTblMapping(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                throw new Exception("��ȡ���ؿ�ӳ��δ�ṩ������");

            // ����->����->��ȡӳ��
            Type tp;
            var tps = AtSys.Stub.StateTbls;
            if (tps != null && tps.TryGetValue(p_tblName.ToLower(), out tp))
                return GetMapping(tp);
            throw new Exception(string.Format("���ؿ��ޡ�{0}����", p_tblName));
        }

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