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
using Dt.Cells.UI;
using Dt.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptView : UserControl
    {
        RptInfo _info;

        public RptView()
        {
            InitializeComponent();

            _excel.Sheets.Clear();
            _excel.CellClick += OnCellClick;
            _excel.SelectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 加载报表内容，前提条件:
        /// <para>报表模板名称</para>
        /// <para>确保查询参数完备</para>
        /// </summary>
        /// <param name="p_info">报表描述信息</param>
        public async void LoadReport(RptInfo p_info)
        {
            // 确保正确加载模板，参数完备
            if (p_info == null
                || !await p_info.InitTemplate()
                || !p_info.IsParamsValid())
                return;

            _excel.IsBusy = true;
            try
            {
                _info = p_info;

                // 绘制报表内容
                if (_info.Sheet == null)
                {
                    RptRootInst inst = new RptRootInst(_info);
                    await inst.Draw();
                }

                using (_excel.Defer())
                {
                    _excel.Sheets.Clear();

                    // 设置分页线
                    _excel.PaperSize = new Size(_info.Root.Setting.ValidWidth, _info.Root.Setting.ValidHeight);
                    
                    // 添加报表页
                    var ws = _info.Sheet;
                    if (ws != null)
                    {
                        // 应用外部可控制属性
                        ws.ColumnHeader.IsVisible = _info.Root.ViewSetting.ShowColHeader;
                        ws.RowHeader.IsVisible = _info.Root.ViewSetting.ShowRowHeader;
                        ws.ShowGridLine = _info.Root.ViewSetting.ShowGridLine;
                        _excel.Sheets.Add(ws);
                    }
                }
            }
            catch
            {
                AtKit.Warn("报表绘制异常！");
            }
            finally
            {
                _excel.IsBusy = false;
            }
        }

        /// <summary>
        /// 点击单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnCellClick(object sender, CellClickEventArgs e)
        {
            //RptTextInst inst = _excel.ActiveSheet[e.Row, e.Column].Tag as RptTextInst;
            //if (inst == null || inst.Item == null)
            //{
            //    return;
            //}

            //RptText txt = (RptText)inst.Item;
            //if (txt.ClickAction == TextClickAction.None
            //    || (txt.ClickAction == TextClickAction.OpenReport && string.IsNullOrEmpty(txt.RptID))
            //    || (txt.ClickAction == TextClickAction.RunScript && string.IsNullOrEmpty(txt.ScriptID)))
            //{
            //    return;
            //}

            //if (txt.ClickAction == TextClickAction.OpenReport)
            //{
            //    RptInfo info = new RptInfo();
            //    info.Name = txt.RptID;
            //    await AtRpt.LoadTemplate(info);

            //    // 构造查询参数
            //    Dict dt = new Dict();
            //    DataRow row = inst.Data;
            //    foreach (DataRow data in info.Root.Params.Data)
            //    {
            //        string id = data.Str("id");
            //        dt[id] = (row != null && row.Contains(id)) ? row[id] : data["val"];
            //    }
            //    info.Params = dt;
            //    LinkReport(info);
            //}
            //else
            //{
            //    inst.Row = e.Row;
            //    inst.Col = e.Column;
            //    _info.OnCellClick(txt.ScriptID, inst);
            //}
        }

        /// <summary>
        /// 选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectionChanged(object sender, EventArgs e)
        {
            //object selectedItem = null;
            //if (_previewMenu != null && _previewMenu.IsOpened)
            //    _previewMenu.Close();

            //Worksheet sheet = _excel.ActiveSheet;
            //if (sheet.Selections.Count == 0)
            //    return;

            //CellRange range = sheet.Selections[0];
            //SheetTable[] st = sheet.GetTables();
            //foreach (SheetTable tbl in st)
            //{
            //    if (tbl.Range.Intersects(range.Row, range.Column, range.RowCount, range.ColumnCount))
            //    {
            //        selectedItem = tbl;
            //        break;
            //    }
            //}
            //if (selectedItem == null && range.RowCount == 1 && range.ColumnCount == 1)
            //    return;

            //if (_previewMenu == null)
            //    _previewMenu = new PreviewMenu(this);
            //_previewMenu.SelectedItem = selectedItem;
            //_previewMenu.Show(this, range);
        }
    }
}
