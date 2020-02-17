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
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 消息内容
    /// </summary>
    public class MsgInfo : IRpcJson
    {
        /// <summary>
        /// 在线时调用客户端的方法名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 在线时调用客户端方法的参数
        /// </summary>
        public List<object> Params { get; set; }

        /// <summary>
        /// 离线推送时的消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 离线推送时的消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 获取在线推送的内容
        /// </summary>
        /// <returns></returns>
        public string GetOnlineMsg()
        {
            return RpcKit.GetCallString(MethodName, Params);
        }

        /// <summary>
        /// 获取Toast内容xml
        /// </summary>
        /// <returns></returns>
        public byte[] GetToastMsg()
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb))
            {
                writer.WriteStartElement("toast");

                // 启动参数
                if (!string.IsNullOrEmpty(MethodName))
                    writer.WriteAttributeString("launch", GetOnlineMsg());
                writer.WriteStartElement("visual");
                writer.WriteStartElement("binding");
                writer.WriteAttributeString("template", "ToastText02");

                writer.WriteStartElement("text");
                writer.WriteAttributeString("id", "1");
                writer.WriteValue(Title);
                writer.WriteEndElement();

                writer.WriteStartElement("text");
                writer.WriteAttributeString("id", "2");
                writer.WriteValue(Content);
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        #region IRpcJson
        void IRpcJson.ReadRpcJson(ref Utf8JsonReader p_reader)
        {
            p_reader.Read();
            MethodName = p_reader.GetString();

            // 参数外层 [
            p_reader.Read();
            Params = new List<object>();
            while (p_reader.Read())
            {
                // 参数外层 ]
                if (p_reader.TokenType == JsonTokenType.EndArray)
                    break;
                Params.Add(JsonRpcSerializer.Deserialize(ref p_reader));
            }

            p_reader.Read();
            Title = p_reader.GetString();
            p_reader.Read();
            Content = p_reader.GetString();

            // 最外层 ]
            p_reader.Read();
        }

        void IRpcJson.WriteRpcJson(Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteStringValue("#msg");
            p_writer.WriteStringValue(MethodName);

            // 参数
            p_writer.WriteStartArray();
            if (Params != null && Params.Count > 0)
            {
                foreach (var par in Params)
                {
                    JsonRpcSerializer.Serialize(par, p_writer);
                }
            }
            p_writer.WriteEndArray();

            p_writer.WriteStringValue(Title);
            p_writer.WriteStringValue(Content);

            p_writer.WriteEndArray();
        }
        #endregion
    }
}
