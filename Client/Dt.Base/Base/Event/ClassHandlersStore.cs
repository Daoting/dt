#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 管理某类型的已注册事件回调
    /// </summary>
    internal class ClassHandlersStore
    {
        ItemStructList<ClassHandlers> _eventHandlersList;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_size">路由事件回调列表大小</param>
        internal ClassHandlersStore(int p_size)
        {
            _eventHandlersList = new ItemStructList<ClassHandlers>(p_size);
        }

        /// <summary>
        /// 关联路由事件和回调列表记录在内部列表中
        /// </summary>
        /// <param name="p_routedEvent"></param>
        /// <param name="p_handlers"></param>
        /// <returns></returns>
        internal int CreateHandlersLink(BaseRoutedEvent p_routedEvent, RoutedEventHandlerInfoList p_handlers)
        {
            ClassHandlers item = new ClassHandlers();
            item.RoutedEvent = p_routedEvent;
            item.Handlers = p_handlers;
            item.HasSelfHandlers = false;
            _eventHandlersList.Add(item);
            return (_eventHandlersList.Count - 1);
        }

        /// <summary>
        /// 向已有的事件处理列表中添加新处理方法原型
        /// </summary>
        /// <param name="p_index"></param>
        /// <param name="p_handler"></param>
        /// <param name="p_handledEventsToo"></param>
        /// <returns></returns>
        internal RoutedEventHandlerInfoList AddToExistingHandlers(int p_index, Delegate p_handler, bool p_handledEventsToo)
        {
            RoutedEventHandlerInfo info = new RoutedEventHandlerInfo(p_handler, p_handledEventsToo);
            RoutedEventHandlerInfoList handlers = _eventHandlersList.List[p_index].Handlers;
            if ((handlers == null) || !_eventHandlersList.List[p_index].HasSelfHandlers)
            {
                handlers = new RoutedEventHandlerInfoList();
                handlers.Handlers = new RoutedEventHandlerInfo[] { info };
                handlers.Next = _eventHandlersList.List[p_index].Handlers;
                _eventHandlersList.List[p_index].Handlers = handlers;
                _eventHandlersList.List[p_index].HasSelfHandlers = true;
                return handlers;
            }
            int length = handlers.Handlers.Length;
            RoutedEventHandlerInfo[] destinationArray = new RoutedEventHandlerInfo[length + 1];
            Array.Copy(handlers.Handlers, 0, destinationArray, 0, length);
            destinationArray[length] = info;
            handlers.Handlers = destinationArray;
            return handlers;
        }

        /// <summary>
        /// 根据路由事件类型查找在列表中的索引
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <returns>-1 表未找到</returns>
        internal int GetHandlersIndex(BaseRoutedEvent routedEvent)
        {
            for (int i = 0; i < _eventHandlersList.Count; i++)
            {
                if (_eventHandlersList.List[i].RoutedEvent == routedEvent)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 返回指定索引处的回调信息列表
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal RoutedEventHandlerInfoList GetExistingHandlers(int index)
        {
            return _eventHandlersList.List[index].Handlers;
        }

        /// <summary>
        /// 更新子类型之间的链接
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="baseClassListeners"></param>
        internal void UpdateSubClassHandlers(BaseRoutedEvent routedEvent, RoutedEventHandlerInfoList baseClassListeners)
        {
            int handlersIndex = GetHandlersIndex(routedEvent);
            if (handlersIndex != -1)
            {
                bool hasSelfHandlers = _eventHandlersList.List[handlersIndex].HasSelfHandlers;
                RoutedEventHandlerInfoList handlers = hasSelfHandlers ? _eventHandlersList.List[handlersIndex].Handlers.Next : _eventHandlersList.List[handlersIndex].Handlers;
                bool flag = false;
                if (handlers != null)
                {
                    if ((baseClassListeners.Next != null) && baseClassListeners.Next.Contains(handlers))
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    if (hasSelfHandlers)
                    {
                        _eventHandlersList.List[handlersIndex].Handlers.Next = baseClassListeners;
                    }
                    else
                    {
                        _eventHandlersList.List[handlersIndex].Handlers = baseClassListeners;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 记录某路由事件的回调列表
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ClassHandlers
    {
        /// <summary>
        /// 路由事件
        /// </summary>
        internal BaseRoutedEvent RoutedEvent;

        /// <summary>
        /// 回调方法原型信息列表
        /// </summary>
        internal RoutedEventHandlerInfoList Handlers;

        /// <summary>
        /// 是否始终回调方法
        /// </summary>
        internal bool HasSelfHandlers;

        public static bool operator ==(ClassHandlers classHandlers1, ClassHandlers classHandlers2)
        {
            return classHandlers1.Equals(classHandlers2);
        }

        public static bool operator !=(ClassHandlers classHandlers1, ClassHandlers classHandlers2)
        {
            return !classHandlers1.Equals(classHandlers2);
        }

        public override bool Equals(object o)
        {
            return Equals((ClassHandlers)o);
        }

        public bool Equals(ClassHandlers classHandlers)
        {
            return ((classHandlers.RoutedEvent == RoutedEvent) && (classHandlers.Handlers == Handlers));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ItemStructList<T>
    {
        public T[] List;
        public int Count;
        public ItemStructList(int capacity)
        {
            List = new T[capacity];
            Count = 0;
        }

        public void EnsureIndex(int index)
        {
            int delta = (index + 1) - Count;
            if (delta > 0)
            {
                Add(delta);
            }
        }

        public bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < Count));
        }

        public int IndexOf(T value)
        {
            for (int i = 0; i < Count; i++)
            {
                if (List[i].Equals(value))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(T value)
        {
            return (IndexOf(value) != -1);
        }

        public void Add(T item)
        {
            int index = Add(1, false);
            List[index] = item;
            Count++;
        }

        public void Add(ref T item)
        {
            int index = Add(1, false);
            List[index] = item;
            Count++;
        }

        public int Add()
        {
            return Add(1, true);
        }

        public int Add(int delta)
        {
            return Add(delta, true);
        }

        public void Sort()
        {
            if (List != null)
            {
                Array.Sort<T>(List, 0, Count);
            }
        }

        public void AppendTo(ref ItemStructList<T> destinationList)
        {
            for (int i = 0; i < Count; i++)
            {
                destinationList.Add(ref List[i]);
            }
        }

        public T[] ToArray()
        {
            T[] destinationArray = new T[Count];
            Array.Copy(List, 0, destinationArray, 0, Count);
            return destinationArray;
        }

        public void Clear()
        {
            Array.Clear(List, 0, Count);
            Count = 0;
        }

        public void Remove(T value)
        {
            int index = IndexOf(value);
            if (index != -1)
            {
                Array.Copy(List, index + 1, List, index, (Count - index) - 1);
                Array.Clear(List, Count - 1, 1);
                Count--;
            }
        }

        int Add(int delta, bool incrCount)
        {
            if (List != null)
            {
                if ((Count + delta) > List.Length)
                {
                    T[] array = new T[Math.Max((int)(List.Length * 2), (int)(Count + delta))];
                    List.CopyTo(array, 0);
                    List = array;
                }
            }
            else
            {
                List = new T[Math.Max(delta, 2)];
            }
            int count = Count;
            if (incrCount)
            {
                Count += delta;
            }
            return count;
        }
    }
}
