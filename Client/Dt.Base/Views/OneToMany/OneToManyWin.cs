#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [View("通用一对多视图")]
    public partial class OneToManyWin : Win
    {
        readonly OneToManyCfg _cfg;
        EntityQuery _query;
        EntityList _parentList;
        List<EntityList> _childLists;
        EntityForm _parentForm;
        Dictionary<string, EntityForm> _childForms = new Dictionary<string, EntityForm>();

        public OneToManyWin(string p_jsonCfg)
            : this(JsonSerializer.Deserialize<OneToManyCfg>(p_jsonCfg))
        { }

        public OneToManyWin(OneToManyCfg p_cfg)
        {
            _cfg = p_cfg;
            CreateContent();
            LoadCfg();
        }

        /// <summary>
        /// Register.xaml中注册类型用
        /// </summary>
        public OneToManyWin()
        { }

        void CreateContent()
        {
            _query = new EntityQuery() { Order = 1 };
            Ex.SetDock(_query, PanePosition.Left);
            Items.Add(_query);

            _parentList = new EntityList { Title = _cfg.ParentCfg.ListCfg.Title };
            Items.Add(_parentList);

            Tabs tabs = new Tabs();
            _childLists = new List<EntityList>();
            foreach (var cfg in _cfg.ChildCfgs)
            {
                var list = new EntityList { Title = cfg.ListCfg.Title };
                _childLists.Add(list);
                tabs.Items.Add(list);
            }
            Ex.SetDock(tabs, PanePosition.Bottom);
            Items.Add(tabs);

            _parentForm = new EntityForm { Title = _cfg.ParentCfg.FormCfg.Title, OwnWin = this };
        }
        
        void LoadCfg()
        {
            _query.LoadCfg(_cfg.ParentCfg);
            _query.Query += e =>
            {
                _parentList.Query(e);
                NaviTo(_parentList.Title);
            };

            _parentList.LoadCfg(_cfg.ParentCfg);
            _parentList.Msg += e => _ = _parentForm.Query(e);
            string tabsTitle = string.Join(",", _childLists.Select(e => e.Title));
            _parentList.Navi += () => NaviTo(tabsTitle);

            _parentForm.LoadCfg(_cfg.ParentCfg);
            _parentForm.UpdateList += e => _ = _parentList.Refresh(e.ID);
            _parentForm.UpdateRelated += e =>
            {
                foreach (var list in _childLists)
                {
                    list.Query(e.ID);
                }
            };

            for (int i = 0; i < _childLists.Count; i++)
            {
                var list = _childLists[i];
                list.LoadCfg(_cfg.ChildCfgs[i]);
                list.Msg += e =>
                {
                    EntityForm form = null;
                    if (e.Action == FormAction.Open && !_childForms.TryGetValue(list.Cfg.Cls, out form))
                    {
                        form = new EntityForm { OwnWin = this };
                        form.LoadCfg(list.Cfg);
                        form.UpdateList += e => _ = list.Refresh(e.ID);
                        _childForms.Add(list.Cfg.Cls, form);
                    }
                    _ = form?.Query(e);
                };
            }
        }
    }
}