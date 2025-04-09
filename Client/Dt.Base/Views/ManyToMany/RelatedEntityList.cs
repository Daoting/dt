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
    public sealed partial class RelatedEntityList : List
    {
        RelatedEntityCfg _cfg;
        Lv _lv;

        public void LoadCfg(RelatedEntityCfg p_cfg)
        {
            _cfg = p_cfg;

            _lv = CreateLv(_cfg);
            Content = _lv;
            Lv = _lv;

            Menu = new Menu();
            if (_cfg.ShowAddMi)
            {
                Menu.Items.Add(Mi.添加(OnAddRelated, enable: false));
            }
            if (_cfg.ShowDelMi)
            {
                Menu.Items.Add(Mi.删除(OnDelRelated));
                _lv.SetMenu(Menu.New(Mi.删除(OnDelRelated)));
            }
            if (_cfg.ShowMultiSelMi)
            {
                _lv.AddMultiSelMenu(Menu);
            }
        }

        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await _cfg.QueryRelated(_parentID.Value);
            }
            else
            {
                _lv.Data = null;
            }

            Mi mi = Menu["添加"];
            if (mi != null)
                mi.IsEnabled = _parentID > 0;
        }

        async void OnAddRelated(Mi e)
        {
            var dlg = new AddRelationDlg();
            if (await dlg.Show(_cfg, _parentID.Value, e))
            {
                var ls = new List<long>();
                foreach (var row in dlg.SelectedRows)
                {
                    ls.Add(row.ID);
                }
                if (ls.Count > 0 && await _cfg.AddRelation(ls, _parentID.Value))
                    await Refresh();
            }
        }

        async void OnDelRelated(Mi e)
        {
            List<long> ls = null;
            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                ls = new List<long>();
                foreach (var row in _lv.SelectedRows)
                {
                    ls.Add(row.ID);
                }
            }
            else
            {
                Row row = e.Row;
                if (row == null)
                    row = _lv.SelectedRow;

                if (row != null)
                    ls = new List<long> { row.ID };
            }

            if (ls != null && ls.Count > 0)
            {
                if (!await Kit.Confirm("确认要删除关联吗？"))
                {
                    Kit.Msg("已取消删除！");
                    return;
                }

                if (await _cfg.DelRelation(ls, _parentID.Value))
                    await Refresh();
            }
        }

        internal static Lv CreateLv(RelatedEntityCfg p_cfg)
        {
            Lv lv = null;
            if (!string.IsNullOrEmpty(p_cfg.ListXaml))
            {
                lv = Kit.LoadXaml<Lv>(p_cfg.ListXaml);
                if (lv == null)
                    Throw.Msg($"加载Lv的xaml时错误：\n{p_cfg.ListXaml}");
            }
            else
            {
                string xaml = GetXaml(p_cfg);
                lv = Kit.LoadXaml<Lv>(xaml);
            }
            return lv;
        }
        
        static string GetXaml(RelatedEntityCfg p_cfg)
        {
            StringBuilder sb = new StringBuilder("<a:Lv>\n<a:Cols>");
            foreach (var col in p_cfg.Table.Columns)
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