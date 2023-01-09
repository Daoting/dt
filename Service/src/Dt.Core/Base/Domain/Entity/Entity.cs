#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity : Row
    {
#if SERVER
        List<DomainEvent> _events;
        DataProvider _dp;

        /// <summary>
        /// 获取数据提供者，提供给 Entity 内部查询使用，禁止保存或删除操作！
        /// </summary>
        public DataProvider Dp
        {
            get
            {
                if (_dp == null)
                    _dp = new DataProvider(false);
                return _dp;
            }
        }

        /// <summary>
        /// 增加领域事件
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_isRemoteEvent"></param>
        protected void AddDomainEvent(EventBus.IEvent p_event, bool p_isRemoteEvent = false)
        {
            if (_events == null)
                _events = new List<DomainEvent>();
            _events.Add(new DomainEvent(p_isRemoteEvent, p_event));
        }

        /// <summary>
        /// 获取实体对象的领域事件
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<DomainEvent> GetDomainEvents()
        {
            return _events;
        }

        /// <summary>
        /// 清空实体对象的领域事件
        /// </summary>
        internal void ClearDomainEvents()
        {
            _events?.Clear();
        }
#endif

        #region Hook
        /// <summary>
        /// 重写该方法用来统一添加当前实体的所有回调方法，回调方法中通过抛出异常使操作失败、阻止继续执行！
        /// <para>主要包括三类回调：</para>
        /// <para>Cell.Val值变化前的回调</para>
        /// <para>Entity保存前回调</para>
        /// <para>Entity删除前回调</para>
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
        /// 注册Entity删除前的回调，回调方法中通过抛出异常使删除失败，并且阻止继续执行！
        /// </summary>
        /// <param name="p_callback"></param>
        protected void OnDeleting(Func<Task> p_callback)
        {
            GetHooks().DeletingHook = p_callback;
        }

        /// <summary>
        /// 注册Cell.Val值变化前的回调，回调方法通常为业务校验，校验失败时触发异常使赋值失败，并使UI重绑回原值
        /// </summary>
        /// <typeparam name="T">Cell值类型</typeparam>
        /// <param name="p_id">nameof(ID)，使用 nameof 避免列名不存在</param>
        /// <param name="p_callback">Hook 方法</param>
        protected void OnChanging<T>(string p_id, Action<T> p_callback)
        {
            if (_cells.Contains(p_id) && p_callback != null)
                GetHooks().AddCellHook(p_id, p_callback);
        }

        /// <summary>
        /// 获取Cell.Val值变化前的回调方法
        /// </summary>
        /// <param name="p_id">Cell.ID</param>
        /// <returns></returns>
        public Action<object> GetCellHook(string p_id)
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
        /// 获取Entity删除前的回调方法
        /// </summary>
        /// <returns></returns>
        public Func<Task> GetDeletingHook()
        {
            return GetHooks().DeletingHook;
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
            readonly Dictionary<string, Action<object>> _cellHooks = new Dictionary<string, Action<object>>(StringComparer.OrdinalIgnoreCase);

            public Func<Task> DeletingHook { get; set; }

            public Func<Task> SavingHook { get; set; }

            public Action<object> GetCellHook(string p_id)
            {
                if (_cellHooks.Count > 0 && _cellHooks.TryGetValue(p_id, out var hook))
                    return hook;
                return null;
            }

            public void AddCellHook<T>(string p_id, Action<T> p_callback)
            {
                // Action<T> 无法转成 Action<object>，只能内部调用
                _cellHooks[p_id] = new Action<object>((o) => p_callback((T)o));
            }
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
