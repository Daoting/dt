#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Core;
using System;
using Windows.Foundation;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptDesignWin : Win
    {
        #region 成员变量
        readonly ExcelClerk _excelClerk;
        readonly SelectionClerk _selectionClerk;
        RptItem _curItem;
        TextForm _fmText;
        TableForm _fmTbl;
        ChartForm _fmChart;
        MatrixForm _fmMatrix;
        MatrixLevelForm _fmMtxLevel;
        MatrixSubtitleForm _fmMtxSubtitle;
        MatrixSubtotalForm _fmMtxSubtotal;
        #endregion

        public RptDesignWin(RptDesignInfo p_info)
        {
            InitializeComponent();
            Info = p_info;
            Info.TemplateChanged += (s, e) => LoadTemplate(e.NewRoot, e.OldRoot);
            Info.PageSettingChanged += (s, e) => RefreshSpliter();

            _excelClerk = new ExcelClerk(this);
            _selectionClerk = new SelectionClerk(this);

            _menu["撤消"].Cmd = new UndoCmd(Info);
            _menu["重做"].Cmd = new RedoCmd(Info);
            _menu["网格"].IsChecked = true;
            LoadTemplate(Info.Root, null);
        }

        internal RptDesignInfo Info { get; }

        internal Excel Excel
        {
            get { return _excel; }
        }

        /// <summary>
        /// 重新刷新页面设计部分的分割线高度。
        /// PaperSize 的一个值为0的时候，不会在页面上画线
        /// </summary>
        public void RefreshSpliter()
        {
            RptPageSetting setting = Info.Root.PageSetting;
            double headHeight = Info.Root.Header.ActualHeight;
            double footHeight = Info.Root.Footer.ActualHeight;
            double newHeight = setting.ValidHeight - headHeight - footHeight;
            double newWidth = setting.ValidWidth;
            if (newHeight > 0 && newWidth > 0)
            {
                _excel.PaperSize = new Size(newWidth, newHeight);
            }
        }

        /// <summary>
        /// 更新选择区域
        /// </summary>
        public void UpdateSelection()
        {
            _selectionClerk.UpdateSelection();
        }

        /// <summary>
        /// 获取当前页面，页眉、页脚或模板
        /// </summary>
        /// <returns></returns>
        internal RptPart GetContainer()
        {
            RptPart container;
            int index = _excel.ActiveSheetIndex;
            if (index == 1)
                container = Info.Root.Header;
            else if (index == 2)
                container = Info.Root.Footer;
            else
                container = Info.Root.Body;
            return container;
        }

        #region 属性表单
        internal void LoadForms(RptItem p_item, CellRange p_range)
        {
            if (p_item == null || p_range == null)
                return;

            _curItem = p_item;
            if (p_item is RptText txt)
            {
                LoadCellForm(txt);
                _tabItem.Content = null;
                _tabCell.IsSelected = true;
                return;
            }

            if (p_item is RptTable tbl)
            {
                txt = tbl.GetText(p_range.Row, p_range.Column);
                LoadCellForm(txt);

                if (_fmTbl == null)
                    _fmTbl = new TableForm(Info);
                TblRangeType tblRng = tbl.GetRangeType(p_range.Row, p_range.Column);
                _fmTbl.LoadItem(txt, tblRng == TblRangeType.Group);
                _tabItem.Content = _fmTbl;
                return;
            }

            if (p_item is RptMatrix mtx)
            {
                txt = mtx.GetText(p_range.Row, p_range.Column);
                LoadCellForm(txt);

                MtxRangeType mtxRng = mtx.GetRangeType(p_range.Row, p_range.Column);
                switch (mtxRng)
                {
                    case MtxRangeType.Level:
                        if (_fmMtxLevel == null)
                            _fmMtxLevel = new MatrixLevelForm(Info);
                        _fmMtxLevel.LoadItem(txt);
                        _tabItem.Content = _fmMtxLevel;
                        break;
                    case MtxRangeType.Subtotal:
                        if (_fmMtxSubtotal == null)
                            _fmMtxSubtotal = new MatrixSubtotalForm(Info);
                        _fmMtxSubtotal.LoadItem(txt);
                        _tabItem.Content = _fmMtxSubtotal;
                        break;
                    case MtxRangeType.Subtitle:
                        if (_fmMtxSubtitle == null)
                            _fmMtxSubtitle = new MatrixSubtitleForm(Info);
                        _fmMtxSubtitle.LoadItem(txt);
                        _tabItem.Content = _fmMtxSubtitle;
                        break;
                    default:
                        if (_fmMatrix == null)
                            _fmMatrix = new MatrixForm { Info = Info };
                        _fmMatrix.LoadItem(mtx);
                        _tabItem.Content = _fmMatrix;
                        break;
                }
                return;
            }

            if (p_item is RptChart chart)
            {
                if (_fmChart == null)
                    _fmChart = new ChartForm(Info);
                _fmChart.LoadItem(chart);
                _tabItem.Content = _fmChart;
                _tabCell.Content = null;
                _tabItem.IsSelected = true;
            }
        }

        internal void ClearForms()
        {
            _curItem = null;
            _tabItem.Content = null;
            _tabCell.Content = null;
        }

        /// <summary>
        /// 删除报表项后，若为正在编辑的对象，卸载属性Form
        /// </summary>
        /// <param name="p_item"></param>
        internal void AfterDelItem(RptItem p_item)
        {
            if ((_tabItem.Content == null && _tabCell.Content == null)
                || p_item == null
                || _curItem == null)
                return;

            if (p_item == _curItem)
            {
                ClearForms();
            }
            else
            {
                CellRange oldItemRange = new CellRange(_curItem.Row, _curItem.Col, _curItem.RowSpan, _curItem.ColSpan);
                CellRange delItemRange = new CellRange(p_item.Row, p_item.Col, p_item.RowSpan, p_item.ColSpan);
                if (delItemRange.Contains(oldItemRange))
                    ClearForms();
            }
        }

        void LoadCellForm(RptText p_txt)
        {
            if (p_txt == null)
            {
                _tabCell.Content = null;
            }
            else
            {
                if (_fmText == null)
                    _fmText = new TextForm();
                _fmText.LoadItem(p_txt);
                _tabCell.Content = _fmText;
            }
        }
        #endregion

        #region 初始加载
        void LoadTemplate(RptRoot p_newRoot, RptRoot p_oldRoot)
        {
            Throw.IfNull(p_newRoot, "报表模板不可为空！");
            _excel.IsBusy = true;

            try
            {
                ClearForms();
                if (p_oldRoot != null)
                {
                    p_oldRoot.Serializing -= OnBeforeSerialize;
                    _excelClerk.DetachEvent(p_oldRoot);
                    _excel.ColumnWidthChanged -= OnColumnWidthChanged;
                    _excel.RowHeightChanged -= OnRowHeightChanged;
                }

                _excelClerk.AttachEvent(p_newRoot);
                using (_excel.Defer())
                {
                    CreateSheets();
                    InitRowColSize();
                    LoadItems();
                    _excel.DecorationRange = null;
                    RefreshSpliter();
                }

                p_newRoot.Serializing += OnBeforeSerialize;
                _excel.ColumnWidthChanged += OnColumnWidthChanged;
                _excel.RowHeightChanged += OnRowHeightChanged;
            }
            finally
            {
                _excel.IsBusy = false;
            }
        }

        /// <summary>
        /// 切换模板时重新构造页面
        /// </summary>
        void CreateSheets()
        {
            _excel.Sheets.Clear();
            Worksheet ws = new Worksheet("模板");
            ws.SelectionPolicy = SelectionPolicy.Range;
            ws.LockCell = true;
            _excel.Sheets.Add(ws);

            ws = new Worksheet("页眉");
            ws.RowCount = 1;
            ws.SelectionPolicy = SelectionPolicy.Range;
            ws.LockCell = true;
            _excel.Sheets.Add(ws);

            ws = new Worksheet("页脚");
            ws.RowCount = 1;
            ws.SelectionPolicy = SelectionPolicy.Range;
            ws.LockCell = true;
            _excel.Sheets.Add(ws);
        }

        /// <summary>
        /// 初始化行高列宽
        /// </summary>
        void InitRowColSize()
        {
            // 内容区域
            Worksheet ws = _excel.Sheets[0];
            double[] size = Info.Root.Body.Rows;
            if (size != null && size.Length > 0)
            {
                for (int i = 0; i < size.Length; i++)
                {
                    ws.Rows[i].Height = size[i];
                }
            }

            // 页眉
            ws = _excel.Sheets[1];
            ws.Rows[0].Height = Info.Root.Header.Height;

            // 页脚
            ws = _excel.Sheets[2];
            ws.Rows[0].Height = Info.Root.Footer.Height;

            // 列宽
            size = Info.Root.Cols;
            if (size != null && size.Length > 0)
            {
                for (int i = 0; i < size.Length; i++)
                {
                    double width = size[i];
                    _excel.Sheets[0].Columns[i].Width = width;
                    _excel.Sheets[1].Columns[i].Width = width;
                    _excel.Sheets[2].Columns[i].Width = width;
                }
            }
        }

        /// <summary>
        /// 加载所有报表项
        /// </summary>
        void LoadItems()
        {
            foreach (RptItem item in Info.Root.Body.Items)
            {
                _excelClerk.LoadItem(item);
            }
            foreach (RptItem item in Info.Root.Header.Items)
            {
                _excelClerk.LoadItem(item);
            }
            foreach (RptItem item in Info.Root.Footer.Items)
            {
                _excelClerk.LoadItem(item);
            }
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 显示网格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCheckChanged(object sender, Mi e)
        {
            _excel.ActiveSheet.ShowGridLine = e.IsChecked;
        }

        /// <summary>
        /// 序列化开始前，整理行高列宽，空行空列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBeforeSerialize(object sender, EventArgs e)
        {
            Worksheet ws = _excel.Sheets[0];
            RptBody body = Info.Root.Body;

            // 列宽
            int maxCol = body.ColSpan;
            int temp = Info.Root.Header.ColSpan;
            if (temp > maxCol)
                maxCol = temp;
            temp = Info.Root.Footer.ColSpan;
            if (temp > maxCol)
                maxCol = temp;

            double[] size = new double[maxCol];
            for (int i = 0; i < maxCol; i++)
            {
                size[i] = ws.Columns[i].Width;
            }
            Info.Root.Cols = size;

            // 内容区域
            int rowSpan = body.RowSpan;
            size = new double[rowSpan];
            for (int i = 0; i < rowSpan; i++)
            {
                size[i] = ws.Rows[i].Height;
            }
            body.Rows = size;

            // 页眉
            ws = _excel.Sheets[1];
            Info.Root.Header.SetHeight(ws.Rows[0].Height);

            // 页脚
            ws = _excel.Sheets[2];
            Info.Root.Footer.SetHeight(ws.Rows[0].Height);
        }

        /// <summary>
        /// 列宽调整时同步三个Sheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnList == null || e.ColumnList.Length == 0)
                return;

            int colIdx = e.ColumnList[0];
            Worksheet sheet = _excel.ActiveSheet;
            double colWidth = sheet.Columns[colIdx].Width;
            for (int i = 0; i < _excel.SheetCount; i++)
            {
                sheet = _excel.Sheets[i];
                if (sheet.Columns != null && sheet.Columns[colIdx] != null)
                {
                    sheet.Columns[colIdx].Width = colWidth;
                }
            }
        }

        /// <summary>
        /// 改变页头尾高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRowHeightChanged(object sender, RowHeightChangedEventArgs e)
        {
            if (e.RowList == null || e.RowList.Length == 0)
                return;
            bool needRefresh = false;
            RptPartType partType = this.GetContainer().PartType;
            if (partType == RptPartType.Header)
            {
                Info.Root.Header.SetHeight(_excel.ActiveSheet.Rows[e.RowList[0]].Height);
                needRefresh = true;
            }
            if (partType == RptPartType.Footer)
            {
                Info.Root.Footer.SetHeight(_excel.ActiveSheet.Rows[e.RowList[0]].Height);
                needRefresh = true;
            }
            if (needRefresh == true)
                RefreshSpliter();
        }
        #endregion

    }
}
