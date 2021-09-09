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
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public partial class UserParamsList : Mv
    {
        public UserParamsList()
        {
            InitializeComponent();
            LoadAll();
        }

        public async void OnSearch(string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (e == "#最近修改")
            {
                LoadLast();
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lv.Data = await AtCm.Query<Params>("参数-模糊查询", new { ID = $"%{e}%" });
            }
            NaviTo(this);
        }

        public async void Refresh()
        {

        }

        async void LoadAll()
        {
            _lv.Data = await AtCm.GetAll<Params>();
        }

        async void LoadLast()
        {
            _lv.Data = await AtCm.Query<Params>("参数-最近修改");
        }

        void OnNaviToSearch(object sender, Mi e)
        {
            NaviTo(_win.Search);
        }

        void OnAdd(object sender, Mi e)
        {
            _win.Edit.LoadData(null);
            NaviTo(_win.Edit);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Edit.LoadData(e.Data.To<Params>().ID);
            NaviTo(_win.Edit);
        }

        UserParamsWin _win => (UserParamsWin)_tab.OwnWin;
    }
}