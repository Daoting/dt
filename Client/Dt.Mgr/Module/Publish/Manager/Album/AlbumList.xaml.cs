#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-17 创建
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

namespace Dt.Mgr.Module
{
    public partial class AlbumList : Tab
    {
        public AlbumList()
        {
            InitializeComponent();
        }

        public void Update()
        {
            Query();
        }

        protected override void OnFirstLoaded()
        {
            Query();
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

        #region 搜索
        string _query;

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                _query = txt;
                Title = "文章专辑列表 - " + txt;
                Query();
            }
        }

        Lazy<FuzzySearch> _lzSm = new Lazy<FuzzySearch>(() => new FuzzySearch
        {
            Placeholder = "文章专辑名称",
            Fixed = { "全部", },
        });


        async void Query()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await AtCm.Query<PubAlbumX>("发布-所有专辑");
            }
            else
            {
                _lv.Data = await AtCm.Query<PubAlbumX>("发布-模糊查询专辑", new { name = $"%{_query}%" });
            }
        }
        #endregion

        AlbumWin _win => (AlbumWin)OwnWin;
    }
}