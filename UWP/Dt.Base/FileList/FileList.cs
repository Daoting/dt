#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 文件上传下载编辑器
    /// </summary>
    public partial class FileList : Control, IMenuHost
    {
        #region 静态成员
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(string),
            typeof(FileList),
            new PropertyMetadata(null, OnDataPropertyChanged));

        public static readonly DependencyProperty MaxFileCountProperty = DependencyProperty.Register(
            "MaxFileCount",
            typeof(int),
            typeof(FileList),
            new PropertyMetadata(int.MaxValue));

        public static readonly DependencyProperty FixedVolumeProperty = DependencyProperty.Register(
            "FixedVolume",
            typeof(string),
            typeof(FileList),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ColCountProperty = DependencyProperty.Register(
            "ColCount",
            typeof(int),
            typeof(FileList),
            new PropertyMetadata(1, OnRefreshPanel));

        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
            "Spacing",
            typeof(double),
            typeof(FileList),
            new PropertyMetadata(0d, OnRefreshPanel));

        public static readonly DependencyProperty ImageStretchProperty = DependencyProperty.Register(
            "ImageStretch",
            typeof(Stretch),
            typeof(FileList),
            new PropertyMetadata(Stretch.Uniform));

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(
            "ImageHeight",
            typeof(double),
            typeof(FileList),
            new PropertyMetadata(82d, OnRefreshPanel));

        public static readonly DependencyProperty EnableClickProperty = DependencyProperty.Register(
            "EnableClick",
            typeof(bool),
            typeof(FileList),
            new PropertyMetadata(true));

        static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileList ft = (FileList)d;
            if (!ft._lockData && ft._pnl != null)
                ft.ReadData((string)e.NewValue);
        }

        static void OnRefreshPanel(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileList ft = (FileList)d;
            if (ft._pnl != null)
                ft._pnl.InvalidateMeasure();
        }
        #endregion

        #region 成员变量
        FileListPanel _pnl;
        bool _lockData;
        CancellationTokenSource _cts;

        BaseCommand _cmdAddImage;
        BaseCommand _cmdAddVideo;
        BaseCommand _cmdAddAudio;
        BaseCommand _cmdAddFile;
        BaseCommand _cmdCaptureVoice;
        BaseCommand _cmdTakePhoto;
        BaseCommand _cmdRecordVideo;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public FileList()
        {
            DefaultStyleKey = typeof(FileList);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 开始上传事件
        /// </summary>
        public event EventHandler UploadStarted;

        /// <summary>
        /// 上传结束事件
        /// </summary>
        public event EventHandler<bool> UploadFinished;

        /// <summary>
        /// 文件列表变化事件
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// 文件打开后事件
        /// </summary>
        public event EventHandler<FileItem> OpenedFile;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置文件列表的json描述信息
        /// </summary>
        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 获取设置文件数量上限，默认int.MaxValue
        /// </summary>
        public int MaxFileCount
        {
            get { return (int)GetValue(MaxFileCountProperty); }
            set { SetValue(MaxFileCountProperty, value); }
        }

        /// <summary>
        /// 获取设置要上传的固定卷名，默认null表示上传到普通卷
        /// </summary>
        public string FixedVolume
        {
            get { return (string)GetValue(FixedVolumeProperty); }
            set { SetValue(FixedVolumeProperty, value); }
        }

        /// <summary>
        /// 获取设置列数，默认1列
        /// </summary>
        public int ColCount
        {
            get { return (int)GetValue(ColCountProperty); }
            set { SetValue(ColCountProperty, value); }
        }

        /// <summary>
        /// 获取设置文件项之间的间隔距离，默认0
        /// </summary>
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// 获取设置图像的显示高度，默认82，0表示和宽度相同
        /// </summary>
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        /// <summary>
        ///  获取设置图像填充模式，默认Uniform
        /// </summary>
        public Stretch ImageStretch
        {
            get { return (Stretch)GetValue(ImageStretchProperty); }
            set { SetValue(ImageStretchProperty, value); }
        }

        /// <summary>
        /// 获取设置文件项是否可点击，默认true
        /// </summary>
        public bool EnableClick
        {
            get { return (bool)GetValue(EnableClickProperty); }
            set { SetValue(EnableClickProperty, value); }
        }

        /// <summary>
        /// 获取所有FileItem
        /// </summary>
        public IEnumerable<FileItem> Items
        {
            get
            {
                return from obj in _pnl.Children
                       let vf = obj as FileItem
                       where vf != null
                       select vf;
            }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 获取添加图片命令
        /// </summary>
        public BaseCommand CmdAddImage
        {
            get
            {
                if (_cmdAddImage == null)
                    _cmdAddImage = new BaseCommand((e) => AddImage());
                return _cmdAddImage;
            }
        }

        /// <summary>
        /// 获取添加视频命令
        /// </summary>
        public BaseCommand CmdAddVideo
        {
            get
            {
                if (_cmdAddVideo == null)
                    _cmdAddVideo = new BaseCommand((e) => AddVideo());
                return _cmdAddVideo;
            }
        }

        /// <summary>
        /// 获取添加音频命令
        /// </summary>
        public BaseCommand CmdAddAudio
        {
            get
            {
                if (_cmdAddAudio == null)
                    _cmdAddAudio = new BaseCommand((e) => AddAudio());
                return _cmdAddAudio;
            }
        }

        /// <summary>
        /// 获取添加文件命令
        /// </summary>
        public BaseCommand CmdAddFile
        {
            get
            {
                if (_cmdAddFile == null)
                    _cmdAddFile = new BaseCommand((e) => AddFile());
                return _cmdAddFile;
            }
        }

        /// <summary>
        /// 获取添加录音命令
        /// </summary>
        public BaseCommand CmdCaptureVoice
        {
            get
            {
                if (_cmdCaptureVoice == null)
                    _cmdCaptureVoice = new BaseCommand((e) => CaptureVoice());
                return _cmdCaptureVoice;
            }
        }

        /// <summary>
        /// 获取拍照命令
        /// </summary>
        public BaseCommand CmdTakePhoto
        {
            get
            {
                if (_cmdTakePhoto == null)
                    _cmdTakePhoto = new BaseCommand((e) => TakePhoto());
                return _cmdTakePhoto;
            }
        }

        /// <summary>
        /// 获取录视频命令
        /// </summary>
        public BaseCommand CmdRecordVideo
        {
            get
            {
                if (_cmdRecordVideo == null)
                    _cmdRecordVideo = new BaseCommand((e) => RecordVideo());
                return _cmdRecordVideo;
            }
        }
        #endregion

        #region 上传
        /// <summary>
        /// 增加图片文件
        /// </summary>
        public void AddImage()
        {
            _ = AppendFile(() => Kit.PickImages(), () => Kit.PickImage());
        }

        /// <summary>
        /// 增加视频文件
        /// </summary>
        public void AddVideo()
        {
            _ = AppendFile(() => Kit.PickVideos(), () => Kit.PickVideo());
        }

        /// <summary>
        /// 增加音频文件
        /// </summary>
        public void AddAudio()
        {
            _ = AppendFile(() => Kit.PickAudios(), () => Kit.PickAudio());
        }

        /// <summary>
        /// 增加媒体文件
        /// </summary>
        public void AddMedia()
        {
            _ = AppendFile(() => Kit.PickMedias(), () => Kit.PickMedia());
        }

        /// <summary>
        /// 增加文件
        /// </summary>
        /// <param name="p_uwpFileTypes">uwp文件过滤类型，如 .png .docx，null时不过滤</param>
        /// <param name="p_androidFileTypes">android文件过滤类型，如 image/png image/*，null时不过滤</param>
        /// <param name="p_iosFileTypes">ios文件过滤类型，如 UTType.Image，null时不过滤</param>
        public void AddFile(string[] p_uwpFileTypes = null, string[] p_androidFileTypes = null, string[] p_iosFileTypes = null)
        {
            _ = AppendFile(

#if UWP
                () => Kit.PickFiles(p_uwpFileTypes),
#elif ANDROID
                () => Kit.PickFiles(p_androidFileTypes),
#elif IOS
                () => Kit.PickFiles(p_iosFileTypes),
#elif WASM
                () => Task.FromResult((List<FileData>)null),
#endif

#if UWP
                () => Kit.PickFile(p_uwpFileTypes));
#elif ANDROID
                () => Kit.PickFile(p_androidFileTypes));
#elif IOS
                () => Kit.PickFile(p_iosFileTypes));
#elif WASM
                () => Task.FromResult((FileData)null));
