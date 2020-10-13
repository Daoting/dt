#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表参数定义
    /// </summary>
    internal class RptParams
    {
        public RptParams(RptRoot p_root)
        {
            Root = p_root;
            Data = new Table
            {
                { "id" },
                { "title" },
                { "type" },
                { "val" },
                { "ismacro", typeof(bool) },
                { "showtitle", typeof(bool) },
                { "titlewidth",  typeof(double)},
                { "showstar", typeof(bool) },
                { "isverticaltitle", typeof(bool) },
                { "ishorstretch", typeof(bool) },
                { "rowspan", typeof(int) },
                { "placeholder" },
                { "isreadonly", typeof(bool) },
                { "hide", typeof(bool) },
                { "note" },
            };
            Data.Changed += Root.OnCellValueChanged;
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 获取数据源列表
        /// </summary>
        public Table Data { get; }

        /// <summary>
        /// 获取宏参数名列表
        /// </summary>
        public List<string> Macros
        {
            get
            {
                List<string> ls = new List<string>();
                foreach (Row row in Data)
                {
                    if (row.Bool("macro"))
                        ls.Add(row.Str("id"));
                }
                if (ls.Count == 0)
                    return null;
                return ls;
            }
        }

        /// <summary>
        /// 根据参数名获取参数定义Row
        /// </summary>
        /// <param name="p_col"></param>
        /// <returns></returns>
        public Row this[string p_col]
        {
            get
            {
                return (from row in Data
                        where row.Str("id") == p_col
                        select row).FirstOrDefault();
            }
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            XmlTable.ReadXml(p_reader, CreateNewRow);
        }

        Row CreateNewRow()
        {
            return Data.AddRow(new
            {
                type = "string",
                showtitle = true,
                rowspan = 1,
            });
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Params");
            foreach (Row row in Data)
            {
                p_writer.WriteStartElement("Param");

                p_writer.WriteAttributeString("id", row.Str("id"));

                string val = row.Str("title");
                if (val != string.Empty)
                    p_writer.WriteAttributeString("title", val);

                val = row.Str("type");
                if (val != string.Empty && val != "string")
                    p_writer.WriteAttributeString("type", val);

                val = row.Str("val");
                if (val != string.Empty)
                    p_writer.WriteAttributeString("val", val);

                if (row.Bool("ismacro"))
                    p_writer.WriteAttributeString("ismacro", "True");
                if (!row.Bool("showtitle"))
                    p_writer.WriteAttributeString("showtitle", "False");
                if (row.Double("titlewidth") > 0)
                    p_writer.WriteAttributeString("titlewidth", row.Int("titlewidth").ToString());
                if (row.Bool("showstar"))
                    p_writer.WriteAttributeString("showstar", "True");
                if (row.Bool("isverticaltitle"))
                    p_writer.WriteAttributeString("isverticaltitle", "True");
                if (row.Bool("ishorstretch"))
                    p_writer.WriteAttributeString("ishorstretch", "True");
                if (row.Int("rowspan") > 1)
                    p_writer.WriteAttributeString("rowspan", row.Int("rowspan").ToString());

                val = row.Str("placeholder");
                if (val != string.Empty)
                    p_writer.WriteAttributeString("placeholder", val);

                if (row.Bool("isreadonly"))
                    p_writer.WriteAttributeString("isreadonly", "True");
                if (row.Bool("hide"))
                    p_writer.WriteAttributeString("hide", "True");

                val = row.Str("note");
                if (val != string.Empty)
                    p_writer.WriteAttributeString("note", val);
                p_writer.WriteEndElement();
            }
            p_writer.WriteEndElement();
        }
    }
}
