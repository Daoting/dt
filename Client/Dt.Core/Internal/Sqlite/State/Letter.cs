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
    /// 聊天内容
    /// </summary>
    [StateTable]
    public class Letter
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = -1;

        /// <summary>
        /// 当前登录者UserID，区分所属聊天人
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// 信息标识，撤回时识别用，不同登录人使用同一设备时可能出现重复(自己发给自己)
        /// </summary>
        public string MsgID { get; set; }

        /// <summary>
        /// 聊天对方的类型
        /// </summary>
        public LetterOtherType OtherType { get; set; }

        /// <summary>
        /// 对方标识，可能为UserID、GroupID、BatchID
        /// </summary>
        public string OtherID { get; set; }

        /// <summary>
        /// 对方名称，可能为用户名、群组名、群发名
        /// </summary>
        public string OtherName { get; set; }

        /// <summary>
        /// 对方用户名，群聊时为发送者名字，群发时为所有接收者名字
        /// </summary>
        public string OtherUser { get; set; }

        /// <summary>
        /// 群发时记录所有接收者ID，逗号隔开
        /// </summary>
        public string BatchID { get; set; }

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
        /// 是否为直接收发（服务器推送）
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 收发时间
        /// </summary>
        public DateTime STime { get; set; }
    }

    /// <summary>
    /// 聊天对方的类型
    /// </summary>
    public enum LetterOtherType
    {
        /// <summary>
        /// 用户一对一聊天
        /// </summary>
        User,

        /// <summary>
        /// 群聊
        /// </summary>
        Group,

        /// <summary>
        /// 批量群发
        /// </summary>
        Batch
    }

    /// <summary>
    /// 信息种类
    /// </summary>
    public enum LetterType
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text,

        /// <summary>
        /// 文件
        /// </summary>
        File,

        /// <summary>
        /// 图片
        /// </summary>
        Image,

        /// <summary>
        /// 声音
        /// </summary>
        Voice,

        /// <summary>
        /// 视频
        /// </summary>
        Video,

        /// <summary>
        /// 链接
        /// </summary>
        Link,

        /// <summary>
        /// 撤回
        /// </summary>
        Undo
    }
}
