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
    [View("$maincls$Win")]
    public partial class $maincls$Win : Win
    {
        public $maincls$Win()
        {
            InitializeComponent();
        }

        public $maincls$List List => _list;

        public $maincls$Form Form => _form;

$relatedlistcs$
    }
}