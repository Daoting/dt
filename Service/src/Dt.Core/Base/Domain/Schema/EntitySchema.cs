#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Concurrent;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体结构定义
    /// </summary>
    public class EntitySchema
    {
        EntitySchema(Type p_type)
        {
            EntityType = p_type;
            Init(p_type);
        }

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// 表结构
        /// </summary>
        public TableSchema Schema { get; private set; }

        /// <summary>
        /// 缓存处理对象，无缓存时null
        /// </summary>
        internal EntityCacher Cacher { get; private set; }

#if SERVER
        void Init(Type p_type)
        {
            var tbl = p_type.GetCustomAttribute<TblAttribute>(false);
            if (tbl == null || string.IsNullOrEmpty(tbl.Name))
                throw new Exception($"实体{p_type.Name}缺少映射表设置！");

            Schema = GetTableSchema(tbl.Name);
            if (Schema.PrimaryKey.Count == 0)
                throw new Exception($"实体{p_type.Name}的映射表{Schema.Name}无主键！");

            // 缓存设置
            var cfg = p_type.GetCustomAttribute<CacheAttribute>(false);
            if (cfg != null)
                Cacher = new EntityCacher(this, cfg);
        }

        internal static TableSchema GetTableSchema(string p_tblName)
        {
            return DbSchema.GetTableSchema(p_tblName);
        }

#else

        /// <summary>
        /// Entity用到的数据访问的描述信息
        /// </summary>
        public AccessInfo AccessInfo { get; private set; }

        void Init(Type p_type)
        {
            AccessInfo ai = null;
            var tbl = p_type.GetCustomAttribute<TblAttribute>(false);
            if (tbl != null && !string.IsNullOrEmpty(tbl.Name))
            {
                Schema = GetTableSchema(tbl.Name);
                ai = new AccessInfo(AccessType.Remote, tbl.Svc);
            }
            else
            {
                var sqlite = p_type.GetCustomAttribute<SqliteAttribute>(false);
                if (sqlite != null && !string.IsNullOrEmpty(sqlite.DbName))
                {
                    Schema = GetSqliteSchema(p_type);
                    ai = new AccessInfo(AccessType.Local, sqlite.DbName);
                }
            }
            if (ai == null)
                throw new Exception($"实体{p_type.Name}缺少 TblAttribute 或 SqliteAttribute 标签！");
            AccessInfo = ai;

            if (Schema.PrimaryKey.Count == 0)
                throw new Exception($"实体{p_type.Name}的映射表{Schema.Name}无主键！");

            // redis缓存设置，本地sqlite库访问无缓存
            if (ai.Type == AccessType.Remote)
            {
                var cfg = p_type.GetCustomAttribute<CacheAttribute>(false);
                if (cfg != null)
                    Cacher = new EntityCacher(this, cfg);
            }
        }

        internal static TableSchema GetTableSchema(string p_tblName)
        {
            var svc = Kit.GetRequiredService<IModelCallback>();
            if (svc == null)
                return null;

            var tblName = p_tblName.ToLower();
            var cols = svc.GetTableColumns(tblName).Result;
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

        static TableSchema GetSqliteSchema(Type p_type)
        {
            // 不包括继承的属性
            var props = p_type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.DeclaredOnly);

            // 删除后缀 X
            var name = p_type.Name.TrimEnd('X');
            TableSchema schema = new TableSchema(name);
            foreach (var p in props)
            {
                if (!p.CanWrite || p.GetCustomAttribute<IgnoreAttribute>(false) != null)
                    continue;

                bool isPK = p.GetCustomAttribute<PrimaryKeyAttribute>(false) != null;
                TableCol col = new TableCol();
                col.Name = p.Name;
                col.Type = p.PropertyType;
                col.Nullable = !isPK;
                if (isPK)
                    schema.PrimaryKey.Add(col);
                else
                    schema.Columns.Add(col);
            }
            return schema;
        }
#endif

        #region 子实体
        List<ChildEntitySchema> _children = null;

        /// <summary>
        /// 子实体列表
        /// </summary>
        public List<ChildEntitySchema> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = ExtractChild();
                }
                return _children;
            }
        }

        /// <summary>
        /// 提取子实体
        /// </summary>
        List<ChildEntitySchema> ExtractChild()
        {
            List<ChildEntitySchema> ls = new List<ChildEntitySchema>();
            PropertyInfo[] pis = EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var pi in pis)
            {
                // 子实体集合为泛型
                var attr = pi.GetCustomAttribute<ChildXAttribute>(false);
                if (attr == null || !pi.PropertyType.IsGenericType)
                    continue;

                Type tpChild = pi.PropertyType.GetGenericArguments()[0];
                ls.Add(new ChildEntitySchema(tpChild, pi, attr.ParentID));
            }
            return ls;
        }
        #endregion

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
    }
}
