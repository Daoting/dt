#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-18 创建
******************************************************************************/
#endregion

#region 引用命名
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
    /// mysql仓库
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repo<TEntity> : IRepository
        where TEntity : Entity
    {
        #region 静态内容
        protected static readonly EntitySchema _model;
        static readonly CacheHandler<TEntity> _cache;
        static readonly CudEvent _cudEvent;

        // 泛型方法 Query<TRow>
        static MethodInfo _query = typeof(Db).GetMethod("Query", 1, new Type[] { typeof(string), typeof(object) });
        // 泛型方法 GetBatchSaveSql
        static MethodInfo _getBatchSaveSql = typeof(TableSchema).GetMethod("GetBatchSaveSql");

        static Repo()
        {
            Type type = typeof(TEntity);
            _model = new EntitySchema(type);

            // 缓存设置
            var cfg = type.GetCustomAttribute<CacheAttribute>(false);
            if (cfg != null && !string.IsNullOrEmpty(cfg.PrefixKey))
                _cache = new CacheHandler<TEntity>(_model, cfg);

            // 触发增删改事件的类型
            var cud = type.GetCustomAttribute<CudEventAttribute>(false);
            if (cud != null)
                _cudEvent = cud.Event;
            else
                _cudEvent = CudEvent.None;
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 业务线上下文
        /// </summary>
        protected readonly LobContext _ = LobContext.Current;
        #endregion

        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体列表，未加载实体的附加数据！
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public Task<Table<TEntity>> Query(string p_keyOrSql, object p_params = null)
        {
            return _.Db.Query<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回实体枚举，未加载实体的附加数据！高性能
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体枚举</returns>
        public Task<IEnumerable<TEntity>> Each(string p_keyOrSql, object p_params = null)
        {
            return _.Db.Each<TEntity>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public async Task<TEntity> Get(string p_keyOrSql, object p_params = null, bool p_loadDetails = false)
        {
            TEntity entity = await _.Db.Get<TEntity>(p_keyOrSql, p_params);
            if (entity != null && p_loadDetails)
                await LoadDetails(entity);
            return entity;
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，主键列名id，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public Task<TEntity> GetByID(string p_id, bool p_loadDetails = false)
        {
            return GetByKey("id", p_id, p_loadDetails);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <param name="p_keyName">主键列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public async Task<TEntity> GetByKey(string p_keyName, string p_keyVal, bool p_loadDetails = false)
        {
            TEntity entity = null;
            if (_cache != null)
                entity = await _cache.Get(p_keyName, p_keyVal);

            if (entity == null)
            {
                entity = await _.Db.Get<TEntity>(
                    $"select * from `{_model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                    new Dict { { p_keyName, p_keyVal } });

                if (entity != null && _cache != null)
                    await _cache.Cache(entity);
            }

            if (entity != null && p_loadDetails)
                await LoadDetails(entity);
            return entity;
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            return _.Db.GetScalar<T>(p_keyOrSql, p_params);
        }

        /// <summary>
        /// 加载附加数据，默认加载所有关联的子表数据
        /// </summary>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        public virtual async Task LoadDetails(TEntity p_entity)
        {
            if (!_model.ExistChild)
                return;

            foreach (var child in _model.Children)
            {
                // 实例化泛型方法 Query<TRow>
                Task task = _query.MakeGenericMethod(child.Type).Invoke(_.Db, new object[] { child.SqlSelect, new { parentid = p_entity.ID } }) as Task;
                await task;
                object result = task.GetType().GetProperty("Result").GetValue(task);
                child.PropInfo.SetValue(p_entity, result);
            }
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存实体数据，同步保存子表数据
        /// </summary>
        /// <param name="p_entity">待保存的实体</param>
        /// <returns>是否成功</returns>
        public async Task<bool> Save(TEntity p_entity)
        {
            // p_entity为null时已触发异常
            Dict dt = _model.Schema.GetSaveSql(p_entity);

            // 无需保存
            if (dt == null)
                return true;

            bool isBatch = false;
            if (_model.ExistChild)
            {
                // 含子表，同步保存
                List<Dict> dts = new List<Dict>();
                List<Row> childRows = new List<Row>();
                GetChildSaveSql(p_entity, dts, childRows);

                // 需要批量保存
                if (dts.Count > 0)
                {
                    isBatch = true;
                    dts.Insert(0, dt);
                    if (!await BatchExec(dts))
                        return false;

                    // 批量成功
                    childRows.ForEach(t => t.AcceptChanges());
                }
            }

            if (!isBatch && await _.Db.Exec((string)dt["text"], (Dict)dt["params"]) != 1)
                return false;

            // 实体事件
            GatherSaveEvents(p_entity);

            // 更新实体时删除缓存
            if (_cache != null && !p_entity.IsAdded && p_entity.IsChanged)
                await _cache.Remove(p_entity);

            p_entity.AcceptChanges();
            return true;
        }

        /// <summary>
        /// 批量保存实体数据，根据实体状态执行增改，同步保存每个实体的子表数据
        /// </summary>
        /// <param name="p_entities">待保存</param>
        /// <returns>true 保存成功</returns>
        public async Task<bool> BatchSave(IList<TEntity> p_entities)
        {
            List<Dict> dts = _model.Schema.GetBatchSaveSql(p_entities);
            if (dts == null)
                dts = new List<Dict>();

            List<Row> childRows = null;
            if (_model.ExistChild)
            {
                childRows = new List<Row>();
                foreach (TEntity entity in p_entities)
                {
                    GetChildSaveSql(entity, dts, childRows);
                }
            }

            // 不需要保存
            if (dts.Count == 0)
                return true;

            bool suc = await BatchExec(dts);
            if (!suc)
                return false;

            // 实体事件
            foreach (TEntity entity in p_entities)
            {
                GatherSaveEvents(entity);
                if (_cache != null && !entity.IsAdded && entity.IsChanged)
                    await _cache.Remove(entity);
                entity.AcceptChanges();
            }

            if (childRows != null && childRows.Count > 0)
                childRows.ForEach(t => t.AcceptChanges());
            return true;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns>实际删除行数</returns>
        public async Task<int> Delete(TEntity p_entity)
        {
            Check.NotNull(p_entity);
            Dict dt = _model.Schema.GetDeleteSql(new List<Row> { p_entity });
            int cnt = await _.Db.Exec((string)dt["text"], ((List<Dict>)dt["params"])[0]);
            if (cnt == 1)
            {
                // 删除实体事件
                GatherDelEvents(p_entity);
                // 删除缓存
                if (_cache != null)
                    await _cache.Remove(p_entity);
            }
            return cnt;
        }

        /// <summary>
        /// 批量删除实体，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <param name="p_entities">实体列表</param>
        /// <returns>实际删除行数</returns>
        public async Task<int> BatchDelete(IList<TEntity> p_entities)
        {
            if (p_entities == null || p_entities.Count == 0)
                return 0;

            int cnt = 0;
            Dict dt = _model.Schema.GetDeleteSql(p_entities);
            string sql = (string)dt["text"];
            List<Dict> ls = (List<Dict>)dt["params"];
            for (int i = 0; i < p_entities.Count; i++)
            {
                if (await _.Db.Exec(sql, ls[i]) == 1)
                {
                    cnt++;
                    GatherDelEvents(p_entities[i]);
                    if (_cache != null)
                        await _cache.Remove(p_entities[i]);
                }
            }
            return cnt;
        }

        /// <summary>
        /// 根据主键删除实体对象，主键列名id，仅支持单主键
        /// 依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>实际删除行数</returns>
        public Task<int> DelByID(string p_id)
        {
            return DelByKey("id", p_id);
        }

        /// <summary>
        /// 根据单主键或唯一索引列删除实体
        /// 依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <returns>实际删除行数</returns>
        public async Task<int> DelByKey(string p_keyName, string p_keyVal)
        {
            // 有缓存或需要触发实体删除事件
            if (_cache != null
                || (_cudEvent & CudEvent.LocalDelete) == CudEvent.LocalDelete
                || (_cudEvent & CudEvent.RemoteDelete) == CudEvent.RemoteDelete)
            {
                TEntity entity = null;
                if (_cache != null)
                    entity = await _cache.Get(p_keyName, p_keyVal);

                if (entity == null)
                {
                    entity = await _.Db.Get<TEntity>(
                        $"select * from `{_model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                        new Dict { { p_keyName, p_keyVal } });
                }
                if (entity != null)
                    return await Delete(entity);
                return 0;
            }

            // 无缓存无事件直接删除
            return await _.Db.Exec(
                $"delete from `{_model.Schema.Name}` where {p_keyName}=@{p_keyName}",
                new Dict { { p_keyName, p_keyVal } });
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 创建空Table
        /// </summary>
        /// <returns>空表</returns>
        public Table<TEntity> NewTable()
        {
            var tbl = new Table<TEntity>();
            foreach (var col in _model.Schema.PrimaryKey.Concat(_model.Schema.Columns))
            {
                tbl.Columns.Add(new Column(col.Name, col.Type));
            }
            return tbl;
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 生成保存子实体的sql
        /// </summary>
        /// <param name="p_entity"></param>
        /// <param name="p_dts"></param>
        /// <param name="p_childRows"></param>
        void GetChildSaveSql(TEntity p_entity, List<Dict> p_dts, List<Row> p_childRows)
        {
            foreach (var child in _model.Children)
            {
                var cRows = child.PropInfo.GetValue(p_entity);
                if (cRows != null)
                {
                    // 生成泛型方法
                    var ls = _getBatchSaveSql.MakeGenericMethod(child.Type).Invoke(child.Schema, new object[] { cRows }) as List<Dict>;
                    if (ls != null && ls.Count > 0)
                    {
                        p_childRows.AddRange((cRows as IList).Cast<Row>());
                        p_dts.AddRange(ls);
                    }
                }
            }
        }

        /// <summary>
        /// 批量执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>true 成功</returns>
        async Task<bool> BatchExec(List<Dict> p_dts)
        {
            if (p_dts == null || p_dts.Count == 0)
                return false;

            foreach (Dict dt in p_dts)
            {
                string sql = (string)dt["text"];
                if (dt["params"] is List<Dict> ls)
                {
                    foreach (var par in ls)
                    {
                        await _.Db.Exec(sql, par);
                    }
                }
                else if (dt["params"] is Dict par)
                {
                    await _.Db.Exec(sql, par);
                }
            }
            return true;
        }

        /// <summary>
        /// 保存实体时收集待发布的领域事件
        /// </summary>
        /// <param name="p_entity"></param>
        void GatherSaveEvents(TEntity p_entity)
        {
            var events = p_entity.GetDomainEvents();
            if (events != null)
                _.AddDomainEvents(events);

            if (_cudEvent != CudEvent.None)
            {
                if (p_entity.IsAdded)
                {
                    if ((_cudEvent & CudEvent.LocalInsert) == CudEvent.LocalInsert)
                        _.AddDomainEvent(new DomainEvent(false, new InsertEvent<TEntity> { Entity = p_entity }));
                    if ((_cudEvent & CudEvent.RemoteInsert) == CudEvent.RemoteInsert)
                        _.AddDomainEvent(new DomainEvent(true, new InsertEvent<TEntity> { Entity = p_entity }));
                }
                else if (p_entity.IsChanged)
                {
                    if ((_cudEvent & CudEvent.LocalUpdate) == CudEvent.LocalUpdate)
                        _.AddDomainEvent(new DomainEvent(false, new UpdateEvent<TEntity> { Entity = p_entity }));
                    if ((_cudEvent & CudEvent.RemoteUpdate) == CudEvent.RemoteUpdate)
                        _.AddDomainEvent(new DomainEvent(true, new UpdateEvent<TEntity> { Entity = p_entity }));
                }
            }
        }

        /// <summary>
        /// 删除实体时收集待发布的领域事件
        /// </summary>
        /// <param name="p_entity"></param>
        void GatherDelEvents(TEntity p_entity)
        {
            var events = p_entity.GetDomainEvents();
            if (events != null)
                _.AddDomainEvents(events);

            if (_cudEvent != CudEvent.None)
            {
                if ((_cudEvent & CudEvent.LocalDelete) == CudEvent.LocalDelete)
                    _.AddDomainEvent(new DomainEvent(false, new DeleteEvent<TEntity> { Entity = p_entity }));
                if ((_cudEvent & CudEvent.RemoteDelete) == CudEvent.RemoteDelete)
                    _.AddDomainEvent(new DomainEvent(true, new DeleteEvent<TEntity> { Entity = p_entity }));
            }
        }
        #endregion
    }
}
