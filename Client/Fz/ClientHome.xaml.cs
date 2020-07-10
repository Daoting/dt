﻿#region 文件描述
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

namespace Dt.Fz
{
    /// <summary>
    /// 首页
    /// </summary>
    [View("福祉主页")]
    public partial class ClientHome : Win
    {
        public ClientHome()
        {
            InitializeComponent();
            _post.Win = this;
        }

        void OnPost(object sender, RoutedEventArgs e)
        {
            AtApp.OpenView("文章管理");
        }
    }
}