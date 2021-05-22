#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace $rootnamespace$
{
    public partial class $safeitemname$ : UserControl, INaviContent
    {
        public $safeitemname$()
        {
            InitializeComponent();
        }

        #region INaviContent
        INaviHost _host;

        void INaviContent.AddToHost(INaviHost p_host)
        {
            _host = p_host;
            _host.Title = "Tab标题";

            var menu = new Menu();
            menu.Items.Add(new Mi { ID = "搜索", Icon = Icons.搜索 });
            menu.Items.Add(new Mi { ID = "保存", Icon = Icons.保存 });
            _host.Menu = menu;
        }
        #endregion
    }
}