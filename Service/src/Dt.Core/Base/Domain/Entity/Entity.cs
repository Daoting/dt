#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Concurrent;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity : Row
    {
        #region 构造方法
        public Entity()
        { }

        /// <summary>
        /// 和外部共用Cells
        /// </summary>
        /// <param name="p_cells"></param>
        public Entity(CellList p_cells)
            : base(p_cells)
        { }
        #endregion

        #region 领域事件
        List<IEvent> _events;

        /// <summary>
        /// 增加领域事件
        /// </summary>
        /// <param name="p_event"></param>
        protected void AddEvent(IEvent p_event)
        {
            if (_events == null)
                _events = new List<IEvent>();
            _events.Add(p_event);
        }

        /// <summary>
        /// 获取实体对象的领域事件
        /// </summary>
        /// <returns></returns>
        internal List<IEvent> GetEvents()
        {
            return _events;
        }

        /// <summary>
        /// 清空实体对象的领域事件
        /// </summary>
        internal void ClearEvents()
        {
            _events?.Clear();
        }
        #endregion

        #region Hook
        /// <summary>
        /// 重写该方法用来统一添加当前实体的所有回调方法，回调方法中通过抛出异常使操作失败、阻止继续执行！
        /// <para>主要包括三类回调：</para>
        /// <para>1. Cell.Val值变化前的回调</para>
        /// <para>2. Entity保存前回调、保存成功后的回调</para>
        /// <para>3. Entity删除前回调、删除成功后的回调</para>
        /// <para>只在初次值变化前、保存前、删除前时调用InitHook，否则始终不调用 InitHook</para>
        /// </summary>
        protected virtual void InitHook()
        {
        }

        /// <summary>
        /// 注册Entity保存前的回调，回调方法中通过抛出异常使保存失败，并且阻止继续执行！
        /// </summary>
        /// <param name="p_callback"></param>
        protected void OnSaving(Func<Task> p_callback)
        {
            GetHooks().SavingHook = p_callback;
        }

        /// <summary>
        /// 注册Entity保存成功后的回调
        /// </summary>
        /// <param name="p_callback"></param>
        protected void OnSaved(Func<Task> p_callback)
        {
            GetHooks().SavedHook = p_callback;
        }

        /// <summary>
        /// 注册Entity删除前的回调，回调方法中通过抛出异常使删除失败，并且阻止继续执行！
        /// </summary>
        /// <param name="p_callback"></param>
        protected void OnDeleting(Func<Task> p_callback)
        {
            GetHooks().DeletingHook = p_callback;
        }

        /// <summary>
        /// 注册Entity删除成功后的回调
        /// </summary>
        /// <param name="p_callback"></param>
        protected void OnDeleted(Func<Task> p_callback)
        {
            GetHooks().DeletedHook = p_callback;
        }

        /// <summary>
        /// 注册Cell.Val值变化前的回调，回调方法通常为业务校验或特殊数据处理，校验失败时触发异常使赋值失败，并使UI重绑回原值
        /// </summary>
        /// <param name="p_cell">当前Cell</param>
        /// <param name="p_callback">Hook 方法</param>
        protected void OnChanging(Cell p_cell, Action<CellValChangingArgs> p_callback)
        {
            if (p_cell != null && p_callback != null)
                GetHooks().AddCellHook(p_cell, p_callback);
        }

        /// <summary>
        /// 获取Cell.Val值变化前的回调方法
        /// </summary>
        /// <param name="p_id">Cell.ID</param>
        /// <returns></returns>
        public override Action<CellValChangingArgs> GetCellHook(string p_id)
        {
            return GetHooks().GetCellHook(p_id);
        }

        /// <summary>
        /// 获取Entity保存前的回调方法
        /// </summary>
        /// <returns></returns>
        public Func<Task> GetSavingHook()
        {
            return GetHooks().SavingHook;
        }

        /// <summary>
        /// 获取Entity成功保存后的回调方法
        /// </summary>
        /// <returns></returns>
        public Func<Task> GetSavedHook()
        {
            return GetHooks().SavedHook;
        }

        /// <summary>
        /// 获取Entity删除前的回调方法
        /// </summary>
        /// <returns></returns>
        public Func<Task> GetDeletingHook()
        {
            return GetHooks().DeletingHook;
        }

        /// <summary>
        /// 获取Entity成功删除后的回调方法
        /// </summary>
        /// <returns></returns>
        public Func<Task> GetDeletedHook()
        {
            return GetHooks().DeletedHook;
        }

        EntityHooks GetHooks()
        {
            if (_hooks == null)
            {
                // 只在初次值变化前、保存前、删除前时调用InitHook，否则始终不调用 InitHook ！
                _hooks = new EntityHooks();
                InitHook();
            }
            return _hooks;
        }

        // 当前实体的Hook
        EntityHooks _hooks;

        class EntityHooks
        {
            readonly Dictionary<string, Action<CellValChangingArgs>> _cellHooks = new Dictionary<string, Action<CellValChangingArgs>>(StringComparer.OrdinalIgnoreCase);

            public Func<Task> SavingHook { get; set; }

            public Func<Task> SavedHook { get; set; }

            public Func<Task> DeletingHook { get; set; }

            public Func<Task> DeletedHook { get; set; }

            public Action<CellValChangingArgs> GetCellHook(string p_id)
            {
                if (_cellHooks.Count > 0 && _cellHooks.TryGetValue(p_id, out var hook))
                    return hook;
                return null;
            }

            public void AddCellHook(Cell p_cell, Action<CellValChangingArgs> p_callback)
            {
                _cellHooks[p_cell.ID] = p_callback;
            }
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 是否为虚拟实体
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static bool IsVirEntity(Type p_type)
        {
            return p_type.GetInterface("IVirEntity") == typeof(IVirEntity);
        }

        // 缓存实体类的特殊属性
        static readonly ConcurrentDictionary<Type, Dictionary<string, Type>> _specialCols = new ConcurrentDictionary<Type, Dictionary<string, Type>>();

        /// <summary>
        /// 提取特殊类型的属性：enum bool
        /// bool：mysql tinyint(1)，sqlserver bit，pg bool 默认映射，只有 oracle char(1) 需特殊处理
        /// enum：mysql sqlserver枚举为byte，oracle pg枚举为short
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static Dictionary<string, Type> GetSpecialCols(Type p_type)
        {
            if (p_type == null)
                return null;

            if (_specialCols.TryGetValue(p_type, out var dt))
                return dt;

            dt = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var props = p_type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (props != null && props.Length > 0)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    var prop = props[i];
                    if (prop.CanWrite)
                    {
                        // 支持可null类型
                        Type tp = prop.PropertyType;
                        if (tp.IsGenericType && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
                            tp = tp.GetGenericArguments()[0];

                        if (tp.IsEnum || tp == typeof(bool))
                        {
                            dt.Add(prop.Name, prop.PropertyType);
                        }
                    }
                }
            }
            _specialCols.TryAdd(p_type, dt);
            return dt;
        }
        #endregion

        #region 判断相同
        /// <summary>
        /// 判断两实体是否相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // 比较的对象为 NULL 或者对象不是派生自 Entity 都视为不相等
            if (obj == null || !(obj is Entity))
                return false;

            // 相同实例
            if (ReferenceEquals(this, obj))
                return true;

            // 类型不同
            if (GetType() != obj.GetType())
                return false;

            // 比较主键
            var other = (Entity)obj;
            if (Contains("id") && other.Contains("id"))
                return GetVal<string>("id") == other.GetVal<string>("id");

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (!Contains("id"))
                return 0;
            return GetVal<string>("id").GetHashCode();
        }

        /// <summary>
        /// 输出实体描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[Entity: {GetType().Name}] ID = {ID}";
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
                return Equals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
        #endregion
    }
}
