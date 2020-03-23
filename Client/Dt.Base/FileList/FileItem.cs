﻿#region 文件描述
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
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName",
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
            new PropertyMetadata("\uE004"));

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

        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
            "Owner",
            typeof(FileList),
            typeof(FileItem),
            new PropertyMetadata(null));

        static void OnFileTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileItem vf = (FileItem)d;
            FileItemType tp = (FileItemType)e.NewValue;
            if (tp == FileItemType.Image)
                vf.Template = AtRes.VirImageTemplate;
            else if (tp == FileItemType.Video)
                vf.Template = AtRes.VirVideoTemplate;
            else
                vf.Template = AtRes.VirFileTemplate;
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
        static MediaPlayerElement _mediaPlayer;
        static FileItem _playerHost;

        readonly FileExtInfo _extInfo = new FileExtInfo();
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
        /// <summary>
        /// 构造方法
        /// </summary>
        public FileItem()
        {
            DefaultStyleKey = typeof(FileItem);
            Loaded += OnLoaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置文件ID：卷名/两级目录/xxx.ext
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 获取设置文件名称，不包括扩展名
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
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
                    _cmdDelete = new BaseCommand((e) => _ = Delete());
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
                    _cmdOpenFile = new BaseCommand((e) => _ = Open());
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
                    _cmdDownload = new BaseCommand((e) => _ = Download());
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
        /// 获取设置所属FileList
        /// </summary>
        internal FileList Owner
        {
            get { return (FileList)GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
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
        #endregion

        #region 文件操作
        /// <summary>
        /// 打开文件
        /// <para>先检查本地有没有，有打开本地，没有先下载；</para>
        /// <para>内部支持音视频的预览，其它类型文件用默认关联程序打开；</para>
        /// </summary>
        public async Task Open()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
                return;

            // 打开本地文件
            string fileName = Path.Combine(AtLocal.CachePath, GetFileName());
            if (!File.Exists(fileName))
            {
                // 先下载
                bool suc = await Download();
                if (!suc)
                    return;
            }

            var tp = FileType;
            if (tp == FileItemType.Sound || tp == FileItemType.Video)
            {
                Play(fileName);
            }
            else
            {
                // 默认关联程序打开
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(fileName)
                });
            }
        }

        /// <summary>
        /// 共享文件
        /// </summary>
        public async Task ShareFile()
        {
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
                File = new ShareFile(Path.Combine(AtLocal.CachePath, GetFileName()))
            });
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        public async Task UpdateFile()
        {
            if (State != FileItemState.None)
                return;

            var file = await FileKit.PickFile();
            if (file != null)
                await Owner.UpdateFile(file, this);
        }

        /// <summary>
        /// 文件另存为
        /// 过程和打开文件相同，先检查本地
        /// </summary>
        public async void SaveAs()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
                return;

            string fileName = Path.Combine(AtLocal.CachePath, GetFileName());
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
            picker.SuggestedFileName = FileName;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                StorageFile temp = await AtLocal.GetStorageFile(GetFileName());
                if (temp != null)
                {
                    await temp.CopyAndReplaceAsync(file);
                    AtKit.Msg("文件另存成功！");
                    return;
                }
            }
#elif ANDROID
            string savePath;
            string msg;
            if (FileType == FileItemType.Image)
            {
                savePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                msg = "照片";
            }
            else if (FileType == FileItemType.Video)
            {
                savePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies).AbsolutePath;
                msg = "电影";
            }
            else if (FileType == FileItemType.Sound)
            {
                savePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;
                msg = "音乐";
            }
            else
            {
                savePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
                msg = "文件目录";
            }

            // 复制本地文件
            try
            {
                File.Copy(fileName, Path.Combine(savePath, GetFileName()), true);
                AtKit.Msg($"已保存到{msg}！");
            }
            catch
            {
                AtKit.Warn("文件保存失败！");
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
                                AtKit.Msg("已保存到照片！");
                        });
                    }
                    else
                    {
                        UIKit.UIVideo.SaveToPhotosAlbum(fileName, (image, error) =>
                        {
                            if (error == null)
                                AtKit.Msg("已保存到照片！");
                        });
                    }
                }
                return;
            }
            // 如何存到"文件"？
            //var doc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), GetFileName());
            //try
            //{
            //    File.Copy(fileName, doc, true);
            //    AtKit.Msg($"已保存到文件！");
            //}
            //catch
            //{
            //    AtKit.Warn("文件保存失败！");
            //}
