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
    [Sqlite("state")]
    public class ClientLog : Entity
    {
        #region 构造方法
        ClientLog() { }

        public ClientLog(
            LogEventLevel Level = default,
            string Content = default,
            DateTime Ctime = default)
        {
            AddCell("ID", 0);
            AddCell("Level", Level);
            AddCell("Content", Content);
            AddCell("Ctime", Ctime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 日志主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        new public int ID
        {
            get { return (int)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogEventLevel Level
        {
            get { return (LogEventLevel)this["Level"]; }
            set { this["Level"] = value; }
        }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content
        {
            get { return (string)this["Content"]; }
            set { this["Content"] = value; }
        }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["CTime"]; }
            set { this["CTime"] = value; }
        }
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
