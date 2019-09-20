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
    public partial class FileTransfer : Control, IDisposable
    {
        #region 静态成员
        public static readonly DependencyProperty XmlProperty = DependencyProperty.Register(
            "Xml",
            typeof(string),
            typeof(FileTransfer),
            new PropertyMetadata(null, OnXmlPropertyChanged));

        public static readonly DependencyProperty AllowMultipleProperty = DependencyProperty.Register(
            "AllowMultiple",
            typeof(bool),
            typeof(FileTransfer),
            new PropertyMetadata(true));

        static void OnXmlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileTransfer ft = (FileTransfer)d;
            if (!ft._lockXml)
                ft.ReadXml((string)e.NewValue);
        }
        #endregion

        #region 成员变量
        readonly StackPanel _pnl;
        bool _lockXml;
        CancellationTokenSource _cts;
        VirFile _current;

        UpdateFileCmd _cmdUpdate;
        DeleteFileCmd _cmdDelete;
        DownloadFileCmd _cmdDownload;
        OpenFileCmd _cmdOpenFile;
        SaveAsCmd _cmdSaveAs;
        BaseCommand _cmdAddOffice;
        BaseCommand _cmdAddImage;
        BaseCommand _cmdAddVideo;
        BaseCommand _cmdAddSound;
        BaseCommand _cmdAddFile;
        BaseCommand _cmdCaptureVoice;
        BaseCommand _cmdTakePhoto;
        BaseCommand _cmdRecordVideo;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public FileTransfer()
        {
            DefaultStyleKey = typeof(FileTransfer);
            _pnl = new StackPanel();
            _cts = new CancellationTokenSource();
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
        public event TypedEventHandler<FileTransfer, bool> UploadFinished;

        /// <summary>
        /// 文件成功删除后事件
        /// </summary>
        public event EventHandler Deleted;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置文件描述信息
        /// </summary>
        public string Xml
        {
            get { return (string)GetValue(XmlProperty); }
            set { SetValue(XmlProperty, value); }
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
        public VirFile Current
        {
            get { return _current; }
            internal set { _current = value; }
        }

        /// <summary>
        /// 获取面板，内部绑定用
        /// </summary>
        public StackPanel Panel
        {
            get { return _pnl; }
        }

        /// <summary>
        /// 取消者令牌
        /// </summary>
        public CancellationTokenSource Cts
        {
            get { return _cts; }
        }

        /// <summary>
        /// 获取所有VirFile
        /// </summary>
        public IEnumerable<VirFile> Items
        {
            get
            {
                return from obj in _pnl.Children
                       let vf = obj as VirFile
                       where vf != null
                       select vf;
            }
        }

        /// <summary>
        /// 获取总文件数
        /// </summary>
        public int FilesCount
        {
            get { return _pnl.Children.Count; }
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
        /// 获取添加Office文档命令
        /// </summary>
        public BaseCommand CmdAddOffice
        {
            get
            {
                if (_cmdAddOffice == null)
                    _cmdAddOffice = new BaseCommand((e) => AddFile(AtKit.OfficeFormat));
                return _cmdAddOffice;
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
                    _cmdAddImage = new BaseCommand((e) => AddFile(AtKit.ImageFormat));
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
                    _cmdAddVideo = new BaseCommand((e) => AddFile(AtKit.VideoFormat));
                return _cmdAddVideo;
            }
        }

        /// <summary>
        /// 获取添加音频命令
        /// </summary>
        public BaseCommand CmdAddSound
        {
            get
            {
                if (_cmdAddSound == null)
                    _cmdAddSound = new BaseCommand((e) => AddFile(AtKit.SoundFormat));
                return _cmdAddSound;
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
                    _cmdAddFile = new BaseCommand((e) => AddFile(null));
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
        /// 增加文件
        /// </summary>
        /// <param name="p_format">文件选择时的过滤类型</param>
        public async void AddFile(List<string> p_format)
        {
            FileOpenPicker picker = new FileOpenPicker();
            var filter = picker.FileTypeFilter;
            if (p_format != null && p_format.Count > 0)
            {
                foreach (var tp in p_format)
                {
                    filter.Add(tp);
                }
            }
            else
            {
                filter.Add("*");
            }

            if (AllowMultiple)
            {
                IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                if (files.Count > 0)
                    await UploadFiles(files);
            }
            else
            {
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // 若已有文件则为更新
                    if (FilesCount > 0)
                        await UpdateFile(file, (VirFile)_pnl.Children[0]);
                    else
                        await UploadFiles(new List<StorageFile>() { file });
                }
            }
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="p_files"></param>
        public async Task UploadFiles(IReadOnlyList<StorageFile> p_files)
        {
            if (p_files == null || p_files.Count == 0)
                return;

            List<IUploadFile> vfs = new List<IUploadFile>();
            string date = AtSys.Now.ToString("yyyy-MM-dd HH:mm");
            foreach (var file in p_files)
            {
                BasicProperties prop = await file.GetBasicPropertiesAsync();
                if (prop.Size > AtKit.GB)
                {
                    AtKit.Warn(string.Format("【{0}】\r\n文件超过1GB限制！", file.DisplayName));
                    continue;
                }

                VirFile vf = new VirFile();
                vf.SetOwner(this);
                await vf.InitUpload(file, AtKit.GetFileSizeDesc(prop.Size), date);
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
        public async Task UpdateFile(StorageFile p_file, VirFile p_vf)
        {
            if (p_file == null || p_vf == null)
                return;

            BasicProperties prop = await p_file.GetBasicPropertiesAsync();
            if (prop.Size > AtKit.GB)
            {
                AtKit.Warn(string.Format("【{0}】\r\n文件超过1GB限制！", p_file.DisplayName));
                return;
            }

            // 新文件属性
            var date = AtSys.Now.ToString("yyyy-MM-dd HH:mm");
            await p_vf.InitUpload(p_file, AtKit.GetFileSizeDesc(prop.Size), date);
            await HandleUpload(new List<IUploadFile> { p_vf });
        }

        /// <summary>
        /// 处理多文件上传
        /// </summary>
        /// <param name="p_vfs"></param>
        async Task HandleUpload(List<IUploadFile> p_vfs)
        {
            UploadStarted?.Invoke(this, EventArgs.Empty);

            bool suc;
            var result = await Uploader.Handle(p_vfs, _cts.Token);
            if (result == null || result.Count != p_vfs.Count)
            {
                // 全部失败
                AtKit.Warn("😢上传失败，请重新上传！");
                suc = false;
                foreach (var vf in p_vfs.Cast<VirFile>())
                {
                    _pnl.Children.Remove(vf);
                }
                ReadXml(Xml);
            }
            else
            {
                suc = true;
                for (int i = 0; i < p_vfs.Count; i++)
                {
                    await (p_vfs[i] as VirFile).UploadSuccess(result[i]);
                }
                WriteXml();
            }

            UploadFinished?.Invoke(this, suc);
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
        public async void DownloadFile(VirFile p_vf)
        {
            if (p_vf != null && await p_vf.Download())
                AtKit.Msg("下载成功！");
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="p_vf"></param>
        public void OpenFile(VirFile p_vf)
        {
            if (p_vf != null)
                p_vf.Open();
        }

        /// <summary>
        /// 文件另存为
        /// </summary>
        /// <param name="p_vf"></param>
        public void SaveAs(VirFile p_vf)
        {
            if (p_vf != null)
                p_vf.SaveAs();
        }

        /// <summary>
        /// 删除已上传的文件
        /// </summary>
        /// <param name="p_vf"></param>
        public async void DeleteFile(VirFile p_vf)
        {
            if (p_vf != null && await p_vf.Delete())
            {
                WriteXml();
                Deleted?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 支持释放资源
        /// </summary>
        public void Dispose()
        {
            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 取消上传或下载
        /// </summary>
        internal void CancelTransfer()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// 移除子项
        /// </summary>
        /// <param name="p_vf"></param>
        internal void RemoveChild(VirFile p_vf)
        {
            _pnl.ChildrenTransitions = AtRes.AddDeleteTransition;
            _pnl.Children.Remove(p_vf);
            _pnl.ChildrenTransitions = null;
        }
        #endregion

        #region Xml
        /// <summary>
        /// 反序列化，初次加载或重新加载
        /// </summary>
        /// <param name="p_xml"></param>
        /// <returns></returns>
        void ReadXml(string p_xml)
        {
            _pnl.Children.Clear();
            if (string.IsNullOrEmpty(p_xml))
                return;

            using (StringReader stream = new StringReader(p_xml))
            using (XmlReader reader = XmlReader.Create(stream, AtKit.ReaderSettings))
            {
                // <Fs>
                if (reader.Read() && reader.Read())
                {
                    while (reader.NodeType != XmlNodeType.None)
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Fs")
                            break;

                        VirFile vf = new VirFile();
                        vf.SetOwner(this);
                        vf.ReadXml(reader);
                        _pnl.Children.Add(vf);
                        reader.ReadToNextSibling("F");
                    }
                }
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        void WriteXml()
        {
            if (_pnl.Children.Count == 0)
            {
                _lockXml = true;
                Xml = null;
                _lockXml = false;
                return;
            }

            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, AtKit.WriterSettings))
            {
                writer.WriteStartElement("Fs");
                foreach (var obj in _pnl.Children)
                {
                    VirFile vf = obj as VirFile;
                    if (vf != null)
                        vf.WriteXml(writer);
                }
                writer.WriteEndElement();
                writer.Flush();
            }
            _lockXml = true;
            Xml = sb.ToString();
            _lockXml = false;
        }
        #endregion
    }
}
