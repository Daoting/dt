﻿#region 文件描述
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class WinLayout : Win
    {
        ParamsWin _nextWin;
        int _rnd = 0;

        public WinLayout()
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
                _nextWin = new ParamsWin();
                string rnd = new Random().Next(1000).ToString();
                _nextWin.Title = "窗口" + rnd;
                ((Button)sender).Content = "子窗口" + rnd;
            }
            _nextWin.Open();
        }

        void OnParamsWin(object sender, RoutedEventArgs e)
        {
            if (_rnd == 0)
            {
                _rnd = new Random().Next(1000);
                ((Button)sender).Content = "参数子窗口" + _rnd.ToString();
            }
            AtApp.OpenWin(typeof(ParamsWin), $"参数窗口{_rnd}", Icons.None, _rnd);
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
    }
}