#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public partial class 入出类别Dlg : Dlg
    {
        public 入出类别Dlg()
        {
            InitializeComponent();
            IsPinned = true;
            ShowVeil = true;
        }

        public 物资入出类别X 入出类别 => _lv.SelectedItem as 物资入出类别X;
        
        public async Task<bool> ShowDlg()
        {
            _lv.Data = await 物资入出类别X.Query(null);
            _lv.SelectedIndex = 0;
            return await ShowAsync();
        }

        void OnItemDoubleClick(object obj)
        {
            Close(true);
        }
    }
}