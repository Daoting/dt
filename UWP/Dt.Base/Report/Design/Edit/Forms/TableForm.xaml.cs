#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class TableForm : UserControl
    {
        RptDesignInfo _info;
        RptText _txt;
        RptTblPartRow _curRow;
        RptTblPart _part;
        RptTable _table;

        public TableForm(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            ((CList)_fv["tbl"]).Data = _info.Root.Data.DataSet;
        }

        internal void LoadItem(RptText p_item, bool p_isGroup)
        {
            _txt = p_item;
            _curRow = p_item.Parent as RptTblPartRow;
            _part = _curRow.Parent as RptTblPart;
            _table = _part.Table;
            _fv.Data = _table.Data;

            UpdateHeaderFooterState();
            if (p_isGroup)
            {
                _fvGrp.Show("field");
                foreach (RptTblGroup grp in _table.Groups)
                {
                    if (grp.Header == _part || grp.Footer == _part)
                    {
                        _fvGrp.Data = grp.Data;
                        break;
                    }
                }
            }
            else
            {
                _fvGrp.Hide("field");
            }
        }

        #region 表头表尾
        void OnToggleHeader(object sender, RoutedEventArgs e)
        {
            ToggleHeaderFooter("Header", _table.Header != null && _table.Header.Rows.Count > 0);
        }

        void OnToggleFooter(object sender, RoutedEventArgs e)
        {
            ToggleHeaderFooter("Footer", _table.Footer != null && _table.Footer.Rows.Count > 0);
        }

        void ToggleHeaderFooter(string p_flag, bool p_include)
        {
            if (!p_include)
            {
                // 先测试扩展位置是否与其他控件冲突
                if (_table.TestIncIntersect(1))
                {
                    Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                    return;
                }
                _info.ExecuteCmd(RptCmds.ConHeadOrFoot, new ContainHeadOrFootCmdArgs(p_flag, _table));
            }
            else
            {
                RptTblPart part;
                if (p_flag == "Header")
                    part = _table.Header;
                else
                    part = _table.Footer;
                if (part == null || part.Rows.Count == 0)
                    return;

                RptTblPartRow[] rows = new RptTblPartRow[part.Rows.Count];
                part.Rows.CopyTo(rows, 0);
                _info.ExecuteCmd(RptCmds.RemHeadOrFoot, new RemoveHeadOrFootCmdArgs(p_flag, _table, rows));
            }
            UpdateHeaderFooterState();
        }

        void UpdateHeaderFooterState()
        {
            _btnHeader.Content = (_table.Header != null && _table.Header.Rows.Count > 0) ? "删除表头" : "增加表头";
            _btnFooter.Content = (_table.Footer != null && _table.Footer.Rows.Count > 0) ? "删除表尾" : "增加表尾";
        }
        #endregion

        #region 分组
        void OnInsertGrpClick(object sender, RoutedEventArgs e)
        {
            // 先测试扩展位置是否与其他控件冲突
            if (_table.TestIncIntersect(2))
            {
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            RptTblGroup grp = new RptTblGroup(_table);
            grp.Header = new RptTblGroupHeader(_table);
            grp.Footer = new RptTblGroupFooter(_table);
            _info.ExecuteCmd(RptCmds.InsertTblGrp, new InsertTblGrpCmdArgs(_table, grp));
        }

        void OnClearGrpClick(object sender, RoutedEventArgs e)
        {
            if (_table.Groups == null || _table.Groups.Count == 0)
                return;
            List<RptTblGroup> grps = new List<RptTblGroup>();
            foreach (RptTblGroup grp in _table.Groups)
            {
                grps.Add(grp);
            }
            _info.ExecuteCmd(RptCmds.ClearTblGrp, new ClearTblGrpCmdArgs(_table, grps));
        }
        #endregion

        #region 行列
        void OnInsertRow(object sender, RoutedEventArgs e)
        {
            // 先测试扩展位置是否与其他控件冲突
            if (_table.TestIncIntersect(1))
            {
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            int index = GetIndex();
            index = (((Button)sender).Tag ?? "").ToString() == "Before" ? index : index + 1;
            _info.ExecuteCmd(RptCmds.InsertTblRow, new InsertTblRowCmdArgs(_part, index));
        }

        void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            _info.ExecuteCmd(RptCmds.DeleTblRow, new DeleTblRowCmdArgs(GetIndex(), _curRow));
        }

        void OnInsertCol(object sender, RoutedEventArgs e)
        {
            // 先测试扩展位置是否与其他控件冲突
            if (_table.TestIncIntersect(0, 1))
            {
                Kit.Warn("增加列后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            int index = GetTextIndex();
            index = (((Button)sender).Tag ?? "").ToString() == "Left" ? index : index + 1;
            _info.ExecuteCmd(RptCmds.InsertTblCol, new InsertRptTblColCmdArgs(_table, index));
        }

        void OnDeleteCol(object sender, RoutedEventArgs e)
        {
            Dictionary<string, RptText> dict = new Dictionary<string, RptText>();
            int index = GetTextIndex();
            _info.ExecuteCmd(RptCmds.DeleTblCol, new DeleRptTblColCmdArgs(_table, index, dict));
        }

        void OnDeleteTbl(object sender, RoutedEventArgs e)
        {
            _info.ExecuteCmd(RptCmds.DelRptItemCmd, new DelRptItemArgs(_table));
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
        #endregion
    }
}
