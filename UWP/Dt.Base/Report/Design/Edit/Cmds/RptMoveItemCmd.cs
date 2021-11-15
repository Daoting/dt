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
using Dt.Cells.UI;

#endregion

namespace Dt.Base.Report
{
    internal class MoveRptItemCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            MoveRptItemArgs args = (MoveRptItemArgs)p_args;
            RptItem drgItem = args.RptItem;
            CellEventArgs dstPos = args.DstPos;

            args.OldRow = drgItem.Row;
            args.OldCol = drgItem.Col;            
            drgItem.Row = dstPos.Row;
            drgItem.Col = dstPos.Column;
            drgItem.Update(true);
            return drgItem;
        }

        public override void Undo(object p_args)
        {
            MoveRptItemArgs args = (MoveRptItemArgs)p_args;
            RptItem drgItem = args.RptItem;
            drgItem.Row = args.OldRow;
            drgItem.Col = args.OldCol;
            drgItem.Update(true);
        }
    }

    internal class MoveRptItemArgs
    {
        public MoveRptItemArgs(RptItem p_rptItem, CellEventArgs p_dstPos)
        {
            RptItem = p_rptItem;
            DstPos = p_dstPos;
        }

        /// <summary>
        /// 获取要放置的区域
        /// </summary>
        public RptItem RptItem { get; }

        public int OldRow { get; set; }

        public int OldCol { get; set; }

        public CellEventArgs DstPos { get; }
    }

} 
