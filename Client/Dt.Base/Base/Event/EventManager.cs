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
using Windows.UI.Xaml.Controls;
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
        /// 类型对事件的回调字典
        /// </summary>
        static Dictionary<DependencyObjectType, ClassHandlersStore> _classHandlers = new Dictionary<DependencyObjectType, ClassHandlersStore>();

        /// <summary>
        /// 原为DependencyObject，修改为Control
        /// </summary>
        static DependencyObjectType _dependencyObjectType = DependencyObjectType.FromSystemType(typeof(Control));

        static object _synchronized = new object();
        #endregion

        /// <summary>
        /// 在内部事件系统中注册新路由事件，必须在静态构造方法中调用，已支持uno
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
        /// 触发路由事件
        /// </summary>
        /// <param name="element">事件源对象</param>
        /// <param name="args">事件参数</param>
        internal static void RaiseEvent(this Control element, BaseRoutedEventArgs args)
        {
            if (args == null || element == null)
            {
                throw new ArgumentNullException("EventManage的RaiseEvent方法参数不能为空！");
            }
            EventInvoker route = new EventInvoker(element, args);
            route.InvokeHandlers();
        }

        /// <summary>
        /// 将指定的路由事件添加到对应的字典列表中
        /// </summary>
        /// <param name="routedEvent">路由事件</param>
        /// <param name="ownerType">路由事件的所有者</param>
        internal static void AddOwner(BaseRoutedEvent routedEvent, Type ownerType)
        {
            // 类型需继承自Control
            if (!ownerType.IsSubclassOf(typeof(Control)))
            {
                throw new Exception("因支持uno，路由事件所有者需继承自Control");
            }

            List<BaseRoutedEvent> list;
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
            for (var type = DependencyObjectType.FromSystemType(ownerType); type != null; type = includeSupers ? type.BaseType : null)
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

