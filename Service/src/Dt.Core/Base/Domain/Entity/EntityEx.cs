#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// Entity扩展
    /// </summary>
    public static class EntityEx
    {
        #region 保存
        /// <summary>
        /// 保存实体数据，成功后：
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save<TEntity>(this TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.Save(p_entity);
            return await uw.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量保存Table中新增、修改、删除的实体数据
        /// <para>删除行通过Table的ExistDeleted DeletedRows判断获取</para>
        /// <para>保存成功后，对于每个保存的实体：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_tbl">实体表</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save<TEntity>(this Table<TEntity> p_tbl, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.Save(p_tbl);
            return await uw.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量保存实体数据，根据实体状态执行新增、修改
        /// <para>保存成功后，对于每个保存的实体：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <param name="p_list">待保存列表</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>true 保存成功</returns>
        public static async Task<bool> Save<TEntity>(this IEnumerable<TEntity> p_list, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.Save(p_list);
            return await uw.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量保存一对多的父实体数据和所有子实体数据
        /// <para>保存成功后，对于每个保存的实体：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <typeparam name="TEntity">父实体类型</typeparam>
        /// <param name="p_entity">待保存的父实体</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> SaveWithChild<TEntity>(this TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.SaveWithChild(p_entity);
            return await uw.Commit(p_isNotify);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体，依靠数据库的级联删除自动删除子实体，成功后：
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的实体</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete<TEntity>(this TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.Delete(p_entity);
            return await uw.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量删除实体数据，成功后对于每个删除的实体：
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_list">待删除实体列表</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete<TEntity>(this IList<TEntity> p_list, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.Delete(p_list);
            return await uw.Commit(p_isNotify);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> DelByID<TEntity>(string p_id, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.DelByID<TEntity>(p_id);
            return await uw.Commit(p_isNotify);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> DelByID<TEntity>(long p_id, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.DelByID<TEntity>(p_id);
            return await uw.Commit(p_isNotify);
        }

        /// <summary>
        /// 根据单主键或唯一索引列删除实体，删除前先获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>实际删除行数</returns>
        public static async Task<bool> DelByKey<TEntity>(string p_keyName, string p_keyVal, bool p_isNotify = true)
            where TEntity : Entity
        {
            var uw = new UnitOfWork();
            await uw.DelByKey<TEntity>(p_keyName, p_keyVal);
            return await uw.Commit(p_isNotify);
        }
        #endregion

        #region 查询
        /// <summary>
        /// 查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有，可能被注入</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Query<TEntity>(string p_filter = null, object p_params = null)
            where TEntity : Entity
        {
            var model = EntitySchema.Get(typeof(TEntity));
            var sql = model.Schema.GetSelectAllSql();
            if (!string.IsNullOrWhiteSpace(p_filter))
                sql += " where " + p_filter;

#if SERVER
            return Kit.ContextDp.Query<TEntity>(sql, p_params);
#else
            return Kit.Rpc<Table<TEntity>>(
                model.SvcName,
                "Da.Query",
                sql,
                p_params);
#endif
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Page<TEntity>(int p_starRow, int p_pageSize, string p_filter = null, object p_params = null)
            where TEntity : Entity
        {
            var model = EntitySchema.Get(typeof(TEntity));
            var sql = model.Schema.GetSelectAllSql();
            if (!string.IsNullOrWhiteSpace(p_filter))
                sql += " where " + p_filter;
            sql += $" limit {p_starRow},{p_pageSize} ";

#if SERVER
            return Kit.ContextDp.Query<TEntity>(sql, p_params);
#else
            return Kit.Rpc<Table<TEntity>>(
                model.SvcName,
                "Da.Query",
                sql,
                p_params);
#endif
        }

        /// <summary>
        /// 返回第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> First<TEntity>(string p_filter, object p_params = null)
            where TEntity : Entity
        {
            var model = EntitySchema.Get(typeof(TEntity));
            var sql = model.Schema.GetSelectAllSql();
            if (!string.IsNullOrWhiteSpace(p_filter))
                sql += " where " + p_filter;

#if SERVER
            return Kit.ContextDp.First<TEntity>(sql, p_params);
#else
            return Kit.Rpc<TEntity>(
                model.SvcName,
                "Da.First",
                sql,
                p_params);
#endif
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            // 主键名不一定是id，但sql语句中的变量名是id！
            return GetByKey<TEntity>("id", p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(long p_id)
            where TEntity : Entity
        {
            return GetByKey<TEntity>("id", p_id.ToString());
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            var model = EntitySchema.Get(typeof(TEntity));
            var sql = $"select * from `{model.Schema.Name}` where {p_keyName}=@{p_keyName}";
            var dt = new Dict { { p_keyName, p_keyVal } };

#if SERVER
            return Kit.ContextDp.First<TEntity>(
                sql,
                dt);
#else
            return Kit.Rpc<TEntity>(
                model.SvcName,
                "Da.First",
                sql,
                dt);
#endif
        }
        #endregion

        #region 新ID和序列
        /// <summary>
        /// 获取新ID，统一服务端和客户端写法
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static Task<long> GetNewID<TEntity>()
            where TEntity : Entity
        {
#if SERVER
            return Task.FromResult(Kit.NewID);
#else
            var model = EntitySchema.Get(typeof(TEntity));
            return Kit.Rpc<long>(
                model.SvcName,
                "Da.NewID"
            );
#endif
        }

        /// <summary>
        /// 获取新序列值，序列名称全小写：表名+字段名，需要在sequence表中手动添加序列名称行
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static async Task<int> GetNewSeq<TEntity>(string p_colName)
            where TEntity : Entity
        {
            Throw.IfEmpty(p_colName, "获取新序列值时，需要提供字段名称！");
            var model = EntitySchema.Get(typeof(TEntity));

            // 序列名称：表名+字段名，全小写
            var seqName = model.Schema.Name + "+" + p_colName.ToLower();
            int seq = 0;
#if SERVER
            seq = await Kit.ContextDp.GetScalar<int>($"select nextval('{seqName}')");
#else
            seq = await Kit.Rpc<int>(
                model.SvcName,
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
