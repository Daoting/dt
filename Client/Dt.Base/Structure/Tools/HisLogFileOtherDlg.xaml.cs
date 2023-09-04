#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-08-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
#endregion

namespace Dt.Base.Tools
{
    public partial class HisLogFileOtherDlg : Dlg
    {
        public HisLogFileOtherDlg()
        {
            InitializeComponent();
            LoadApps();
        }

        public FileInfo FileInfo => ((LocalFileItem)_lvFile.SelectedItem)?.Info;

        void LoadApps()
        {
            _lvApp.Data = PackageKit.GetLogPaths();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            var info = e.Data.To<PackageKit.LogPathInfo>();
            _tb.Text = info.Path;
            Nl<LocalFileItem> list = new Nl<LocalFileItem>();
            var di = new DirectoryInfo(info.Path);
            if (di.Exists)
            {
                foreach (FileInfo fi in di.GetFiles().OrderBy((f) => f.Name))
                {
                    list.Add(new LocalFileItem(fi));
                }
            }
            _lvFile.Data = list;
        }

        void OnOpen(object sender, Mi e)
        {
            DoOpen();
        }

        void OnDblClick(object sender, object e)
        {
            DoOpen();
        }

        void DoOpen()
        {
            if (_lvFile.SelectedItem == null)
            {
                Kit.Warn("未选择日志文件");
            }
            else
            {
                Close(true);
            }
        }

        void CopyPath(object sender, RoutedEventArgs e)
        {
            if (_lvApp.SelectedItem is PackageKit.LogPathInfo info)
            {
                SysTrace.CopyToClipboard(info.Path);
            }
        }
    }
}