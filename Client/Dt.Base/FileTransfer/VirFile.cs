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
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.Storage.FileProperties;
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
    public partial class VirFile : Control, IUploadFile
    {
        #region 静态成员
        /// <summary>
        /// 文件名称，不包括扩展名
        /// </summary>
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName",
            typeof(string),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 文件种类
        /// </summary>
        public static readonly DependencyProperty FileTypeProperty = DependencyProperty.Register(
            "FileType",
            typeof(VirFileType),
            typeof(VirFile),
            new PropertyMetadata(VirFileType.File, OnFileTypePropertyChanged));

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public static readonly DependencyProperty ExtProperty = DependencyProperty.Register(
            "Ext",
            typeof(string),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 未读标志
        /// </summary>
        public static readonly DependencyProperty UnreadProperty = DependencyProperty.Register(
            "Unread",
            typeof(bool),
            typeof(VirFile),
            new PropertyMetadata(false));

        /// <summary>
        /// 文件类型及说明
        /// </summary>
        public static readonly DependencyProperty FileDescProperty = DependencyProperty.Register(
            "FileDesc",
            typeof(string),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 文件大小
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length",
            typeof(string),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 上传文件用户
        /// </summary>
        public static readonly DependencyProperty UploaderProperty = DependencyProperty.Register(
            "Uploader",
            typeof(string),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 文件上传日期
        /// </summary>
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            "Date",
            typeof(string),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 图标
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(object),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 当前状态
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State",
            typeof(VirFileState),
            typeof(VirFile),
            new PropertyMetadata(VirFileState.None, OnStatePropertyChanged));

        /// <summary>
        /// 进度条进度
        /// </summary>
        public static readonly DependencyProperty ProgressWidthProperty = DependencyProperty.Register(
            "ProgressWidth",
            typeof(double),
            typeof(VirFile),
            new PropertyMetadata(0));

        /// <summary>
        /// 进度百分比
        /// </summary>
        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
            "Percent",
            typeof(string),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 源图像
        /// </summary>
        public static readonly DependencyProperty BitmapProperty = DependencyProperty.Register(
            "Bitmap",
            typeof(ImageSource),
            typeof(VirFile),
            new PropertyMetadata(null));

        /// <summary>
        /// 播放器
        /// </summary>
        public static readonly DependencyProperty MediaPlayerProperty = DependencyProperty.Register(
            "MediaPlayer",
            typeof(object),
            typeof(VirFile),
            new PropertyMetadata(null));

        static void OnFileTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirFile vf = (VirFile)d;
            VirFileType tp = (VirFileType)e.NewValue;
            if (tp == VirFileType.Image)
                vf.Template = AtRes.VirImageTemplate;
            else if (tp == VirFileType.Sound)
                vf.Template = AtRes.VirSoundTemplate;
            else if (tp == VirFileType.Video)
                vf.Template = AtRes.VirVideoTemplate;
            else
                vf.Template = AtRes.VirFileTemplate;
        }

        static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirFile vf = (VirFile)d;
            if (vf._loaded)
            {
                var state = (VirFileState)e.NewValue;
                VisualStateManager.GoToState(vf, state.ToString(), true);
                vf.ProgressWidth = 0.0;
            }
        }
        #endregion

        #region 成员变量
        static MediaElement _player;
        static VirFile _playerHost;

        uint? _pointerID;
        bool _loaded;
        FileTransfer _owner;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public VirFile()
        {
            DefaultStyleKey = typeof(VirFile);
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
        public VirFileType FileType
        {
            get { return (VirFileType)GetValue(FileTypeProperty); }
            set { SetValue(FileTypeProperty, value); }
        }

        /// <summary>
        /// 获取设置文件扩展名
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
        public string Length
        {
            get { return (string)GetValue(LengthProperty); }
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
        public VirFileState State
        {
            get { return (VirFileState)GetValue(StateProperty); }
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
        /// 获取设置所属FileTransfer
        /// </summary>
        public FileTransfer Owner
        {
            get { return _owner; }
        }

        /// <summary>
        /// 获取保存时的文件说明
        /// </summary>
        public string SaveDesc
        {
            get
            {
                switch (FileType)
                {
                    case VirFileType.Image:
                        return "图像";
                    case VirFileType.Sound:
                        return "音频";
                    case VirFileType.Video:
                        return "视频";
                }
                return FileDesc;
            }
        }
        #endregion

        #region IUploadFile
        /// <summary>
        /// 获取设置待上传的文件
        /// </summary>
        public StorageFile File { get; set; }

        /// <summary>
        /// 获取上传进度处理对象
        /// </summary>
        public ProgressDelegate UploadProgress
        {
            get { return OnUploadProgress; }
        }
        #endregion

        #region 上传
        /// <summary>
        /// 准备上传，初始化文件属性
        /// </summary>
        /// <param name="p_file"></param>
        /// <param name="p_length"></param>
        /// <param name="p_date"></param>
        /// <returns></returns>
        public async Task InitUpload(StorageFile p_file, string p_length, string p_date)
        {
            State = VirFileState.UploadWaiting;
            File = p_file;

            // 基础属性
            FileName = p_file.DisplayName;
            var ext = p_file.FileType.ToLower();
            Ext = ext;
            Length = p_length;
            Uploader = AtUser.Name;
            Date = p_date;

            // 类型及文件描述
            if (AtKit.ImageFormat.Contains(ext))
            {
                FileType = VirFileType.Image;
                var prop = await p_file.Properties.GetImagePropertiesAsync();
                FileDesc = string.Format("{0} x {1} ({2})", prop.Width, prop.Height, ext);

                using (var fs = await p_file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bmp = new BitmapImage();
                    if (prop.Width > prop.Height)
                    {
                        if (prop.Width > 256)
                            bmp.DecodePixelWidth = 256;
                    }
                    else if (prop.Height > 256)
                    {
                        bmp.DecodePixelHeight = 256;
                    }
                    await bmp.SetSourceAsync(fs);
                    Bitmap = bmp;
                }
            }
            else if (AtKit.SoundFormat.Contains(ext))
            {
                FileType = VirFileType.Sound;
                var prop = await p_file.Properties.GetMusicPropertiesAsync();
                FileDesc = string.Format("{0:mm:ss}", new DateTime(prop.Duration.Ticks));
            }
            else if (AtKit.VideoFormat.Contains(ext))
            {
                FileType = VirFileType.Video;
                var prop = await p_file.Properties.GetVideoPropertiesAsync();
                FileDesc = string.Format("{0:HH:mm:ss} ({1} x {2})", new DateTime(prop.Duration.Ticks), prop.Width, prop.Height);

                // 生成缩略图
            }
            else
            {
                FileDesc = string.Format("{0} ({1})", p_file.DisplayType, ext);
                UpdateIcon();
            }
        }

        /// <summary>
        /// 上传成功后
        /// </summary>
        /// <param name="p_id">文件上传路径</param>
        /// <returns></returns>
        public async Task UploadSuccess(string p_id)
        {
            // 更新时删除旧文件
            if (!string.IsNullOrEmpty(ID))
            {
                // 删除服务器端旧文件
                await AtFile.Delete(ID);
                // 删除本地旧文件
                AtKit.DeleteTempFile(ID.Substring(ID.LastIndexOf('/') + 1));
            }

            // 复制到临时文件夹，免去再次下载
            ID = p_id;
            try
            {
                await File.CopyAsync(ApplicationData.Current.TemporaryFolder, ID.Substring(ID.LastIndexOf('/') + 1), NameCollisionOption.ReplaceExisting);
            }
            catch { }
            File = null;
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 执行下载
        /// </summary>
        /// <param name="p_small">是否为缩略图</param>
        /// <param name="p_prompt">是否提示下载失败信息</param>
        /// <returns></returns>
        public async Task<bool> Download(bool p_small = false, bool p_prompt = true)
        {
            string name = GetIDName();
            if (string.IsNullOrEmpty(name))
            {
                if (p_prompt)
                    AtKit.Warn("当前状态不可下载！");
                return false;
            }

            StorageFile file;
            try
            {
                file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            }
            catch (Exception ex)
            {
                if (p_prompt)
                    AtKit.Warn(string.Format("下载前创建文件时出错：\r\n{0}", ex.Message));
                return false;
            }

            State = VirFileState.Downloading;
            DownloadInfo info = new DownloadInfo
            {
                Path = (p_small && AtKit.ImageFormat.Contains(Ext)) ? $"{ID}/256" : ID,
                TgtFile = file,
                Progress = OnDownloadProgress,
            };
            bool suc = await Downloader.Handle(info, Owner.Cts.Token);
            State = VirFileState.None;

            if (!suc)
            {
                if (p_prompt)
                    AtKit.Warn(info.Error);

                // 未成功，删除缓存文件，避免打开时出错
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            return suc;
        }

        /// <summary>
        /// 打开文件
        /// <para>先检查本地有没有，有打开本地，没有先下载；</para>
        /// <para>内部支持图片和音视频的预览，其它类型文件用默认关联程序打开；</para>
        /// </summary>
        public async void Open()
        {
            string name = GetIDName();
            if (string.IsNullOrEmpty(name))
            {
                AtKit.Warn("当前状态不允许打开文件！");
                return;
            }

            // 打开本地文件
            StorageFile file = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name) as StorageFile;
            if (file == null)
            {
                // 先下载再打开
                bool suc = await Download();
                if (suc)
                    file = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name) as StorageFile;
            }
            if (file == null)
                return;

            //var tp = FileType;
            //if (tp == VirFileType.Sound || tp == VirFileType.Video)
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
            string name = GetIDName();
            if (string.IsNullOrEmpty(name))
            {
                AtKit.Warn("当前状态不允许文件另存！");
                return;
            }

            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add(SaveDesc, new List<string>() { Ext });
            picker.SuggestedFileName = FileName;
            StorageFile file = await picker.PickSaveFileAsync();
            if (file == null)
                return;

            // 复制本地文件
            StorageFile temp = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name) as StorageFile;
            if (temp != null)
            {
                await temp.CopyAndReplaceAsync(file);
                AtKit.Msg("文件另存成功！");
                return;
            }

            // 先下载再另存
            if (await Download())
            {
                temp = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name) as StorageFile;
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
            if (State != VirFileState.None || string.IsNullOrEmpty(ID))
            {
                AtKit.Warn("当前状态不允许删除文件！");
                return false;
            }

            bool suc = await AtFile.Delete(ID);
            if (suc)
                _owner.RemoveChild(this);
            else
                AtKit.Warn("删除失败！");
            return suc;
        }

        /// <summary>
        /// 设置所属FileTransfer
        /// </summary>
        /// <param name="p_owner"></param>
        internal void SetOwner(FileTransfer p_owner)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 获取路径中的文件名
        /// </summary>
        /// <returns></returns>
        string GetIDName()
        {
            int idx;
            if (State != VirFileState.None
                || string.IsNullOrEmpty(ID)
                || (idx = ID.LastIndexOf('/')) == -1)
                return null;
            return ID.Substring(idx + 1);
        }
        #endregion

        #region Xml
        /// <summary>
        /// 反序列化，初次加载
        /// </summary>
        /// <param name="p_reader"></param>
        /// <returns></returns>
        internal void ReadXml(XmlReader p_reader)
        {
            ID = p_reader.GetAttribute("ID");
            FileName = p_reader.GetAttribute("Name");
            Ext = p_reader.GetAttribute("Ext");
            FileDesc = p_reader.GetAttribute("Desc");
            Length = p_reader.GetAttribute("Length");
            Uploader = p_reader.GetAttribute("Uploader");
            Date = p_reader.GetAttribute("Date");

            VirFileType tp = VirFileType.File;
            if (AtKit.ImageFormat.Contains(Ext))
                tp = VirFileType.Image;
            else if (AtKit.SoundFormat.Contains(Ext))
                tp = VirFileType.Sound;
            else if (AtKit.VideoFormat.Contains(Ext))
                tp = VirFileType.Video;
            else
                UpdateIcon();

            // 切换模板
            if (tp != VirFileType.File)
                FileType = tp;

            // 缩略图固定大小
            if (tp == VirFileType.Image || tp == VirFileType.Video)
            {
                double size;
                string temp = p_reader.GetAttribute("TW");
                if (!string.IsNullOrEmpty(temp) && double.TryParse(temp, out size))
                    Width = size;

                temp = p_reader.GetAttribute("TH");
                if (!string.IsNullOrEmpty(temp) && double.TryParse(temp, out size))
                    Height = size;
            }

            // 未读标志
            if (tp != VirFileType.Image && Uploader != AtUser.Name)
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
        internal void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("F");
            p_writer.WriteAttributeString("ID", ID);
            p_writer.WriteAttributeString("Name", FileName);
            p_writer.WriteAttributeString("Ext", Ext);
            p_writer.WriteAttributeString("Desc", FileDesc);
            p_writer.WriteAttributeString("Length", Length);
            p_writer.WriteAttributeString("Uploader", Uploader);
            p_writer.WriteAttributeString("Date", Date);

            // 记录缩略图大小
            VirFileType tp = FileType;
            if (tp == VirFileType.Image || tp == VirFileType.Video)
            {
                if (!double.IsNaN(Width))
                    p_writer.WriteAttributeString("TW", Width.ToString());
                if (!double.IsNaN(Height))
                    p_writer.WriteAttributeString("TH", Height.ToString());
            }
            p_writer.WriteEndElement();
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            VisualStateManager.GoToState(this, State.ToString(), true);
        }

        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            if (_pointerID != e.Pointer.PointerId)
                VisualStateManager.GoToState(this, "PointerOver", true);
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// 抬起
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_pointerID == e.Pointer.PointerId)
            {
                ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }

        /// <summary>
        /// 失去捕获
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            _pointerID = null;
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch && this.ContainPoint(e.GetCurrentPoint(null).Position))
                VisualStateManager.GoToState(this, "PointerOver", true);
        }

        /// <summary>
        /// 鼠标移出
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            if (_pointerID != e.Pointer.PointerId)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        /// <summary>
        /// 长按
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            base.OnHolding(e);
            if (e.HoldingState == HoldingState.Started)
                _owner.Current = this;
        }

        /// <summary>
        /// 执行打开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            switch (State)
            {
                case VirFileState.None:
                    Open();
                    break;
                case VirFileState.UploadWaiting:
                    _owner.RemoveChild(this);
                    break;
                case VirFileState.Uploading:
                case VirFileState.Downloading:
                    _owner.CancelTransfer();
                    break;
            }
        }

        /// <summary>
        /// 右键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            base.OnRightTapped(e);
            _owner.Current = this;
        }
        #endregion

        #region 内部方法
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            _loaded = true;
            var ft = FileType;
            if (ft == VirFileType.Image || ft == VirFileType.Video)
                LoadImage();
        }

        /// <summary>
        /// 加载图像
        /// </summary>
        async void LoadImage()
        {
            // 缩略图
            //string name = ThumbnailName;
            //bool suc = true;
            //StorageFile file = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name) as StorageFile;
            //if (file == null)
            //{
            //    // 下载图像
            //    suc = await Download(true, false);
            //    if (suc)
            //        file = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(name) as StorageFile;
            //}

            //if (suc)
            //{
            //    using (var fs = await file.OpenAsync(FileAccessMode.Read))
            //    {
            //        BitmapImage bmp = new BitmapImage();
            //        await bmp.SetSourceAsync(fs);
            //        Bitmap = bmp;
            //    }
            //}
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
            _player.TransportControls.IsFullWindowButtonVisible = (FileType == VirFileType.Video);
            var stream = await p_file.OpenAsync(FileAccessMode.Read);
            _player.SetSource(stream, p_file.FileType);
            _playerHost = this;
            MediaPlayer = _player;
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
                    State = VirFileState.Uploading;

                double percent = p_bytesSent * 100 / p_totalBytesToSend;
                Percent = string.Format("{0}%", percent);
                ProgressWidth = Math.Ceiling(percent / 100 * ActualWidth);

                // 完成
                if (p_bytesStep == 0 && p_bytesSent == p_totalBytesToSend)
                {
                    State = VirFileState.None;
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
                    State = VirFileState.None;
                    ClearValue(PercentProperty);
                }
            }));
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
    }
}