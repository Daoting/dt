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
        // 所有实体类型的Hook
        static readonly Dictionary<Type, Dictionary<string, Action<object>>> _allHooks = new Dictionary<Type, Dictionary<string, Action<object>>>();
        // 当前实体的Hook
        Dictionary<string, Action<object>> _hooks;

        /// <summary>
        /// 获取 Cell 的 Hook 方法
        /// </summary>
        /// <param name="p_id">Cell.ID</param>
        /// <returns></returns>
        public Action<object> GetHook(string p_id)
        {
            if (_hooks == null)
            {
                Type tp = GetType();
                if (!_allHooks.TryGetValue(tp, out _hooks))
                {
                    // 初次解析
                    _hooks = new Dictionary<string, Action<object>>(StringComparer.OrdinalIgnoreCase);
                    // 每个类型只调用一次
                    OnHook();
                    _allHooks[tp] = _hooks;
                }
            }

            if (_hooks.Count > 0 && _hooks.TryGetValue(p_id, out var hook))
                return hook;
            return null;
        }

        /// <summary>
        /// 重写该方法用来统一添加当前实体的所有 Hook
        /// </summary>
        protected virtual void OnHook()
        {
        }

        /// <summary>
        /// 注册 Cell 的 Hook 方法
        /// </summary>
        /// <typeparam name="T">Cell值类型</typeparam>
        /// <param name="p_id">Cell.ID</param>
        /// <param name="p_callback">Hook 方法</param>
        protected void Hook<T>(string p_id, Action<T> p_callback)
        {
            if (_cells.Contains(p_id) && p_callback != null)
            {
                // Action<T> 无法转成 Action<object>，只能内部调用
                _hooks[p_id] = new Action<object>((o) => p_callback((T)o));
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
