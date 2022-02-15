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
        /// 存根类型
        /// </summary>
        public abstract Type Stub { get; }

        protected override async void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            // 确保state.db正常打开，否则每次都重建！
            await Task.Delay(100);
            await Startup.Launch(Stub, p_args.Arguments);
        }
    }
}
#endif