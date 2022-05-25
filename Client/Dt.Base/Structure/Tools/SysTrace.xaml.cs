#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog.Events;
using System.Reflection;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
#endregion

namespace Dt.Base.Tools
{
    /// <summary>
    /// 系统日志输出面板
    /// </summary>
    public sealed partial class SysTrace : Dlg
    {
        const string _copyMsg = "已复制到剪切板！";
        static SysTrace _dlg;

        public SysTrace()
        {
            InitializeComponent();

            Title = "系统日志";
            IsPinned = true;
            WinPlacement = DlgPlacement.FromRight;

            _lv.CellEx = typeof(TraceViewEx);

#if DEBUG && WIN
            Mi mi = new Mi { ID = "存根代码", Icon = Icons.链接 };
            mi.Click += OnStub;
            Menu.Items.Insert(1, mi);
#endif
        }

        public static void ShowBox()
        {
            if (_dlg == null)
            {
                _dlg = new SysTrace { ShowVeil = false };
                if (!Kit.IsPhoneUI)
                    _dlg.Width = 400;

                // 数据源和Lv都是静态的，即使关闭仍做绑定处理
                _dlg.Closed += (s, e) => _dlg._lv.Data = null;
            }

            _dlg._lv.Data = TraceLogs.Data;
            _dlg.Show();
        }

        /// <summary>
        /// 模式切换时关闭dlg并置null，再次打开时重新创建
        /// </summary>
        internal static void OnUIModeChanged()
        {
            if (_dlg != null)
            {
                _dlg.Close();
                _dlg = null;
            }
        }

        void OnClear(object sender, Mi e)
        {
            TraceLogs.Clear();
        }

        void OnLocalDb(object sender, Mi e)
        {
            OpenWin(typeof(LocalDbView), "本地库");
        }

        void OnLocalFiles(object sender, Mi e)
        {
            OpenWin(typeof(LocalFileView), "本地文件");
        }

        void OpenWin(Type p_type, string p_title)
        {
            if (SysVisual.RootContent is Desktop)
            {
                // win模式已登录
                Kit.OpenWin(p_type);
            }
            else if (Kit.IsPhoneUI)
            {
                // phone模式，先关闭当前对话框
                Close();
                Kit.OpenWin(p_type);
            }
            else
            {
                // win模式未登录
                new Dlg
                {
                    Title = p_title,
                    Content = Activator.CreateInstance(p_type),
                    IsPinned = true,
                    WinPlacement = DlgPlacement.Maximized,
                }.Show();
            }
        }

        void OnLocalPath(object sender, Mi e)
        {
            Log.Debug(ApplicationData.Current.LocalFolder.Path);
        }

        void OnInstallPath(object sender, Mi e)
        {
            Log.Debug(Package.Current.InstalledLocation.Path);
        }

        void OnHostOS(object sender, Mi e)
        {
            Log.Debug(Kit.HostOS.ToString());
        }

        /// <summary>
        /// 将文本复制到剪贴板
        /// </summary>
        /// <param name="p_text"></param>
        /// <param name="p_showText"></param>
        void CopyToClipboard(string p_text, bool p_showText)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_text);
            Clipboard.SetContent(data);
            if (p_showText)
                Kit.Msg($"{_copyMsg}\r\n{p_text}");
            else
                Kit.Msg(_copyMsg);
        }

        void OnPageType(object sender, Mi e)
        {
            string name;
            if (SysVisual.RootContent is Desktop)
            {
                name = Desktop.Inst.MainWin.GetType().FullName;
            }
            else if (SysVisual.RootContent is Frame frame)
            {
                if (frame.Content is PhonePage page)
                {
                    if (page.Content is Tab tab)
                        name = tab.OwnWin?.GetType().FullName;
                    else if (page.Content is PhoneTabs tabs)
                        name = tabs.OwnWin?.GetType().FullName;
                    else
                        name = page.Content.GetType().FullName;
                }
                else
                {
                    name = frame.Content.GetType().FullName;
                }
            }
            else
            {
                name = SysVisual.RootContent.GetType().FullName;
            }
            Log.Debug(name);
        }

        void OnOpenMenu(object sender, AsyncCancelEventArgs e)
        {
            Menu m = (Menu)sender;
            var item = m.TargetData as TraceLogItem;
            m["复制json"].Visibility = item.Log.Properties.ContainsKey("Json") ? Visibility.Visible : Visibility.Collapsed;
            m["复制异常"].Visibility = item.Log.Exception != null ? Visibility.Visible : Visibility.Collapsed;
        }

        void OnCopyTitle(object sender, Mi e)
        {
            CopyToClipboard(((TraceLogItem)e.Data).Message, false);
        }

        void OnCopyJson(object sender, Mi e)
        {
            var item = (TraceLogItem)e.Data;
            if (item.Log.Properties.TryGetValue("Json", out var val))
            {
                var txt = TraceLogs.GetRpcJson(val.ToString("l", null));
                if (!string.IsNullOrEmpty(txt))
                    CopyToClipboard(txt, false);
                else
                    Kit.Msg("json内容为空");
            }
        }

        void OnCopyExcept(object sender, Mi e)
        {
            CopyToClipboard(((TraceLogItem)e.Data).ExceptionMsg, false);
        }

        #region 生成存根代码
