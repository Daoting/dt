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

            _nav.Data = new Nl<Nav>
            {
                new Nav("内容为窗口", typeof(SingleViewWin), Icons.公告),
                new Nav("内容为嵌套窗口", typeof(ToggleWinCenter), Icons.田字格),
                new Nav("内容为UserControl", typeof(TabNav1), Icons.饼图),
                new Nav("内容为空", default(Type), Icons.全选),
            };

            LoadMain(new Button { Content = "abc" });
        }

        void OnChanged(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)sender;
            if (btn.Tag != null)
            {
                LoadMain(btn.Tag);
            }
            else
            {
                Type tp = Type.GetType(btn.GetTagCls(), false);
                if (tp != null)
                {
                    var obj = Activator.CreateInstance(tp);
                    btn.Tag = obj;
                    LoadMain(obj);
                }
            }
        }

        void OnCenterNull(object sender, RoutedEventArgs e)
        {
            LoadMain(null);
        }

        void OnCenterBtn(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)sender;
            if (btn.Tag == null)
                btn.Tag = new Button { Content = "按钮", Margin = new Thickness(40) };
            LoadMain(btn.Tag);
        }

        void OnCenterStr(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)sender;
            if (btn.Tag == null)
                btn.Tag = "字符串";
            LoadMain(btn.Tag);
        }
    }
}