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
    /// 客户端日志
    /// </summary>
    [StateTable]
    public class ClientLog
    {
        /// <summary>
        /// 日志主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogEventLevel Level { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime CTime { get; set; }
    }

    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogEventLevel
    {
        /// <summary>
        /// 调试级别日志
        /// </summary>
        Debug = 0,

        /// <summary>
        /// 警告级别日志
        /// </summary>
        Warning = 1,

        /// <summary>
        /// 错误级别日志
        /// </summary>
        Error = 2,
    }
}
