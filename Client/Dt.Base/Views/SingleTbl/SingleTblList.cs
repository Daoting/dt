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
    public partial class SingleTblList : List
    {
        SingleTblWin _win;
        Lv _lv;

        public void LoadCfg(SingleTblWin p_win)
        {
            Title = "列表";
            Icon = Icons.列表;
            _win = p_win;

            if (!string.IsNullOrEmpty(_win.Cfg.ListCfg.Xaml))
            {
                _lv = Kit.LoadXaml<Lv>(_win.Cfg.ListCfg.Xaml);
                if (_lv == null)
                    Throw.Msg($"加载Lv的xaml时错误：\n{_win.Cfg.ListCfg.Xaml}");
            }
            else
            {
                string xaml = GetXaml();
                _lv = Kit.LoadXaml<Lv>(xaml);
            }

            Content = _lv;
            Lv = _lv;
            Msg += e => _ = _win.Form.Query(e);

            var cfg = _win.Cfg.ListCfg;
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
            if (_clause == null)
            {
                _lv.Data = await _win.Cfg.Query(null);
            }
            else
            {
                var par = await _clause.Build(_win.Cfg.EntityType);
                _lv.Data = await _win.Cfg.Query(par.Sql, par.Params);
            }
        }

        string GetXaml()
        {
            StringBuilder sb = new StringBuilder("<a:Lv>\n<a:Cols>");
            foreach (var col in _win.Cfg.Table.Columns)
            {
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