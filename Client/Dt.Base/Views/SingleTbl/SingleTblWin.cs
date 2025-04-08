#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [View("通用单表视图")]
    public partial class SingleTblWin : Win
    {
        readonly EntityCfg _cfg;
        EntityQuery _query;
        EntityList _list;
        EntityForm _form;
        
        public SingleTblWin(string p_jsonCfg)
        : this(EntityCfg.Deserialize(p_jsonCfg))
        { }

        public SingleTblWin(EntityCfg p_cfg)
        {
            _cfg = p_cfg;
            CreateContent();
            LoadCfg();
        }

        /// <summary>
        /// Register.xaml中注册类型用
        /// </summary>
        public SingleTblWin()
        { }

        void CreateContent()
        {
            _query = new EntityQuery() { Order = 1 };
            Ex.SetDock(_query, PanePosition.Left);
            Items.Add(_query);

            _list = new EntityList { Title = _cfg.ListCfg.Title };
            Items.Add(_list);
            
            _form = new EntityForm { Title = _cfg.FormCfg.Title, OwnWin = this };
        }
        
        void LoadCfg()
        {
            _query.LoadCfg(_cfg);
            _query.Query += e =>
            {
                _list.Query(e);
                NaviTo(_list.Title);
            };

            _list.LoadCfg(_cfg);
            _list.Msg += e => _ = _form.Query(e);

            _form.LoadCfg(_cfg);
            _form.UpdateList += e => _ = _list.Refresh(e.ID);
        }
    }
}