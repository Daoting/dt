#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Demo.UI
{
    public partial class HtmlBoxDemo : Win
    {
        const string _initHtml = @"<p><span style=""font-size: 24px;"">初始内容</span></p>

<ol>
	<li><span style=""font-size: 14px;""><strong>粗体</strong></span></li>
	<li><s>删除线</s></li>
</ol>";
        
        public HtmlBoxDemo()
        {
            InitializeComponent();
            _html.SetHtml(_initHtml);
        }

        void OnLoad1(object sender, RoutedEventArgs e)
        {
            _html.SetHtml("<p>内容1</p>");
        }

        void OnLoad2(object sender, RoutedEventArgs e)
        {
            _html.SetHtml("<p>内容2</p>");
        }

        async void OnGet(object sender, RoutedEventArgs e)
        {
            var str = await _html.GetHtml();
            Kit.Msg("Html内容：\r\n" + str);
        }

        protected override void OnClosed()
        {
            _html.Close();
        }
    }
}