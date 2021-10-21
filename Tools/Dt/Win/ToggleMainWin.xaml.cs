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
    public partial class $safeitemname$ : Win
    {
        public $safeitemname$()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("窗口内容", typeof($safeitemname$), Icons.公告) { Desc = "描述" },
            };
        }

    }
}