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
    internal static class RptCmds
    {
        public static readonly InsertTextCmd InsertText = new InsertTextCmd();

        public static readonly InsertTableCmd InsertTable = new InsertTableCmd();

        public static readonly InsertMatrixCmd InsertMatrix = new InsertMatrixCmd();

        public static readonly InsertChartCmd InsertChart = new InsertChartCmd();

        public static readonly ValueChangedCmd ValueChanged = new ValueChangedCmd();

        public static readonly InsertTblRowCmd InsertTblRow = new InsertTblRowCmd();

        public static readonly DeleTblRowCmd DeleTblRow = new DeleTblRowCmd();

        public static readonly InsertTblGrpCmd InsertTblGrp = new InsertTblGrpCmd();

        public static readonly HideMatrixHeaderCmd HideMtxHeader = new HideMatrixHeaderCmd();

        public static readonly AddSubLevelCmd AddSubLevel = new AddSubLevelCmd();

        public static readonly DelSubLevelCmd DelSubLevel = new DelSubLevelCmd();

        public static readonly AddSubTotalCmd AddSubTotal = new AddSubTotalCmd();

        public static readonly DelSubTotalCmd DelSubTotal = new DelSubTotalCmd();

        public static readonly ChangeSubTotalLocCmd ChangeTotalLocCmd = new ChangeSubTotalLocCmd();

        public static readonly SubTotalSpanCmd ChangeTotalSpanCmd = new SubTotalSpanCmd();

        public static readonly AddSubTitleCmd AddSubTitle = new AddSubTitleCmd();

        public static readonly DelSubTitleCmd DelSubTitle = new DelSubTitleCmd();

        public static readonly SubTitleSpanCmd ChangeTitleSpanCmd = new SubTitleSpanCmd();

        public static readonly DelRptItemCmd DelRptItemCmd = new DelRptItemCmd();

        public static readonly MoveRptItemCmd MoveRptItemCmd = new MoveRptItemCmd();

        public static readonly ClearTblGrpCmd ClearTblGrp = new ClearTblGrpCmd();

        public static readonly InsertTblColCmd InsertTblCol = new InsertTblColCmd();

        public static readonly DeleTblColCmd DeleTblCol = new DeleTblColCmd();

        public static readonly ContainHeadOrFootCmd ConHeadOrFoot = new ContainHeadOrFootCmd();

        public static readonly RemoveHeadOrFootCmd RemHeadOrFoot = new RemoveHeadOrFootCmd();

        public static readonly CopyItemCmd CopyItem = new CopyItemCmd();
    }
}
