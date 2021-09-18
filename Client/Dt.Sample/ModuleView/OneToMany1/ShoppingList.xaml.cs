#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Sample.ModuleView.OneToMany1
{
    public partial class ShoppingList : Mv
    {
        public ShoppingList()
        {
            InitializeComponent();
        }

        public void Update()
        {
            Query();
        }

        protected override void OnInit(object p_params)
        {
            Query();
        }

        void OnAdd(object sender, Mi e)
        {
            _win.Form.Update(-1);
            NaviToChildren();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Form.Update(e.Row.ID);
            NaviToChildren();
        }

        void NaviToChildren()
        {
            NaviTo(new List<Mv> { _win.Form,  _win.GoodsList, });
        }

        #region 搜索
        string _query;

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                _query = txt;
                Title = "购物列表 - " + txt;
                Query();
            }
        }

        Lazy<SearchMv> _lzSm = new Lazy<SearchMv>(() => new SearchMv
        {
            Placeholder = "购物名称",
            Fixed = { "全部", },
        });

        async void Query()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await AtCm.Query<ShoppingObj>("select * from oa_shopping");
            }
            else
            {
                _lv.Data = await AtCm.Query<ShoppingObj>("购物-模糊查询", new { ID = $"%{_query}%" });
            }
        }
        #endregion

        ShoppingWin _win => (ShoppingWin)_tab.OwnWin;
    }
}