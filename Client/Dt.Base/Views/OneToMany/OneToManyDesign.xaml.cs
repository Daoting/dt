#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [ViewParamsEditor("通用一对多视图")]
    public sealed partial class OneToManyDesign : Dlg, IViewParamsEditor
    {
        OneToManyCfg _cfg;

        public OneToManyDesign()
        {
            InitializeComponent();
            IsPinned = true;
            ShowVeil = true;

            if (!Kit.IsPhoneUI)
            {
                Width = 460;
                Height = 400;
            }
        }

        public async Task<string> ShowDlg(string p_params)
        {
            _cfg = OneToManyCfg.Deserialize(p_params);
            _fv.Data = _cfg.ParentCfg;
            _lv.Data = _cfg.ChildCfgs;
            _cb.IsChecked = _cfg.IsUnionForm;
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

        async void OnEditParent(object sender, TappedRoutedEventArgs e)
        {
            var dlg = new EntityDesign();
            var json = await dlg.ShowDlg(_cfg.ParentCfg.Serialize());
            if (!string.IsNullOrEmpty(json))
            {
                var cfg = JsonSerializer.Deserialize<EntityCfg>(json);
                if (cfg != null)
                {
                    _cfg.ParentCfg = cfg;
                    _fv.Data = _cfg.ParentCfg;
                }
            }
        }

        async void OnAddChild()
        {
            var dlg = new EntityDesign();
            var json = await dlg.ShowDlg(new EntityCfg { IsChild = true }.Serialize());
            if (!string.IsNullOrEmpty(json))
            {
                var cfg = JsonSerializer.Deserialize<EntityCfg>(json);
                if (cfg != null)
                {
                    _cfg.ChildCfgs.Add(cfg);
                }
            }
        }

        async void OnDelChild(Mi e)
        {
            EntityCfg cfg = null;
            if (e.Data is EntityCfg entity)
            {
                cfg = entity;
            }
            else if (_lv.SelectedItem is EntityCfg en)
            {
                cfg = en;
            }

            if (cfg != null
                && await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                _cfg.ChildCfgs.Remove(cfg);
            }
        }

        void OnEditChild(Mi e)
        {
            if (e.Data is EntityCfg cfg)
                EditChild(cfg);
        }

        void OnItemDbClick(object obj)
        {
            if (_lv.SelectedItem is EntityCfg cfg)
                EditChild(cfg);
        }

        async void EditChild(EntityCfg cfg)
        {
            var dlg = new EntityDesign();
            var json = await dlg.ShowDlg(cfg.Serialize());
            if (!string.IsNullOrEmpty(json))
            {
                var ncfg = JsonSerializer.Deserialize<EntityCfg>(json);
                if (ncfg != null)
                {
                    int idx = _cfg.ChildCfgs.IndexOf(cfg);
                    _cfg.ChildCfgs.RemoveAt(idx);
                    _cfg.ChildCfgs.Insert(idx, ncfg);
                }
            }
        }

        void OnUnionClick(object sender, RoutedEventArgs e)
        {
            _cfg.IsUnionForm = _cb.IsChecked == true;
        }
    }
}