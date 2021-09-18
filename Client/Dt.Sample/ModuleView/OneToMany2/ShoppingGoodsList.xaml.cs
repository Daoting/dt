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
    public sealed partial class ShoppingGoodsList : Mv
    {
        long _id;

        public ShoppingGoodsList()
        {
            InitializeComponent();
        }

        public void Update(long p_id)
        {
            _id = p_id;
            Menu["添加"].IsEnabled = true;
            Query();
        }

        public void Clear()
        {
            _id = -1;
            Menu["添加"].IsEnabled = false;
            _lv.Data = null;
        }

        async void Query()
        {
            _lv.Data = await AtCm.Query<GoodsObj>("select * from oa_goods where ParentID=@ParentID", new { ParentID = _id });
        }

        void OnAdd(object sender, Mi e)
        {
            ShowForm(-1);
        }

        void OnEdit(object sender, Mi e)
        {
            ShowForm(e.Data.To<GoodsObj>().ID);
        }

        async void ShowForm(long p_id)
        {
            var form = new ShoppingGoodsForm();
            form.Update(p_id, _id);
            if (await Forward<bool>(form, null, true))
                Query();
        }

        async void OnDel(object sender, Mi e)
        {
            var d = e.Data.To<GoodsObj>();
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

        ShoppingWin _win => (ShoppingWin)_tab.OwnWin;
    }
}
