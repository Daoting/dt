﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Concurrent;
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

#if SERVER
            // 领域事件类型
            var cud = p_type.GetCustomAttribute<CudEventAttribute>(false);
            if (cud != null)
                CudEvent = cud.Event;
            else
                CudEvent = CudEvent.None;

            // 缓存设置
            var cfg = p_type.GetCustomAttribute<CacheAttribute>(false);
            if (cfg != null && !string.IsNullOrEmpty(cfg.PrefixKey))
                CacheHandler = new CacheHandler(this, cfg);
#endif
        }

        /// <summary>
        /// 表结构
        /// </summary>
        public TableSchema Schema { get; private set; }

#if SERVER
        /// <summary>
        /// 触发增删改领域事件的类型
        /// </summary>
        public CudEvent CudEvent { get; }

        /// <summary>
        /// 缓存处理对象，无缓存时null
        /// </summary>
        internal CacheHandler CacheHandler { get; }

        internal static TableSchema GetTableSchema(string p_tblName)
        {
            return DbSchema.GetTableSchema(p_tblName);
        }
#else
        static TableSchema GetTableSchema(string p_tblName)
        {
            var tblName = p_tblName.ToLower();
            var cols = Kit.GetRequiredService<IModelCallback>().GetTableColumns(tblName);
            TableSchema schema = new TableSchema(tblName);
            foreach (var oc in cols)
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

        #region 静态内容
        static readonly ConcurrentDictionary<Type, EntitySchema> _models = new ConcurrentDictionary<Type, EntitySchema>();

        /// <summary>
        /// 获取实体类型的定义
        /// </summary>
        /// <param name="p_type">实体类型</param>
        /// <returns></returns>
        public static EntitySchema Get(Type p_type)
        {
            if (_models.TryGetValue(p_type, out var m))
                return m;

            var model = new EntitySchema(p_type);
            _models[p_type] = model;
            return model;
        }
        #endregion

        #region 废除子实体
        ///// <summary>
        ///// 子实体列表
        ///// </summary>
        //public List<ChildEntitySchema> Children { get; private set; }

        ///// <summary>
        ///// 是否存在子实体
        ///// </summary>
        //public bool ExistChild
        //{
        //    get { return Children != null; }
        //}

        ///// <summary>
        ///// 提取子实体
        ///// </summary>
        ///// <param name="p_type"></param>
        //void Extract(Type p_type)
        //{
        //    List<ChildEntitySchema> ls = new List<ChildEntitySchema>();
        //    PropertyInfo[] pis = p_type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        //    foreach (var pi in pis)
        //    {
        //        // 子实体集合为泛型
        //        ChildTblAttribute attr = pi.GetCustomAttribute<ChildTblAttribute>(false);
        //        if (attr == null || !pi.PropertyType.IsGenericType)
        //            continue;

        //        Type tpChild = pi.PropertyType.GetGenericArguments()[0];
        //        ls.Add(new ChildEntitySchema(tpChild, pi, attr.ParentID));
        //    }
        //    if (ls.Count > 0)
        //        Children = ls;
        //}
        #endregion
    }
}
