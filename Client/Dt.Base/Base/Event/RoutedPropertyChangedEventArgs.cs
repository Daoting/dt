#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Base
{

    internal delegate void PhiRoutedPropertyChangedEventHandler<T>(object sender, PhiRoutedPropertyChangedEventArgs<T> e);

    /// <summary>
    /// 为 PropertyChanged 事件提供数据。
    /// </summary>
    /// <typeparam name="T">已更改的属性类型。</typeparam>
    internal class PhiRoutedPropertyChangedEventArgs<T> : BaseRoutedEventArgs
    {
        T newValue;
        T oldValue;

        /// <summary>
        /// 初始化 RadRoutedPropertyChangedEventArgs 类的一个新实例.
        /// </summary>
        /// <param name="oldValue">旧的属性值</param>
        /// <param name="newValue">新的属性值</param>
        public PhiRoutedPropertyChangedEventArgs(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// 初始化 RadRoutedPropertyChangedEventArgs 类的一个新实例.
        /// </summary>
        /// <param name="oldValue">旧的属性值</param>
        /// <param name="newValue">新的属性值</param>
        /// <param name="routedEvent">路由事件</param>
        public PhiRoutedPropertyChangedEventArgs(T oldValue, T newValue, BaseRoutedEvent routedEvent) : this(oldValue, newValue)
        {
            base.RoutedEvent = routedEvent;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            PhiRoutedPropertyChangedEventHandler<T> handler = (PhiRoutedPropertyChangedEventHandler<T>)genericHandler;
            handler(genericTarget, (PhiRoutedPropertyChangedEventArgs<T>)this);
        }

        /// <summary>
        /// 获取新的属性值
        /// </summary>
        /// <value>新的属性值</value>
        public T NewValue
        {
            get { return  this.newValue; }
        }

        /// <summary>
        /// 获取旧的属性值
        /// </summary>
        /// <value>旧的属性值</value>
        public T OldValue
        {
            get { return  this.oldValue; }
        }
    }
}

