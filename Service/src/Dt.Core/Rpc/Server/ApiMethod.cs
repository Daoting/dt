#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Api方法描述类
    /// </summary>
    public class ApiMethod
    {
        public ApiMethod(MethodInfo p_method, ApiCallMode p_callMode, AuthAttribute p_auth, bool p_isTransactional)
        {
            Method = p_method;
            CallMode = p_callMode;
            Auth = p_auth;
            IsTransactional = p_isTransactional;
        }

        /// <summary>
        /// Api方法
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Api调用模式
        /// </summary>
        public ApiCallMode CallMode { get; }

        /// <summary>
        /// 校验Api调用授权
        /// </summary>
        public AuthAttribute Auth { get; }

        /// <summary>
        /// 是否自动为方法启用事务
        /// </summary>
        public bool IsTransactional { get; }
    }
}

