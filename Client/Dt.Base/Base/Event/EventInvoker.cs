using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Dt.Base
{
    /// <summary>
    /// 负责执行对事件源对象的事件回调
    /// </summary>
    internal class EventInvoker
    {
        /// <summary>
        /// 事件要以过的路由元素列表
        /// </summary>
        List<RouteItem> _routeItemList;

        /// <summary>
        /// 事件源列表
        /// </summary>
        List<SourceItem> _sourceItemList;

        /// <summary>
        /// 事件源对象
        /// </summary>
        DependencyObject _source;

        /// <summary>
        /// 事件参数
        /// </summary>
        BaseRoutedEventArgs _eventArgs;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_element">触发事件的事件源对象</param>
        /// <param name="p_args">事件参数</param>
        internal EventInvoker(DependencyObject p_element, BaseRoutedEventArgs p_args)
        {
            if (p_element == null || p_args == null || p_args.RoutedEvent == null)
            {
                throw new ArgumentNullException("routedEvent");
            }
            _source = p_element;
            _eventArgs = p_args;
            p_args.Source = p_element;
            _routeItemList = new List<RouteItem>();
            _sourceItemList = new List<SourceItem>();
        }

        /// <summary>
        /// 调用所有已订阅的事件回调方法
        /// </summary>
        internal void InvokeHandlers()
        {
            if (_source == null || _eventArgs == null || _eventArgs.Source == null)
            {
                throw new ArgumentNullException("EventManager的InvokeHandlers方法调用异常！");
            }

            // 计算所有的路由对象
            BuildRoute();

            // 若为冒泡事件或直接事件
            if ((_eventArgs.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble)
                || (_eventArgs.RoutedEvent.RoutingStrategy == RoutingStrategy.Direct))
            {
                int endIndex = 0;
                // 顺序执行所有路由对象的回调方法
                for (int i = 0; i < _routeItemList.Count; i++)
                {
                    if (i >= endIndex)
                    {
                        // 设置当前事件源对象
                        _eventArgs.Source = GetBubbleSource(i, out endIndex) ?? _source;
                    }
                    _routeItemList[i].InvokeHandler(_eventArgs);
                }
            }
            else
            {
                int num3;
                int count = _routeItemList.Count;
                for (int j = _routeItemList.Count - 1; j >= 0; j = num3)
                {
                    RouteItem item5 = _routeItemList[j];
                    object target = item5.Target;
                    num3 = j;
                    while (num3 >= 0)
                    {
                        RouteItem item4 = _routeItemList[num3];
                        if (item4.Target != target)
                        {
                            break;
                        }
                        num3--;
                    }
                    for (int k = num3 + 1; k <= j; k++)
                    {
                        if (k < count)
                        {
                            _eventArgs.Source = GetTunnelSource(k, out count) ?? _source;
                        }
                        _routeItemList[k].InvokeHandler(_eventArgs);
                    }
                }
            }

            _eventArgs.Source = _eventArgs.OriginalSource;
            _routeItemList.Clear();
            _sourceItemList.Clear();
        }

        /// <summary>
        /// 计算事件所涉及的所有元素
        /// </summary>
        void BuildRoute()
        {
            if (_eventArgs.RoutedEvent.RoutingStrategy == RoutingStrategy.Direct)
            {
                AddToEventRoute(_source);
            }
            else
            {
                int nestLevel = 0;
                DependencyObject element = _source;
                while (element != null)
                {
                    if (nestLevel++ > 0x100)
                    {
                        throw new InvalidOperationException("递归调用层数过多！");
                    }
                    if (element != null)
                    {
                        AddToEventRoute(element);
                        element = FindParent(element as FrameworkElement);
                    }
                    if (element == _eventArgs.Source)
                    {
                        _sourceItemList.Add(new SourceItem(_routeItemList.Count, element));
                    }
                }
            }
        }

        /// <summary>
        /// 添加给定元素的
        /// </summary>
        /// <param name="element"></param>
        void AddToEventRoute(DependencyObject element)
        {
            DependencyObjectType dependencyType = Attached.GetDependencyObjectType(element);
            if (dependencyType == null)
            {
                dependencyType = DependencyObjectType.FromSystemType(element.GetType());
                Attached.SetDependencyObjectType(element, dependencyType);
            }
            for (RoutedEventHandlerInfoList list = EventManager.GetDTypedClassListeners(dependencyType, _eventArgs.RoutedEvent); list != null; list = list.Next)
            {
                for (int i = 0; i < list.Handlers.Length; i++)
                {
                    RouteItem item = new RouteItem(element, new RoutedEventHandlerInfo(list.Handlers[i].Handler, list.Handlers[i].InvokeHandledEventsToo));
                    _routeItemList.Add(item);
                }
            }

            ObjectHandlersStore eventHandlersStore = Attached.GetEventHandlersStore(element);
            if (eventHandlersStore != null)
            {
                List<RoutedEventHandlerInfo> list2 = eventHandlersStore[_eventArgs.RoutedEvent];
                if (list2 != null)
                {
                    for (int j = 0; j < list2.Count; j++)
                    {
                        RouteItem item = new RouteItem(element, new RoutedEventHandlerInfo(list2[j].Handler, list2[j].InvokeHandledEventsToo));
                        _routeItemList.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 获取冒泡源
        /// </summary>
        /// <param name="index"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        object GetBubbleSource(int index, out int endIndex)
        {
            if (_sourceItemList.Count == 0)
            {
                endIndex = _routeItemList.Count;
                return null;
            }
            SourceItem item7 = _sourceItemList[0];
            if (index < item7.StartIndex)
            {
                SourceItem item6 = _sourceItemList[0];
                endIndex = item6.StartIndex;
                return null;
            }
            for (int i = 0; i < (_sourceItemList.Count - 1); i++)
            {
                SourceItem item5 = _sourceItemList[i];
                if (index >= item5.StartIndex)
                {
                    SourceItem item4 = _sourceItemList[i + 1];
                    if (index < item4.StartIndex)
                    {
                        SourceItem item3 = _sourceItemList[i + 1];
                        endIndex = item3.StartIndex;
                        SourceItem item2 = _sourceItemList[i];
                        return item2.Source;
                    }
                }
            }
            endIndex = _routeItemList.Count;
            SourceItem item = _sourceItemList[_sourceItemList.Count - 1];
            return item.Source;
        }

        /// <summary>
        /// 获取隧道策略的源对象
        /// </summary>
        /// <param name="index"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        object GetTunnelSource(int index, out int startIndex)
        {
            if (_sourceItemList.Count == 0)
            {
                startIndex = 0;
                return null;
            }
            SourceItem item7 = _sourceItemList[0];
            if (index < item7.StartIndex)
            {
                startIndex = 0;
                return null;
            }
            for (int i = 0; i < (_sourceItemList.Count - 1); i++)
            {
                SourceItem item6 = _sourceItemList[i];
                if (index >= item6.StartIndex)
                {
                    SourceItem item5 = _sourceItemList[i + 1];
                    if (index < item5.StartIndex)
                    {
                        SourceItem item4 = _sourceItemList[i];
                        startIndex = item4.StartIndex;
                        SourceItem item3 = _sourceItemList[i];
                        return item3.Source;
                    }
                }
            }
            SourceItem item2 = _sourceItemList[_sourceItemList.Count - 1];
            startIndex = item2.StartIndex;
            SourceItem item = _sourceItemList[_sourceItemList.Count - 1];
            return item.Source;
        }

        static DependencyObject FindParent(FrameworkElement item)
        {
            if (item == null)
                return item;

            if (item is IBubbleItem bi)
                return bi.GetBubbleParent();

            FrameworkElement parent = VisualTreeHelper.GetParent(item) as FrameworkElement;
            if (parent == null)
                parent = item.Parent as FrameworkElement;
            return parent;
        }

        #region RouteItem
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct RouteItem
        {
            RoutedEventHandlerInfo _routedEventHandlerInfo;

            internal RouteItem(object target, RoutedEventHandlerInfo routedEventHandlerInfo)
            {
                this = new RouteItem();
                Target = target;
                _routedEventHandlerInfo = routedEventHandlerInfo;
            }

            internal void InvokeHandler(BaseRoutedEventArgs routedEventArgs)
            {
                _routedEventHandlerInfo.InvokeHandler(Target, routedEventArgs);
            }

            internal object Target { get; set; }

            public static bool operator ==(RouteItem routeItem1, RouteItem routeItem2)
            {
                return routeItem1.Equals(routeItem2);
            }

            public static bool operator !=(RouteItem routeItem1, RouteItem routeItem2)
            {
                return !routeItem1.Equals(routeItem2);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object o)
            {
                return Equals((RouteItem)o);
            }

            public bool Equals(RouteItem routeItem)
            {
                return (routeItem.Target == Target && routeItem._routedEventHandlerInfo == _routedEventHandlerInfo);
            }

        }

        #endregion

        #region SourceItem
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SourceItem
        {
            internal SourceItem(int startIndex, object source)
            {
                this = new SourceItem();
                StartIndex = startIndex;
                Source = source;
            }

            internal int StartIndex { get; set; }

            internal object Source { get; set; }

            public static bool operator ==(SourceItem sourceItem1, SourceItem sourceItem2)
            {
                return sourceItem1.Equals(sourceItem2);
            }

            public static bool operator !=(SourceItem sourceItem1, SourceItem sourceItem2)
            {
                return !sourceItem1.Equals(sourceItem2);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object o)
            {
                return Equals((SourceItem)o);
            }

            public bool Equals(SourceItem sourceItem)
            {
                return ((sourceItem.StartIndex == StartIndex) && (sourceItem.Source == Source));
            }
        }
        #endregion
    }

    /// <summary>
    /// 冒泡事件的元素
    /// </summary>
    public interface IBubbleItem
    {
        /// <summary>
        /// 冒泡事件时获取父元素
        /// </summary>
        /// <returns></returns>
        FrameworkElement GetBubbleParent();
    }
}
