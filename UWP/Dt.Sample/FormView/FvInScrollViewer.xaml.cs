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
    public partial class FvInScrollViewer : Win
    {
        public FvInScrollViewer()
        {
            InitializeComponent();

            Table tbl = new Table
            {
                { "txt1" },
                { "txt2" },
                { "txt3" },
                { "txt4" },
                { "txt5" },
                { "txt6" },
            };
            _fv.Data = tbl.AddRow();
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            int index = new Random().Next(0, _fv.Items.Count);
            _fv.ScrollInto(index);
            Kit.Msg($"滚动到第 {index + 1} 个单元格");
        }

        void OnScrollTop(object sender, RoutedEventArgs e)
        {
            _fv.ScrollTop();
        }

        void OnScrollBottom(object sender, RoutedEventArgs e)
        {
            _fv.ScrollBottom();
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