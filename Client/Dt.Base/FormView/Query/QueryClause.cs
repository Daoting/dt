#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class QueryClause
    {
        string _selectAll;

        public QueryClause(QueryFv p_fv)
        {
            Fv = p_fv;
        }

        public QueryClause(string p_fuzzyOrWhere)
        {
            FuzzyOrWhere = p_fuzzyOrWhere;
        }

        /// <summary>
        /// where后面的条件子句
        /// </summary>
        public QueryFv Fv { get; }

        /// <summary>
        /// 模糊查询的字符串 或 where子句
        /// </summary>
        public string FuzzyOrWhere { get; }

        /// <summary>
        /// 构造查询的sql和参数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_fullSql">是否为完整的sql语句，默认true，false只返回where子句，查询视图时常用</param>
        /// <returns>返回始终不为null</returns>
        public Task<QueryParams> Build<TEntity>(bool p_fullSql = true)
            where TEntity : Entity
        {
            return Build(typeof(TEntity), p_fullSql);
        }

        /// <summary>
        /// 构造查询的sql和参数
        /// </summary>
        /// <param name="p_entityType">实体类型</param>
        /// <param name="p_fullSql">是否为完整的sql语句，默认true，false只返回where子句，查询视图时常用</param>
        /// <returns>返回始终不为null</returns>
        public async Task<QueryParams> Build(Type p_entityType, bool p_fullSql = true)
        {
            List<TableSchema> schemas = new List<TableSchema>();
            bool isVirEntity = p_entityType.GetInterface("IVirEntity") == typeof(IVirEntity);

            if (isVirEntity)
            {
                var vm = await VirEntitySchema.Get(p_entityType);
                _selectAll = vm.GetSelectAllSql();
                foreach (var sc in vm.Schemas)
                {
                    schemas.Add(sc.Schema);
                }
            }
            else
            {
                var model = await EntitySchema.Get(p_entityType);
                _selectAll = model.Schema.GetSelectAllSql();
                schemas.Add(model.Schema);
            }

            if (Fv != null)
                return BuildFv(schemas, p_fullSql);

            if (!string.IsNullOrWhiteSpace(FuzzyOrWhere) && !FuzzyOrWhere.StartsWith('#'))
            {
                // where 子句
                var str = FuzzyOrWhere.Trim();
                if (str.StartsWith("where ", StringComparison.OrdinalIgnoreCase))
                    return new QueryParams(p_fullSql ? _selectAll + " " + str : str, null);

                return BuildFuzzy(schemas, p_fullSql);
            }

            // 模糊串为空或以#开头时，如 #全部
            return new QueryParams(p_fullSql ? _selectAll : null, null);
        }

        QueryParams BuildFuzzy(List<TableSchema> p_schemas, bool p_fullSql)
        {
            // 参数前缀
            var prefix = p_schemas[0].VarPrefix;

            StringBuilder sb = new StringBuilder("where 1!=1");
            foreach (var schema in p_schemas)
            {
                foreach (var col in schema.Columns)
                {
                    if (col.Type == typeof(string))
                    {
                        sb.Append(" or (");
                        sb.Append(col.Name);
                        sb.Append(" like ");
                        sb.Append(prefix);
                        sb.Append("input)");
                    }
                }
            }
            return new QueryParams(p_fullSql ? _selectAll + " " + sb.ToString() : sb.ToString(), new Dict { { "input", $"%{FuzzyOrWhere}%" } });
        }

        QueryParams BuildFv(List<TableSchema> p_schemas, bool p_fullSql)
        {
            // 无需构造查询条件
            if (Fv.Data is not Row row)
                return new QueryParams(p_fullSql ? _selectAll : null, null);

            // 参数前缀
            var prefix = p_schemas[0].VarPrefix;
            HashSet<string> cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var schema in p_schemas)
            {
                foreach (var c in schema.PrimaryKey.Concat(schema.Columns))
                {
                    cols.Add(c.Name);
                }
            }

            // 生成sql查询的where后面的条件子句

            // 加无用的条件，方便字符串处理
            string where = Fv.IsAnd ? "where 1=1" : "where 1!=1";
            StringBuilder sw = new StringBuilder(where);
            Dict dt = new Dict();
            foreach (var cell in Fv.IDCells)
            {
                // 过滤掉忽略的
                if (cell.Query == QueryType.Disable
                    || cell.QueryFlag == CompFlag.Ignore)
                    continue;

                var id = cell.ID;
                var c = row.Cells[id];

                // 忽略空字符串
                if (c.Type == typeof(string) && c.GetVal<string>() == "" && cell.QueryFlag != CompFlag.Null)
                    continue;

                // 忽略空时间
                if (c.Type == typeof(DateTime) && (DateTime)c.Val == default)
                    continue;

                // 忽略id为0的情况
                if (c.Type == typeof(long) && c.Long == 0 && cell.QueryFlag == CompFlag.Equal)
                    continue;

                // 可null值类型
                if (c.Type.IsGenericType
                    && c.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && c.Val == null
                    && cell.QueryFlag != CompFlag.Null)
                    continue;

                // 单字段多条件查询，形如：xxx_min, xxx_max
                var name = id;
                int pos = id.LastIndexOf("_");
                if (pos > -1)
                    name = id.Substring(0, pos);

                // 表结构中没有该列
                if (!cols.Contains(name))
                    continue;

                var val = c.Val;
                sw.Append(Fv.IsAnd ? " and (" : " or (");
                switch (cell.QueryFlag)
                {
                    case CompFlag.Equal://相等 equal
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" = ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.Unequal://不等 unequal
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" != ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.Less://小于 less
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" < ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.Ceil://小于等于
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" <= ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.Greater://大于 more
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" > ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.Floor://大于等于 more equal
                        dt[id] = val;
                        sw.Append(name);
                        sw.Append(" >= ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.StartsWith:// begin with
                        dt[id] = val.ToString() + "%";
                        sw.Append(name);
                        sw.Append(" like ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.EndsWith:// finish with
                        dt[id] = "%" + val.ToString();
                        sw.Append(name);
                        sw.Append(" like ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.Contains:// any where 
                        dt[id] = "%" + val.ToString() + "%";
                        sw.Append(name);
                        sw.Append(" like ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;

                    case CompFlag.Null:
                        sw.Append(name);
                        sw.Append(" is null");
                        if (c.Type == typeof(string))
                        {
                            sw.Append(" or ");
                            sw.Append(name);
                            sw.Append(" = ''");
                        }
                        break;

                    default:
                        dt[id] = val;
                        sw.Append(" = ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;
                }
                sw.Append(")");
            }

            if (sw.Length == where.Length)
            {
                // 无过滤条件，查询所有
                return new QueryParams(p_fullSql ? _selectAll : null, null);
            }

            return new QueryParams(p_fullSql ? _selectAll + " " + sw.ToString() : sw.ToString(), dt);
        }
    }

    public class QueryParams
    {
        public QueryParams(string p_sql, Dict p_params)
        {
            Sql = p_sql;
            Params = p_params;
        }

        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; }

        /// <summary>
        /// sql参数值
        /// </summary>
        public Dict Params { get; }
    }
}