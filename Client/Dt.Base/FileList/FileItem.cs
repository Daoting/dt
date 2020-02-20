#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Input;
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
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 上传下载文件描述
    /// </summary>
    public partial class FileItem : Control, IUploadFile
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

        public static readonly DependencyProperty ExtProperty = DependencyProperty.Register(
            "Ext",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty UnreadProperty = DependencyProperty.Register(
            "Unread",
            typeof(bool),
            typeof(FileItem),
            new PropertyMetadata(false));

        public static readonly DependencyProperty FileDescProperty = DependencyProperty.Register(
            "FileDesc",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length",
            typeof(ulong),
            typeof(FileItem),
            new PropertyMetadata(0));

        public static readonly DependencyProperty UploaderProperty = DependencyProperty.Register(
            "Uploader",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            "Date",
            typeof(string),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(object),
            typeof(FileItem),
            new PropertyMetadata(null));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State",
            typeof(FileItemState),
            typeof(FileItem),
            new PropertyMetadata(FileItemState.None, OnStatePropertyChanged));

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

        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(
            "MediaPlayer",
            typeof(object),
            typeof(FileItem),
            new PropertyMetadata(null));

        static void OnFileTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileItem vf = (FileItem)d;
            FileItemType tp = (FileItemType)e.NewValue;
            if (tp == FileItemType.Image)
                vf.Template = AtRes.VirImageTemplate;
            else if (tp == FileItemType.Sound)
                vf.Template = AtRes.VirSoundTemplate;
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
        static MediaElement _player;
        static FileItem _playerHost;

        uint? _pointerID;
        bool _loaded;
        CancellationTokenSource _ctsDownload;
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

        /// <summary>
        /// 获取设置文件种类
        /// </summary>
        public FileItemType FileType
        {
            get { return (FileItemType)GetValue(FileTypeProperty); }
            set { SetValue(FileTypeProperty, value); }
        }

        /// <summary>
        /// 获取设置文件扩展名，以 . 开头
        /// </summary>
        public string Ext
        {
            get { return (string)GetValue(ExtProperty); }
            set { SetValue(ExtProperty, value); }
        }

        /// <summary>
        /// 获取设置文件类型及说明，绑定用，形如：文本文件 (.txt)
        /// </summary>
        public string FileDesc
        {
            get { return (string)GetValue(FileDescProperty); }
            set { SetValue(FileDescProperty, value); }
        }

        /// <summary>
        /// 获取设置未读标志
        /// </summary>
        public bool Unread
        {
            get { return (bool)GetValue(UnreadProperty); }
            set { SetValue(UnreadProperty, value); }
        }

        /// <summary>
        /// 获取设置文件大小
        /// </summary>
        public ulong Length
        {
            get { return (ulong)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        /// <summary>
        /// 获取或设置上传文件用户
        /// </summary>
        public string Uploader
        {
            get { return (string)GetValue(UploaderProperty); }
            set { SetValue(UploaderProperty, value); }
        }

        /// <summary>
        /// 获取或设置文件上传日期
        /// </summary>
        public string Date
        {
            get { return (string)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        /// <summary>
        /// 获取设置上传下载的当前状态
        /// </summary>
        public FileItemState State
        {
            get { return (FileItemState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// 获取设置图标
        /// </summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置进度条进度
        /// </summary>
        public double ProgressWidth
        {
            get { return (double)GetValue(ProgressWidthProperty); }
            set { SetValue(ProgressWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置进度百分比
        /// </summary>
        public string Percent
        {
            get { return (string)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        /// <summary>
        /// 获取设置源图像
        /// </summary>
        public ImageSource Bitmap
        {
            get { return (ImageSource)GetValue(BitmapProperty); }
            set { SetValue(BitmapProperty, value); }
        }

        /// <summary>
        /// 获取设置播放器
        /// </summary>
        public object MediaPlayer
        {
            get { return GetValue(MediaPlayerProperty); }
            set { SetValue(MediaPlayerProperty, value); }
        }

        /// <summary>
        /// 获取设置所属FileList
        /// </summary>
        public FileList Owner { get; internal set; }

        /// <summary>
        /// 获取保存时的文件说明
        /// </summary>
        public string SaveDesc
        {
            get
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
                return FileDesc;
            }
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 打开文件
        /// <para>先检查本地有没有，有打开本地，没有先下载；</para>
        /// <para>内部支持图片和音视频的预览，其它类型文件用默认关联程序打开；</para>
        /// </summary>
        public async void Open()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
            {
                AtKit.Warn("当前状态不允许打开文件！");
                return;
            }

            // 打开本地文件
            string name = GetFileName();
            StorageFile file = await AtSys.GetUwpDocFile(name);
            if (file == null)
            {
                // 先下载再打开
                bool suc = await Download();
                if (suc)
                    file = await AtSys.GetUwpDocFile(name);
            }
            if (file == null)
                return;

            //var tp = FileType;
            //if (tp == FileItemType.Sound || tp == FileItemType.Video)
            //    Play(file);
            //else
            //    AtKit.OpenFile(file);

            // 记录已读标志
            if (Unread)
            {
                FileReadLog log = new FileReadLog();
                log.FileID = ID;
                log.UserName = AtUser.Name;
                AtLocal.Insert(log);
                Unread = false;
            }
        }

        /// <summary>
        /// 文件另存为
        /// 过程和打开文件相同，先检查本地
        /// </summary>
        public async void SaveAs()
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
            {
                AtKit.Warn("当前状态不允许文件另存！");
                return;
            }

            string name = GetFileName();
            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add(SaveDesc, new List<string>() { Ext });
            picker.SuggestedFileName = FileName;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file == null)
                return;

            // 复制本地文件
            StorageFile temp = await AtSys.GetUwpDocFile(name);
            if (temp != null)
            {
                await temp.CopyAndReplaceAsync(file);
                AtKit.Msg("文件另存成功！");
                return;
            }

            // 先下载再另存
            if (await Download())
            {
                temp = await AtSys.GetUwpDocFile(name);
                if (temp != null)
                {
                    await temp.CopyAndReplaceAsync(file);
                    AtKit.Msg("下载完毕，文件另存成功！");
                }
            }
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
                Owner.RemoveChild(this);
            else
                AtKit.Warn("删除失败！");
            return suc;
        }
        #endregion

        #region 上传
        /// <summary>
        /// 获取设置待上传的文件
        /// </summary>
        public FileData File { get; set; }

        /// <summary>
        /// 获取上传进度处理对象
        /// </summary>
        public ProgressDelegate UploadProgress
        {
            get { return OnUploadProgress; }
        }

        /// <summary>
        /// 准备上传，初始化文件属性
        /// </summary>
        /// <param name="p_file"></param>
        /// <param name="p_date"></param>
        /// <returns></returns>
        internal async Task InitUpload(FileData p_file, string p_date)
        {
            State = FileItemState.UploadWaiting;
            File = p_file;

            // 基础属性
            FileName = p_file.DisplayName;
            var ext = p_file.Ext;
            Ext = ext;
            FileDesc = p_file.Desc;
            Length = p_file.Size;
            Uploader = AtUser.Name;
            Date = p_date;

            // 类型及文件描述
            if (FileFilter.UwpImage.Contains(ext))
            {
                FileType = FileItemType.Image;
            }
            else if (FileFilter.UwpVideo.Contains(ext))
            {
                FileType = FileItemType.Video;
            }
            else if (FileFilter.UwpAudio.Contains(ext))
            {
                FileType = FileItemType.Sound;
            }
            else
            {
                UpdateIcon();
            }

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
        /// <returns></returns>
        internal async Task UploadSuccess(string p_id)
        {
            // 更新时删除旧文件
            if (!string.IsNullOrEmpty(ID))
            {
                // 删除服务器端旧文件
                await AtFile.Delete(ID);
                // 删除本地旧文件
                AtSys.DeleteDocFile(GetFileName());
            }

            ID = p_id;
            string fileName = GetFileName();

            // 复制到临时文件夹，免去再次下载
#if UWP
            try
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(File.FilePath);
                await file.CopyAsync(await StorageFolder.GetFolderFromPathAsync(AtSys.DocPath), fileName, NameCollisionOption.ReplaceExisting);
            }
            catch { }
#else
            try
            {
                FileInfo fi = new FileInfo(File.FilePath);
                fi.CopyTo(Path.Combine(AtSys.DocPath, fileName), true);
            }
            catch { }
#endif

            // 缩略图重命名
            if (!string.IsNullOrEmpty(File.ThumbPath))
            {
                string thumbName = GetThumbName();
#if UWP
                try
                {
                    var sf = await StorageFile.GetFileFromPathAsync(File.ThumbPath);
                    await sf.RenameAsync(thumbName);
                }
                catch { }
#else
                try
                {
                    FileInfo fi = new FileInfo(File.ThumbPath);
                    if (fi.Exists)
                    {
                        string newPath = Path.Combine(File.ThumbPath.Substring(0, File.ThumbPath.LastIndexOf('/')), thumbName);
                        fi.MoveTo(newPath);
                    }
                }
                catch { }
#endif
            }

            File = null;
        }

        /// <summary>
        /// 上传失败
        /// </summary>
        internal void UploadFail()
        {
            File.DeleteThumbnail();
            File = null;
        }
        #endregion

        #region 下载
        /// <summary>
        /// 执行下载
        /// </summary>
        /// <param name="p_isThumbnail">是否为缩略图</param>
        /// <param name="p_prompt">是否提示下载失败信息</param>
        /// <returns></returns>
        internal async Task<bool> Download(bool p_isThumbnail = false, bool p_prompt = true)
        {
            if (State != FileItemState.None || string.IsNullOrEmpty(ID))
            {
                if (p_prompt)
                    AtKit.Warn("当前状态不可下载！");
                return false;
            }

            string a = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string b = ApplicationData.Current.LocalFolder.Path;

            string name = p_isThumbnail ? GetThumbName() : GetFileName();
            string path = Path.Combine(AtSys.DocPath, name);
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
                Path = p_isThumbnail ? ID.Substring(0, ID.LastIndexOf('.')) + "-t.jpg" : ID,
                TgtStream = stream,
                Progress = OnDownloadProgress,
            };

            bool suc = false;
            _ctsDownload?.Dispose();
            _ctsDownload = new CancellationTokenSource();
            try
            {
                suc = await Downloader.Handle(info, _ctsDownload.Token);
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
            return suc;
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, State.ToString(), true);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            if (_pointerID != e.Pointer.PointerId)
                VisualStateManager.GoToState(this, "PointerOver", true);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _pointerID = e.Pointer.PointerId;
                VisualStateManager.GoToState(this, "Pressed", true);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_pointerID == e.Pointer.PointerId)
            {
                ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            _pointerID = null;
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch && this.ContainPoint(e.GetCurrentPoint(null).Position))
                VisualStateManager.GoToState(this, "PointerOver", true);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            if (_pointerID != e.Pointer.PointerId)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            base.OnHolding(e);
            if (e.HoldingState == HoldingState.Started)
                Owner.Current = this;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
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

        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            base.OnRightTapped(e);
            Owner.Current = this;
        }
        #endregion

        #region 内部方法
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            _loaded = true;

            if ((FileType == FileItemType.Image || FileType == FileItemType.Video)
                && !string.IsNullOrEmpty(ID)
                && Bitmap == null)
            {
                LoadImage();
            }
        }

#if UWP
        /// <summary>
        /// 加载图像
        /// </summary>
        async void LoadImage()
        {
            string thumbName = GetThumbName();
            StorageFile file = await AtSys.GetUwpDocFile(thumbName);
            if (file == null)
            {
                bool existThumbnail = ExistThumbnail();

                // 无缩略图时取原始图片
                if (!existThumbnail)
                    file = await AtSys.GetUwpDocFile(GetFileName());

                if (file == null)
                {
                    // 下载缩略图或原始图
                    if (await Download(existThumbnail, false))
                        file = await AtSys.GetUwpDocFile(existThumbnail ? thumbName : GetFileName());
                }
            }

            if (file != null)
            {
                using (var fs = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bmp = new BitmapImage();
                    await bmp.SetSourceAsync(fs);
                    Bitmap = bmp;
                }
            }
        }
#else
        async void LoadImage()
        {
            string thumbName = GetThumbName();
            FileInfo fi = new FileInfo(Path.Combine(AtSys.DocPath, thumbName));
            if (!fi.Exists)
            {
                bool existThumbnail = ExistThumbnail();

                // 无缩略图时取原始图片
                if (!existThumbnail)
                    fi = new FileInfo(Path.Combine(AtSys.DocPath, GetFileName()));

                if (!fi.Exists)
                {
                    // 下载缩略图或原始图
                    if (await Download(existThumbnail, false))
                        fi = new FileInfo(Path.Combine(AtSys.DocPath, existThumbnail ? thumbName : GetFileName()));
                }
            }

            if (fi.Exists)
            {
                using (var fs = fi.OpenRead())
                {
                    BitmapImage bmp = new BitmapImage();
                    await bmp.SetSourceAsync(fs);
                    Bitmap = bmp;
                }
            }
        }
#endif

        /// <summary>
        /// 分析原始图片是否有缩略图
        /// </summary>
        /// <returns></returns>
        bool ExistThumbnail()
        {
            bool exist = true;

            if (FileType == FileItemType.Image && !string.IsNullOrEmpty(FileDesc))
            {
                string[] size;
                int index = FileDesc.IndexOf('(');
                if (index > 0
                    && (size = FileDesc.Substring(0, index).Split('x')).Length == 2
                    && int.TryParse(size[0].Trim(), out int width)
                    && int.TryParse(size[1].Trim(), out int height)
                    && width < FileData.ThumbSize
                    && height < FileData.ThumbSize)
                {
                    exist = false;
                }
            }
            return exist;
        }

        /// <summary>
        /// 播放声音或视频
        /// </summary>
        /// <param name="p_file">待播放文件</param>
        async void Play(StorageFile p_file)
        {
            if (_playerHost == this || p_file == null)
                return;

            if (_player == null)
            {
                // 初始化播放器
                _player = new MediaElement();
                _player.AutoPlay = true;
                _player.MediaEnded += (sender, e) => StopPlayer();
                _player.MediaFailed += (sender, e) => StopPlayer();

                // 自定义播放器内容
                _player.AreTransportControlsEnabled = true;
                var con = _player.TransportControls;
                con.IsCompact = true;
                con.IsVolumeButtonVisible = false;
                con.IsVolumeEnabled = false;
                con.IsZoomButtonVisible = false;
                con.IsZoomEnabled = false;
            }
            else
            {
                StopPlayer();
            }

            _player.Height = ActualHeight;
            _player.TransportControls.IsFullWindowButtonVisible = (FileType == FileItemType.Video);
            var stream = await p_file.OpenAsync(FileAccessMode.Read);
            _player.SetSource(stream, p_file.FileType);
            _playerHost = this;
            MediaPlayer = _player;
        }

        /// <summary>
        /// 更新图标，缺图标！
        /// </summary>
        void UpdateIcon()
        {
            Icon = AtRes.GetIcon(Icons.文件, 30, AtRes.主题蓝色);

            //string ext = Ext;
            //if (string.IsNullOrEmpty(ext) || ext == ".")
            //{
            //    Icon = AtRes.GetIcon(Icons.文件, 30, AtRes.主题蓝色);
            //    return;
            //}

            //switch (ext)
            //{
            //    case ".doc":
            //    case ".docx":
            //        Icon = AtRes.GetIcon(Icons.Word, 30, AtRes.主题蓝色);
            //        break;
            //    case ".xls":
            //    case ".xlsx":
            //        Icon = AtRes.GetIcon(Icons.Excel, 30, AtRes.主题蓝色);
            //        break;
            //    case ".zip":
            //        Icon = AtRes.GetIcon(Icons.Zip, 30, AtRes.主题蓝色);
            //        break;
            //    case ".rar":
            //        Icon = AtRes.GetIcon(Icons.Folder, 30, AtRes.主题蓝色);
            //        break;
            //    case ".txt":
            //        Icon = AtRes.GetIcon(Icons.OpenFile, 30, AtRes.主题蓝色);
            //        break;
            //    case ".ppt":
            //    case ".pptx":
            //        Icon = AtRes.GetIcon(Icons.PowerPoint, 30, AtRes.主题蓝色);
            //        break;
            //    case ".htm":
            //    case ".html":
            //        Icon = AtRes.GetIcon(Icons.Explorer, 30, AtRes.主题蓝色);
            //        break;
            //    case ".exe":
            //        Icon = AtRes.GetIcon(Icons.Task, 30, AtRes.主题蓝色);
            //        break;
            //    default:
            //        Icon = AtRes.GetIcon(Icons.File, 30, AtRes.主题蓝色);
            //        break;
            //}
        }

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
        /// 关闭卸载播放器
        /// </summary>
        static void StopPlayer()
        {
            if (_playerHost != null)
            {
                _player.Stop();
                _playerHost.MediaPlayer = null;
                _playerHost = null;
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
            FileDesc = p_reader.ReadAsString();
            if (ulong.TryParse(p_reader.ReadAsString(), out ulong length))
                Length = length;
            Uploader = p_reader.ReadAsString();
            Date = p_reader.ReadAsString();

            // FileItem ]
            p_reader.Read();

            // 文件类型
            Ext = ID.Substring(ID.LastIndexOf('.'));
            FileItemType tp = FileItemType.File;
            if (FileFilter.UwpImage.Contains(Ext))
                tp = FileItemType.Image;
            else if (FileFilter.UwpAudio.Contains(Ext))
                tp = FileItemType.Sound;
            else if (FileFilter.UwpVideo.Contains(Ext))
                tp = FileItemType.Video;
            else
                UpdateIcon();

            // 切换模板
            if (tp != FileItemType.File)
                FileType = tp;

            // 未读标志
            if (tp != FileItemType.Image && Uploader != AtUser.Name)
            {
                int cnt = AtLocal.GetScalar<int>($"select count(*) from FileReadLog where FileID='{ID}' and UserName='{AtUser.Name}'");
                if (cnt == 0)
                    Unread = true;
            }
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
            p_writer.WriteStringValue(FileDesc);
            p_writer.WriteNumberValue(Length);
            p_writer.WriteStringValue(Uploader);
            p_writer.WriteStringValue(Date);
            p_writer.WriteEndArray();
        }
        #endregion

        #region 上传下载进度
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
    }
}