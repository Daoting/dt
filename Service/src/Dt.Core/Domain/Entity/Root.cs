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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Domain
{
    /// <summary>
    /// 包含"ID"主键的聚合根基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Root<TKey> : Entity<TKey>, IRoot<TKey>
    {
        List<IEvent> _localEvents;
        List<IEvent> _remoteEvents;

        protected void AddLocalEvent(IEvent eventData)
        {
            if (_localEvents == null)
                _localEvents = new List<IEvent>();
            _localEvents.Add(eventData);
        }

        protected void AddRemoteEvent(IEvent eventData)
        {
            if (_remoteEvents == null)
                _remoteEvents = new List<IEvent>();
            _remoteEvents.Add(eventData);
        }

        public IReadOnlyCollection<IEvent> GetLocalEvents()
        {
            return _localEvents?.AsReadOnly();
        }

        public IReadOnlyCollection<IEvent> GetRemoteEvents()
        {
            return _remoteEvents?.AsReadOnly();
        }

        public void ClearLocalEvents()
        {
            _localEvents?.Clear();
        }

        public void ClearRemoteEvents()
        {
            _remoteEvents?.Clear();
        }
    }

    /// <summary>
    /// 主键ID为long类型的聚合根基类
    /// </summary>
    public abstract class Root : Root<long>
    {

    }
}
