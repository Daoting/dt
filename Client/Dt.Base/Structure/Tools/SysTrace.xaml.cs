﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 监视输出面板
    /// </summary>
    public sealed partial class SysTrace : Win
    {
        static SysTrace _win;
        static Dlg _dlg;
        static Dlg _dlgDb;

        public SysTrace()
        {
            InitializeComponent();
            _lv.View = new TraceItemSelector
            {
                Normal = (DataTemplate)Resources["Normal"],
                Call = (DataTemplate)Resources["Call"],
                Recv = (DataTemplate)Resources["Recv"],
                Exception = (DataTemplate)Resources["Exception"],
            };
            _lv.CellEx = typeof(TraceViewEx);
            _lv.Data = AtKit.TraceList;
            _lv.ItemClick += OnOutputClick;

            // mono不支持 Stream.Position = 0，JsonRpc第136行！无法输出返回内容
            if (!AtSys.IsPhoneUI)
            {
                _lv.Loaded += OnLoaded;
                _lv.Unloaded += OnUnloaded;
            }
        }

        public static void ShowBox()
        {
            // 桌面
            if (SysVisual.RootContent is Desktop)
            {
                // 桌面时停靠在左侧
                if (_win == null)
                {
                    _win = new SysTrace();
                    _win.SetSplitWidth(400);
                }

                // 注销后再打开时可能异常！
                Desktop.Inst.LeftWin = _win;
                return;
            }

            // phone模式
            if (SysVisual.RootContent is Frame)
            {
                AtApp.OpenWin(typeof(SysTrace));
                return;
            }

            // win模式未登录
            if (_dlg == null)
            {
                var trace = new SysTrace();
                _dlg = new Dlg
                {
                    Title = "系统监视",
                    Content = trace,
                    IsPinned = true,
                    WinPlacement = DlgPlacement.FromLeft,
                    BorderBrush = AtRes.浅灰边框,
                    Width = 400
                };
            }
            _dlg.Show();
        }

        void OnClear(object sender, Mi e)
        {
            AtKit.TraceList.Clear();
        }

        void OnLocalDb(object sender, Mi e)
        {
            if (AtSys.IsPhoneUI || SysVisual.RootContent is Desktop)
            {
                AtApp.OpenWin(typeof(LocalDbView));
                return;
            }

            // win模式未登录
            if (_dlgDb == null)
            {
                _dlgDb = new Dlg
                {
                    Title = "本地库",
                    Content = new LocalDbView(),
                    IsPinned = true,
                    WinPlacement = DlgPlacement.Maximized,
                    BorderBrush = AtRes.浅灰边框,
                };
            }
            _dlgDb.Show();
        }

        /// <summary>
        /// 选择输出行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOutputClick(object sender, ItemClickArgs e)
        {
            Row row = e.Row;
            string txt = row.Str("content");
            if (txt.Length > 2 && txt[0] == '[' && txt[1] != '\r')
            {
                // 初次时对Json格式化，带缩进
                txt = FormatJson(txt);
                row.InitVal("content", txt);
            }
            _tb.Text = txt;
        }

        void OnCopy(object sender, Mi e)
        {
            if (_tb.Text != "")
                CopyToClipboard(_tb.Text);
        }

        void OnWrap(object sender, Mi e)
        {
            _tb.TextWrapping = e.IsChecked ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        void OnLocalPath(object sender, Mi e)
        {
            _tb.Text = ApplicationData.Current.LocalFolder.Path;
            CopyToClipboard(_tb.Text);
        }

        void OnInstallPath(object sender, Mi e)
        {
            _tb.Text = Package.Current.InstalledLocation.Path;
            CopyToClipboard(_tb.Text);
        }

        /// <summary>
        /// 将文本复制到剪贴板
        /// </summary>
        /// <param name="p_text"></param>
        void CopyToClipboard(string p_text)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_text);
            Clipboard.SetContent(data);
            AtKit.Msg("已复制到剪切板！");
        }

        /// <summary>
        /// 格式化Json串，带缩进
        /// </summary>
        /// <param name="p_json"></param>
        /// <returns></returns>
        string FormatJson(string p_json)
        {
            try
            {
                return JsonSerializer.Serialize<object>(JsonSerializer.Deserialize<object>(p_json), JsonOptions.IndentedSerializer);
            }
            catch { }
            return p_json;
        }

        void OnPageType(object sender, Mi e)
        {
            if (SysVisual.RootContent is Desktop)
                _tb.Text = Desktop.Inst.MainWin.GetType().FullName;
            else if (SysVisual.RootContent is Frame frame)
                _tb.Text = frame.Content.GetType().FullName;
            else
                _tb.Text = SysVisual.RootContent.GetType().FullName;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            AtSys.TraceRpc = true;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            AtSys.TraceRpc = false;
        }

        #region 生成存根代码
