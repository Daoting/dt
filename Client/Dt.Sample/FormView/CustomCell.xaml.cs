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
    public partial class CustomCell : Win
    {
        int _cnt = 0;

        public CustomCell()
        {
            InitializeComponent();
            _fv.CellClick += OnCellClick;
        }

        void OnCellClick(object sender, FvCell e)
        {
            _ob.Data = e;
        }

        void OnToggleContent(object sender, RoutedEventArgs e)
        {
            ((CFree)_fv.Items[0]).Content = new TextBox { PlaceholderText = (++_cnt).ToString() };
        }

        void OnToggleBar(object sender, RoutedEventArgs e)
        {
            if (_bar.Content is Button)
                _bar.Title = $"标题{++_cnt}";
            else
                _bar.Content = new Button { Content = $"按钮{++_cnt}" };
        }
    }
}