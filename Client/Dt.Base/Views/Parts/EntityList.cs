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

            if (!string.IsNullOrEmpty(_cfg.ListCfg.Xaml))
            {
                _lv = Kit.LoadXaml<Lv>(_cfg.ListCfg.Xaml);
                if (_lv == null)
                    Throw.Msg($"加载Lv的xaml时错误：\n{_cfg.ListCfg.Xaml}");
            }
            else
            {
                string xaml = GetXaml();
                _lv = Kit.LoadXaml<Lv>(xaml);
            }

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

        string GetXaml()
        {
            StringBuilder sb = new StringBuilder("<a:Lv>\n<a:Cols>");
            foreach (var col in _cfg.Table.Columns)
            {
                // 过滤掉父表ID列
                if (_cfg.IsChild && col.Name.Equals(_cfg.ParentID, StringComparison.OrdinalIgnoreCase))
                    continue;

                string title = "";

                // 字段名中文时不再需要Title
                if (!string.IsNullOrEmpty(col.Comments)
                    && !col.IsChinessName)
                {
                    if (col.IsEnumCol)
                    {
                        string tpName = col.GetEnumName();
                        title = $" Title=\"{col.Comments.Substring(tpName.Length + 2)}\"";
                    }
                    else
                    {
                        title = $" Title=\"{col.Comments}\"";
                    }
                }

                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append($"<a:Col ID=\"{col.Name.ToLower()}\"{title} />");
            }
            sb.Append("\n</a:Cols>\n</a:Lv>");
            return sb.ToString();
        }
    }
}