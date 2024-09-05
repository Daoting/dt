#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;

#endregion

namespace Dt.Core
{
    /// <summary>
    /// Entity视图查询
    /// </summary>
    public class EntityView<TEntity, TView>
        where TEntity : Entity
        where TView : EntityViewAttribute
    {
        /// <summary>
        /// 查询视图列表，可以提供 where子句 或 Sql语句 进行查询
        /// </summary>
        /// <param name="p_whereOrSql">三种查询：
        /// <para>1. where子句，以where开头的过滤条件，返回的视图包含所有列值</para>
        /// <para>2. Sql语句，自由查询，返回的列值自由</para>
        /// <para>3. null时返回所有行</para>
        /// </param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回视图列表</returns>
        public static async Task<Table<TEntity>> Query(string p_whereOrSql, object p_params = null)
        {
            var model = await EntitySchema.Get(typeof(TEntity));
            var sql = GetSelectSql(model, p_whereOrSql);
#if SERVER
            return await Kit.DataAccess.Query<TEntity>(sql, p_params);
#else
            return await model.AccessInfo.GetDa().Query<TEntity>(sql, p_params);
#endif
        }

        /// <summary>
        /// 按页查询视图列表，可以提供 where子句 或 Sql语句 进行查询
        /// </summary>
        /// <param name="p_starRow">起始序号：第一行的序号统一为0</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_whereOrSql">两种查询：
        /// <para>1. where子句，以where开头的过滤条件，返回的视图包含所有列值</para>
        /// <para>2. Sql语句自由查询，返回的视图列值自由</para>
        /// <para>3. null时返回所有视图</para>
        /// </param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回视图列表</returns>
        public static async Task<Table<TEntity>> Page(int p_starRow, int p_pageSize, string p_whereOrSql, object p_params = null)
        {
            var model = await EntitySchema.Get(typeof(TEntity));
            var sql = GetSelectSql(model, p_whereOrSql);
#if SERVER
            return await Kit.DataAccess.Page<TEntity>(p_starRow, p_pageSize, sql, p_params);
#else
            return await model.AccessInfo.GetDa().Page<TEntity>(p_starRow, p_pageSize, sql, p_params);
#endif
        }

        /// <summary>
        /// 返回第一个视图对象，不存在时返回null，可以提供 where子句 或 Sql语句 或 存储过程名 进行查询
        /// </summary>
        /// <param name="p_whereOrSql">三种查询：
        /// <para>1. where子句，以where开头的过滤条件，返回的视图包含所有列值</para>
        /// <para>2. Sql语句，自由查询，返回的视图列值自由</para>
        /// <para>3. null时返回第一个视图</para>
        /// </param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回视图对象或null</returns>
        public static async Task<TEntity> First(string p_whereOrSql, object p_params = null)
        {
            var model = await EntitySchema.Get(typeof(TEntity));
            var sql = GetSelectSql(model, p_whereOrSql);
#if SERVER
            return await Kit.DataAccess.First<TEntity>(sql, p_params);
#else
            return await model.AccessInfo.GetDa().First<TEntity>(sql, p_params);
#endif
        }

        /// <summary>
        /// 根据主键获得视图对象(包含所有列值)，仅支持单主键
        /// </summary>
        /// <param name="p_id">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public static async Task<TEntity> GetByID(object p_id)
        {
            if (p_id == null)
                return default(TEntity);

            var model = await EntitySchema.Get(typeof(TEntity));
            if (model.Schema.PrimaryKey.Count != 1)
                Throw.Msg("视图中根据主键获得实体对象时仅支持单主键！");

            return await GetByKeyInternal(model.Schema.PrimaryKey[0].Name, p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<TEntity> GetByKey(string p_keyName, object p_keyVal)
        {
            if (string.IsNullOrWhiteSpace(p_keyName) || p_keyVal == null)
                Throw.Msg("GetByKey查询时主键或唯一索引不可为空！");

            return GetByKeyInternal(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 获取符合条件的行数
        /// </summary>
        /// <param name="p_where">where子句，空或null返回总行数</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns></returns>
        public static async Task<int> GetCount(string p_where, object p_params = null)
        {
            var model = await EntitySchema.Get(typeof(TEntity));
            var sql = GetCountSql(model, p_where);
#if SERVER
            return await Kit.DataAccess.GetScalar<int>(sql, p_params);
#else
            return await model.AccessInfo.GetDa().GetScalar<int>(sql, p_params);
#endif
        }

        static string GetSelectSql(EntitySchema p_model, string p_whereOrSql)
        {
            var sql = $"select * from {p_model.Schema.Prefix}{GetViewName()}{p_model.Schema.Prefix} a";

            if (!string.IsNullOrWhiteSpace(p_whereOrSql))
            {
                var txt = p_whereOrSql.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    sql += " " + txt;
                }
                else
                {
                    // sql语句，自由查询
                    sql = txt;
                }
            }
            return sql;
        }

        static async Task<TEntity> GetByKeyInternal(string p_keyName, object p_keyVal)
        {
            var dt = new Dict { { p_keyName, p_keyVal } };
            var model = await EntitySchema.Get(typeof(TEntity));
            var sql = string.Format("select * from {0}{1}{0} where {0}{2}{0}={3}{2}",
                model.Schema.Prefix,
                GetViewName(),
                p_keyName,
                model.Schema.VarPrefix);
#if SERVER
            return await Kit.DataAccess.First<TEntity>(sql, dt);
#else
            return await model.AccessInfo.GetDa().First<TEntity>(sql, dt);
#endif
        }

        static string GetCountSql(EntitySchema p_model, string p_where)
        {
            var sql = $"select count(*) from {p_model.Schema.Prefix}{GetViewName()}{p_model.Schema.Prefix}";

            if (!string.IsNullOrWhiteSpace(p_where))
            {
                var txt = p_where.Trim();
                if (txt.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                {
                    // where子句
                    sql += " " + txt;
                }
            }
            return sql;
        }

        static string GetViewName()
        {
            if (string.IsNullOrEmpty(_viewName))
            {
                Type tp = typeof(TEntity);
                var view = tp.GetCustomAttribute(typeof(TView), false) as EntityViewAttribute;
                if (view == null || string.IsNullOrEmpty(view.Name))
                    throw new Exception($"实体 [{tp.Name}] 缺少视图 [{typeof(TView).Name}] 的设置！");
                _viewName = view.Name;
            }
            return _viewName;
        }

        static string _viewName;
    }
}
