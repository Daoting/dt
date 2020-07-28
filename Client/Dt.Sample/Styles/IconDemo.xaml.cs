#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class IconDemo : Win
    {
        const int ItemWidth = 70;
        const int ItemHeight = 100;
        List<StackPanel> _icons = new List<StackPanel>();

        public IconDemo()
        {
            InitializeComponent();
            _container.Loaded += OnLoaded;
        }

        /// <summary>
        /// 加载后生成图标及排列图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            _container.Loaded -= OnLoaded;
            GenIcons();
            LoadIcons();
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// 页面大小改变时，改变图标排列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width == e.NewSize.Width)
                return;

            LoadIcons();
        }

        /// <summary>
        /// 生成列并排列图标
        /// </summary>
        void LoadIcons()
        {
            GenColmns();
            ArrangeIcons();
        }

        /// <summary>
        /// 排列图标
        /// </summary>
        void ArrangeIcons()
        {
            if (_container.ColumnDefinitions.Count <= 0)
                return;

            _container.Children.Clear();
            _container.RowDefinitions.Clear();
            int row = -1;
            for (int i = 0; i < _icons.Count; i++)
            {
                int col = i % _container.ColumnDefinitions.Count;
                if (col == 0)
                {
                    _container.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(ItemHeight) });
                    row += 1;
                }
                _icons[i].SetValue(Grid.ColumnProperty, col);
                _icons[i].SetValue(Grid.RowProperty, row);
                _container.Children.Add(_icons[i]);
            }
        }

        /// <summary>
        /// 生成图标展示页面元素
        /// </summary>
        void GenIcons()
        {
            string[] iconNames = Enum.GetNames(typeof(Icons));
            foreach (string item in iconNames)
            {
                string tmpIcon = AtRes.GetIconChar((Icons)Enum.Parse(typeof(Icons), item));
                if (string.IsNullOrEmpty(tmpIcon))
                    continue;

                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                TextBlock tName = new TextBlock() { Text = item, FontSize = 12 };
                TextBlock tIcon = new TextBlock() { Text = tmpIcon, Style = AtRes.字符, FontSize = 40 };
                TextBlock tCode = new TextBlock() { Text = Convert.ToString((Int32)tmpIcon.ToCharArray()[0], 16), FontSize = 12 };
                stackPanel.Children.Add(tName);
                stackPanel.Children.Add(tIcon);
                stackPanel.Children.Add(tCode);
                _icons.Add(stackPanel);
            }
        }

        /// <summary>
        /// 生成容器列
        /// </summary>
        void GenColmns()
        {
            double width = ActualWidth - 20;
            if (width <= 0)
                return;

            _container.ColumnDefinitions.Clear();
            int colCount = (int)(width / ItemWidth);
            double colWidth = width / colCount;
            for (int i = 0; i < colCount; i++)
            {
                _container.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(colWidth) });
            }
        }
    }
}
