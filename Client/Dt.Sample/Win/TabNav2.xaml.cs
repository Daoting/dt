#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class TabNav2 : UserControl, INaviContent
    {
        public TabNav2()
        {
            InitializeComponent();
        }

        public int Index { get; set; } = 1;

        void OnNextPage(object sender, RoutedEventArgs e)
        {
            _host.NaviTo(new TabNav2 { Index = Index + 1 });
        }

        void OnBackPage(object sender, RoutedEventArgs e)
        {
            _host.GoBack();
        }

        #region INaviContent
        INaviHost _host;

        void INaviContent.AddToHost(INaviHost p_host)
        {
            _host = p_host;
            _host.Title = $"第{Index}页";

            var menu = new Menu();
            menu.Items.Add(new Mi { ID = "保存", Icon = Icons.保存, ShowInPhone = VisibleInPhone.Icon });
            menu.Items.Add(new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon });
            _host.Menu = menu;
        }
        #endregion
    }
}
