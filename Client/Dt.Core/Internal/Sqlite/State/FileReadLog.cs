#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using System;
#endregion

namespace Dt.Core.Model
{
    /// <summary>
    /// 文件打开记录
    /// </summary>
    [StateTable]
    public class FileReadLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// 文件标识
        /// </summary>
        public string FileID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }
}
