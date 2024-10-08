﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体写入器，对所有实体的的持久化做统一管理，解决领域模型存储和变更工作
    /// <para>写入器在一个事务内批量处理所有待保存、待删除的实体数据，失败时回滚</para>
    /// <para>无论提交成功失败都清空状态，准备下次提交！</para>
    /// </summary>
    class EntityWriter : IEntityWriter
    {
        #region 成员变量
        static MethodInfo _saveGenericTbl;
        static MethodInfo _deleteGenericTbl;
        readonly List<UnitItem> _items = new List<UnitItem>();
        readonly IDataAccess _da;
        // 当前正在保存的Table
        readonly HashSet<Table> _curTbls = new HashSet<Table>();
        #endregion

        #region 构造方法
        public EntityWriter(IDataAccess p_da)
        {
            _da = p_da;
        }
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
        /// <para>删除行通过Table的 IsLockedCollection 判断获取</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_tbl">实体表</param>
        /// <returns></returns>
        public async Task Save<TEntity>(Table<TEntity> p_tbl)
            where TEntity : Entity
        {
            if (p_tbl == null)
                return;

            if (p_tbl.Count > 0)
            {
                var ls = (from en in (IList<TEntity>)p_tbl
                          where en.IsAdded || en.IsChanged
                          select en).ToList();
                if (ls.Count > 0)
                {
                    await Save(ls);
                    _curTbls.Add(p_tbl);
                }
            }

            if (p_tbl.IsLockedCollection)
            {
                // 包含删除行的情况
                var ls = (from row in p_tbl.DeletedRows
                          let en = row as TEntity
                          where en != null && !en.IsAdded
                          select en).ToList();
                if (ls.Count > 0)
                {
                    await Delete(ls);
                    _curTbls.Add(p_tbl);
                }
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
            if (p_entity.IsAdded || p_entity.IsChanged)
                await Save(new List<TEntity> { p_entity });

            var model = await EntitySchema.Get(p_entity.GetType());
            var children = await model.GetChildren();
            if (children.Count == 0)
                return;

            // 泛型方法：Save<TEntity>(Table<TEntity> p_tbl)
            var save = GetSaveGenericTblMethod();

            foreach (var child in children)
            {
                // 获取子实体列表
                var tbl = child.PropInfo.GetValue(p_entity);

                // 构造泛型方法
                var mi = save.MakeGenericMethod(child.Type);

                // 调用Save<>方法
                var task = (Task)mi.Invoke(this, new object[1] { tbl });
                await task;
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

            var first = p_list.FirstOrDefault();
            if (first == null)
                return;

            // 虚拟实体特殊处理
            if (Entity.IsVirEntity(first.GetType()))
            {
                await SaveVirEntity(p_list);
                return;
            }

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

            var model = await EntitySchema.Get(first.GetType());
            if (IsValidSvc(model))
            {
                var dts = model.Schema.GetSaveSql(ls);
                if (dts != null && dts.Count > 0)
                {
                    var ui = new UnitItem(model, ls, dts);
                    _items.Add(ui);
                }
            }
        }

        /// <summary>
        /// 为方便无法以泛型 Table`1 使用的情况，内部调用：Save`1(Table`1 p_tbl)，
        /// <para>添加Table中新增、修改、删除的待保存实体，最后由Commit统一提交</para>
        /// <para>删除行通过Table的 IsLockedCollection 判断获取</para>
        /// </summary>
        /// <param name="p_tbl">实体表</param>
        /// <returns></returns>
        public async Task Save(Table p_tbl)
        {
            if (p_tbl == null || !p_tbl.GetType().IsGenericType)
                return;

            // 泛型方法：Save<TEntity>(Table<TEntity> p_tbl)
            var save = GetSaveGenericTblMethod();

            // 构造泛型方法
            var mi = save.MakeGenericMethod(p_tbl.GetType().GenericTypeArguments[0]);

            // 调用Save<>方法
            var task = (Task)mi.Invoke(this, new object[1] { p_tbl });
            await task;
        }

        /// <summary>
        /// 保存虚拟实体，批量保存时未将相同类型的实体形成列表统一生成sql！
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_list"></param>
        /// <returns></returns>
        async Task SaveVirEntity<TEntity>(IEnumerable<TEntity> p_list)
            where TEntity : Entity
        {
            // 不再判断null和虚拟实体类型

            foreach (var item in p_list)
            {
                // 虚拟实体的增加、修改状态仍然有效
                if (item == null || (!item.IsAdded && !item.IsChanged))
                    continue;

                // 虚拟实体内部包含的实体对象
                foreach (var obj in ((IVirEntity)item).GetEntities())
                {
                    if (obj.GetSavingHook() is Func<Task> hook)
                    {
                        // 保存前外部校验，校验不通过抛出异常
                        await hook();
                    }

                    var model = await EntitySchema.Get(obj.GetType());
                    if (IsValidSvc(model))
                    {
                        // 单个处理
                        var ls = new List<Entity> { obj };
                        // 可能存在即使有修改标志，也无需update的情况！
                        var dts = model.Schema.GetSaveSql(ls);
                        if (dts != null && dts.Count > 0)
                        {
                            var ui = new UnitItem(model, ls, dts);
                            _items.Add(ui);
                        }
                    }
                }
            }
        }

        static MethodInfo GetSaveGenericTblMethod()
        {
            if (_saveGenericTbl == null)
            {
                // 构造方法里的泛型参数 TEntity
                Type tpEntity = Type.MakeGenericMethodParameter(0);
                // 构造泛型Table<>
                var tpTbl = typeof(Table<>).MakeGenericType(tpEntity);
                // 泛型方法：Save<TEntity>(Table<TEntity> p_tbl)
                _saveGenericTbl = typeof(EntityWriter).GetMethod("Save", 1, new Type[1] { tpTbl });
            }
            return _saveGenericTbl;
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
        /// <param name="p_tbl">待删除实体列表</param>
        /// <returns></returns>
        public Task Delete<TEntity>(Table<TEntity> p_tbl)
            where TEntity : Entity
        {
            return Delete((IList<TEntity>)p_tbl);
        }

        /// <summary>
        /// 为方便无法以泛型 Table`1 使用的情况，内部调用：Delete`1(Table`1 p_tbl)
        /// <para>批量添加待删除的实体，最后由Commit统一提交</para>
        /// </summary>
        /// <param name="p_tbl">待删除实体列表</param>
        /// <returns></returns>
        public async Task Delete(Table p_tbl)
        {
            if (p_tbl == null
                || p_tbl.Count == 0
                || !p_tbl.GetType().IsGenericType)
                return;
            
            // 泛型方法：Delete<TEntity>(Table<TEntity> p_tbl)
            var delete = GetDeleteGenericTblMethod();

            // 构造泛型方法
            var mi = delete.MakeGenericMethod(p_tbl.GetType().GenericTypeArguments[0]);

            // 调用Delete<>方法
            var task = (Task)mi.Invoke(this, new object[1] { p_tbl });
            await task;
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

            // 实际类型
            Type tp = p_list[0].GetType();

            // 虚拟实体特殊处理
            if (Entity.IsVirEntity(tp))
            {
                await DeleteVirEntity(p_list);
                return;
            }

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

            var model = await EntitySchema.Get(tp);
            if (IsValidSvc(model))
            {
                Dict dt = model.Schema.GetDeleteSql(p_list);
                var ui = new UnitItem(model, (IList)p_list, new List<Dict> { dt });
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
        public async Task DelByID<TEntity>(object p_id)
            where TEntity : Entity
        {
            if (p_id == null || string.IsNullOrEmpty(p_id.ToString()))
                return;

            var entity = await EntityX<TEntity>.GetByID(p_id);
            if (entity != null)
                await Delete(new List<TEntity> { entity });
        }

        /// <summary>
        /// 先根据主键批量获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_ids">主键列表</param>
        /// <returns></returns>
        public async Task DelByIDs<TEntity>(IList p_ids)
            where TEntity : Entity
        {
            if (p_ids == null || p_ids.Count == 0)
                return;

            var ls = new List<TEntity>();
            foreach (var id in p_ids)
            {
                if (id == null || string.IsNullOrEmpty(id.ToString()))
                    continue;

                var entity = await EntityX<TEntity>.GetByID(id);
                if (entity != null)
                    ls.Add(entity);
            }

            if (ls.Count > 0)
                await Delete(ls);
        }

        /// <summary>
        /// 删除虚拟实体，批量删除时未将相同类型的实体形成列表统一生成sql！
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_list"></param>
        /// <returns></returns>
        async Task DeleteVirEntity<TEntity>(IEnumerable<TEntity> p_list)
            where TEntity : Entity
        {
            // 不再判断null和虚拟实体类型

            foreach (var item in p_list.Cast<IVirEntity>())
            {
                if (item == null)
                    continue;

                // 虚拟实体内部包含的实体对象
                foreach (var obj in item.GetEntities())
                {
                    if (obj.GetDeletingHook() is Func<Task> hook)
                    {
                        // 删除前外部校验，校验不通过抛出异常
                        await hook();
                    }

                    var model = await EntitySchema.Get(obj.GetType());
                    if (IsValidSvc(model))
                    {
                        // 单个处理
                        var ls = new List<Entity> { obj };
                        Dict dt = model.Schema.GetDeleteSql((IList<Entity>)ls);
                        var ui = new UnitItem(model, ls, new List<Dict> { dt });
                        ui.IsDelete = true;
                        _items.Add(ui);
                    }
                }
            }
        }

        static MethodInfo GetDeleteGenericTblMethod()
        {
            if (_deleteGenericTbl == null)
            {
                // 构造方法里的泛型参数 TEntity
                Type tpEntity = Type.MakeGenericMethodParameter(0);
                // 构造泛型Table<>
                var tpTbl = typeof(Table<>).MakeGenericType(tpEntity);
                // 泛型方法：Delete<TEntity>(Table<TEntity> p_tbl)
                _deleteGenericTbl = typeof(EntityWriter).GetMethod("Delete", 1, new Type[1] { tpTbl });
            }
            return _deleteGenericTbl;
        }
        #endregion

        #region 提交
        /// <summary>
        /// 是否有数据需要提交
        /// </summary>
        public bool NeedCommit => _items.Count > 0;

        /// <summary>
        /// 一个事务内批量处理所有待保存、待删除的实体数据，失败时回滚
        /// <para>无论提交成功失败都清空状态，准备下次提交！</para>
        /// <para>处理成功后，对于每个实体：</para>
        /// <para>1. 调用保存后或删除后回调</para>
        /// <para>2. 对于新增或修改的实体进行状态复位</para>
        /// <para>3. 若存在领域事件，则发布事件</para>
        /// </summary>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <param name="p_notifyIfNotNeed">没有需要保存的数据时是否提示，null时由p_isNotify控制，客户端有效</param>
        /// <returns>是否成功</returns>
        public async Task<bool> Commit(bool p_isNotify = true, bool? p_notifyIfNotNeed = null)
        {
            if (_items.Count == 0)
            {
#if !SERVER
                if ((p_notifyIfNotNeed == null && p_isNotify)
                    || (p_notifyIfNotNeed.HasValue && p_notifyIfNotNeed.Value))
                {
                    Kit.Warn("没有需要保存的数据！");
                }
#endif
                return true;
            }

            var dts = new List<Dict>();
            _items.ForEach(item => dts.AddRange(item.Exec));

            bool suc;
            if (dts.Count == 1 && dts[0]["params"] is Dict par)
            {
                suc = await _da.Exec((string)dts[0]["text"], par) > 0;
            }
            else
            {
                suc = await _da.BatchExec(dts) > 0;
            }

#if !SERVER
            if (p_isNotify)
            {
                bool isSave = (from item in _items
                               where !item.IsDelete
                               select item).Any();
                if (suc)
                {
                    Kit.Msg(isSave ? "保存成功！" : "删除成功！");
                }
                else
                {
                    Kit.Warn(isSave ? "保存失败！" : "删除失败！");
                }
            }
#endif
            if (!suc)
            {
                _items.Clear();
                return false;
            }

            // 成功后：调用回调、状态复位、发布事件
            foreach (var item in _items)
            {
                await item.OnCommited();
            }

            // 清空后可复用
            _items.Clear();

            // 将当前正在保存的Table状态复位
            if (_curTbls.Count > 0)
            {
                foreach (var tbl in  _curTbls)
                {
                    if (tbl.IsChanged || tbl.IsDirty)
                        tbl.AcceptChanges();
                }
                _curTbls.Clear();
            }
            return true;
        }
        #endregion

        #region 内部方法
#if SERVER
        bool IsValidSvc(EntitySchema p_model) => true;
#else
        /// <summary>
        /// 确保所有实体使用同一服务
        /// </summary>
        /// <param name="p_model"></param>
        /// <returns></returns>
        bool IsValidSvc(EntitySchema p_model)
        {
            if (p_model.AccessInfo != _da.AccessInfo)
            {
                Throw.Msg($"实体类型[{p_model.EntityType.Name}]无法在[{_da.AccessInfo.Type}.{_da.AccessInfo.Name}]中存储，EntityWriter中无法跨服务或跨库存储数据！");
                return false;
            }
            return true;
        }
#endif
        #endregion
    }
}
