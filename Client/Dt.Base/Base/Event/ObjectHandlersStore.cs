#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 附加在依赖对象之上，用于存储该对象的所有事件处理详情
    /// </summary>
    internal class ObjectHandlersStore
    {
        /// <summary>
        /// 以事件种类为键
        /// </summary>
        Dictionary<BaseRoutedEvent, List<RoutedEventHandlerInfo>> _entries;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObjectHandlersStore()
        {
            _entries = new Dictionary<BaseRoutedEvent, List<RoutedEventHandlerInfo>>();
        }

        /// <summary>
        /// 添加一个对给定事件的回调方法
        /// </summary>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="handler">回调方法</param>
        /// <param name="handledEventsToo">是否始终执行</param>
        public void AddRoutedEventHandler(BaseRoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            RoutedEventHandlerInfo info = new RoutedEventHandlerInfo(handler, handledEventsToo);
            List<RoutedEventHandlerInfo> list = this[routedEvent];
            if (list == null)
            {
                list = new List<RoutedEventHandlerInfo>();
                _entries[routedEvent] = list;
            }
            list.Add(info);
        }

        /// <summary>
        /// 从当前对象上移除一个事件回调
        /// </summary>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="handler">回调方法</param>
        public void RemoveRoutedEventHandler(BaseRoutedEvent routedEvent, Delegate handler)
        {
            List<RoutedEventHandlerInfo> list = this[routedEvent];
            if (list != null && list.Count > 0)
            {
                if (list.Count == 1)
                {
                    if (list[0].Handler == handler)
                    {
                        _entries[routedEvent] = null;
                        return;
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    RoutedEventHandlerInfo info = list[i];
                    if (info.Handler == handler)
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 当前对象字典中的记录数
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        /// 获取当前对象的某事件的回调方法列表
        /// </summary>
        /// <param name="key">以事件种类为键</param>
        /// <returns></returns>
        public List<RoutedEventHandlerInfo> this[BaseRoutedEvent key]
        {
            get
            {
                if (_entries.ContainsKey(key))
                {
                    return (List<RoutedEventHandlerInfo>) _entries[key];
                }
                return null;
            }
        }
    }
}

