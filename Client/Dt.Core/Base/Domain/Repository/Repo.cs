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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端数据仓库，和服务端Repo方法基本相同
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
            return new UnaryRpc(
                GetModel(typeof(TEntity)).Svc,
                "Da.Query",
                p_keyOrSql,
                p_params
            ).Call<Table<TEntity>>();
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
                GetModel(typeof(TEntity)).Svc,
                "Da.GetPage",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            ).Call<Table<TEntity>>();
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
            return new UnaryRpc(
                GetModel(typeof(TEntity)).Svc,
                "Da.GetRow",
                p_keyOrSql,
                p_params
                ).Call<TEntity>();
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
            return Get<TEntity>(GetModel(typeof(TEntity)).Schema.SqlSelect, new { id = p_id });
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
            return Get<TEntity>(GetModel(typeof(TEntity)).Schema.SqlSelect, new { id = p_id });
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
                    AtKit.Warn(_unchangedMsg);
                return false;
            }

            var model = GetModel(typeof(TEntity));

            // 保存前外部校验，不合格在外部抛出异常
            if (model.OnSaving != null)
            {
                try
                {
                    if (model.OnSaving.ReturnType == typeof(Task))
                        await (Task)model.OnSaving.Invoke(p_entity, null);
                    else
                        model.OnSaving.Invoke(p_entity, null);
                }
                catch (Exception ex)
                {
                    if (p_isNotify)
                    {
                        if (ex.InnerException is KnownException kex)
                            AtKit.Warn(kex.Message);
                        else
                            AtKit.Warn(ex.Message);
                    }
                    return false;
                }
            }

            Dict dt = model.Schema.GetSaveSql(p_entity);
            int cnt = await Exec(model.Svc, (string)dt["text"], (Dict)dt["params"]);
            if (cnt > 0)
            {
                p_entity.AcceptChanges();
                if (p_isNotify)
                    AtKit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                AtKit.Warn("保存失败！");
            return false;
        }

        /// <summary>
        /// 批量保存实体数据，根据实体状态执行增改
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entities">待保存</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>true 保存成功</returns>
        public static async Task<bool> BatchSave<TEntity>(IList<TEntity> p_entities, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entities == null || p_entities.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Warn(_unchangedMsg);
                return false;
            }

            var model = GetModel(typeof(TEntity));

            // 保存前外部校验，不合格在外部抛出异常
            if (model.OnSaving != null)
            {
                foreach (var item in p_entities)
                {
                    try
                    {
                        if (model.OnSaving.ReturnType == typeof(Task))
                            await (Task)model.OnSaving.Invoke(item, null);
                        else
                            model.OnSaving.Invoke(item, null);
                    }
                    catch (Exception ex)
                    {
                        if (p_isNotify)
                        {
                            if (ex.InnerException is KnownException kex)
                                AtKit.Warn(kex.Message);
                            else
                                AtKit.Warn(ex.Message);
                        }
                        return false;
                    }
                }
            }

            List<Dict> dts = model.Schema.GetBatchSaveSql(p_entities);

            // 不需要保存
            if (dts == null || dts.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Msg(_unchangedMsg);
                return true;
            }

            bool suc = await BatchExec(model.Svc, dts);
            if (suc)
            {
                if (p_entities is Table tbl)
                {
                    tbl.AcceptChanges();
                }
                else
                {
                    foreach (var row in p_entities)
                    {
                        row.AcceptChanges();
                    }
                }

                if (p_isNotify)
                    AtKit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                AtKit.Warn("保存失败！");
            return false;
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
            if (p_entity == null)
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            var model = GetModel(typeof(TEntity));

            // 删除前外部校验，不合格在外部抛出异常
            if (model.OnDeleting != null)
            {
                try
                {
                    if (model.OnDeleting.ReturnType == typeof(Task))
                        await (Task)model.OnDeleting.Invoke(p_entity, null);
                    else
                        model.OnDeleting.Invoke(p_entity, null);
                }
                catch (Exception ex)
                {
                    if (p_isNotify)
                    {
                        if (ex.InnerException is KnownException kex)
                            AtKit.Warn(kex.Message);
                        else
                            AtKit.Warn(ex.Message);
                    }
                    return false;
                }
            }

            Dict dt = model.Schema.GetDeleteSql(new List<Row> { p_entity });
            bool suc = await Exec(model.Svc, (string)dt["text"], ((List<Dict>)dt["params"])[0]) == 1;
            if (p_isNotify)
            {
                if (suc)
                    AtKit.Msg("删除成功！");
                else
                    AtKit.Warn("删除失败！");
            }
            return suc;
        }

        /// <summary>
        /// 批量删除实体，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entities">实体列表</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> BatchDelete<TEntity>(IList<TEntity> p_entities, bool p_isNotify = true)
            where TEntity : Entity
        {
            if (p_entities == null || p_entities.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            var model = GetModel(typeof(TEntity));

            // 删除前外部校验，不合格在外部抛出异常
            if (model.OnDeleting != null)
            {
                foreach (var item in p_entities)
                {
                    try
                    {
                        if (model.OnDeleting.ReturnType == typeof(Task))
                            await (Task)model.OnDeleting.Invoke(item, null);
                        else
                            model.OnDeleting.Invoke(item, null);
                    }
                    catch (Exception ex)
                    {
                        if (p_isNotify)
                        {
                            if (ex.InnerException is KnownException kex)
                                AtKit.Warn(kex.Message);
                            else
                                AtKit.Warn(ex.Message);
                        }
                        return false;
                    }
                }
            }

            Dict dt = model.Schema.GetDeleteSql(p_entities);
            bool suc = await BatchExec(model.Svc, new List<Dict> { dt });
            if (p_isNotify)
            {
                if (suc)
                    AtKit.Msg("删除成功！");
                else
                    AtKit.Warn("删除失败！");
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
            var model = GetModel(typeof(TEntity));
            bool suc = await Exec(model.Svc, model.Schema.SqlDelete, new { id = p_id }) == 1;
            if (p_isNotify)
                AtKit.Msg(suc ? "删除成功！" : "删除失败！");
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
            var model = GetModel(typeof(TEntity));
            bool suc = await Exec(model.Svc, model.Schema.SqlDelete, new { id = p_id }) == 1;
            if (p_isNotify)
                AtKit.Msg(suc ? "删除成功！" : "删除失败！");
            return suc;
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

            return BatchSave<TEntity>(tbl, false);
        }
        #endregion

        #region 内部方法
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

        /// <summary>
        /// 一个事务内执行Sql语句，返回影响的行数，p_params为IEnumerable时执行批量操作
        /// </summary>
        /// <param name="p_svc">服务地址</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，为IEnumerable时执行批量操作</param>
        /// <returns>返回执行后影响的行数</returns>
        static Task<int> Exec(string p_svc, string p_keyOrSql, object p_params = null)
        {
            Throw.IfNullOrEmpty(p_keyOrSql);
            return new UnaryRpc(
                p_svc,
                "Da.Exec",
                p_keyOrSql,
                p_params
            ).Call<int>();
        }

        /// <summary>
        /// 一个事务内执行多个Sql
        /// </summary>
        /// <param name="p_svc">服务地址</param>
        /// <param name="p_dts">参数列表，每个Dict中包含两个键：text,params，text为sql语句params类型为Dict或List{Dict}</param>
        /// <returns>true 成功</returns>
        static Task<bool> BatchExec(string p_svc, List<Dict> p_dts)
        {
            Throw.If(p_dts == null || p_dts.Count == 0);
            return new UnaryRpc(
                p_svc,
                "Da.BatchExec",
                p_dts
            ).Call<bool>();
        }
        #endregion
    }
}
