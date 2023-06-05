#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 服务存根
    /// </summary>
    public abstract class Stub
    {
        /// <summary>
        /// 服务名称，小写
        /// </summary>
        public virtual string SvcName { get; set; }

        /// <summary>
        /// 是否允许单体服务模式
        /// </summary>
        public virtual bool AllowSingleton => true;

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_services"></param>
        public virtual void ConfigureServices(IServiceCollection p_services) { }

        /// <summary>
        /// 自定义请求处理或定义请求管道的中间件
        /// </summary>
        /// <param name="p_app"></param>
        /// <param name="p_handlers">注册自定义请求处理</param>
        public virtual void Configure(IApplicationBuilder p_app, IDictionary<string, RequestDelegate> p_handlers) { }
    }

    /// <summary>
    /// 默认服务存根，无Api的空服务使用
    /// </summary>
    public class DefaultStub : Stub
    { }
}