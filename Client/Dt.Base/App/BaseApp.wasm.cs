#if WASM
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认的Application行为
    /// </summary>
    public abstract class BaseApp : Application
    {
        /// <summary>
        /// 存根
        /// </summary>
        protected Stub _stub;

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            _ = Startup.Launch(_stub, p_args.Arguments);
        }
    }
}
#endif