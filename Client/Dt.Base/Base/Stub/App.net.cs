#if WASM || DESKTOP
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认存根
    /// </summary>
    public partial class DefaultStub : Stub
    {
        public override Task OnLaunched(LaunchActivatedEventArgs p_args)
        {
            return Launch();
        }
    }
}
#endif