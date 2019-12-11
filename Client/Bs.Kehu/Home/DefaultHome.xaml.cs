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
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 首页
    /// </summary>
    [View(AtUI.HomeView)]
    public partial class DefaultHome : Win
    {
        public DefaultHome()
        {
            InitializeComponent();
        }

        void OnInit(object sender, RoutedEventArgs e)
        {
            AtLocal.DeleteCookie("ShowGuide");
        }
    }
}