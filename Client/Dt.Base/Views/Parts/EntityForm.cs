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
    public sealed partial class EntityForm : Form
    {
        EntityCfg _cfg;
        Fv _fv;

        public void LoadCfg(EntityCfg p_cfg)
        {
            _cfg = p_cfg;
            _fv = _cfg.BuildFv();
            MainFv = _fv;
         
            var cfg = _cfg.FormCfg;
            if (cfg.ShowAddMi || cfg.ShowDelMi || cfg.ShowSaveMi)
            {
                Menu = CreateMenu(null, cfg.ShowAddMi, cfg.ShowSaveMi, cfg.ShowDelMi);
            }
        }

        protected override async Task OnAdd()
        {
            var en = await _cfg.New();
            if (_cfg.IsChild && _args.ParentID.HasValue)
            {
                // 子表单
                en[_cfg.ParentID] = _args.ParentID.Value;
            }
            _fv.Data = en;
        }

        protected override async Task OnGet()
        {
            _fv.Data = await _cfg.GetByID(_args.ID);
        }
    }
}