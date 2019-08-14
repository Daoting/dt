#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using System;
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
        /// 
        /// </summary>
        /// <param name="p_isTransactional">是否为每个方法启动事务，默认false</param>
        /// <param name="p_group">所属分组</param>
        /// <param name="p_agentMode">生成代理代码的模式，默认固定服务名模式</param>
        public ApiAttribute(bool p_isTransactional = false, string p_group = null, AgentMode p_agentMode = AgentMode.Default)
        {
            IsTransactional = p_isTransactional;
            Group = p_group;
            AgentMode = p_agentMode;
        }

        /// <summary>
        /// 所属分组
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// 生成代理代码的模式
        /// </summary>
        public AgentMode AgentMode { get; }

        /// <summary>
        /// 是否为每个方法启动事务，默认false
        /// </summary>
        public bool IsTransactional { get; }
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
    {
        public TransactionAttribute(bool p_isTransactional = true)
        {
            IsTransactional = p_isTransactional;
        }

        /// <summary>
        /// 是否自动为方法启用事务
        /// </summary>
        public bool IsTransactional { get; }
    }

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
