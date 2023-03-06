#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Model
{
    public partial class PrvList : Tab
    {
        string _query;

        public PrvList()
        {
            InitializeComponent();
        }

        public async void Update()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await AtCm.Query<PrvX>("权限-所有");
            }
            else
            {
                _lv.Data = await AtCm.Query<PrvX>("权限-模糊查询", new { ID = $"%{_query}%" });
            }
        }

        protected override void OnFirstLoaded()
        {
            Update();
        }

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                _query = txt;
                Title = "权限列表 - " + txt;
                Update();
            }
        }

        Lazy<FuzzySearch> _lzSm = new Lazy<FuzzySearch>(() => new FuzzySearch
        {
            Placeholder = "权限名称",
            Fixed = { "全部",  },
        });

        void OnAdd(object sender, Mi e)
        {
            _win.Form.Update(null);
            NaviToChildren();
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Form.Update(e.Data.To<PrvX>().ID);
            NaviToChildren();
        }

        void OnUserList(object sender, Mi e)
        {
            Forward(new PrvUserList(), e.Data.To<PrvX>().ID);
        }

        void NaviToChildren()
        {
            NaviTo(new List<Tab> { _win.Form,  _win.RoleList, _win.UserList });
        }

        PrvWin _win => (PrvWin)OwnWin;
    }
}