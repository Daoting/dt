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
            Host.NaviTo(new TabNav2 { Index = Index + 1 });
        }

        void OnBackPage(object sender, RoutedEventArgs e)
        {
            Host.GoBack();
        }

        #region INaviContent
        public INaviHost Host { get; set; }

        Menu _menu;
        public Menu HostMenu
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new Menu();
                    _menu.Items.Add(new Mi { ID = "保存", Icon = Icons.保存, ShowInPhone = VisibleInPhone.Icon });
                    _menu.Items.Add(new Mi { ID = "搜索", Icon = Icons.搜索, ShowInPhone = VisibleInPhone.Icon });
                }
                return _menu;
            }
        }

        public string HostTitle
        {
            get { return $"第{Index}页"; }
        }
        #endregion
    }
}
