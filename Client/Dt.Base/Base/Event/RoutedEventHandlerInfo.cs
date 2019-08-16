#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 路由事件回调处理的管理类
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RoutedEventHandlerInfo
    {   
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="handler">回调方法</param>
        /// <param name="handledEventsToo">是否始终执行</param>
        internal RoutedEventHandlerInfo(Delegate handler, bool handledEventsToo)
        {
            this = new RoutedEventHandlerInfo();
            Handler = handler;
            InvokeHandledEventsToo = handledEventsToo;
        }

        /// <summary>
        /// 执行回调方法，在RouteItem中调用
        /// </summary>
        /// <param name="target"></param>
        /// <param name="routedEventArgs"></param>
        internal void InvokeHandler(object target, BaseRoutedEventArgs routedEventArgs)
        {
            // 若未处理或始终执行回调
            if (!routedEventArgs.Handled || InvokeHandledEventsToo)
            {
                if (Handler is RoutedEventHandler)
                {
                    ((RoutedEventHandler)Handler)(target, routedEventArgs);
                }
                else
                {
                    routedEventArgs.InvokeHandler(Handler, target);
                }
            }
        }

        /// <summary>
        /// 回调方法
        /// </summary>
        public Delegate Handler { get; set; }

        /// <summary>
        /// true 表始终调用处理事件即使标记已处理，false 则根据标记进行处理
        /// </summary>
        public bool InvokeHandledEventsToo { get; set; }

        /// <summary>
        /// 重载内置运算符==
        /// </summary>
        /// <param name="handlerInfo1">路由事件回调处理的管理类1</param>
        /// <param name="handlerInfo2">路由事件回调处理的管理类2</param>
        /// <returns></returns>
        public static bool operator ==(RoutedEventHandlerInfo handlerInfo1, RoutedEventHandlerInfo handlerInfo2)
        {
            return handlerInfo1.Equals(handlerInfo2);
        }

        /// <summary>
        /// 重载内置运算符=!=
        /// </summary>
        /// <param name="handlerInfo1">路由事件回调处理的管理类1</param>
        /// <param name="handlerInfo2">路由事件回调处理的管理类2</param>
        /// <returns></returns>
        public static bool operator !=(RoutedEventHandlerInfo handlerInfo1, RoutedEventHandlerInfo handlerInfo2)
        {
            return !handlerInfo1.Equals(handlerInfo2);
        }

        /// <summary>
        /// 获取哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
       
        /// <summary>
        /// 确定提供的对象是否等于当前RoutedEventHandlerInfo
        /// </summary>
        /// <param name="obj">要与当前实例进行比较的RoutedEventHandlerInfo</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (((obj != null) && (obj is RoutedEventHandlerInfo)) && Equals((RoutedEventHandlerInfo)obj));
        }

        /// <summary>
        /// 确定提供的对象是否等于当前RoutedEventHandlerInfo
        /// </summary>
        /// <param name="handlerInfo">要与当前实例进行比较的RoutedEventHandlerInfo</param>
        /// <returns></returns>
        public bool Equals(RoutedEventHandlerInfo handlerInfo)
        {
            return ((Handler == handlerInfo.Handler) && (InvokeHandledEventsToo == handlerInfo.InvokeHandledEventsToo));
        }
    }

    /// <summary>
    /// 路由事件回调列表
    /// </summary>
    internal class RoutedEventHandlerInfoList
    {
        internal bool Contains(RoutedEventHandlerInfoList handlers)
        {
            for (RoutedEventHandlerInfoList list = this; list != null; list = list.Next)
            {
                if (list == handlers)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal RoutedEventHandlerInfo[] Handlers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal RoutedEventHandlerInfoList Next { get; set; }
    }
}
