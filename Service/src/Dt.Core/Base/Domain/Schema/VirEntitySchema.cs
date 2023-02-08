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

        public VirEntitySchema(Type p_type)
        {
            if (!p_type.IsGenericType || !Entity.IsVirEntity(p_type))
                throw new Exception($"{p_type.Name}不是虚拟实体类型！");

#if !SERVER
            AccessInfo ai = null;
#endif
            var tps = p_type.GetGenericArguments();
            foreach (var tp in tps)
            {
                var schema = EntitySchema.Get(tp);

                if (schema.Schema.PrimaryKey.Count != 1)
                {
                    throw new Exception("虚拟实体中的各实体只支持单主键！");
                }

#if !SERVER
                if (ai == null)
                {
                    ai = schema.AccessInfo;
                }
                else if (ai != schema.AccessInfo)
                {
                    throw new Exception("虚拟实体中的各实体无法跨服务进行数据存储！");
                }
#endif
                _schemas.Add(schema);
            }
        }

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
        /// 主键字段名
        /// </summary>
        public string PrimaryKeyName => _schemas[0].Schema.PrimaryKey[0].Name;

#if !SERVER
        /// <summary>
        /// 获取内部实体的AccessInfo
        /// </summary>
        public AccessInfo AccessInfo => _schemas[0].AccessInfo;
#endif

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
                    tbls = $"`{schema.Name}` {prefix}";
                }
                else
                {
                    tbls += $" JOIN `{schema.Name}` {prefix} on a.{schema.PrimaryKey[0].Name}={prefix}.{schema.PrimaryKey[0].Name}";
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
            _selectByID = _selectAll + $" where a.{_schemas[0].Schema.PrimaryKey[0].Name}=@id";
        }

        #region 静态内容
        static readonly ConcurrentDictionary<Type, VirEntitySchema> _models = new ConcurrentDictionary<Type, VirEntitySchema>();

        /// <summary>
        /// 获取实体类型的定义
        /// </summary>
        /// <param name="p_type">实体类型</param>
        /// <returns></returns>
        public static VirEntitySchema Get(Type p_type)
        {
            if (_models.TryGetValue(p_type, out var m))
                return m;

            var model = new VirEntitySchema(p_type);
            _models[p_type] = model;
            return model;
        }
        #endregion
    }
}
