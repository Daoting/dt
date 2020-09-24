#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Collections.Generic;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格容器内的行
    /// </summary>
    internal class RptTblPartRow : RptItemBase
    {
        public RptTblPartRow(RptTblPart p_part)
        {
            TblPart = p_part;
            Cells = new List<RptText>();
        }
       
        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public override RptRoot Root
        {
            get { return TblPart.Part.Root; }
        }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public override RptPart Part
        {
            get { return TblPart.Part; }
        }

        /// <summary>
        /// 获取报表项所属父项
        /// </summary>
        public override RptItemBase Parent
        {
            get { return TblPart; }
        }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public override int Row
        {
            get 
            {
                for (int i = 0; i < TblPart.Rows.Count; i++) 
                {
                    if (this == TblPart.Rows[i]) 
                    {
                        return TblPart.Row + i;
                    }
                }
                return TblPart.Row;
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get { return TblPart.Table.Col; }
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
        public RptTable Table
        {
            get { return TblPart.Table; }
        }

        /// <summary>
        /// 获取所属的RptTblPart
        /// </summary>
        public RptTblPart TblPart { get; }

        public List<RptText> Cells { get; set; }

        protected override void ReadChildXml(XmlReader p_reader)
        {
            if (p_reader.Name == "Text")
            {
                if (Cells == null)
                    Cells = new List<RptText>();
                RptText cell = new RptText(this);
                cell.ReadXml(p_reader);
                Cells.Add(cell);
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("TRow");
            if (Cells != null)
            {
                foreach (RptText cell in Cells)
                {
                    cell.WriteXml(p_writer);
                }
            }
            p_writer.WriteEndElement();
        }
    }
}
