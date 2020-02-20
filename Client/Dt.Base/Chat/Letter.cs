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
    /// 聊天内容
    /// </summary>
    [StateTable]
    public class Letter
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// 当前登录者UserID，区分所属聊天人
        /// </summary>
        public long LoginID { get; set; }

        /// <summary>
        /// 信息标识，撤回时识别用，不同登录人使用同一设备时可能出现重复(自己发给自己)
        /// </summary>
        public string MsgID { get; set; }

        /// <summary>
        /// 对方UserID
        /// </summary>
        public long OtherID { get; set; }

        /// <summary>
        /// 对方用户名
        /// </summary>
        public string OtherName { get; set; }

        /// <summary>
        /// 接收标志
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// 未读标志
        /// </summary>
        public bool Unread { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public LetterType LetterType { get; set; }

        /// <summary>
        /// 对方接收时是否在线，只发送时有效
        /// </summary>
        public bool OtherIsOnline { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 收发时间
        /// </summary>
        public DateTime STime { get; set; }
    }
}
