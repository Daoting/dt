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

namespace Demo.UI
{
    public sealed partial class StyleHome : Win
    {
        public StyleHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("系统按钮", typeof(SysButtonDemo), Icons.词典),
                new Nav("输入控件", typeof(SysInputDemo), Icons.分组),
                new Nav("日历时间", typeof(SysDatePickerDemo), Icons.图片) ,
                new Nav("颜色选择", typeof(SysColorDemo), Icons.全选),
                new Nav("进度条", typeof(ProgressDemo), Icons.Bug),
                new Nav("Lottie动画", typeof(LottieDemo), Icons.小图标),
            };
        }
    }
}
