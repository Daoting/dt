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
    /// 文件扩展信息
    /// </summary>
    class FileItemInfo
    {
        /// <summary>
        /// 获取设置原始文件名称，不包括扩展名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 获取设置文件类型及说明，绑定用，形如：文本文件 (.txt)
        /// </summary>
        public string FileDesc { get; set; }

        /// <summary>
        /// 获取设置文件大小
        /// </summary>
        public ulong Length { get; set; }

        /// <summary>
        /// 获取或设置上传文件用户
        /// </summary>
        public string Uploader { get; set; }

        /// <summary>
        /// 获取或设置文件上传日期
        /// </summary>
        public string Date { get; set; }
    }
}
