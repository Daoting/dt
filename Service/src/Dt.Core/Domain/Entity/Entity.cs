#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.EventBus;
using System;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity : Row
    {
        List<DomainEvent> _events;

        /// <summary>
        /// 增加领域事件
        /// </summary>
        /// <param name="p_event"></param>
        /// <param name="p_isRemoteEvent"></param>
        protected void AddDomainEvent(IEvent p_event, bool p_isRemoteEvent = false)
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

        /// <summary>
        /// 反序列化时附加Hook方法
        /// </summary>
        protected override void AttachHook()
        {
            Type tp = GetType();
            foreach (var cell in _cells)
            {
                // 私有方法，SetXXX中的XXX为Cell.ID
                // 一个入参，和Cell.Type相同
                // 无返回值
                var mi = tp.GetMethod("Set" + cell.ID, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly);
                if (mi != null)
                {
                    var input = mi.GetParameters();
                    if (input != null
                        && input.Length == 1
                        && input[0].ParameterType == cell.Type
                        && mi.ReturnType == typeof(void))
                    {
                        cell.Hook = mi;
                    }
                }
            }
        }

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
    }
}
