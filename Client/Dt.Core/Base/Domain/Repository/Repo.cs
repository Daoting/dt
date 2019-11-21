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
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据仓库
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class Repo<TEntity>
        where TEntity : Entity
    {
        #region 静态内容
        protected static readonly EntitySchema _model;
        // 泛型方法 Call<T>
        static MethodInfo _callMethod = typeof(UnaryRpc).GetMethod("Call");
        // 泛型方法 GetBatchSaveSql
        static MethodInfo _getBatchSaveSql = typeof(TableSchema).GetMethod("GetBatchSaveSql");

        static Repo()
        {
            _model = new EntitySchema(typeof(TEntity));
        }
        #endregion

        #region 成员变量
        const string _unchangedMsg = "没有需要保存的数据！";
        const string _saveError = "数据源不可为空！";
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
            return new UnaryRpc(
                _model.Svc,
                "Da.Query",
                p_keyOrSql,
                p_params
            ).Call<Table<TEntity>>();
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <typeparam name="TRow">实体类型</typeparam>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table<TEntity>> GetPage(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
        {
            return new UnaryRpc(
                _model.Svc,
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
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public async Task<TEntity> Get(string p_keyOrSql, object p_params = null, bool p_loadDetails = false)
        {
            TEntity entity = await new UnaryRpc(_model.Svc, "Da.GetRow", p_keyOrSql, p_params).Call<TEntity>();
            if (entity != null && p_loadDetails)
                await LoadDetails(entity);
            return entity;
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，不存在时返回null，仅支持单主键
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public Task<TEntity> GetByKey(long p_id, bool p_loadDetails = false)
        {
            return Get(_model.Schema.SqlSelect, new { id = p_id }, p_loadDetails);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，不存在时返回null，仅支持单主键
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_loadDetails">是否加载附加数据，默认false</param>
        /// <returns>返回实体对象或null</returns>
        public Task<TEntity> GetByKey(string p_id, bool p_loadDetails = false)
        {
            return Get(_model.Schema.SqlSelect, new { id = p_id }, p_loadDetails);
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
                Type tblType = typeof(Table<>).MakeGenericType(child.Type);
                var rpc = new UnaryRpc(_model.Svc, "Da.Query", child.SqlSelect, new { parentid = p_entity.ID });
                Task task = _callMethod.MakeGenericMethod(tblType).Invoke(rpc, null) as Task;
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
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public async Task<bool> Save(TEntity p_entity, bool p_isNotify = true)
        {
            // p_entity为null时已触发异常
            Dict dt = _model.Schema.GetSaveSql(p_entity);

            // 无需保存
            if (dt == null)
            {
                if (p_isNotify)
                    AtKit.Warn(_unchangedMsg);
                return true;
            }

            if (_model.ExistChild)
            {
                // 含子表，同步保存
                List<Dict> dts = new List<Dict>();
                List<Row> childRows = new List<Row>();
                GetChildSaveSql(p_entity, dts, childRows);

                // 需要批量保存
                if (dts.Count > 0)
                {
                    dts.Insert(0, dt);
                    if (!await BatchExec(dts))
                    {
                        if (p_isNotify)
                            AtKit.Warn("保存失败！");
                        return false;
                    }

                    // 批量成功
                    p_entity.AcceptChanges();
                    childRows.ForEach(t => t.AcceptChanges());
                    if (p_isNotify)
                        AtKit.Msg("保存成功！");
                    return true;
                }
            }

            // 只需保存实体
            int cnt = await Exec((string)dt["text"], (Dict)dt["params"]);
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
        /// 批量保存实体数据，根据实体状态执行增改，同步保存每个实体的子表数据
        /// </summary>
        /// <param name="p_entities">待保存</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>true 保存成功</returns>
        public async Task<bool> Save(IList<TEntity> p_entities, bool p_isNotify = true)
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
            {
                if (p_isNotify)
                    AtKit.Msg(_unchangedMsg);
                return true;
            }

            bool suc = await BatchExec(dts);
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
                if (childRows != null && childRows.Count > 0)
                    childRows.ForEach(t => t.AcceptChanges());

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
        /// <param name="p_row">待删除的行</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public async Task<bool> Delete(TEntity p_entity, bool p_isNotify = true)
        {
            Check.NotNull(p_entity);
            Dict dt = _model.Schema.GetDeleteSql(new List<Row> { p_entity });
            bool suc = await Exec((string)dt["text"], ((List<Dict>)dt["params"])[0]) == 1;
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
        /// <param name="p_rows">实体列表</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public async Task<bool> Delete(IList<TEntity> p_entities, bool p_isNotify = true)
        {
            if (p_entities == null || p_entities.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            Dict dt = _model.Schema.GetDeleteSql(p_entities);
            bool suc = await BatchExec(new List<Dict> { dt });
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
        /// 根据主键删除实体对象，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>实际删除行数</returns>
        public async Task<bool> Delete(long p_id, bool p_isNotify = true)
        {
            bool suc = await Exec(_model.Schema.SqlDelete, new { id = p_id }) == 1;
            if (p_isNotify)
                AtKit.Msg(suc ? "删除成功！" : "删除失败！");
            return suc;
        }

        /// <summary>
        /// 根据主键删除实体对象，依靠数据库的级联删除自动删除子实体
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>实际删除行数</returns>
        public async Task<bool> Delete(string p_id, bool p_isNotify = true)
        {
            bool suc = await Exec(_model.Schema.SqlDelete, new { id = p_id }) == 1;
            if (p_isNotify)
                AtKit.Msg(suc ? "删除成功！" : "删除失败！");
            return suc;
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

        /// <summary>
        /// 互换两行的显示位置，确保包含 id,dispidx 列
        /// </summary>
        /// <param name="p_src"></param>
        /// <param name="p_tgt"></param>
        /// <returns>true 互换成功</returns>
        public Task<bool> ExchangeDispidx(TEntity p_src, TEntity p_tgt)
        {
            var tbl = new Table<TEntity> { { "id", typeof(long) }, { "dispidx", typeof(int) } };

            var save = tbl.AddRow(new { id = p_src.ID });
            save.AcceptChanges();
            save["dispidx"] = p_tgt["dispidx"];

            save = tbl.AddRow(new { id = p_tgt.ID });
            save.AcceptChanges();
            save["dispidx"] = p_src["dispidx"];

            return Save(tbl, false);
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
        #endregion
    }
}
