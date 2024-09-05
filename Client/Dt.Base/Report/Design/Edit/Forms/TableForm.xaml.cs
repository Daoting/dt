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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

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
            var cell = _table.Data.Cells["tbl"];
            cell.Changed -= OnTblNameChanged;
            cell.Changed += OnTblNameChanged;

            UpdateHeaderFooterState();
            if (p_isGroup)
            {
                _fvGrp.Show("field");
                _pnlGrpRow.Visibility = Visibility.Visible;
                foreach (RptTblGroup grp in _table.Groups)
                {
                    if (grp == _part)
                    {
                        _fvGrp.Data = grp.Data;
                        break;
                    }
                }
                ((CList)_fvGrp["field"]).Data = _info.Root.Data.GetColsData(_table.Data.Str("tbl"));
            }
            else
            {
                _fvGrp.Hide("field");
                _pnlGrpRow.Visibility = Visibility.Collapsed;
            }
        }

        void OnTblNameChanged(Cell p_cell)
        {
            ((CList)_fvGrp["field"]).Data = _info.Root.Data.GetColsData(p_cell.Str);
        }

        #region 列头列尾
        void OnToggleHeader(object sender, RoutedEventArgs e)
        {
            ToggleHeaderFooter("Header", _table.ColHeader != null && _table.ColHeader.Rows.Count > 0);
        }

        void OnToggleFooter(object sender, RoutedEventArgs e)
        {
            ToggleHeaderFooter("Footer", _table.ColFooter != null && _table.ColFooter.Rows.Count > 0);
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
                    part = _table.ColHeader;
                else
                    part = _table.ColFooter;
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
            _btnHeader.Content = (_table.ColHeader != null && _table.ColHeader.Rows.Count > 0) ? "删除列头" : "增加列头";
            _btnFooter.Content = (_table.ColFooter != null && _table.ColFooter.Rows.Count > 0) ? "删除列尾" : "增加列尾";
        }
        #endregion

        #region 分组
        void OnInsertGrpClick(object sender, RoutedEventArgs e)
        {
            // 先测试扩展位置是否与其他控件冲突
            if (_table.TestIncIntersect(1))
            {
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            RptTblGroup grp = new RptTblGroup(_table);
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

        void OnInsertGrpRow(object sender, RoutedEventArgs e)
        {
            if (_part is not RptTblGroup grp)
            {
                Kit.Warn("当前未选择分组！");
                return;
            }

            // 先测试扩展位置是否与其他控件冲突
            if (_table.TestIncIntersect(1))
            {
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            _info.ExecuteCmd(RptCmds.InsertTblGrpRow, new InsertTblGrpRowCmdArgs(_table, grp));
        }

        void OnDelGrpRow(object sender, RoutedEventArgs e)
        {
            if (_part is not RptTblGroup grp || _curRow == null)
            {
                Kit.Warn("当前未选择分组！");
            }
            else if (grp.Rows.Count == 1)
            {
                _info.ExecuteCmd(RptCmds.DelTblGrp, new InsertTblGrpCmdArgs(_table, grp));
            }
            else
            {
                _info.ExecuteCmd(RptCmds.DelTblGrpRow, new InsertTblGrpRowCmdArgs(_table, grp) { Row = _curRow });
            }
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
            index = (((Button)sender).Tag ?? "").ToString() == "Left" ? index : index + _txt.ColSpan;
            _info.ExecuteCmd(RptCmds.InsertTblCol, new InsertRptTblColCmdArgs(_table, index));
        }

        void OnDeleteCol(object sender, RoutedEventArgs e)
        {
            int index = GetTextIndex();
            _info.ExecuteCmd(RptCmds.DeleTblCol, new DeleRptTblColCmdArgs(_table, index));
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
                foreach (var cell in _curRow.Cells)
                {
                    if (cell != _txt)
                        index += cell.ColSpan;
                    else
                        return index;
                }
            }
            return index;
        }
        #endregion
    }
}
