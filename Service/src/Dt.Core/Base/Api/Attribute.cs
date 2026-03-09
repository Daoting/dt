#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 可远程调用Api的标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ApiAttribute : Attribute
    {
        /// <summary>
        /// 生成代理代码的模式
        /// </summary>
        public AgentMode AgentMode { get; set; }
        
        /// <summary>
        /// 是否为测试用的Api
        /// </summary>
        public bool IsTest { get; set; }
    }

    /// <summary>
    /// 路由处理标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_path">路由路径</param>
        /// <param name="p_dbKey">数据源键名，空时为当前服务的默认数据源</param>
        public RouteAttribute(string p_path, string p_dbKey = null)
        {
            Path = p_path;
            DbKey = p_dbKey;
        }

        /// <summary>
        /// 路由路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 数据源键名，空时为当前服务的默认数据源
        /// </summary>
        public string DbKey { get; }
    }
    
    /// <summary>
    /// 生成代理代码的模式
    /// </summary>
    public enum AgentMode
    {
        /// <summary>
        /// 固定服务名
        /// </summary>
        Default,

        /// <summary>
        /// 泛型模式
        /// </summary>
        Generic,

        /// <summary>
        /// 自定义服务名
        /// </summary>
        Custom
    }
    
    /// <summary>
    /// 授权标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthAttribute : Attribute
    {
        /// <summary>
        /// 所有登录用户
        /// </summary>
        public AuthAttribute()
        { }

        /// <summary>
        /// 自定义校验授权方法
        /// Attribute 的参数只支持简单类型、Type、enum，无法指定委托
        /// </summary>
        /// <param name="p_customAuthType">自定义校验授权方法的类型</param>
        public AuthAttribute(Type p_customAuthType)
        {
            if (p_customAuthType.GetInterface("ICustomAuth") == null)
                throw new Exception(p_customAuthType.FullName + " 需要实现 ICustomAuth 接口");
            CustomAuthType = p_customAuthType;
        }

        /// <summary>
        /// 自定义校验授权方法的类型
        /// </summary>
        public Type CustomAuthType { get; set; }
    }

    /// <summary>
    /// 自定义校验授权接口
    /// </summary>
    public interface ICustomAuth
    {
        Task<bool> IsAuthenticated(HttpContext p_context);
    }
}
