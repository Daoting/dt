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
using System.Reflection;
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
        Dictionary<string, object> _originalVal;
        List<IEvent> _localEvents;
        List<IEvent> _remoteEvents;

        /// <summary>
        /// 启动跟踪属性值变化，Update时只更新变化的列
        /// </summary>
        public void StartTrack()
        {
            if (_originalVal == null)
                _originalVal = new Dictionary<string, object>();
            else
                _originalVal.Clear();

            Type type = GetType();
            var tbl = type.GetCustomAttribute<TblAttribute>(false);
            if (tbl == null || string.IsNullOrEmpty(tbl.Name))
                throw new Exception($"实体{type.Name}缺少映射表设置！");

            // 根据表结构保存原始值
            var schema = DbSchema.GetTableSchema(tbl.Name.ToLower());
            foreach (var col in schema.Columns)
            {
                var pi = type.GetProperty(col.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi != null)
                    _originalVal[col.Name] = pi.GetValue(this);
            }
        }

        /// <summary>
        /// 是否已启动跟踪
        /// </summary>
        /// <returns></returns>
        public bool IsTracking()
        {
            return _originalVal != null;
        }

        /// <summary>
        /// 判断属性值是否和原始值相同
        /// </summary>
        /// <param name="p_propName">属性名</param>
        /// <param name="p_val">当前值</param>
        /// <returns></returns>
        public bool IsOriginalVal(string p_propName, object p_val)
        {
            object orgVal;
            if (_originalVal == null || !_originalVal.TryGetValue(p_propName, out orgVal))
                return false;

            if (orgVal == null)
                return p_val == null;

            return orgVal.Equals(p_val);
        }

        /// <summary>
        /// 停止跟踪
        /// </summary>
        public void StopTrack()
        {
            if (_originalVal != null)
            {
                _originalVal.Clear();
                _originalVal = null;
            }
        }

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
