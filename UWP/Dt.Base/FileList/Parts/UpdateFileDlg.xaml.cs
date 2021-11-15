#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.Base.FileLists
{
    public sealed partial class UpdateFileDlg : Dlg
    {
        int _result = -1;

        public UpdateFileDlg()
        {
            InitializeComponent();
        }

        public async Task<int> ShowDlg()
        {
            await ShowAsync();
            return _result;
        }

        void OnSelect(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _result = 0;
            Close();
        }

        void OnUpload(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _result = 1;
            Close();
        }
    }
}
