#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class Dlg1 : Dlg
    {
        public Dlg1()
        {
            InitializeComponent();
            Closing += OnClosing;
            Closed += OnClosed;
        }

        public string Result { get { return _tbResult.Text; } }

        void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        async void OnClosing(object sender, DlgClosingEventArgs e)
        {
            using (e.Wait())
            {
                await Task.Delay(100);
                e.Cancel = (bool)_cbClosing.IsChecked;
                if (e.Cancel)
                    Kit.Msg("事件中设置禁止关闭");
            }
        }

        void OnClosed(object sender, bool e)
        {
            Kit.Msg("关闭后事件");
        }

        void OnNewDlg(object sender, RoutedEventArgs e)
        {
            var dlg = new Dlg1();
            dlg.Show();
        }
    }
}
