#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 文件管理的参数设置
    /// </summary>
    public class FileMgrSetting
    {
        /// <summary>
        /// 允许编辑
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// 是否记录已读文件
        /// </summary>
        public bool SaveHistory { get; set; } = true;

        /// <summary>
        /// 打开文件后的回调方法
        /// </summary>
        public Action OnOpenedFile { get; set; }
    }
}
