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
    /// 添加子层次命令
    /// </summary>
    internal class AddSubLevelCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubLevelCmdArgs args = (SubLevelCmdArgs)p_args;
            RptMtxHeader header = args.Header;
            RptMtxLevel level = args.Level;
            RptMatrix mtx = null;
            if (level == null)
            {
                level = new RptMtxLevel(header);
                level.Item.Val =  string.Format("{0}_{1}", header.Levels[0].Item.Val, header.Levels.Count);
                level.Field = "";
                args.InitData();
                mtx = header.Parent as RptMatrix;
                RptMtxLevel parentLevel = header.Levels[header.Levels.Count - 1];
                if (parentLevel.SubTitles.Count > 0)
                {
                    int curIndex = 0;
                    if (header is RptMtxRowHeader)
                    {
                        curIndex = parentLevel.Row - mtx.ColHeader.RowSpan;
                        args.OpsRows.Add(InsertNewRow(mtx, curIndex));
                    }
                    else
                    {
                        curIndex = parentLevel.Col - mtx.RowHeader.ColSpan;
                        foreach (RptMtxRow row in mtx.Rows)
                        {
                            args.OpsCells.Add(row, InsertNewCell(mtx, row, curIndex).ToList());
                        }
                    }
                    args.CurIndex = curIndex;
                }
            }
            else
            {
                args.BackMtxRow();
            }
            header.Levels.Add(level);
            args.Level = level;
            mtx.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            SubLevelCmdArgs args = (SubLevelCmdArgs)p_args;
            RptMtxHeader header = args.Header;
            RptMtxLevel level = args.Level;
            RptMatrix mtx = header.Parent as RptMatrix;
            RptMtxLevel parentLevel = header.Levels[header.Levels.Count - 2];
            if (parentLevel.SubTitles.Count > 0)
            {
                args.DelMtxRows();
            }
            header.Levels.Remove(level);
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
    internal class DelSubLevelCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            SubLevelCmdArgs args = (SubLevelCmdArgs)p_args;
            RptMtxHeader header = args.Header;
            RptMtxLevel level = args.Level;
            args.InitData();
            DelRows(args, header, level);
            for (int i = header.Levels.IndexOf(level) + 1; i < header.Levels.Count; i++)
            {
                args.SubLevels.Add(header.Levels[i]);
                header.Levels.Remove(header.Levels[i]);
            }
            header.Levels.Remove(level);
            (header.Parent as RptMatrix).Update(true);
            return level;
        }

        public override void Undo(object p_args)
        {
            SubLevelCmdArgs args = (SubLevelCmdArgs)p_args;
            RptMtxHeader header = args.Header;
            RptMtxLevel level = args.Level;
            args.BackMtxRow();
            header.Levels.Add(level);
            foreach (RptMtxLevel subLevel in args.SubLevels)
            {
                header.Levels.Add(subLevel);
            }
            (header.Parent as RptMatrix).Update(true);
        }

        /// <summary>
        /// 删除对应数据行
        /// </summary>
        /// <param name="p_args"></param>
        /// <param name="p_header"></param>
        /// <param name="p_level"></param>
        void DelRows(SubLevelCmdArgs p_args, RptMtxHeader p_header, RptMtxLevel p_level)
        {
            RptMatrix mat = p_header.Parent as RptMatrix;
            RptMtxLevel parentLevel = p_header.Levels[p_header.Levels.IndexOf(p_level) - 1];
            if (p_header is RptMtxRowHeader)
            {
                int curIndex = parentLevel.Row - (mat.Row + mat.ColHeader.RowSpan);
                for (int i = 0; i < parentLevel.RowSpan - 1; i++)
                {
                    p_args.OpsRows.Add(mat.Rows[curIndex + i]);
                }
                p_args.CurIndex = curIndex;
                mat.Rows.RemoveRange(curIndex, parentLevel.RowSpan - 1);
            }
            else
            {
                int curIndex = parentLevel.Col - (mat.Col + mat.RowHeader.ColSpan);
                List<RptText> cells;
                foreach (RptMtxRow row in mat.Rows)
                {
                    cells = new List<RptText>();
                    for (int i = 0; i < parentLevel.ColSpan - 1; i++)
                    {
                        cells.Add(row.Cells[curIndex + i]);
                    }
                    p_args.OpsCells.Add(row, cells);
                    row.Cells.RemoveRange(curIndex, parentLevel.ColSpan - 1);
                }
                p_args.CurIndex = curIndex;
            }
        }
    }

    /// <summary>
    /// 层次 操作参数
    /// </summary>
    internal class SubLevelCmdArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_mtxHeader">所属头</param>
        /// <param name="p_level">当前操作层</param>
        public SubLevelCmdArgs(RptMtxHeader p_mtxHeader, RptMtxLevel p_level = null)
        {
            Header = p_mtxHeader;
            Level = p_level;
            InitData();
        }

        /// <summary>
        /// 获取对应头信息
        /// </summary>
        public RptMtxHeader Header { get; }

        /// <summary>
        /// 获取设置当前操作层
        /// </summary>
        public RptMtxLevel Level { get; set; }

        /// <summary>
        /// 获取设置包含的子层次
        /// </summary>
        public List<RptMtxLevel> SubLevels { get; set; }

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
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            OpsRows = new List<RptMtxRow>();
            OpsCells = new Dictionary<RptMtxRow, List<RptText>>();
            SubLevels = new List<RptMtxLevel>();
        }

        /// <summary>
        /// 删除对应数据行
        /// </summary>
        public void DelMtxRows()
        {
            RptMatrix mat = Header.Parent as RptMatrix;
            if (Header is RptMtxRowHeader)
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
            RptMatrix mat = Header.Parent as RptMatrix;
            if (Header is RptMtxRowHeader)
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
