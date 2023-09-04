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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class ToggleWinCenter : Win
    {
        string _id;

        public ToggleWinCenter()
        {
            InitializeComponent();

            _id = new Random().Next(1000).ToString();
            _nav.Data = new Nl<Nav>
            {
                new Nav("内容为窗口", typeof(SingleViewWin), Icons.公告),
                new Nav("内容为嵌套窗口", typeof(ToggleWinCenter), Icons.田字格),
                new Nav("内容为Button", typeof(TabNaviItem), Icons.保存) { Callback = OnCreateBtn },
                new Nav("内容为空", default(Type), Icons.全选),
                new Nav("窗口标识：" + _id),
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

        Button _btn;
        void OnCreateBtn(object p_owner, Nav p_nav)
        {
            if (_btn == null)
                _btn = new Button { Content = "按钮", Margin = new Thickness(40) };
            LoadMain(_btn);
        }

        void OnCenterStr(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)sender;
            if (btn.Tag == null)
                btn.Tag = "字符串";
            LoadMain(btn.Tag);
        }

        protected override Task<bool> OnClosing()
        {
            return Kit.Confirm("确认允许关闭吗？窗口标识：" + _id);
        }
    }
}