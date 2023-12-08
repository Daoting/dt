﻿#region 文件描述
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
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

        async void OnClosing(Dlg arg1, DlgClosingArgs arg2)
        {
            using (arg2.Wait())
            {
                await Task.Delay(100);
                arg2.Cancel = (bool)_cbClosing.IsChecked;
                if (arg2.Cancel)
                    Kit.Msg("事件中设置禁止关闭");
            }
        }

        void OnClosed(Dlg arg1, bool arg2)
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
