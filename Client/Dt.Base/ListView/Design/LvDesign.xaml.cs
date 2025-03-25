#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Base
{
    public partial class LvDesign : Win
    {
        readonly LvDesignInfo _info;
        Lv _lv;

        public LvDesign(LvDesignInfo p_info)
        {
            InitializeComponent();

            Throw.IfNull(p_info);
            _info = p_info;
            Jz(_info.Xaml);
            JzOnce();
        }

        /// <summary>
        /// 显示设计列表对话框
        /// </summary>
        /// <param name="p_info"></param>
        /// <returns></returns>
        public static async Task<string> ShowDlg(LvDesignInfo p_info)
        {
            Dlg dlg = new Dlg { IsPinned = true, Title = "设计列表" };
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 1000;
                dlg.Height = 800;
            }

            var win = new LvDesign(p_info);
            dlg.LoadWin(win);
            if (await dlg.ShowAsync())
                return win._lv.ExportXaml();
            return null;
        }

        /// <summary>
        /// 显示设计列表窗口
        /// </summary>
        public static void ShowWin()
        {
            Kit.OpenWin(typeof(LvDesign), "设计列表", Icons.排列, new LvDesignInfo());
        }

        internal LvDesignInfo Info => _info;

        internal Lv Lv => _lv;

        public void Jz(string p_xaml)
        {
            if (_lv != null)
            {
                _lv.Data = null;
                if (_lv.View is Cols cs)
                    cs.LayoutChanged -= OnColsChanged;
            }

            if (!string.IsNullOrEmpty(p_xaml))
            {
                _lv = Kit.LoadXaml<Lv>(p_xaml);
                if (_lv == null)
                    Throw.Msg("无法根据xaml创建列表：\n" + p_xaml);

                if (_lv.View != null)
                {
                    var doc = new XmlDocument();
                    using (var stringReader = new StringReader(p_xaml))
                    using (XmlReader reader = XmlReader.Create(
                        stringReader,
                        new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Fragment,
                            IgnoreWhitespace = true,
                            IgnoreComments = true,
                            IgnoreProcessingInstructions = true
                        },
                        FvDesignKit.GetXmlContext()))
                    {
                        doc.Load(reader);
                    }

                    var chs = doc.DocumentElement.ChildNodes;
                    for (int i = 0; i < chs.Count; i++)
                    {
                        var ch = chs[i];
                        if (!ch.LocalName.StartsWith("Lv."))
                        {
                            _lv.ViewXaml = FvDesignKit.GetNodeXml(ch, false);
                        }
                        else if (ch.LocalName == "Lv.FilterCfg")
                        {
                            _lv.FilterCfgXaml = FvDesignKit.GetNodeXml(ch, false);
                        }
                    }
                }
            }
            else
            {
                _lv = new Lv();
            }

            _tabMain.Content = _lv;
            _fv.Data = _lv;

            if (_lv.View is Cols cols)
                cols.LayoutChanged += OnColsChanged;
            LoadTestData();
        }

        void OnCopyXaml()
        {
            Kit.CopyToClipboard(_lv.ExportXaml());
        }

        void OnShowEditXaml()
        {
            new LvXamlEditDlg().ShowDlg(this);
        }

        void OnSave()
        {
            OwnDlg?.OnOK();
        }

        void OnApplyView(object sender, RoutedEventArgs e)
        {
            Jz(_lv.ExportXaml());
        }

        void OnViewTemp(Mi mi)
        {
            StringBuilder sb = new StringBuilder();
            if (mi.ID == "完整Cols定义")
            {
                if (_info.Cols != null && _info.Cols.Count > 0)
                {
                    sb.AppendLine("<a:Cols>");
                    foreach (var col in _info.Cols)
                    {
                        sb.AppendLine($"    <a:Col ID=\"{col.Name}\" Title=\"{col.Name}\" />");
                    }
                    sb.Append("</a:Cols>");
                }
                else
                {
                    sb.AppendLine("<a:Cols>");
                    sb.AppendLine("    <a:Col ID=\"txt\" Title=\"标题\" />");
                    sb.Append("</a:Cols>");
                }
            }
            else if (mi.ID == "Col")
            {
                sb.AppendLine("    <a:Col ID=\"txt\" Title=\"标题\" />");
            }
            else
            {
                if (_info.Cols != null && _info.Cols.Count > 0)
                {
                    sb.AppendLine("<DataTemplate>");
                    sb.AppendLine("  <StackPanel Padding=\"10\">");
                    foreach (var col in _info.Cols)
                    {
                        sb.AppendLine($"    <a:Dot ID=\"{col.Name}\" />");
                    }
                    sb.AppendLine("  </StackPanel>");
                    sb.Append("</DataTemplate>");
                }
                else
                {
                    sb.AppendLine("<DataTemplate>");
                    sb.AppendLine("  <StackPanel Padding=\"10\">");
                    sb.AppendLine("    <a:Dot ID=\"txt\" />");
                    sb.AppendLine("  </StackPanel>");
                    sb.Append("</DataTemplate>");
                }
            }
            FvDesignKit.AddXamlToCText((CText)_fv["ViewXaml"], sb.ToString());
        }

        void OnColsChanged()
        {
            if (_lv.View is Cols cols)
                _fv["ViewXaml"].Val = cols.ExportXaml();
        }

        void LoadTestData()
        {
            var tbl = new Table();
            if (_lv.View is Cols cols)
            {
                foreach (var col in cols.OfType<Col>())
                {
                    if (_info.Cols != null && _info.Cols.FirstOrDefault(p => p.Name == col.ID) is EntityCol src)
                        tbl.Add(col.ID, src.Type);
                    else
                        tbl.Add(col.ID);
                }
            }
            else if (_lv.View is DataTemplate view)
            {
                var c = view.LoadContent() as UIElement;
                foreach (var dot in c.FindChildrenByType<Dot>())
                {
                    if (_info.Cols != null && _info.Cols.FirstOrDefault(p => p.Name == dot.ID) is EntityCol src)
                        tbl.Add(dot.ID, src.Type);
                    else
                        tbl.Add(dot.ID);
                }
            }

            if (tbl.Columns.Count > 0)
            {
                Random r = new Random();
                for (int i = 0; i < 20; i++)
                {
                    var row = tbl.AddRow();
                    for (int j = 0; j < tbl.Columns.Count; j++)
                    {
                        var tp = tbl.Columns[j].Type;
                        if (tp == typeof(string))
                            row[j] = "测试" + r.Next(10000);
                        else if (tp == typeof(int) || tp == typeof(double) || tp == typeof(long))
                            row[j] = r.Next(10000);
                        else if (tp == typeof(DateTime))
                            row[j] = DateTime.Now.AddDays(r.Next(100));
                        else if (tp == typeof(bool))
                            row[j] = r.Next(2) == 1;
                    }
                }
            }
            _lv.Data = tbl;
        }

        void JzOnce()
        {
            if (_info.Cols != null && _info.Cols.Count > 0)
            {
                Nl<string> ls = new Nl<string>();
                foreach (var col in _info.Cols)
                {
                    ls.Add(col.Name);
                }
                ((CList)_fv["GroupName"]).Data = ls;
            }
        }

        void OnFilterCfgTemp(Mi mi)
        {
            if (mi.ID == "默认")
            {
                FvDesignKit.AddXamlToCText((CText)_fv["FilterCfgXaml"], "<a:FilterCfg />");
            }
            else
            {
                FvDesignKit.AddXamlToCText((CText)_fv["FilterCfgXaml"], "<a:FilterCfg FilterCols=\"xm\" EnablePinYin=\"true\" IsRealtime=\"true\" />");
            }
        }

        void OnApplyFilterCfg(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_lv.FilterCfgXaml))
            {
                _lv.FilterCfg = null;
            }
            else
            {
                _lv.FilterCfg = Kit.LoadXaml<FilterCfg>(_lv.FilterCfgXaml);
            }
        }
    }
}