#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-19 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Concurrent;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 虚拟实体结构定义
    /// </summary>
    public class VirEntitySchema
    {
        readonly List<EntitySchema> _schemas = new List<EntitySchema>();
        string _selectAll;
        string _selectByID;
        string _allTbls;

        /// <summary>
        /// 获取选择所有数据的sql
        /// </summary>
        /// <returns></returns>
        public string GetSelectAllSql()
        {
            if (_selectAll == null)
                CreateSql();
            return _selectAll;
        }

        /// <summary>
        /// 获取选择所有数据的sql
        /// </summary>
        /// <returns></returns>
        public string GetSelectByIDSql()
        {
            if (_selectByID == null)
                CreateSql();
            return _selectByID;
        }

        /// <summary>
        /// 获取按主键列过滤的sql
        /// </summary>
        /// <param name="p_keyName"></param>
        /// <returns></returns>
        public string GetSelectByKeySql(string p_keyName)
        {
            var sql = GetSelectAllSql();
            return $"{sql} where a.{p_keyName}={_schemas[0].Schema.VarPrefix}{p_keyName}";
        }

        /// <summary>
        /// 获取行数的sql
        /// </summary>
        /// <returns></returns>
        public string GetCountSql()
        {
            if (_allTbls == null)
                CreateSql();
            return "select count(*) from " + _allTbls;
        }

        /// <summary>
        /// 获取查询单列的sql
        /// </summary>
        /// <param name="p_colName"></param>
        /// <returns></returns>
        public string GetScalarSql(string p_colName)
        {
            if (string.IsNullOrEmpty(p_colName))
                Throw.Msg("GetScalarSql方法未提供字段名！");

            if (_allTbls == null)
                CreateSql();
            return $"select {p_colName} from {_allTbls}";
        }
        
        /// <summary>
        /// 主键字段名
        /// </summary>
        public string PrimaryKeyName => _schemas[0].Schema.PrimaryKey[0].Name;

#if !SERVER
        /// <summary>
        /// 获取内部实体的DataAccess
        /// </summary>
        public IAccessInfo AccessInfo => _schemas[0].AccessInfo;
#endif
        /// <summary>
        /// 所有实体结构
        /// </summary>
        public IReadOnlyList<EntitySchema> Schemas => _schemas;

        /// <summary>
        /// 获取列定义
        /// </summary>
        /// <param name="p_colName"></param>
        /// <returns></returns>
        public TableCol GetColumn(string p_colName)
        {
            if (string.IsNullOrEmpty(p_colName))
                return null;

            foreach (var sc in _schemas)
            {
                if (sc.Schema.Columns.TryGetValue(p_colName, out var col))
                    return col;

                col = (from c in sc.Schema.PrimaryKey
                       where p_colName.Equals(c.Name, StringComparison.OrdinalIgnoreCase)
                       select c).FirstOrDefault();
                if (col != null)
                    return col;
            }
            return null;
        }

        void CreateSql()
        {
            StringBuilder sb = new StringBuilder();
            string tbls = "";
            for (int i = 0; i < _schemas.Count; i++)
            {
                var schema = _schemas[i].Schema;
                char prefix = (char)('a' + i);
                if (i == 0)
                {
                    // 只使用第一个表的主键
                    sb.Append(prefix);
                    sb.Append(".");
                    sb.Append(schema.PrimaryKey[0].Name);
                    tbls = $"{schema.Prefix}{schema.Name}{schema.Postfix} {prefix}";
                }
                else
                {
                    tbls += $" JOIN {schema.Prefix}{schema.Name}{schema.Postfix} {prefix} on a.{schema.PrimaryKey[0].Name}={prefix}.{schema.PrimaryKey[0].Name}";
                }

                foreach (var col in schema.Columns)
                {
                    // 去除多表中列名重复的列
                    if (i > 0)
                    {
                        bool repeat = false;
                        for (int j = 0; j < i; j++)
                        {
                            var ccs = _schemas[j].Schema.Columns;
                            foreach (var cc in ccs)
                            {
                                if (col.Name.Equals(cc.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    repeat = true;
                                    break;
                                }
                            }
                            if (repeat)
                                break;
                        }
                        if (repeat)
                            continue;
                    }

                    sb.Append(", ");
                    sb.Append(prefix);
                    sb.Append(".");
                    sb.Append(col.Name);
                }
            }
            _selectAll = $"select {sb} from {tbls}";
            _selectByID = _selectAll + $" where a.{_schemas[0].Schema.PrimaryKey[0].Name}={_schemas[0].Schema.VarPrefix}id";
            _allTbls = tbls;
        }

        async Task Init(Type p_type)
        {
            if (!p_type.IsGenericType || !Entity.IsVirEntity(p_type))
                throw new Exception($"{p_type.Name}不是虚拟实体类型！");

            var tps = p_type.GetGenericArguments();
            foreach (var tp in tps)
            {
                var schema = await EntitySchema.Get(tp);

                if (schema.Schema.PrimaryKey.Count != 1)
                {
                    throw new Exception("虚拟实体中的各实体只支持单主键！");
                }

                _schemas.Add(schema);
            }
        }

        #region 静态内容
        static readonly ConcurrentDictionary<Type, VirEntitySchema> _models = new ConcurrentDictionary<Type, VirEntitySchema>();

        /// <summary>
        /// 获取实体类型的定义
        /// </summary>
        /// <param name="p_type">实体类型</param>
        /// <returns></returns>
        public static async Task<VirEntitySchema> Get(Type p_type)
        {
            if (_models.TryGetValue(p_type, out var m))
                return m;

            var model = new VirEntitySchema();
            await model.Init(p_type);
            _models[p_type] = model;
            return model;
        }
        #endregion
    }
}
