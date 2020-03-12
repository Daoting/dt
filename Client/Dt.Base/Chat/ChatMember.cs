#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 聊天人员信息
    /// </summary>
    [StateTable]
    public class ChatMember
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        [PrimaryKey]
        public long ID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 性别，0女1男
        /// </summary>
        public bool Sex { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime { get; set; }
    }
}
