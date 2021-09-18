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

namespace Dt.Sample.ModuleView.OneToMany2
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
            ShowForm(-1);
        }

        void OnEdit(object sender, Mi e)
        {
            ShowForm(e.Data.To<ShoppingObj>().ID);
        }

        async void ShowForm(long p_id)
        {
            var form = new ShoppingForm();
            form.Update(p_id);
            if (await Forward<bool>(form, null, true))
                Query();
        }

        async void OnDel(object sender, Mi e)
        {
            var d = e.Data.To<ShoppingObj>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await AtCm.Delete(d))
                Query();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            NaviTo(new List<Mv> {  _win.GoodsList, });
            if (!e.IsChanged)
                return;

            var p_id = e.Row.ID;
            _win?.GoodsList.Update(p_id);
        }

        void OnDataChanged(object sender, INotifyList e)
        {
            _win?.GoodsList.Clear();
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