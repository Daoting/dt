#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端数据提供者，和服务端DataProvider方法基本相同
    /// </summary>
    /// <typeparam name="TSvc">类型名称当服务名用</typeparam>
    public abstract class DataProvider<TSvc>
    {
        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Task<Table> Query(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.Query",
                p_keyOrSql,
                p_params
            ).Call<Table>();
        }

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
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.Query",
                p_keyOrSql,
                p_params
            ).Call<Table<TEntity>>();
        }

        /// <summary>
        /// 返回所有实体列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns></returns>
        public static Task<Table<TEntity>> GetAll<TEntity>()
            where TEntity : Entity
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.Query",
                EntitySchema.Get(typeof(TEntity)).Schema.GetSelectAllSql(),
                null
            ).Call<Table<TEntity>>();
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据</returns>
        public static Task<Table> GetPage(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.GetPage",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            ).Call<Table>();
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public static Task<Table<TEntity>> GetPage<TEntity>(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.GetPage",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            ).Call<Table<TEntity>>();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一个单元格数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一个单元格数据</returns>
        public static Task<T> GetScalar<T>(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.GetScalar",
                p_keyOrSql,
                p_params
            ).Call<T>();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public static Task<Row> First(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.First",
                p_keyOrSql,
                p_params
            ).Call<Row>();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回第一个实体对象，实体属性由Sql决定，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> First<TEntity>(string p_keyOrSql, object p_params = null)
            where TEntity : Entity
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.First",
                p_keyOrSql,
                p_params
                ).Call<TEntity>();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，返回符合条件的第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T">第一列数据类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一列数据的泛型列表</returns>
        public static Task<List<T>> FirstCol<T>(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.FirstCol",
                typeof(T).FullName,
                p_keyOrSql,
                p_params
                ).Call<List<T>>();
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键id，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(string p_id)
            where TEntity : Entity
        {
            return First<TEntity>(EntitySchema.Get(typeof(TEntity)).Schema.SqlSelect, new { id = p_id });
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键id，不存在时返回null
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByID<TEntity>(long p_id)
            where TEntity : Entity
        {
            return First<TEntity>(EntitySchema.Get(typeof(TEntity)).Schema.SqlSelect, new { id = p_id });
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.NewID"
            ).Call<long>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_seqName">序列名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_seqName)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.NewSeq",
                p_seqName
            ).Call<int>();
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存实体数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entity == null
                || (!p_entity.IsAdded && !p_entity.IsChanged))
            {
                if (p_isNotify)
                    Kit.Warn(_unchangedMsg);
                return false;
            }

            var model = EntitySchema.Get(typeof(TEntity));
            if (model.OnSaving != null)
            {
                // 保存前外部校验，不合格在外部抛出异常
                if (!await OnSaving(model, p_entity))
                    return false;
            }

            Dict dt = model.Schema.GetSaveSql(p_entity);
            int cnt = await Exec((string)dt["text"], (Dict)dt["params"]);
            if (cnt > 0)
            {
                p_entity.AcceptChanges();
                if (p_isNotify)
                    Kit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                Kit.Warn("保存失败！");
            return false;
        }

        /// <summary>
        /// 一个事务内批量保存实体数据，根据实体状态执行增改，Table&lt;Entity&gt;支持删除，列表类型支持：
        /// <para>Table&lt;Entity&gt;，单表增删改</para>
        /// <para>List&lt;Entity&gt;，单表增改</para>
        /// <para>IList，多表增删改，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待保存列表</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>true 保存成功</returns>
        public static Task<bool> BatchSave(IList p_list, bool p_isNotify = true)
        {
            if (p_list == null || p_list.Count == 0)
            {
                if (p_isNotify)
                    Kit.Warn(_unchangedMsg);
                return Task.FromResult(false);
            }

            Type tp = p_list.GetType();
            if (tp.IsGenericType
                && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
            {
                return BatchSaveSameType(p_list, p_isNotify);
            }
            return BatchSaveMultiTypes(p_list, p_isNotify);
        }

        /// <summary>
        /// 将实体数据传输到服务端，由服务端DataProvider保存实体，用于需要触发领域事件或同步缓存的情况
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> SaveBySvc<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entity == null
                || (!p_entity.IsAdded && !p_entity.IsChanged))
            {
                if (p_isNotify)
                    Kit.Warn(_unchangedMsg);
                return false;
            }

            var model = EntitySchema.Get(typeof(TEntity));
            bool suc = await new UnaryRpc(
                typeof(TSvc).Name,
                "EntityAccess.Save",
                p_entity,
                model.Schema.Name
            ).Call<bool>();

            if (suc)
            {
                p_entity.AcceptChanges();
                if (p_isNotify)
                    Kit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                Kit.Warn("保存失败！");
            return false;
        }

        /// <summary>
        /// 单表增删改，列表中的实体类型相同
        /// </summary>
        /// <param name="p_list"></param>
        /// <param name="p_isNotify"></param>
        /// <returns></returns>
        static async Task<bool> BatchSaveSameType(IList p_list, bool p_isNotify)
        {
            var model = EntitySchema.Get(p_list.GetType().GetGenericArguments()[0]);
            if (model.OnSaving != null)
            {
                foreach (var item in p_list)
                {
                    if (item != null && !await OnSaving(model, item))
                        return false;
                }
            }
            var dts = model.Schema.GetBatchSaveSql(p_list);

            // 不需要保存
            if (dts == null || dts.Count == 0)
            {
                if (p_isNotify)
                    Kit.Msg(_unchangedMsg);
                return false;
            }

            if (await BatchExec(dts) > 0)
            {
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

                if (p_isNotify)
                    Kit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                Kit.Warn("保存失败！");
            return false;
        }

        /// <summary>
        /// 多表增删改
        /// </summary>
        /// <param name="p_list"></param>
        /// <param name="p_isNotify"></param>
        /// <returns></returns>
        static async Task<bool> BatchSaveMultiTypes(IList p_list, bool p_isNotify)
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
                        {
                            if (!await OnSaving(model, entity))
                                return false;
                        }

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
                                if (!await OnSaving(model, ci))
                                    return false;
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
            {
                if (p_isNotify)
                    Kit.Msg(_unchangedMsg);
                return true;
            }

            bool suc = await BatchExec(dts) > 0;
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

                if (p_isNotify)
                    Kit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                Kit.Warn("保存失败！");
            return false;
        }

        /// <summary>
        /// 保存前外部校验，不合格在外部抛出异常
        /// </summary>
        /// <param name="p_model"></param>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        static async Task<bool> OnSaving(EntitySchema p_model, object p_entity)
        {
            try
            {
                await (Task)p_model.OnSaving.Invoke(p_entity, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is KnownException kex)
                    Kit.Warn(kex.Message);
                else
                    Kit.Warn(ex.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的行</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entity == null || p_entity.IsAdded)
            {
                if (p_isNotify)
                    Kit.Warn(_saveError);
                return false;
            }

            var model = EntitySchema.Get(typeof(TEntity));
            if (model.OnDeleting != null)
            {
                if (!await OnDeleting(model, p_entity))
                    return false;
            }

            Dict dt = model.Schema.GetDeleteSql(new List<Row> { p_entity });
            bool suc = await Exec((string)dt["text"], ((List<Dict>)dt["params"])[0]) == 1;
            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
            return suc;
        }

        /// <summary>
        /// 批量删除实体，单表或多表，列表类型支持：
        /// <para>Table&lt;Entity&gt;，单表删除</para>
        /// <para>List&lt;Entity&gt;，单表删除</para>
        /// <para>IList，多表删除，成员可为Entity,List&lt;Entity&gt;,Table&lt;Entity&gt;的混合</para>
        /// </summary>
        /// <param name="p_list">待删除实体列表</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> BatchDelete(IList p_list, bool p_isNotify = true)
        {
            if (p_list == null || p_list.Count == 0)
            {
                if (p_isNotify)
                    Kit.Warn(_saveError);
                return Task.FromResult(false);
            }

            Type tp = p_list.GetType();
            if (tp.IsGenericType
                && tp.GetGenericArguments()[0].IsSubclassOf(typeof(Entity)))
            {
                return BatchDeleteSameType(p_list, p_isNotify);
            }
            return BatchDeleteMultiTypes(p_list, p_isNotify);
        }

        /// <summary>
        /// 将实体数据传输到服务端，由服务端DataProvider删除实体，用于需要触发领域事件或同步缓存的情况
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的行</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> DeleteBySvc<TEntity>(TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entity == null || p_entity.IsAdded)
            {
                if (p_isNotify)
                    Kit.Warn(_saveError);
                return false;
            }

            var model = EntitySchema.Get(typeof(TEntity));
            bool suc = await new UnaryRpc(
                typeof(TSvc).Name,
                "EntityAccess.Delete",
                p_entity,
                model.Schema.Name
            ).Call<bool>();

            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
            return suc;
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键id，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> DelByID<TEntity>(string p_id, bool p_isNotify = true)
        {
            var model = EntitySchema.Get(typeof(TEntity));
            bool suc = await Exec(model.Schema.SqlDelete, new { id = p_id }) == 1;
            if (p_isNotify)
                Kit.Msg(suc ? "删除成功！" : "删除失败！");
            return suc;
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键id，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> DelByID<TEntity>(long p_id, bool p_isNotify = true)
        {
            var model = EntitySchema.Get(typeof(TEntity));
            bool suc = await Exec(model.Schema.SqlDelete, new { id = p_id }) == 1;
            if (p_isNotify)
                Kit.Msg(suc ? "删除成功！" : "删除失败！");
            return suc;
        }

        /// <summary>
        /// 单表批量删除
        /// </summary>
        /// <param name="p_list"></param>
        /// <param name="p_isNotify"></param>
        /// <returns></returns>
        static async Task<bool> BatchDeleteSameType(IList p_list, bool p_isNotify)
        {
            var model = EntitySchema.Get(p_list.GetType().GetGenericArguments()[0]);
            if (model.OnDeleting != null)
            {
                foreach (var item in p_list)
                {
                    if (item != null && !await OnDeleting(model, item))
                        return false;
                }
            }

            Dict dt = model.Schema.GetDeleteSql(p_list);
            bool suc = await BatchExec(new List<Dict> { dt }) > 0;
            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
            return suc;
        }

        /// <summary>
        /// 多表批量删除
        /// </summary>
        /// <param name="p_list"></param>
        /// <param name="p_isNotify"></param>
        /// <returns></returns>
        static async Task<bool> BatchDeleteMultiTypes(IList p_list, bool p_isNotify)
        {
            var dts = new List<Dict>();
            foreach (var item in p_list)
            {
                if (item is Entity entity)
                {
                    var model = EntitySchema.Get(item.GetType());
                    if (model.OnDeleting != null)
                    {
                        if (!await OnDeleting(model, entity))
                            return false;
                    }

                    dts.Add(model.Schema.GetDeleteSql(new List<Row> { entity }));
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
                                if (!await OnDeleting(model, ci))
                                    return false;
                            }
                        }

                        dts.Add(model.Schema.GetDeleteSql(clist));
                    }
                }
            }

            // 不需要删除
            if (dts == null || dts.Count == 0)
            {
                if (p_isNotify)
                    Kit.Msg(_unchangedMsg);
                return true;
            }

            bool suc = await BatchExec(dts) > 0;
            if (p_isNotify)
            {
                if (suc)
                    Kit.Msg("删除成功！");
                else
                    Kit.Warn("删除失败！");
            }
            return suc;
        }

        /// <summary>
        /// 删除前外部校验，不合格在外部抛出异常
        /// </summary>
        /// <param name="p_model"></param>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        static async Task<bool> OnDeleting(EntitySchema p_model, object p_entity)
        {
            try
            {
                await (Task)p_model.OnDeleting.Invoke(p_entity, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is KnownException kex)
                    Kit.Warn(kex.Message);
                else
                    Kit.Warn(ex.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> Exec(string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.Exec",
                p_keyOrSql,
                p_params
            ).Call<int>();
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>返回执行后影响的行数</returns>
        public static Task<int> BatchExec(List<Dict> p_dts)
        {
            return new UnaryRpc(
                typeof(TSvc).Name,
                "Da.BatchExec",
                p_dts
            ).Call<int>();
        }

        /// <summary>
        /// 互换两行的显示位置，确保包含 id,dispidx 列
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_src"></param>
        /// <param name="p_tgt"></param>
        /// <returns>true 互换成功</returns>
        public static Task<bool> ExchangeDispidx<TEntity>(TEntity p_src, TEntity p_tgt)
            where TEntity : Entity
        {
            var tbl = new Table<TEntity> { { "id", typeof(long) }, { "dispidx", typeof(int) } };

            var save = tbl.AddRow(new { id = p_src.ID });
            save.AcceptChanges();
            save["dispidx"] = p_tgt["dispidx"];

            save = tbl.AddRow(new { id = p_tgt.ID });
            save.AcceptChanges();
            save["dispidx"] = p_src["dispidx"];

            return BatchSave(tbl, false);
        }
        #endregion

        #region 成员变量
        const string _unchangedMsg = "没有需要保存的数据！";
        const string _saveError = "数据源不可为空！";

        #endregion
    }
}
