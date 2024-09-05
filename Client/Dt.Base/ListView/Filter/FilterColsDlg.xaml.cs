#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.ListView
{
    public partial class FilterColsDlg : Dlg
    {
        public FilterColsDlg()
        {
            InitializeComponent();
        }

        public void ShowDlg(Table p_tbl)
        {
            _lv.Data = p_tbl;
            if (!Kit.IsPhoneUI)
            {
                Height = Kit.ViewHeight / 2;
            }
            Show();
        }

        void OnItemClick(ItemClickArgs e)
        {
            var r = e.Row;
            r["ischecked"] = !r.Bool("ischecked");
        }

        void OnSelectAll()
        {
            foreach (var r in _lv.Table)
            {
                r["ischecked"] = true;
            }
        }

        void OnClearAll()
        {
            foreach (var r in _lv.Table)
            {
                r["ischecked"] = false;
            }
        }
    }
}