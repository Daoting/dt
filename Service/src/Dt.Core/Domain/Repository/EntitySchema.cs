#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体结构定义
    /// </summary>
    public class EntitySchema
    {
        public EntitySchema(Type p_type)
        {
            var tbl = p_type.GetCustomAttribute<TblAttribute>(false);
            if (tbl == null || string.IsNullOrEmpty(tbl.Name))
                throw new Exception($"实体{p_type.Name}缺少映射表设置！");

            Schema = GetTableSchema(tbl.Name);
            if (Schema.PrimaryKey.Count == 0)
                throw new Exception($"实体{p_type.Name}的映射表{Schema.Name}无主键！");
            Extract(p_type);

            OnSaving = GetMethod(p_type, "OnSaving");
            OnDeleting = GetMethod(p_type, "OnDeleting");
            Svc = tbl.Svc;
        }
        
        /// <summary>
        /// 表结构
        /// </summary>
        public TableSchema Schema { get; private set; }

        /// <summary>
        /// 子实体列表
        /// </summary>
        public List<ChildEntitySchema> Children { get; private set; }

        /// <summary>
        /// 是否存在子实体
        /// </summary>
        public bool ExistChild
        {
            get { return Children != null; }
        }

        /// <summary>
        /// 实体所属的服务，客户端用
        /// </summary>
        public string Svc { get; }

        /// <summary>
        /// 保存前的处理，抛出异常时取消保存，实体中的方法规范：私有方法OnSaving，无入参，返回值void 或 Task
        /// </summary>
        public MethodInfo OnSaving { get; }

        /// <summary>
        /// 删除前的处理，抛出异常时取消删除，实体中的方法规范：私有方法OnDeleting，无入参，返回值void 或 Task
        /// </summary>
        public MethodInfo OnDeleting { get; }

#if SERVER
        internal static TableSchema GetTableSchema(string p_tblName)
        {
            return DbSchema.GetTableSchema(p_tblName);
        }
#else
        internal static TableSchema GetTableSchema(string p_tblName)
        {
            TableSchema schema = new TableSchema(p_tblName.ToLower());
            foreach (var oc in AtLocal.QueryColumns(p_tblName.ToLower()))
            {
                TableCol col = new TableCol();
                col.Name = oc.ColName;
                col.Type = Table.GetColType(oc.DbType);
                col.Length = oc.Length;
                col.Nullable = oc.Nullable;
                col.Comments = oc.Comments;
                if (oc.IsPrimary)
                    schema.PrimaryKey.Add(col);
                else
                    schema.Columns.Add(col);
            }
            return schema;
        }
#endif

        /// <summary>
        /// 提取子实体
        /// </summary>
        /// <param name="p_type"></param>
        void Extract(Type p_type)
        {
            List<ChildEntitySchema> ls = new List<ChildEntitySchema>();
            PropertyInfo[] pis = p_type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var pi in pis)
            {
                // 子实体集合为泛型
                ChildTblAttribute attr = pi.GetCustomAttribute<ChildTblAttribute>(false);
                if (attr == null || !pi.PropertyType.IsGenericType)
                    continue;

                Type tpChild = pi.PropertyType.GetGenericArguments()[0];
                ls.Add(new ChildEntitySchema(tpChild, pi, attr.ParentID));
            }
            if (ls.Count > 0)
                Children = ls;
        }

        static MethodInfo GetMethod(Type p_type, string p_name)
        {
            var mi = p_type.GetMethod(p_name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly);
            if (mi != null
                && (mi.ReturnType == typeof(void) || mi.ReturnType == typeof(Task))
                && mi.GetParameters().Length == 0)
            {
                return mi;
            }
            return null;
        }
    }
}
