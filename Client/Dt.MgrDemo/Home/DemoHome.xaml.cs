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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Mgr;
#endregion

namespace Dt.MgrDemo
{
    /// <summary>
    /// 覆盖默认主页
    /// </summary>
    [View(LobViews.主页)]
    public partial class DemoHome : Win
    {
        public DemoHome()
        {
            InitializeComponent();
        }
    }
}