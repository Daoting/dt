#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 文件上传过程的UI
    /// </summary>
    public interface IUploadUI
    {
        /// <summary>
        /// 上传进度回调
        /// </summary>
        ProgressDelegate UploadProgress { get; }

        /// <summary>
        /// 准备上传
        /// </summary>
        /// <param name="p_file">待上传文件对象</param>
        /// <returns></returns>
        Task InitUpload(FileData p_file);

        /// <summary>
        /// 上传成功后
        /// </summary>
        /// <param name="p_id">文件上传路径</param>
        /// <param name="p_file">已上传文件对象</param>
        /// <returns></returns>
        Task UploadSuccess(string p_id, FileData p_file);

        /// <summary>
        /// 上传失败后
        /// </summary>
        /// <param name="p_file"></param>
        void UploadFail(FileData p_file);
    }
}