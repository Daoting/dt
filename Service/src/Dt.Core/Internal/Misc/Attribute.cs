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
        /// 所属分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 生成代理代码的模式
        /// </summary>
        public AgentMode AgentMode { get; set; }

        /// <summary>
        /// 拦截器类型数组
        /// </summary>
        public Type[] Interceptors { get; set; }
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
    /// 服务标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SvcAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_lifetime">服务的生命周期，默认Transient</param>
        public SvcAttribute(ServiceLifetime p_lifetime = ServiceLifetime.Transient)
        {
            Lifetime = p_lifetime;
        }

        /// <summary>
        /// 服务的生命周期
        /// </summary>
        public ServiceLifetime Lifetime { get; }
    }

    /// <summary>
    /// 自动为方法启用事务的标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TransactionAttribute : Attribute
    { }

    /// <summary>
    /// 自定义Agent方法代码
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomAgentAttribute : Attribute
    {
        public CustomAgentAttribute(string p_code)
        {
            Code = p_code;
        }

        /// <summary>
        /// 自定义Agent方法代码
        /// </summary>
        public string Code { get; }
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

    /// <summary>
    /// 类型可序列化属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JsonObjAttribute : AliasAttribute
    {
        public JsonObjAttribute(string p_alias)
            : base(p_alias)
        {
        }
    }

    /// <summary>
    /// 基类属性
    /// </summary>
    public abstract class AliasAttribute : Attribute
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p_alias">名称</param>
        protected AliasAttribute(string p_alias)
        {
            Alias = p_alias;
        }
    }
}
