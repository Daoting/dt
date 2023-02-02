#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-11 创建
******************************************************************************/
#endregion

#region 引用命名
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

//#if WIN
//            Mi mi = new Mi { ID = "存根", Icon = Icons.链接 };
//            mi.Click += OnStub;
//            Menu.Items.Insert(1, mi);
//#endif
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
            if (UITree.RootContent is Desktop)
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
            if (UITree.RootContent is Desktop)
            {
                name = Desktop.Inst.MainWin.GetType().FullName;
            }
            else if (UITree.RootContent is Frame frame)
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
                name = UITree.RootContent.GetType().FullName;
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
#if WIN
        Dictionary<string, SqliteDbTbls> _sqliteTbls;

        /// <summary>
        /// 生成存根代码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStub(object sender, Mi e)
        {
            _sqliteTbls = new Dictionary<string, SqliteDbTbls>();
            StringBuilder sb = new StringBuilder();

            var stubType = Stub.Inst.GetType();
            while (stubType != typeof(Stub))
            {
                ExtractAssembly(stubType.Assembly);

                if (stubType == typeof(DefaultStub))
                    ExtractAssembly(Assembly.Load(new AssemblyName("Dt.Core")));

                sb.AppendLine(stubType.Name);
                sb.AppendLine("\t\t#region 自动生成");
                sb.AppendLine("\t\t// 本地库结构变化后，需通过《 win版app -> 系统日志 -> 存根 》重新生成！");
                sb.AppendLine();
                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// 合并本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型");
                sb.AppendLine("\t\t/// 先调用base.MergeSqliteDbs，不可覆盖上级的同名本地库");
                sb.AppendLine("\t\t/// </summary>");
                sb.AppendLine("\t\t/// <param name=\"p_sqliteDbs\"></param>");
                sb.AppendLine("\t\tprotected override void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_sqliteDbs)\r\n\t\t{");
                BuildSqliteDict(sb, false);

                sb.AppendLine("\r\n\t\t#endregion\r\n\r\n\r\n");

                _sqliteTbls.Clear();
                stubType = stubType.BaseType;
            }

            DataPackage data = new DataPackage();
            data.SetText(sb.ToString());
            Clipboard.SetContent(data);
            Kit.Msg("已复制到剪切板！");
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
                        if (attr is SqliteAttribute sqlite)
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

        void BuildSqliteDict(StringBuilder p_sb, bool p_isInternal)
        {
            p_sb.AppendLine("\t\t\tbase.MergeSqliteDbs(p_sqliteDbs);");
            if (_sqliteTbls.Count == 0)
            {
                p_sb.AppendLine("\r\n\t\t}");
                return;
            }

            foreach (var item in _sqliteTbls)
            {
                p_sb.AppendLine($"\t\t\tp_sqliteDbs[\"{item.Key}\"] = new SqliteTblsInfo");
                p_sb.AppendLine("\t\t\t{");
                p_sb.AppendLine($"\t\t\t\tVersion = \"{item.Value.GetVer()}\",");
                p_sb.AppendLine("\t\t\t\tTables = new List<Type>");
                p_sb.AppendLine("\t\t\t\t{");

                foreach (var tp in item.Value.Tbls)
                {
                    p_sb.AppendLine($"\t\t\t\t\ttypeof({tp.FullName}),");
                }

                p_sb.AppendLine("\t\t\t\t}");
                p_sb.AppendLine("\t\t\t};");
            }
            p_sb.Append("\t\t}");
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

    [LvCall]
    public class SysTraceCellUI
    {
        public static void FormatTitle(Env e)
        {
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
            var tbTime = new TextBlock { Foreground = Res.深灰1, VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(tbTime);

            var tbLevel = new TextBlock { Foreground = Res.WhiteBrush, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(tbLevel, 1);
            grid.Children.Add(tbLevel);

            Button btn = new Button { Content = "\uE006", Style = Res.浅字符按钮, HorizontalAlignment = HorizontalAlignment.Right };
            btn.Click += OnShowDetail;
            Grid.SetColumn(btn, 2);
            grid.Children.Add(btn);
            e.UI = grid;

            e.Set += c =>
            {
                var item = (TraceLogItem)c.Data;
                tbTime.Text = item.Log.Timestamp.ToString("HH:mm:ss");

                tbLevel.Text = item.Log.Level.ToString();
                if (item.Log.Level == LogEventLevel.Error || item.Log.Level == LogEventLevel.Fatal)
                {
                    tbLevel.Foreground = Res.RedBrush;
                }
                else if (item.Log.Level == LogEventLevel.Warning)
                {
                    tbLevel.Foreground = Res.YellowBrush;
                }
                else if (item.Log.Properties.TryGetValue("Rpc", out var val))
                {
                    tbLevel.Foreground = Res.湖蓝;
                    var tp = val.ToString("l", null);
                    tbLevel.Text = (tp == "Call") ? "Call" : "Recv";
                }
                else if (item.Log.Level == LogEventLevel.Information)
                {
                    tbLevel.Text = "Inf";
                }

                if (item.Log.Exception != null || item.Log.Properties.ContainsKey("Json"))
                {
                    btn.Visibility = Visibility.Visible;
                }
                else
                {
                    btn.Visibility = Visibility.Collapsed;
                }
            };
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
