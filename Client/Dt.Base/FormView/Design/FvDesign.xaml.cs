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
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using System.Reflection;
using System.Threading.Tasks;
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
            Jz(_info.Xaml);
        }

        public static async Task<string> Show(FvDesignInfo p_info)
        {
            Dlg dlg = new Dlg { IsPinned = true, Title = "设计表单" };
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 850;
                dlg.Height = 600;
            }

            var win = new FvDesign(p_info);
            dlg.LoadWin(win);
            if (await dlg.ShowAsync())
                return win._fv.ExportXaml();
            return null;
        }

        public static void ShowWin()
        {
            var win = (FvDesign)Kit.OpenWin(typeof(FvDesign), "设计表单", Icons.排列, new FvDesignInfo());
            win._tabMain.Menu["确定"].Visibility = Visibility.Collapsed;
        }
        
        public bool IsFixCols => _info.Cols != null && _info.Cols.Count > 0;

        public IEnumerable<EntityCol> GetUnusedCols()
        {
            if (!IsFixCols)
                yield return null;

            foreach (var col in _info.Cols)
            {
                if (_fv.Items.FirstOrDefault(c => c is FvCell fc && fc.ID == col.Name) == null)
                    yield return col;
            }
        }

        public void Jz(string p_xaml)
        {
            _fv = string.IsNullOrEmpty(p_xaml) ? new Fv() : Fv.CreateByXaml(p_xaml);
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
                }
                _fv.Items.Add(cell);
            }
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
    }
}