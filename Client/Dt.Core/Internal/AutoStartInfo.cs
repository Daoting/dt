#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 自启动信息描述
    /// </summary>
    internal class AutoStartInfo
    {
        /// <summary>
        /// 获取设置自启动的窗口类型
        /// </summary>
        public string WinType { get; set; }

        /// <summary>
        /// 获取设置初始参数
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// 获取设置Win标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置图标名称
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 反序列化启动参数
        /// </summary>
        /// <param name="p_xml"></param>
        /// <returns></returns>
        public static AutoStartInfo ReadXml(string p_xml)
        {
            if (string.IsNullOrEmpty(p_xml))
                return null;

            AutoStartInfo result = null;
            try
            {
                AutoStartInfo info = new AutoStartInfo();
                using (StringReader stream = new StringReader(p_xml))
                using (XmlReader reader = XmlReader.Create(stream, AtKit.ReaderSettings))
                {
                    reader.Read();
                    info.WinType = reader.GetAttribute("WinType");
                    if (string.IsNullOrEmpty(info.WinType))
                        throw new Exception("");

                    info.Params = reader.GetAttribute("Params");
                    info.Title = reader.GetAttribute("Title");
                    info.Icon = reader.GetAttribute("Icon");
                }
                result = info;
            }
            catch { }
            return result;
        }

        /// <summary>
        /// 序列化启动参数
        /// </summary>
        /// <returns></returns>
        public string WriteXml()
        {
            StringBuilder xml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(xml, AtKit.WriterSettings))
            {
                writer.WriteStartElement("P");
                writer.WriteAttributeString("WinType", WinType);
                if (!string.IsNullOrEmpty(Params))
                    writer.WriteAttributeString("Params", Params);
                writer.WriteAttributeString("Title", Title);
                writer.WriteAttributeString("Icon", Icon);
                writer.WriteEndElement();
                writer.Flush();
            }
            return xml.ToString();
        }
    }
}
