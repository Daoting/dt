#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前整个http请求期间的上下文
    /// </summary>
    public partial class Kit
    {
        internal const string ContextItemName = "bag";

        /// <summary>
        /// 获取当前http请求上下文的数据提供者，无http请求上下文时返回新对象！AutoClose为true
        /// <para>如：本地定时器调用或RabbitMQ消息产生的调用无http请求上下文</para>
        /// </summary>
        public static IDataProvider ContextDp
        {
            get
            {
                if (HttpContext != null)
                    return ((Bag)HttpContext.Items[ContextItemName]).Dp;
                return GetService<IDataProvider>();
            }
        }

        /// <summary>
        /// 获取当前http请求上下文的用户标识，UI客户端rpc为实际登录用户ID
        /// <para>特殊标识：110为admin页面，111为RabbitMQ rpc，112为本地调用，113无http请求上下文</para>
        /// </summary>
        public static long ContextUserID => HttpContext != null ? ((Bag)HttpContext.Items[ContextItemName]).UserID : 113;

        /// <summary>
        /// 获取当前http请求上下文的日志对象，日志中比静态Log类多出Api名称和当前UserID，无http请求上下文时返回全局对象
        /// </summary>
        public static ILogger ContextLog => HttpContext != null ? ((Bag)HttpContext.Items[ContextItemName]).Log : Log.Logger;

        /// <summary>
        /// 当前http请求是否为匿名用户
        /// </summary>
        public static bool ContextIsAnonymous => ContextUserID == -1;
    }
}
