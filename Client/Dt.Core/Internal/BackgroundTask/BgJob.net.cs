#if WASM || DESKTOP
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text.Json;
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
        /// <summary>
        /// 注册后台任务
        /// </summary>
        public static void Register()
        {
        }

        /// <summary>
        /// 注销后台任务
        /// </summary>
        public static void Unregister()
        {
        }

        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo)
        {
            
        }
    }
}
#endif