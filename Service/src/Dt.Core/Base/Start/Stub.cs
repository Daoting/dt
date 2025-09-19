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
        /// 当前微服务http post请求的最大长度，0时采用默认28.6M
        /// </summary>
        public virtual long MaxRequestBodySize { get; }

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
}