#endif
        }

        /// <summary>
        /// 删除已上传的文件
        /// </summary>
        public async Task<bool> Delete()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
            {
                AtKit.Warn("当前状态不允许删除文件！");
                return false;
            }

            bool suc = await AtFile.Delete(ID);
            if (suc)
                Owner.AfterDeleteItem(this);
            else
                AtKit.Warn("删除失败！");
            return suc;
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _loaded = true;

            // 上下文菜单
            Menu menu = Ex.GetMenu(Owner);
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
            if (FileType == FileItemType.Image || FileType == FileItemType.Video)
            {
                Grid pg = (Grid)GetTemplateChild("ContentGrid");
                pg.Tapped += OnTapped;
            }
            else
            {
                Grid rg = (Grid)GetTemplateChild("RootGrid");
                rg.PointerEntered += OnPointerEntered;
                rg.PointerPressed += OnPointerPressed;
                rg.PointerReleased += OnPointerReleased;
                rg.PointerCaptureLost += OnPointerCaptureLost;
                rg.PointerExited += OnPointerExited;
            }

            VisualStateManager.GoToState(this, State.ToString(), true);
            UpdateCachedFlag();
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
            FileName = p_file.DisplayName;
            _extInfo.FileDesc = p_file.Desc;
            _extInfo.Length = p_file.Size;
            _extInfo.Uploader = AtUser.Name;
            _extInfo.Date = AtSys.Now.ToString("yyyy-MM-dd HH:mm");

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
                AtLocal.DeleteFile(GetFileName());
            }

            ID = p_id;
            string fileName = GetFileName();

            // 复制到临时文件夹，免去再次下载
            //#if UWP
            //            try
            //            {
            //                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(p_file.FilePath);
            //                await file.CopyAsync(await StorageFolder.GetFolderFromPathAsync(AtLocal.CachePath), fileName, NameCollisionOption.ReplaceExisting);
            //            }
            //            catch { }
            //#else
            //            try
            //            {
            //                FileInfo fi = new FileInfo(p_file.FilePath);
            //                fi.CopyTo(Path.Combine(AtLocal.CachePath, fileName), true);
            //            }
            //            catch { }
            //#endif

            // 缩略图重命名
            if (!string.IsNullOrEmpty(p_file.ThumbPath))
            {
                string thumbName = GetThumbName();
#if UWP
                try
                {
                    var sf = await StorageFile.GetFileFromPathAsync(p_file.ThumbPath);
                    await sf.RenameAsync(thumbName);
                }
                catch { }
#else
                try
                {
                    FileInfo fi = new FileInfo(p_file.ThumbPath);
                    if (fi.Exists)
                    {
                        string newPath = Path.Combine(p_file.ThumbPath.Substring(0, p_file.ThumbPath.LastIndexOf('/')), thumbName);
                        fi.MoveTo(newPath);
                    }
                }
                catch { }
#endif
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
                    AtKit.Warn("当前状态不可下载！");
                return false;
            }

            bool downloadThumb = false;
            if (p_priorThumbnail)
            {
                // 优先下载缩略图时，先判断是否存在
                downloadThumb = await AtFile.Exists(ID.Substring(0, ID.LastIndexOf('.')) + "-t.jpg");
                // 缩略图不存在时，若为视频文件不下载
                if (!downloadThumb && FileType != FileItemType.Image)
                    return false;
            }

            string name = downloadThumb ? GetThumbName() : GetFileName();
            string path = Path.Combine(AtLocal.CachePath, name);
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
                    AtKit.Warn("创建文件出错！");
                Log.Error(ex, "下载前创建文件出错！");
                return false;
            }

            State = FileItemState.Downloading;
            DownloadInfo info = new DownloadInfo
            {
                Path = downloadThumb ? ID.Substring(0, ID.LastIndexOf('.')) + "-t.jpg" : ID,
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
                    AtKit.Warn(info.Error);

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
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            if ((FileType == FileItemType.Image || FileType == FileItemType.Video)
                && !string.IsNullOrEmpty(ID)
                && Bitmap == null)
            {
                LoadImage();
            }
        }

        async void LoadImage()
        {
            string thumbName = Path.Combine(AtLocal.CachePath, GetThumbName());
            string fileName = Path.Combine(AtLocal.CachePath, GetFileName());
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
                Icon = "\uE078";
                FileType = FileItemType.Sound;
                FileName = _extInfo.FileDesc;
                ExtInfo = $"{AtKit.GetFileSizeDesc(_extInfo.Length)}\r\n{_extInfo.Uploader}\r\n{_extInfo.Date}";
            }
            else if (FileFilter.UwpVideo.Contains(p_ext))
            {
                FileType = FileItemType.Video;
                ExtInfo = _extInfo.FileDesc;
            }
            else
            {
                UpdateIcon(p_ext);
                ExtInfo = $"{_extInfo.FileDesc}\r\n{AtKit.GetFileSizeDesc(_extInfo.Length)}  {_extInfo.Uploader}\r\n{_extInfo.Date}";
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
                    Icon = AtRes.GetIconChar(Icons.Word);
                    break;
                case ".xls":
                case ".xlsx":
                    Icon = AtRes.GetIconChar(Icons.Excel);
                    break;
                case ".zip":
                    Icon = AtRes.GetIconChar(Icons.Zip);
                    break;
                case ".rar":
                    Icon = AtRes.GetIconChar(Icons.Rar);
                    break;
                case ".txt":
                    Icon = AtRes.GetIconChar(Icons.文件);
                    break;
                case ".ppt":
                case ".pptx":
                    Icon = AtRes.GetIconChar(Icons.Ppt);
                    break;
                case ".htm":
                case ".html":
                    Icon = AtRes.GetIconChar(Icons.Html);
                    break;
                case ".exe":
                    Icon = AtRes.GetIconChar(Icons.Exe);
                    break;
                default:
                    Icon = AtRes.GetIconChar(Icons.文件);
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
                string path = System.IO.Path.Combine(AtLocal.CachePath, ID.Substring(index + 1));
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
            {
                int start = ID.LastIndexOf('/') + 1;
                return ID.Substring(start, ID.LastIndexOf('.') - start) + "-t.jpg";
            }
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
            return _extInfo.FileDesc;
        }
        #endregion

        #region 交互事件
        void TappedItem()
        {
            switch (State)
            {
                case FileItemState.None:
                    Open();
                    break;
                case FileItemState.UploadWaiting:
                case FileItemState.Uploading:
                    Owner.CancelTransfer();
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

        void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            TappedItem();
        }

        void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // 右键无效
            if (e.IsRightButton())
                return;

            if (((UIElement)sender).CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _pointerID = e.Pointer.PointerId;
                _ptLast = e.GetCurrentPoint(null).Position;
                VisualStateManager.GoToState(this, "Pressed", true);
            }
        }

        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_pointerID != e.Pointer.PointerId)
                return;

            e.Handled = true;
            ((UIElement)sender).ReleasePointerCapture(e.Pointer);
            _pointerID = null;
            Point cur = e.GetCurrentPoint(null).Position;
            if (Math.Abs(cur.X - _ptLast.X) < 6 && Math.Abs(cur.Y - _ptLast.Y) < 6)
                TappedItem();
        }

        void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
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
            if (AtSys.IsPhoneUI)
            {
                // PhoneUI模式
                if (p_menu.TouchTrigger == TouchTriggerEvent.Custom)
                {
                    btn = CreateMenuButton(p_menu);
                }
                else if (p_menu.TouchTrigger == TouchTriggerEvent.Holding)
                {
                    Holding += (s, e) =>
                    {
                        if (e.HoldingState == HoldingState.Started)
                            OpenContextMenu(default);
                    };
                    // win上触摸模式使用鼠标时不触发Holding事件！
                    if (AtSys.System == TargetSystem.Windows)
                        RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                }
                else
                {
                    Tapped += (s, e) => OpenContextMenu(default);
                }
            }
            else
            {
                // WindowsUI模式
                if (p_menu.MouseTrigger == MouseTriggerEvent.RightTapped)
                    RightTapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                else if (p_menu.MouseTrigger == MouseTriggerEvent.LeftTapped)
                    Tapped += (s, e) => OpenContextMenu(e.GetPosition(null));
                else
                    btn = CreateMenuButton(p_menu);
            }
            return btn;
        }

        Button CreateMenuButton(Menu p_menu)
        {
            // 自定义按钮触发
            var btn = new Button { Content = "\uE0DC", Style = AtRes.字符按钮, Foreground = AtRes.深灰边框, HorizontalAlignment = HorizontalAlignment.Right };
            btn.Click += (s, e) => OpenContextMenu(new Point(), (Button)s);
            if (AtSys.System == TargetSystem.Windows)
                p_menu.WinPlacement = MenuPosition.OuterLeftTop;
            return btn;
        }

        void OpenContextMenu(Point p_pos, FrameworkElement p_tgt = null)
        {
            Menu menu = Ex.GetMenu(Owner);
            if (menu != null)
            {
                menu.TargetData = this;
                menu.DataContext = this;
                menu.OpenContextMenu(p_pos, p_tgt);
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
            FileName = p_reader.ReadAsString();
            _extInfo.FileDesc = p_reader.ReadAsString();
            _extInfo.Length = (ulong)p_reader.ReadAsLong();
            _extInfo.Uploader = p_reader.ReadAsString();
            _extInfo.Date = p_reader.ReadAsString();

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
            p_writer.WriteStringValue(FileName);
            p_writer.WriteStringValue(_extInfo.FileDesc);
            p_writer.WriteNumberValue(_extInfo.Length);
            p_writer.WriteStringValue(_extInfo.Uploader);
            p_writer.WriteStringValue(_extInfo.Date);
            p_writer.WriteEndArray();
        }
        #endregion
    }
}