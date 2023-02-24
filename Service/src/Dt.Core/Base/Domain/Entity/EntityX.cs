#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 借用泛型参数 TEntity 实现通用的静态方法
    /// </summary>
    /// <typeparam name="TEntity">实体类型参数</typeparam>
    public abstract class EntityX<TEntity> : Entity
        where TEntity : Entity
    {
        #region 构造方法
        public EntityX()
        { }

        /// <summary>
        /// 和外部共用Cells
        /// </summary>
        /// <param name="p_cells"></param>
        public EntityX(CellList p_cells)
            : base(p_cells)
        { }
        #endregion

        #region 删除
        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，支持 直接删除 或 通过实体系统删除
        /// <para>直接删除就是根据主键删除数据，和实体系统无关，默认方式</para>
        /// <para>非直接删除时，删除前先根据主键获取该实体对象，进行删除前的校验，删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_directDel">是否直接删除不通过实体系统，默认true</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> DelByID(object p_id, bool p_directDel = true, bool p_isNotify = true)
        {
            if (p_id == null || string.IsNullOrEmpty(p_id.ToString()))
            {
#if !SERVER
                if (p_isNotify)
                    Kit.Warn("根据主键删除实体对象时主键不可为空！");
#endif
                return false;
            }

            if (!p_directDel)
            {
                var ew = new EntityWriter();
                await ew.DelByID<TEntity>(p_id);
                return await ew.Commit(p_isNotify);
            }

            if (_isVirEntity)
                return await DelVirEntityDirect(new List<object> { p_id }, p_isNotify);
            return await DelDirect(new List<object> { p_id }, p_isNotify);
        }

        /// <summary>
        /// 根据主键批量删除实体对象，仅支持单主键，支持 直接删除 或 通过实体系统删除
        /// <para>直接删除就是根据主键删除数据，和实体系统无关，默认方式</para>
        /// <para>非直接删除时，删除前先根据主键获取该实体对象，进行删除前的校验，删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_ids">主键列表</param>
        /// <param name="p_directDel">是否直接删除不通过实体系统，默认true</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> DelByIDs(IList p_ids, bool p_directDel = true, bool p_isNotify = true)
        {
            if (p_ids == null || p_ids.Count == 0)
            {
#if !SERVER
                if (p_isNotify)
                    Kit.Warn("根据主键批量删除实体对象时主键列表不可为空！");
#endif
                return false;
            }

            if (!p_directDel)
            {
                var ew = new EntityWriter();
                await ew.DelByIDs<TEntity>(p_ids);
                return await ew.Commit(p_isNotify);
            }

            if (_isVirEntity)
                return await DelVirEntityDirect(p_ids, p_isNotify);
            return await DelDirect(p_ids, p_isNotify);
        }

        static async Task<bool> DelDirect(IList p_ids, bool p_isNotify = true)
        {
            var model = EntitySchema.Get(typeof(TEntity));
            Dict dt = model.Schema.GetDelSqlByIDs(p_ids);
#if SERVER
            if (dt == null)
                return false;

            bool suc;
            if (dt["params"] is Dict par)
            {
                suc = await Kit.DataAccess.Exec((string)dt["text"], par) > 0;
            }
            else
            {
                suc = await Kit.DataAccess.BatchExec(new List<Dict> { dt }) > 0;
            }
            return suc;
#else
            if (dt == null)
            {
                if (p_isNotify)
                    Kit.Warn("没有需要删除的数据！");
                return false;
            }

            var ac = model.AccessInfo.GetDataAccess();
            bool suc;
            if (dt["params"] is Dict par)
            {
                suc = await ac.Exec((string)dt["text"], par) > 0;
            }
            else
            {
                suc = await ac.BatchExec(new List<Dict> { dt }) > 0;
            }

            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
            return suc;
#endif
        }

        static async Task<bool> DelVirEntityDirect(IList p_ids, bool p_isNotify = true)
        {
            Type tpVir = typeof(TEntity);
            while (!tpVir.IsGenericType)
            {
                tpVir = tpVir.BaseType;
            }

            var ls = new List<Dict>();
            var entities = tpVir.GenericTypeArguments;

            // 虚拟实体内部包含的实体对象
            foreach (var tp in entities)
            {
                var model = EntitySchema.Get(tp);
                Dict dt = model.Schema.GetDelSqlByIDs(p_ids);
                if (dt != null)
                    ls.Add(dt);
            }

#if SERVER
            if (ls.Count == 0)
                return false;

            return await Kit.DataAccess.BatchExec(ls) > 0;
#else
            if (ls.Count == 0)
            {
                if (p_isNotify)
                    Kit.Warn("没有需要删除的数据！");
                return false;
            }

            var m = EntitySchema.Get(entities[0]);
            var ac = m.AccessInfo.GetDataAccess();
            bool suc = await ac.BatchExec(ls) > 0;

            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
            return suc;
#endif
        }
        #endregion

        #region 查询
        /// <summary>
        /// 查询实体列表，可以提供 where子句 或 Sql字典的键名 或 Sql语句进行查询
        /// </summary>
        /// <param name="p_whereOrKeyOrSql">三种查询：
        /// <para>1. where子句，以where开头的过滤条件，返回的实体包含所有列值</para>
        /// <para>2. Sql键名或Sql语句，自由查询，返回的实体列值自由</para>
        /// <para>3. null时返回所有实体</para>
        /// </param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Query(string p_whereOrKeyOrSql, object p_params = null)
        {
#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_whereOrKeyOrSql))
            {
                var txt = p_whereOrKeyOrSql.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    sql += " " + txt;
                }
                else
                {
                    sql = txt;
                }
            }
            return Kit.DataAccess.Query<TEntity>(sql, p_params);
#else
            var res = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_whereOrKeyOrSql))
            {
                var txt = p_whereOrKeyOrSql.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    res.Item2 += " " + txt;
                }
                else
                {
                    res.Item2 = txt;
                }
            }
            return res.Item1.Query<TEntity>(res.Item2, p_params);
