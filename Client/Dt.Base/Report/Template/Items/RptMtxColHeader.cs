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
    /// 矩阵列头
    /// </summary>
    internal class RptMtxColHeader : RptMtxHeader
    {
        public RptMtxColHeader(RptMatrix p_matrix)
            : base(p_matrix)
        {
        }

        public override int RowSpan
        {
            get
            {
                int maxRow = 0;
                foreach (RptMtxLevel level in Levels)
                {
                    if (maxRow < level.Row + level.RowSpan)
                        maxRow = level.Row + level.RowSpan;
                    int totalRow = GetSubTotalMaxRow(level.SubTotals);
                    if (maxRow < totalRow)
                        maxRow = totalRow;
                    int titleRow = GetSubTitleMaxRow(level.SubTitles);
                    if (maxRow < titleRow)
                        maxRow = titleRow;
                }
                return maxRow - Row;
            }
            set { }
        }

        public override int ColSpan
        {
            get
            {
                int span = 0;
                foreach (RptMtxSubtotal total in Levels[0].SubTotals)
                {
                    span += total.GetCount();
                }
                return Levels[0].ColSpan + span;
            }
            set { }
        }

        /// <summary>
        /// 克隆列头
        /// </summary>
        /// <param name="p_mtx"></param>
        /// <returns></returns>
        public RptMtxColHeader Clone(RptMatrix p_mtx)
        {
            RptMtxColHeader header = new RptMtxColHeader(p_mtx);
            foreach (RptMtxLevel level in Levels)
            {
                header.Levels.Add(level.Clone(header));
            }
            return header;
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("ColHeader");
            WriteChildXml(p_writer);
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 获取节点小计最大个数
        /// </summary>
        /// <param name="p_totals"></param>
        /// <returns></returns>
        int GetSubTotalMaxRow(List<RptMtxSubtotal> p_totals)
        {
            int num = 0;
            foreach (RptMtxSubtotal total in p_totals)
            {
                int tmpNum = 0;
                if (total.SubTotals != null && total.SubTotals.Count > 0)
                {
                    tmpNum = GetSubTotalMaxRow(total.SubTotals);
                }
                else
                {
                    tmpNum = total.Row + total.RowSpan;
                }
                if (num < tmpNum)
                    num = tmpNum;
            }
            return num;
        }

        /// <summary>
        /// 获取节点标题最大个数
        /// </summary>
        /// <param name="p_titles"></param>
        /// <returns></returns>
        int GetSubTitleMaxRow(List<RptMtxSubtitle> p_titles)
        {
            int num = 0;
            foreach (RptMtxSubtitle title in p_titles)
            {
                int tmpNum = 0;
                if (title.SubTitles != null && title.SubTitles.Count > 0)
                {
                    tmpNum = GetSubTitleMaxRow(title.SubTitles);
                }
                else
                {
                    tmpNum = title.Row + title.RowSpan;
                }
                if (num < tmpNum)
                    num = tmpNum;
            }
            return num;
        }
    }
}
