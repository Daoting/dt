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

        }
        
        void OnColsChanged()
        {
            if (_lv.View is Cols cols)
                _fv["ViewXaml"].Val = cols.ExportXaml();
        }
    }
}