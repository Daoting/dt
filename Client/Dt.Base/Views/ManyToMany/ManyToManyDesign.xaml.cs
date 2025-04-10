#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Input;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [ViewParamsEditor("通用多对多视图")]
    public sealed partial class ManyToManyDesign : Dlg, IViewParamsEditor
    {
        ManyToManyCfg _cfg;

        public ManyToManyDesign()
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
            _cfg = ManyToManyCfg.Deserialize(p_params);
            _fv.Data = _cfg.MainCfg;
            _lv.Data = _cfg.RelatedCfgs;
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
            var json = await dlg.ShowDlg(_cfg.MainCfg.Serialize());
            if (!string.IsNullOrEmpty(json))
            {
                var cfg = JsonSerializer.Deserialize<EntityCfg>(json);
                if (cfg != null)
                {
                    _cfg.MainCfg = cfg;
                    _fv.Data = _cfg.MainCfg;
                }
            }
        }

        async void OnAddChild()
        {
            var dlg = new RelatedEntityDesign();
            var json = await dlg.ShowDlg(new RelatedEntityCfg().Serialize());
            if (!string.IsNullOrEmpty(json))
            {
                var cfg = JsonSerializer.Deserialize<RelatedEntityCfg>(json);
                if (cfg != null)
                {
                    _cfg.RelatedCfgs.Add(cfg);
                }
            }
        }

        async void OnDelChild(Mi e)
        {
            RelatedEntityCfg cfg = null;
            if (e.Data is RelatedEntityCfg entity)
            {
                cfg = entity;
            }
            else if (_lv.SelectedItem is RelatedEntityCfg en)
            {
                cfg = en;
            }

            if (cfg != null
                && await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                _cfg.RelatedCfgs.Remove(cfg);
            }
        }

        void OnEditChild(Mi e)
        {
            if (e.Data is RelatedEntityCfg cfg)
                EditChild(cfg);
        }

        void OnItemDbClick(object obj)
        {
            if (_lv.SelectedItem is RelatedEntityCfg cfg)
                EditChild(cfg);
        }

        async void EditChild(RelatedEntityCfg cfg)
        {
            var dlg = new RelatedEntityDesign();
            var json = await dlg.ShowDlg(cfg.Serialize());
            if (!string.IsNullOrEmpty(json))
            {
                var ncfg = JsonSerializer.Deserialize<RelatedEntityCfg>(json);
                if (ncfg != null)
                {
                    int idx = _cfg.RelatedCfgs.IndexOf(cfg);
                    _cfg.RelatedCfgs.RemoveAt(idx);
                    _cfg.RelatedCfgs.Insert(idx, ncfg);
                }
            }
        }
    }
}