#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-31 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;

#endregion

namespace Dt.Base.Report
{
    public sealed partial class HeaderFooterMenu : Menu
    {
        RptDesignWin _owner;

        public HeaderFooterMenu(RptDesignWin p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        void OnInsertText(object sender,Mi e)
        {
            Worksheet sheet = _owner.Excel.ActiveSheet;
            CellRange range = sheet.Selections[0];
            Dt.Cells.Data.Cell cell = sheet[range.Row, range.Column];
            //合并单元格
            if (range.RowCount > 1 || range.ColumnCount > 1)
            {
                cell.RowSpan = range.RowCount;
                cell.ColumnSpan = range.ColumnCount;
            }
            _owner.Info.ExecuteCmd(RptCmds.InsertText, new InsertCmdArgs(new RptText(_owner.GetContainer()), range));
        }
    }
}
