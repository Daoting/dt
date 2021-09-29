#if UWP
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台作业
    /// </summary>
    public static partial class BgJob
    {
        public static async Task Run(IStub p_stub)
        {
            await Task.Delay(5000);

        }

        public static void Toast(string p_title, string p_msg, string p_params)
        {
        }

    }
}
#endif