#endif
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="p_files"></param>
        public async Task<bool> UploadFiles(IList<FileData> p_files)
        {
            if (p_files == null
                || p_files.Count == 0
                || p_files.Contains(null))
                return false;

            if (p_files.Count + _pnl.Children.Count > MaxFileCount)
            {
                Kit.Warn($"最多可上传 {MaxFileCount} 个文件！");
                return false;
            }

            var overlength = (from f in p_files
                              where f.Size > Kit.GB
                              select f).Any();
            if (overlength)
            {
                Kit.Warn("上传文件超过1GB限制！");
                return false;
            }

            foreach (var file in p_files)
            {
                FileItem vf = new FileItem(this);
                file.UploadUI = vf;
                await ((IUploadUI)file.UploadUI).InitUpload(file);
                _pnl.Children.Add(vf);
            }

            return await HandleUpload(p_files);
        }

        /// <summary>
        /// 更新已上传的文件
        /// </summary>
        /// <param name="p_file">新文件</param>
        /// <param name="p_vf">待更新的旧文件</param>
        /// <returns></returns>
        public async Task<bool> UpdateFile(FileData p_file, FileItem p_vf)
        {
            if (p_file == null || p_vf == null)
                return false;

            if (p_file.Size > Kit.GB)
            {
                Kit.Warn(string.Format("【{0}】\r\n文件超过1GB限制！", p_file.DisplayName));
                return false;
            }

            // 新文件属性
            p_file.UploadUI = p_vf;
            await ((IUploadUI)p_file.UploadUI).InitUpload(p_file);
            return await HandleUpload(new List<FileData> { p_file });
        }

        /// <summary>
        /// 处理多文件上传
        /// </summary>
        /// <param name="p_files"></param>
        async Task<bool> HandleUpload(IList<FileData> p_files)
        {
            UploadStarted?.Invoke(this, EventArgs.Empty);

            List<string> result = null;
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            try
            {
                result = await Uploader.Send(p_files, FixedVolume, _cts);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "上传出错");
            }
            finally
            {
                _cts?.Dispose();
            }

            bool suc = false;
            if (result == null || result.Count != p_files.Count)
            {
                // 失败
                if (_cts == null)
                    Kit.Msg("已取消上传！");
                else
                    Kit.Warn("😢上传失败，请重新上传！");

                foreach (var vf in p_files)
                {
                    ((IUploadUI)vf.UploadUI).UploadFail(vf);
                }
                // 加载旧列表
                ReadData(Data);
            }
            else
            {
                suc = true;
                for (int i = 0; i < p_files.Count; i++)
                {
                    var vf = p_files[i];
                    await ((IUploadUI)vf.UploadUI).UploadSuccess(result[i], vf);
                }
                WriteData();
            }
            UploadFinished?.Invoke(this, suc);
            return suc;
        }

        async Task AppendFile(Func<Task<List<FileData>>> p_funMulti, Func<Task<FileData>> p_funSingle)
        {
            if (MaxFileCount > 1)
            {
                var files = await p_funMulti();
                if (files != null && files.Count > 0)
                    await UploadFiles(files);
            }
            else
            {
                var file = await p_funSingle();
                if (file != null)
                {
                    // 若已有文件则为更新
                    if (_pnl.Children.Count > 0)
                        await UpdateFile(file, (FileItem)_pnl.Children[0]);
                    else
                        await UploadFiles(new List<FileData>() { file });
                }
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 通过从库中选择文件添加，无上传过程，只更新文件信息
        /// 每个字符串为独立的文件描述json，如：["v0/52/37/142888904373956608.xlsx","12","xlsx文件",8153,"daoting","2020-10-29 15:09"]
        /// </summary>
        /// <param name="p_filesJson"></param>
        public void AddExistFiles(List<string> p_filesJson)
        {
            if (p_filesJson == null || p_filesJson.Count == 0)
                return;

            if (MaxFileCount == 1)
            {
                if (p_filesJson.Count > 1)
                {
                    Kit.Warn($"最多可上传 {MaxFileCount} 个文件！");
                    return;
                }

                Data = "[" + p_filesJson[0] + "]";
                return;
            }

            if (p_filesJson.Count + _pnl.Children.Count > MaxFileCount)
            {
                Kit.Warn($"最多可上传 {MaxFileCount} 个文件！");
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var file in p_filesJson)
            {
                if (string.IsNullOrEmpty(file)
                    || !file.StartsWith("[")
                    || !file.EndsWith("]"))
                {
                    Kit.Warn($"文件描述json错误！");
                    return;
                }

                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(file);
            }

            string data = Data;
            if (string.IsNullOrEmpty(data) || data == "[]")
                Data = "[" + sb.ToString() + "]";
            else
                Data = data.Insert(data.Length - 1, "," + sb.ToString());
        }

        /// <summary>
        /// 增加录音
        /// </summary>
        public async void CaptureVoice()
        {
            var fd = await Kit.TakeAudio(this);
            if (fd != null)
                await UploadFiles(new List<FileData> { fd });
        }

        /// <summary>
        /// 增加拍照
        /// </summary>
        public async void TakePhoto()
        {
            var fd = await Kit.TakePhoto();
            if (fd != null)
                await UploadFiles(new List<FileData> { fd });
        }

        /// <summary>
        /// 增加录视频
        /// </summary>
        public async void RecordVideo()
        {
            var fd = await Kit.TakeVideo();
            if (fd != null)
                await UploadFiles(new List<FileData> { fd });
        }

        /// <summary>
        /// 取消上传或下载
        /// </summary>
        internal void CancelTransfer()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        /// <summary>
        /// 移除子项
        /// </summary>
        /// <param name="p_vf"></param>
        internal void AfterDeleteItem(FileItem p_vf)
        {
            _pnl.ChildrenTransitions = Res.AddDeleteTransition;
            _pnl.Children.Remove(p_vf);
            _pnl.ChildrenTransitions = null;

            WriteData();
        }

        /// <summary>
        /// 通过从库中选择文件进行更新，无需上传
        /// </summary>
        /// <param name="p_fileJson"></param>
        /// <param name="p_vf"></param>
        internal void UpdateExistFiles(string p_fileJson, FileItem p_vf)
        {
            int index = _pnl.Children.IndexOf(p_vf);
            _pnl.Children.RemoveAt(index);

            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(p_fileJson));
            reader.Read();
            FileItem vf = new FileItem(this);
            vf.ReadData(ref reader);
            _pnl.Children.Insert(index, vf);

            WriteData();
        }
        #endregion

        #region 加载过程
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _pnl = (FileListPanel)GetTemplateChild("Panel");
            _pnl.Owner = this;

            // 初次加载
            if (!string.IsNullOrEmpty(Data))
                ReadData(Data);
        }
        #endregion

        #region IMenuHost
        /// <summary>
        /// 切换上下文菜单或修改触发事件种类时通知宿主刷新
        /// </summary>
        void IMenuHost.UpdateContextMenu()
        {
            if (_pnl != null
                && _pnl.Children.Count > 0
                && ((FileItem)_pnl.Children[0]).IsLoaded)
            {
                // 重新加载所有项
                ReadData(Data);
            }
        }
        #endregion

        #region Json
        /// <summary>
        /// 反序列化，初次加载或重新加载
        /// </summary>
        /// <param name="p_json"></param>
        /// <returns></returns>
        void ReadData(string p_json)
        {
            if (_pnl == null)
                return;

            _pnl.Children.Clear();
            if (string.IsNullOrEmpty(p_json))
                return;

            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(p_json));
            // 最外层 [
            reader.Read();

            // FileItem [
            while (reader.Read())
            {
                // 最外层 ]
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                FileItem vf = new FileItem(this);
                vf.ReadData(ref reader);
                _pnl.Children.Add(vf);
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        void WriteData()
        {
            if (_pnl.Children.Count == 0)
            {
                _lockData = true;
                Data = null;
                _lockData = false;
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
                    {
                        writer.WriteStartArray();
                        foreach (var obj in _pnl.Children)
                        {
                            if (obj is FileItem vf)
                                vf.WriteData(writer);
                        }
                        writer.WriteEndArray();
                    }
                    _lockData = true;
                    Data = Encoding.UTF8.GetString(stream.ToArray());
                    _lockData = false;
                }
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region 触发事件
        internal void OnOpenedFile(FileItem p_item)
        {
            OpenedFile?.Invoke(this, p_item);
        }
        #endregion
    }
}
