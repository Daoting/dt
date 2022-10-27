#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 存根
    /// </summary>
    public class LobStub : DefaultStub
    {
        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            p_svcs.AddSingleton<IRpcConfig, LobRpcConfig>();
        }
    }
}