#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FileLists;
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 上传下载文件描述
    /// </summary>
    public partial class FileItem : Control, IUploadUI
    {
        #region 静态成员
        /// <summary>
        /// 缩略图后缀名
        /// </summary>
        const string ThumbPostfix = "-t.jpg";

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FileTypeProperty = DependencyProperty.Register(
            "FileType",
            typeof(FileItemType),
            typeof(FileItem),
            new PropertyMetadata(FileItemType.File, OnFileTypePropertyChanged));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State",
            typeof(FileItemState),
            typeof(FileItem),
            new PropertyMetadata(FileItemState.None, OnStatePropertyChanged));

        public static readonly DependencyProperty ExtInfoProperty = DependencyProperty.Register(
            "ExtInfo",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CachedFlagProperty = DependencyProperty.Register(
            "CachedFlag",
            typeof(Visibility),
            typeof(FileItem),
            new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata("\uE002"));

        public static readonly DependencyProperty ProgressWidthProperty = DependencyProperty.Register(
            "ProgressWidth",
            typeof(double),
            typeof(FileItem),
            new PropertyMetadata(0));

        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
            "Percent",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty BitmapProperty = DependencyProperty.Register(
            "Bitmap",
            typeof(ImageSource),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ImageStretchProperty = DependencyProperty.Register(
            "ImageStretch",
            typeof(Stretch),
            typeof(FileItem),
            new PropertyMetadata(Stretch.Uniform));

        static void OnFileTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileItem vf = (FileItem)d;
            FileItemType tp = (FileItemType)e.NewValue;
            if (tp == FileItemType.Image)
                vf.Template = Res.VirImageTemplate;
            else if (tp == FileItemType.Video)
                vf.Template = Res.VirVideoTemplate;
            else
                vf.Template = Res.VirFileTemplate;
        }

        static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileItem vf = (FileItem)d;
            if (vf._loaded)
            {
                var state = (FileItemState)e.NewValue;
                VisualStateManager.GoToState(vf, state.ToString(), true);
                vf.ProgressWidth = 0.0;
            }
        }
        #endregion

        #region 成员变量
        internal const string SelectFileDlgType = "Dt.App.File.SelectFileDlg,Dt.App";
        public const string ImageExt = "png,jpg,jpeg,bmp,gif,ico,tif";
        public const string VideoExt = "mp4,wmv,mov";

        static MediaPlayerElement _mediaPlayer;
        static FileItem _playerHost;

        readonly FileList _owner;
        readonly FileItemInfo _itemInfo = new FileItemInfo();
        bool _loaded;
        CancellationTokenSource _ctsDownload;
        uint? _pointerID;
        Point _ptLast;

        BaseCommand _cmdShare;
        BaseCommand _cmdUpdate;
        BaseCommand _cmdDelete;
        BaseCommand _cmdDownload;
        BaseCommand _cmdOpenFile;
        BaseCommand _cmdSaveAs;
        #endregion

        #region 构造方法
        public FileItem(FileList p_owner)
        {
            DefaultStyleKey = typeof(FileItem);
            _owner = p_owner;
            Loaded += OnLoaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置文件ID：卷名/两级目录/xxx.ext
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 获取设置标题，普通文件为名称，音频文件为时长
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 获取共享文件命令
        /// </summary>
        public BaseCommand CmdShare
        {
            get
            {
                if (_cmdShare == null)
                    _cmdShare = new BaseCommand((e) => _ = ShareFile());
                return _cmdShare;
            }
        }

        /// <summary>
        /// 获取更新文件命令
        /// </summary>
        public BaseCommand CmdUpdate
        {
            get
            {
                if (_cmdUpdate == null)
                    _cmdUpdate = new BaseCommand((e) => _ = UpdateFile());
                return _cmdUpdate;
            }
        }

        /// <summary>
        /// 获取删除上传文件
        /// </summary>
        public BaseCommand CmdDelete
        {
            get
            {
                if (_cmdDelete == null)
                    _cmdDelete = new BaseCommand((e) => _ = DeleteFile());
                return _cmdDelete;
            }
        }

        /// <summary>
        /// 获取打开文件命令
        /// </summary>
        public BaseCommand CmdOpen
        {
            get
            {
                if (_cmdOpenFile == null)
                    _cmdOpenFile = new BaseCommand((e) => _ = OpenFile());
                return _cmdOpenFile;
            }
        }

        /// <summary>
        /// 获取另存为命令
        /// </summary>
        public BaseCommand CmdSaveAs
        {
            get
            {
                if (_cmdSaveAs == null)
                    _cmdSaveAs = new BaseCommand((e) => SaveAs());
                return _cmdSaveAs;
            }
        }

        /// <summary>
        /// 获取下载命令
        /// </summary>
        public BaseCommand CmdDownload
        {
            get
            {
                if (_cmdDownload == null)
                    _cmdDownload = new BaseCommand((e) => DownloadFile());
                return _cmdDownload;
            }
        }
        #endregion

        #region 内部属性
        /// <summary>
        /// 获取设置文件种类
        /// </summary>
        internal FileItemType FileType
        {
            get { return (FileItemType)GetValue(FileTypeProperty); }
            set { SetValue(FileTypeProperty, value); }
        }

        /// <summary>
        /// 获取设置上传下载的当前状态
        /// </summary>
        internal FileItemState State
        {
            get { return (FileItemState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// 获取设置文件扩展信息
        /// </summary>
        internal string ExtInfo
        {
            get { return (string)GetValue(ExtInfoProperty); }
            set { SetValue(ExtInfoProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示已下载标志
        /// </summary>
        internal Visibility CachedFlag
        {
            get { return (Visibility)GetValue(CachedFlagProperty); }
            set { SetValue(CachedFlagProperty, value); }
        }

        /// <summary>
        /// 获取设置图标
        /// </summary>
        internal string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置进度条进度
        /// </summary>
        internal double ProgressWidth
        {
            get { return (double)GetValue(ProgressWidthProperty); }
            set { SetValue(ProgressWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置进度百分比
        /// </summary>
        internal string Percent
        {
            get { return (string)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        /// <summary>
        /// 获取设置源图像
        /// </summary>
        internal ImageSource Bitmap
        {
            get { return (ImageSource)GetValue(BitmapProperty); }
            set { SetValue(BitmapProperty, value); }
        }

        /// <summary>
        ///  获取设置图像填充模式，默认Uniform
        /// </summary>
        internal Stretch ImageStretch
        {
            get { return (Stretch)GetValue(ImageStretchProperty); }
            set { SetValue(ImageStretchProperty, value); }
        }
        #endregion

        #region 文件操作
#if !WASM
        /// <summary>
        /// 打开文件
        /// <para>先检查本地有没有，有打开本地，没有先下载；</para>
        /// <para>内部支持音视频的预览，其它类型文件用默认关联程序打开；</para>
        /// </summary>
        public async Task OpenFile()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
                return;

            // 打开本地文件
            string fileName = Path.Combine(Kit.CachePath, GetFileName());
            if (!File.Exists(fileName))
            {
                // 先下载
                bool suc = await Download();
                if (!suc)
                    return;
            }

            switch (FileType)
            {
                case FileItemType.Image:
                    await new ImageFileView().ShowDlg(_owner, this);
                    break;

                case FileItemType.Video:
                case FileItemType.Sound:
                    Play(fileName);
                    break;

                default:
                    // 默认关联程序打开
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(fileName)
                    });
                    break;
            }

            _owner.OnOpenedFile(this);
        }

        /// <summary>
        /// 共享文件
        /// </summary>
        public async Task ShareFile()
        {
            string fileName = Path.Combine(Kit.CachePath, GetFileName());
            if (!File.Exists(fileName))
            {
                // 先下载
                bool suc = await Download();
                if (!suc)
                    return;
            }

            string title;
            switch (FileType)
            {
                case FileItemType.Image:
                    title = "分享图片";
                    break;
                case FileItemType.Video:
                    title = "分享视频";
                    break;
                case FileItemType.Sound:
                    title = "分享音乐";
                    break;
                default:
                    title = "分享文件";
                    break;
            }

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(fileName)
            });
        }

        /// <summary>
        /// 文件另存为
        /// 过程和打开文件相同，先检查本地
        /// </summary>
        public async void SaveAs()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
                return;

            string fileName = Path.Combine(Kit.CachePath, GetFileName());
            if (!File.Exists(fileName))
            {
                // 先下载
                bool suc = await Download();
                if (!suc)
                    return;
            }

#if UWP
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add(GetSaveDesc(), new List<string>() { GetExtName() });
            picker.SuggestedFileName = _itemInfo.FileName;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(Kit.CachePath);
                var temp = await folder.TryGetItemAsync(GetFileName()) as StorageFile;
                if (temp != null)
                {
                    await temp.CopyAndReplaceAsync(file);
                    Kit.Msg("文件另存成功！");
                    return;
                }
            }
#elif ANDROID
            //string savePath;
            //string msg;
            //if (FileType == FileItemType.Image)
            //{
            //    savePath = IOUtil.GetPicturesPath();
            //    msg = "照片";
            //}
            //else if (FileType == FileItemType.Video)
            //{
            //    savePath = IOUtil.GetMoviesPath();
            //    msg = "电影";
            //}
            //else if (FileType == FileItemType.Sound)
            //{
            //    savePath = IOUtil.GetMusicPath();
            //    msg = "音乐";
            //}
            //else
            //{
            //    savePath = IOUtil.GetDownloadsPath();
            //    msg = "下载";
            //}
            try
            {
                // 复制本地文件
                // android 11.0 因放在其他公共目录时被过滤显示，统一放在下载
                File.Copy(fileName, Path.Combine(IOUtil.GetDownloadsPath(), GetFileName()), true);
                Kit.Msg("已保存到下载目录！");
            }
            catch
            {
                Kit.Warn("文件保存失败！");
            }
#elif IOS
            if (FileType == FileItemType.Image || FileType == FileItemType.Video)
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Photos>();

                if (status == PermissionStatus.Granted)
                {
                    if (FileType == FileItemType.Image)
                    {
                        var imageData = System.IO.File.ReadAllBytes(fileName);
                        var myImage = new UIKit.UIImage(Foundation.NSData.FromArray(imageData));
                        myImage.SaveToPhotosAlbum((image, error) =>
                        {
                            if (error == null)
                                Kit.Msg("已保存到照片！");
                        });
                    }
                    else
                    {
                        UIKit.UIVideo.SaveToPhotosAlbum(fileName, (image, error) =>
                        {
                            if (error == null)
                                Kit.Msg("已保存到照片！");
                        });
                    }
                }
            }
            else
            {
                // 普通文件以共享方式保存
                await ShareFile();
            }
#endif
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public void DownloadFile()
        {
            _ = Download();
        }

        internal async Task<string> EnsureFileExists()
        {
            string fileName = Path.Combine(Kit.CachePath, GetFileName());
            if (!File.Exists(fileName))
            {
                // 先下载
                await Download();
            }
            return fileName;
        }
#endif

        /// <summary>
        /// 更新文件
        /// </summary>
        public async Task UpdateFile()
        {
            if (State != FileItemState.None)
                return;

            if (Type.GetType(SelectFileDlgType) != null)
            {
                int result = await new UpdateFileDlg().ShowDlg();
                if (result < 0)
                    return;

                if (result == 0)
                {
                    // 通过从库中选择文件进行更新，无需上传
                    string ext = null;
                    if (FileType == FileItemType.Image)
                        ext = ImageExt;
                    else if (FileType == FileItemType.Video)
                        ext = VideoExt;

                    var dlg = (ISelectFileDlg)Activator.CreateInstance(Type.GetType(SelectFileDlgType));
                    if (await dlg.Show(false, ext)
                        && dlg.SelectedFiles != null
                        && dlg.SelectedFiles.Count > 0)
                    {
                        _owner.UpdateExistFiles(dlg.SelectedFiles[0], this);
                    }
                    return;
                }
            }

            // 上传同类型文件
            FileData file;
            if (FileType == FileItemType.Image)
                file = await Kit.PickImage();
            else if (FileType == FileItemType.Video)
                file = await Kit.PickVideo();
            else
                file = await Kit.PickFile();
            if (file != null)
                await _owner.UpdateFile(file, this);
        }

        /// <summary>
        /// 删除已上传的文件
        /// </summary>
        public async Task<bool> DeleteFile()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
            {
                Kit.Warn("当前状态不允许删除文件！");
                return false;
            }

            bool suc = await AtFile.Delete(ID);
            if (suc)
                _owner.AfterDeleteItem(this);
            else
                Kit.Warn("删除文件失败！");
            return suc;
        }
        #endregion

        #region IUploadUI 上传过程
        /// <summary>
        /// 上传进度回调
        /// </summary>
        ProgressDelegate IUploadUI.UploadProgress => OnUploadProgress;

        /// <summary>
        /// 准备上传
        /// </summary>
        /// <param name="p_file">待上传文件对象</param>
        /// <returns></returns>
        async Task IUploadUI.InitUpload(FileData p_file)
        {
            State = FileItemState.UploadWaiting;

            // 基础属性
            _itemInfo.FileName = p_file.DisplayName;
            _itemInfo.FileDesc = p_file.Desc;
            _itemInfo.Length = p_file.Size;
            _itemInfo.Uploader = Kit.UserName;
            _itemInfo.Date = Kit.Now.ToString("yyyy-MM-dd HH:mm");

            // 更新控件模板及扩展信息
            UpdateTemplate(p_file.Ext);

            // 含缩略图
            if (FileType == FileItemType.Image || FileType == FileItemType.Video)
            {
                BitmapImage bmp = new BitmapImage();
#if UWP
                StorageFile sf = null;
                if (!string.IsNullOrEmpty(p_file.ThumbPath))
                {
                    // 缩略图
                    sf = await StorageFile.GetFileFromPathAsync(p_file.ThumbPath);
                }
                else if (FileType == FileItemType.Image)
                {
                    // 原始图
                    sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(p_file.FilePath);
                }

                if (sf != null)
                {
                    using (var stream = await sf.OpenAsync(FileAccessMode.Read))
                    {
                        await bmp.SetSourceAsync(stream);
                    }
                }
#elif ANDROID
                if (!string.IsNullOrEmpty(p_file.ThumbPath))
                {
                    // 缩略图
                    using (var stream = System.IO.File.OpenRead(p_file.ThumbPath))
                    {
                        await bmp.SetSourceAsync(stream);
                    }
                }
                else
                {
                    // 原始图
                    using (var stream = await p_file.GetStream())
                    {
                        await bmp.SetSourceAsync(stream);
                    }
                }
#elif IOS
                if (!string.IsNullOrEmpty(p_file.ThumbPath))
                {
                    // 缩略图
                    using (var stream = new FileStream(p_file.ThumbPath, FileMode.Open, FileAccess.Read))
                    {
                        await bmp.SetSourceAsync(stream);
                    }
                }
                else
                {
                    // 原始图
                    using (var stream = await p_file.GetStream())
                    {
                        await bmp.SetSourceAsync(stream);
                    }
                }
#elif WASM
                // 暂未实现
                await Task.CompletedTask;
#endif
                Bitmap = bmp;
            }
        }

        /// <summary>
        /// 上传成功后
        /// </summary>
        /// <param name="p_id">文件上传路径</param>
        /// <param name="p_file">已上传文件对象</param>
        /// <returns></returns>
        async Task IUploadUI.UploadSuccess(string p_id, FileData p_file)
        {
            // 更新时删除旧文件
            if (!string.IsNullOrEmpty(ID))
            {
                // 删除服务器端旧文件
                await AtFile.Delete(ID);
                // 删除本地旧文件
                Kit.DeleteCacheFile(GetFileName());
            }

            ID = p_id;

            // 若文件已在CachePath，重命名免去再次下载，如录音、拍照、录视频等临时文件已在CachePath
            string filePath = p_file.FilePath;
#if UWP
            // 为安全访问ID
            if (filePath.StartsWith("{"))
            {
                try
                {
                    var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(filePath);
                    filePath = file.Path;
                }
                catch { }
            }
#endif
            if (filePath.StartsWith(Kit.CachePath))
            {
                try
                {
                    FileInfo fi = new FileInfo(filePath);
                    if (fi.Exists)
                    {
                        // 重命名免去再次下载
                        string newPath = Path.Combine(Kit.CachePath, GetFileName());
                        fi.MoveTo(newPath);
                        UpdateCachedFlag();
                    }
                }
                catch { }
            }

            // 缩略图重命名
            if (!string.IsNullOrEmpty(p_file.ThumbPath))
            {
                try
                {
                    FileInfo fi = new FileInfo(p_file.ThumbPath);
                    if (fi.Exists)
                    {
                        string newPath = Path.Combine(Kit.CachePath, GetThumbName());
                        fi.MoveTo(newPath);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 上传失败后
        /// </summary>
        /// <param name="p_file"></param>
        void IUploadUI.UploadFail(FileData p_file)
        {
            p_file.DeleteThumbnail();
        }

        /// <summary>
        /// 上传进度回调
        /// </summary>
        /// <param name="p_bytesStep">本次发送字节数</param>
        /// <param name="p_bytesSent">共发送字节数</param>
        /// <param name="p_totalBytesToSend">总字节数</param>
        async void OnUploadProgress(long p_bytesStep, long p_bytesSent, long p_totalBytesToSend)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                // 开始发送
                if (p_bytesStep == p_bytesSent)
                    State = FileItemState.Uploading;

                double percent = p_bytesSent * 100 / p_totalBytesToSend;
                Percent = string.Format("{0}%", percent);
                ProgressWidth = Math.Ceiling(percent / 100 * ActualWidth);

                // 完成
                if (p_bytesSent == p_totalBytesToSend)
                {
                    State = FileItemState.None;
                    ClearValue(PercentProperty);
                }
            }));
        }
        #endregion

        #region 下载
        /// <summary>
        /// 执行下载
        /// </summary>
        /// <param name="p_priorThumbnail">是否优先下载缩略图</param>
        /// <param name="p_prompt">是否提示下载失败信息</param>
        /// <returns></returns>
        internal async Task<bool> Download(bool p_priorThumbnail = false, bool p_prompt = true)
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
            {
                if (p_prompt)
                    Kit.Warn("当前状态不可下载！");
                return false;
            }

            bool downloadThumb = false;
            if (p_priorThumbnail)
            {
                // 优先下载缩略图时，先判断是否存在
                downloadThumb = await AtFile.Exists(ID + ThumbPostfix);
                // 缩略图不存在时，若为视频文件不下载
                if (!downloadThumb && FileType != FileItemType.Image)
                    return false;
            }

            string path = Path.Combine(Kit.CachePath, downloadThumb ? GetThumbName() : GetFileName());
            FileStream stream = null;
            try
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                stream = System.IO.File.Create(path);
            }
            catch (Exception ex)
            {
                if (p_prompt)
                    Kit.Warn("创建文件出错！");
                Log.Error(ex, "下载前创建文件出错！");
                return false;
            }

            State = FileItemState.Downloading;
            DownloadInfo info = new DownloadInfo
            {
                Path = downloadThumb ? ID + ThumbPostfix : ID,
                TgtStream = stream,
                Progress = OnDownloadProgress,
            };

            bool suc = false;
            _ctsDownload?.Dispose();
            _ctsDownload = new CancellationTokenSource();
            try
            {
                suc = await Downloader.GetFile(info, _ctsDownload.Token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "下载出错");
            }
            finally
            {
                _ctsDownload?.Dispose();
                info.TgtStream.Close();
                State = FileItemState.None;
            }

            if (!suc)
            {
                if (p_prompt)
                    Kit.Warn(info.Error);

                // 未成功，删除缓存文件，避免打开时出错
                try
                {
                    // mono中 FileInfo 的 Exists 状态不同步！
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                catch { }
            }
            else
            {
                VisualStateManager.GoToState(this, "Cached", true);
            }
            return suc;
        }

        /// <summary>
        /// 下载进度回调
        /// </summary>
        /// <param name="p_bytesStep"></param>
        /// <param name="p_bytesSent"></param>
        /// <param name="p_totalBytesToSend"></param>
        async void OnDownloadProgress(long p_bytesStep, long p_bytesSent, long p_totalBytesToSend)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                double percent = p_bytesSent * 100 / p_totalBytesToSend;
                Percent = string.Format("{0}%", percent);
                ProgressWidth = Math.Ceiling(percent / 100 * ActualWidth);

                // 完成
                if (p_bytesStep == 0 && p_bytesSent == p_totalBytesToSend)
                {
                    State = FileItemState.None;
                    ClearValue(PercentProperty);
                }
            }));
        }
        #endregion

        #region 加载后
        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            // -----原来放在OnApplyTemplate-----
            _loaded = true;

            // 上下文菜单
            Menu menu = Ex.GetMenu(_owner);
            if (menu != null)
            {
                var btn = AttachContextMenu(menu);
                if (btn != null)
                {
                    Grid grid = (Grid)GetTemplateChild("RootGrid");
                    grid.Children.Insert(grid.Children.Count - 2, btn);
                }
            }

            // 交互事件
            Grid pg;
            if (FileType == FileItemType.Image || FileType == FileItemType.Video)
                pg = (Grid)GetTemplateChild("ContentGrid");
            else
                pg = (Grid)GetTemplateChild("RootGrid");
            pg.PointerEntered += OnPointerEntered;
            pg.PointerPressed += OnPointerPressed;
            pg.PointerReleased += OnPointerReleased;
            pg.PointerCaptureLost += OnPointerCaptureLost;
            pg.PointerExited += OnPointerExited;
            Tapped += OnTapped;
            RightTapped += OnRightTapped;

            VisualStateManager.GoToState(this, State.ToString(), true);
            UpdateCachedFlag();
            // ---------------------

            if ((FileType == FileItemType.Image || FileType == FileItemType.Video)
                && !string.IsNullOrEmpty(ID)
                && Bitmap == null)
            {
                await LoadImage();
            }

            // 控件中的尽可能避免使用设置DataContext方式！！！
            // 采用DataContext设置为_owner时，uno存在的问题：
            // 通过Owner.XXX的方式总是重置FileList.DataContext为null！
            // 不可将此赋值放在LoadImage之前，否则无法显示图片
            SetBinding(BorderBrushProperty, new Binding { Path = new PropertyPath("BorderBrush"), Source = _owner, Mode = BindingMode.OneWay });
            SetBinding(ImageStretchProperty, new Binding { Path = new PropertyPath("ImageStretch"), Source = _owner, Mode = BindingMode.OneWay });
        }

