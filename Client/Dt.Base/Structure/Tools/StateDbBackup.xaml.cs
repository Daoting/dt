#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    public partial class StateDbBackup : PageWin
    {
        public StateDbBackup()
        {
            InitializeComponent();
        }

        async void OnBackup(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_tb.Text))
                _tb.Text = "开始备份...";
            else
                _tb.Text += "\r\n\r\n开始备份...";

            StorageFolder tgtRoot;
            var folder = await GetBakFolder();
            try
            {
                tgtRoot = await folder.CreateFolderAsync(DateTime.Now.ToString("yyyyMMdd_HHmmss"), CreationCollisionOption.FailIfExists);
            }
            catch
            {
                _tb.Text += "\r\n创建备份文件夹时失败！";
                return;
            }

            _tb.Text += "\r\n备份路径：" + tgtRoot.Path;
            if ((bool)_rbAll.IsChecked)
            {
                int cnt = 0;
                foreach (var file in await ApplicationData.Current.LocalFolder.GetFilesAsync())
                {
                    // 模型库文件不复制
                    if (file.FileType == ".db" && file.Name != "State.db")
                        continue;
                    var prop = await file.GetBasicPropertiesAsync();
                    _tb.Text += string.Format("\r\n{0}  {1}", AtKit.GetFileSizeDesc(prop.Size).PadRight(15), file.Name);
                    await file.CopyAsync(tgtRoot);
                    cnt++;
                }
                _tb.Text += string.Format("\r\n共{0}个文件，备份结束！", cnt);
            }
            else
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(AtLocal.StateDbName);
                if (file == null)
                {
                    _tb.Text += "\r\n备份失败，未找到数据文件！";
                    return;
                }
                var prop = await file.GetBasicPropertiesAsync();
                _tb.Text += string.Format("\r\n{0}    {1}", file.Name, AtKit.GetFileSizeDesc(prop.Size));
                await file.CopyAsync(tgtRoot);
                _tb.Text += "\r\n备份结束！";
            }
        }

        static async Task<StorageFolder> GetBakFolder()
        {
            return await KnownFolders.VideosLibrary.CreateFolderAsync(AtSys.Stub.Title, CreationCollisionOption.OpenIfExists);
        }
    }
}