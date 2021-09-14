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

namespace Dt.Sample
{
    public partial class MyEntityList : Mv
    {
        string _query;

        public MyEntityList()
        {
            InitializeComponent();
        }

        public void OnSearch(string p_txt)
        {
            if (!string.IsNullOrEmpty(p_txt))
            {
                _query = p_txt;
                Title = "列表 - " + p_txt;
                Update();
            }

            NaviTo(this);
        }

        public async void Update()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await App.AtCm.Query("select * from cm_role");
            }
            else if (_query == "#最近修改")
            {
                //_lv.Data = await AtCm.Query<Params>("参数-最近修改");
            }
            else
            {
                //_lv.Data = await AtCm.Query<Params>("参数-模糊查询", new { ID = $"%{_query}%" });
            }
        }

        protected override void OnInit(object p_params)
        {
            Update();
        }

        void OnNaviToSearch(object sender, Mi e)
        {
            NaviTo(_win.Search);
        }

        void OnAdd(object sender, Mi e)
        {
            _win.Form.Update(-1);
            NaviTo(_win.Form);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Form.Update(e.Row.ID);
            NaviTo(_win.Form);
        }

        MyEntityWin _win => (MyEntityWin)_tab.OwnWin;
    }
}