#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class TableForm : UserControl
    {
        RptDesignWin _owner;
        RptText _txt;
        RptTblPartRow _curRow;
        RptTblPart _part;
        RptTable _table;
        bool _isGroup;

        public TableForm(RptDesignWin p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
        }

        internal void LoadItem(RptText p_item, bool p_isGroup)
        {
            _txt = p_item;
            _curRow = p_item.Parent as RptTblPartRow;
            _part = _curRow.Parent as RptTblPart;
            _table = _part.Table;
            _isGroup = p_isGroup;
        }

        void OnInsertRow(object sender, RoutedEventArgs e)
        {
            // 先测试扩展位置是否与其他控件冲突
            if (_table.TestIncIntersect(1))
            {
                AtKit.Error("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            int index = GetIndex();
            index = (((Button)sender).Tag ?? string.Empty).ToString() == "Before" ? index : index + 1;
            _owner.Info.ExecuteCmd(RptCmds.InsertTblRow, new InsertTblRowCmdArgs(_part, index));
        }

        void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            _owner.Info.ExecuteCmd(RptCmds.DeleTblRow, new DeleTblRowCmdArgs(GetIndex(), _curRow));
        }

        int GetIndex()
        {
            int index = 0;
            if (_part != null)
            {
                index = _part.Rows.IndexOf(_curRow);
            }
            return index;
        }

        int GetTextIndex()
        {
            int index = 0;
            if (_curRow != null)
            {
                index = _curRow.Cells.IndexOf(_txt);
            }
            return index;
        }

        void OnInsertCol(object sender, RoutedEventArgs e)
        {
            // 先测试扩展位置是否与其他控件冲突
            if (_table.TestIncIntersect(0, 1))
            {
                AtKit.Warn("增加列后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            int index = GetTextIndex();
            index = (((Button)sender).Tag ?? string.Empty).ToString() == "Left" ? index : index + 1;
            _owner.Info.ExecuteCmd(RptCmds.InsertTblCol, new InsertRptTblColCmdArgs(_table, index));
        }

        void OnDeleteCol(object sender, RoutedEventArgs e)
        {
            Dictionary<string, RptText> dict = new Dictionary<string, RptText>();
            int index = GetTextIndex();
            _owner.Info.ExecuteCmd(RptCmds.DeleTblCol, new DeleRptTblColCmdArgs(_table, index, dict));
        }

        void OnDeleteTbl(object sender, RoutedEventArgs e)
        {
            _owner.Info.ExecuteCmd(RptCmds.DelRptItemCmd, new DelRptItemArgs(_owner.Excel.ActiveSheet, _table));
        }
    }
}
