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
using System.Collections;
using System.Collections.ObjectModel;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 插入矩阵
    /// </summary>
    internal class InsertMatrixCmd : InsertCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertCmdArgs args = (InsertCmdArgs)p_args;
            RptMatrix mat = args.RptItem as RptMatrix;
            RptPart con = mat.Part;

            // 是通过重做来执行的，直接返回对象。
            if (mat.RowHeader != null || mat.ColHeader != null)
            {
                con.Items.Add(mat);
                return mat;
            }

            CellRange range = args.CellRange;
            mat.Row = range.Row;
            mat.Col = range.Column;
            mat.RowSpan = range.RowCount;
            mat.ColSpan = range.ColumnCount;
            mat.Corner = new RptMtxCorner(mat);
            RptMtxRowHeader rowheader = new RptMtxRowHeader(mat);
            mat.RowHeader = rowheader;
            RptMtxLevel level2 = new RptMtxLevel(rowheader);
            level2.Item.Val = "level2";
            rowheader.Levels.Add(level2);

            RptMtxColHeader colheader = new RptMtxColHeader(mat);
            mat.ColHeader = colheader;
            RptMtxLevel level1 = new RptMtxLevel(colheader);
            level1.Item.Val = "level1";
            colheader.Levels.Add(level1);

            RptMtxRow row = new RptMtxRow(mat);
            row.Cells.Add(new RptText(row) { Val = "cell0" });
            mat.Rows.Add(row);
            con.Items.Add(mat);
            return mat;
        }
    }

    /// <summary>
    /// 显示隐藏行头、列头命令
    /// </summary>
    internal class HideMatrixHeaderCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            MatrixArgs args = p_args as MatrixArgs;
            RptMatrix mtx = args.Matrix;
            if (args.HeaderType == RptMtxHeaderType.Col)
            {
                //重做
                if (!args.IsFirst)
                {
                    mtx.HideColHeader = args.Val;
                }
                else
                {
                    args.Val = mtx.HideColHeader;
                }
                mtx.RowSpan += mtx.ColHeader.RowSpan * (args.Val ? -1 : 1);
            }
            else
            {
                //重做
                if (!args.IsFirst)
                {
                    mtx.HideRowHeader = args.Val;
                }
                else
                {
                    args.Val = mtx.HideRowHeader;
                }
                mtx.ColSpan += mtx.RowHeader.ColSpan * (args.Val ? -1 : 1);
            }
            if (args.IsFirst)
                args.IsFirst = false;
            mtx.Update(true);
            return mtx;
        }

        public override void Undo(object p_args)
        {
            MatrixArgs args = p_args as MatrixArgs;
            RptMatrix mtx = args.Matrix;
            if (args.HeaderType == RptMtxHeaderType.Col)
            {
                mtx.HideColHeader = !args.Val;
                mtx.RowSpan += mtx.ColHeader.RowSpan * (!args.Val ? -1 : 1);
            }
            else
            {
                mtx.HideRowHeader = !args.Val;
                mtx.ColSpan += mtx.RowHeader.ColSpan * (!args.Val ? -1 : 1);
            }
            mtx.Update(true);
        }
    }

    /// <summary>
    /// 矩阵显示隐藏行头、列头参数
    /// </summary>
    internal class MatrixArgs
    {
        public MatrixArgs(RptMatrix p_mtx, RptMtxHeaderType p_type)
        {
            Matrix = p_mtx;
            HeaderType = p_type;
            IsFirst = true;
        }

        /// <summary>
        /// 获取矩阵
        /// </summary>
        public RptMatrix Matrix { get; }

        /// <summary>
        /// 获取行头还是列头
        /// </summary>
        public RptMtxHeaderType HeaderType { get; }

        /// <summary>
        /// 获取设置当前值
        /// </summary>
        public bool Val { get; set; }

        /// <summary>
        /// 获取设置是否首次（如果是false,表未重做）
        /// </summary>
        public bool IsFirst { get; set; }
    }
}
