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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class SingleViewWin : Win
    {
        ParamsWin _nextWin;
        int _rnd = 0;

        public SingleViewWin()
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
            Kit.OpenWin(typeof(ParamsWin), $"参数窗口{_rnd}", Icons.None, _rnd);
        }

        void OnClosing(object sender, AsyncCancelArgs e)
        {
            e.Cancel = (bool)_cbClosing.IsChecked;
            if (e.Cancel)
                Kit.Msg($"{Title} - 事件中设置禁止关闭");
        }

        void OnClosed(object sender, EventArgs e)
        {
            Kit.Msg($"{Title} - 关闭后事件");
        }

        void OnTabClick(object sender, RoutedEventArgs e)
        {
            var pos = ((RadioButton)sender).Tag.ToString();
            if (int.TryParse(pos, out int plc))
            {
                _tabs.TabStripPlacement = (ItemPlacement)plc;
            }
        }
    }
}