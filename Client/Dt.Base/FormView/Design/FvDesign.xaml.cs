#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using System.Xml;
#endregion

namespace Dt.Base
{
    public partial class FvDesign : Win
    {
        readonly FvDesignInfo _info;
        Fv _fv;

        public FvDesign(FvDesignInfo p_info)
        {
            InitializeComponent();

            Throw.IfNull(p_info);
            _info = p_info;
            if (_info.Cols != null && _info.Cols.Count > 0)
            {
                _tabMain.Menu["增加"].ShowBtn = true;
            }
            Jz(_info.Xaml);
        }

        /// <summary>
        /// 显示设计表单对话框
        /// </summary>
        /// <param name="p_info"></param>
        /// <returns></returns>
        public static async Task<string> ShowDlg(FvDesignInfo p_info)
        {
            Dlg dlg = new Dlg { IsPinned = true, Title = "设计表单" };
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 1000;
                dlg.Height = 800;
            }

            var win = new FvDesign(p_info);
            dlg.LoadWin(win);
            if (await dlg.ShowAsync())
                return win._fv.ExportXaml();
            return null;
        }

        /// <summary>
        /// 显示设计表单窗口
        /// </summary>
        public static void ShowWin()
        {
            var win = (FvDesign)Kit.OpenWin(typeof(FvDesign), "设计表单", Icons.排列, new FvDesignInfo());
            win._tabMain.Menu["确定"].Visibility = Visibility.Collapsed;
        }

        internal FvDesignInfo Info => _info;

        internal Fv Fv => _fv;

        public void Jz(string p_xaml)
        {
            if (!string.IsNullOrEmpty(p_xaml))
            {
                _fv = _info.IsQueryFv ? Kit.LoadXaml<QueryFv>(p_xaml) : Kit.LoadXaml<Fv>(p_xaml);
                if (_fv == null)
                    Throw.Msg("无法根据xaml创建表单：\n" + p_xaml);
                
                var doc = new XmlDocument();
                doc.LoadXml(FvDesignKit.AddXmlns(p_xaml));
                var chs = doc.DocumentElement.ChildNodes;
                for (int i = 0; i < chs.Count; i++)
                {
                    var obj = _fv.Items[i];
                    var ch = chs[i];
                    if (ch.ChildNodes.Count > 0 && ch.LocalName == obj.GetType().Name)
                    {
                        if (obj is FvCell fc)
                        {

                        }
                        else if (obj is CBar bar)
                        {
                            bar.LoadXamlString(ch);
                        }
                    }
                }
            }
            else
            {
                _fv = _info.IsQueryFv ? new QueryFv() : new Fv();
            }

            _fv.IsDesignMode = true;
            _tabMain.Content = _fv;
            _fv.CellClick += (e) => FvDesignKit.LoadCellProps(e, _fvProp);
        }

        public string GetXaml()
        {
            return _fv.ExportXaml();
        }

        async void OnAdd()
        {
            _fv.ClearDesignCell();
            _fvProp.Data = null;
            _fvProp.Items.Clear();

            var dlg = new SelectFvCellDlg(this);
            if (await dlg.ShowAsync())
            {
                Row row = dlg.Row;
                var cell = Activator.CreateInstance(row[0] as Type);
                if (cell is FvCell fc)
                {
                    fc.ID = row.Str("ID");
                    if (_info.IsQueryFv)
                        fc.Query = QueryType.Editable;
                }
                _fv.Items.Add(cell);
            }
        }

        void OnBatchAdd()
        {
            int cnt = _fv.Items.Count;
            foreach (var col in _info.Cols)
            {
                if (_fv.Items.FirstOrDefault(c => c is FvCell fc && fc.ID == col.Name) == null)
                {
                    var fc = Fv.CreateCell(col.Type, col.Name);
                    if (_info.IsQueryFv)
                        fc.Query = QueryType.Editable;
                    _fv.Items.Add(fc);
                }
            }
            if (cnt == _fv.Items.Count)
                Kit.Warn("无新列可添加！");
            else
                Kit.Msg($"已批量添加{_fv.Items.Count - cnt}个格！");
        }

        void OnDel()
        {
            _fv.DelDesignCell();
            _fvProp.Data = null;
            _fvProp.Items.Clear();
        }

        void OnCopyXaml()
        {
            Kit.CopyToClipboard(_fv.ExportXaml());
        }

        void OnShowEditXaml()
        {
            new FvXamlEditDlg().ShowDlg(this);
        }

        void OnSave()
        {
            OwnDlg?.OnOK();
        }

        void OnCreatePreview()
        {
            var fv = Kit.LoadXaml<Fv>(_fv.ExportXaml());
            Row r = new Row();
            foreach (var c in fv.IDCells)
            {
                if (c is CText)
                    r.Add(c.ID, "");
                else if (c is CNum num)
                    r.Add(c.ID, num.IsInteger ? (int)0 : (double)0.1);
                else if (c is CBool)
                    r.Add(c.ID, false);
                else if (c is CDate)
                    r.Add(c.ID, Kit.Now);
                else if (c is CIcon)
                    r.Add(c.ID, Icons.选日);
                else
                    r.Add(c.ID, "");
            }
            fv.Data = r;
            var dlg = new Dlg { Title = "预览", MinHeight = 300 };
            dlg.Content = fv;
            dlg.Show();
        }
    }
}