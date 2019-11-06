#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-05 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Reflection;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 聚合根子实体的描述类
    /// </summary>
    public class ChildEntity
    {
        public ChildEntity(Type p_type, PropertyInfo p_propInfo, string p_parentID)
        {
            var tbl = p_type.GetCustomAttribute<TblAttribute>(false);
            if (tbl == null || string.IsNullOrEmpty(tbl.Name))
                throw new Exception($"实体{p_type.Name}缺少映射表设置！");

            Type = p_type;
            PropInfo = p_propInfo;
            ParentID = p_parentID;
            TblName = tbl.Name.ToLower();

            // sql变量名parentid固定
            SqlSelect = $"select * from `{TblName}` where {ParentID}=@parentid";
            SqlInsert = ModelBuilder.GetInsertSql(p_type, TblName);
            SqlDelete = $"delete from `{TblName}` where {ParentID}=@parentid";
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
        /// 子实体对应的表名
        /// </summary>
        public string TblName { get; }

        /// <summary>
        /// 查询所有子实体的sql
        /// </summary>
        public string SqlSelect { get; }

        /// <summary>
        /// 插入子实体的sql
        /// </summary>
        public string SqlInsert { get; }

        /// <summary>
        /// 删除所有子实体的sql
        /// </summary>
        public string SqlDelete { get; }
    }
}
