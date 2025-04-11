#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Markup;
using System.Text;
#endregion

namespace Dt.Base.Views
{
    public sealed partial class EntityList : List
    {
        EntityCfg _cfg;
        Lv _lv;

        public EntityCfg Cfg => _cfg;

        public void LoadCfg(EntityCfg p_cfg)
        {
            _cfg = p_cfg;
            
            _lv = _cfg.BuildLv();
            Content = _lv;
            Lv = _lv;

            var cfg = _cfg.ListCfg;
            if (cfg.ShowAddMi || cfg.ShowDelMi)
            {
                Menu = CreateMenu(null, cfg.ShowAddMi, cfg.ShowDelMi);
                if (cfg.ShowMultiSelMi)
                    _lv.AddMultiSelMenu(Menu);
                _lv.SetMenu(CreateContextMenu(null, cfg.ShowAddMi, cfg.ShowDelMi));
            }
        }

        protected override async Task OnQuery()
        {
            if (_cfg.IsChild)
            {
                if (_parentID > 0)
                    _lv.Data = await _cfg.Query($"where {_cfg.ParentID}={_parentID}");
                else
                    _lv.Data = null;
            }
            else if (_clause == null)
            {
                _lv.Data = await _cfg.Query(null);
            }
            else
            {
                var par = await _clause.Build(_cfg.EntityType);
                _lv.Data = await _cfg.Query(par.Sql, par.Params);
            }
        }
    }
}