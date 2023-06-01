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

        public QueryClause(string p_fuzzyStr)
        {
            FuzzyStr = p_fuzzyStr;
        }

        /// <summary>
        /// where后面的条件子句
        /// </summary>
        public QueryFv Fv { get; }

        /// <summary>
        /// 模糊查询的字符串值
        /// </summary>
        public string FuzzyStr { get; }

        /// <summary>
        /// 构造查询的sql和参数
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>返回始终不为null</returns>
        public async Task<QueryParams> Build<TEntity>()
            where TEntity : Entity
        {
            List<TableSchema> schemas = new List<TableSchema>();
            var type = typeof(TEntity);
            bool isVirEntity = type.GetInterface("IVirEntity") == typeof(IVirEntity);

            if (isVirEntity)
            {
                var vm = await VirEntitySchema.Get(type);
                _selectAll = vm.GetSelectAllSql();
                foreach (var sc in vm.Schemas)
                {
                    schemas.Add(sc.Schema);
                }
            }
            else
            {
                var model = await EntitySchema.Get(type);
                _selectAll = model.Schema.GetSelectAllSql();
                schemas.Add(model.Schema);
            }

            if (Fv != null)
                return BuildFv(schemas);

            if (!string.IsNullOrWhiteSpace(FuzzyStr) && !FuzzyStr.StartsWith('#'))
                return BuildFuzzy(schemas);

            // 模糊串为空或以#开头时，如 #全部
            return new QueryParams(_selectAll, null);
        }

        QueryParams BuildFuzzy(List<TableSchema> p_schemas)
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
            return new QueryParams(_selectAll + " " + sb.ToString(), new Dict { { "input", $"%{FuzzyStr}%" } });
        }

        QueryParams BuildFv(List<TableSchema> p_schemas)
        {
            // 无需构造查询条件
            if (Fv.Data is not Row row)
                return new QueryParams(_selectAll, null);

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
            StringBuilder sw = new StringBuilder(Fv.IsAnd ? "where 1=1" : "where 1!=1");
            Dict dt = new Dict();
            foreach (var cell in Fv.IDCells)
            {
                // 过滤掉忽略的
                if (cell.Query == QueryType.Disable
                    || cell.QueryFlag == CompFlag.Ignore)
                    continue;

                var id = cell.ID;
                var c = row.Cells[id];

                // 忽略空字符串、空时间
                if ((c.Type == typeof(string) && c.GetVal<string>() == "")
                    || (c.Type == typeof(DateTime) && (DateTime)c.Val == default))
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
                    default:
                        dt[id] = val;
                        sw.Append(" = ");
                        sw.Append(prefix);
                        sw.Append(id);
                        break;
                }
                sw.Append(")");
            }

            if (dt.Count == 0)
            {
                // 无过滤条件，查询所有
                return new QueryParams(_selectAll, null);
            }

            return new QueryParams(_selectAll + " " + sw.ToString(), dt);
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