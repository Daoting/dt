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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PlanTest1 : PageWin
    {

        public PlanTest1()
        {
            InitializeComponent();

        }

        void OnNext(object sender, RoutedEventArgs e)
        {
            AtApp.OpenWin(typeof(PlanTest2));
        }
    }
}