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
        internal const string ContextItemName = "ApiContext";

        /// <summary>
        /// 获取当前http请求上下文的数据提供者
        /// </summary>
        public static DataProvider ContextDp => ((Bag)HttpContext.Items[ContextItemName]).GetDataProvider();

        /// <summary>
        /// 获取当前http请求上下文的用户标识
        /// </summary>
        public static long ContextUserID => ((Bag)HttpContext.Items[ContextItemName]).Invoker.UserID;

        /// <summary>
        /// 获取当前http请求上下文的日志对象，日志中比静态Log类多出Api名称和当前UserID
        /// </summary>
        public static ILogger ContextLog => ((Bag)Kit.HttpContext.Items[ContextItemName]).Invoker.Log;

        /// <summary>
        /// 当前http请求是否为匿名用户
        /// </summary>
        public static bool ContextIsAnonymous => ((Bag)Kit.HttpContext.Items[ContextItemName]).Invoker.UserID == -1;

        /// <summary>
        /// 添加共享数据项，在整个http请求期间有效
        /// </summary>
        /// <param name="p_name">共享项名称</param>
        /// <param name="p_value">共享项</param>
        public static void AddShareItem(string p_name, object p_value)
        {
            Throw.IfNullOrEmpty(p_name);
            ((Bag)HttpContext.Items[ContextItemName]).AddItem(p_name, p_value);
        }

        /// <summary>
        /// 获取共享数据项，在整个http请求期间有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_name">共享项名称</param>
        /// <returns></returns>
        public static T GetShareItem<T>(string p_name)
        {
            Throw.IfNullOrEmpty(p_name);
            return ((Bag)HttpContext.Items[ContextItemName]).GetItem<T>(p_name);
        }
    }
}
