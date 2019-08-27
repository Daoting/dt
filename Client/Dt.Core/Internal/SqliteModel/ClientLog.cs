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
        /// 当前登录用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime CTime { get; set; }
    }
}
