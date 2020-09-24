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
using System.Collections.Specialized;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 矩阵分层标题
    /// </summary>
    internal class RptMtxSubtitle : RptItemBase
    {
        #region 私有变量
        readonly RptItemBase _parent;
        #endregion

        #region 构造
        public RptMtxSubtitle(RptItemBase p_parent)
        {
            _parent = p_parent;
            Item = new RptText(this);
            SubTitles = new List<RptMtxSubtitle>();
            // 所占行数/列数
            _data.AddCell("span", 1);
        }
        #endregion

        #region 外部属性
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
        /// 获取标题列表
        /// </summary>
        public List<RptMtxSubtitle> SubTitles { get; set; }

        /// <summary>
        /// 获取对应的文本框
        /// </summary>
        public RptText Item { get; }

        /// <summary>
        /// 获取设置所占行数/列数
        /// </summary>
        public int Span
        {
            get { return _data.Int("span"); }
            set { _data["span"] = value; }
        }

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
                RptMtxSubtitle rootTitle = this;
                int subtitleCount = 1;
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
                        RptMtxSubtitle tmp = parent as RptMtxSubtitle;
                        int tmpIndex = tmp.SubTitles.IndexOf(rootTitle);
                        int rowcount = 0;
                        for (int i = 0; i < tmpIndex; i++)
                        {
                            rowcount += tmp.SubTitles[i].GetCount();
                        }
                        index += rowcount;
                        rootTitle = tmp;
                    }
                    subtitleCount++;
                    parent = parent.Parent;
                }
                int count = 0;
                if ((level.Parent as RptMtxHeader) is RptMtxRowHeader)
                {
                    foreach (RptMtxSubtitle title in level.SubTitles)
                    {
                        if (title == rootTitle)
                            break;
                        count += title.GetCount();
                    }

                    RptMtxHeader header = level.Parent as RptMtxHeader;
                    int subLevelSpan = 0;
                    if (header.Levels.Count > header.Levels.IndexOf(level) + 1)
                    {
                        RptMtxLevel subLevel = header.Levels[header.Levels.IndexOf(level) + 1];
                        subLevelSpan = subLevel.RowSpan + subLevel.GetTotalSpan(subLevel.SubTotals);
                    }
                    return level.Row + count + index + subLevelSpan;
                }
                else
                {
                    return level.Row + subtitleCount;
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
                RptMtxSubtitle rootTitle = this;
                int subtitleCount = 1;
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
                        RptMtxSubtitle tmp = parent as RptMtxSubtitle;
                        int tmpIndex = tmp.SubTitles.IndexOf(rootTitle);
                        int rowcount = 0;
                        for (int i = 0; i < tmpIndex; i++)
                        {
                            rowcount += tmp.SubTitles[i].GetCount();
                        }
                        index += rowcount;
                        rootTitle = tmp;
                    }
                    subtitleCount++;
                    parent = parent.Parent;
                }
                int count = 0;
                if ((level.Parent as RptMtxHeader) is RptMtxRowHeader)
                {
                    return level.Col + subtitleCount;
                }
                else
                {
                    foreach (RptMtxSubtitle title in level.SubTitles)
                    {
                        if (title == rootTitle)
                            break;
                        count += title.GetCount();
                    }

                    RptMtxHeader header = level.Parent as RptMtxHeader;
                    int subLevelSpan = 0;
                    if (header.Levels.Count > header.Levels.IndexOf(level) + 1)
                    {
                        RptMtxLevel subLevel = header.Levels[header.Levels.IndexOf(level) + 1];
                        subLevelSpan = subLevel.ColSpan + subLevel.GetTotalSpan(subLevel.SubTotals);
                    }
                    return level.Col + count + index + subLevelSpan;
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
        /// 克隆标题
        /// </summary>
        /// <param name="p_parent"></param>
        /// <returns></returns>
        public RptMtxSubtitle Clone(RptItemBase p_parent)
        {
            RptMtxSubtitle title = new RptMtxSubtitle(p_parent);
            title.Data.Copy(this.Data);
            title.Item.Data.Copy(Item.Data);
            foreach (RptMtxSubtitle sub in SubTitles)
            {
                title.SubTitles.Add(sub.Clone(title));
            }
            return title;
        }

        /// <summary>
        /// 获取当前占行（列）数
        /// </summary>
        /// <returns></returns>
        internal int GetCount()
        {
            if (SubTitles == null || SubTitles.Count == 0)
                return 1;
            int span = 0;
            foreach (RptMtxSubtitle title in SubTitles)
            {
                span += title.GetCount();
            }
            return span;
        }

        /// <summary>
        /// 获取关联数据行（行头时有效）
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
        /// 获取关联数据列（列头时有效）
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
                case "Subtitle":
                    if (SubTitles == null)
                        SubTitles = new List<RptMtxSubtitle>();
                    RptMtxSubtitle title = new RptMtxSubtitle(this);
                    title.ReadXml(p_reader);
                    SubTitles.Add(title);
                    break;
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Subtitle");
            if (Span != 1)
                p_writer.WriteAttributeString("span", _data.Str("span"));

            Item.WriteXml(p_writer);
            if (SubTitles != null && SubTitles.Count > 0)
            {
                foreach (RptMtxSubtitle title in SubTitles)
                {
                    title.WriteXml(p_writer);
                }
            }
            p_writer.WriteEndElement();
        }
        #endregion
    }
}
