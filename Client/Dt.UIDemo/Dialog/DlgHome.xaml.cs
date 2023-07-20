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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public sealed partial class DlgHome : Win
    {
        public DlgHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("自定义对话框", typeof(DlgDemo), Icons.日历) { Desc = "对话框常用属性" },
                new Nav("常用对话框", typeof(SysDlgDemo), Icons.Bug),
                new Nav("请稍等对话框", typeof(BusyDlgDemo), Icons.Wifi),
            };
        }
    }
}
