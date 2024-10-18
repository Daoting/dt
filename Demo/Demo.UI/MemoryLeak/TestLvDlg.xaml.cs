#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-10-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class TestLvDlg : Dlg
    {
        public TestLvDlg()
        {
            InitializeComponent();
            
            _lv.GroupName = "bumen";
            _lv.Data = SampleData.CreatePersonsTbl(100);
            if (!Kit.IsPhoneUI)
            {
                Width = Kit.ViewWidth - 200;
                Height = Kit.ViewHeight - 100;
            }
        }

        void OnLoadData(Mi e)
        {
            _lv.Data = SampleData.CreatePersonsTbl(int.Parse(e.Tag.ToString()));
        }

        void OnLoadNull(Mi e)
        {
            _lv.Data = null;
        }
    }
}