#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using Dt.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
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

        }

        /// <summary>
        /// 定义请求管道的中间件和初始化服务
        /// </summary>
        /// <param name="p_app"></param>
        public void Configure(IApplicationBuilder p_app)
        {
            p_app.UseMiddleware<CmMiddleware>();
            SqliteModel.Init().Wait();
        }
    }
}
