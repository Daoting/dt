#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base.FileLists
{
    /// <summary>
    /// 上传下载文件的状态
    /// </summary>
    enum FileItemState
    {
        /// <summary>
        /// 普通状态，无正在上传下载
        /// </summary>
        None,

        /// <summary>
        /// 等待上传
        /// </summary>
        UploadWaiting,

        /// <summary>
        /// 正在上传
        /// </summary>
        Uploading,
        
        /// <summary>
        /// 正在下载
        /// </summary>
        Downloading
    }
}
