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
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class ToggleWinCenter : Win
    {
        public ToggleWinCenter()
        {
            InitializeComponent();

            _lv.Data = new List<CenterInfo>
            {
                new CenterInfo(Icons.公告, "内容为窗口", typeof(SingleViewWin), null),
                new CenterInfo(Icons.田字格, "内容为嵌套窗口", typeof(ToggleWinCenter), null),
                new CenterInfo(Icons.饼图, "内容为UserControl", typeof(TabNav1), null),
                new CenterInfo(Icons.详细, "内容为空", null, null),
            };

            LoadCenter(new Button { Content = "abc" });
        }

        void OnChanged(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)sender;
            LoadCenter(btn.GetTagClsObj());
        }

        void OnCenterNull(object sender, RoutedEventArgs e)
        {
            LoadCenter(null);
        }

        void OnCenterBtn(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)sender;
            if (btn.Tag == null)
                btn.Tag = new Button { Content = "按钮", Margin = new Thickness(40) };
            LoadCenter(btn.Tag);
        }

        void OnCenterStr(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)sender;
            if (btn.Tag == null)
                btn.Tag = "字符串";
            LoadCenter(btn.Tag);
        }
    }
}