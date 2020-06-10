#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Fz.Base;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Fz.Kehu
{
    public partial class PostList : UserControl
    {
        PostDetail _detail;
        public PostList()
        {
            InitializeComponent();
            _lv.PageData = new PageData { NextPage = OnNextPage, PageSize = 5 };
            ImgKit.LoadImage("photo/banner.png", _imgBanner);
        }

        public Win Win { get; set; }

        void OnPostClick(object sender, ItemClickArgs e)
        {
            if (_detail == null)
                _detail = new PostDetail();
            _detail.Refresh(e.Row);
            Win.LoadCenter(_detail);
        }

        async void OnNextPage(PageData e)
        {
            var ls = await AtPublish.Query("select id,title,cover,url from pub_post where IsPublish=1 order by Dispidx desc");
            e.LoadPageData(ls);
        }
    }
}