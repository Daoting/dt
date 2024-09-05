#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-05 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Dt.Core.Rpc;
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 聊天信息描述
    /// </summary>
    public class LetterInfo : IRpcJson
    {
        /// <summary>
        /// 信息标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 发送者标识
        /// </summary>
        public long SenderID { get; set; }

        /// <summary>
        /// 发送者名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public LetterType LetterType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        #region IRpcJson
        void IRpcJson.ReadRpcJson(ref Utf8JsonReader p_reader)
        {
            p_reader.Read();
            ID = p_reader.GetString();
            p_reader.Read();
            SenderID = p_reader.GetInt64();
            p_reader.Read();
            SenderName = p_reader.GetString();
            p_reader.Read();
            LetterType = (LetterType)p_reader.GetInt32();
            p_reader.Read();
            Content = p_reader.GetString();
            p_reader.Read();
            SendTime = p_reader.GetDateTime();

            // ]
            p_reader.Read();
        }

        void IRpcJson.WriteRpcJson(Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteStringValue("#letter");

            p_writer.WriteStringValue(ID);
            p_writer.WriteNumberValue(SenderID);
            p_writer.WriteStringValue(SenderName);
            p_writer.WriteNumberValue((int)LetterType);
            p_writer.WriteStringValue(Content);
            p_writer.WriteStringValue(SendTime);

            p_writer.WriteEndArray();
        }
        #endregion
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