#if DEBUG && WIN
        Dictionary<string, Type> _viewTypes;
        Dictionary<string, Type> _pushHandlers;
        Dictionary<string, SqliteDbTbls> _sqliteTbls;

        /// <summary>
        /// 生成存根代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStub(object sender, Mi e)
        {
            _viewTypes = new Dictionary<string, Type>();
            _pushHandlers = new Dictionary<string, Type>();
            _sqliteTbls = new Dictionary<string, SqliteDbTbls>();

            //IReadOnlyList<StorageFile> files = await Package.Current.InstalledLocation.GetFilesAsync();
            //foreach (StorageFile file in files)
            //{
            //    if (file.DisplayName.StartsWith("Dt.") && file.FileType == ".dll")
            //        ExtractAssembly(Assembly.Load(new AssemblyName(file.DisplayName)));
            //}
            ExtractAssembly(Assembly.Load(new AssemblyName("Dt.Core")));
            ExtractAssembly(Assembly.Load(new AssemblyName("Dt.Base")));
            ExtractAssembly(Assembly.Load(new AssemblyName("Dt.App")));

            // 最后提取业务程序集，确保业务程序集的类型优先级最高，比如替换系统自带的视图
            ExtractAssembly(Stub.Inst.GetType().Assembly);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t\t#region 自动生成");
            sb.AppendLine("\t\tprotected override void Init()\r\n\t\t{");

            sb.AppendLine("\t\t\t// 视图名称与窗口类型的映射字典，主要菜单项用");
            sb.AppendLine("\t\t\tViewTypes = new Dictionary<string, Type>");
            BuildStubDict(sb, _viewTypes);

            sb.AppendLine("\t\t\t// 处理服务器推送的类型字典");
            sb.AppendLine("\t\t\tPushHandlers = new Dictionary<string, Type>");
            BuildStubDict(sb, _pushHandlers);

            sb.AppendLine("\t\t\t// 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型");
            sb.AppendLine("\t\t\tSqliteDb = new Dictionary<string, SqliteTblsInfo>");
            BuildSqliteDict(sb);

            sb.AppendLine("\t\t}");
            sb.Append("\t\t#endregion");

            DataPackage data = new DataPackage();
            data.SetText(sb.ToString());
            Clipboard.SetContent(data);
            Kit.Msg("已复制到剪切板！");

            _viewTypes.Clear();
            _pushHandlers.Clear();
            _sqliteTbls.Clear();
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
                        else if (attr is SqliteAttribute sqlite)
                        {
                            // 本地sqlite库类型
                            SqliteDbTbls tbls;
                            if (!_sqliteTbls.TryGetValue(sqlite.DbName, out tbls))
                            {
                                tbls = new SqliteDbTbls();
                                _sqliteTbls[sqlite.DbName] = tbls;
                            }

                            tbls.Tbls.Add(tp);
                            foreach (var pro in tp.GetRuntimeProperties())
                            {
                                if (pro.GetCustomAttribute<IgnoreAttribute>(false) == null)
                                    tbls.Cols.Append(pro.Name);
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
                p_sb.AppendLine("\t\t\t{};");
                p_sb.AppendLine();
                return;
            }

            p_sb.AppendLine("\t\t\t{");
            foreach (var item in p_dt)
            {
                p_sb.AppendFormat("\t\t\t\t{{ \"{0}\", typeof({1}) }},\r\n", item.Key, item.Value.FullName);
            }
            p_sb.AppendLine("\t\t\t};");
            p_sb.AppendLine();
        }

        void BuildSqliteDict(StringBuilder p_sb)
        {
            if (_sqliteTbls.Count == 0)
            {
                p_sb.AppendLine("\t\t\t{};");
                return;
            }

            p_sb.AppendLine("\t\t\t{");
            foreach (var item in _sqliteTbls)
            {
                p_sb.AppendLine("\t\t\t\t{");
                p_sb.AppendLine($"\t\t\t\t\t\"{item.Key}\",");

                p_sb.AppendLine("\t\t\t\t\tnew SqliteTblsInfo");
                p_sb.AppendLine("\t\t\t\t\t{");
                p_sb.AppendLine($"\t\t\t\t\t\tVersion = \"{item.Value.GetVer()}\",");
                p_sb.AppendLine("\t\t\t\t\t\tTables = new List<Type>");
                p_sb.AppendLine("\t\t\t\t\t\t{");

                foreach (var tp in item.Value.Tbls)
                {
                    p_sb.AppendLine($"\t\t\t\t\t\t\ttypeof({tp.FullName}),");
                }

                p_sb.AppendLine("\t\t\t\t\t\t}");
                p_sb.AppendLine("\t\t\t\t\t}");
                p_sb.AppendLine("\t\t\t\t},");
            }
            p_sb.AppendLine("\t\t\t};");
        }

        class SqliteDbTbls
        {
            public List<Type> Tbls { get; } = new List<Type>();

            public StringBuilder Cols { get; } = new StringBuilder();

            public string GetVer()
            {
                return Kit.GetMD5(Cols.ToString());
            }
        }
#endif
        #endregion
    }

    public class TraceViewEx
    {
        public static Grid Title(LvItem p_vr)
        {
            var item = (TraceLogItem)p_vr.Data;
            Grid grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                },
                Height = 40,
            };

            TextBlock tb = new TextBlock { Text = item.Log.Timestamp.ToString("HH:mm:ss"), Foreground = Res.深灰1, VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(tb);

            tb = new TextBlock { Foreground = Res.WhiteBrush, Text = item.Log.Level.ToString(), Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            if (item.Log.Level == LogEventLevel.Error || item.Log.Level == LogEventLevel.Fatal)
            {
                tb.Foreground = Res.RedBrush;
            }
            else if (item.Log.Level == LogEventLevel.Warning)
            {
                tb.Foreground = Res.YellowBrush;
            }
            else if (item.Log.Properties.TryGetValue("Rpc", out var val))
            {
                tb.Foreground = Res.湖蓝;
                var tp = val.ToString("l", null);
                tb.Text = (tp == "Call") ? "Call" : "Recv";
            }
            else if (item.Log.Level == LogEventLevel.Information)
            {
                tb.Text = "Inf";
            }
            Grid.SetColumn(tb, 1);
            grid.Children.Add(tb);

            if (item.Log.Exception != null || item.Log.Properties.ContainsKey("Json"))
            {
                Button btn = new Button { Content = "\uE006", Style = Res.浅字符按钮, HorizontalAlignment = HorizontalAlignment.Right };
                btn.Click += OnShowDetail;
                Grid.SetColumn(btn, 2);
                grid.Children.Add(btn);
            }
            return grid;
        }

        static void OnShowDetail(object sender, RoutedEventArgs e)
        {
            var item = (TraceLogItem)((LvItem)((Button)sender).DataContext).Data;
            TextBox tb = new TextBox
            {
                AcceptsReturn = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderThickness = new Thickness(),
            };
            ScrollViewer.SetHorizontalScrollBarVisibility(tb, ScrollBarVisibility.Auto);
            ScrollViewer.SetVerticalScrollBarVisibility(tb, ScrollBarVisibility.Auto);
            if (item.Log.Properties.TryGetValue("Json", out var val))
            {
                var txt = TraceLogs.GetRpcJson(val.ToString("l", null));
                if (!string.IsNullOrEmpty(txt))
                    tb.Text = txt;
            }
            else
            {
                tb.Text = item.ExceptionMsg;
            }

            Dlg dlg = new Dlg
            {
                Title = item.Log.Timestamp.ToString("HH:mm:ss"),
                Content = tb,
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
    }
}
