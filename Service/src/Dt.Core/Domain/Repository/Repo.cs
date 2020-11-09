#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// mysql数据仓库，和客户端Repo方法基本相同
    /// </summary>
    public static class Repo
    {
        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<TEntity>> Query<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return LobContext.Current.Db.Query<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体枚举，高性能
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体枚举</returns>
        public static Task<IEnumerable<TEntity>> Each<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return LobContext.Current.Db.Each<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> Get<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return LobContext.Current.Db.Get<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，主键列名id，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(string p_id)
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
        public static Task<TEntity> GetByID<TEntity>(long p_id)
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
        public static async Task<TEntity> GetByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            var model = GetModel(typeof(TEntity));
            TEntity entity = null;
            if (model.CacheHandler != null)
                entity = await model.CacheHandler.Get<TEntity>(p_keyName, p_keyVal);

            if (entity == null)
            {
                entity = await LobContext.Current.Db.Get<TEntity>(
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
        public static async Task<bool> Save<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.If(p_entity == null || (!p_entity.IsAdded && !p_entity.IsChanged), _unchangedMsg);
            var model = GetModel(typeof(TEntity));

            // 保存前外部校验，不合格在外部抛出异常
            if (model.OnSaving != null)
            {
                if (model.OnSaving.ReturnType == typeof(Task))
                    await (Task)model.OnSaving.Invoke(p_entity, null);
                else
                    model.OnSaving.Invoke(p_entity, null);
            }

            Dict dt = model.Schema.GetSaveSql(p_entity);
            if (await LobContext.Current.Db.Exec((string)dt["text"], (Dict)dt["params"]) != 1)
                return false;

            // 实体事件
            GatherSaveEvents(p_entity);

            // 更新实体时删除缓存
            if (model.CacheHandler != null && !p_entity.IsAdded && p_entity.IsChanged)
                await model.CacheHandler.Remove(p_entity);

            p_entity.AcceptChanges();
            return true;
        }

        /// <summary>
        /// 批量保存实体数据，根据实体状态执行增改
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entities">待保存</param>
        /// <returns>true 保存成功</returns>
        public static async Task<bool> BatchSave<TEntity>(IList<TEntity> p_entities)
            where TEntity : Entity
        {
            Throw.If(p_entities == null || p_entities.Count == 0, _unchangedMsg);
            var model = GetModel(typeof(TEntity));

            // 保存前外部校验，不合格在外部抛出异常
            if (model.OnSaving != null)
            {
                foreach (var item in p_entities)
                {
                    if (model.OnSaving.ReturnType == typeof(Task))
                        await (Task)model.OnSaving.Invoke(item, null);
                    else
                        model.OnSaving.Invoke(item, null);
                }
            }

            List<Dict> dts = null; //model.Schema.GetBatchSaveSql(p_entities);

            // 不需要保存
            if (dts == null || dts.Count == 0)
                return true;

            // 批量执行多个Sql
            // 参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}
            var db = LobContext.Current.Db;
            foreach (Dict dt in dts)
            {
                string sql = (string)dt["text"];
                if (dt["params"] is List<Dict> ls)
                {
                    foreach (var par in ls)
                    {
                        await db.Exec(sql, par);
                    }
                }
                else if (dt["params"] is Dict par)
                {
                    await db.Exec(sql, par);
                }
            }

            // 实体事件
            foreach (TEntity entity in p_entities)
            {
                GatherSaveEvents(entity);
                if (model.CacheHandler != null && !entity.IsAdded && entity.IsChanged)
                    await model.CacheHandler.Remove(entity);
                entity.AcceptChanges();
            }
            return true;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns>实际删除行数</returns>
        public static async Task<int> Delete<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            Throw.IfNull(p_entity, _saveError);
            var model = GetModel(typeof(TEntity));

            // 删除前外部校验，不合格在外部抛出异常
            if (model.OnDeleting != null)
            {
                if (model.OnDeleting.ReturnType == typeof(Task))
                    await (Task)model.OnDeleting.Invoke(p_entity, null);
                else
                    model.OnDeleting.Invoke(p_entity, null);
            }

            Dict dt = model.Schema.GetDeleteSql(new List<Row> { p_entity });
            int cnt = await LobContext.Current.Db.Exec((string)dt["text"], ((List<Dict>)dt["params"])[0]);
            if (cnt == 1)
            {
                // 删除实体事件
                GatherDelEvents(p_entity);
                // 删除缓存
                if (model.CacheHandler != null)
                    await model.CacheHandler.Remove(p_entity);
            }
            return cnt;
        }

        /// <summary>
        /// 批量删除实体，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entities">实体列表</param>
        /// <returns>实际删除行数</returns>
        public static async Task<int> BatchDelete<TEntity>(IList<TEntity> p_entities)
            where TEntity : Entity
        {
            Throw.If(p_entities == null || p_entities.Count == 0, _saveError);
            var model = GetModel(typeof(TEntity));

            // 删除前外部校验，不合格在外部抛出异常
            if (model.OnDeleting != null)
            {
                foreach (var item in p_entities)
                {
                    if (model.OnDeleting.ReturnType == typeof(Task))
                        await (Task)model.OnDeleting.Invoke(item, null);
                    else
                        model.OnDeleting.Invoke(item, null);
                }
            }

            int cnt = 0;
            Dict dt = model.Schema.GetDeleteSql(p_entities);
            string sql = (string)dt["text"];
            List<Dict> ls = (List<Dict>)dt["params"];
            var db = LobContext.Current.Db;
            for (int i = 0; i < p_entities.Count; i++)
            {
                if (await db.Exec(sql, ls[i]) == 1)
                {
                    cnt++;
                    GatherDelEvents(p_entities[i]);
                    if (model.CacheHandler != null)
                        await model.CacheHandler.Remove(p_entities[i]);
                }
            }
            return cnt;
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键id，同步删除缓存，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>实际删除行数</returns>
        public static Task<int> DelByID<TEntity>(string p_id)
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
        public static Task<int> DelByID<TEntity>(long p_id)
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
        public static Task<int> DelByKey<TEntity>(string p_keyName, long p_keyVal)
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
        public static async Task<int> DelByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            var model = GetModel(typeof(TEntity));

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
                    entity = await LobContext.Current.Db.Get<TEntity>(
                        $"select * from `{model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                        new Dict { { p_keyName, p_keyVal } });
                }
                if (entity != null)
                    return await Delete(entity);
                return 0;
            }

            // 无缓存无事件直接删除
            return await LobContext.Current.Db.Exec(
                $"delete from `{model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                new Dict { { p_keyName, p_keyVal } });
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 创建空Table
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>空表</returns>
        public static Table<TEntity> NewTable<TEntity>()
            where TEntity : Entity
        {
            var model = GetModel(typeof(TEntity));
            var tbl = new Table<TEntity>();
            foreach (var col in model.Schema.PrimaryKey.Concat(model.Schema.Columns))
            {
                tbl.Columns.Add(new Column(col.Name, col.Type));
            }
            return tbl;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 保存实体时收集待发布的领域事件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity"></param>
        static void GatherSaveEvents<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            var lob = LobContext.Current;
            var events = p_entity.GetDomainEvents();
            if (events != null)
                lob.AddDomainEvents(events);

            var cudEvent = GetModel(typeof(TEntity)).CudEvent;
            if (cudEvent != CudEvent.None)
            {
                if (p_entity.IsAdded)
                {
                    if ((cudEvent & CudEvent.LocalInsert) == CudEvent.LocalInsert)
                        lob.AddDomainEvent(new DomainEvent(false, new InsertEvent<TEntity> { Entity = p_entity }));
                    if ((cudEvent & CudEvent.RemoteInsert) == CudEvent.RemoteInsert)
                        lob.AddDomainEvent(new DomainEvent(true, new InsertEvent<TEntity> { Entity = p_entity }));
                }
                else if (p_entity.IsChanged)
                {
                    if ((cudEvent & CudEvent.LocalUpdate) == CudEvent.LocalUpdate)
                        lob.AddDomainEvent(new DomainEvent(false, new UpdateEvent<TEntity> { Entity = p_entity }));
                    if ((cudEvent & CudEvent.RemoteUpdate) == CudEvent.RemoteUpdate)
                        lob.AddDomainEvent(new DomainEvent(true, new UpdateEvent<TEntity> { Entity = p_entity }));
                }
            }
        }

        /// <summary>
        /// 删除实体时收集待发布的领域事件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity"></param>
        static void GatherDelEvents<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            var lob = LobContext.Current;
            var events = p_entity.GetDomainEvents();
            if (events != null)
                lob.AddDomainEvents(events);

            var cudEvent = GetModel(typeof(TEntity)).CudEvent;
            if (cudEvent != CudEvent.None)
            {
                if ((cudEvent & CudEvent.LocalDelete) == CudEvent.LocalDelete)
                    lob.AddDomainEvent(new DomainEvent(false, new DeleteEvent<TEntity> { Entity = p_entity }));
                if ((cudEvent & CudEvent.RemoteDelete) == CudEvent.RemoteDelete)
                    lob.AddDomainEvent(new DomainEvent(true, new DeleteEvent<TEntity> { Entity = p_entity }));
            }
        }

        const string _unchangedMsg = "没有需要保存的数据！";
        const string _saveError = "数据源不可为空！";

        static readonly ConcurrentDictionary<Type, EntitySchema> _models = new ConcurrentDictionary<Type, EntitySchema>();

        static EntitySchema GetModel(Type p_type)
        {
            if (_models.TryGetValue(p_type, out var m))
                return m;

            var model = new EntitySchema(p_type);
            _models[p_type] = model;
            return model;
        }
        #endregion
    }
}
