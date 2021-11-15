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
    public partial class RptList : Mv
    {
        string _query;

        public RptList()
        {
            InitializeComponent();
        }

        public void OnSearch(string p_txt)
        {
            if (!string.IsNullOrEmpty(p_txt))
            {
                _query = p_txt;
                Title = "报表列表 - " + p_txt;
                Update();
            }

            NaviTo(this);
        }

        public async void Update()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await AtCm.Query<RptObj>("报表-所有");
            }
            else if (_query == "#最近修改")
            {
                _lv.Data = await AtCm.Query<RptObj>("报表-最近修改");
            }
            else
            {
                _lv.Data = await AtCm.Query<RptObj>("报表-模糊查询", new { ID = $"%{_query}%" });
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

        RptWin _win => (RptWin)_tab.OwnWin;
    }
}