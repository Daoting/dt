#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using System;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 打开过的文件历史
    /// </summary>
    [StateTable]
    public class ReadFileHistory
    {
        /// <summary>
        /// 文件标识
        /// </summary>
        [PrimaryKey]
        public long ID { get; set; }

        /// <summary>
        /// 文件描述
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 最后打开时间
        /// </summary>
        public DateTime LastReadTime { get; set; }
    }
}
