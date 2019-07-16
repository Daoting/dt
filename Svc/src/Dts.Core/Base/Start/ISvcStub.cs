#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 服务存根接口
    /// </summary>
    public interface ISvcStub
    {
        /// <summary>
        /// 定义全局服务
        /// </summary>
        /// <param name="p_services"></param>
        void ConfigureServices(IServiceCollection p_services);

        /// <summary>
        /// 定义请求管道的中间件和初始化服务
        /// </summary>
        /// <param name="p_app"></param>
        /// <param name="p_env"></param>
        void Configure(IApplicationBuilder p_app, IHostingEnvironment p_env);
    }
}