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
    public partial class PlanTest3 : PageWin
    {

        public PlanTest3()
        {
            InitializeComponent();

        }

        void OnNo(object sender, RoutedEventArgs e)
        {
            var frame = AtApp.RootFrame;
            if (frame != null)
            {
                frame.GoBack();
                frame.GoBack();
                frame.GoBack();
            }
        }

        void OnYes(object sender, RoutedEventArgs e)
        {
            AtApp.OpenWin(typeof(PlanTest4));
        }
    }
}