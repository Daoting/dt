#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
#endregion

namespace Dt.Base.Views
{
    public sealed partial class EntityUnionForm : Form
    {
        OneToManyCfg _cfg;
        Fv _fv;

        public void LoadCfg(OneToManyCfg p_cfg)
        {
            _cfg = p_cfg;
            _fv = _cfg.ParentCfg.BuildFv();
            MainFv = _fv;
            BuildChild();
            
            var fc = _cfg.ParentCfg.FormCfg;
            if (fc.ShowAddMi || fc.ShowDelMi || fc.ShowSaveMi)
            {
                Menu = CreateMenu(null, fc.ShowAddMi, fc.ShowSaveMi, fc.ShowDelMi);
            }
            ShowVeil = true;
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await _cfg.ParentCfg.New();
        }

        protected override async Task OnAddChild(Fv p_fv)
        {
            for (int i = 0; i < _cfg.ChildCfgs.Count; i++)
            {
                if (Items[i].Fv == p_fv)
                {
                    var cfg = _cfg.ChildCfgs[i];
                    var en = await cfg.New();
                    en[cfg.ParentID] = _fv.Row.ID;
                    p_fv.Data = en;
                    return;
                }
            }
        }
        
        protected override async Task OnGet()
        {
            _fv.Data = await _cfg.ParentCfg.GetByID(_args.ID);
            for (int i = 0; i < _cfg.ChildCfgs.Count; i++)
            {
                var cfg = _cfg.ChildCfgs[i];
                Items[i].Lv.Data = await cfg.Query($"where {cfg.ParentID}={_args.ID}");
            }
        }

        void BuildChild()
        {
            foreach (var cfg in _cfg.ChildCfgs)
            {
                Fi fi = new Fi();
                fi.Title = cfg.ListCfg.Title;
                fi.Lv = cfg.BuildLv();
                fi.Fv = cfg.BuildFv();
                Items.Add(fi);
            }
        }
    }
}