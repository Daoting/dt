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
    /// <summary>
    /// 指示路由事件的路由策略。
    /// </summary>
    public enum RoutingStrategy
    {
        /// <summary>
        /// 路由事件使用隧道策略，以便事件实例通过树向下路由（从根到源元素）。
        /// </summary>
        Tunnel,

        /// <summary>
        /// 路由事件使用冒泡策略，以便事件实例通过树向上路由（从事件元素到根）。
        /// </summary>
        Bubble,

        /// <summary>
        /// 路由事件不通过元素树路由
        /// </summary>
        Direct
    }
}

