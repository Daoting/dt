#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Core;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 添加小计命令
    /// </summary>
    internal class AddSubTitleCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubTitleCmdArgs args = (SubTitleCmdArgs)p_args;
            RptItemBase parent = args.Parent;
            RptMtxHeaderType headerType = RptMtxHeaderType.Col;
            RptMtxSubtitle title = args.SubTitle;
            RptMatrix mtx = null;
            if (title == null)
            {
                title = new RptMtxSubtitle(parent);
                args.InitData();
                if (parent is RptMtxLevel)
                {
                    RptMtxLevel pLevel = parent as RptMtxLevel;
                    mtx = pLevel.Matrix;
                    if (pLevel.Parent is RptMtxRowHeader)
                        headerType = RptMtxHeaderType.Row;
                    title.Item.Val = string.Format("subtitle{0}", mtx.GetMaxTitle());

                    RptMtxHeader header = pLevel.Parent as RptMtxHeader;
                    if (pLevel.SubTitles.Count > 0 || header.Levels.Count > header.Levels.IndexOf(pLevel) + 1)
                    {
                        int curIndex = 0;
                        if (headerType == RptMtxHeaderType.Row)
                        {
                            int subLevelSpan = 0;
                            if (header.Levels.Count > header.Levels.IndexOf(pLevel) + 1)
                                subLevelSpan = header.Levels[header.Levels.IndexOf(pLevel) + 1].RowSpan;

                            curIndex = pLevel.Row - mtx.ColHeader.Row - mtx.ColHeader.RowSpan + subLevelSpan + pLevel.GetTitleSpan(pLevel.SubTitles);
                            args.OpsRows.Add(InsertNewRow(mtx, curIndex));
                        }
                        else
                        {
                            int subLevelSpan = 0;
                            if (header.Levels.Count > header.Levels.IndexOf(pLevel) + 1)
                                subLevelSpan = header.Levels[header.Levels.IndexOf(pLevel) + 1].ColSpan;

                            curIndex = pLevel.Col - mtx.RowHeader.Col - mtx.RowHeader.ColSpan + subLevelSpan + pLevel.GetTitleSpan(pLevel.SubTitles);
                            foreach (RptMtxRow row in mtx.Rows)
                            {
                                args.OpsCells.Add(row, InsertNewCell(mtx, row, curIndex).ToList());
                            }
                        }
                        args.CurIndex = curIndex;
                    }
                    pLevel.SubTitles.Add(title);
                }
                else
                {
                    RptMtxSubtitle pTitle = parent as RptMtxSubtitle;
                    mtx = pTitle.Level.Matrix;
                    if (pTitle.Level.Parent is RptMtxRowHeader)
                        headerType = RptMtxHeaderType.Row;
                    title.Item.Val = string.Format("subtitle{0}", mtx.GetMaxTitle());
                    if (pTitle.SubTitles.Count > 0)
                    {
                        int curIndex = 0;
                        if (headerType == RptMtxHeaderType.Row)
                        {
                            curIndex = pTitle.Row + pTitle.RowSpan - mtx.ColHeader.Row - mtx.ColHeader.RowSpan;
                            args.OpsRows.Add(InsertNewRow(mtx, curIndex));
                        }
                        else
                        {
                            curIndex = pTitle.Col + pTitle.ColSpan - mtx.RowHeader.Col - mtx.RowHeader.ColSpan;
                            foreach (RptMtxRow row in mtx.Rows)
                            {
                                args.OpsCells.Add(row, InsertNewCell(mtx, row, curIndex).ToList());
                            }
                        }
                        args.CurIndex = curIndex;
                    }
                    pTitle.SubTitles.Add(title);
                }
                args.SubTitle = title;
            }
            else
            {
                mtx = title.Level.Matrix;
                if (parent is RptMtxLevel)
                {
                    args.BackMtxRow();
                    (parent as RptMtxLevel).SubTitles.Add(title);
                }
                else
                {
                    (parent as RptMtxSubtitle).SubTitles.Add(title);
                }
            }
            mtx.Update(true);
            return title;
        }

        public override void Undo(object p_args)
        {
            SubTitleCmdArgs args = (SubTitleCmdArgs)p_args;
            RptMtxSubtitle title = args.SubTitle;
            RptItemBase parent = args.Parent;
            RptMatrix mtx = null;
            if (parent is RptMtxLevel)
            {
                RptMtxLevel pLevel = parent as RptMtxLevel;
                mtx = pLevel.Matrix;
                args.DelMtxRows();
                pLevel.SubTitles.Remove(title);
            }
            else
            {
                RptMtxSubtitle pTitle = parent as RptMtxSubtitle;
                mtx = pTitle.Level.Matrix;
                args.DelMtxRows();
                pTitle.SubTitles.Remove(title);
            }
            mtx.Update(true);
        }

        /// <summary>
        /// 添加新行
        /// </summary>
        /// <param name="p_matrix">矩阵</param>
        /// <param name="p_index">插入位置</param>
        /// <returns>新增行</returns>
        RptMtxRow InsertNewRow(RptMatrix p_matrix, int p_index)
        {
            RptMtxRow row = new RptMtxRow(p_matrix);
            int rowCount = p_matrix.GetCellsCount();
            for (int i = 0; i < p_matrix.Rows[0].Cells.Count; i++)
            {
                RptText text = new RptText(row);
                text.Val = string.Format("cell{0}", (rowCount + i).ToString());
                row.Cells.Add(text);
            }
            p_matrix.Rows.Insert(p_index, row);
            return row;
        }

        /// <summary>
        /// 为当前行添加新列
        /// </summary>
        /// <param name="p_matrix">矩阵</param>
        /// <param name="p_row">所属行</param>
        /// <param name="p_index">插入位置</param>
        /// <returns>新增列的集合</returns>
        IEnumerable<RptText> InsertNewCell(RptMatrix p_matrix, RptMtxRow p_row, int p_index)
        {
            RptText text = new RptText(p_row);
            text.Val = string.Format("cell{0}", p_matrix.GetCellsCount().ToString());
            p_row.Cells.Insert(p_index, text);
            yield return text;
        }
    }

    /// <summary>
    /// 删除子层次命令
    /// </summary>
    internal class DelSubTitleCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubTitleCmdArgs args = (SubTitleCmdArgs)p_args;
            RptItemBase parent = args.Parent;
            RptMtxSubtitle title = args.SubTitle;
            RptMtxHeaderType headerType = RptMtxHeaderType.Col;
            RptMatrix mtx = null;
            args.InitData();
            if (parent is RptMtxLevel)
            {
                RptMtxLevel pLevel = parent as RptMtxLevel;
                mtx = pLevel.Matrix;
                if (pLevel.Parent is RptMtxRowHeader)
                    headerType = RptMtxHeaderType.Row;
                DelRows(args, headerType, mtx, title);
                args.TilteIndex = pLevel.SubTitles.IndexOf(title);
                pLevel.SubTitles.Remove(title);
            }
            else
            {
                RptMtxSubtitle pTotal = parent as RptMtxSubtitle;
                mtx = pTotal.Level.Matrix;
                if (pTotal.Level.Parent is RptMtxRowHeader)
                    headerType = RptMtxHeaderType.Row;
                DelRows(args, headerType, mtx, title);
                args.TilteIndex = pTotal.SubTitles.IndexOf(title);
                pTotal.SubTitles.Remove(title);
            }
            mtx.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            SubTitleCmdArgs args = (SubTitleCmdArgs)p_args;
            RptMtxSubtitle title = args.SubTitle;
            RptItemBase parent = args.Parent;
            RptMatrix mtx = null;
            if (parent is RptMtxLevel)
            {
                RptMtxLevel pLevel = parent as RptMtxLevel;
                mtx = pLevel.Matrix;
                args.BackMtxRow();
                pLevel.SubTitles.Insert(args.TilteIndex, title);
            }
            else
            {
                RptMtxSubtitle pTotal = parent as RptMtxSubtitle;
                mtx = pTotal.Level.Matrix;
                args.BackMtxRow();
                pTotal.SubTitles.Insert(args.TilteIndex, title);
            }
            mtx.Update(true);
        }

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="p_args"></param>
        /// <param name="p_headerType"></param>
        /// <param name="p_mat"></param>
        /// <param name="p_title"></param>
        void DelRows(SubTitleCmdArgs p_args, RptMtxHeaderType p_headerType, RptMatrix p_mat, RptMtxSubtitle p_title)
        {
            if ((p_title.Parent is RptMtxLevel && (p_title.Parent as RptMtxLevel).SubTitles.Count > 1)
                || (p_title.Parent is RptMtxSubtitle && (p_title.Parent as RptMtxSubtitle).SubTitles.Count > 1))
            {
                if (p_headerType == RptMtxHeaderType.Row)
                {
                    int index = -1;
                    List<RptMtxRow> opsRows = p_title.GetRptRows(out index);
                    if (opsRows != null)
                    {
                        p_args.OpsRows = opsRows;
                        p_args.CurIndex = index;
                        p_mat.Rows.RemoveRange(index, p_title.RowSpan);
                    }
                }
                else
                {
                    int index = -1;
                    Dictionary<RptMtxRow, List<RptText>> opsCells = p_title.GetRptCells(out index);
                    if (opsCells != null)
                    {
                        foreach (RptMtxRow row in opsCells.Keys)
                        {
                            row.Cells.RemoveRange(index, p_title.ColSpan);
                        }
                        p_args.CurIndex = index;
                        p_args.OpsCells = opsCells;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 标题跨度命令
    /// </summary>
    internal class SubTitleSpanCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubTitleCmdArgs args = (SubTitleCmdArgs)p_args;
            RptMtxSubtitle title = args.SubTitle;
            RptMatrix mtx = title.Level.Matrix;
            args.OldSpan = title.Span;
            title.Span = args.NewSpan;
            mtx.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            SubTitleCmdArgs args = (SubTitleCmdArgs)p_args;
            RptMtxSubtitle title = args.SubTitle;
            RptMatrix mtx = title.Level.Matrix;
            title.Span = args.OldSpan;
            mtx.Update(true);
        }
    }

    /// <summary>
    /// 标题参数
    /// </summary>
    internal class SubTitleCmdArgs
    {
        public SubTitleCmdArgs(RptItemBase p_parent, RptMtxSubtitle p_title = null, int p_newSpan = -1)
        {
            Parent = p_parent;
            SubTitle = p_title;
            NewSpan = p_newSpan;
            InitData();
        }

        public RptItemBase Parent { get; }

        public RptMtxSubtitle SubTitle { get; set; }

        /// <summary>
        /// 获取设置操作的标题位置
        /// </summary>
        public int TilteIndex { get; set; }

        /// <summary>
        /// 获取设置行、列插入位置
        /// </summary>
        public int CurIndex { get; set; }

        /// <summary>
        /// 获取设置操作影响的数据行（行头操作有效）
        /// </summary>
        public List<RptMtxRow> OpsRows { get; set; }

        /// <summary>
        /// 获取设置操作影响的列（列头操作有效）
        /// </summary>
        public Dictionary<RptMtxRow, List<RptText>> OpsCells { get; set; }

        /// <summary>
        /// 获取设置上一次跨度
        /// </summary>
        public int OldSpan { get; set; }

        /// <summary>
        /// 获取设置新跨度
        /// </summary>
        public int NewSpan { get; set; }

        /// <summary>
        /// 初始化数据集合
        /// </summary>
        public void InitData()
        {
            OpsRows = new List<RptMtxRow>();
            OpsCells = new Dictionary<RptMtxRow, List<RptText>>();
        }

        /// <summary>
        /// 删除对应数据行
        /// </summary>
        public void DelMtxRows()
        {
            RptMtxHeader header = SubTitle.Level.Parent as RptMtxHeader;
            RptMatrix mat = header.Parent as RptMatrix;
            if (header is RptMtxRowHeader)
            {
                if (OpsRows.Count == 0)
                    return;
                mat.Rows.RemoveRange(CurIndex, OpsRows.Count);
            }
            else
            {
                if (OpsCells.Count == 0)
                    return;
                foreach (RptMtxRow row in OpsCells.Keys)
                {
                    row.Cells.RemoveRange(CurIndex, OpsCells[row].Count());
                }
            }
        }

        /// <summary>
        /// 撤消操作时回退数据
        /// </summary>
        public void BackMtxRow()
        {
            RptMtxHeader header = SubTitle.Level.Parent as RptMtxHeader;
            RptMatrix mat = header.Parent as RptMatrix;
            if (header is RptMtxRowHeader)
            {
                if (OpsRows.Count == 0)
                    return;
                mat.Rows.InsertRange(CurIndex, OpsRows);
            }
            else
            {
                if (OpsCells.Count == 0)
                    return;
                foreach (RptMtxRow row in mat.Rows)
                {
                    row.Cells.InsertRange(CurIndex, OpsCells[row]);
                }
            }
        }
    }
}
