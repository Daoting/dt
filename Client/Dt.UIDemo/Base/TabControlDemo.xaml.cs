#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TabControlDemo : Win
    {
        public TabControlDemo()
        {
            InitializeComponent();
        }

        void OnAddTabItem(object sender, RoutedEventArgs e)
        {
            TabItem item = new TabItem();
            item.Title = "新增加标签";
            _tab.Items.Add(item);
        }

        void OnRemoveTabItem(object sender, RoutedEventArgs e)
        {
            _tab.Items.RemoveAt(_tab.Items.Count - 1);
        }

        void OnReplaceItem(object sender, RoutedEventArgs e)
        {
            string msg = "新替换" + new Random().Next(1000);
            _tab.ReplaceItem(_tab.SelectedItem, new TabItem
            {
                Title = msg,
                Content = new TextBlock
                { 
                    Text = msg,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            });
        }
    }
}