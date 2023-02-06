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
            var tbl = p_type.GetCustomAttribute<TblAttribute>(false);
            if (tbl == null || string.IsNullOrEmpty(tbl.Name))
                throw new Exception($"实体{p_type.Name}缺少映射表设置！");

            Type = p_type;
            PropInfo = p_propInfo;
            ParentID = p_parentID;
            Schema = EntitySchema.GetTableSchema(tbl.Name);
            if (Schema.PrimaryKey.Count == 0)
                throw new Exception($"实体{p_type.Name}的映射表{Schema.Name}无主键！");

            // sql变量名parentid固定
            SqlSelect = $"select * from `{Schema.Name}` where {ParentID}=@parentid";
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
        public TableSchema Schema { get; }

        /// <summary>
        /// 查询所有子实体的sql
        /// </summary>
        public string SqlSelect { get; }
    }
}
