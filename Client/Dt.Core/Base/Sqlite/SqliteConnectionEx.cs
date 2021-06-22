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
        /// SQL��ѯ�����ض����б�
        /// </summary>
        /// <typeparam name="TEntity">��������</typeparam>
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
        /// ��ѯ�����ؿ�ö�ٶ����б�
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
        /// ��ѯ����ֵ
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
                cmd.CommandText = "select name from sqlite_master where type='table' and name<>'sqlite_sequence' order by name";
                return cmd.ExecuteQuery();
            }
        }
        #endregion

        #region ����
        /// <summary>
        /// ����ʵ�����ݣ�����ʵ��״̬ȷ����������
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_entity">ʵ��</param>
        /// <returns></returns>
        public bool Save<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.IfNull(p_entity);

            bool suc = false;
            if (p_entity.IsAdded)
                suc = Insert(p_entity) > 0;
            else if (p_entity.IsChanged)
                suc = Update(p_entity) > 0;

            if (suc)
                p_entity.AcceptChanges();
            return suc;
        }

        /// <summary>
        /// һ����������������ʵ�����ݣ�����ʵ��״ִ̬�����ģ�Table&lt;Entity&gt;֧��ɾ�����б�����֧�֣�
        /// <para>Table&lt;Entity&gt;��������ɾ��</para>
        /// <para>List&lt;Entity&gt;����������</para>
        /// <para>IList�������ɾ�ģ���Ա��ΪEntity,List&lt;Entity&gt;,Table&lt;Entity&gt;�Ļ��</para>
        /// </summary>
        /// <param name="p_list">�������б�</param>
        /// <returns></returns>
        public bool BatchSave(IList p_list)
        {
            if (p_list == null || p_list.Count == 0)
                return false;

            bool suc = false;
            RunInTransaction(() =>
            {
                Type tp = p_list.GetType();
                if (tp.IsGenericType
                    && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                {
                    suc = BatchSaveSameType(p_list) > 0;
                    if (p_list is Table tbl)
                    {
                        tbl.AcceptChanges();
                    }
                    else
                    {
                        foreach (var row in p_list.OfType<Row>())
                        {
                            if (row.IsChanged || row.IsAdded)
                                row.AcceptChanges();
                        }
                    }
                }
                else
                {
                    suc = BatchSaveMultiTypes(p_list) > 0;
                    if (suc)
                    {
                        foreach (var item in p_list)
                        {
                            if (item is Entity entity)
                            {
                                entity.AcceptChanges();
                            }
                            else if (item is Table tbl)
                            {
                                tbl.AcceptChanges();
                                tbl.DeletedRows?.Clear();
                            }
                            else if (item is IList clist && clist.Count > 0)
                            {
                                foreach (var ci in clist)
                                {
                                    if (ci is Row row
                                        && (row.IsAdded || row.IsChanged))
                                    {
                                        row.AcceptChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            });
            return suc;
        }

        int Insert<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            var map = GetMapping(typeof(TEntity));
            int cnt = 0;

            using (var cmd = CreateCmd())
            {
                StringBuilder insertCol = new StringBuilder();
                StringBuilder insertVal = new StringBuilder();
                foreach (var col in map.InsertColumns)
                {
                    if (!p_entity.Contains(col.Name))
                        continue;

                    if (insertCol.Length > 0)
                        insertCol.Append(",");
                    insertCol.Append(col.Name);

                    if (insertVal.Length > 0)
                        insertVal.Append(",");
                    insertVal.Append("@");
                    insertVal.Append(col.Name);

                    cmd.Parameters.AddWithValue(col.Name, p_entity[col.Name]);
                }
                cmd.CommandText = $"insert into '{map.TableName}'({insertCol}) values ({insertVal})";
                cnt = cmd.ExecuteNonQuery();

                // ������
                if (map.HasAutoIncPK)
                {
                    cmd.CommandText = _lastRowid;
                    cmd.Parameters.Clear();
                    long id = (long)cmd.ExecuteScalar();
                    map.SetAutoIncPK(p_entity, id);
                }
            }
            return cnt;
        }

        int Update<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            var map = GetMapping(typeof(TEntity));
            int cnt = 0;

            using (var cmd = CreateCmd())
            {
                StringBuilder updateVal = new StringBuilder();
                int updateIndex = 1;
                foreach (var col in map.Columns)
                {
                    if (!p_entity.Contains(col.Name) || !p_entity.Cells[col.Name].IsChanged)
                        continue;

                    // ֻ���±仯����
                    if (updateVal.Length > 0)
                        updateVal.Append(", ");
                    updateVal.Append(col.Name);
                    updateVal.Append("=@");
                    updateVal.Append(updateIndex);

                    // ��������ʱ�����ظ�
                    cmd.Parameters.AddWithValue(updateIndex.ToString(), p_entity[col.Name]);
                    updateIndex++;
                }

                // �������ܱ����£���֧�ָ�������
                cmd.Parameters.AddWithValue(map.PK.Name, p_entity.Cells[map.PK.Name].OriginalVal);

                cmd.CommandText = $"update '{map.TableName}' set {updateVal} where {map.PK.Name}=@{map.PK.Name}";
                cnt = cmd.ExecuteNonQuery();
            }
            return cnt;
        }

        int BatchSaveSameType(IList p_list)
        {
            var map = GetMapping(p_list.GetType().GetGenericArguments()[0]);
            int cnt = 0;

            using (var cmd = CreateCmd())
            {
                // �ȴ������
                bool update = false;
                SqliteCommand idCmd = null;
                foreach (Entity r in p_list)
                {
                    if (!r.IsAdded)
                    {
                        if (r.IsChanged)
                            update = true;
                        continue;
                    }

                    if (string.IsNullOrEmpty(cmd.CommandText))
                        LoadInsertSql(r, map, cmd);

                    foreach (var col in map.InsertColumns)
                    {
                        if (r.Contains(col.Name))
                            cmd.Parameters[col.Name].Value = r[col.Name];
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

                // ����
                if (update)
                {
                    foreach (Entity r in p_list)
                    {
                        if (r.IsAdded || !r.IsChanged)
                            continue;

                        LoadUpdateSql(r, map, cmd);
                        cnt += cmd.ExecuteNonQuery();
                    }
                }

                // ɾ��
                if (p_list is Table tbl && tbl.ExistDeleted)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText = $"delete from '{map.TableName}' where {map.PK.Name}=@{map.PK.Name}";
                    cmd.Parameters.Add(map.PK.Name, ToDbType(map.PK.ColumnType));
                    foreach (var r in tbl.DeletedRows)
                    {
                        cmd.Parameters[map.PK.Name].Value = r[map.PK.Name];
                        cnt += cmd.ExecuteNonQuery();
                    }
                }
            }
            return cnt;
        }

        int BatchSaveMultiTypes(IList p_list)
        {
            int cnt = 0;
            foreach (var item in p_list)
            {
                if (item is Entity entity)
                {
                    if (entity.IsAdded)
                        cnt += Insert(entity);
                    else if (entity.IsChanged)
                        cnt += Update(entity);
                }
                else if (item is IList clist
                    && item.GetType().IsGenericType
                    && item.GetType().GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                {
                    cnt += BatchSaveSameType(clist);
                }
                else
                {
                    throw new Exception($"�������治֧��[{item.GetType().Name}]���ͣ�");
                }
            }
            return cnt;
        }

        void LoadInsertSql(Entity p_entity, TableMapping p_map, SqliteCommandEx p_cmd)
        {
            StringBuilder insertCol = new StringBuilder();
            StringBuilder insertVal = new StringBuilder();
            foreach (var col in p_map.InsertColumns)
            {
                if (!p_entity.Contains(col.Name))
                    continue;

                if (insertCol.Length > 0)
                    insertCol.Append(",");
                insertCol.Append(col.Name);

                if (insertVal.Length > 0)
                    insertVal.Append(",");
                insertVal.Append("@");
                insertVal.Append(col.Name);

                p_cmd.Parameters.Add(col.Name, ToDbType(col.ColumnType));
            }
            p_cmd.CommandText = $"insert into '{p_map.TableName}'({insertCol}) values ({insertVal})";
        }

        void LoadUpdateSql(Entity p_entity, TableMapping p_map, SqliteCommandEx p_cmd)
        {
            StringBuilder updateVal = new StringBuilder();
            int updateIndex = 1;
            p_cmd.Parameters.Clear();
            foreach (var col in p_map.Columns)
            {
                if (!p_entity.Contains(col.Name) || !p_entity.Cells[col.Name].IsChanged)
                    continue;

                // ֻ���±仯����
                if (updateVal.Length > 0)
                    updateVal.Append(", ");
                updateVal.Append(col.Name);
                updateVal.Append("=@");
                updateVal.Append(updateIndex);

                // ��������ʱ�����ظ�
                p_cmd.Parameters.AddWithValue(updateIndex.ToString(), p_entity[col.Name]);
                updateIndex++;
            }

            // �������ܱ����£���֧�ָ�������
            p_cmd.Parameters.AddWithValue(p_map.PK.Name, p_entity.Cells[p_map.PK.Name].OriginalVal);
            p_cmd.CommandText = $"update '{p_map.TableName}' set {updateVal} where {p_map.PK.Name}=@{p_map.PK.Name}";
        }
        #endregion

        #region ɾ��
        /// <summary>
        /// ɾ��ʵ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ������</typeparam>
        /// <param name="p_entity">��ɾ������</param>
        /// <returns>true ɾ���ɹ�</returns>
        public bool Delete<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.IfNull(p_entity);
            var map = GetMapping(typeof(TEntity));
            int cnt = 0;

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = $"delete from '{map.TableName}' where {map.PK.Name}=@{map.PK.Name}";
                cmd.Parameters.AddWithValue(map.PK.Name, p_entity[map.PK.Name]);
                cnt = cmd.ExecuteNonQuery();
            }
            return cnt > 0;
        }

        /// <summary>
        /// ��������ɾ��ʵ����󣬽�֧�ֵ����������������ڲ�ȷ��
        /// </summary>
        /// <typeparam name="TEntity">ʵ������</typeparam>
        /// <param name="p_id">����ֵ</param>
        /// <returns></returns>
        public bool DelByPK<TEntity>(object p_id)
            where TEntity : Entity
        {
            var map = GetMapping(typeof(TEntity));
            int cnt = 0;

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = $"delete from '{map.TableName}' where {map.PK.Name}=@{map.PK.Name}";
                cmd.Parameters.AddWithValue(map.PK.Name, p_id);
                cnt = cmd.ExecuteNonQuery();
            }
            return cnt > 0;
        }

        /// <summary>
        /// ����ɾ��ʵ�壬��������б�����֧�֣�
        /// <para>Table&lt;Entity&gt;������ɾ��</para>
        /// <para>List&lt;Entity&gt;������ɾ��</para>
        /// <para>IList�����ɾ������Ա��ΪEntity,List&lt;Entity&gt;,Table&lt;Entity&gt;�Ļ��</para>
        /// </summary>
        /// <param name="p_list">��ɾ��ʵ���б�</param>
        /// <returns>true ɾ���ɹ�</returns>
        public bool BatchDelete(IList p_list)
        {
            if (p_list == null || p_list.Count == 0)
                return false;

            bool suc = false;
            RunInTransaction(() =>
            {
                Type tp = p_list.GetType();
                if (tp.IsGenericType
                    && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                {
                    suc = BatchDeleteSameType(p_list) > 0;
                }
                else
                {
                    suc = BatchDeleteMultiTypes(p_list) > 0;
                }
            });
            return suc;
        }

        int BatchDeleteSameType(IList p_list)
        {
            var map = GetMapping(p_list.GetType().GetGenericArguments()[0]);
            int cnt = 0;

            using (var cmd = CreateCmd())
            {
                cmd.CommandText = $"delete from '{map.TableName}' where {map.PK.Name}=@{map.PK.Name}";
                cmd.Parameters.Add(map.PK.Name, ToDbType(map.PK.ColumnType));
                foreach (Row r in p_list)
                {
                    cmd.Parameters[map.PK.Name].Value = r[map.PK.Name];
                    cnt += cmd.ExecuteNonQuery();
                }
            }
            return cnt;
        }

        int BatchDeleteMultiTypes(IList p_list)
        {
            int cnt = 0;
            foreach (var item in p_list)
            {
                if (item is Entity entity)
                {
                    cnt += Delete(entity) ? 1 : 0;
                }
                else if (item is IList clist
                    && item.GetType().IsGenericType
                    && item.GetType().GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                {
                    cnt += BatchDeleteSameType(clist);
                }
                //else
                //{
                //    throw new Exception($"�������治֧��[{item.GetType().Name}]���ͣ�");
                //}
            }
            return cnt;
        }
        #endregion

        #region ִ��
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

        /// <summary>
        /// ����ִ��
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
        internal static TableMapping GetMapping(Type type)
        {
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