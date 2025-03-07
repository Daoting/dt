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
            _fv.CellClick += OnCellClick;

            return;
        }

        void OnCellClick(FvCell e)
        {
            if (_fvProp.Data == e)
                return;

            if (_fvProp.Data != null && _fvProp.Data.GetType() == e.GetType())
            {
                _fvProp.Data = e;
                return;
            }
            
            var items = _fvProp.Items;
            using (items.Defer())
            {
                items.Clear();
                FvCell cell = new CTip();
                cell.ID = "ID";
                items.Add(cell);

                cell = new CList();
                cell.ID = "Title";
                cell.Title = "标题";
                items.Add(cell);
                
                foreach (var info in e.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    // 可设置属性
                    var attr = (CellParamAttribute)info.GetCustomAttribute(typeof(CellParamAttribute), false);
                    if (attr == null)
                        continue;

                    cell = Fv.CreateCell(info.PropertyType, info.Name);
                    cell.Title = attr.Title;
                    items.Add(cell);
                }

                cell = new CBool();
                cell.ID = "ShowTitle";
                cell.Title = "显示标题";
                items.Add(cell);
            }
            _fvProp.Data = e;
        }
        
        string GetXaml()
        {
            return "";
        }

        async void OnAdd()
        {
            var dlg = new SelectFvCellDlg();
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