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
using System.Text.Json;
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

        #region 静态方法
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

        public static void CopyLocalPath()
        {
            CopyToClipboard(ApplicationData.Current.LocalFolder.Path, true);
        }

        public static void CopyWinType()
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
            CopyToClipboard(name, true);
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

        /// <summary>
        /// 将文本复制到剪贴板
        /// </summary>
        /// <param name="p_text"></param>
        /// <param name="p_showText">是否显示要复制的内容</param>
        static void CopyToClipboard(string p_text, bool p_showText = false)
        {
            DataPackage data = new DataPackage();
            data.SetText(p_text);
            Clipboard.SetContent(data);
            if (p_showText)
                Kit.Msg("已复制到剪切板：\r\n" + p_text);
            else
                Kit.Msg("已复制到剪切板！");
        }
        #endregion

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

        void OnSvcLog(object sender, Mi e)
        {
            Kit.OpenUrl(Kit.GetSvcUrl("cm") + "/.output");
        }

        void OnInstallPath(object sender, Mi e)
        {
            Log.Debug(Package.Current.InstalledLocation.Path);
        }

        void OnHostOS(object sender, Mi e)
        {
            Log.Debug(Kit.HostOS.ToString());
        }

        void OnAbout(object sender, Mi e)
        {
            Log.Information("搬运工\r\n版本 V4.2.1");
        }

        #region 输出
        void OnOutputClick(object sender, ItemClickArgs e)
        {
            var item = e.Data.To<TraceLogItem>();
            _tb.Text = item.Detial;
            _tb.Tag = item;
        }

        void CopyTitle(object sender, Mi e)
        {
            var item = _tb.Tag as TraceLogItem;
            if (item == null)
                return;

            string msg;
            if (item.Log.Properties.TryGetValue("SourceContext", out var val))
            {
                // 含日志来源，不显示命名空间，后缀为级别
                msg = val.ToString("l", null);
                int index = msg.LastIndexOf('.');
                if (index > -1)
                    msg = msg.Substring(index + 1);
            }
            else
            {
                msg = item.Log.Level.ToString();
            }
            CopyToClipboard(msg);
        }

        void CopyMessage(object sender, Mi e)
        {
            var item = _tb.Tag as TraceLogItem;
            if (item != null)
                CopyToClipboard(item.Message);
        }

        void CopyDetail(object sender, Mi e)
        {
            var item = _tb.Tag as TraceLogItem;
            if (item != null)
                CopyToClipboard(item.Detial);
        }

        void CopySql(object sender, Mi e)
        {
            string con;
            var item = _tb.Tag as TraceLogItem;
            if (item == null
                || !item.ExistDetial
                || (con = item.Detial) == null
                || !con.StartsWith('[')
                || !con.EndsWith(']'))
            {
                Kit.Warn("无 select 查询语句！");
                return;
            }

            bool find = false;
            try
            {
                var elem = JsonSerializer.Deserialize<JsonElement>(con);
                if (elem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var it in elem.EnumerateArray())
                    {
                        if (it.ValueKind == JsonValueKind.String)
                        {
                            var val = it.GetString().Trim();
                            if (val.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                            {
                                CopyToClipboard(val);
                                find = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch { }

            if (!find)
                Kit.Warn("无 select 查询语句！");
        }

        #endregion

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
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                },
                Height = 40,
            };
            var tbTime = new TextBlock { Foreground = Res.深灰1, VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(tbTime);

            var tbLevel = new TextBlock { Foreground = Res.深灰1, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(tbLevel, 1);
            grid.Children.Add(tbLevel);
            e.UI = grid;

            e.Set += c =>
            {
                var item = (TraceLogItem)c.Data;
                tbTime.Text = item.Log.Timestamp.ToString("HH:mm:ss");

                if (item.Log.Properties.TryGetValue("SourceContext", out var val))
                {
                    // 含日志来源，不显示命名空间，后缀为级别
                    var txt = val.ToString("l", null);
                    int index = txt.LastIndexOf('.');
                    if (index > -1)
                        txt = txt.Substring(index + 1);

                    if (item.Log.Level == LogEventLevel.Warning
                        || item.Log.Level == LogEventLevel.Error
                        || item.Log.Level == LogEventLevel.Fatal)
                    {
                        txt = $"{txt} — {item.Log.Level}";
                    }
                    tbLevel.Text = txt;
                }
                else if (item.Log.Level == LogEventLevel.Information)
                {
                    tbLevel.Text = "Inf";
                }
                else
                {
                    tbLevel.Text = item.Log.Level.ToString();
                }

                // 级别控制颜色
                if (item.Log.Level == LogEventLevel.Error || item.Log.Level == LogEventLevel.Fatal)
                {
                    tbLevel.Foreground = Res.RedBrush;
                }
                else if (item.Log.Level == LogEventLevel.Warning)
                {
                    tbLevel.Foreground = Res.YellowBrush;
                }
            };
        }
    }
}
