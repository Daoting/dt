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
        public ApiMethod(MethodInfo p_method, ApiCallMode p_callMode, AuthAttribute p_auth, string p_svcName)
        {
            Method = p_method;
            CallMode = p_callMode;
            Auth = p_auth;
            SvcName = p_svcName;
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
        /// 服务名称
        /// </summary>
        public string SvcName { get; }
    }
}

