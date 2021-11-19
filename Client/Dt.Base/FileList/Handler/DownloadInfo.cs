#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System.IO;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 文件下载描述
    /// </summary>
    public class DownloadInfo
    {
        /// <summary>
        /// 获取设置要下载的文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 获取设置下载内容要保存的目标文件流
        /// </summary>
        public Stream TgtStream { get; set; }

        /// <summary>
        /// 获取设置错误提示信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 下载进度，可以为null
        /// </summary>
        public ProgressDelegate Progress { get; set; }
    }
}