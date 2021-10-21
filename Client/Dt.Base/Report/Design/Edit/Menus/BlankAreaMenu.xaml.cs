#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-31 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#endregion

namespace Dt.Base.Report
{
    public sealed partial class BlankAreaMenu : Menu
    {
        RptDesignWin _owner;

        public BlankAreaMenu(RptDesignWin p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        void OnInsertText(object sender, Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptText(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertText, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertTbl(object sender, Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptTable(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertTable, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertMtx(object sender, Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptMatrix(_owner.GetContainer());
            range = new CellRange(range.Row, range.Column, 2, 2);
            if (RptItem.ValidEmptyRange(item.Part, range))
            {
                Kit.Error("对象创建后与其他对象位置冲突，请确认。");
                return;
            }
            _owner.Excel.ActiveSheet.SetSelection(range);
            _owner.Info.ExecuteCmd(RptCmds.InsertMatrix, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertChart(object sender, Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptChart(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertChart, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertCopy(object sender, Mi e)
        {

        }
    }
}
