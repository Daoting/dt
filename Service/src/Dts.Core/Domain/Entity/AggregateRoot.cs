#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace Dts.Core
{
    public abstract class AggregateRoot : Entity,
        IAggregateRoot,
        IGeneratesDomainEvents,
        IHasConcurrencyStamp
    {
        public virtual Dictionary<string, object> ExtraProperties { get; protected set; }

        public virtual string ConcurrencyStamp { get; set; }

        private readonly ICollection<object> _localEvents = new Collection<object>();
        private readonly ICollection<object> _distributedEvents = new Collection<object>();

        protected AggregateRoot()
        {
            ExtraProperties = new Dictionary<string, object>();
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        protected virtual void AddLocalEvent(object eventData)
        {
            _localEvents.Add(eventData);
        }

        protected virtual void AddDistributedEvent(object eventData)
        {
            _distributedEvents.Add(eventData);
        }

        public virtual IEnumerable<object> GetLocalEvents()
        {
            return _localEvents;
        }

        public virtual IEnumerable<object> GetDistributedEvents()
        {
            return _distributedEvents;
        }

        public virtual void ClearLocalEvents()
        {
            _localEvents.Clear();
        }

        public virtual void ClearDistributedEvents()
        {
            _distributedEvents.Clear();
        }
    }

    public abstract class AggregateRoot<TKey> : Entity<TKey>,
        IAggregateRoot<TKey>,
        IGeneratesDomainEvents,
        IHasConcurrencyStamp
    {
        public virtual Dictionary<string, object> ExtraProperties { get; protected set; }

        public virtual string ConcurrencyStamp { get; set; }

        private readonly ICollection<object> _localEvents = new Collection<object>();
        private readonly ICollection<object> _distributedEvents = new Collection<object>();

        protected AggregateRoot()
        {
            ExtraProperties = new Dictionary<string, object>();
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        protected AggregateRoot(TKey id)
            : base(id)
        {
            ExtraProperties = new Dictionary<string, object>();
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        protected virtual void AddLocalEvent(object eventData)
        {
            _localEvents.Add(eventData);
        }

        protected virtual void AddDistributedEvent(object eventData)
        {
            _distributedEvents.Add(eventData);
        }

        public virtual IEnumerable<object> GetLocalEvents()
        {
            return _localEvents;
        }

        public virtual IEnumerable<object> GetDistributedEvents()
        {
            return _distributedEvents;
        }

        public virtual void ClearLocalEvents()
        {
            _localEvents.Clear();
        }

        public virtual void ClearDistributedEvents()
        {
            _distributedEvents.Clear();
        }
    }
}
