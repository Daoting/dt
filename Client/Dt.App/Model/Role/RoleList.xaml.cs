#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public partial class RoleList : Mv
    {
        string _query;

        public RoleList()
        {
            InitializeComponent();
        }

        public async void Update()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await AtCm.Query<RoleObj>("角色-所有");
            }
            else if (_query == "#系统角色")
            {
                _lv.Data = await AtCm.Query<RoleObj>("角色-系统角色");
            }
            else
            {
                _lv.Data = await AtCm.Query<RoleObj>("角色-模糊查询", new { name = $"%{_query}%" });
            }
        }

        protected override void OnInit(object p_params)
        {
            Update();
        }

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                _query = txt;
                Title = "角色列表 - " + txt;
                Update();
            }
        }

        Lazy<SearchMv> _lzSm = new Lazy<SearchMv>(() => new SearchMv
        {
            Placeholder = "角色名称",
            Fixed = { "全部", "系统角色", },
        });

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
            NaviTo(new List<Mv> { _win.Form, _win.UserList, _win.MenuList, _win.PrvList });
        }

        RoleWin _win => (RoleWin)_tab.OwnWin;
    }
}