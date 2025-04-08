#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [ViewParamsEditor("通用单表视图")]
    public sealed partial class EntityDesign : Dlg, IViewParamsEditor
    {
        static Table _entityCls;
        EntityCfg _cfg;

        public EntityDesign()
        {
            InitializeComponent();
            IsPinned = true;
            ShowVeil = true;
        }

        public async Task<string> ShowDlg(string p_params)
        {
            _cfg = EntityCfg.Deserialize(p_params);
            if (_cfg.IsChild)
            {
                _barQuery.Visibility = Visibility.Collapsed;
                _fvMain["QueryFvXaml"].Visibility = Visibility.Collapsed;
                _fvMain["ParentID"].Visibility = Visibility.Visible;
            }

            _fvMain.Data = _cfg;
            _fvList.Data = _cfg.ListCfg;
            _fvForm.Data = _cfg.FormCfg;

            if (await ShowAsync())
            {
                return GetResult();
            }
            return null;
        }

        string GetResult()
        {
            return _cfg.Serialize();
        }

        async void EditQueryFvXaml(object sender, RoutedEventArgs e)
        {
            var info = new FvDesignInfo { Xaml = _cfg.QueryFvXaml, IsQueryFv = true };
            if (_cfg.Table != null)
            {
                var cols = new List<EntityCol>();
                foreach (var col in _cfg.Table.Columns)
                {
                    cols.Add(new EntityCol(col.Name, col.Type));
                }
                info.Cols = cols;
            }

            var xaml = await FvDesign.ShowDlg(info);
            if (!string.IsNullOrEmpty(xaml))
                _fvMain["QueryFvXaml"].Val = xaml;
        }

        async void EditListXaml(object sender, RoutedEventArgs e)
        {
            var info = new LvDesignInfo { Xaml = _cfg.ListCfg.Xaml, };
            if (_cfg.Table != null)
            {
                var cols = new List<EntityCol>();
                foreach (var col in _cfg.Table.Columns)
                {
                    cols.Add(new EntityCol(col.Name, col.Type));
                }
                info.Cols = cols;
            }

            var xaml = await LvDesign.ShowDlg(info);
            if (!string.IsNullOrEmpty(xaml))
                _fvList["Xaml"].Val = xaml;
        }

        async void EditFormXaml(object sender, RoutedEventArgs e)
        {
            var info = new FvDesignInfo { Xaml = _cfg.FormCfg.Xaml, };
            if (_cfg.Table != null)
            {
                var cols = new List<EntityCol>();
                foreach (var col in _cfg.Table.Columns)
                {
                    cols.Add(new EntityCol(col.Name, col.Type));
                }
                info.Cols = cols;
            }

            var xaml = await FvDesign.ShowDlg(info);
            if (!string.IsNullOrEmpty(xaml))
                _fvForm["Xaml"].Val = xaml;
        }

        void OnLoadEntityCls(CList p_list, AsyncArgs p_args)
        {
            if (_entityCls == null)
            {
                string prefix = "Tbl-";
                _entityCls = new Table { { "cls" }, { "tbl" }, { "group" } };
                var ls = from item in Kit.AllAliasTypes
                         where item.Key.StartsWith(prefix)
                         orderby item.Value.Name
                         select item;
                foreach (var item in ls)
                {
                    int index = item.Value.AssemblyQualifiedName.IndexOf(", Version=");
                    _entityCls.AddRow(new
                    {
                        cls = item.Value.AssemblyQualifiedName.Substring(0, index),
                        tbl = item.Key.Substring(prefix.Length),
                        group = "远程库",
                    });
                }

                foreach (var item in Kit.AllSqliteDbs)
                {
                    foreach (var tbl in item.Value.Tables.OrderBy(t => t.Name))
                    {
                        int index = tbl.AssemblyQualifiedName.IndexOf(", Version=");
                        _entityCls.AddRow(new
                        {
                            cls = tbl.AssemblyQualifiedName.Substring(0, index),
                            tbl = tbl.Name.TrimEnd('X') + "，库" + item.Key,
                            group = "本地库",
                        });
                    }
                }
            }

            p_list.Data = _entityCls;
        }

        async void OnClsChanged(FvCell arg1, object arg2)
        {
            var list = (CList)_fvMain["ParentID"];
            list.Value = null;
            list.Data = null;

            if (!string.IsNullOrEmpty(_cfg.QueryFvXaml)
                || _cfg.ParentID != null
                || _cfg.ListCfg.IsChanged
                || _cfg.FormCfg.IsChanged)
            {
                if (await Kit.Confirm("实体类型已修改，是否清空所有配置？"))
                {
                    _cfg.QueryFvXaml = null;
                    _cfg.ListCfg = new EntityListCfg { Owner = _cfg };
                    _cfg.FormCfg = new EntityFormCfg { Owner = _cfg };

                    _fvMain.Data = null;
                    _fvMain.Data = _cfg;
                    _fvList.Data = _cfg.ListCfg;
                    _fvForm.Data = _cfg.FormCfg;
                }
            }
        }

        void OnLoadKeyCols(CList arg1, AsyncArgs arg2)
        {
            if (_cfg.Table != null)
            {
                var cols = new Nl<string>();
                foreach (var col in _cfg.Table.Columns)
                {
                    cols.Add(col.Name);
                }
                arg1.Data = cols;
            }
        }
    }
}