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
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class TestWinLeak : Win
    {
        string _id;

        public TestWinLeak()
        {
            InitializeComponent();

            _id = new Random().Next(1000).ToString();
            _nav.Data = new Nl<Nav>
            {
                new Nav("递归嵌套窗口", typeof(TestWinLeak), Icons.田字格),
                new Nav("Lv内存泄漏", typeof(TestLvLeak), Icons.公告),
                new Nav("窗口标识：" + _id),
            };
        }
    }
}