#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;

using System.Diagnostics.CodeAnalysis;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 路由事件回调方法原型
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void BaseRoutedEventHandler(object sender, BaseRoutedEventArgs e);

    /// <summary>
    /// 包含与路由事件关联的状态信息和事件数据
    /// </summary>
    public class BaseRoutedEventArgs : RoutedEventArgs
    {
        #region 成员变量
        bool _handled;
        bool _invokingHandler;
        object _originalSource;
        BaseRoutedEvent _routedEvent;
        object _source;

        #endregion

        #region 构造方法
        /// <summary>
        /// 给定的路由事件和事件源对象构造实例
        /// </summary>
        /// <param name="routedEvent">
        /// 路由事件
        /// </param>
        /// <param name="source">
        /// 事件源对象
        /// </param>
        public BaseRoutedEventArgs(BaseRoutedEvent routedEvent, object source)
        {
            _routedEvent = routedEvent;
            _source = _originalSource = source;
        }

        /// <summary>
        /// 给定的路由事件构造实例
        /// </summary>
        /// <param name="routedEvent">
        /// 路由事件
        /// </param>
        public BaseRoutedEventArgs(BaseRoutedEvent routedEvent)
            : this(routedEvent, null)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public BaseRoutedEventArgs()
        {
        }

        #endregion

        /// <summary>
        /// 执行路由事件的回调方法
        /// </summary>
        /// <param name="handler">回调方法</param>
        /// <param name="target">目标对象</param>
        internal void InvokeHandler(Delegate handler, object target)
        {
            InvokingHandler = true;
            try
            {
                InvokeEventHandler(handler, target);
            }
            finally
            {
                InvokingHandler = false;
            }
        }

        /// <summary>
        /// 重置路由事件类型
        /// </summary>
        /// <param name="newRoutedEvent">当前参数对象所关联的新的RoutedEvent对象</param>
        internal void OverrideRoutedEvent(BaseRoutedEvent newRoutedEvent)
        {
            _routedEvent = newRoutedEvent;
        }

        /// <summary>
        /// 重置事件源
        /// </summary>
        /// <param name="newSource">新事件源</param>
        internal void OverrideSource(object newSource)
        {
            _source = newSource;
        }

        /// <summary>
        /// 获取设置触发事件的源对象实例
        /// </summary>
        public object Source
        {
            get { return _source; }
            set
            {
                if (InvokingHandler)
                {
                    throw new InvalidOperationException("路由事件的回调方法原型不匹配！");
                }
                if (_routedEvent == null)
                {
                    throw new InvalidOperationException("必须指定路由事件的类型！");
                }
                object newSource = value;
                if ((_source == null) && (_originalSource == null))
                {
                    _source = _originalSource = newSource;
                    OnSetSource(_source);
                }
                else if (_source != newSource)
                {
                    _source = newSource;
                    OnSetSource(_source);
                }
            }
        }

        /// <summary>
        /// 获取设置当前路由事件是否已被处理
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set
            {
                if (_routedEvent == null)
                {
                    throw new InvalidOperationException("必须指定路由事件的类型！");
                }
                _handled = value;
            }
        }

        /// <summary>
        /// 获取引发事件的原始对象
        /// </summary>
        new public object OriginalSource
        {
            get { return _originalSource; }
        }

        /// <summary>
        /// 获取设置当前参数对象所关联的RoutedEvent对象
        /// </summary>
        public BaseRoutedEvent RoutedEvent
        {
            get { return _routedEvent; }
            set
            {
                if (InvokingHandler)
                {
                    throw new InvalidOperationException("回调方法正在执行中！");
                }
                _routedEvent = value;
            }
        }

        /// <summary>
        /// 回调方法是否正在执行中
        /// </summary>
        bool InvokingHandler
        {
            get { return _invokingHandler; }
            set { _invokingHandler = value; }
        }

        /// <summary>
        /// 执行事件的回调方法，可根据需要重写
        /// </summary>
        /// <param name="genericHandler">回调方法</param>
        /// <param name="genericTarget">目标对象</param>
        protected virtual void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            if (genericHandler == null)
            {
                throw new ArgumentNullException("genericHandler");
            }
            if (genericTarget == null)
            {
                throw new ArgumentNullException("genericTarget");
            }
            if (RoutedEvent == null)
            {
                throw new InvalidOperationException("必须指定路由事件的类型！");
            }
            BaseRoutedEventHandler routedEventHandler = genericHandler as BaseRoutedEventHandler;
            if (routedEventHandler != null)
            {
                routedEventHandler(genericTarget, this);
            }
            else
            {
                genericHandler.DynamicInvoke(new object[] { genericTarget, this });
            }
        }

        /// <summary>
        /// 设置新的事件源
        /// </summary>
        /// <param name="newSource">新事件源</param>
        protected virtual void OnSetSource(object newSource)
        {
        }
    }
}

