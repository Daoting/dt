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
    /// 服务类标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ServiceAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_lifetime">服务的生命周期，默认Transient</param>
        public ServiceAttribute(ServiceLifetime p_lifetime = ServiceLifetime.Transient)
        {
            Lifetime = p_lifetime;
        }

        /// <summary>
        /// 服务的生命周期
        /// </summary>
        public ServiceLifetime Lifetime { get; }
    }

    /// <summary>
    /// 事件处理类型标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EventHandlerAttribute : Attribute
    {
        public EventHandlerAttribute() { }
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
