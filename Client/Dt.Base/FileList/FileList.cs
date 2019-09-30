#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Transfer;
using Dt.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 文件上传下载编辑器
    /// </summary>
    public partial class FileList : Control
    {
        #region 静态成员
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(string),
            typeof(FileList),
            new PropertyMetadata(null, OnDataPropertyChanged));

        public static readonly DependencyProperty AllowMultipleProperty = DependencyProperty.Register(
            "AllowMultiple",
            typeof(bool),
            typeof(FileList),
            new PropertyMetadata(true));

        static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileList ft = (FileList)d;
            if (!ft._lockData)
                ft.ReadData((string)e.NewValue);
        }
        #endregion

        #region 成员变量
        readonly StackPanel _pnl;
        bool _lockData;
        CancellationTokenSource _cts;
        UpdateFileCmd _cmdUpdate;
        DeleteFileCmd _cmdDelete;
        DownloadFileCmd _cmdDownload;
        OpenFileCmd _cmdOpenFile;
        SaveAsCmd _cmdSaveAs;
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
            _pnl = new StackPanel();
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
        public event TypedEventHandler<FileList, bool> UploadFinished;

        /// <summary>
        /// 文件成功删除后事件
        /// </summary>
        public event EventHandler Deleted;
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
        /// 获取设置是否允许多文件，只在文件选择时控制
        /// </summary>
        public bool AllowMultiple
        {
            get { return (bool)GetValue(AllowMultipleProperty); }
            set { SetValue(AllowMultipleProperty, value); }
        }

        /// <summary>
        /// 获取当前选择的文件
        /// </summary>
        public FileItem Current { get; internal set; }

        /// <summary>
        /// 获取面板，内部绑定用
        /// </summary>
        public StackPanel Panel
        {
            get { return _pnl; }
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
        /// 获取更新文件命令
        /// </summary>
        public UpdateFileCmd CmdUpdate
        {
            get
            {
                if (_cmdUpdate == null)
                    _cmdUpdate = new UpdateFileCmd(this);
                return _cmdUpdate;
            }
        }

        /// <summary>
        /// 获取删除上传文件
        /// </summary>
        public DeleteFileCmd CmdDelete
        {
            get
            {
                if (_cmdDelete == null)
                    _cmdDelete = new DeleteFileCmd(this);
                return _cmdDelete;
            }
        }

        /// <summary>
        /// 获取下载命令
        /// </summary>
        public DownloadFileCmd CmdDownload
        {
            get
            {
                if (_cmdDownload == null)
                    _cmdDownload = new DownloadFileCmd(this);
                return _cmdDownload;
            }
        }

        /// <summary>
        /// 获取打开文件命令
        /// </summary>
        public OpenFileCmd CmdOpen
        {
            get
            {
                if (_cmdOpenFile == null)
                    _cmdOpenFile = new OpenFileCmd(this);
                return _cmdOpenFile;
            }
        }

        /// <summary>
        /// 获取另存为命令
        /// </summary>
        public SaveAsCmd CmdSaveAs
        {
            get
            {
                if (_cmdSaveAs == null)
                    _cmdSaveAs = new SaveAsCmd(this);
                return _cmdSaveAs;
            }
        }

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
            _ = AppendFile(() => FileKit.PickImages(), () => FileKit.PickImage());
        }

        /// <summary>
        /// 增加视频文件
        /// </summary>
        public void AddVideo()
        {
            _ = AppendFile(() => FileKit.PickVideos(), () => FileKit.PickVideo());
        }

        /// <summary>
        /// 增加音频文件
        /// </summary>
        public void AddAudio()
        {
            _ = AppendFile(() => FileKit.PickAudios(), () => FileKit.PickAudio());
        }

        /// <summary>
        /// 增加媒体文件
        /// </summary>
        public void AddMedia()
        {
            _ = AppendFile(() => FileKit.PickMedias(), () => FileKit.PickMedia());
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
                () => FileKit.PickFiles(p_uwpFileTypes, p_androidFileTypes, p_iosFileTypes),
                () => FileKit.PickFile(p_uwpFileTypes, p_androidFileTypes, p_iosFileTypes));
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="p_files"></param>
        public async Task UploadFiles(IList<FileData> p_files)
        {
            if (p_files == null || p_files.Count == 0)
                return;

            List<IUploadFile> vfs = new List<IUploadFile>();
            string date = AtSys.Now.ToString("yyyy-MM-dd HH:mm");
            foreach (var file in p_files)
            {
                if (file.Size > AtKit.GB)
                {
                    AtKit.Warn(string.Format("【{0}】\r\n文件超过1GB限制！", file.FileName));
                    continue;
                }

                FileItem vf = new FileItem();
                vf.Owner = this;
                await vf.InitUpload(file, date);
                _pnl.Children.Add(vf);
                vfs.Add(vf);
            }

            if (vfs.Count > 0)
                await HandleUpload(vfs);
            else
                UploadFinished?.Invoke(this, false);
        }

        /// <summary>
        /// 更新已上传的文件
        /// </summary>
        /// <param name="p_file">新文件</param>
        /// <param name="p_vf">待更新的旧文件</param>
        /// <returns></returns>
        public async Task UpdateFile(FileData p_file, FileItem p_vf)
        {
            if (p_file == null || p_vf == null)
                return;

            if (p_file.Size > AtKit.GB)
            {
                AtKit.Warn(string.Format("【{0}】\r\n文件超过1GB限制！", p_file.DisplayName));
                return;
            }

            // 新文件属性
            var date = AtSys.Now.ToString("yyyy-MM-dd HH:mm");
            await p_vf.InitUpload(p_file, date);
            await HandleUpload(new List<IUploadFile> { p_vf });
        }

        /// <summary>
        /// 处理多文件上传
        /// </summary>
        /// <param name="p_vfs"></param>
        async Task HandleUpload(List<IUploadFile> p_vfs)
        {
            UploadStarted?.Invoke(this, EventArgs.Empty);

            List<string> result = null;
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            try
            {
                result = await Uploader.Handle(p_vfs, _cts.Token);
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
            if (result == null || result.Count != p_vfs.Count)
            {
                // 全部失败
                AtKit.Warn("😢上传失败，请重新上传！");
                foreach (var vf in p_vfs.Cast<FileItem>())
                {
                    _pnl.Children.Remove(vf);
                }
                ReadData(Data);
            }
            else
            {
                suc = true;
                for (int i = 0; i < p_vfs.Count; i++)
                {
                    await (p_vfs[i] as FileItem).UploadSuccess(result[i]);
                }
                WriteData();
            }
            UploadFinished?.Invoke(this, suc);
        }

        async Task AppendFile(Func<Task<List<FileData>>> p_funMulti, Func<Task<FileData>> p_funSingle)
        {
            if (AllowMultiple)
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
        /// 增加录音
        /// </summary>
        public void CaptureVoice()
        {
            AtKit.Msg("录音");
        }

        /// <summary>
        /// 增加拍照
        /// </summary>
        public void TakePhoto()
        {
            AtKit.Msg("拍照");
        }

        /// <summary>
        /// 增加录视频
        /// </summary>
        public void RecordVideo()
        {
            AtKit.Msg("录视频");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="p_vf"></param>
        public async void DownloadFile(FileItem p_vf)
        {
            if (p_vf != null && await p_vf.Download())
                AtKit.Msg("下载成功！");
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="p_vf"></param>
        public void OpenFile(FileItem p_vf)
        {
            if (p_vf != null)
                p_vf.Open();
        }

        /// <summary>
        /// 文件另存为
        /// </summary>
        /// <param name="p_vf"></param>
        public void SaveAs(FileItem p_vf)
        {
            if (p_vf != null)
                p_vf.SaveAs();
        }

        /// <summary>
        /// 删除已上传的文件
        /// </summary>
        /// <param name="p_vf"></param>
        public async void DeleteFile(FileItem p_vf)
        {
            if (p_vf != null && await p_vf.Delete())
            {
                WriteData();
                Deleted?.Invoke(this, EventArgs.Empty);
            }
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
        internal void RemoveChild(FileItem p_vf)
        {
            _pnl.ChildrenTransitions = AtRes.AddDeleteTransition;
            _pnl.Children.Remove(p_vf);
            _pnl.ChildrenTransitions = null;
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
            _pnl.Children.Clear();
            if (string.IsNullOrEmpty(p_json))
                return;

            using (StringReader sr = new StringReader(p_json))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                // 最外层 [
                reader.Read();

                // FileItem [
                while (reader.Read())
                {
                    // 最外层 ]
                    if (reader.TokenType == JsonToken.EndArray)
                        break;

                    FileItem vf = new FileItem();
                    vf.Owner = this;
                    vf.ReadData(reader);
                    _pnl.Children.Add(vf);
                }
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
                return;
            }

            StringBuilder sb = new StringBuilder();
            using (StringWriter sr = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sr))
            {
                writer.WriteStartArray();
                foreach (var obj in _pnl.Children)
                {
                    if (obj is FileItem vf)
                        vf.WriteData(writer);
                }
                writer.WriteEndArray();
                writer.Flush();
            }

            _lockData = true;
            Data = sb.ToString();
            _lockData = false;
        }
        #endregion
    }
}
