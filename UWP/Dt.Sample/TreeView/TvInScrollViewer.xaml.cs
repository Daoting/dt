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
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class TvInScrollViewer : Win
    {
        public TvInScrollViewer()
        {
            InitializeComponent();
            _tv.Data = TvData.GetTbl();
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            var tbl = (Table)_tv.Data;
            int index = new Random().Next(0, tbl.Count);
            _tv.SelectedItem = tbl[index];
            Kit.Msg($"已选择 {tbl[index].Str("name")}");
        }

        void OnScrollTop(object sender, RoutedEventArgs e)
        {
            _tv.ScrollTop();
        }

        void OnScrollBottom(object sender, RoutedEventArgs e)
        {
            _tv.ScrollBottom();
        }

        void OnTopMax(object sender, RoutedEventArgs e)
        {
            _top.Height = _sv.ViewportHeight + 100;
        }

        void OnTop(object sender, RoutedEventArgs e)
        {
            _top.Height = _sv.ViewportHeight / 2;
        }

        void OnTopZero(object sender, RoutedEventArgs e)
        {
            _top.Height = 0;
        }

        void OnBottomMax(object sender, RoutedEventArgs e)
        {
            _bottom.Height = _sv.ViewportHeight + 100;
        }

        void OnBottom(object sender, RoutedEventArgs e)
        {
            _bottom.Height = _sv.ViewportHeight / 2;
        }

        void OnBottomZero(object sender, RoutedEventArgs e)
        {
            _bottom.Height = 0;
        }
    }
}