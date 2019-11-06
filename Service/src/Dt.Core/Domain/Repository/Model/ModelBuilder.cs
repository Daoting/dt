#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 聚合根映射表管理类
    /// </summary>
    public class ModelBuilder
    {
        public ModelBuilder(Type p_type)
        {
            var tbl = p_type.GetCustomAttribute<TblAttribute>(false);
            if (tbl == null || string.IsNullOrEmpty(tbl.Name))
                throw new Exception($"实体{p_type.Name}缺少映射表设置！");

            TblName = tbl.Name.ToLower();
            SqlSelect = $"select * from `{TblName}` where id=@id";
            SqlInsert = GetInsertSql(p_type, TblName);
            SqlUpdate = GetUpdateSql(p_type, TblName);
            SqlDelete = $"delete from `{TblName}` where id=@id";
            Extract(p_type);
        }

        /// <summary>
        /// 实体类对应的表名
        /// </summary>
        public string TblName { get; }

        /// <summary>
        /// 根据id查询实体的sql
        /// </summary>
        public string SqlSelect { get; }

        /// <summary>
        /// 插入实体的sql
        /// </summary>
        public string SqlInsert { get; }

        /// <summary>
        /// 更新实体的sql
        /// </summary>
        public string SqlUpdate { get; }

        /// <summary>
        /// 删除实体的sql
        /// </summary>
        public string SqlDelete { get; }

        /// <summary>
        /// 子实体列表
        /// </summary>
        public List<ChildEntity> Children { get; private set; }

        /// <summary>
        /// 是否存在子实体
        /// </summary>
        public bool ExistChild
        {
            get { return Children != null; }
        }

        /// <summary>
        /// 提取子实体
        /// </summary>
        /// <param name="p_type"></param>
        void Extract(Type p_type)
        {
            List<ChildEntity> ls = new List<ChildEntity>();
            PropertyInfo[] pis = p_type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var pi in pis)
            {
                // 子实体集合为泛型
                ChildTblAttribute attr = pi.GetCustomAttribute<ChildTblAttribute>(false);
                if (attr == null || !pi.PropertyType.IsGenericType)
                    continue;

                Type tpChild = pi.PropertyType.GetGenericArguments()[0];
                ls.Add(new ChildEntity(tpChild, pi, attr.ParentID));
            }
            if (ls.Count > 0)
                Children = ls;
        }

        /// <summary>
        /// 获取实体类型对应的insert语句模板
        /// </summary>
        /// <param name="p_entityType">实体类型</param>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static string GetInsertSql(Type p_entityType, string p_tblName)
        {
            StringBuilder insertCol = new StringBuilder();
            StringBuilder insertVal = new StringBuilder();
            var schema = DbSchema.GetTableSchema(p_tblName);
            foreach (var col in schema.PrimaryKey.Concat(schema.Columns))
            {
                // 忽略没有的属性列
                if (p_entityType.GetProperty(col.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null)
                {
                    // 自动设置创建时间和修改时间
                    if (col.Name == "ctime" || col.Name == "mtime")
                    {
                        insertCol.Append(col.Name);
                        insertCol.Append(",");

                        insertVal.Append("now(),");
                    }
                    continue;
                }

                insertCol.Append(col.Name);
                insertCol.Append(",");

                insertVal.Append("@");
                insertVal.Append(col.Name);
                insertVal.Append(",");
            }
            return $"insert into `{p_tblName}` ({insertCol.ToString().TrimEnd(',')}) values ({insertVal.ToString().TrimEnd(',')})";
        }

        /// <summary>
        /// 获取实体类型对应的update语句模板
        /// </summary>
        /// <param name="p_entityType">实体类型</param>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static string GetUpdateSql(Type p_entityType, string p_tblName)
        {
            StringBuilder updateVal = new StringBuilder();
            StringBuilder whereVal = new StringBuilder();
            var schema = DbSchema.GetTableSchema(p_tblName);
            foreach (var col in schema.PrimaryKey)
            {
                if (p_entityType.GetProperty(col.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null)
                    throw new Exception($"实体类型{p_entityType.Name}中缺少主键列{col.Name}！");

                if (whereVal.Length > 0)
                    whereVal.Append(" and ");
                whereVal.Append(col.Name);
                whereVal.Append("=@");
                whereVal.Append(col.Name);
            }
            foreach (var col in schema.Columns)
            {
                // 忽略没有的属性列
                if (p_entityType.GetProperty(col.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null)
                {
                    // 自动设置修改时间
                    if (col.Name == "mtime")
                    {
                        if (updateVal.Length > 0)
                            updateVal.Append(", ");
                        updateVal.Append("mtime=now()");
                    }
                    continue;
                }

                if (updateVal.Length > 0)
                    updateVal.Append(", ");
                updateVal.Append(col.Name);
                updateVal.Append("=@");
                updateVal.Append(col.Name);
            }
            return $"update `{p_tblName}` set {updateVal} where {whereVal}";
        }
    }
}
