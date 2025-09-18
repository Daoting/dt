#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 子实体结构定义
    /// </summary>
    public class ChildEntitySchema
    {
        public ChildEntitySchema(Type p_type, PropertyInfo p_propInfo, string p_parentID)
        {
            Type = p_type;
            PropInfo = p_propInfo;
            ParentID = p_parentID;
        }

        /// <summary>
        /// 子实体类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 子实体集合在父实体的属性
        /// </summary>
        public PropertyInfo PropInfo { get; }

        /// <summary>
        /// 父表外键字段名
        /// </summary>
        public string ParentID { get; }

        /// <summary>
        /// 表结构
        /// </summary>
        public TableSchema Schema { get; private set; }

        /// <summary>
        /// 查询所有子实体的sql，sql变量名parentid固定
        /// </summary>
        public string SqlSelect => $"select * from {Schema.Prefix}{Schema.Name}{Schema.Postfix} where {ParentID}={Schema.VarPrefix}parentid";

        internal async Task Init()
        {
            var tbl = Type.GetCustomAttribute<TblAttribute>(false);
            if (tbl != null && !string.IsNullOrEmpty(tbl.Name))
            {
                Schema = await EntitySchema.GetTableSchema(tbl);
            }
#if !SERVER
            else
            {
                var sqlite = Type.GetCustomAttribute<SqliteAttribute>(false);
                if (sqlite != null && !string.IsNullOrEmpty(sqlite.DbName))
                {
                    Schema = EntitySchema.GetSqliteSchema(Type, sqlite.DbName);
                }
            }
#endif
            if (Schema == null)
                throw new Exception($"实体{Type.Name}缺少映射表设置！");
        }
    }
}