#endif
        }

        /// <summary>
        /// 按页查询实体列表，可以提供 where子句 或 Sql字典的键名 或 Sql语句进行查询
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_whereOrKeyOrSql">三种查询：
        /// <para>1. where子句，以where开头的过滤条件，返回的实体包含所有列值</para>
        /// <para>2. Sql键名或Sql语句，自由查询，返回的实体列值自由</para>
        /// <para>3. null时返回所有实体</para>
        /// </param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Page(int p_starRow, int p_pageSize, string p_whereOrKeyOrSql, object p_params = null)
        {
#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_whereOrKeyOrSql))
            {
                var txt = p_whereOrKeyOrSql.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    sql += " " + txt;
                }
                else
                {
                    // Sql键名或Sql语句，自由查询
                    return Kit.DataAccess.Page<TEntity>(p_starRow, p_pageSize, p_whereOrKeyOrSql, p_params);
                }
            }
            sql += $" limit {p_starRow},{p_pageSize} ";
            return Kit.DataAccess.Query<TEntity>(sql, p_params);
#else
            var res = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_whereOrKeyOrSql))
            {
                var txt = p_whereOrKeyOrSql.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    res.Item2 += " " + txt;
                }
                else
                {
                    // Sql键名或Sql语句，自由查询
                    return res.Item1.Page<TEntity>(p_starRow, p_pageSize, p_whereOrKeyOrSql, p_params);
                }
            }

            res.Item2 += $" limit {p_starRow},{p_pageSize} ";
            return res.Item1.Query<TEntity>(res.Item2, p_params);
