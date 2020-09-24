#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 数据源包装类
    /// </summary>
    internal static class RptKit
    {
        // 报表模板缓存
        static readonly Dictionary<string, RptRoot> _tempCache = new Dictionary<string, RptRoot>();

        /// <summary>
        /// 反序列化报表模板
        /// </summary>
        /// <param name="p_define"></param>
        /// <returns></returns>
        internal static Task<RptRoot> DeserializeTemplate(string p_define)
        {
            return Task.Run(() =>
            {
                RptRoot root = new RptRoot();
                if (!string.IsNullOrEmpty(p_define))
                {
                    try
                    {
                        using (StringReader stream = new StringReader(p_define))
                        {
                            using (XmlReader reader = XmlReader.Create(stream, AtKit.ReaderSettings))
                            {
                                reader.Read();
                                root.ReadXml(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("反序列化报表模板时异常：{0}", ex.Message));
                    }
                }
                return root;
            });
        }

        /// <summary>
        /// 序列化报表模板
        /// </summary>
        /// <param name="p_root"></param>
        /// <returns></returns>
        internal static string SerializeTemplate(RptRoot p_root)
        {
            if (p_root == null)
                return string.Empty;

            p_root.OnBeforeSerialize();
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                p_root.WriteXml(writer);
                writer.Flush();
            }
            p_root.OnAfterSerialize();
            return sb.ToString();
        }
    }
}
