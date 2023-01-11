#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-10 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 工作单元，解决领域模型存储和变更工作，对所有仓储的的持久化做统一管理
    /// </summary>
    public class UnitOfWork
    {
        #region 成员变量
        readonly List<UnitItem> _items = new List<UnitItem>();
        string _svcName;
        bool _commited;
        #endregion

        #region 保存
        /// <summary>
        /// 添加待保存的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <returns></returns>
        public Task Save<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            if (p_entity != null
                && (p_entity.IsAdded || p_entity.IsChanged))
            {
                return Save(new List<TEntity> { p_entity });
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 添加Table中新增、修改、删除的待保存实体，最后由Commit统一提交
        /// <para>删除行通过Table的ExistDeleted DeletedRows判断获取</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_tbl">实体表</param>
        /// <returns></returns>
        public async Task Save<TEntity>(Table<TEntity> p_tbl)
            where TEntity : Entity
        {
            if (p_tbl == null || p_tbl.Count == 0)
                return;

            var ls = from en in (IList<TEntity>)p_tbl
                     where en.IsAdded || en.IsChanged
                     select en;
            await Save(ls);

            if (p_tbl.ExistDeleted)
            {
                // 包含删除行的情况
                await Delete(p_tbl.DeletedRows.Cast<TEntity>().ToList());
            }
        }

        /// <summary>
        /// 批量添加一对多的父实体和所有子实体中新增、修改、删除的待保存实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        public async Task SaveWithChild<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            if (p_entity == null)
                return;

            if (p_entity.IsAdded || p_entity.IsChanged)
                await Save(new List<TEntity> { p_entity });

            var ls = p_entity.GetChildren();
            if (ls != null && ls.Count > 0)
            {
                foreach (var child in ls)
                {
                    if (child != null && child.Count > 0)
                        await Save(child);
                }
            }
        }

        /// <summary>
        /// 添加列表中新增、修改的待保存实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_list">实体列表</param>
        /// <returns></returns>
        public async Task Save<TEntity>(IEnumerable<TEntity> p_list)
            where TEntity : Entity
        {
            if (p_list == null)
                return;

            // 提取并校验需要保存的实体
            List<TEntity> ls = new List<TEntity>();
            foreach (var item in p_list)
            {
                if (item == null || (!item.IsAdded && !item.IsChanged))
                    continue;

                ls.Add(item);
                if (item.GetSavingHook() is Func<Task> hook)
                {
                    // 保存前外部校验，校验不通过抛出异常
                    await hook();
                }
            }
            if (ls.Count == 0)
                return;

            var model = EntitySchema.Get(typeof(TEntity));
            if (IsValidSvc(model))
            {
                var dts = model.Schema.GetSaveSql(ls);
                if (dts != null && dts.Count > 0)
                {
                    var ui = new UnitItem(model, (IList<Entity>)ls, dts);
                    _items.Add(ui);
                }
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 添加待删除的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns></returns>
        public Task Delete<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            if (p_entity != null)
                return Delete(new List<TEntity> { p_entity });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量添加待删除的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_list">待删除实体列表</param>
        /// <returns></returns>
        public async Task Delete<TEntity>(IList<TEntity> p_list)
            where TEntity : Entity
        {
            if (p_list == null || p_list.Count == 0)
                return;

            foreach (var item in p_list)
            {
                if (item.GetDeletingHook() is Func<Task> hook)
                {
                    // 删除前外部校验，校验不通过抛出异常
                    await hook();
                }
                else
                {
                    // 第一个实体若不包含Hook，其他也不包含
                    break;
                }
            }

            var model = EntitySchema.Get(typeof(TEntity));
            if (IsValidSvc(model))
            {
                Dict dt = model.Schema.GetDeleteSql(p_list);
                var ui = new UnitItem(model, (IList<Entity>)p_list, new List<Dict> { dt });
                ui.IsDelete = true;
                _items.Add(ui);
            }
        }

        /// <summary>
        /// 先根据主键获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns></returns>
        public async Task DelByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            var entity = await EntityEx.GetByID<TEntity>(p_id);
            if (entity != null)
                await Delete(new List<TEntity> { entity });
        }

        /// <summary>
        /// 先根据主键获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns></returns>
        public async Task DelByID<TEntity>(long p_id)
            where TEntity : Entity
        {
            var entity = await EntityEx.GetByID<TEntity>(p_id);
            if (entity != null)
                await Delete(new List<TEntity> { entity });
        }

        /// <summary>
        /// 先根据主键或唯一索引获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <returns></returns>
        public async Task DelByKey<TEntity>(string p_keyName, string p_keyVal)
            where TEntity : Entity
        {
            var entity = await EntityEx.GetByKey<TEntity>(p_keyName, p_keyVal);
            if (entity != null)
                await Delete(new List<TEntity> { entity });
        }
        #endregion

        #region 提交
        /// <summary>
        /// 一个事务内批量处理所有待保存、待删除的实体数据，失败时回滚，不可重复提交！
        /// <para>处理成功后，对于每个实体：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 对于新增、修改的实体进行状态复位</para>
        /// </summary>
        /// <param name="p_isNotify"></param>
        /// <returns></returns>
        public async Task<bool> Commit(bool p_isNotify = true)
        {
            Throw.If(_commited, "当前工作单元已经提交，不可重复提交！");

            if (_items.Count == 0)
            {
#if !SERVER
                if (p_isNotify)
                    Kit.Warn("没有需要保存的数据！");
#endif
                return false;
            }

            _commited = true;
            var dts = new List<Dict>();
            _items.ForEach(item => dts.AddRange(item.Exec));

            bool suc;
            if (dts.Count == 1 && dts[0]["params"] is Dict par)
            {
                suc = await Exec((string)dts[0]["text"], par) > 0;
            }
            else
            {
                suc = await BatchExec(dts) > 0;
            }

#if !SERVER
            if (p_isNotify)
            {
                Kit.Warn(suc ? "保存成功！" : "保存失败！");
            }
#endif
            if (!suc)
                return false;

            // 数据存储成功的后续处理：发布领域事件、删除服务端缓存、实体状态复位
            _items.ForEach(item => item.OnCommited());
            return true;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 确保所有实体使用同一服务
        /// </summary>
        /// <param name="p_model"></param>
        /// <returns></returns>
        bool IsValidSvc(EntitySchema p_model)
        {
            if (string.IsNullOrEmpty(_svcName))
            {
                _svcName = p_model.SvcName;
            }
            else
            {
                Throw.If(_svcName != p_model.SvcName, "工作单元中无法跨服务进行数据存储！");
            }
            return true;
        }

        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        Task<int> Exec(string p_sql, object p_params = null)
        {
#if SERVER
            return new MySqlAccess().Exec(p_sql, p_params);
#else
            return Kit.Rpc<int>(
                _svcName,
                "Da.Exec",
                p_sql,
                p_params);
#endif
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        Task<int> BatchExec(List<Dict> p_dts)
        {
#if SERVER
            return new Da().BatchExec(p_dts);
#else
            return Kit.Rpc<int>(
                _svcName,
                "Da.BatchExec",
                p_dts);
#endif
        }
        #endregion
    }
}
