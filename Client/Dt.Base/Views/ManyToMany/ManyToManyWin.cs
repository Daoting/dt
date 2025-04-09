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
    [View("通用多对多视图")]
    public partial class ManyToManyWin : Win
    {
        readonly ManyToManyCfg _cfg;
        EntityQuery _query;
        EntityList _mainList;
        List<RelatedEntityList> _relatedLists;
        EntityForm _mainForm;

        public ManyToManyWin(string p_jsonCfg)
            : this(ManyToManyCfg.Deserialize(p_jsonCfg))
        { }

        public ManyToManyWin(ManyToManyCfg p_cfg)
        {
            _cfg = p_cfg;
            CreateContent();
            LoadCfg();
        }

        /// <summary>
        /// Register.xaml中注册类型用
        /// </summary>
        public ManyToManyWin()
        { }

        void CreateContent()
        {
            _query = new EntityQuery() { Order = 1 };
            Ex.SetDock(_query, PanePosition.Left);
            Items.Add(_query);

            _mainList = new EntityList { Title = _cfg.MainCfg.ListCfg.Title };
            Items.Add(_mainList);

            Tabs tabs = new Tabs();
            _relatedLists = new List<RelatedEntityList>();
            foreach (var cfg in _cfg.RelatedCfgs)
            {
                var list = new RelatedEntityList { Title = cfg.Title };
                _relatedLists.Add(list);
                tabs.Items.Add(list);
            }
            Ex.SetDock(tabs, PanePosition.Right);
            Items.Add(tabs);

            _mainForm = new EntityForm { Title = _cfg.MainCfg.FormCfg.Title, OwnWin = this };
        }
        
        void LoadCfg()
        {
            _query.LoadCfg(_cfg.MainCfg);
            _query.Query += e =>
            {
                _mainList.Query(e);
                NaviTo(_mainList.Title);
            };

            _mainList.LoadCfg(_cfg.MainCfg);
            _mainList.Msg += e => _ = _mainForm.Query(e);
            string tabsTitle = string.Join(",", _relatedLists.Select(e => e.Title));
            _mainList.Navi += () => NaviTo(tabsTitle);

            _mainForm.LoadCfg(_cfg.MainCfg);
            _mainForm.UpdateList += e => _ = _mainList.Refresh(e.ID);
            _mainForm.UpdateRelated += e =>
            {
                foreach (var list in _relatedLists)
                {
                    list.Query(e.ID);
                }
            };

            for (int i = 0; i < _relatedLists.Count; i++)
            {
                var list = _relatedLists[i];
                list.LoadCfg(_cfg.RelatedCfgs[i]);
            }
        }
    }
}