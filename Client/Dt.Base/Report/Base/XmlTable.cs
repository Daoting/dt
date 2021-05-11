#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base.Report;
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
    /// Table、Row对xml的扩展方法
    /// </summary>
    internal static class XmlTable
    {
        #region Table
        /// <summary>
        /// 读取xml中的行数据，根元素和行元素名称任意，xml内容形如：
        /// \Params>
        ///   \Param id="参数标识" name="参数名" type="double"\
        ///     <![CDATA[xaml内容]]>
        ///   /Param>
        /// /Params>
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <param name="p_reader"></param>
        /// <param name="p_cdataCol"></param>
        public static void ReadXml(this Table p_tbl, XmlReader p_reader, string p_cdataCol)
        {
            if (p_reader == null)
                return;

            if (p_reader.IsEmptyElement)
            {
                p_reader.Read();
                return;
            }

            string root = p_reader.Name;
            p_reader.Read();
            while (p_reader.NodeType != XmlNodeType.None)
            {
                if (p_reader.NodeType == XmlNodeType.EndElement && p_reader.Name == root)
                    break;

                Row row = p_tbl.AddRow();
                row.IsAdded = false;
                for (int i = 0; i < p_reader.AttributeCount; i++)
                {
                    p_reader.MoveToAttribute(i);
                    string id = p_reader.Name;
                    if (row.Cells.Contains(id))
                        row.Cells[id].InitVal(p_reader.Value);
                }

                p_reader.Read();
                if (p_reader.NodeType == XmlNodeType.CDATA)
                {
                    row.Cells[p_cdataCol].InitVal(p_reader.Value);
                    p_reader.Read();
                    p_reader.Read();
                }
            }
            p_reader.Read();
        }

        /// <summary>
        /// 按指定元素名称序列化行数据xml
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <param name="p_writer"></param>
        /// <param name="p_rootName">根节点名称</param>
        /// <param name="p_rowName">行节点名称</param>
        public static void WriteXml(this Table p_tbl, XmlWriter p_writer, string p_rootName, string p_rowName)
        {
            p_writer.WriteStartElement(p_rootName);
            foreach (Row row in p_tbl)
            {
                p_writer.WriteStartElement(p_rowName);
                foreach (var cell in row.Cells)
                {
                    // 为空或与默认值相同时不输出
                    if (cell.Val == null
                        || (cell.Type == typeof(string) && cell.GetVal<string>() == "")
                        || (cell.Type == typeof(bool) && !cell.GetVal<bool>()))
                        continue;

                    // 列值以属性输出
                    p_writer.WriteAttributeString(cell.ID, cell.Val.ToString());
                }
                p_writer.WriteEndElement();
            }
            p_writer.WriteEndElement();
        }
        #endregion

        #region Row
        /// <summary>
        /// 读取xml中的单元格数据，元素名称任意，xml内容形如：
        ///   Param id="参数标识" name="参数名" type="double"
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_reader"></param>
        public static void ReadXml(this Row p_row, XmlReader p_reader)
        {
            if (p_reader == null)
                return;

            for (int i = 0; i < p_reader.AttributeCount; i++)
            {
                p_reader.MoveToAttribute(i);
                string id = p_reader.Name;
                if (p_row.Cells.Contains(id))
                    p_row.Cells[id].InitVal(p_reader.Value);
            }
            p_reader.MoveToElement();
        }

        /// <summary>
        /// 序列化行数据xml
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_writer"></param>
        public static void WriteXml(this Row p_row, XmlWriter p_writer)
        {
            if (p_writer == null)
                return;

            foreach (var cell in p_row.Cells)
            {
                string val = cell.GetVal<string>();

                // 为空或与默认值相同时不输出
                if (string.IsNullOrEmpty(val))
                    continue;

                // 列值以属性输出
                if (cell.Type == typeof(DateTime))
                    p_writer.WriteAttributeString(cell.ID, ((DateTime)cell.Val).ToString("yyyy-MM-ddTHH:mm:ss.ffffff"));
                else
                    p_writer.WriteAttributeString(cell.ID, val);
            }
        }
        #endregion
    }
}
