#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-08 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    public partial class SelectRptList : Dlg
    {
        public SelectRptList()
        {
            InitializeComponent();
            LoadData();
        }

        public static async Task<string> ShowDlg()
        {
            var dlg = new SelectRptList();
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 600;
                dlg.Height = 400;
            }
            if (await dlg.ShowAsync())
                return dlg.GetSelectRpt();
            return null;
        }

        string GetSelectRpt()
        {
            if (_lv.SelectedItem is Row row)
            {
                return "rpt://" + row.Str("name");
            }
            return null;
        }

        async void LoadData()
        {
            _lv.Data = await RptX.Query("select name,note,mtime from cm_rpt order by name");
            _lv.FilterCfg = new FilterCfg
            {
                EnablePinYin = true,
                IsRealtime = true,
                FilterCols = "name",
                Placeholder = "请输入报表名称的中文或简拼进行筛选",
            };
        }

        void OnItemDbClick(object obj)
        {
            Close(true);
        }

        void OnOk()
        {
            Close(true);
        }
    }
}