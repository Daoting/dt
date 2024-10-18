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
        Dlg _dlg;
        Dlg _dlgWin;
        
        public TestDlgLeak()
        {
            InitializeComponent();
        }

        async void OnDispose(object sender, RoutedEventArgs e)
        {
            Dlg dlg = new Dlg();
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = Kit.ViewWidth - 200;
                dlg.Height = Kit.ViewHeight - 100;
            }
            dlg.LoadWin(new TestLvLeak());
            await dlg.ShowAsync();
            dlg.Destroy();
        }

        void OnWinDispose(object sender, RoutedEventArgs e)
        {
            if (_dlgWin == null)
            {
                _dlgWin = new Dlg { OwnWin = this };
                if (!Kit.IsPhoneUI)
                {
                    _dlgWin.Width = Kit.ViewWidth - 200;
                    _dlgWin.Height = Kit.ViewHeight - 100;
                }
                _dlgWin.LoadWin(new TestLvLeak());
            }
            _dlgWin.Show();
        }

        void OnUndispose(object sender, RoutedEventArgs e)
        {
            if (_dlg == null)
            {
                _dlg = new Dlg();
                if (!Kit.IsPhoneUI)
                {
                    _dlg.Width = Kit.ViewWidth - 200;
                    _dlg.Height = Kit.ViewHeight - 100;
                }
                _dlg.LoadWin(new TestLvLeak());
            }
            _dlg.Show();
        }
    }
}