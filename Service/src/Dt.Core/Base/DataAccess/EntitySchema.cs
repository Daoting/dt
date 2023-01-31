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
        internal CacheHandler CacheHandler { get; private set; }

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
            if (cfg != null && !string.IsNullOrEmpty(cfg.PrefixKey))
                CacheHandler = new CacheHandler(this, cfg);
        }

        static TableSchema GetTableSchema(string p_tblName)
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
                Schema = GetTableSchema(tbl.Name).Result;
                int index = tbl.Name.IndexOf("_");
                var svc = index == -1 ? "cm" : tbl.Name.Substring(0, index).ToLower();
                ai = new AccessInfo(AccessType.Remote, svc);
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
                if (cfg != null && !string.IsNullOrEmpty(cfg.PrefixKey))
                    CacheHandler = new CacheHandler(this, cfg);
            }
        }

        static async Task<TableSchema> GetTableSchema(string p_tblName)
        {
            var svc = Kit.GetRequiredService<IModelCallback>();
            if (svc == null)
                return null;

            var tblName = p_tblName.ToLower();
            var cols = await svc.GetTableColumns(tblName);
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

            // 删除尾部Obj
            var name = p_type.Name;
            if (name.EndsWith("Obj", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(0, name.Length - 3);
            
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
