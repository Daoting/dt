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
    public partial class HisLogFileDlg : Dlg
    {
        public HisLogFileDlg()
        {
            InitializeComponent();
            LoadFiles();
        }

        public FileInfo FileInfo => ((LocalFileItem)_lv.SelectedItem)?.Info;

        void OnOpen(Mi e)
        {
            DoOpen();
        }

        void OnDblClick(object e)
        {
            DoOpen();
        }

        void LoadFiles()
        {
            Nl<LocalFileItem> list = new Nl<LocalFileItem>();
            var di = new DirectoryInfo(Path.Combine(ApplicationData.Current.LocalFolder.Path, ".log"));
            if (di.Exists )
            {
                foreach (FileInfo fi in di.GetFiles().OrderBy((f) => f.Name))
                {
                    list.Add(new LocalFileItem(fi));
                }
            }
            _lv.Data = list;
        }

        void DoOpen()
        {
            if (_lv.SelectedItem == null)
            {
                Kit.Warn("未选择日志文件");
            }
            else
            {
                Close(true);
            }
        }
    }
}