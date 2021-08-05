#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 宿主操作系统类型
    /// </summary>
    public enum HostOS
    {
        /// <summary>
        /// Windows系统
        /// </summary>
        Windows,

        /// <summary>
        /// iphone
        /// </summary>
        iOS,

        /// <summary>
        /// 安卓系统
        /// </summary>
        Android,

        /// <summary>
        /// Mac系统，wasm有效
        /// </summary>
        Mac,

        /// <summary>
        /// Linux系统，wasm有效
        /// </summary>
        Linux,

        /// <summary>
        /// 其他操作系统，wasm有效
        /// </summary>
        Other
    }
}