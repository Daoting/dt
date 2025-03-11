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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class CustomCell : Win
    {
        int _cnt = 0;

        public CustomCell()
        {
            InitializeComponent();
            _fv.CellClick += OnCellClick;
        }

        void OnCellClick(object e)
        {
            _ob.Data = e;
        }

        void OnToggleContent(object sender, RoutedEventArgs e)
        {
            ((CFree)_fv.Items[0]).Content = new TextBox { PlaceholderText = (++_cnt).ToString() };
        }

        void OnToggleBar(object sender, RoutedEventArgs e)
        {
            int tp = _cnt % 4;
            if (tp == 0)
            {
                _bar.Title = null;
                _bar.Content = null;
            }
            else if (tp == 1)
            {
                _bar.Title = $"标题{_cnt}";
                _bar.Content = null;
            }
            else if (tp == 2)
            {
                _bar.Title = null;
                _bar.Content = new Button { Content = $"按钮{_cnt}" };
            }
            else
            {
                _bar.Title = $"标题{_cnt}";
                _bar.Content = new Button { Content = $"按钮{_cnt}" };
            }
            _cnt++;
        }
    }
}