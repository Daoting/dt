#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格分组
    /// </summary>
    internal class RptTblGroup : RptItemBase
    {
        public RptTblGroup(RptTable p_table)
        {
            Table = p_table;
            // 分组字段
            _data.AddCell<string>("field");
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public override RptRoot Root
        {
            get { return Table.Part.Root; }
        }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public override RptPart Part
        {
            get { return Table.Part; }
        }

        /// <summary>
        /// 获取报表项所属父项
        /// </summary>
        public override RptItemBase Parent
        {
            get { return Table; }
        }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public override int Row
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占行数
        /// </summary>
        public override int RowSpan
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占列数
        /// </summary>
        public override int ColSpan
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// 获取所属表格
        /// </summary>
        public RptTable Table { get; }

        /// <summary>
        /// 获取设置分组字段
        /// </summary>
        public string Field
        {
            get { return _data.Str("field"); }
            set { _data["field"] = value; }
        }

        public RptTblGroupHeader Header { get; set; }

        public RptTblGroupFooter Footer { get; set; }

        protected override void ReadChildXml(XmlReader p_reader)
        {
            if (p_reader.Name == "TGroupHeader")
            {
                Header = new RptTblGroupHeader(Table);
                Header.ReadXml(p_reader);
            }
            else if (p_reader.Name == "TGroupFooter")
            {
                Footer = new RptTblGroupFooter(Table);
                Footer.ReadXml(p_reader);
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("TGroup");
            string val = _data.Str("field");
            if (val != "")
                p_writer.WriteAttributeString("field", val);

            if (Header != null)
                Header.WriteXml(p_writer);
            if (Footer != null)
                Footer.WriteXml(p_writer);

            p_writer.WriteEndElement();
        }
    }
}
