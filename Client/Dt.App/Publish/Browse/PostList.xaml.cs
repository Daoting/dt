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
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;
#endregion

namespace Dt.App.Publish
{
    public partial class PostList : UserControl
    {
        readonly Win _win;
        PostDetail _detail;
        long _testID;

        public PostList(Win p_win, long p_testID)
        {
            InitializeComponent();

            _win = p_win;
            _lv.View = new PostItemSelector
            {
                Default = (DataTemplate)Resources["Default"],
                CoverTitle = (DataTemplate)Resources["CoverTitle"],
            };

            if (p_testID < 0)
            {
                _lv.PageData = new PageData { NextPage = OnNextPage, PageSize = 5 };
            }
            else
            {
                _testID = p_testID;
                Loaded += OnLoadTest;
            }
        }

        async void OnLoadTest(object sender, RoutedEventArgs e)
        {
            _lv.Data = await AtPublish.Query("select id,Title,Cover,Summary,Url,TempType,AllowCoverClick,AllowComment from pub_post where id=@id", new { id = _testID });
        }

        void OnPostClick(object sender, ItemClickArgs e)
        {
            if (_detail == null)
                _detail = new PostDetail();
            _detail.Refresh(e.Row);
            _win.LoadMain(_detail);
        }

        async void OnNextPage(PageData e)
        {
            var ls = await AtPublish.Query("select id,Title,Cover,Summary,Url,TempType,AllowCoverClick,AllowComment from pub_post where IsPublish=1 order by Dispidx desc");
            e.LoadPageData(ls);
        }

        async void OnShare(object sender, RoutedEventArgs e)
        {
            Row row = ((LvItem)((Button)sender).DataContext).Row;
            await Share.RequestAsync(new ShareTextRequest
            {
                Subject = "福祉堂",
                Text = row.Str("Title"),
                Uri = row.Str("Url"),
            });
        }
    }

    public class PostItemSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate CoverTitle { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (((LvItem)item).Row.Int("TempType"))
            {
                case 0:
                    return Default;
                case 1:
                    return CoverTitle;
            }
            return Default;
        }
    }
}