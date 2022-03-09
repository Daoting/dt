#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.Maui.Essentials;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 本地文件
    /// </summary>
    public sealed partial class LocalFileView : Win
    {
        string _dir = ".doc";

        public LocalFileView()
        {
            InitializeComponent();
        }

        void OnOpenPath(object sender, RoutedEventArgs e)
        {
            _dir = ((BtnItem)sender).Title;
            LoadFiles();
            NaviTo("文件列表");
        }

        void LoadFiles()
        {
            Nl<LocalFileItem> list = new Nl<LocalFileItem>();
            var di = new DirectoryInfo(Path.Combine(ApplicationData.Current.LocalFolder.Path, _dir));
            foreach (FileInfo fi in di.GetFiles().OrderBy((f) => f.Name))
            {
                list.Add(new LocalFileItem(fi));
            }
            _lv.Data = list;
        }

        async void OnOpen(object sender, Mi e)
        {
            var fi = e.Data.To<LocalFileItem>();

            // 默认内部打开log txt文件
            if (fi.Name.EndsWith(".log") || fi.Name.EndsWith(".txt"))
            {
                ScrollViewer sv = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
                TextBlock tb = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(10), IsTextSelectionEnabled = true };
                sv.Content = tb;
                using (var stream = new StreamReader(fi.Info.OpenRead()))
                {
                    tb.Text = stream.ReadToEnd();
                }

                Dlg dlg = new Dlg
                {
                    Title = fi.Name,
                    Content = sv,
                    IsPinned = true,
                };
                if (!Kit.IsPhoneUI)
                {
                    dlg.MinWidth = 400;
                    dlg.MaxWidth = Kit.ViewWidth - 80;
                    dlg.MinHeight = 300;
                    dlg.MaxHeight = Kit.ViewHeight - 80;
                }
                dlg.Show();
            }
            else
            {
                try
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(fi.Info.FullName)
                    });
                }
                catch
                {
                    Kit.Warn("暂未实现");
                }
            }
        }


        async void OnShare(object sender, Mi e)
        {
            try
            {
                var file = e.Data.To<LocalFileItem>();
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "分享文件",
                    File = new ShareFile(file.Info.FullName)
                });
            }
            catch
            {
                Kit.Warn("暂未实现");
            }
        }

#if WIN
        async void OnSaveAs(object sender, Mi e)
        {
            var fi = e.Data.To<LocalFileItem>();
            var picker = Kit.GetFileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            string ext = ".txt";
            int index = fi.Name.LastIndexOf('.');
            if (index > -1)
                ext = fi.Name.Substring(index);
            picker.FileTypeChoices.Add("文件", new List<string>() { ext });
            picker.SuggestedFileName = fi.Name;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(fi.Info.DirectoryName);
                var temp = await folder.TryGetItemAsync(fi.Name) as StorageFile;
                if (temp != null)
                {
                    await temp.CopyAndReplaceAsync(file);
                    Kit.Msg("文件保存成功！");
                }
            }
        }
#elif ANDROID
        void OnSaveAs(object sender, Mi e)
        {
            var fi = e.Data.To<LocalFileItem>();
            try
            {
                string ext = "";
                int index = fi.Name.LastIndexOf('.');
                if (index > -1)
                    ext = fi.Name.Substring(index);
                var tgtName = $"{Guid.NewGuid().ToString().Substring(0, 8)}{ext}";
                fi.Info.CopyTo(Path.Combine(IOUtil.GetDownloadsPath(), tgtName));
                Kit.Msg("已保存到下载目录：\r\n" + tgtName, 0);
            }
            catch
            {
                Kit.Warn("文件保存失败！");
            }
        }
#elif IOS
        async void OnSaveAs(object sender, Mi e)
        {
            var file = e.Data.To<LocalFileItem>();
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "分享文件",
                File = new ShareFile(file.Info.FullName)
            });
        }
#elif WASM
        async void OnSaveAs(object sender, Mi e)
        {
            var fi = e.Data.To<LocalFileItem>();
            var picker = Kit.GetFileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            string ext = ".txt";
            int index = fi.Name.LastIndexOf('.');
            if (index > -1)
                ext = fi.Name.Substring(index);
            picker.FileTypeChoices.Add("文件", new List<string>() { ext });
            picker.SuggestedFileName = fi.Name;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var data = File.ReadAllBytes(fi.Info.FullName);
                //Log.Debug($"长度：{data.Length}");
                //Log.Debug($"路径：{file.Path}");

                try
                {
                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    Kit.Msg("文件保存成功！");
                }
                catch
                {
                    Kit.Warn("文件保存失败！");
                }
            }
        }
#endif

        async void OnDel(object sender, Mi e)
        {
            var fi = e.Data.To<LocalFileItem>();
            if (_dir == ".data"
                && (fi.Name == "state.db" || fi.Name == "model.db"))
            {
                Kit.Warn("db文件正在使用中，删除对应的.ver文件并重启应用即可更新对应的db文件！", 0);
                return;
            }

            if (await Kit.Confirm($"确认要删除 [{fi.Name}] 吗?"))
            {
                try
                {
                    fi.Info.Delete();
                    LoadFiles();
                    Kit.Msg("文件删除成功");
                }
                catch
                {
                    Kit.Warn("文件删除失败！");
                }
            }
        }
    }

    class LocalFileItem
    {
        public LocalFileItem(FileInfo p_info)
        {
            Info = p_info;
        }

        public FileInfo Info { get; }

        public string Name => Info.Name;

        public string Date => Info.LastWriteTime.ToLongDateString();

        public string Size => Kit.GetFileSizeDesc((ulong)Info.Length);
    }
}
