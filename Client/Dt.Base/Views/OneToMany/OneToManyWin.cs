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
    [View("通用一对多视图")]
    public partial class OneToManyWin : Win
    {
        readonly OneToManyCfg _cfg;
        EntityQuery _query;
        
        public OneToManyWin(string p_jsonCfg)
            : this(JsonSerializer.Deserialize<OneToManyCfg>(p_jsonCfg))
        { }

        public OneToManyWin(OneToManyCfg p_cfg)
        {
            _cfg = p_cfg;

            _query = new EntityQuery() { Order = 1 };
            Ex.SetDock(_query, PanePosition.Left);
            Items.Add(_query);
            
            LoadCfg();
        }

        public OneToManyWin()
        {
            
        }
        
        async void LoadCfg()
        {
            await _cfg.Init();

            _query.LoadCfg(_cfg.ParentCfg);
            _query.Query += e =>
            {
                
            };
            ApplyTemplate();
        }
    }
}