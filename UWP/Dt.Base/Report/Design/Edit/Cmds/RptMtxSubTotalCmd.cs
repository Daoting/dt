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
    internal class AddSubTotalCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptItemBase parent = args.Parent;
            RptMtxHeaderType headerType = RptMtxHeaderType.Col;
            RptMtxSubtotal total = args.SubTotal;
            RptMatrix mtx = null;
            if (total == null)
            {
                total = new RptMtxSubtotal(parent);
                args.InitData();
                if (parent is RptMtxLevel)
                {
                    RptMtxLevel pLevel = parent as RptMtxLevel;
                    mtx = pLevel.Matrix;
                    if (pLevel.Parent is RptMtxRowHeader)
                        headerType = RptMtxHeaderType.Row;
                    total.Item.Val = string.Format("subtotal{0}", mtx.GetMaxTotal());
                    int curIndex = 0;
                    if (headerType == RptMtxHeaderType.Row)
                    {
                        curIndex = pLevel.Row;
                        if (!mtx.HideRowHeader && !mtx.HideColHeader)
                            curIndex -= (mtx.Corner.Row + mtx.Corner.RowSpan);
                        args.OpsRows.Add(InsertNewRow(mtx, curIndex));
                    }
                    else
                    {
                        curIndex = pLevel.Col - mtx.Rows[0].Col;
                        foreach (RptMtxRow row in mtx.Rows)
                        {
                            args.OpsCells.Add(row, InsertNewCell(mtx, row, curIndex).ToList());
                        }
                    }
                    args.CurIndex = curIndex;
                }
                else
                {
                    RptMtxSubtotal pTotal = parent as RptMtxSubtotal;
                    mtx = pTotal.Level.Matrix;
                    if (pTotal.Level.Parent is RptMtxRowHeader)
                        headerType = RptMtxHeaderType.Row;
                    total.Item.Val = string.Format("subtotal{0}", mtx.GetMaxTotal());
                    if (pTotal.SubTotals.Count > 0)
                    {
                        int curIndex = 0;
                        if (headerType == RptMtxHeaderType.Row)
                        {
                            curIndex = pTotal.Row  + pTotal.GetCount();
                            if(!mtx.HideColHeader && !mtx.HideRowHeader)
                            {
                                curIndex -= (mtx.Corner.Row + mtx.Corner.RowSpan);
                            }
                            args.OpsRows.Add(InsertNewRow(mtx, curIndex));
                        }
                        else
                        {
                            curIndex = pTotal.Col - mtx.Rows[0].Col + pTotal.GetCount();
                            foreach (RptMtxRow row in mtx.Rows)
                            {
                                args.OpsCells.Add(row, InsertNewCell(mtx, row, curIndex).ToList());
                            }
                        }
                        args.CurIndex = curIndex;
                    }
                }
            }
            else
            {
                mtx = total.Level.Matrix;
                args.BackMtxRow();
            }
            if (parent is RptMtxLevel)
            {
                (parent as RptMtxLevel).SubTotals.Add(total);
            }
            else
            {
                (parent as RptMtxSubtotal).SubTotals.Add(total);
            }
            args.SubTotal = total;
            mtx.Update(true);
            return total;
        }

        public override void Undo(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptMtxSubtotal total = args.SubTotal;
            RptItemBase parent = args.Parent;
            RptMatrix mtx = null;
            if (parent is RptMtxLevel)
            {
                RptMtxLevel pLevel = parent as RptMtxLevel;
                mtx = pLevel.Matrix;
                args.DelMtxRows();
                pLevel.SubTotals.Remove(total);
            }
            else
            {
                RptMtxSubtotal pTotal = parent as RptMtxSubtotal;
                mtx = pTotal.Level.Matrix;
                args.DelMtxRows();
                pTotal.SubTotals.Remove(total);
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
    /// 删除小计命令
    /// </summary>
    internal class DelSubTotalCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptItemBase parent = args.Parent;
            RptMtxSubtotal total = args.SubTotal;
            RptMtxHeaderType headerType = RptMtxHeaderType.Col;
            RptMatrix mtx = null;
            args.InitData();
            if (parent is RptMtxLevel)
            {
                RptMtxLevel pLevel = parent as RptMtxLevel;
                mtx = pLevel.Matrix;
                if (pLevel.Parent is RptMtxRowHeader)
                    headerType = RptMtxHeaderType.Row;
                DelRows(args, headerType, mtx, total);
                args.TotalIndex = pLevel.SubTotals.IndexOf(total);
                pLevel.SubTotals.Remove(total);
            }
            else
            {
                RptMtxSubtotal pTotal = parent as RptMtxSubtotal;
                mtx = pTotal.Level.Matrix;
                if (pTotal.Level.Parent is RptMtxRowHeader)
                    headerType = RptMtxHeaderType.Row;
                DelRows(args, headerType, mtx, total);
                args.TotalIndex = pTotal.SubTotals.IndexOf(total);
                pTotal.SubTotals.Remove(total);
            }
            mtx.Update(true);
            return total;
        }

        public override void Undo(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptMtxSubtotal total = args.SubTotal;
            RptItemBase parent = args.Parent;
            RptMatrix mtx = null;
            if (parent is RptMtxLevel)
            {
                RptMtxLevel pLevel = parent as RptMtxLevel;
                mtx = pLevel.Matrix;
                args.BackMtxRow();
                pLevel.SubTotals.Insert(args.TotalIndex, total);
            }
            else
            {
                RptMtxSubtotal pTotal = parent as RptMtxSubtotal;
                mtx = pTotal.Level.Matrix;
                args.BackMtxRow();
                pTotal.SubTotals.Insert(args.TotalIndex, total);
            }
            mtx.Update(true);
        }

        /// <summary>
        /// 删除对应数据
        /// </summary>
        /// <param name="p_args"></param>
        /// <param name="p_headerType"></param>
        /// <param name="p_mat"></param>
        /// <param name="p_total"></param>
        void DelRows(SubTotalCmdArgs p_args, RptMtxHeaderType p_headerType, RptMatrix p_mat, RptMtxSubtotal p_total)
        {
            if (p_total.Parent is RptMtxLevel || (p_total.Parent as RptMtxSubtotal).SubTotals.Count > 1)
            {
                if (p_headerType == RptMtxHeaderType.Row)
                {
                    int index = -1;
                    List<RptMtxRow> opsRows = p_total.GetRptRows(out index);
                    if (opsRows != null)
                    {
                        p_args.OpsRows = opsRows;
                        p_args.CurIndex = index;
                        p_mat.Rows.RemoveRange(index, p_total.RowSpan);
                    }
                }
                else
                {
                    int index = -1;
                    Dictionary<RptMtxRow, List<RptText>> opsCells = p_total.GetRptCells(out index);
                    if (opsCells != null)
                    {
                        foreach (RptMtxRow row in opsCells.Keys)
                        {
                            row.Cells.RemoveRange(index, p_total.ColSpan);
                        }
                        p_args.CurIndex = index;
                        p_args.OpsCells = opsCells;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 更改小计位置命令
    /// </summary>
    internal class ChangeSubTotalLocCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptMtxLevel pLevel = args.Parent as RptMtxLevel;
            RptMtxSubtotal total = args.SubTotal;
            RptMatrix mtx = pLevel.Matrix;
            if (pLevel != null)
            {
                int count = 0;
                int newIndex = 0;
                int index = 0;
                if (pLevel.Parent is RptMtxRowHeader)
                {
                    args.OpsRows = total.GetRptRows(out index);
                    mtx.Rows.RemoveRange(index, args.OpsRows.Count);
                    if (total.TotalLoc == TotalLocation.Before)
                    {
                        count = (from c in pLevel.SubTotals
                                 where c.TotalLoc == total.TotalLoc
                                        && pLevel.SubTotals.IndexOf(c) >= pLevel.SubTotals.IndexOf(total)
                                 select c).Count();
                        newIndex = pLevel.Row - count - (mtx.Row + mtx.ColHeader.RowSpan);
                    }
                    else
                    {
                        count = (from c in pLevel.SubTotals
                                 where c.TotalLoc == total.TotalLoc
                                        && pLevel.SubTotals.IndexOf(c) < pLevel.SubTotals.IndexOf(total)
                                 select c).Count();
                        newIndex = pLevel.Row + pLevel.RowSpan + count - (mtx.Row + mtx.ColHeader.RowSpan);
                    }
                    mtx.Rows.InsertRange(newIndex, args.OpsRows);
                }
                else
                {
                    args.OpsCells = total.GetRptCells(out index);
                    foreach (RptMtxRow row in args.OpsCells.Keys)
                    {
                        row.Cells.RemoveRange(index, args.OpsCells[row].Count);
                        if (total.TotalLoc == TotalLocation.Before)
                        {
                            count = (from c in pLevel.SubTotals
                                     where c.TotalLoc == total.TotalLoc
                                            && pLevel.SubTotals.IndexOf(c) >= pLevel.SubTotals.IndexOf(total)
                                     select c).Count();
                            newIndex = pLevel.Col - count - (mtx.Col + mtx.RowHeader.ColSpan);
                        }
                        else
                        {
                            count = (from c in pLevel.SubTotals
                                     where c.TotalLoc == total.TotalLoc
                                            && pLevel.SubTotals.IndexOf(c) < pLevel.SubTotals.IndexOf(total)
                                     select c).Count();
                            newIndex = pLevel.Col + pLevel.ColSpan + count - (mtx.Col + mtx.RowHeader.ColSpan);
                        }
                        row.Cells.InsertRange(newIndex, args.OpsCells[row]);
                    }
                }
                args.BefIndex = index;
                args.CurIndex = newIndex;
            }
            mtx.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptMtxLevel pLevel = args.Parent as RptMtxLevel;
            RptMtxSubtotal total = args.SubTotal;
            RptMatrix mtx = pLevel.Matrix;
            total.TotalLoc = total.TotalLoc == TotalLocation.After ? TotalLocation.Before : TotalLocation.After;
            if (pLevel != null)
            {
                if (pLevel.Parent is RptMtxRowHeader)
                {
                    mtx.Rows.RemoveRange(args.CurIndex, args.OpsRows.Count);
                    mtx.Rows.InsertRange(args.BefIndex, args.OpsRows);
                }
                else
                {
                    foreach (RptMtxRow row in args.OpsCells.Keys)
                    {
                        row.Cells.RemoveRange(args.CurIndex, args.OpsCells[row].Count);
                        row.Cells.InsertRange(args.BefIndex, args.OpsCells[row]);
                    }
                }
            }
            mtx.Update(true);
        }
    }

    /// <summary>
    /// 小计跨度命令
    /// </summary>
    internal class SubTotalSpanCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptMtxSubtotal total = args.SubTotal;
            RptMatrix mtx = total.Level.Matrix; 
            args.OldSpan = total.Span;
            total.Span = args.NewSpan;
            mtx.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            SubTotalCmdArgs args = (SubTotalCmdArgs)p_args;
            RptMtxSubtotal total = args.SubTotal;
            RptMatrix mtx = total.Level.Matrix;
            total.Span = args.OldSpan;
            mtx.Update(true);
        }
    }

    internal class SubTotalCmdArgs
    {
        /// <summary>
        /// 小计参数
        /// </summary>
        /// <param name="p_parent">小计所属上级</param>
        /// <param name="p_total">当前操作的小计</param>
        /// <param name="p_newSpan"></param>
        public SubTotalCmdArgs(RptItemBase p_parent, RptMtxSubtotal p_total = null, int p_newSpan = -1)
        {
            Parent = p_parent;
            SubTotal = p_total;
            NewSpan = p_newSpan;
            InitData();
        }

        /// <summary>
        /// 获取设置父对象
        /// </summary>
        public RptItemBase Parent { get; set; }

        /// <summary>
        /// 获取设置操作对象
        /// </summary>
        public RptMtxSubtotal SubTotal { get; set; }

        /// <summary>
        /// 获取设置操作对象在父对象中的索引
        /// </summary>
        public int TotalIndex { get; set; }

        /// <summary>
        /// 获取设置行、列上次移移的位置
        /// </summary>
        public int BefIndex { get; set; }

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
            RptMtxHeader header = SubTotal.Level.Parent as RptMtxHeader;
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
            RptMtxHeader header = SubTotal.Level.Parent as RptMtxHeader;
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
