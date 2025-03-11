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
        const string _xamlPrefix = "<a:Fv xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:a=\"using:Dt.Base\">";
        const string _xamlPostfix = "</a:Fv>";
        readonly FvDesignInfo _info;
        Fv _fv;

        public FvDesign(FvDesignInfo p_info)
        {
            InitializeComponent();

            Throw.IfNull(p_info);
            _info = p_info;
            Jz();
        }

        public static async Task<string> Show(FvDesignInfo p_info)
        {
            Dlg dlg = new Dlg { IsPinned = true, Title = "设计表单" };
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 850;
                dlg.Height = 500;
            }

            var win = new FvDesign(p_info);
            dlg.LoadWin(win);
            if (await dlg.ShowAsync())
                return win.GetXaml();
            return null;
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
        
        void Jz()
        {
            if (!string.IsNullOrEmpty(_info.Xaml))
            {
                var xaml = _info.Xaml.Trim();
                if (!xaml.StartsWith("<a:Fv "))
                {
                    xaml = _xamlPrefix + xaml + _xamlPostfix;
                }
                _fv = XamlReader.Load(xaml) as Fv;
            }
            else
            {
                _fv = new Fv();
            }
            _tabMain.Content = _fv;
            _fv.CellClick += (e) => FvDesignKit.LoadCellProperties(e, _fvProp);

            return;
        }
        
        string GetXaml()
        {
            return "";
        }

        async void OnAdd()
        {
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
    }
}