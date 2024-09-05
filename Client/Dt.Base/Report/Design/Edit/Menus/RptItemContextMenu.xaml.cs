#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-31 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.Foundation;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptItemContextMenu : Menu
    {
        RptDesignHome _owner;

        public RptItemContextMenu(RptDesignHome p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
            Opening += OnOpening;
        }

        public List<RptItem> TargetItems { get; set; }

        void OnDelete()
        {
            foreach (var item in TargetItems)
            {
                _owner.Info.ExecuteCmd(RptCmds.DelRptItemCmd, new DelRptItemArgs(item));
            }
        }

        void OnBorder()
        {
            foreach (var rt in GetAllRptText())
            {
                rt?.SetBorder(false);
            }
        }

        void OnDelBorder()
        {
            foreach (var rt in GetAllRptText())
            {
                rt?.SetBorder(true);
            }
        }

        void OnMergeCell()
        {
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var tbl = TargetItems[0] as RptTable;
            var txt = tbl.GetText(range.Row, range.Column);
            if (txt.ColSpan >= range.ColumnCount)
            {
                _owner.Info.ExecuteCmd(RptCmds.SplitTblGrpCell, new SplitTblGrpCellCmdArgs(txt, range));
            }
            else
            {
                _owner.Info.ExecuteCmd(RptCmds.MergeTblGrpCell, new SplitTblGrpCellCmdArgs(txt, range));
            }
        }

        void OnSelectImg()
        {
            ((RptImage)TargetItems[0]).SelectImage();
        }

        void OnClearImg()
        {
            ((RptImage)TargetItems[0]).ClearImage();
        }

        void OnCopy()
        {
            BlankAreaMenu.CopyItem(TargetItems[0]);
        }
        
        IEnumerable<RptText> GetAllRptText()
        {
            CellRange range = null;
            Worksheet sheet = _owner.Excel.ActiveSheet;
            if (sheet.Selections.Count == 0
                || (range = sheet.Selections[0]).Row == -1
                || range.Column == -1)
            {
                yield return null;
            }

            foreach (var item in TargetItems)
            {
                if (item is RptText rt)
                {
                    if (range.Contains(rt.Row, rt.Col, rt.RowSpan, rt.ColSpan))
                        yield return rt;
                }
                else if (item is RptTable tbl)
                {
                    if (tbl.ColHeader != null)
                    {
                        foreach (var pi in GetAllRptText(tbl.ColHeader, range))
                        {
                            yield return pi;
                        }
                    }
                    if (tbl.Body != null)
                    {
                        foreach (var pi in GetAllRptText(tbl.Body, range))
                        {
                            yield return pi;
                        }
                    }
                    if (tbl.Groups != null && tbl.Groups.Count > 0)
                    {
                        foreach (var grp in tbl.Groups)
                        {
                            foreach (var pi in GetAllRptText(grp, range))
                            {
                                yield return pi;
                            }
                        }
                    }
                    if (tbl.ColFooter != null)
                    {
                        foreach (var pi in GetAllRptText(tbl.ColFooter, range))
                        {
                            yield return pi;
                        }
                    }
                }
                else if (item is RptMatrix mtx)
                {
                    if (mtx.Corner != null)
                    {
                        rt = mtx.Corner.Item;
                        if (range.Contains(rt.Row, rt.Col, rt.RowSpan, rt.ColSpan))
                            yield return rt;
                    }

                    if (mtx.ColHeader != null
                        && mtx.ColHeader.Levels != null
                        && mtx.ColHeader.Levels.Count > 0)
                    {
                        foreach (var pi in mtx.ColHeader.Levels)
                        {
                            if (range.Contains(pi.Item.Row, pi.Item.Col, pi.Item.RowSpan, pi.Item.ColSpan))
                                yield return pi.Item;
                        }
                    }

                    if (mtx.RowHeader != null
                        && mtx.RowHeader.Levels != null
                        && mtx.RowHeader.Levels.Count > 0)
                    {
                        foreach (var pi in mtx.RowHeader.Levels)
                        {
                            if (range.Contains(pi.Item.Row, pi.Item.Col, pi.Item.RowSpan, pi.Item.ColSpan))
                                yield return pi.Item;
                        }
                    }

                    if (mtx.Rows != null && mtx.Rows.Count > 0)
                    {
                        foreach (var row in mtx.Rows)
                        {
                            foreach (var col in row.Cells)
                            {
                                if (range.Contains(col.Row, col.Col, col.RowSpan, col.ColSpan))
                                    yield return col;
                            }
                        }
                    }
                }
            }
        }

        IEnumerable<RptText> GetAllRptText(RptTblPart p_part, CellRange p_range)
        {
            foreach (RptTblPartRow row in p_part.Rows)
            {
                List<RptText> cells = row.Cells;
                if (cells != null && cells.Count > 0)
                {
                    foreach (RptText rt in cells)
                    {
                        if (p_range.Contains(rt.Row, rt.Col, rt.RowSpan, rt.ColSpan))
                            yield return rt;
                    }
                }
            }
        }

        void OnOpening(Menu menu, AsyncCancelArgs args)
        {
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            if (TargetItems.Count == 1)
            {
                if (TargetItems[0] is RptTable tbl
                   && tbl.GetRangeType(range.Row, range.Column) == TblRangeType.Group
                   && range.RowCount == 1
                   && range.ColumnCount > 1)
                {
                    var txt = tbl.GetText(range.Row, range.Column);
                    if (txt.ColSpan >= range.ColumnCount)
                    {
                        _miMerge.ID = "拆分单元格";
                    }
                    else
                    {
                        _miMerge.ID = "合并单元格";
                    }
                    _miMerge.Visibility = Visibility.Visible;
                }
                else
                {
                    _miMerge.Visibility = Visibility.Collapsed;
                }

                if (TargetItems[0] is RptImage rt)
                {
                    this["选择图片"].Visibility = Visibility.Visible;
                    if (rt.ImgData != null && rt.ImgData.Length > 0)
                    {
                        this["清除图片"].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this["清除图片"].Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    this["选择图片"].Visibility = Visibility.Collapsed;
                    this["清除图片"].Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _miMerge.Visibility = Visibility.Collapsed;
                this["选择图片"].Visibility = Visibility.Collapsed;
                this["清除图片"].Visibility = Visibility.Collapsed;
            }
        }

        async void OnMove()
        {
            CellRange range = null;
            Worksheet sheet = _owner.Excel.ActiveSheet;
            if (sheet.Selections.Count == 0
                || (range = sheet.Selections[0]).Row == -1
                || range.Column == -1)
            {
                return;
            }
            
            var dlg = new MoveItemsDlg();
            if (await dlg.ShowAsync()
                && (dlg.DeltaX != 0 || dlg.DeltaY != 0))
            {
                _owner.Info.ExecuteCmd(RptCmds.MoveItems, new MoveItemsCmdArgs(TargetItems, dlg.DeltaX, dlg.DeltaY, range, _owner));
            }
        }
    }
}
