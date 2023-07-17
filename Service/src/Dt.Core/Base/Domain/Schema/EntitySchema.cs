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
        }

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// 表结构
        /// </summary>
        public TableSchema Schema { get; private set; }

#if SERVER
        async Task Init()
        {
            var tbl = EntityType.GetCustomAttribute<TblAttribute>(false);
            if (tbl == null || string.IsNullOrEmpty(tbl.Name))
                throw new Exception($"实体{EntityType.Name}缺少映射表设置！");

            Schema = await GetTableSchema(tbl);
            if (Schema.PrimaryKey.Count == 0)
                throw new Exception($"实体{EntityType.Name}的映射表{Schema.Name}无主键！");
        }

        internal static Task<TableSchema> GetTableSchema(TblAttribute p_tblAttr)
        {
            return DbSchema.GetTableSchema(p_tblAttr.Name);
        }

#else

        /// <summary>
        /// Entity用到的数据访问的描述信息
        /// </summary>
        public AccessInfo AccessInfo { get; private set; }

        async Task Init()
        {
            AccessInfo ai = null;
            var tbl = EntityType.GetCustomAttribute<TblAttribute>(false);
            if (tbl != null && !string.IsNullOrEmpty(tbl.Name))
            {
                Schema = await GetTableSchema(tbl);
                ai = new AccessInfo(AccessType.Remote, tbl.Svc);
            }
            else
            {
                var sqlite = EntityType.GetCustomAttribute<SqliteAttribute>(false);
                if (sqlite != null && !string.IsNullOrEmpty(sqlite.DbName))
                {
                    Schema = GetSqliteSchema(EntityType);
                    ai = new AccessInfo(AccessType.Local, sqlite.DbName);
                }
            }
            if (ai == null)
                throw new Exception($"实体{EntityType.Name}缺少 TblAttribute 或 SqliteAttribute 标签！");
            AccessInfo = ai;

            if (Schema.PrimaryKey.Count == 0)
                throw new Exception($"实体{EntityType.Name}的映射表{Schema.Name}无主键！");
        }

        internal static Task<TableSchema> GetTableSchema(TblAttribute p_tblAttr)
        {
            return Kit.GetRequiredService<IModelCallback>().GetTableSchema(p_tblAttr);
        }

        internal static TableSchema GetSqliteSchema(Type p_type)
        {
            // 不包括继承的属性
            var props = p_type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.DeclaredOnly);

            // 删除后缀 X
            var name = p_type.Name.TrimEnd('X');
            TableSchema schema = new TableSchema(name, DatabaseType.Sqlite);
            foreach (var p in props)
            {
                if (!p.CanWrite || p.GetCustomAttribute<IgnoreAttribute>(false) != null)
                    continue;

                bool isPK = p.GetCustomAttribute<PrimaryKeyAttribute>(false) != null;
                TableCol col = new TableCol(schema);
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
        public async Task<List<ChildEntitySchema>> GetChildren()
        {
            if (_children == null)
            {
                // 提取子实体
                _children = new List<ChildEntitySchema>();
                PropertyInfo[] pis = EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var pi in pis)
                {
                    // 子实体集合为泛型
                    var attr = pi.GetCustomAttribute<ChildXAttribute>(false);
                    if (attr == null || !pi.PropertyType.IsGenericType)
                        continue;

                    Type tpChild = pi.PropertyType.GetGenericArguments()[0];
                    var child = new ChildEntitySchema(tpChild, pi, attr.ParentID);
                    await child.Init();
                    _children.Add(child);
                }
            }
            return _children;
        }
        #endregion

        #region 静态内容
        static readonly ConcurrentDictionary<Type, EntitySchema> _models = new ConcurrentDictionary<Type, EntitySchema>();

        /// <summary>
        /// 获取实体类型的定义
        /// </summary>
        /// <param name="p_type">实体类型</param>
        /// <returns></returns>
        public static async Task<EntitySchema> Get(Type p_type)
        {
            if (_models.TryGetValue(p_type, out var m))
                return m;

            var model = new EntitySchema(p_type);
            await model.Init();
            _models[p_type] = model;
            return model;
        }
        #endregion
    }
}
