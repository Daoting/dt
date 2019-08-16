#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 路由事件类型
    /// </summary>
    public sealed class BaseRoutedEvent
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="routingStrategy">路由策略</param>
        /// <param name="handlerType">事件处理类型</param>
        /// <param name="ownerType">注册事件的所有者类型</param>
        internal BaseRoutedEvent(string name, RoutingStrategy routingStrategy, Type handlerType, Type ownerType)
        {
            Name = name;
            RoutingStrategy = routingStrategy;
            HandlerType = handlerType;
            OwnerType = ownerType;
        }

        /// <summary>
        /// 关联另外的事件所有者，并且所有者可以路由事件和处理事件
        /// </summary>
        /// <param name="ownerType">待添加事件所有者的类型.</param>
        /// <returns>
        /// 返回的路由事件应该是静态只读变量，为public类型以便实例对象通过AddHandler方法附加事件
        /// </returns>
        public BaseRoutedEvent AddOwner(Type ownerType)
        {
            EventManager.AddOwner(this, ownerType);
            return this;
        }

        /// <summary>
        /// 返回表明当前事件的完整名称
        /// </summary>
        /// <returns>限定事件名称</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { OwnerType.Name, Name });
        }

        /// <summary>
        /// 判断回调方法原型是否正确
        /// </summary>
        /// <param name="handler">回调方法</param>
        /// <returns></returns>
        internal bool IsLegalHandler(Delegate handler)
        {
            Type type = handler.GetType();
            if (type != HandlerType)
            {
                return (type == typeof(RoutedEventHandler));
            }
            return true;
        }

        /// <summary>
        /// 获取事件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取设置当前事件所采用的路由策略
        /// </summary>
        public RoutingStrategy RoutingStrategy { get; set; }

        /// <summary>
        /// 获取设置事件处理类型
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// 获取设置注册事件的所有者类型
        /// </summary>
        public Type OwnerType { get; set; }
    }
}