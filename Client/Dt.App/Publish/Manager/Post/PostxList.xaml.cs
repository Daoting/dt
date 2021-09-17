#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-17 创建
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

namespace Dt.App.Publish
{
    public partial class PostxList : Mv
    {
        public PostxList()
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
            NaviTo(new List<Mv> { _win.Form,  _win.KeywordList, _win.AlbumList, });
        }

        #region 搜索
        string _query;

        void OnSearch(object sender, string e)
        {
            _query = e;
            Query();
        }

        async void Query()
        {
            if (string.IsNullOrEmpty(_query))
            {
                _lv.Data = await AtPublish.Query<PostObj>("文章-管理列表");
            }
            else
            {
                _lv.Data = await AtPublish.Query<PostObj>("文章-模糊查询", new { input = $"%{_query}%" });
            }
        }
        #endregion

        PostWin _win => (PostWin)_tab.OwnWin;
    }
}