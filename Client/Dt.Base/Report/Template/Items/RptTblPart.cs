#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格容器基类，列头、表体、列尾、分组头尾
    /// </summary>
    public abstract class RptTblPart : RptItemBase
    {
        public RptTblPart(RptTable p_table)
        {
            Table = p_table;
            Rows = new List<RptTblPartRow>();
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
            get
            {
                if (this is RptTblColHeader)
                {
                    return Table.Row;
                }
                else if (this is RptTblRow)
                {
                    int count = 0;
                    if (Table.ColHeader != null)
                    {
                        count += Table.ColHeader.Rows.Count;
                    }
                    if (Table.Groups != null)
                    {
                        foreach (RptTblGroup grp in Table.Groups)
                        {
                            count += grp.Rows.Count;
                        }
                    }
                    return Table.Row + count;
                }
                else if (this is RptTblFooter)
                {
                    int count = 0;
                    if (Table.ColHeader != null)
                    {
                        count += Table.ColHeader.Rows.Count;
                    }
                    if (Table.Groups != null)
                    {
                        foreach (RptTblGroup grp in Table.Groups)
                        {
                            count += grp.Rows.Count;
                        }
                    }
                    if (Table.Body != null)
                    {
                        count += Table.Body.Rows.Count;
                    }
                    return Table.Row + count;
                }
                else if (this is RptTblGroup)
                {
                    int count = 0;
                    if (Table.ColHeader != null)
                    {
                        count += Table.ColHeader.Rows.Count;
                    }
                    foreach (RptTblGroup grp in Table.Groups) 
                    {
                        if (grp == this)
                            break;
                        if (grp != null) 
                        {
                            count += grp.Rows.Count;
                        }
                    }
                    return Table.Row + count;
                }
                return Table.Row;
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get { return Table.Col; }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占行数
        /// </summary>
        public override int RowSpan
        {
            get { return Rows.Count; }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占列数
        /// </summary>
        public override int ColSpan
        {
            get { return Table.ColSpan; }
            set { }
        }

        /// <summary>
        /// 获取所属表格
        /// </summary>
        public RptTable Table { get; }

        public List<RptTblPartRow> Rows { get; }

        /// <summary>
        /// 构造子元素
        /// </summary>
        protected async Task BuildChild()
        {
            foreach (RptTblPartRow row in Rows)
            {
                List<RptText> cells = row.Cells;
                if (cells != null && cells.Count > 0)
                {
                    foreach (RptText cell in cells)
                    {
                        await cell.Build();
                    }
                }
            }
        }

        /// <summary>
        /// 读取子元素xml，结束时定位在该子元素的末尾元素上
        /// </summary>
        /// <param name="p_reader"></param>
        protected override void ReadChildXml(XmlReader p_reader)
        {
            if (p_reader.Name == "TRow")
            {
                RptTblPartRow row = new RptTblPartRow(this);
                row.ReadXml(p_reader);
                Rows.Add(row);
            }
        }

        /// <summary>
        /// 序列化子元素
        /// </summary>
        /// <param name="p_writer"></param>
        protected void WriteChildXml(XmlWriter p_writer)
        {
            if (Rows.Count > 0)
            {
                foreach (RptTblPartRow row in Rows)
                {
                    row.WriteXml(p_writer);
                }
            }
        }
    }
}