#endif
        }

        /// <summary>
        /// 返回第一个实体对象，不存在时返回null，可以提供 where子句 或 Sql字典的键名 或 Sql语句进行查询
        /// </summary>
        /// <param name="p_whereOrKeyOrSql">三种查询：
        /// <para>1. where子句，以where开头的过滤条件，返回的实体包含所有列值</para>
        /// <para>2. Sql键名或Sql语句，自由查询，返回的实体列值自由</para>
        /// <para>3. null时返回第一个实体</para>
        /// </param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> First(string p_whereOrKeyOrSql, object p_params = null)
        {
#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_whereOrKeyOrSql))
            {
                var txt = p_whereOrKeyOrSql.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    sql += " " + txt;
                }
                else
                {
                    sql = txt;
                }
            }
            return Kit.DataAccess.First<TEntity>(sql, p_params);
#else
            var res = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_whereOrKeyOrSql))
            {
                var txt = p_whereOrKeyOrSql.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    res.Item2 += " " + txt;
                }
                else
                {
                    res.Item2 = txt;
                }
            }
            return res.Item1.First<TEntity>(res.Item2, p_params);
#endif
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID(object p_id)
        {
            if (p_id == null || string.IsNullOrWhiteSpace(p_id.ToString()))
                return Task.FromResult(default(TEntity));

            if (_isVirEntity)
            {
                // 虚拟实体不涉及缓存
                var vm = VirEntitySchema.Get(typeof(TEntity));
                return GetByKeyInternal(vm.PrimaryKeyName, p_id.ToString());
            }

            var model = EntitySchema.Get(typeof(TEntity));
            if (model.Schema.PrimaryKey.Count != 1)
                Throw.Msg("根据主键获得实体对象时仅支持单主键！");

            return GetByKey(model.Schema.PrimaryKey[0].Name, p_id.ToString());
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static async Task<TEntity> GetByKey(string p_keyName, string p_keyVal)
        {
            if (string.IsNullOrWhiteSpace(p_keyName) || string.IsNullOrWhiteSpace(p_keyVal))
                Throw.Msg("GetByKey查询时主键或唯一索引不可为空！");

            if (_isVirEntity)
            {
                // 虚拟实体不涉及缓存
                return await GetByKeyInternal(p_keyName, p_keyVal);
            }

            TEntity entity = null;
            var model = EntitySchema.Get(typeof(TEntity));
            if (model.Cacher != null)
            {
                // 首先从缓存中获取，有则直接返回
                entity = await model.Cacher.Get<TEntity>(p_keyName, p_keyVal);
            }

            if (entity == null)
            {
                // 无则查询数据库
                entity = await GetByKeyInternal(p_keyName, p_keyVal);
                // 并将查询结果添加到缓存以备下次使用
                if (entity != null && model.Cacher != null)
                    await model.Cacher.Cache(entity);
            }
            return entity;
        }

        /// <summary>
        /// 根据主键获得实体对象及所有子实体列表，仅支持单主键，不涉及缓存！
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static async Task<TEntity> GetByIDWithChild(object p_id)
        {
            if (_isVirEntity)
                Throw.Msg("虚拟实体不支持子实体列表！");

            if (p_id == null || string.IsNullOrWhiteSpace(p_id.ToString()))
                return default;

            var model = EntitySchema.Get(typeof(TEntity));
            if (model.Schema.PrimaryKey.Count != 1)
                Throw.Msg("根据主键获得实体对象时仅支持单主键！");

            var parent = await GetByKeyInternal(model.Schema.PrimaryKey[0].Name, p_id.ToString());
            if (parent == null)
                return default;

            if (model.Children.Count == 0)
                return parent;

#if SERVER
            var da = Kit.DataAccess;
#else
            var da = model.AccessInfo.GetDataAccess();
#endif
            var query = typeof(IDataAccess).GetMethod("Query", 1, new Type[2] { typeof(string), typeof(object) });
            foreach (var child in model.Children)
            {
                // 构造泛型方法
                var mi = query.MakeGenericMethod(child.Type);

                // 调用Query<>方法，sql变量名parentid固定
                var task = (Task)mi.Invoke(da, new object[2] { child.SqlSelect, new Dict { { "parentid", p_id } } });
                await task;
                var result = task.GetType().GetProperty("Result").GetValue(task);

                // 设置属性值为子实体列表
                child.PropInfo.SetValue(parent, result);
            }
            return parent;
        }

        static Task<TEntity> GetByKeyInternal(string p_keyName, string p_keyVal)
        {
            // 不再校验
            // if (string.IsNullOrWhiteSpace(p_keyName) || string.IsNullOrWhiteSpace(p_keyVal))
            //    return default;

            var dt = new Dict { { p_keyName, p_keyVal } };

#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            sql += $" where {(_isVirEntity ? "a." : "")}{p_keyName}=@{p_keyName}";
            return Kit.DataAccess.First<TEntity>(sql, dt);
#else
            var res = GetSelectSql(typeof(TEntity));
            // 虚拟实体需要a.前缀
            res.Item2 += $" where {(_isVirEntity ? "a." : "")}{p_keyName}=@{p_keyName}";
            return res.Item1.First<TEntity>(res.Item2, dt);
#endif
        }

#if SERVER
        static string GetSelectSql(Type p_type)
        {
            if (_isVirEntity)
            {
                return VirEntitySchema.Get(p_type).GetSelectAllSql();
            }
            return EntitySchema.Get(p_type).Schema.GetSelectAllSql();
        }
#else
        static (IDataAccess, string) GetSelectSql(Type p_type)
        {
            if (_isVirEntity)
            {
                var vm = VirEntitySchema.Get(p_type);
                return (vm.AccessInfo.GetDataAccess(), vm.GetSelectAllSql());
            }

            var model = EntitySchema.Get(p_type);
            return (model.AccessInfo.GetDataAccess(), model.Schema.GetSelectAllSql());
        }
#endif
        #endregion

        #region 新ID和序列
#if !SERVER
        static readonly SnowflakeId _snowflake = new SnowflakeId();
#endif

        /// <summary>
        /// 获取新ID，统一服务端和客户端写法
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
#if SERVER
            return Task.FromResult(Kit.NewID);
#else
            AccessInfo ai;
            if (_isVirEntity)
            {
                ai = VirEntitySchema.Get(typeof(TEntity)).AccessInfo;
            }
            else
            {
                ai = EntitySchema.Get(typeof(TEntity)).AccessInfo;
            }

            if (ai.Type == AccessType.Remote)
            {
                return Kit.Rpc<long>(ai.Name, "Da.NewID");
            }

            // 本地库，和服务端算法的_workerId不同
            return Task.FromResult(_snowflake.NextId());
#endif
        }

        /// <summary>
        /// 获取新序列值，序列名称全小写：表名+字段名，需要在sequence表中手动添加序列名称行
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static async Task<int> NewSeq(string p_colName)
        {
            Throw.IfEmpty(p_colName, "获取新序列值时，需要提供字段名称！");
            if (_isVirEntity)
                Throw.Msg("无法通过虚拟实体获取新序列值时！");

            var model = EntitySchema.Get(typeof(TEntity));

            // 序列名称：表名+字段名，全小写
            var seqName = model.Schema.Name + "+" + p_colName.ToLower();
            int seq = 0;
#if SERVER
            seq = await Kit.DataAccess.GetScalar<int>($"select nextval('{seqName}')");
#else
            Throw.If(model.AccessInfo.Type == AccessType.Local, "sqlite本地库不支持序列功能！");
            seq = await Kit.Rpc<int>(
                model.AccessInfo.Name,
                "Da.NewSeq",
                seqName
            );
#endif
            Throw.If(seq == 0, $"序列【{seqName}】不存在，请在sequence表中手动添加！");
            return seq;
        }
        #endregion

        #region 工具方法
        protected static void AddEntityCells(Entity p_entity, Type p_type)
        {
            var schema = EntitySchema.Get(p_type).Schema;
            foreach (var col in schema.PrimaryKey.Concat(schema.Columns))
            {
                if (!p_entity.Contains(col.Name))
                {
                    new Cell(p_entity, col.Name, col.Type);
                }
            }
        }

        // 是否为虚拟实体
        static bool _isVirEntity = typeof(TEntity).GetInterface("IVirEntity") == typeof(IVirEntity);
        #endregion
    }
}
