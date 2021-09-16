#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
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

namespace Dt.App.Model
{
    public partial class MenuList : Mv
    {
        public MenuList()
        {
            InitializeComponent();
        }

        public async void Update()
        {
            // 记录已选择的节点
            var m = _tv.Selected<MenuObj>();
            long id = m == null ? -1 : m.ID;
            _tv.Data = await AtCm.Query<MenuObj>("菜单-完整树");

            object select = null;
            if (id > 0)
            {
                select = (from row in (Table)_tv.Data
                          where row.ID == id
                          select row).FirstOrDefault();
            }
            _tv.SelectedItem = (select == null) ? _tv.FixedRoot : select;
        }

        protected override void OnInit(object p_params)
        {
            MenuObj m = new MenuObj(ID: 0, Name: "菜单", IsGroup: true, Icon: "主页");
            m.AddCell("parentname", "");
            _tv.FixedRoot = m;

            Update();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            _win.Form.Update(e.Row.ID);
            NaviTo(new List<Mv> { _win.Form, _win.RoleList, });
        }

        void OnMoveUp(object sender, Mi e)
        {
            var src = e.Data.To<MenuObj>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetTopBrother(src) as MenuObj;
            if (tgt != null)
                Exchange(src, tgt);
        }

        void OnMoveDown(object sender, Mi e)
        {
            var src = e.Data.To<MenuObj>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetFollowingBrother(src) as MenuObj;
            if (tgt != null)
                Exchange(src, tgt);
        }

        async void Exchange(MenuObj src, MenuObj tgt)
        {
            if (await AtCm.ExchangeDispidx(src, tgt))
            {
                Update();
                AtCm.PromptForUpdateModel("菜单调序成功");
            }
        }

        MenuWin _win => (MenuWin)_tab.OwnWin;
    }
}