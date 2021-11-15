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
using System.Linq;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 矩阵分层小计
    /// </summary>
    internal class RptMtxSubtotal : RptItemBase
    {
        #region 私有变量
        readonly RptItemBase _parent;
        #endregion

        #region 构造
        public RptMtxSubtotal(RptItemBase p_parent)
        {
            _parent = p_parent;
            Item = new RptText(this);
            SubTotals = new List<RptMtxSubtotal>();

            // 是否在层次前面
            _data.AddCell("beforelevel", "1");
            // 所占行数/列数
            _data.AddCell("span", 1);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public override RptRoot Root
        {
            get { return _parent.Root; }
        }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public override RptPart Part
        {
            get { return _parent.Part; }
        }

        /// <summary>
        /// 获取报表项所属父项
        /// </summary>
        public override RptItemBase Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// 获取对应的文本框
        /// </summary>
        public RptText Item { get; }

        /// <summary>
        /// 获取设置小计位置
        /// </summary>
        public TotalLocation TotalLoc
        {
            get { return _data.Bool("beforelevel") ? TotalLocation.Before : TotalLocation.After; }
            set { _data["beforelevel"] = value == Report.TotalLocation.Before ? "1" : "0"; }
        }

        /// <summary>
        /// 获取设置所占行数/列数
        /// </summary>
        public int Span
        {
            get { return _data.Int("span"); }
            set { _data["span"] = value; }
        }

        /// <summary>
        /// 获取小计集合
        /// </summary>
        public List<RptMtxSubtotal> SubTotals { get; set; }

        /// <summary>
        /// 获取所属层
        /// </summary>
        public RptMtxLevel Level
        {
            get
            {
                RptItemBase parent = _parent;
                RptMtxLevel level = parent as RptMtxLevel;
                while (level == null)
                {
                    parent = parent.Parent;
                    level = parent as RptMtxLevel;
                }
                return level;
            }
        }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public override int Row
        {
            get
            {
                RptMtxSubtotal rootTotal = this;
                int subtotalCount = 0;
                int index = 0;
                RptItemBase parent = _parent;
                RptMtxLevel level = parent as RptMtxLevel;
                while (level == null)
                {
                    if (parent is RptMtxLevel)
                    {
                        level = parent as RptMtxLevel;
                        break;
                    }
                    else
                    {
                        RptMtxSubtotal tmp = parent as RptMtxSubtotal;
                        int tmpIndex = tmp.SubTotals.IndexOf(rootTotal);
                        int rowcount = 0;
                        for (int i = 0; i < tmpIndex; i++)
                        {
                            rowcount += tmp.SubTotals[i].GetCount();
                        }
                        index += rowcount;
                        rootTotal = tmp;
                    }
                    subtotalCount++;
                    parent = parent.Parent;
                }
                int count = 0;
                IEnumerable<RptMtxSubtotal> tmptotals = from c in level.SubTotals
                                                        where c.TotalLoc == rootTotal.TotalLoc
                                                        select c;
                if ((level.Parent as RptMtxHeader) is RptMtxRowHeader)
                {
                    if (rootTotal.TotalLoc == TotalLocation.Before)
                    {
                        List<RptMtxSubtotal> tmplist = tmptotals.ToList();
                        for (int i = tmplist.Count - 1; i >= 0; i--)
                        {
                            RptMtxSubtotal total = tmplist[i];
                            if (rootTotal == total)
                                break;
                            count += total.GetCount();
                        }
                        int span = rootTotal.GetCount();
                        if (span > 1)
                            span -= index;
                        return level.Row - count - span;
                    }
                    else
                    {
                        foreach (RptMtxSubtotal total in tmptotals)
                        {
                            if (total == rootTotal)
                                break;
                            count += total.GetCount();
                        }
                        return level.Row + count + level.RowSpan + index;
                    }
                }
                else
                {
                    return level.Row + subtotalCount;
                }
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get
            {
                RptMtxSubtotal rootTotal = this;
                int subtotalCount = 0;
                int index = 0;
                RptItemBase parent = _parent;
                RptMtxLevel level = parent as RptMtxLevel;
                while (level == null)
                {
                    if (parent is RptMtxLevel)
                    {
                        level = parent as RptMtxLevel;
                        break;
                    }
                    else
                    {
                        RptMtxSubtotal tmp = parent as RptMtxSubtotal;
                        int tmpIndex = tmp.SubTotals.IndexOf(rootTotal);
                        int rowcount = 0;
                        for (int i = 0; i < tmpIndex; i++)
                        {
                            rowcount += tmp.SubTotals[i].GetCount();
                        }
                        index += rowcount;
                        rootTotal = tmp;
                    }
                    subtotalCount++;
                    parent = parent.Parent;
                }
                int count = 0;
                IEnumerable<RptMtxSubtotal> tmptotals = from c in level.SubTotals
                                                        where c.TotalLoc == rootTotal.TotalLoc
                                                        select c;
                if ((level.Parent as RptMtxHeader) is RptMtxRowHeader)
                {
                    return level.Col + subtotalCount;
                }
                else
                {
                    if (rootTotal.TotalLoc == TotalLocation.Before)
                    {
                        List<RptMtxSubtotal> tmplist = tmptotals.ToList();
                        for (int i = tmplist.Count - 1; i >= 0; i--)
                        {
                            RptMtxSubtotal total = tmplist[i];
                            if (rootTotal == total)
                                break;
                            count += total.GetCount();
                        }
                        int span = rootTotal.GetCount();
                        if (span > 1)
                            span -= index;
                        return level.Col - count - span;
                    }
                    else
                    {
                        foreach (RptMtxSubtotal total in tmptotals)
                        {
                            if (total == rootTotal)
                                break;
                            count += total.GetCount();
                        }
                        return level.Col + level.ColSpan + count + index;
                    }
                }
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占行数
        /// </summary>
        public override int RowSpan
        {
            get { return Level.Parent is RptMtxRowHeader ? GetCount() : Span; }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占列数
        /// </summary>
        public override int ColSpan
        {
            get { return Level.Parent is RptMtxRowHeader ? Span : GetCount(); }
            set { }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 克隆小计
        /// </summary>
        /// <param name="p_parent"></param>
        /// <returns></returns>
        public RptMtxSubtotal Clone(RptItemBase p_parent)
        {
            RptMtxSubtotal total = new RptMtxSubtotal(p_parent);
            total.Data.Copy(this.Data);
            total.Item.Data.Copy(Item.Data);
            foreach (RptMtxSubtotal sub in SubTotals)
            {
                total.SubTotals.Add(sub.Clone(total));
            }
            return total;
        }

        /// <summary>
        /// 获取当前占行（列）数
        /// </summary>
        /// <returns></returns>
        internal int GetCount()
        {
            if (SubTotals == null || SubTotals.Count == 0)
                return 1;
            int span = 0;
            foreach (RptMtxSubtotal total in SubTotals)
            {
                span += total.GetCount();
            }
            return span;
        }

        /// <summary>
        /// 获取数据行（行头时有效）
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        internal List<RptMtxRow> GetRptRows(out int p_index)
        {
            if (Level.Parent is RptMtxRowHeader)
            {
                List<RptMtxRow> opsRows = new List<RptMtxRow>();
                RptMatrix mat = Level.Matrix;
                int curIndex = Row - (mat.Row + mat.ColHeader.RowSpan);
                for (int i = 0; i < RowSpan; i++)
                {
                    opsRows.Add(mat.Rows[curIndex + i]);
                }
                p_index = curIndex;
                return opsRows;
            }
            p_index = -1;
            return null;
        }

        /// <summary>
        /// 获取相关数据列（列头时有效）
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        internal Dictionary<RptMtxRow, List<RptText>> GetRptCells(out int p_index)
        {
            if (Level.Parent is RptMtxColHeader)
            {
                Dictionary<RptMtxRow, List<RptText>> opsCells = new Dictionary<RptMtxRow, List<RptText>>();
                RptMatrix mat = Level.Matrix;
                int curIndex = Col - (mat.Col + mat.RowHeader.ColSpan);
                List<RptText> cells;
                foreach (RptMtxRow row in mat.Rows)
                {
                    cells = new List<RptText>();
                    for (int i = 0; i < ColSpan; i++)
                    {
                        cells.Add(row.Cells[curIndex + i]);
                    }
                    opsCells.Add(row, cells);
                }
                p_index = curIndex;
                return opsCells;
            }
            p_index = -1;
            return null;
        }
        #endregion

        #region xml
        protected override void ReadChildXml(XmlReader p_reader)
        {
            switch (p_reader.Name)
            {
                case "Text":
                    Item.ReadXml(p_reader);
                    break;
                case "Subtotal":
                    if (SubTotals == null)
                        SubTotals = new List<RptMtxSubtotal>();
                    RptMtxSubtotal sub = new RptMtxSubtotal(this);
                    sub.ReadXml(p_reader);
                    SubTotals.Add(sub);
                    break;
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Subtotal");

            string val = _data.Str("beforelevel");
            if (val != "1")
                p_writer.WriteAttributeString("beforelevel", val);
            if (Span != 1)
                p_writer.WriteAttributeString("span", _data.Str("span"));

            Item.WriteXml(p_writer);
            if (SubTotals != null && SubTotals.Count > 0)
            {
                foreach (RptMtxSubtotal sub in SubTotals)
                {
                    sub.WriteXml(p_writer);
                }
            }
            p_writer.WriteEndElement();
        }
        #endregion
    }

    /// <summary>
    /// 小计位置
    /// </summary>
    public enum TotalLocation
    {
        /// <summary>
        /// 之前
        /// </summary>
        Before,

        /// <summary>
        /// 之后
        /// </summary>
        After,
    }
}
