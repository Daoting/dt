#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格
    /// </summary>
    internal class RptTable : RptItem
    {
        public RptTable(RptPart p_owner)
            : base(p_owner)
        {
            // 数据源名称
            _data.AddCell<string>("tbl");
            // 在每页重复表头表尾
            _data.AddCell<bool>("repeatheader");
            _data.AddCell<bool>("repeatfooter");
            // 最少行数
            _data.AddCell("minrowcount", 0);
            // 多列显示
            _data.AddCell("rowbreakcount", 0);
            _data.AddCell("colbreakcount", 1);
        }

        /// <summary>
        /// 获取设置表头
        /// </summary>
        public RptTblHeader Header { get; set; }

        /// <summary>
        /// 获取设置数据行
        /// </summary>
        public RptTblRow Body { get; set; }

        /// <summary>
        /// 获取设置表尾
        /// </summary>
        public RptTblFooter Footer { get; set; }

        /// <summary>
        /// 获取设置所有分组
        /// </summary>
        public List<RptTblGroup> Groups { get; set; }

        /// <summary>
        /// 获取设置是否重复表头
        /// </summary>
        public bool RepeatHeader
        {
            get { return _data.Bool("repeatheader"); }
        }

        /// <summary>
        /// 获取设置是否重复表尾
        /// </summary>
        public bool RepeatFooter
        {
            get { return _data.Bool("repeatfooter"); }
        }

        /// <summary>
        /// 获取表格显示多少行数据后换列显示，默认0
        /// 0： 表示从不换列
        /// -1：填满页面后自动换列
        /// n： n行数据后换列
        /// </summary>
        public int RowBreakCount
        {
            get { return _data.Int("rowbreakcount"); }
        }

        /// <summary>
        /// 获取表格重复的列数，达到后换行显示，在RowBreakCount非0时有效
        /// </summary>
        public int ColBreakCount
        {
            get { return _data.Int("colbreakcount"); }
        }

        /// <summary>
        /// 获取数据区域最少行数，不够时增加空行
        /// </summary>
        public int MinRowCount
        {
            get { return _data.Int("minrowcount"); }
        }

        /// <summary>
        /// 获取数据源名称
        /// </summary>
        public string Tbl
        {
            get { return _data.Str("tbl"); }
        }

        /// <summary>
        /// 构造报表项实例
        /// </summary>
        public override async Task Build()
        {
            RptRootInst inst = _part.Inst;
            string tblName = _data.Str("tbl");
            if (string.IsNullOrEmpty(tblName))
                return;

            // 使用时再加载数据
            var rptData = await inst.Info.GetData(tblName);
            if (rptData == null)
                return;

            inst.CurrentParent = null;
            RptTableInst tbl = new RptTableInst(this);
            inst.Body.AddChild(tbl);
            inst.CurrentTable = tbl;
            tbl.Data = rptData;
            Table data = tbl.Data.Data;

            if (Header != null && Header.Rows.Count > 0)
                await Header.Build();

            if (Body != null)
            {
                for (int num = 0; num < data.Count; num++)
                {
                    RptTblGroup group;
                    Row curRow = data[num];
                    Row preRow = null;
                    Row nextRow = null;
                    if (num > 0)
                        preRow = data[num - 1];
                    if (num < data.Count - 1)
                        nextRow = data[num + 1];

                    // 分组头
                    if (Groups != null)
                    {
                        for (int i = 0; i < Groups.Count; i++)
                        {
                            group = Groups[i];
                            string field = group.Field;
                            if (!string.IsNullOrEmpty(field)
                                && (preRow == null || curRow.Str(field) != preRow.Str(field)))
                            {
                                for (int j = i; j < Groups.Count; j++)
                                {
                                    group = Groups[j];
                                    if (group.Header != null && group.Header.Rows.Count > 0)
                                    {
                                        Dictionary<string, string> dict = GetFilters(j, curRow);
                                        await group.Header.Build(dict);
                                    }
                                }
                                break;
                            }
                        }
                    }

                    await Body.Build();

                    // 分组尾
                    if (Groups != null)
                    {
                        for (int i = 0; i < Groups.Count; i++)
                        {
                            group = Groups[i];
                            string field = group.Field;
                            if (!string.IsNullOrEmpty(field)
                                && (nextRow == null || curRow.Str(field) != nextRow.Str(field)))
                            {
                                for (int j = Groups.Count - 1; j >= i; j--)
                                {
                                    group = Groups[j];
                                    if (group.Footer != null && group.Footer.Rows.Count > 0)
                                    {
                                        Dictionary<string, string> dict = GetFilters(j, curRow);
                                        await group.Footer.Build(dict);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

                // 未达到最少行数时增加空行
                int minCount = MinRowCount;
                if (minCount > 0 && minCount > data.Count)
                {
                    int tail = minCount - data.Count;
                    for (int i = 0; i < tail; i++)
                    {
                        await Body.Build();
                    }
                }
            }

            if (Footer != null && Footer.Rows.Count > 0)
                await Footer.Build();
        }

        /// <summary>
        /// 获取指定位置所属区域类型
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        public TblRangeType GetRangeType(int p_row, int p_col)
        {
            if (Contains(p_row, p_col, 1, 1))
            {
                if (Header != null)
                {
                    int headerRow = Header.Row;
                    if (headerRow <= p_row && p_row < headerRow + Header.RowSpan)
                    {
                        return TblRangeType.Header;
                    }
                }
                if (Body != null)
                {
                    int bodyRow = Body.Row;
                    if (bodyRow <= p_row && p_row < bodyRow + Body.RowSpan)
                    {
                        return TblRangeType.Body;
                    }
                }
                if (Footer != null)
                {
                    int footerRow = Footer.Row;
                    if (footerRow <= p_row && p_row < footerRow + Footer.RowSpan)
                    {
                        return TblRangeType.Footer;
                    }
                }
                return TblRangeType.Group;
            }
            return TblRangeType.Outer;
        }

        /// <summary>
        /// 获取内部指定位置的文本
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        public RptText GetText(int p_row, int p_col)
        {
            TblRangeType tblType = GetRangeType(p_row, p_col);
            switch (tblType)
            {
                case TblRangeType.Header:
                    {
                        return Header.Rows[p_row - Header.Row].Cells[p_col - Col];
                    }
                case TblRangeType.Group:
                    {
                        foreach (RptTblGroup grp in Groups)
                        {
                            if (grp.Header != null)
                            {
                                int hRow = grp.Header.Row;
                                if (hRow <= p_row && p_row < hRow + grp.Header.RowSpan)
                                {
                                    return grp.Header.Rows[p_row - hRow].Cells[p_col - Col];
                                }
                            }
                            if (grp.Footer != null)
                            {
                                int fRow = grp.Footer.Row;
                                if (fRow <= p_row && p_row < fRow + grp.Footer.RowSpan)
                                {
                                    return grp.Footer.Rows[p_row - fRow].Cells[p_col - Col];
                                }
                            }
                        }
                        return null;
                    }
                case TblRangeType.Body:
                    {
                        return Body.Rows[p_row - Body.Row].Cells[p_col - Col];
                    }
                case TblRangeType.Footer:
                    {
                        return Footer.Rows[p_row - Footer.Row].Cells[p_col - Col];
                    }
                case TblRangeType.Outer:
                default:
                    return null;
            }
        }

        public void CalcRowSpan()
        {
            int count = 0;
            if (Header != null)
            {
                count += Header.Rows.Count;
            }
            if (Body != null)
            {
                count += Body.Rows.Count;
            }
            if (Footer != null)
            {
                count += Footer.Rows.Count;
            }
            if (Groups != null)
            {
                foreach (RptTblGroup grp in Groups)
                {
                    if (grp.Header != null)
                    {
                        count += grp.Header.Rows.Count;
                    }
                    if (grp.Footer != null)
                    {
                        count += grp.Footer.Rows.Count;
                    }
                }
            }
            Data["rowspan"] = count;
        }

        /// <summary>
        /// 拷贝对象方法
        /// </summary>
        /// <returns></returns>
        public override RptItem Clone()
        {
            RptTable newOne = new RptTable(this._part);
            newOne._data.Copy(this._data);
            CopyTableStruct(this, newOne);

            return newOne as RptItem;
        }

        void CopyTableStruct(RptTable p_source, RptTable p_dest)
        {
            int rowCount = 0;
            RptTblPartRow tmpRow = null;

            if (p_source.Header != null && (rowCount = p_source.Header.Rows.Count) > 0)
            {
                RptTblHeader header = new RptTblHeader(p_dest);
                for (int i = 0; i < rowCount; i++)
                {
                    tmpRow = new RptTblPartRow(header);
                    header.Rows.Add(CopyTableRow(this.Header.Rows[i], tmpRow));
                }
                p_dest.Header = header;
            }

            if (p_source.Body != null && (rowCount = p_source.Body.Rows.Count) > 0)
            {
                RptTblRow body = new RptTblRow(p_dest);
                for (int i = 0; i < rowCount; i++)
                {
                    tmpRow = new RptTblPartRow(body);
                    body.Rows.Add(CopyTableRow(this.Body.Rows[i], tmpRow));
                }
                p_dest.Body = body;
            }

            if (p_source.Footer != null && (rowCount = p_source.Footer.Rows.Count) > 0)
            {
                RptTblFooter foot = new RptTblFooter(p_dest);
                for (int i = 0; i < rowCount; i++)
                {
                    tmpRow = new RptTblPartRow(foot);
                    foot.Rows.Add(CopyTableRow(this.Footer.Rows[i], tmpRow));
                }
                p_dest.Footer = foot;
            }

            int grpCount = 0;
            if (p_source.Groups != null && (grpCount = p_source.Groups.Count) > 0)
            {
                RptTblGroup grp = null;
                RptTblGroupHeader grpHeader = null;
                RptTblGroupFooter grpFooter = null;
                RptTblGroup newGrp = null;
                RptTblGroupHeader newGrpHeader = null;
                RptTblGroupFooter newGrpFooter = null;
                if (p_dest.Groups == null)
                {
                    p_dest.Groups = new List<RptTblGroup>();
                }

                for (int j = 0; j < grpCount; j++)
                {
                    grp = p_source.Groups[j];
                    newGrp = new RptTblGroup(p_dest);
                    p_dest.Groups.Add(newGrp);
                    newGrp.Field = grp.Field;
                    if ((grpHeader = grp.Header) != null && (rowCount = grpHeader.Rows.Count) > 0)
                    {
                        newGrpHeader = new RptTblGroupHeader(p_dest);
                        for (int i = 0; i < rowCount; i++)
                        {
                            tmpRow = new RptTblPartRow(newGrpHeader);
                            newGrpHeader.Rows.Add(CopyTableRow(grpHeader.Rows[i], tmpRow));
                        }
                        newGrp.Header = newGrpHeader;
                    }

                    if ((grpFooter = grp.Footer) != null && (rowCount = grpFooter.Rows.Count) > 0)
                    {
                        newGrpFooter = new RptTblGroupFooter(p_dest);
                        for (int i = 0; i < rowCount; i++)
                        {
                            tmpRow = new RptTblPartRow(grpFooter);
                            newGrpFooter.Rows.Add(CopyTableRow(grpFooter.Rows[i], tmpRow));
                        }
                        newGrp.Footer = newGrpFooter;
                    }
                }
            }
        }

        RptTblPartRow CopyTableRow(RptTblPartRow p_source, RptTblPartRow p_dest)
        {
            RptText text = null;
            RptText newTxt = null;
            for (int i = 0; i < p_source.Cells.Count; i++)
            {
                text = p_source.Cells[i];
                newTxt = (RptText)text.Clone();
                newTxt.SetParentItem(p_dest);
                p_dest.Cells.Add(newTxt);
            }

            return p_dest;
        }

        /// <summary>
        /// 获得过滤参数
        /// </summary>
        /// <param name="p_cur"></param>
        /// <param name="p_curRow"></param>
        /// <returns></returns>
        Dictionary<string, string> GetFilters(int p_cur, Row p_curRow)
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();
            for (int i = 0; i <= p_cur; i++)
            {
                string filed = Groups[i].Field;
                filters.Add(filed, p_curRow.Str(filed));
            }
            return filters;
        }

        #region xml
        protected override void ReadChildXml(XmlReader p_reader)
        {
            switch (p_reader.Name)
            {
                case "THeader":
                    Header = new RptTblHeader(this);
                    Header.ReadXml(p_reader);
                    break;
                case "TBody":
                    Body = new RptTblRow(this);
                    Body.ReadXml(p_reader);
                    break;
                case "TFooter":
                    Footer = new RptTblFooter(this);
                    Footer.ReadXml(p_reader);
                    break;
                case "TGroup":
                    if (Groups == null)
                        Groups = new List<RptTblGroup>();
                    RptTblGroup group = new RptTblGroup(this);
                    group.ReadXml(p_reader);
                    Groups.Add(group);
                    break;
                default:
                    break;
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Table");
            WritePosition(p_writer);

            string val = _data.Str("tbl");
            if (val != "")
                p_writer.WriteAttributeString("tbl", val);
            if (RepeatHeader)
                p_writer.WriteAttributeString("repeatheader", "True");
            if (RepeatFooter)
                p_writer.WriteAttributeString("repeatfooter", "True");
            val = _data.Str("minrowcount");
            if (val != "0")
                p_writer.WriteAttributeString("minrowcount", val);
            val = _data.Str("rowbreakcount");
            if (val != "0")
                p_writer.WriteAttributeString("rowbreakcount", val);
            val = _data.Str("colbreakcount");
            if (val != "1")
                p_writer.WriteAttributeString("colbreakcount", val);

            if (Header != null)
                Header.WriteXml(p_writer);
            if (Body != null)
                Body.WriteXml(p_writer);
            if (Footer != null)
                Footer.WriteXml(p_writer);
            if (Groups != null)
            {
                foreach (RptTblGroup group in Groups)
                {
                    group.WriteXml(p_writer);
                }
            }
            p_writer.WriteEndElement();
        }
        #endregion
    }

    /// <summary>
    /// 表格区域类型
    /// </summary>
    public enum TblRangeType
    {
        /// <summary>
        /// 表头
        /// </summary>
        Header,

        /// <summary>
        /// 分组
        /// </summary>
        Group,

        /// <summary>
        /// 数据区
        /// </summary>
        Body,

        /// <summary>
        /// 表尾
        /// </summary>
        Footer,

        /// <summary>
        /// 外部
        /// </summary>
        Outer
    }
}