#if !WASM
        async Task LoadImage()
        {
            string thumbName = Path.Combine(Kit.CachePath, GetThumbName());
            string fileName = Path.Combine(Kit.CachePath, GetFileName());
            string path = null;
            if (File.Exists(thumbName))
            {
                // 缩略图
                path = thumbName;
            }
            else if (FileType == FileItemType.Image && File.Exists(fileName))
            {
                // 无缩略图时取原始图片
                path = fileName;
            }
            else if (await Download(true, false))
            {
                // 优先下载缩略图，无缩略图下载原始图
                if (File.Exists(thumbName))
                    path = thumbName;
                else if (FileType == FileItemType.Image && File.Exists(fileName))
                    path = fileName;
            }

            if (!string.IsNullOrEmpty(path))
                Bitmap = new BitmapImage(new Uri(path));
        }
#endif
        #endregion

        #region 更新UI
        /// <summary>
        /// 更新控件模板及扩展信息
        /// </summary>
        /// <param name="p_ext">扩展名</param>
        void UpdateTemplate(string p_ext)
        {
            // 更新控件模板
            if (FileFilter.UwpImage.Contains(p_ext))
            {
                FileType = FileItemType.Image;
            }
            else if (FileFilter.UwpAudio.Contains(p_ext))
            {
                FileType = FileItemType.Sound;
                Icon = "\uE0A6";
                Title = _itemInfo.FileDesc;
                ExtInfo = $"{Kit.GetFileSizeDesc(_itemInfo.Length)}\r\n{_itemInfo.Uploader}\r\n{_itemInfo.Date}";
            }
            else if (FileFilter.UwpVideo.Contains(p_ext))
            {
                FileType = FileItemType.Video;
                ExtInfo = _itemInfo.FileDesc;
            }
            else
            {
                UpdateIcon(p_ext);
                Title = _itemInfo.FileName;
                ExtInfo = $"{_itemInfo.FileDesc}\r\n{Kit.GetFileSizeDesc(_itemInfo.Length)}  {_itemInfo.Uploader}\r\n{_itemInfo.Date}";
            }

            if (_loaded)
                UpdateCachedFlag();
        }

        /// <summary>
        /// 更新图标，缺图标！
        /// </summary>
        /// <param name="p_ext"></param>
        void UpdateIcon(string p_ext)
        {
            switch (p_ext)
            {
                case ".doc":
                case ".docx":
                    Icon = Res.GetIconChar(Icons.Word);
                    break;
                case ".xls":
                case ".xlsx":
                    Icon = Res.GetIconChar(Icons.Excel);
                    break;
                case ".zip":
                    Icon = Res.GetIconChar(Icons.Zip);
                    break;
                case ".rar":
                    Icon = Res.GetIconChar(Icons.Rar);
                    break;
                case ".txt":
                    Icon = Res.GetIconChar(Icons.文件);
                    break;
                case ".ppt":
                case ".pptx":
                    Icon = Res.GetIconChar(Icons.Ppt);
                    break;
                case ".htm":
                case ".html":
                    Icon = Res.GetIconChar(Icons.Html);
                    break;
                case ".exe":
                    Icon = Res.GetIconChar(Icons.Exe);
                    break;
                default:
                    Icon = Res.GetIconChar(Icons.文件);
                    break;
            }
        }

        /// <summary>
        /// 更新已下载标志
        /// </summary>
        void UpdateCachedFlag()
        {
            int index;
            string state = "NoCache";
            if (!string.IsNullOrEmpty(ID) && (index = ID.LastIndexOf('/')) > 0)
            {
                string path = System.IO.Path.Combine(Kit.CachePath, ID.Substring(index + 1));
                if (System.IO.File.Exists(path))
                    state = "Cached";
            }
            VisualStateManager.GoToState(this, state, true);
        }
        #endregion

        #region 内部方法
        string GetFileName()
        {
            if (!string.IsNullOrEmpty(ID))
                return ID.Substring(ID.LastIndexOf('/') + 1);
            return null;
        }

        string GetThumbName()
        {
            if (!string.IsNullOrEmpty(ID))
                return ID.Substring(ID.LastIndexOf('/') + 1) + ThumbPostfix;
            return null;
        }

        /// <summary>
        /// 获取扩展名，以 . 开头
        /// </summary>
        /// <returns></returns>
        string GetExtName()
        {
            int index;
            if (!string.IsNullOrEmpty(ID) && (index = ID.LastIndexOf('.')) != -1)
            {
                return ID.Substring(index);
            }
            return null;
        }

        /// <summary>
        /// 获取保存时的文件说明
        /// </summary>
        string GetSaveDesc()
        {
            switch (FileType)
            {
                case FileItemType.Image:
                    return "图像";
                case FileItemType.Sound:
                    return "音频";
                case FileItemType.Video:
                    return "视频";
            }
            return _itemInfo.FileDesc;
        }
        #endregion

        #region 交互事件
        void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_owner.EnableClick)
                return;

            e.Handled = true;
            switch (State)
            {
                case FileItemState.None:
                    _ = OpenFile();
                    break;
                case FileItemState.UploadWaiting:
                case FileItemState.Uploading:
                    _owner.CancelTransfer();
                    break;
                case FileItemState.Downloading:
                    if (_ctsDownload != null)
                    {
                        _ctsDownload.Cancel();
                        _ctsDownload.Dispose();
                        _ctsDownload = null;
                    }
                    break;
            }
        }

        void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // uno中只附加Tapped事件时长按也触发Tapped，两事件都附加时长按只触发RightTapped！！！
            // 只为屏蔽长按时的Tapped
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (_owner.EnableClick)
                VisualStateManager.GoToState(this, "PointerOver", true);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // 右键无效
            if (!_owner.EnableClick || e.IsRightButton())
                return;

            if (((UIElement)sender).CapturePointer(e.Pointer))
            {
                _pointerID = e.Pointer.PointerId;
                _ptLast = e.GetCurrentPoint(null).Position;
                VisualStateManager.GoToState(this, "Pressed", true);
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_owner.EnableClick || _pointerID != e.Pointer.PointerId)
                return;

            ((UIElement)sender).ReleasePointerCapture(e.Pointer);
            _pointerID = null;
        }

        void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (_owner.EnableClick)
                VisualStateManager.GoToState(this, "Normal", true);
        }
        #endregion

        #region 播放音视频
        /// <summary>
        /// 播放声音或视频
        /// </summary>
        /// <param name="p_file">待播放文件</param>
        async void Play(string p_file)
        {
            if (_playerHost == this)
            {
                if (FileType == FileItemType.Video)
                {
                    var player = _mediaPlayer.MediaPlayer;
                    if (player.PlaybackSession.PlaybackState == Windows.Media.Playback.MediaPlaybackState.Playing)
                    {
                        _mediaPlayer.AreTransportControlsEnabled = true;
                        player.Pause();
                    }
                    else if (player.PlaybackSession.PlaybackState == Windows.Media.Playback.MediaPlaybackState.Paused)
                    {
                        _mediaPlayer.AreTransportControlsEnabled = false;
                        player.Play();
                    }
                }
                return;
            }

            if (_mediaPlayer == null)
            {
                // 初始化播放器
                _mediaPlayer = new MediaPlayerElement();
                _mediaPlayer.AutoPlay = true;

                var player = _mediaPlayer.MediaPlayer;
                if (player == null)
                {
                    player = new Windows.Media.Playback.MediaPlayer();
                    _mediaPlayer.SetMediaPlayer(player);
                }
                player.MediaEnded += (sender, e) => StopPlayer();
                player.MediaFailed += (sender, e) => StopPlayer();

                // 自定义播放器内容
                //_mediaPlayer.AreTransportControlsEnabled = true;
                //var con = _mediaPlayer.TransportControls;
                //con.IsCompact = true;
                //con.IsVolumeButtonVisible = false;
                //con.IsVolumeEnabled = false;
                //con.IsZoomButtonVisible = false;
                //con.IsZoomEnabled = false;
            }
            else
            {
                await StopPlayer();
            }

            _mediaPlayer.AreTransportControlsEnabled = (FileType == FileItemType.Sound);
            LoadPlayer();
            _mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(p_file));
        }

        void LoadPlayer()
        {
            Grid grid = (Grid)GetTemplateChild(FileType == FileItemType.Video ? "ContentGrid" : "RootGrid");
            if (grid != null
                && grid.Children[grid.Children.Count - 1] != _mediaPlayer)
            {
                _mediaPlayer.Height = ActualHeight;
                grid.Children.Add(_mediaPlayer);
                _playerHost = this;
            }
        }

        Task UnloadPlayer()
        {
            return Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                Grid grid = (Grid)GetTemplateChild(FileType == FileItemType.Video ? "ContentGrid" : "RootGrid");
                if (grid != null
                    && grid.Children[grid.Children.Count - 1] == _mediaPlayer)
                {
                    grid.Children.RemoveAt(grid.Children.Count - 1);
                }
                _playerHost = null;
            })).AsTask();
        }

        static Task StopPlayer()
        {
            if (_playerHost != null)
                return _playerHost.UnloadPlayer();
            return Task.CompletedTask;
        }
        #endregion

        #region 上下文菜单
        /// <summary>
        /// 附加上下文菜单触发事件
        /// </summary>
        /// <param name="p_menu"></param>
        /// <returns></returns>
        Button AttachContextMenu(Menu p_menu)
        {
            Button btn = null;
            if (p_menu.TriggerEvent == TriggerEvent.RightTapped)
                RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
            else if (p_menu.TriggerEvent == TriggerEvent.LeftTapped)
                Tapped += (s, e) => OpenContextMenu(e.GetPosition(null));
            else
                btn = CreateMenuButton(p_menu);
            return btn;
        }

        Button CreateMenuButton(Menu p_menu)
        {
            // 自定义按钮触发
            var btn = new Button { Content = "\uE03F", Style = Res.字符按钮, Foreground = Res.深灰2, HorizontalAlignment = HorizontalAlignment.Right };
            btn.Click += (s, e) => OpenContextMenu(new Point(), (Button)s);
            if (!Kit.IsPhoneUI)
                p_menu.Placement = MenuPosition.OuterLeftTop;
            return btn;
        }

        void OpenContextMenu(Point p_pos, FrameworkElement p_tgt = null)
        {
            Menu menu = Ex.GetMenu(_owner);
            if (menu != null)
            {
                menu.TargetData = this;
                menu.DataContext = this;
                _ = menu.OpenContextMenu(p_pos, p_tgt);
            }
        }
        #endregion

        #region Json
        /// <summary>
        /// 反序列化，初次加载
        /// </summary>
        /// <param name="p_reader"></param>
        /// <returns></returns>
        internal void ReadData(ref Utf8JsonReader p_reader)
        {
            ID = p_reader.ReadAsString();
            _itemInfo.FileName = p_reader.ReadAsString();
            _itemInfo.FileDesc = p_reader.ReadAsString();
            _itemInfo.Length = (ulong)p_reader.ReadAsLong();
            _itemInfo.Uploader = p_reader.ReadAsString();
            _itemInfo.Date = p_reader.ReadAsString();

            // FileItem ]
            p_reader.Read();

            UpdateTemplate(GetExtName());
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="p_writer"></param>
        internal void WriteData(Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteStringValue(ID);
            p_writer.WriteStringValue(_itemInfo.FileName);
            p_writer.WriteStringValue(_itemInfo.FileDesc);
            p_writer.WriteNumberValue(_itemInfo.Length);
            p_writer.WriteStringValue(_itemInfo.Uploader);
            p_writer.WriteStringValue(_itemInfo.Date);
            p_writer.WriteEndArray();
        }
        #endregion
    }
}