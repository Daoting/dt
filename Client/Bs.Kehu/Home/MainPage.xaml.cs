#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 主页
    /// </summary>
    public sealed partial class MainPage : UserControl
    {
        DataTemplate _itemTemp;
        Menu _menu;

        public MainPage()
        {
            InitializeComponent();

#if UWP
            _itemTemp = Resources["ItemTemp"] as DataTemplate;
#else
            _itemTemp = StaticResources.ItemTemp;
#endif

            // android上快速滑动时未触发PointerMoved！
#if ANDROID
            _sv.ViewChanged += OnScrollViewChanged;
#endif

            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            LoadArticles();

        }

        #region 加载文章
        void LoadArticles()
        {
            for (int i = 0; i < 10; i++)
            {
                AddArticleItem(new Article
                {
                    AuthorIcon = "ms-appx:///Bs.Kehu/Assets/header.png",
                    AuthorName = "胡志强",
                    JobTitle = "高级介护师",
                    PubDate = "2019-12-27",
                    Title = "赵阿姨脑卒中的康复",
                    Img = "ms-appx:///Bs.Kehu/Assets/u285.jpg",
                    ShareCnt = "12",
                    CommentCnt = "6792",
                    LikeCnt = "323k"
                });

                AddArticleItem(new Article
                {
                    AuthorIcon = "ms-appx:///Bs.Kehu/Assets/u366.png",
                    AuthorName = "福祉堂",
                    JobTitle = "全国连锁",
                    PubDate = "2019-11-27",
                    Title = "虚弱的防治",
                    Img = "ms-appx:///Bs.Kehu/Assets/u287.png",
                    ShareCnt = "3908",
                    CommentCnt = "16k",
                    LikeCnt = "323k"
                });

                AddArticleItem(new Article
                {
                    AuthorIcon = "ms-appx:///Bs.Kehu/Assets/u364.png",
                    AuthorName = "李小琳",
                    JobTitle = "中级介护师",
                    PubDate = "2019-08-17",
                    Title = "慢性气管炎",
                    Img = "ms-appx:///Bs.Kehu/Assets/u298.jpg",
                    ShareCnt = "9",
                    CommentCnt = "5896",
                    LikeCnt = "8563"
                });
            }
        }

        void AddArticleItem(Article p_article)
        {
            _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var item = _itemTemp.LoadContent() as FrameworkElement;
            item.DataContext = p_article;
            Grid.SetRow(item, _grid.RowDefinitions.Count - 1);
            _grid.Children.Add(item);
        }
        #endregion


        void OnJieHuClick(object sender, RoutedEventArgs e)
        {
            AtJieHu.ShowDetail((((Button)sender).DataContext as Article).AuthorID);
        }

        void OnShowContextMenu(object sender, RoutedEventArgs e)
        {
            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true, WinPlacement = MenuPosition.BottomLeft };
                Mi mi = new Mi { ID = "收藏", Icon = Icons.收藏 };
                mi.Click += OnFavorite;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "关注作者", Icon = Icons.登记 };
                mi.Click += OnFollow;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "分享", Icon = Icons.分享 };
                mi.Click += OnShare;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "投诉", Icon = Icons.警告 };
                mi.Click += OnComplaint;
                _menu.Items.Add(mi);
            }

            var btn = (Button)sender;
            _menu.DataContext = btn.DataContext;
            _menu.OpenContextMenu(btn);
        }

        void OnFavorite(object sender, Mi e)
        {
            AtArticle.Favorite((e.DataContext as Article).ID);
        }

        void OnFollow(object sender, Mi e)
        {
            
        }

        void OnShare(object sender, Mi e)
        {

        }

        void OnComplaint(object sender, Mi e)
        {

        }

        #region 文章行交互
        uint? _pointerID;
        Point _ptLast;
        Grid _gridLast;

        void OnItemPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = (Grid)sender;
            if (grid.CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _pointerID = e.Pointer.PointerId;
                _ptLast = e.GetCurrentPoint(null).Position;
                _gridLast = grid;
                ((Rectangle)grid.Children[grid.Children.Count - 1]).Fill = AtRes.暗遮罩;
            }
        }

        void OnItemPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_pointerID != e.Pointer.PointerId || sender != _gridLast)
                return;

            // 允许有短距离移动
            e.Handled = true;
            Point cur = e.GetCurrentPoint(null).Position;
            if (Math.Abs(cur.X - _ptLast.X) > 4 || Math.Abs(cur.Y - _ptLast.Y) > 4)
            {
                _gridLast.ReleasePointerCapture(e.Pointer);
                ((Rectangle)_gridLast.Children[_gridLast.Children.Count - 1]).Fill = null;
                _pointerID = null;
                _gridLast = null;
            }
        }

        void OnItemPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_pointerID != e.Pointer.PointerId || sender != _gridLast)
                return;

            AtArticle.Open(_gridLast.DataContext as Article);

            e.Handled = true;
            _gridLast.ReleasePointerCapture(e.Pointer);
            ((Rectangle)_gridLast.Children[_gridLast.Children.Count - 1]).Fill = null;
            _pointerID = null;
            _gridLast = null;
        }

        void OnScrollViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_gridLast != null)
                ((Rectangle)_gridLast.Children[_gridLast.Children.Count - 1]).Fill = null;
        }
        #endregion

    }
}
