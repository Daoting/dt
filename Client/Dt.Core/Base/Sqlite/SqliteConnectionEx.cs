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

        #region 保存
        /// <summary>
        /// 保存实体数据，根据实体状态确定插入或更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_entity">实体</param>
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
        /// 一个事务内批量保存实体数据，根据实体状态执行增改，Table&lt;Entity&gt;支持删除，列表类型支持：
        /// <para>Table&lt;Entity&gt;，单表增删改</para>
        /// <para>List&lt;Entity&gt;，单表增改</para>
        /// <para>IList，多表增删改，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待保存列表</param>
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

                // 自增列
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

                    // 只更新变化的列
                    if (updateVal.Length > 0)
                        updateVal.Append(", ");
                    updateVal.Append(col.Name);
                    updateVal.Append("=@");
                    updateVal.Append(updateIndex);

                    // 更新主键时避免重复
                    cmd.Parameters.AddWithValue(updateIndex.ToString(), p_entity[col.Name]);
                    updateIndex++;
                }

                // 主键可能被更新，不支持复合主键
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
                // 先处理插入
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

                // 更新
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

                // 删除
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
                    throw new Exception($"批量保存不支持[{item.GetType().Name}]类型！");
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

                // 只更新变化的列
                if (updateVal.Length > 0)
                    updateVal.Append(", ");
                updateVal.Append(col.Name);
                updateVal.Append("=@");
                updateVal.Append(updateIndex);

                // 更新主键时避免重复
                p_cmd.Parameters.AddWithValue(updateIndex.ToString(), p_entity[col.Name]);
                updateIndex++;
            }

            // 主键可能被更新，不支持复合主键
            p_cmd.Parameters.AddWithValue(p_map.PK.Name, p_entity.Cells[p_map.PK.Name].OriginalVal);
            p_cmd.CommandText = $"update '{p_map.TableName}' set {updateVal} where {p_map.PK.Name}=@{p_map.PK.Name}";
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的行</param>
        /// <returns>true 删除成功</returns>
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
        /// 根据主键删除实体对象，仅支持单主键，主键列名内部确定
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键值</param>
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
        /// 批量删除实体，单表或多表，列表类型支持：
        /// <para>Table&lt;Entity&gt;，单表删除</para>
        /// <para>List&lt;Entity&gt;，单表删除</para>
        /// <para>IList，多表删除，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待删除实体列表</param>
        /// <returns>true 删除成功</returns>
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
                //    throw new Exception($"批量保存不支持[{item.GetType().Name}]类型！");
                //}
            }
            return cnt;
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