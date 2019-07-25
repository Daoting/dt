#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// Api方法描述类
    /// </summary>
    public class ApiMethod
    {
        public ApiMethod(MethodInfo p_method, ApiMethodUsage p_usage, bool p_isTransactional)
        {
            Method = p_method;
            Usage = p_usage;
            IsTransactional = p_isTransactional;
        }

        /// <summary>
        /// Api方法
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Api方法种类
        /// </summary>
        public ApiMethodUsage Usage { get; }

        /// <summary>
        /// 是否自动为方法启用事务
        /// </summary>
        public bool IsTransactional { get; }
    }

    /// <summary>
    /// Api方法的种类
    /// </summary>
    public enum ApiMethodUsage
    {
        /// <summary>
        /// 同步方法
        /// </summary>
        SyncMethod,

        /// <summary>
        /// 异步方法，无返回值
        /// </summary>
        AsyncVoid,

        /// <summary>
        /// 异步方法，有返回值
        /// </summary>
        AsyncResult,
    }
}

