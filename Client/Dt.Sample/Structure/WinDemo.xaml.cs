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
    public partial class WinDemo : Win
    {
        WinDemo _nextWin;

        public WinDemo()
        {
            InitializeComponent();
            Closing += OnClosing;
            Closed += OnClosed;
        }

        void OnToggleIcon(object sender, RoutedEventArgs e)
        {
            int index = (int)Icon + 1;
            if (index >= 200)
                index = 0;
            Icon = (Icons)index;
        }

        void OnNewWin(object sender, RoutedEventArgs e)
        {
            if (_nextWin == null)
            {
                _nextWin = new WinDemo();
                string rnd = new Random().Next(1000).ToString();
                _nextWin.Title = "窗口" + rnd;
                ((Button)sender).Content = "子窗口" + rnd;
            }
            _nextWin.Open();
        }

        void OnClosing(object sender, AsyncCancelEventArgs e)
        {
            e.Cancel = (bool)_cbClosing.IsChecked;
            if (e.Cancel)
                AtKit.Msg($"{Title} - 事件中设置禁止关闭");
        }

        void OnClosed(object sender, EventArgs e)
        {
            AtKit.Msg($"{Title} - 关闭后事件");
        }

        void OnNavi(object sender, RoutedEventArgs e)
        {
            NaviTo((string)((Button)sender).Content);
        }
    }
}