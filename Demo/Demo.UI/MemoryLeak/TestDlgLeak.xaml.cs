#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/5/22 8:53:08 创建
******************************************************************************/
#endregion

#region 引用命名
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

namespace Demo.UI
{
    public partial class TestDlgLeak : Win
    {
        TestLvDlg _dlg;
        TestLvDlg _dlgWin;
        
        public TestDlgLeak()
        {
            InitializeComponent();
        }

        async void OnDispose(object sender, RoutedEventArgs e)
        {
            var dlg = new TestLvDlg();
            await dlg.ShowAsync();
            dlg.Destroy();
        }

        void OnWinDispose(object sender, RoutedEventArgs e)
        {
            if (_dlgWin == null)
                _dlgWin = new TestLvDlg { OwnWin = this };
            _dlgWin.Show();
        }

        void OnUndispose(object sender, RoutedEventArgs e)
        {
            if (_dlg == null)
                _dlg = new TestLvDlg();
            _dlg.Show();
        }
    }
}