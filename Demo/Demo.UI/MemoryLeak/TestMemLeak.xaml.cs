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
    public sealed partial class TestMemLeak : Win
    {
        public TestMemLeak()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("Lv内存泄漏", typeof(TestLvLeak), Icons.表格) { Desc = "测试Lv各种情况的内存泄漏" },
                new Nav("Fv内存泄漏", typeof(TestFvLeak), Icons.汉堡) { Desc = "测试Fv内存泄漏" },
                new Nav("Excel内存泄漏", typeof(TestExcelLeak), Icons.Excel) { Desc = "测试Excel内存泄漏" },
                new Nav("Tv内存泄漏", typeof(TestTvLeak), Icons.树形) { Desc = "测试Tv内存泄漏" },
                new Nav("嵌套窗口", typeof(TestWinLeak), Icons.Windows) { Desc = "测试嵌套窗口的内存泄漏" },
                new Nav("对话框", typeof(TestDlgLeak), Icons.公告) { Desc = "测试对话框容器的内存泄漏" },
            };
        }
    }
}
