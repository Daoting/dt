#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// mysql数据提供者，和客户端DataProvider方法基本相同
    /// </summary>
    public class DataProvider
    {
        #region 成员变量
        const string _unchangedMsg = "没有需要保存的数据！";
        const string _saveError = "数据源不可为空！";
        readonly MySqlAccess _db;
        List<DomainEvent> _domainEvents;
        #endregion

        #region 构造方法
        /// <summary>
        /// 禁止外部直接new
        /// </summary>
        /// <param name="p_isTransactional"></param>
        internal DataProvider(bool p_isTransactional)
        {
            // 根据方法的 Transaction 标签确定是否自动启动事务
            // 整个Api调用结束后自动提交或回滚事务、关闭连接
            _db = new MySqlAccess(false);
            if (p_isTransactional)
                _db.BeginTrans().Wait();
        }
        #endregion

        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return _db.Query(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public Task<Table<TEntity>> Query<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return _db.Query<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回Row枚举，高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Row枚举</returns>
        public Task<IEnumerable<Row>> Each(string p_keyOrSql, object p_params = null)
        {
            return _db.Each(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体枚举，高性能
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体枚举</returns>
        public Task<IEnumerable<TEntity>> Each<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return _db.Each<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public Task<Row> First(string p_keyOrSql, object p_params = null)
        {
            return _db.First(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public Task<TEntity> First<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return _db.First<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public Task<List<T>> FirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            return _db.FirstCol<T>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一列枚举，高性能
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型枚举</returns>
        public Task<IEnumerable<T>> EachFirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            return _db.EachFirstCol<T>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 返回所有实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns></returns>
        public Task<Table<TEntity>> GetAll<TEntity>()
            where TEntity : Entity
        {
            return _db.Query<TEntity>(EntitySchema.Get(typeof(TEntity)).Schema.GetSelectAllSql(), null);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            return _db.GetScalar<T>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，主键列名id，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public Task<TEntity> GetByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            return GetByKey<TEntity>("id", p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，主键列名id，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public Task<TEntity> GetByID<TEntity>(long p_id)
            where TEntity : Entity
        {
            return GetByKey<TEntity>("id", p_id.ToString());
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public async Task<TEntity> GetByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            var model = EntitySchema.Get(typeof(TEntity));
            TEntity entity = null;
            if (model.CacheHandler != null)
                entity = await model.CacheHandler.Get<TEntity>(p_keyName, p_keyVal);

            if (entity == null)
            {
                entity = await _db.First<TEntity>(
                    $"select * from `{model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                    new Dict { { p_keyName, p_keyVal } });

                if (entity != null && model.CacheHandler != null)
                    await model.CacheHandler.Cache(entity);
            }
            return entity;
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存实体数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <returns>是否成功</returns>
        public async Task<bool> Save<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.If(p_entity == null || (!p_entity.IsAdded && !p_entity.IsChanged), _unchangedMsg);

            var model = EntitySchema.Get(typeof(TEntity));
            if (model.OnSaving != null)
                await (Task)model.OnSaving.Invoke(p_entity, null);

            Dict dt = model.Schema.GetSaveSql(p_entity);
            if (await _db.Exec((string)dt["text"], (Dict)dt["params"]) != 1)
                return false;

            // 实体事件
            GatherSaveEvents(p_entity, model);

            // 更新实体时删除缓存
            if (model.CacheHandler != null && !p_entity.IsAdded && p_entity.IsChanged)
                await model.CacheHandler.Remove(p_entity);

            p_entity.AcceptChanges();
            return true;
        }

        /// <summary>
        /// 批量保存实体数据，根据实体状态执行增改，Table&lt;Entity&gt;支持删除，方法内部未启动事务！
        /// <para>列表类型支持：</para>
        /// <para>Table&lt;Entity&gt;，单表增删改</para>
        /// <para>List&lt;Entity&gt;，单表增改</para>
        /// <para>IList，多表增删改，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待保存</param>
        /// <returns>true 保存成功</returns>
        public Task<bool> BatchSave(IList p_list)
        {
            Throw.If(p_list == null || p_list.Count == 0, _unchangedMsg);

            Type tp = p_list.GetType();
            if (tp.IsGenericType
                && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
            {
                return BatchSaveSameType(p_list);
            }
            return BatchSaveMultiTypes(p_list);
        }

        /// <summary>
        /// 单表增删改，列表中的实体类型相同
        /// </summary>
        /// <param name="p_list"></param>
        /// <returns></returns>
        async Task<bool> BatchSaveSameType(IList p_list)
        {
            var model = EntitySchema.Get(p_list.GetType().GetGenericArguments()[0]);
            if (model.OnSaving != null)
            {
                foreach (var item in p_list)
                {
                    await (Task)model.OnSaving.Invoke(item, null);
                }
            }
            var dts = model.Schema.GetBatchSaveSql(p_list);

            // 不需要保存
            if (dts == null || dts.Count == 0)
                Throw.Msg(_unchangedMsg);

            await BatchExec(dts);

            // 实体事件、缓存
            foreach (var entity in p_list.OfType<Entity>())
            {
                if (entity.IsChanged || entity.IsAdded)
                    await ApplyEventAndCache(entity, model);
            }

            if (p_list is Table tbl)
                tbl.DeletedRows?.Clear();
            return true;
        }

        /// <summary>
        /// 多表增删改
        /// </summary>
        /// <param name="p_list"></param>
        /// <returns></returns>
        async Task<bool> BatchSaveMultiTypes(IList p_list)
        {
            var dts = new List<Dict>();
            foreach (var item in p_list)
            {
                if (item is Entity entity)
                {
                    if (entity.IsAdded || entity.IsChanged)
                    {
                        var model = EntitySchema.Get(item.GetType());
                        if (model.OnSaving != null)
                            await (Task)model.OnSaving.Invoke(entity, null);

                        dts.Add(model.Schema.GetSaveSql(entity));
                    }
                }
                else if (item is IList clist)
                {
                    // 不判断列表项数0，因可能Table<Entity>只包含删除列表的情况！
                    Type tp = item.GetType();
                    if (tp.IsGenericType && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                    {
                        // IList<Entity> 或 Table<Entity>
                        var model = EntitySchema.Get(tp.GetGenericArguments()[0]);
                        if (model.OnSaving != null)
                        {
                            foreach (var ci in clist)
                            {
                                await (Task)model.OnSaving.Invoke(ci, null);
                            }
                        }

                        var cdts = model.Schema.GetBatchSaveSql(clist);
                        if (cdts != null && cdts.Count > 0)
                            dts.AddRange(cdts);
                    }
                }
            }

            // 不需要保存
            if (dts == null || dts.Count == 0)
                Throw.Msg(_unchangedMsg);

            await BatchExec(dts);

            foreach (var item in p_list)
            {
                if (item is Entity entity)
                {
                    if (entity.IsChanged || entity.IsAdded)
                    {
                        await ApplyEventAndCache(entity, EntitySchema.Get(item.GetType()));
                    }
                }
                else if (item is IList clist && clist.Count > 0)
                {
                    Type tp = item.GetType();
                    if (tp.IsGenericType && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                    {
                        // IList<Entity> 或 Table<Entity>
                        var model = EntitySchema.Get(tp.GetGenericArguments()[0]);
                        foreach (var row in clist.OfType<Entity>())
                        {
                            if (row.IsAdded || row.IsChanged)
                            {
                                await ApplyEventAndCache(row, model);
                            }
                        }

                        if (item is Table tbl)
                            tbl.DeletedRows?.Clear();
                    }
                }
            }
            return true;
        }

        async Task ApplyEventAndCache(Entity p_entity, EntitySchema p_model)
        {
            if (p_entity.IsAdded || p_entity.IsChanged)
            {
                // 不直接调用GatherSaveEvents，确保事件参数类型准确，不然发布事件时无法类型转换！
                _gatherSaveEvents.MakeGenericMethod(p_entity.GetType()).Invoke(this, new object[] { p_entity, p_model });
                //GatherSaveEvents(p_entity, p_model);
                p_entity.AcceptChanges();
            }

            if (p_model.CacheHandler != null && !p_entity.IsAdded && p_entity.IsChanged)
                await p_model.CacheHandler.Remove(p_entity);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns>true 删除成功</returns>
        public async Task<bool> Delete<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.IfNull(p_entity, _saveError);

            var model = EntitySchema.Get(typeof(TEntity));
            if (model.OnDeleting != null)
                await (Task)model.OnDeleting.Invoke(p_entity, null);

            Dict dt = model.Schema.GetDeleteSql(new List<Entity> { p_entity });
            return await BatchExecDelete(dt, new List<Entity> { p_entity }, model) == 1;
        }

        /// <summary>
        /// 批量删除实体，单表或多表，列表类型支持：
        /// <para>Table&lt;Entity&gt;，单表删除</para>
        /// <para>List&lt;Entity&gt;，单表删除</para>
        /// <para>IList，多表删除，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待删除实体列表</param>
        /// <returns>实际删除行数</returns>
        public Task<int> BatchDelete(IList p_list)
        {
            Throw.If(p_list == null || p_list.Count == 0, _saveError);

            Type tp = p_list.GetType();
            if (tp.IsGenericType
                && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
            {
                return BatchDeleteSameType(p_list);
            }
            return BatchDeleteMultiTypes(p_list);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键id，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>实际删除行数</returns>
        public Task<bool> DelByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            return DelByKey<TEntity>("id", p_id);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键id，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>实际删除行数</returns>
        public Task<bool> DelByID<TEntity>(long p_id)
            where TEntity : Entity
        {
            return DelByKey<TEntity>("id", p_id.ToString());
        }

        /// <summary>
        /// 根据单主键或唯一索引列删除实体，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <returns>实际删除行数</returns>
        public Task<bool> DelByKey<TEntity>(string p_keyName, long p_keyVal)
            where TEntity : Entity
        {
            return DelByKey<TEntity>(p_keyName, p_keyVal.ToString());
        }

        /// <summary>
        /// 根据单主键或唯一索引列删除实体，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <returns>实际删除行数</returns>
        public async Task<bool> DelByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            var model = EntitySchema.Get(typeof(TEntity));

            // 有缓存或需要触发实体删除事件
            if (model.CacheHandler != null
                || (model.CudEvent & CudEvent.LocalDelete) == CudEvent.LocalDelete
                || (model.CudEvent & CudEvent.RemoteDelete) == CudEvent.RemoteDelete)
            {
                TEntity entity = null;
                if (model.CacheHandler != null)
                    entity = await model.CacheHandler.Get<TEntity>(p_keyName, p_keyVal);

                if (entity == null)
                {
                    entity = await _db.First<TEntity>(
                        $"select * from `{model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                        new Dict { { p_keyName, p_keyVal } });
                }
                if (entity != null)
                    return await Delete(entity);
                return false;
            }

            // 无缓存无事件直接删除
            return await _db.Exec(
                $"delete from `{model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                new Dict { { p_keyName, p_keyVal } }) == 1;
        }

        /// <summary>
        /// 单表批量删除
        /// </summary>
        /// <param name="p_list"></param>
        /// <returns></returns>
        async Task<int> BatchDeleteSameType(IList p_list)
        {
            var model = EntitySchema.Get(p_list.GetType().GetGenericArguments()[0]);
            if (model.OnDeleting != null)
            {
                foreach (var item in p_list)
                {
                    await (Task)model.OnDeleting.Invoke(item, null);
                }
            }

            Dict dt = model.Schema.GetDeleteSql(p_list);
            return await BatchExecDelete(dt, p_list, model);
        }

        /// <summary>
        /// 多表批量删除
        /// </summary>
        /// <param name="p_list"></param>
        /// <returns></returns>
        async Task<int> BatchDeleteMultiTypes(IList p_list)
        {
            int cnt = 0;
            foreach (var item in p_list)
            {
                if (item is Entity entity)
                {
                    var model = EntitySchema.Get(item.GetType());
                    if (model.OnDeleting != null)
                        await (Task)model.OnDeleting.Invoke(item, null);

                    var ls = new List<Row> { entity };
                    Dict dt = model.Schema.GetDeleteSql(ls);
                    cnt += await BatchExecDelete(dt, ls, model);
                }
                else if (item is IList clist && clist.Count > 0)
                {
                    Type tp = item.GetType();
                    if (tp.IsGenericType && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
                    {
                        // IList<Entity> 或 Table<Entity>
                        var model = EntitySchema.Get(tp.GetGenericArguments()[0]);
                        if (model.OnDeleting != null)
                        {
                            foreach (var ci in clist)
                            {
                                await (Task)model.OnDeleting.Invoke(item, null);
                            }
                        }

                        Dict dt = model.Schema.GetDeleteSql(clist);
                        cnt += await BatchExecDelete(dt, clist, model);
                    }
                }
            }
            return cnt;
        }

        /// <summary>
        /// 单表批量执行，运行sql删除、收集领域事件、同步缓存
        /// </summary>
        /// <param name="p_dt"></param>
        /// <param name="p_list"></param>
        /// <param name="p_model"></param>
        /// <returns></returns>
        async Task<int> BatchExecDelete(Dict p_dt, IList p_list, EntitySchema p_model)
        {
            int cnt = 0;
            string sql = (string)p_dt["text"];
            List<Dict> ls = (List<Dict>)p_dt["params"];
            var db = _db;

            for (int i = 0; i < p_list.Count; i++)
            {
                if (await db.Exec(sql, ls[i]) == 1)
                {
                    cnt++;
                    var entity = p_list[i];
                    // 删除实体事件，不直接调GatherDelEvents！
                    _gatherDelEvents.MakeGenericMethod(entity.GetType()).Invoke(this, new object[] { entity, p_model });
                    // 删除缓存
                    if (p_model.CacheHandler != null)
                        await p_model.CacheHandler.Remove(entity as Entity);
                }
            }
            return cnt;
        }
        #endregion

        #region Exec
        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        public Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return _db.Exec(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 批量执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public async Task<int> BatchExec(List<Dict> p_dts)
        {
            if (p_dts == null || p_dts.Count == 0)
                return 0;

            int cnt = 0;
            foreach (Dict dt in p_dts)
            {
                string sql = (string)dt["text"];
                if (dt["params"] is List<Dict> ls)
                {
                    foreach (var par in ls)
                    {
                        cnt += await _db.Exec(sql, par);
                    }
                }
                else if (dt["params"] is Dict par)
                {
                    cnt += await _db.Exec(sql, par);
                }
            }
            return cnt;
        }
        #endregion

        #region 领域事件

        // 确保事件参数类型准确，不然发布事件时无法类型转换！
        static MethodInfo _gatherSaveEvents = typeof(DataProvider).GetMethod("GatherSaveEvents", BindingFlags.Instance | BindingFlags.NonPublic);
        static MethodInfo _gatherDelEvents = typeof(DataProvider).GetMethod("GatherDelEvents", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 保存实体时收集待发布的领域事件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity"></param>
        /// <param name="p_model"></param>
        void GatherSaveEvents<TEntity>(TEntity p_entity, EntitySchema p_model)
            where TEntity : Entity
        {
            var events = p_entity.GetDomainEvents();
            if (events != null)
                AddDomainEvents(events);

            if (p_model.CudEvent != CudEvent.None)
            {
                if (p_entity.IsAdded)
                {
                    if ((p_model.CudEvent & CudEvent.LocalInsert) == CudEvent.LocalInsert)
                        AddDomainEvent(new DomainEvent(false, new InsertEvent<TEntity> { Entity = p_entity }));
                    if ((p_model.CudEvent & CudEvent.RemoteInsert) == CudEvent.RemoteInsert)
                        AddDomainEvent(new DomainEvent(true, new InsertEvent<TEntity> { Entity = p_entity }));
                }
                else if (p_entity.IsChanged)
                {
                    if ((p_model.CudEvent & CudEvent.LocalUpdate) == CudEvent.LocalUpdate)
                        AddDomainEvent(new DomainEvent(false, new UpdateEvent<TEntity> { Entity = p_entity }));
                    if ((p_model.CudEvent & CudEvent.RemoteUpdate) == CudEvent.RemoteUpdate)
                        AddDomainEvent(new DomainEvent(true, new UpdateEvent<TEntity> { Entity = p_entity }));
                }
            }
        }

        /// <summary>
        /// 删除实体时收集待发布的领域事件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity"></param>
        /// <param name="p_model"></param>
        void GatherDelEvents<TEntity>(TEntity p_entity, EntitySchema p_model)
            where TEntity : Entity
        {
            var events = p_entity.GetDomainEvents();
            if (events != null)
                AddDomainEvents(events);

            if (p_model.CudEvent != CudEvent.None)
            {
                if ((p_model.CudEvent & CudEvent.LocalDelete) == CudEvent.LocalDelete)
                    AddDomainEvent(new DomainEvent(false, new DeleteEvent<TEntity> { Entity = p_entity }));
                if ((p_model.CudEvent & CudEvent.RemoteDelete) == CudEvent.RemoteDelete)
                    AddDomainEvent(new DomainEvent(true, new DeleteEvent<TEntity> { Entity = p_entity }));
            }
        }

        void AddDomainEvents(IEnumerable<DomainEvent> p_events)
        {
            if (_domainEvents == null)
                _domainEvents = new List<DomainEvent>();
            _domainEvents.AddRange(p_events);
        }

        void AddDomainEvent(DomainEvent p_event)
        {
            if (_domainEvents == null)
                _domainEvents = new List<DomainEvent>();
            _domainEvents.Add(p_event);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// Api调用结束后释放资源，提交或回滚事务、关闭数据库连接、发布领域事件
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal async Task Close(bool p_suc)
        {
            await _db.Close(p_suc);

            // 发布领域事件
            if (p_suc && _domainEvents != null)
            {
                var localEB = Kit.GetObj<LocalEventBus>();
                var remoteEB = Kit.GetObj<RemoteEventBus>();
                foreach (var de in _domainEvents)
                {
                    if (de.IsRemoteEvent)
                        remoteEB.Broadcast(de.Event, false);
                    else
                        localEB.Publish(de.Event);
                }
            }
            _domainEvents?.Clear();
        }
        #endregion
    }
}