#if DEBUG
        Dictionary<string, Type> _viewTypes;
        Dictionary<string, Type> _pushHandlers;
        Dictionary<string, Type> _serializeTypes;
        Dictionary<string, Type> _stateTbls;
        StringBuilder _sbState;

        /// <summary>
        /// 生成存根代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnStub(object sender, Mi e)
        {
            _viewTypes = new Dictionary<string, Type>();
            _pushHandlers = new Dictionary<string, Type>();
            _serializeTypes = new Dictionary<string, Type>();
            _stateTbls = new Dictionary<string, Type>();
            _sbState = new StringBuilder();

            StorageFile exeFile = null;
            IReadOnlyList<StorageFile> files = await Package.Current.InstalledLocation.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                // Bs临时
                if (file.DisplayName.StartsWith("Dt.") || file.DisplayName.StartsWith("Bs."))
                {
                    if (file.FileType == ".dll")
                        ExtractAssembly(Assembly.Load(new AssemblyName(file.DisplayName)));
                    else if (file.FileType == ".exe")
                        exeFile = file;
                }
            }

            // 最后加载exe，确保exe中提取的类型优先级最高
            if (exeFile != null)
                ExtractAssembly(Assembly.Load(new AssemblyName(exeFile.DisplayName)));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t\t#region 自动生成");

            sb.AppendLine("\t\t/// <summary>");
            sb.AppendLine("\t\t/// 获取视图字典");
            sb.AppendLine("\t\t/// </summary>");
            sb.AppendLine("\t\tpublic Dictionary<string, Type> ViewTypes => new Dictionary<string, Type>");
            BuildStubDict(sb, _viewTypes);

            sb.AppendLine("\t\t/// <summary>");
            sb.AppendLine("\t\t/// 处理服务器推送的类型字典");
            sb.AppendLine("\t\t/// </summary>");
            sb.AppendLine("\t\tpublic Dictionary<string, Type> PushHandlers => new Dictionary<string, Type>");
            BuildStubDict(sb, _pushHandlers);

            sb.AppendLine("\t\t/// <summary>");
            sb.AppendLine("\t\t/// 获取自定义可序列化类型字典");
            sb.AppendLine("\t\t/// </summary>");
            sb.AppendLine("\t\tpublic Dictionary<string, Type> SerializeTypes => new Dictionary<string, Type>");
            BuildStubDict(sb, _serializeTypes);

            sb.AppendLine("\t\t/// <summary>");
            sb.AppendLine("\t\t/// 获取状态库表类型");
            sb.AppendLine("\t\t/// </summary>");
            sb.AppendLine("\t\tpublic Dictionary<string, Type> StateTbls => new Dictionary<string, Type>");
            BuildStubDict(sb, _stateTbls);

            // 所有状态库类型和属性的字符串，取MD5值以区分每次的变化
            sb.AppendLine("\t\t/// <summary>");
            sb.AppendLine("\t\t/// 获取状态库版本号，和本地不同时自动更新");
            sb.AppendLine("\t\t/// </summary>");
            sb.AppendFormat("\t\tpublic string StateDbVer => \"{0}\";", AtKit.GetMD5(_sbState.ToString()));
            sb.AppendLine();

            sb.Append("\t\t#endregion");

            DataPackage data = new DataPackage();
            data.SetText(sb.ToString());
            Clipboard.SetContent(data);
            AtKit.Msg("已复制到剪切板！");

            _viewTypes.Clear();
            _pushHandlers.Clear();
            _serializeTypes.Clear();
            _stateTbls.Clear();
        }

        /// <summary>
        /// 提取程序集中预定义的有用类型，只在初始化时调用
        /// </summary>
        /// <param name="p_asm">程序集</param>
        void ExtractAssembly(Assembly p_asm)
        {
            if (p_asm == null)
                return;

            try
            {
                foreach (Type tp in p_asm.ExportedTypes)
                {
                    TypeInfo tpInfo = tp.GetTypeInfo();
                    // 枚举标签
                    foreach (Attribute attr in tpInfo.GetCustomAttributes(false))
                    {
                        if (attr is ViewAttribute va)
                        {
                            // 视图
                            _viewTypes[va.Alias] = tp;
                        }
                        else if (attr is JsonObjAttribute ja)
                        {
                            // 可序列化类型
                            _serializeTypes[ja.Alias] = tp;
                        }
                        else if (attr is StateTableAttribute)
                        {
                            // 本地状态库类型
                            _stateTbls[tp.Name.ToLower()] = tp;
                            _sbState.Append(tp.Name);
                            foreach (var pro in tp.GetRuntimeProperties())
                            {
                                if (pro.GetCustomAttribute<IgnoreAttribute>(false) == null)
                                    _sbState.Append(pro.Name);
                            }
                        }
                        else if (attr is PushApiAttribute)
                        {
                            // 客户端服务方法
                            _pushHandlers[tp.Name.ToLower()] = tp;
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException loadEx)
            {
                string msg;
                if (loadEx.LoaderExceptions.Count() > 0)
                {
                    msg = loadEx.LoaderExceptions[0].Message;
                }
                else
                {
                    msg = string.Format("加载程序集【{0}】的过程中异常：\r\n{1}", p_asm.FullName, loadEx.Message);
                }
                throw new Exception(msg);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("加载程序集【{0}】的过程中异常：\r\n{1}", p_asm.FullName, ex.Message));
            }
        }

        void BuildStubDict(StringBuilder p_sb, Dictionary<string, Type> p_dt)
        {
            if (p_dt.Count == 0)
            {
                p_sb.AppendLine("\t\t{};");
                p_sb.AppendLine();
                return;
            }

            p_sb.AppendLine("\t\t{");
            foreach (var item in p_dt)
            {
                p_sb.AppendFormat("\t\t\t{{ \"{0}\", typeof({1}) }},\r\n", item.Key, item.Value.FullName);
            }
            p_sb.AppendLine("\t\t};");
            p_sb.AppendLine();
        }

#else
        void OnStub(object sender, Mi e)
        {
        }
#endif
        #endregion
    }

    public class TraceItemSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }
        public DataTemplate Call { get; set; }
        public DataTemplate Recv { get; set; }
        public DataTemplate Exception { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch ((TraceOutType)((LvItem)item).Row.Int("type"))
            {
                case TraceOutType.RpcCall:
                case TraceOutType.WsCall:
                    return Call;
                case TraceOutType.RpcRecv:
                case TraceOutType.WsRecv:
                case TraceOutType.ServerPush:
                    return Recv;
                case TraceOutType.RpcException:
                case TraceOutType.UnhandledException:
                    return Exception;
                default:
                    return Normal;
            }
        }
    }

    public class TraceViewEx
    {
        public static string ServiceName(LvItem p_vr)
        {
            var row = (Row)p_vr.Data;
            switch ((TraceOutType)row["type"])
            {
                case TraceOutType.RpcCall:
                case TraceOutType.RpcRecv:
                    return row.Str("service");
                case TraceOutType.WsCall:
                case TraceOutType.WsRecv:
                    return "ws";
                case TraceOutType.ServerPush:
                    return "push";
                default:
                    return null;
            }
        }
    }
}
