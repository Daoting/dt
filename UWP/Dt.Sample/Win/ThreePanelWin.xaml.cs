#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/5/22 13:18:36 创建
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

namespace Dt.Sample
{
    public partial class ThreePanelWin : Win
    {
        public ThreePanelWin()
        {
            InitializeComponent();

        }

        void OnGoto(object sender, RoutedEventArgs e)
        {
            NaviTo("主区,右区");
        }
    }
}