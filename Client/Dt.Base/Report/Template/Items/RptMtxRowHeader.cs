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
    /// 矩阵行头
    /// </summary>
    internal class RptMtxRowHeader : RptMtxHeader
    {
        public RptMtxRowHeader(RptMatrix p_matrix)
            : base(p_matrix)
        {
        }

        public override int RowSpan
        {
            get
            {
                int span = 0;
                foreach (RptMtxSubtotal total in Levels[0].SubTotals)
                {
                    span += total.GetCount();
                }
                return Levels[0].RowSpan + span;
            }
            set { }
        }

        public override int ColSpan
        {
            get
            {
                int maxCol = 0;
                foreach (RptMtxLevel level in Levels)
                {
                    if (maxCol < level.Col + level.ColSpan)
                        maxCol = level.Col + level.ColSpan;
                    int totalCol = GetSubTotalMaxCol(level.SubTotals);
                    if (maxCol < totalCol)
                        maxCol = totalCol;
                    int titleCol = GetSubTitleMaxCol(level.SubTitles);
                    if (maxCol < titleCol)
                        maxCol = titleCol;
                }
                return maxCol - Col;
            }
            set { }
        }

        /// <summary>
        /// 克隆行头
        /// </summary>
        /// <param name="p_mtx"></param>
        /// <returns></returns>
        public RptMtxRowHeader Clone(RptMatrix p_mtx)
        {
            RptMtxRowHeader header = new RptMtxRowHeader(p_mtx);
            foreach(RptMtxLevel level in Levels)
            {
                header.Levels.Add(level.Clone(header));
            }
            return header;
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("RowHeader");
            WriteChildXml(p_writer);
            p_writer.WriteEndElement();
        }

        #region 私有方法
        int GetSubTotalMaxCol(List<RptMtxSubtotal> p_totals)
        {
            int span = 0;
            foreach (RptMtxSubtotal total in p_totals)
            {
                int tmpspan = 0;
                if (total.SubTotals != null && total.SubTotals.Count > 0)
                {
                    tmpspan = GetSubTotalMaxCol(total.SubTotals);
                }
                else
                {
                    tmpspan = total.Col + total.ColSpan;
                }
                if (span < tmpspan)
                    span = tmpspan;
            }
            return span;
        }

        int GetSubTitleMaxCol(List<RptMtxSubtitle> p_titles)
        {
            int span = 0;
            foreach (RptMtxSubtitle title in p_titles)
            {
                int tmpspan = 0;
                if (title.SubTitles != null && title.SubTitles.Count > 0)
                {
                    tmpspan = GetSubTitleMaxCol(title.SubTitles);
                }
                else
                {
                    tmpspan = title.Col + title.ColSpan;
                }
                if (span < tmpspan)
                    span = tmpspan;
            }
            return span;
        }
        #endregion
    }
}
