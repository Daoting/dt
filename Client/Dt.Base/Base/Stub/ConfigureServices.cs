#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-20
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 注入默认服务
    /// </summary>
    public partial class DefaultStub : Stub
    {
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            p_svcs.AddSingleton<IUICallback, DefUICallback>();
        }
    }
}