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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// SqliteConnection������չ��
    /// </summary>
    public class SqliteConnectionEx : SqliteConnection
    {
        #region ��Ա����
        const string _errQuery = "��ѯ��䲻��Ϊ��";
        const string _lastRowid = "select last_insert_rowid()";
        static readonly ConcurrentDictionary<string, TableMapping> _mappings = new ConcurrentDictionary<string, TableMapping>();
        #endregion

        #region ���췽��
        public SqliteConnectionEx()
        {
        }

        public SqliteConnectionEx(string p_connectStr)
            : base(p_connectStr)
        {
        }
        #endregion

        #region ��ѯ
        /// <summary>
        /// SQL��ѯ������Table
        /// </summary>
        /// <param name="p_sql">sql���</param>
        /// <param name="p_params">����ֵ�б�</param>
        /// <returns>Table</returns>
        public Task<Table> Query(string p_sql, object p_params = null)
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
        /// SQL��ѯ�����ض����б�
        /// </summary>
        /// <typeparam name="TEntity">��������</typeparam>
        /// <param name="p_sql">sql���</param>
        /// <param name="p_params">����ֵ�б�</param>
        /// <returns>�����б�</returns>
        public Task<Table<TEntity>> Query<TEntity>(string p_sql, object p_params = null)
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
        /// ��ѯ�����ؿ�ö�ٶ����б�
        /// </summary>
        /// <typeparam name="TRow"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public Task<IEnumerable<TRow>> ForEach<TRow>(string p_sql, object p_params = null)
            where TRow : Row
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            // ���õ�yield���أ���SqliteCommandEx�ڲ��ͷţ�
            var cmd = CreateCmd();
            cmd.CommandText = p_sql;
            WrapParams(cmd.Parameters, p_params);
            return cmd.ForEach<TRow>();
        }

        /// <summary>
        /// ��ѯ����ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public Task<T> GetScalar<T>(string p_sql, object p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                cmd.ExecuteScalar();
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
        public Task<List<T>> GetFirstCol<T>(string p_sql, object p_params = null)
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
        public Task<IEnumerable<T>> EachFirstCol<T>(string p_sql, object p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            // ���õ�yield���أ���SqliteCommandEx�ڲ��ͷţ�
            var cmd = CreateCmd();
            cmd.CommandText = p_sql;
            WrapParams(cmd.Parameters, p_params);
            return cmd.EachFirstCol<T>();
        }
        #endregion

        #region ִ��
        /// <summary>
        /// ִ�в�ѯ����������Ӱ�����������
        /// </summary>
        /// <param name="p_sql"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public Task<int> Execute(string p_sql, object p_params = null)
        {
            if (string.IsNullOrEmpty(p_sql))
                throw new Exception(_errQuery);

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = p_sql;
                WrapParams(cmd.Parameters, p_params);
                return cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// һ��������ִ�ж��Sql
        /// </summary>
        /// <param name="p_dts">�����б�ÿ��Dict�а�����������text,params��textΪsql���params����ΪDict��List{Dict}</param>
        /// <returns>����ִ�к�Ӱ�������</returns>
        public async Task<int> BatchExec(List<Dict> p_dts)
        {
            if (p_dts == null || p_dts.Count == 0)
                return 0;

            int cnt = 0;
            Func<Task> exec = async () =>
            {
                using (var cmd = CreateCmd())
                {
                    foreach (Dict dt in p_dts)
                    {
                        cmd.CommandText = (string)dt["text"];
                        if (dt["params"] is List<Dict> ls)
                        {
                            foreach (var par in ls)
                            {
                                cmd.Parameters.Clear();
                                WrapParams(cmd.Parameters, par);
                                cnt += await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        else if (dt["params"] is Dict par)
                        {
                            cmd.Parameters.Clear();
                            WrapParams(cmd.Parameters, par);
                            cnt += await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            };


#if WASM
            // wasm�� BeginTransaction �쳣��
            await exec();
#else
            using (var trans = BeginTransaction())
            {
                try
                {
                    await exec();
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    // �ⲿ֪��ʧ��
                    cnt = 0;
                }
            }
#endif
            return cnt;
        }
        #endregion

        #region ��ʼ��״̬��
        /// <summary>
        /// ��ʼ�����ṹ
        /// </summary>
        /// <param name="p_types">ӳ������</param>
        internal async Task InitDb(IList<Type> p_types)
        {
            if (p_types == null || p_types.Count == 0)
                return;

            foreach (var item in p_types)
            {
                await CreateTable(item);
            }
        }

        /// <summary>
        /// �������ʹ�����ṹ
        /// </summary>
        /// <param name="p_type"></param>
        async Task CreateTable(Type p_type)
        {
            TableMapping map = GetMapping(p_type);

            // ɾ����׺ X
            var tblName = p_type.Name.TrimEnd('X');
            bool exist = await GetScalar<int>($"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tblName}'") > 0;
            if (exist)
            {
                // ��ѯ����������ԭ��ʹ�� pragma table_info(tblName)
                var cols = await GetFirstCol<string>($"select name from pragma_table_info('{tblName}')");

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
                    var addCol = "alter table \"" + tblName + "\" add column " + TableMapping.SqlDecl(p);
                    await Execute(addCol);
                }
            }
            else
            {
                // ������
                await Execute(map.GetCreateSql());
            }

            // ����
            var indexes = new Dictionary<string, IndexInfo>();
            foreach (var c in map.Columns)
            {
                foreach (var i in c.Indices)
                {
                    var iname = i.Name ?? tblName + "_" + c.Name;
                    IndexInfo iinfo;
                    if (!indexes.TryGetValue(iname, out iinfo))
                    {
                        iinfo = new IndexInfo
                        {
                            IndexName = iname,
                            TableName = tblName,
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
        internal static TableMapping GetMapping(Type type)
        {
            // ɾ����׺ X
            var tblName = type.Name.TrimEnd('X');
            return _mappings.GetOrAdd(tblName, (name) => new TableMapping(type));
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
            if (p_param is Dict dt)
            {
                if (dt.Count > 0)
                {
                    foreach (var item in dt)
                    {
                        // nullʱ����ֵ����Ϊ DBNull.Value ��
                        var par = p_collection.AddWithValue(item.Key, item.Value == null ? DBNull.Value : item.Value);
                    }
                }
            }
            else if (p_param != null)
            {
                // ���������޷���GetPropertyʱָ��BindingFlags��
                foreach (var prop in p_param.GetType().GetProperties())
                {
                    var val = prop.GetValue(p_param);
                    p_collection.AddWithValue(prop.Name, val == null ? DBNull.Value : val);
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