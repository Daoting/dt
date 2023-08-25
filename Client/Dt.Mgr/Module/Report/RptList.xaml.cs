#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
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
using Dt.Mgr.Rbac;
using Dt.Base.Tools;
#endregion

namespace Dt.Mgr.Module
{
    public partial class RptList : Tab
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
                _lv.Data = await RptX.Query(null);
            }
            else
            {
                _lv.Data = await RptX.Query($"where name like '%{_query}%'");
            }
        }

        protected override void OnFirstLoaded()
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

        void OnRefresh(object sender, Mi e)
        {
            RefreshSqliteWin.UpdateSqliteFile("report");
        }

        RptWin _win => (RptWin)OwnWin;
    }
}