#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dts.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dts.Auth
{
    /// <summary>
    /// 服务存根
    /// </summary>
    public class Stub : ISvcStub
    {
        /// <summary>
        /// 定义全局服务
        /// </summary>
        /// <param name="p_services"></param>
        public void ConfigureServices(IServiceCollection p_services)
        {
            p_services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddResourceOwnerValidator<AccountValidator>()
                .AddProfileService<ProfileService>();
        }

        /// <summary>
        /// 定义请求管道的中间件和初始化服务
        /// </summary>
        /// <param name="p_app"></param>
        public void Configure(IApplicationBuilder p_app)
        {
            p_app.UseIdentityServer();
        }
    }
}
