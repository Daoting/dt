#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Runtime.InteropServices;
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 路由事件管理静态类，提供路由事件的相关方法，
    /// 如注册事件，注册类型回调、事件回调，移除回调，触发事件等
    /// </summary>
    public static class EventManager
    {
        #region 成员变量
        /// <summary>
        /// 以依赖类型声明为键的事件列表
        /// </summary>
        static Dictionary<DependencyObjectType, List<BaseRoutedEvent>> _dependencyEvents = new Dictionary<DependencyObjectType, List<BaseRoutedEvent>>();

        /// <summary>
        /// 非DependencyObject子类型的事件列表
        /// 类型声明为键，值为该类型的所有事件列表
        /// </summary>
        static Dictionary<Type, List<BaseRoutedEvent>> _classEvents = new Dictionary<Type, List<BaseRoutedEvent>>();

        /// <summary>
        /// 类型对事件的回调字典
        /// </summary>
        static Dictionary<DependencyObjectType, ClassHandlersStore> _classHandlers = new Dictionary<DependencyObjectType, ClassHandlersStore>();

        static DependencyObjectType _dependencyObjectType = DependencyObjectType.FromSystemType(typeof(DependencyObject));

        /// <summary>
        /// 路由事件索引记数器
        /// </summary>
        static int _routedEventsCount;
        static object _synchronized = new object();

        #endregion

        /// <summary>
        /// 在uno中异常！！！
        /// 在内部事件系统中注册新路由事件，必须在静态构造方法中调用
        /// </summary>
        /// <param name="name">
        /// 路由事件名称，必须保证在同一类型内唯一，不可为null或空
        /// </param>
        /// <param name="routingStrategy">
        /// 路由策略，enum
        /// </param>
        /// <param name="handlerType">
        /// 回调方法原型，不可为空
        /// </param>
        /// <param name="ownerType">
        /// 路由事件的所有者，不可为空
        /// </param>
        /// <returns>
        /// 返回新注册的路由事件，该对象可以作为类型的静态成员，
        /// 通过this.AddHandler(HoverEvent, value);的方式附加事件
        /// </returns>
        public static BaseRoutedEvent RegisterRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            CheckParameters(name, routingStrategy, handlerType, ownerType);
            lock (_synchronized)
            {
                BaseRoutedEvent routedEvent = new BaseRoutedEvent(name, routingStrategy, handlerType, ownerType);
                _routedEventsCount++;
                AddOwner(routedEvent, ownerType);
                return routedEvent;
            }
        }

        /// <summary>
        /// 为指定的路由事件注册类型的回调方法，在静态构造方法中注册，为该类型所有
        /// </summary>
        /// <param name="classType">包含回调方法的类型，必须为UIElement的子类型</param>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="handler">回调方法原型</param>
        /// <param name="handledEventsToo">
        /// true 表始终调用处理事件即使标记已处理，false 则根据标记进行处理
        /// </param>
        public static void RegisterClassHandler(Type classType, BaseRoutedEvent routedEvent, Delegate handler, bool handledEventsToo = false)
        {
            CheckParameters(classType, routedEvent, handler);
            ClassHandlersStore store;
            int index;
            DependencyObjectType dependencyType = DependencyObjectType.FromSystemType(classType);
            GetDTypedClassListeners(dependencyType, routedEvent, out store, out index);
            lock (_synchronized)
            {
                RoutedEventHandlerInfoList baseClassListeners = store.AddToExistingHandlers(index, handler, handledEventsToo);
                foreach (KeyValuePair<DependencyObjectType, ClassHandlersStore> item in _classHandlers)
                {
                    if (item.Key.IsSubclassOf(dependencyType))
                    {
                        item.Value.UpdateSubClassHandlers(routedEvent, baseClassListeners);
                    }
                }
            }
        }

        /// <summary>
        /// 注册依赖对象的事件回调，该回调为对象实例所有
        /// </summary>
        /// <param name="element">要注册的依赖对象</param>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="handler">回调方法</param>
        /// <param name="handledEventsToo">是否始终回调</param>
        internal static void RegisterObjectHandler(DependencyObject element, BaseRoutedEvent routedEvent, Delegate handler, bool handledEventsToo)
        {
            CheckParameters(routedEvent, handler);
            ObjectHandlersStore store = Attached.GetEventHandlersStore(element);
            if (store == null)
            {
                store = new ObjectHandlersStore();
                Attached.SetEventHandlersStore(element, store);
            }
            store.AddRoutedEventHandler(routedEvent, handler, handledEventsToo);
        }

        /// <summary>
        /// 触发路由事件
        /// </summary>
        /// <param name="element">事件源对象</param>
        /// <param name="args">事件参数</param>
        internal static void RaiseEvent(DependencyObject element, BaseRoutedEventArgs args)
        {
            if (args == null || element == null)
            {
                throw new ArgumentNullException("EventManage的RaiseEvent方法参数不能为空！");
            }
            EventInvoker route = new EventInvoker(element, args);
            route.InvokeHandlers();
        }

        /// <summary>
        /// 为指定的依赖对象移除事件回调方法
        /// </summary>
        /// <param name="element">要移除回调方法的对象</param>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="handler">回调方法</param>
        internal static void RemoveEventHandler(DependencyObject element, BaseRoutedEvent routedEvent, Delegate handler)
        {
            CheckParameters(routedEvent, handler);
            ObjectHandlersStore eventHandlersStore = Attached.GetEventHandlersStore(element);
            if (eventHandlersStore != null)
            {
                eventHandlersStore.RemoveRoutedEventHandler(routedEvent, handler);
                if (eventHandlersStore.Count == 0)
                {
                    Attached.SetEventHandlersStore(element, null);
                }
            }
        }

        /// <summary>
        /// 将指定的路由事件添加到对应的字典列表中
        /// </summary>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="ownerType">路由事件的所有者</param>
        internal static void AddOwner(BaseRoutedEvent routedEvent, Type ownerType)
        {
            // 该类型的事件种类列表
            List<BaseRoutedEvent> list;

            // 若为依赖项类型
            if (ownerType == typeof(DependencyObject) || ownerType.GetTypeInfo().IsSubclassOf(typeof(DependencyObject)))
            {
                DependencyObjectType type = DependencyObjectType.FromSystemType(ownerType);
                if (_dependencyEvents.ContainsKey(type))
                {
                    list = _dependencyEvents[type]; ;
                }
                else
                {
                    list = new List<BaseRoutedEvent>();
                    _dependencyEvents[type] = list;
                }
            }
            else
            {
                // 非DependencyObject子类型的事件列表
                if (_classEvents.ContainsKey(ownerType))
                {
                    list = _classEvents[ownerType];
                }
                else
                {
                    list = new List<BaseRoutedEvent>();
                    _classEvents[ownerType] = list;
                }
            }

            if (!list.Contains(routedEvent))
            {
                list.Add(routedEvent);
            }
        }

        /// <summary>
        /// 获取某类型对某事件的事件处理列表
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="routedEvent">路由事件</param>
        /// <returns></returns>
        internal static RoutedEventHandlerInfoList GetDTypedClassListeners(DependencyObjectType dependencyType, BaseRoutedEvent routedEvent)
        {
            int num;
            ClassHandlersStore store;
            return GetDTypedClassListeners(dependencyType, routedEvent, out store, out num);
        }

        /// <summary>
        /// 获取某类型对某事件的事件处理列表
        /// </summary>
        /// <param name="dependencyType"></param>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="classListenersLists"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static RoutedEventHandlerInfoList GetDTypedClassListeners(DependencyObjectType dependencyType, BaseRoutedEvent routedEvent, out ClassHandlersStore classListenersLists, out int index)
        {
            classListenersLists = null;
            if (_classHandlers.ContainsKey(dependencyType))
            {
                classListenersLists = _classHandlers[dependencyType];
                if (classListenersLists != null)
                {
                    index = classListenersLists.GetHandlersIndex(routedEvent);
                    if (index != -1)
                    {
                        return classListenersLists.GetExistingHandlers(index);
                    }
                }
            }

            // 若当前字典中没有
            lock (_synchronized)
            {
                DependencyObjectType baseType = dependencyType;
                ClassHandlersStore store = null;
                RoutedEventHandlerInfoList handlers = null;
                int handlersIndex = -1;
                while (handlersIndex == -1 && baseType.ID != _dependencyObjectType.ID)
                {
                    baseType = baseType.BaseType;
                    if (_classHandlers.ContainsKey(baseType))
                    {
                        store = _classHandlers[baseType];
                        if (store != null)
                        {
                            handlersIndex = store.GetHandlersIndex(routedEvent);
                            if (handlersIndex != -1)
                            {
                                handlers = store.GetExistingHandlers(handlersIndex);
                            }
                        }
                    }
                }
                if (classListenersLists == null)
                {
                    if (dependencyType.SystemType == typeof(UIElement))
                    {
                        classListenersLists = new ClassHandlersStore(80);
                    }
                    else
                    {
                        classListenersLists = new ClassHandlersStore(1);
                    }
                    _classHandlers[dependencyType] = classListenersLists;
                }
                index = classListenersLists.CreateHandlersLink(routedEvent, handlers);
                return handlers;
            }
        }

        /// <summary>
        /// 查询给定类型给定事件名称的路由事件
        /// </summary>
        /// <param name="name">路由事件名称</param>
        /// <param name="ownerType">路由事件的所有者</param>
        /// <param name="includeSupers"></param>
        /// <returns></returns>
        internal static BaseRoutedEvent GetRoutedEventFromName(string name, Type ownerType, bool includeSupers)
        {
            if ((ownerType != typeof(DependencyObject)) && !ownerType.GetTypeInfo().IsSubclassOf(typeof(DependencyObject)))
            {
                while (ownerType != null)
                {
                    if (_classEvents.ContainsKey(ownerType))
                    {
                        List<BaseRoutedEvent> list = _classEvents[ownerType];
                        if (list != null)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                BaseRoutedEvent event2 = list[i];
                                if (event2.Name.Equals(name))
                                {
                                    return event2;
                                }
                            }
                        }
                    }
                    ownerType = includeSupers ? ownerType.GetTypeInfo().BaseType : null;
                }
            }
            else
            {
                for (DependencyObjectType type = DependencyObjectType.FromSystemType(ownerType); type != null; type = includeSupers ? type.BaseType : null)
                {
                    if (_dependencyEvents.ContainsKey(type))
                    {
                        List<BaseRoutedEvent> list = _dependencyEvents[type];
                        if (list != null)
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                BaseRoutedEvent re = list[j];
                                if (re.Name.Equals(name))
                                {
                                    return re;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有的路由事件列表
        /// </summary>
        /// <returns></returns>
        internal static BaseRoutedEvent[] GetRoutedEvents()
        {
            BaseRoutedEvent[] eventArray;
            lock (_synchronized)
            {
                eventArray = new BaseRoutedEvent[_routedEventsCount];
                int index = 0;
                foreach (KeyValuePair<DependencyObjectType, List<BaseRoutedEvent>> item in _dependencyEvents)
                {
                    foreach (BaseRoutedEvent re in item.Value)
                    {
                        if (Array.IndexOf<BaseRoutedEvent>(eventArray, re) < 0)
                        {
                            eventArray[index++] = re;
                        }
                    }
                }

                foreach (KeyValuePair<Type, List<BaseRoutedEvent>> item in _classEvents)
                {
                    foreach (BaseRoutedEvent re in item.Value)
                    {
                        if (Array.IndexOf<BaseRoutedEvent>(eventArray, re) < 0)
                        {
                            eventArray[index++] = re;
                        }
                    }
                }
            }
            return eventArray;
        }

        /// <summary>
        /// 获取给定类型的所有路由事件
        /// </summary>
        /// <param name="ownerType">路由事件的所有者</param>
        /// <returns></returns>
        internal static BaseRoutedEvent[] GetRoutedEventsForOwner(Type ownerType)
        {
            if (ownerType == typeof(DependencyObject) || ownerType.GetTypeInfo().IsSubclassOf(typeof(DependencyObject)))
            {
                DependencyObjectType type = DependencyObjectType.FromSystemType(ownerType);
                if (_dependencyEvents.ContainsKey(type))
                {
                    List<BaseRoutedEvent> list = _dependencyEvents[type];
                    if (list != null)
                    {
                        return list.ToArray();
                    }
                }
            }
            else if (_classEvents.ContainsKey(ownerType))
            {
                List<BaseRoutedEvent> list = _classEvents[ownerType];
                if (list != null)
                {
                    return list.ToArray();
                }
            }
            return null;
        }

        #region 内部方法
        static void CheckParameters(BaseRoutedEvent routedEvent, Delegate handler)
        {
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException("事件回调方法原型不匹配！");
            }
        }

        static void CheckParameters(Type classType, BaseRoutedEvent routedEvent, Delegate handler)
        {
            if (classType == null)
            {
                throw new ArgumentNullException("classType");
            }
            if (routedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (!typeof(UIElement).GetTypeInfo().IsAssignableFrom(classType.GetTypeInfo()))
            {
                throw new ArgumentException("ClassTypeIllegal");
            }
            if (!routedEvent.IsLegalHandler(handler))
            {
                throw new ArgumentException("HandlerTypeIllegal");
            }
        }

        static void CheckParameters(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (((routingStrategy != RoutingStrategy.Tunnel) && (routingStrategy != RoutingStrategy.Bubble)) && (routingStrategy != RoutingStrategy.Direct))
            {
                object[] obj = new object[] { "routingStrategy", ((int)routingStrategy).ToString(CultureInfo.CurrentCulture), "RoutingStrategy" };
                throw new ArgumentException("InvalidEnumArgument");
            }
            if (handlerType == null)
            {
                throw new ArgumentNullException("handlerType");
            }
            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }
            if (GetRoutedEventFromName(name, ownerType, false) != null)
            {
                throw new ArgumentException("DuplicateEventName");
            }
        }

        #endregion


    }
}

