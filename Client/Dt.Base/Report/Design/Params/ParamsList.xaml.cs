#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public partial class ParamsList : Tab
    {
        ParamsDlg _dlg;
        
        public ParamsList(ParamsDlg p_dlg)
        {
            InitializeComponent();
            _dlg = p_dlg;
            _lv.Data = _dlg.Info.Root.Params.Data;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dlg.Form.Update(_lv.SelectedRow);
        }

        void OnItemDbClick(object e)
        {
            _dlg.Form.Open(_lv.SelectedRow);
        }

        void OnAdd()
        {
            var row = _dlg.Info.Root.Params.Data.AddRow(new { name = "新参数", type = "string" });
            _dlg.Form.Open(row);
        }

        async void OnDel()
        {
            if (_lv.SelectedRow != null && await Kit.Confirm("确认要删除吗？"))
            {
                _lv.Table.Remove(_lv.SelectedRow);
                _dlg.Form.Update(null);
            }
        }
    }
}