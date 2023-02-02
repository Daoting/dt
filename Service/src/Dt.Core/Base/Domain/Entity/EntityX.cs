#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
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

            if (IsVirEntity(typeof(TEntity)))
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

            if (IsVirEntity(typeof(TEntity)))
                return await DelVirEntityDirect(p_ids, p_isNotify);
            return await DelDirect(p_ids, p_isNotify);
        }

        static async Task<bool> DelDirect(IList p_ids, bool p_isNotify = true)
        {
            var model = EntitySchema.Get(typeof(TEntity));
            Dict dt = model.Schema.GetDelSqlByIDs(p_ids);
            if (dt == null)
            {
#if !SERVER
                if (p_isNotify)
                    Kit.Warn("没有需要删除的数据！");
#endif
                return false;
            }

            var ac = model.AccessInfo.GetEntityAccess();
            bool suc;
            if (dt["params"] is Dict par)
            {
                suc = await ac.Exec((string)dt["text"], par) > 0;
            }
            else
            {
                suc = await ac.BatchExec(new List<Dict> { dt }) > 0;
            }

#if !SERVER
            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
#endif
            return suc;
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

            if (ls.Count == 0)
            {
#if !SERVER
                if (p_isNotify)
                    Kit.Warn("没有需要删除的数据！");
#endif
                return false;
            }

            var m = EntitySchema.Get(entities[0]);
            var ac = m.AccessInfo.GetEntityAccess();
            bool suc = await ac.BatchExec(ls) > 0;

#if !SERVER
            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
#endif
            return suc;
        }
        #endregion

        #region 查询
        /// <summary>
        /// 查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有，可能被注入</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Query(string p_filter = null, object p_params = null)
        {
#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_filter))
                sql += " where " + p_filter;
            return Kit.ContextEa.Query<TEntity>(sql, p_params);
#else
            var res = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_filter))
                res.Item2 += " where " + p_filter;
            return res.Item1.Query<TEntity>(res.Item2, p_params);
#endif
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Page(int p_starRow, int p_pageSize, string p_filter = null, object p_params = null)
        {
#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_filter))
                sql += " where " + p_filter;
            sql += $" limit {p_starRow},{p_pageSize} ";
            return Kit.ContextEa.Query<TEntity>(sql, p_params);
#else
            var res = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_filter))
                res.Item2 += " where " + p_filter;
            res.Item2 += $" limit {p_starRow},{p_pageSize} ";
            return res.Item1.Query<TEntity>(res.Item2, p_params);
#endif
        }

        /// <summary>
        /// 返回第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> First(string p_filter, object p_params = null)
        {
#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_filter))
                sql += " where " + p_filter;
            return Kit.ContextEa.First<TEntity>(sql, p_params);
#else
            var res = GetSelectSql(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(p_filter))
                res.Item2 += " where " + p_filter;
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
            if (p_id == null || string.IsNullOrEmpty(p_id.ToString()))
                return default;

            // 主键名不一定是id，但sql语句中的变量名是id！
            var dt = new Dict { { "id", p_id.ToString() } };

#if SERVER
            string sql;
            if (IsVirEntity(typeof(TEntity)))
            {
                var vm = VirEntitySchema.Get(typeof(TEntity));
                sql = vm.GetSelectByIDSql();
            }
            else
            {
                var model = EntitySchema.Get(typeof(TEntity));
                sql = model.Schema.GetSelectByIDSql();
            }

            return Kit.ContextEa.First<TEntity>(sql, dt);
#else
            string sql;
            IEntityAccess ea;
            if (IsVirEntity(typeof(TEntity)))
            {
                var vm = VirEntitySchema.Get(typeof(TEntity));
                sql = vm.GetSelectByIDSql();
                ea = vm.AccessInfo.GetEntityAccess();
            }
            else
            {
                var model = EntitySchema.Get(typeof(TEntity));
                sql = model.Schema.GetSelectByIDSql();
                ea = model.AccessInfo.GetEntityAccess();
            }
            return ea.First<TEntity>(sql, dt);
#endif
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByKey(string p_keyName, string p_keyVal)
        {
            Throw.IfEmpty(p_keyName, "GetByKey查询时主键或唯一索引列名不可为空！");
            var dt = new Dict { { p_keyName, p_keyVal } };

#if SERVER
            var sql = GetSelectSql(typeof(TEntity));
            sql += $" where {p_keyName}=@{p_keyName}";
            return Kit.ContextEa.First<TEntity>(sql, dt);
#else
            var res = GetSelectSql(typeof(TEntity));
            res.Item2 += $" where {p_keyName}=@{p_keyName}";
            return res.Item1.First<TEntity>(res.Item2, dt);
#endif
        }

#if SERVER
        static string GetSelectSql(Type p_type)
        {
            if (IsVirEntity(p_type))
            {
                return VirEntitySchema.Get(p_type).GetSelectAllSql();
            }
            return EntitySchema.Get(p_type).Schema.GetSelectAllSql();
        }
#else
        static (IEntityAccess, string) GetSelectSql(Type p_type)
        {
            if (IsVirEntity(p_type))
            {
                var vm = VirEntitySchema.Get(p_type);
                return (vm.AccessInfo.GetEntityAccess(), vm.GetSelectAllSql());
            }

            var model = EntitySchema.Get(p_type);
            return (model.AccessInfo.GetEntityAccess(), model.Schema.GetSelectAllSql());
        }
#endif
        #endregion

        #region 新ID和序列
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
            if (IsVirEntity(typeof(TEntity)))
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

            // 本地库采用自增
            return Task.FromResult(0L);
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
            if (IsVirEntity(typeof(TEntity)))
                Throw.Msg("无法通过虚拟实体获取新序列值时！");

            var model = EntitySchema.Get(typeof(TEntity));

            // 序列名称：表名+字段名，全小写
            var seqName = model.Schema.Name + "+" + p_colName.ToLower();
            int seq = 0;
#if SERVER
            seq = await Kit.ContextEa.GetScalar<int>($"select nextval('{seqName}')");
#else
            Throw.If(model.AccessInfo.Type == AccessType.Local, "暂不支持sqlite本地库的序列功能！");
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
    }
}
